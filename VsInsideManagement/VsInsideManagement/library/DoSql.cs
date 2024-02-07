using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace VsInsideManagement.library
{
    public class DoSql
    {
        private string connectionString = "";
        private string sqlExpression;
        private SqlParameter[] parameters;
        public DoSql(string sql, SqlParameter[] parametrs)
        {
            string filePath = "ServerName.txt";
            string content = File.ReadAllText(filePath);
            
            connectionString = $"Data Source={content};Initial Catalog=VsInsideDB;Integrated Security=true;";
            
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