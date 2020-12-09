using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UtilidadesWFA.Db
{
    public class GenericMySql
    {
        private string ParametroBanco = "@";
        private MySql Conexao;
        public GenericMySql(string pServidor, string pUsuario = "root", string pSenha = null)
        {
            Conexao = new MySql(pServidor, pUsuario, pSenha);
        }
        public List<T> GetSelect<T>(string pComando, List<DBParametros> pParametros)
        {
            try
            {
                DataTable dtConsulta = Conexao.ExecuteDataTable(pComando.Replace("[", "").Replace("]", ""), pParametros);
                return new GenericCommand().GetSelect<T>(dtConsulta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T Salvar<T>(T pObjeto, string pDataBase)
        {
            try
            {
                GenericCommand.Generic<T> Ger = new GenericCommand.Generic<T>(pObjeto, pDataBase + "." + typeof(T).Name + " ", ParametroBanco, "; SELECT LAST_INSERT_ID();");
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
        public string Deletar<T>(T pObjeto, string pDataBase)
        {
            try
            {
                GenericCommand.Generic<T> Ger = new GenericCommand.Generic<T>(pObjeto, pDataBase + "." + typeof(T).Name, ParametroBanco);
                Conexao.ExecuteNonQuery(Ger.Query, Ger.Parametros);
                return "Deletado com sucesso";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<T> GetItens<T>(List<Parametro> pParametros, string pDataBase, int pTop = 0, string pOrderBy = "")
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT * ");
            sbQuery.AppendLine("FROM " + pDataBase + "." + typeof(T).Name);
            sbQuery.AppendLine("WHERE 1=1");

            List<DBParametros> pmts = null;
            if (pParametros != null)
            {
                pmts = new List<DBParametros>();
                pParametros.ForEach((parametro) =>
                {
                    sbQuery.AppendLine("AND (" + parametro.GetCondicao(ParametroBanco, TpBanco.MySql) + ")");
                    pmts.AddRange(parametro.GetParametros());
                });
            }

            if (!string.IsNullOrWhiteSpace(pOrderBy))
                sbQuery.AppendLine("ORDER BY " + pOrderBy);
            if (pTop > 0)
                sbQuery.AppendLine("LIMIT " + pTop.ToString());

            return GetSelect<T>(sbQuery.ToString(), pmts);
        }
        public T GetItem<T>(List<Parametro> pParametros, string pDataBase)
        {
            try
            {
                List<T> lista = GetItens<T>(pParametros, pDataBase);
                return (lista.Count > 0) ? lista[0] : Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
