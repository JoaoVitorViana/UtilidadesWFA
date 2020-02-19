using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace UtilidadesWFA.Db
{
    public class SqlServer
    {
        protected SqlConnection Conn;
        protected SqlCommand Cmd;
        protected SqlTransaction Tra = null;
        /// <summary>
        /// Retorna Conexão Windows Authentication
        /// </summary>
        /// <param name="pServidor">Servidor</param>
        /// <param name="pDataBase">Data Base</param>
        /// <returns></returns>
        private SqlConnection RetornaConexao(string pServidor, string pDataBase)
        {
            SqlConnectionStringBuilder Connection = new SqlConnectionStringBuilder();
            Connection.DataSource = pServidor;
            Connection.IntegratedSecurity = true;
            Connection.InitialCatalog = pDataBase;

            SqlConnection oSqlConn = new SqlConnection();
            oSqlConn.ConnectionString = Connection.ConnectionString;
            return oSqlConn;
        }
        /// <summary>
        /// Retorna Conexão SQL Server Authentication
        /// </summary>
        /// <param name="pServidor">Servidor</param>
        /// <param name="pUsuario">Usuario</param>
        /// <param name="pSenha">Senha</param>
        /// <param name="pDataBase">Data Base</param>
        /// <returns></returns>
        private SqlConnection RetornaConexao(string pServidor, string pUsuario, string pSenha, string pDataBase)
        {
            SqlConnectionStringBuilder Connection = new SqlConnectionStringBuilder();
            Connection.Password = pSenha;
            Connection.UserID = pUsuario;
            Connection.PersistSecurityInfo = true;
            Connection.InitialCatalog = pDataBase;
            Connection.DataSource = pServidor;

            SqlConnection oSqlConn = new SqlConnection();
            oSqlConn.ConnectionString = Connection.ConnectionString;
            return oSqlConn;
        }
        /// <summary>
        /// Conexão com Windows Authentication
        /// </summary>
        /// <param name="pServidor">Servidor (default no Config)</param>
        /// <param name="pDataBase">Data Base (default Master)</param>
        public SqlServer(string pServidor, string pDataBase = "master")
        {
            try
            {
                Conn = new SqlConnection();
                Conn = RetornaConexao(pServidor, pDataBase);
                Cmd = new SqlCommand("", Conn);
                Cmd.CommandTimeout = 100000;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  Conexão com SQL Server Authentication
        /// </summary>
        /// <param name="pServidor">Servidor</param>
        /// <param name="pDataBase">Data Base</param>
        /// <param name="pUsuario">Usuario</param>
        /// <param name="pSenha">Senha</param>
        public SqlServer(string pServidor, string pDataBase, string pUsuario, string pSenha)
        {
            try
            {
                Conn = new SqlConnection();
                Conn = RetornaConexao(pServidor, pUsuario, pSenha, pDataBase);
                Cmd = new SqlCommand("", Conn);
                Cmd.CommandTimeout = 100000;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AbreConexao()
        {
            if (Conn.State != ConnectionState.Open)
                Conn.Open();
        }
        public void FechaConexao()
        {
            if (Conn.State == ConnectionState.Open)
                Conn.Close();
        }
        public object ExecuteScalar(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            object retorno;
            AbreConexao();
            try
            {
                Cmd.Connection = Conn;
                Cmd.Parameters.Clear();
                Cmd.CommandText = pComandoSQL;
                Cmd.CommandType = CommandType.Text;

                if (pParametros != null)
                {
                    foreach (DBParametros item in pParametros)
                    {
                        Cmd.Parameters.AddWithValue(item.Name, item.Value == null ? DBNull.Value : item.Value);
                    }
                }

                retorno = Cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FechaConexao();
            }

            return retorno;
        }
        public DataSet ExecuteDataSet(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            DataSet ds = new DataSet();
            AbreConexao();

            try
            {
                Cmd.Connection = Conn;
                Cmd.Parameters.Clear();
                Cmd.CommandText = pComandoSQL;
                Cmd.CommandType = CommandType.Text;

                if (pParametros != null)
                {
                    foreach (DBParametros item in pParametros)
                    {
                        Cmd.Parameters.AddWithValue(item.Name, item.Value == null ? DBNull.Value : item.Value);
                    }
                }

                SqlDataAdapter adp = new SqlDataAdapter(Cmd);
                adp.SelectCommand.CommandTimeout = 100000;
                adp.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FechaConexao();
            }

            return ds;
        }
        public DataTable ExecuteDataTable(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = ExecuteDataSet(pComandoSQL, pParametros).Tables[0];
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }
        public int ExecuteNonQuery(string pComandoSQL, List<DBParametros> pParametros = null)
        {
            int retorno;
            AbreConexao();
            try
            {
                Cmd.Connection = Conn;
                Cmd.Parameters.Clear();
                Cmd.CommandText = pComandoSQL;
                Cmd.CommandType = CommandType.Text;

                if (pParametros != null)
                {
                    foreach (DBParametros item in pParametros)
                    {
                        Cmd.Parameters.AddWithValue(item.Name, item.Value == null ? DBNull.Value : item.Value);
                    }
                }

                retorno = Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FechaConexao();
            }

            return retorno;
        }
        public void IniciarTransacao()
        {
            Tra = Conn.BeginTransaction();
            Cmd.Transaction = Tra;
        }
        public void ComitarTransacao()
        {
            Tra.Commit();
            Cmd.Transaction = null;
            Tra = null;
        }
        public void DesfazerTransacao()
        {
            Tra.Rollback();
            Cmd.Transaction = null;
            Tra = null;
        }
        public bool Bulk(string pTabelaDestino, DataTable pDtDados)
        {
            int qtdCopiada;
            int qtdPlanilha;
            try
            {
                AbreConexao();

                using (SqlBulkCopy bc = new SqlBulkCopy(Conn))
                {
                    bc.BulkCopyTimeout = 120 * 1000;
                    bc.DestinationTableName = pTabelaDestino;
                    bc.WriteToServer(pDtDados);

                    FieldInfo rowsCopiedField = typeof(SqlBulkCopy).GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                    qtdCopiada = (int)rowsCopiedField.GetValue(bc);
                    qtdPlanilha = pDtDados.Rows.Count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FechaConexao();
            }

            if (qtdCopiada != qtdPlanilha)
                return false;
            else
                return true;
        }
    }
}
