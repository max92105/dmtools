using System.Data.SQLite;
using System.IO;
using FluentMigrator;
using FluentMigrator.Runner.Announcers;
using System.Reflection;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors.SQLite;
using System;
using Data.Constants;

namespace DatabaseCreationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(DatabaseInfo.DatabasePath))
                    SQLiteConnection.CreateFile(DatabaseInfo.DatabasePath);

                DateTime start = DateTime.Now;
                string connectionString = DatabaseInfo.ConnectionString;

                Announcer announcer = new TextWriterAnnouncer(WriteToConsole);

                #if DEBUG
                announcer.ShowSql = true;
                #endif

                Assembly assembly = Assembly.GetExecutingAssembly();
                IRunnerContext migrationContext = new RunnerContext(announcer);

                var options = new ProcessorOptions
                {
                    PreviewOnly = false,  // set to true to see the SQL
                    Timeout = 60
                };

                var factory = new SQLiteProcessorFactory();

                using (IMigrationProcessor processor = factory.Create(connectionString, announcer, options))
                {
                    var runner = new MigrationRunner(assembly, migrationContext, processor);
                    runner.MigrateUp(true);
                }

                WriteToConsole(String.Format("----< Les Migrations ont durées : {0} >----", DateTime.Now.Subtract(start)));
            }
            catch(Exception ex)
            {
                WriteToConsole(ex.Message);
            }

            #if DEBUG
            Console.WriteLine("");
            Console.WriteLine("----< PRESS ENTER TO END PROGRAM >----");
            Console.ReadLine();
            #endif
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
