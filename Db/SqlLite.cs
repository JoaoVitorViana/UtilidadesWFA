using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace UtilidadesWFA.Db
{
    public class SqlLite
    {
        public readonly string connectionString;
        public SqlLite(string pBanco)
        {
            connectionString = $"Data Source={pBanco}; Version=3;";
        }
        public DataTable ExecuteDataTable(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var sqliteConnection = new SQLiteConnection(connectionString))
                {
                    if ((sqliteConnection.State == ConnectionState.Broken || sqliteConnection.State == ConnectionState.Closed) && (sqliteConnection.State != ConnectionState.Open))
                    {
                        sqliteConnection.Open();
                    }

                    using (var cmd = sqliteConnection.CreateCommand())
                    {
                        cmd.CommandText = pComandoSQL;
                        if (pParametros != null)
                            pParametros.ForEach(item => cmd.Parameters.AddWithValue(item.Name, item.Value == null ? DBNull.Value : item.Value));
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public object ExecuteScalar(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            object retorno;
            try
            {
                using (var sqliteConnection = new SQLiteConnection(connectionString))
                {
                    if ((sqliteConnection.State == ConnectionState.Broken || sqliteConnection.State == ConnectionState.Closed) && (sqliteConnection.State != ConnectionState.Open))
                    {
                        sqliteConnection.Open();
                    }
                    using (var cmd = sqliteConnection.CreateCommand())
                    {
                        cmd.CommandText = pComandoSQL;
                        if (pParametros != null)
                            pParametros.ForEach(item => cmd.Parameters.AddWithValue(item.Name, item.Value == null ? DBNull.Value : item.Value));
                        retorno = cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        public int ExecuteNonQuery(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            int retorno;
            try
            {
                using (var sqliteConnection = new SQLiteConnection(connectionString))
                {
                    if ((sqliteConnection.State == ConnectionState.Broken || sqliteConnection.State == ConnectionState.Closed) && (sqliteConnection.State != ConnectionState.Open))
                    {
                        sqliteConnection.Open();
                    }
                    using (var cmd = sqliteConnection.CreateCommand())
                    {
                        cmd.CommandText = pComandoSQL;
                        if (pParametros != null)
                            pParametros.ForEach(item => cmd.Parameters.AddWithValue(item.Name, item.Value == null ? DBNull.Value : item.Value));
                        retorno = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }
    }
}
