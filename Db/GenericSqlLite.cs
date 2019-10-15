using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UtilidadesWFA.Db
{
    public class GenericSqlLite
    {
        public readonly string _dataBase;
        public GenericSqlLite(string pDataBase)
        {
            _dataBase = pDataBase;
        }
        private string ParametroBanco = ":";
        public List<T> GetSelect<T>(string pComando, List<DBParametros> pParametros = null)
        {
            try
            {
                DataTable dtConsulta = new SqlLite(_dataBase).ExecuteDataTable(pComando, pParametros);
                return new GenericCommand().GetSelect<T>(dtConsulta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T Salvar<T>(T pObjeto)
        {
            try
            {
                GenericCommand.Generic<T> Ger = new GenericCommand.Generic<T>(pObjeto, "[" + typeof(T).Name + "]", ParametroBanco, "; SELECT 1;");
                if (Ger.CamposPrimaryKeys.Count > 0 && !Ger.Update)
                    Ger.CamposPrimaryKeys[0].SetValue(pObjeto, Convert.ToInt32(new SqlLite(_dataBase).ExecuteScalar(Ger.Query, Ger.Parametros)));
                else
                    new SqlLite(_dataBase).ExecuteNonQuery(Ger.Query, Ger.Parametros);
                return pObjeto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Deletar<T>(T pObjeto)
        {
            try
            {
                GenericCommand.Generic<T> Ger = new GenericCommand.Generic<T>(pObjeto, "[" + typeof(T).Name + "]", ParametroBanco);
                new SqlLite(_dataBase).ExecuteNonQuery(Ger.Query, Ger.Parametros);
                return "Deletado com sucesso";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<T> GetItens<T>(List<Parametro> pParametros = null, int pTop = 0, string pOrderBy = "")
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT * ");
            sbQuery.AppendLine("FROM " + typeof(T).Name);
            sbQuery.AppendLine("WHERE 1=1");

            List<DBParametros> pmts = null;
            if (pParametros != null)
            {
                pmts = new List<DBParametros>();
                pParametros.ForEach((parametro) =>
                {
                    sbQuery.AppendLine("AND (" + parametro.GetCondicao(ParametroBanco, TpBanco.Sqlite) + ")");
                    pmts.AddRange(parametro.GetParametros());
                });
            }

            if (!string.IsNullOrWhiteSpace(pOrderBy))
                sbQuery.AppendLine("ORDER BY " + pOrderBy);
            if (pTop > 0)
                sbQuery.AppendLine("LIMIT " + pTop.ToString());

            return GetSelect<T>(sbQuery.ToString(), pmts);
        }
        public T GetItem<T>(List<Parametro> pParametros)
        {
            try
            {
                List<T> lista = GetItens<T>(pParametros, 0, null);
                return lista.Count > 0 ? lista[0] : Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}