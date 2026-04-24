using System;
using System.Configuration;

namespace Data.Constants
{
    public sealed class DatabaseInfo
    {
        public static String ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        public static String DatabasePath = ConfigurationManager.AppSettings.Get("DatabasePath");
    }
}
