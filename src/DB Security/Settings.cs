using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB_Security
{
    public static class Settings
    {
        //static string _connectionString = @"Data Source=SOMPHOP-PC\SQLEXPRESS;Initial Catalog=dbBO23Kiosk;User ID=bo23kiosk;Password=thunder@11";
        static string _connectionString = @"Data Source=BHMVISION\SQLEXPRESS;Initial Catalog=dbBO23Kiosk;User ID=bo23kiosk;Password=thunder@11";
        public static string ConnectionString { get { return _connectionString; } }
    }
}
