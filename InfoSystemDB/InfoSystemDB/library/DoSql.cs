using System;
using System.Data.SqlClient;
using System.Windows;

namespace InfoSystemDB
{
    public class DoSql
    {
        private string connectionString = "Data Source=WIN-FSJH44K4B7V;Initial Catalog=VsInsideDB;Integrated Security=true;";
        private string sqlExpression;
        private SqlParameter[] parameters;
        
        public DoSql(string sql, SqlParameter[] parametrs)
        {
            sqlExpression = sql;
            parameters = parametrs;
        }
        
        public void ToExecuteQuery()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(sqlExpression, connection);
                connection.Open();

                foreach (SqlParameter i in parameters)
                {
                    cmd.Parameters.Add(i);
                }
                
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public SqlDataReader ToReadQuery()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();
                connection.Open();

                cmd.Connection = connection;
                cmd.CommandText = sqlExpression;
                
                foreach (SqlParameter i in parameters)
                {
                    cmd.Parameters.Add(i);
                }

                SqlDataReader reader = cmd.ExecuteReader();

                return reader;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new SqlCommand().ExecuteReader();
            }
        }
    }
}