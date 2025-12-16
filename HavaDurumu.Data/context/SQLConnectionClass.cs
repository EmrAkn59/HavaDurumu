using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HavaDurumu.Data.Context
{
    public class SQLConnectionClass
    {
        public static SqlConnection connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=AirQualityDB;Integrated Security=True");

        public static void CheckConnection() 
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }
    }
}
