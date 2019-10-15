using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilidadesWFA.Db
{
    public class GenericCommand
    {
        public List<T> GetSelect<T>(DataTable pDtConsulta)
        {
            try
            {
                List<T> lista = new List<T>();

                foreach (DataRow dr in pDtConsulta.Rows)
                {
                    T objeto = Activator.CreateInstance<T>();
                    foreach (DataColumn columns in dr.Table.Columns)
                    {
                        PropertyInfo campo = typeof(T).GetProperties().Where(c => c.Name == columns.ColumnName).FirstOrDefault();
                        if (campo != null && dr[columns] != DBNull.Value)
                        {
                            Type propertyType = Nullable.GetUnderlyingType(campo.PropertyType) != null ? Nullable.GetUnderlyingType(campo.PropertyType) : campo.PropertyType;
                            campo.SetValue(objeto, Convert.ChangeType(dr[columns], propertyType));
                        }
                    }
                    lista.Add(objeto);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public class Generic<T>
        {
            public List<PropertyInfo> Campos { get; }
            public List<PropertyInfo> CamposPrimaryKeys { get; }
            public List<DBParametros> Parametros { get; }
            public bool Update { get; }
            public string Query { get; }
            /// <summary>
            /// Generic Insert Or Update
            /// </summary>
            /// <param name="pObjeto"></param>
            /// <param name="pTabela"></param>
            /// <param name="pParametroBanco"></param>
            /// <param name="pGetId"></param>
            public Generic(T pObjeto, string pTabela, string pParametroBanco, string pGetId)
            {
                this.Campos = typeof(T).GetProperties().Where(x => x.GetCustomAttributes<NotMappedAttribute>(true).Count() == 0).ToList();
                this.CamposPrimaryKeys = Campos.Where(x => x.GetCustomAttributes<KeyAttribute>(true).Count() > 0).ToList();
                this.Update = (CamposPrimaryKeys.Where(x => Convert.ToInt64(x.GetValue(pObjeto)) > 0).Count() > 0);

                StringBuilder sbQuery = new StringBuilder();
                sbQuery.Append(Update ? "UPDATE " : "INSERT INTO ");
                sbQuery.Append(pTabela + " ");
                sbQuery.Append(Update ? "SET " : "(");

                StringBuilder sbCampos = new StringBuilder();
                this.Parametros = new List<DBParametros>();
                int i = 0;
                foreach (PropertyInfo campo in Campos)
                {
                    if (CamposPrimaryKeys.Contains(campo))
                        continue;

                    if ((campo.GetValue(pObjeto) == null || campo.GetValue(pObjeto) == DBNull.Value) && !Update)
                        continue;

                    if ((campo.GetValue(pObjeto) == null || campo.GetValue(pObjeto) == DBNull.Value) && Update)
                        sbQuery.AppendLine(((i > 0) ? "," : string.Empty) + campo.Name + " = NULL ");
                    else
                    {
                        sbQuery.AppendLine(((i > 0) ? "," : string.Empty) + ((Update) ? campo.Name + " = " + pParametroBanco + campo.Name : campo.Name));
                        if (!Update)
                            sbCampos.AppendLine(((i > 0) ? "," : string.Empty) + pParametroBanco + campo.Name);

                        Parametros.Add(new DBParametros { Name = campo.Name, Value = campo.GetValue(pObjeto) });
                    }
                    i++;
                }

                if (Update)
                {
                    foreach (PropertyInfo chave in CamposPrimaryKeys)
                    {
                        sbCampos.AppendLine("AND " + chave.Name + " = " + pParametroBanco + chave.Name);
                        Parametros.Add(new DBParametros { Name = chave.Name, Value = chave.GetValue(pObjeto) });
                    }

                    sbQuery.Append(" WHERE 1=1 " + sbCampos.ToString());
                }
                else
                {
                    sbQuery.Append(") VALUES (" + sbCampos.ToString() + ")");
                    if (CamposPrimaryKeys.Count > 0)
                        sbQuery.Append(pGetId);
                }

                this.Query = sbQuery.ToString();
            }
            /// <summary>
            /// Generic Delete
            /// </summary>
            /// <param name="pObjeto"></param>
            /// <param name="pTabela"></param>
            /// <param name="pParametroBanco"></param>
            public Generic(T pObjeto, string pTabela, string pParametroBanco)
            {
                StringBuilder sbQuery = new StringBuilder();
                sbQuery.Append("DELETE FROM ");
                sbQuery.Append(pTabela + " ");
                sbQuery.Append("WHERE 1=1 ");

                this.CamposPrimaryKeys = typeof(T).GetProperties().Where(x => x.GetCustomAttributes<KeyAttribute>(true).Count() > 0).ToList();
                if (CamposPrimaryKeys.Count == 0)
                    throw new Exception("Chave primária não informada");

                this.Parametros = new List<DBParametros>();
                CamposPrimaryKeys.ForEach((campos) =>
                {
                    if (campos.GetValue(pObjeto) != null && campos.GetValue(pObjeto) != DBNull.Value)
                    {
                        sbQuery.Append("AND " + campos.Name + " = " + pParametroBanco + campos.Name + " ");
                        Parametros.Add(new DBParametros { Value = campos.GetValue(pObjeto), Name = campos.Name });
                    }
                });

                if (Parametros.Count == 0)
                    throw new Exception("Chave primária não informada");

                this.Query = sbQuery.ToString();
                this.Update = false;
            }
        }
    }
}