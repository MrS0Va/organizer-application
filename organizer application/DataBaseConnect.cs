using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organizer_application
{
    public static class DataBaseConnect
    {
        private static readonly string connectionString = "Data Source=WIN-GCR09CENVB3\\SQLEXPRESS;Initial Catalog=TaskOrganizer;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
