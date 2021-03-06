﻿using System;
using DT = System.Data;
using QC = System.Data.SqlClient;

namespace SpendingDatabase
{
    public class Program
    {
        private static readonly String CONNECTION_STR = @"Data Source=LENOVO-PC;Initial Catalog=SpendingTracker;Integrated Security=True";
            //"Data Source=LAPTOP-DL0OAIPO\SQLEXPRESS;Initial Catalog=SpendingTracker;Integrated Security=True";

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

        static public void InsertCategory(int User_id, string Name)
        {
            using (var connection = new QC.SqlConnection(CONNECTION_STR))
            {
                connection.Open();
                QC.SqlParameter parameter;
                using (var command = new QC.SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = $@"INSERT INTO [dbo].[categories] (category_name, spending_id) VALUES(@Name, (select top 1 spending_id from spendings order by id desc));";

                    parameter = new QC.SqlParameter("@Name", DT.SqlDbType.NChar, 50);
                    parameter.Value = Name;
                    command.Parameters.Add(parameter);

                    QC.SqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("Insert category - OK");
                }
            }
        }

        static public void InsertSpending(int User_id, double Amount)
        {
            using (var connection = new QC.SqlConnection(CONNECTION_STR))
            {
                connection.Open();
                DateTime dt = DateTime.Now;
                QC.SqlParameter parameter;
                using (var command = new QC.SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText = $@"INSERT INTO [dbo].[spendings] (user_id, amount, dt) VALUES(@User_id, @Amount, @dt);";

                    parameter = new QC.SqlParameter("@User_id", DT.SqlDbType.Int);
                    parameter.Value = User_id;
                    command.Parameters.Add(parameter);

                    parameter = new QC.SqlParameter("@Amount", DT.SqlDbType.Float);
                    parameter.Value = Amount;
                    command.Parameters.Add(parameter);

                    parameter = new QC.SqlParameter("@dt", DT.SqlDbType.DateTime);
                    parameter.Value = dt;
                    command.Parameters.Add(parameter);
                    QC.SqlDataReader reader = command.ExecuteReader();
                }
            }
        }

        static public void DeleteRows(int User_id)
        {
            using (var command = new QC.SqlCommand())
            {
                using (var connection = new QC.SqlConnection(CONNECTION_STR))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandType = DT.CommandType.Text;
                    command.CommandText =
                    $@"DELETE FROM [dbo].[categories]
                       DELETE FROM [dbo].[spendings] WHERE user_id = {User_id} 
                       DELETE FROM [dbo].[users] WHERE user_id = {User_id};";
                    QC.SqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("Delete - OK");
                }
            }
        }

    }
}
