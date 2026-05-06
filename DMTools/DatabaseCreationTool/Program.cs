using Data.Constants;
using DatabaseCreationTool.Migrations;
using LiteDB;
using System;
using System.IO;
using System.Reflection;

namespace DatabaseCreationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DateTime start = DateTime.Now;
                string liteDbPath = DatabaseInfo.DatabasePath;

                Console.WriteLine("========================================");
                Console.WriteLine("  DMTools Database Tool");
                Console.WriteLine("========================================");
                Console.WriteLine("");
                Console.WriteLine("1. Run LiteDB migrations");
                Console.WriteLine("2. Import data from SQLite database");
                Console.WriteLine("3. Run migrations + Import from SQLite");
                Console.WriteLine("");
                Console.Write("Choose an option (1/2/3): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunMigrations(liteDbPath);
                        break;

                    case "2":
                        RunSqliteImport(liteDbPath);
                        break;

                    case "3":
                        RunMigrations(liteDbPath);
                        RunSqliteImport(liteDbPath);
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                WriteToConsole(String.Format("----< Completed in: {0} >----", DateTime.Now.Subtract(start)));
            }
            catch (Exception ex)
            {
                WriteToConsole("ERROR: " + ex.Message);
                WriteToConsole(ex.StackTrace);
            }

            #if DEBUG
            Console.WriteLine("");
            Console.WriteLine("----< PRESS ENTER TO END PROGRAM >----");
            Console.ReadLine();
            #endif
        }

        private static void RunMigrations(string liteDbPath)
        {
            WriteToConsole("----< LiteDB Migration Runner >----");
            WriteToConsole(String.Format("Database Path: {0}", liteDbPath));

            // Ensure directory exists
            string directory = Path.GetDirectoryName(liteDbPath);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var db = new LiteDatabase(liteDbPath))
            {
                var runner = new LiteDbMigrationRunner(db);
                runner.LoadMigrationsFromAssembly(Assembly.GetExecutingAssembly());
                runner.MigrateUp();
            }

            WriteToConsole("----< Migrations complete >----");
        }

        private static void RunSqliteImport(string liteDbPath)
        {
            Console.WriteLine("");
            Console.Write("Enter the path to your SQLite database file: ");
            string sqlitePath = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(sqlitePath))
            {
                Console.WriteLine("No path provided, skipping import.");
                return;
            }

            // Remove quotes if user pasted a path with quotes
            sqlitePath = sqlitePath.Trim('"', ' ');

            if (!File.Exists(sqlitePath))
            {
                Console.WriteLine("ERROR: File not found: " + sqlitePath);
                return;
            }

            WriteToConsole("----< SQLite to LiteDB Import >----");
            var importer = new SqliteToLiteDbImporter(sqlitePath, liteDbPath);
            importer.Import();
            WriteToConsole("----< Import complete >----");
        }

        private static void WriteToConsole(String message)
        {
            #if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
            #endif

            Console.WriteLine(message);
        }
    }
}
