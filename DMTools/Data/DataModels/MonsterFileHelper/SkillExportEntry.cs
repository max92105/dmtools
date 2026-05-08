using System;

namespace Data.DataModels.MonsterFileHelper
{
    public class SkillExportEntry
    {
        public Guid Id { get; set; }
        public Guid MonsterId { get; set; }
        public Guid SkillTypeId { get; set; }
        public string SkillTypeName { get; set; }
        public short Save { get; set; }
    }
}
