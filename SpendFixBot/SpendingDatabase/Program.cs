using System;
using DT = System.Data;
using QC = System.Data.SqlClient;

namespace SpendingDatabase
{
    public class Program
    {
        private static readonly String CONNECTION_STR = @"Data Source=LAPTOP-DL0OAIPO\SQLEXPRESS;Initial Catalog=SpendingTracker;Integrated Security=True";

        static void Main(string[] args)
        {
        }

        static public void InsertUser(int User_id, string Name)
        {
            using (var connection = new QC.SqlConnection(CONNECTION_STR))
            {
                connection.Open();
                QC.SqlParameter parameter;
                using (var command = new QC.SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = $@"IF NOT EXISTS (SELECT user_name FROM users WHERE user_id = '{User_id}') 
                                            INSERT INTO users(user_id, user_name) VALUES( @User_id, @Name);";

                    parameter = new QC.SqlParameter("@Name", DT.SqlDbType.NChar, 50);
                    parameter.Value = Name;
                    command.Parameters.Add(parameter);

                    parameter = new QC.SqlParameter("@User_id", DT.SqlDbType.Int);
                    parameter.Value = (int)User_id;
                    command.Parameters.Add(parameter);
                    QC.SqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("Insert - OK");
                }
            }
        }
    }
}
