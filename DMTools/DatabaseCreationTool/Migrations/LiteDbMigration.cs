using LiteDB;

namespace DatabaseCreationTool.Migrations
{
    public abstract class LiteDbMigration
    {
        public abstract long Version { get; }
        public abstract string Description { get; }

        public abstract void Up(LiteDatabase db);
        public abstract void Down(LiteDatabase db);
    }
}
