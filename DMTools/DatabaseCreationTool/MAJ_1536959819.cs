using FluentMigrator;
using System;

namespace DatabaseCreationTool
{
    [Migration(1536959819)]
    public class MAJ_1536959819 : Migration
    {
        public override void Up()
        {
            Alter.Table("PlayerCharacters").AddColumn("InitiativeBonus").AsInt16().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}