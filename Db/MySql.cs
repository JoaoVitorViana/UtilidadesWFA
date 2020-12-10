using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace UtilidadesWFA.Db
{
    public class MySql
    {
        protected MySqlConnection Conn;
        protected MySqlCommand Cmd;
        protected MySqlTransaction Tra = null;

        private MySqlConnection RetornaConexao(string pConexao, string pDataBase, string pUsuario, string pSenha)
        {
            MySqlConnectionStringBuilder Connection = new MySqlConnectionStringBuilder();
            Connection.Server = pConexao;
            Connection.UserID = pUsuario;
            Connection.Database = pDataBase;
            if (!string.IsNullOrWhiteSpace(pSenha))
                Connection.Password = pSenha;

            MySqlConnection oSqlConn = new MySqlConnection();
            oSqlConn.ConnectionString = Connection.ConnectionString;
            return oSqlConn;
        }

        public MySql(string pServer, string pDataBase, string pUsuario = "root", string pSenha = null)
        {
            try
            {
                Conn = new MySqlConnection();
                Conn = RetornaConexao(pServer, pDataBase, pUsuario, pSenha);
                Cmd = new MySqlCommand("", Conn);
                Cmd.CommandTimeout = 100000;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AbreConexao()
        {
            try
            {
                if (Conn.State != ConnectionState.Open)
                    Conn.Open();
            }
            catch { throw new Exception("Erro ao conectar no banco de dados"); }
        }

        public void FechaConexao()
        {
            try
            {
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
            }
            catch { throw new Exception("Erro ao conectar no banco de dados"); }
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

                MySqlDataAdapter adp = new MySqlDataAdapter(Cmd);
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
    }
}