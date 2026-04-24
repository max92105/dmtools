using FluentMigrator;
using System;

namespace DatabaseCreationTool
{
    [Migration(1550614474)]
    public class MAJ_1550614474 : Migration
    {
        public override void Up()
        {
            Create.Table("Groups").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("Name").AsString().NotNullable();

            Create.Table("PlayerCharacterGroups").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("GroupId").AsGuid().NotNullable()
                    .WithColumn("PlayerCharacterId").AsGuid().NotNullable();

            Alter.Table("PlayerCharacters").AddColumn("Level").AsInt16().NotNullable().WithDefaultValue(1);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}