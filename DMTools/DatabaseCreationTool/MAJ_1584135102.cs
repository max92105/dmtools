using FluentMigrator;
using System;

namespace DatabaseCreationTool
{
    [Migration(1584135102)]
    public class MAJ_1584135102 : Migration
    {
        public override void Up()
        {
            Alter.Table("Actions").AddColumn("IsBonus").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("Actions").AddColumn("IsReaction").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}