using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UtilidadesWFA.Db
{
    public class GenericSqlServer
    {
        private SqlServer Conexao;
        private string ParametroBanco = "@";

        public GenericSqlServer(string pServidor, string pDataBase = "master")
        {
            Conexao = new SqlServer(pServidor, pDataBase);
        }

        public GenericSqlServer(string pServidor, string pDataBase, string pUsuario, string pSenha)
        {
            Conexao = new SqlServer(pServidor, pDataBase, pUsuario, pSenha);
        }

        public List<T> GetSelect<T>(string pComando, List<DBParametros> pParametros)
        {
            try
            {
                DataTable dtConsulta = Conexao.ExecuteDataTable(pComando, pParametros);
                return new GenericCommand().GetSelect<T>(dtConsulta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Salvar<T>(T pObjeto, string pSchema = "dbo")
        {
            try
            {
                GenericCommand.Generic<T> Ger = new GenericCommand.Generic<T>(pObjeto, $"[{pSchema}].[{typeof(T).Name}]", ParametroBanco, "SCOPE_IDENTITY()");
                if (Ger.CamposPrimaryKeys.Count > 0 && !Ger.Update)
                    Ger.CamposPrimaryKeys[0].SetValue(pObjeto, Convert.ToInt32(Conexao.ExecuteScalar(Ger.Query, Ger.Parametros)));
                else
                    Conexao.ExecuteNonQuery(Ger.Query, Ger.Parametros);
                return pObjeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Deletar<T>(T pObjeto, string pSchema = "dbo")
        {
            try
            {
                GenericCommand.Generic<T> Ger = new GenericCommand.Generic<T>(pObjeto, $"[{pSchema}].[{typeof(T).Name}]", ParametroBanco);
                Conexao.ExecuteNonQuery(Ger.Query, Ger.Parametros);
                return "Deletado com sucesso.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> GetItens<T>(List<Parametro> pParametros, int pTop = 0, string pOrderBy = "", string pSchema = "dbo")
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine($"SELECT {(pTop > 0 ? $"TOP {pTop.ToString()} " : "")}*");
            sbQuery.AppendLine($"FROM [{pSchema}].[{typeof(T).Name}]");
            sbQuery.AppendLine("WHERE 1=1");

            List<DBParametros> pmts = null;
            if (pParametros != null)
            {
                pmts = new List<DBParametros>();
                pParametros.ForEach((parametro) =>
                {
                    sbQuery.AppendLine($"AND ({parametro.GetCondicao(ParametroBanco, TpBanco.SqlServer)})");
                    pmts.AddRange(parametro.GetParametros());
                });
            }

            if (!string.IsNullOrWhiteSpace(pOrderBy))
                sbQuery.AppendLine($"ORDER BY {pOrderBy}");

            return GetSelect<T>(sbQuery.ToString(), pmts);
        }

        public T GetItem<T>(List<Parametro> pParametros, string pSchema = "dbo")
        {
            try
            {
                List<T> lista = GetItens<T>(pParametros, 0, null, pSchema);
                return (lista.Count > 0) ? lista[0] : Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}