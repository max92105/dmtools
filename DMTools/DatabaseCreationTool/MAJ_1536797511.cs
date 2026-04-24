using FluentMigrator;
using System;

namespace DatabaseCreationTool
{
    [Migration(1536797511)]
    public class MAJ_1536797511 : Migration
    {
        public override void Up()
        {
            Create.Table("PlayerCharacters").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("Name").AsString().NotNullable()
                    .WithColumn("ArmorClass").AsInt16().NotNullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}