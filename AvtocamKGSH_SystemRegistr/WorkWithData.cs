using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvtocamKGSH_SystemRegistr
{
    internal class WorkWithData
    {
        private static SqlConnection connection { get; } = new SqlConnection(Properties.Settings.Default.KGSH_Avtocam_RegistrConnectionString);
        private static SqlConnection connection_1 { get; } = new SqlConnection(Properties.Settings.Default.KGSH_Avtocam_RegistrConnectionString);
        public static SqlDataReader DevicesReader = null;
        private static string sqlExpression;

        // выбираем записи по одному ID 
        static public  void InsertValue(Int16 adr, float temp)
        {
            
                sqlExpression = "InsertTemperature";
                DateTime curTime = DateTime.Now;
                using (SqlCommand insert_command = new SqlCommand(sqlExpression, connection))
                {
                    if (connection.State == 0) connection.Open();
                    // указываем, что команда представляет хранимую процедуру
                    insert_command.CommandType = System.Data.CommandType.StoredProcedure;
                    // параметр для ввода имени
                    insert_command.Parameters.AddWithValue("adr", adr);
                    insert_command.Parameters.AddWithValue("temp", temp);
                    insert_command.Parameters.AddWithValue("currtime", curTime);
                    insert_command.ExecuteNonQuery();
                }
        }

        public  static void ReadDevices()
        {
            if (connection_1.State == 0) connection_1.Open();
            string sqlExpression = "SELECT modbus_adress from Devices";
            SqlCommand read_command = new SqlCommand(sqlExpression, connection_1);
            DevicesReader = read_command.ExecuteReader();
        }
    }
}
