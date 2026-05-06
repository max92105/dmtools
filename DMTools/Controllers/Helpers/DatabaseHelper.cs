using Data.Constants;
using LiteDB;

namespace Controllers.Helpers
{
    public static class DatabaseHelper
    {
        public static LiteDatabase GetDatabase()
        {
            return new LiteDatabase(DatabaseInfo.DatabasePath);
        }
    }
}
