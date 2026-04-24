using System;

namespace Data.Constants
{
    public sealed class SkillTypes
    {
        public static String Acrobatics = "9573927d-9c94-421d-8b18-b2debeef8745";
        public static String AnimalHandling = "886c05c7-3d9c-45b0-87ca-216b71d909bc";
        public static String Arcana = "8ae70126-bc8d-47d7-b3a6-86e29790b1fd";
        public static String Athletics = "cf72463a-79f8-450d-8813-4a16c8b9c958";
        public static String Deception = "69ce9255-0b61-4628-9054-e5179740b346";
        public static String History = "8187ded7-d483-4be3-9325-377e0c642cbd";
        public static String Insight = "7bb93d48-b4ec-4af5-b75d-433bffc170ae";
        public static String Intimidation = "cf572e73-3539-453a-9993-7c99f117ae0a";
        public static String Investigation = "29ce9bab-55f7-434b-8566-8a1dfd6e534f";
        public static String Medicine = "713e2b39-358b-4984-b668-fa1359ff9baf";
        public static String Nature = "f2a4a614-9d7f-4f97-8fa2-7a7fbaa136b5";
        public static String Perception = "567426db-519a-410b-86c1-7a46293fed4c";
        public static String Performance = "887ee217-163d-4938-a1d8-f6e767d609c5";
        public static String Persuasion = "47d5d78b-ff2a-48c2-ba55-53ed459f1c1e";
        public static String Religion = "f9de3362-b90e-411e-8c4e-0b789ba879b8";
        public static String SleightOfHand = "2fa5771b-7a73-46db-91b1-9a9f1fb362c4";
        public static String Stealth = "c3b1adb9-73db-40a5-9b7e-6b3bb93b46d6";
        public static String Survival = "18b8f0cf-00ee-47ac-b207-b99fbed6dc31";

        public static Guid AcrobaticsId
        {
            get { return new Guid(Acrobatics); }
        }

        public static Guid AnimalHandlingId
        {
            get { return new Guid(AnimalHandling); }
        }

        public static Guid ArcanaId
        {
            get { return new Guid(Arcana); }
        }

        public static Guid AthleticsId
        {
            get { return new Guid(Athletics); }
        }

        public static Guid DeceptionId
        {
            get { return new Guid(Deception); }
        }

        public static Guid HistoryId
        {
            get { return new Guid(History); }
        }

        public static Guid InsightId
        {
            get { return new Guid(Insight); }
        }

        public static Guid IntimidationId
        {
            get { return new Guid(Intimidation); }
        }

        public static Guid InvestigationId
        {
            get { return new Guid(Investigation); }
        }

        public static Guid MedicineId
        {
            get { return new Guid(Medicine); }
        }

        public static Guid NatureId
        {
            get { return new Guid(Nature); }
        }

        public static Guid PerceptionId
        {
            get { return new Guid(Perception); }
        }

        public static Guid PerformanceId
        {
            get { return new Guid(Performance); }
        }

        public static Guid PersuasionId
        {
            get { return new Guid(Persuasion); }
        }

        public static Guid ReligionId
        {
            get { return new Guid(Religion); }
        }

        public static Guid SleightOfHandId
        {
            get { return new Guid(SleightOfHand); }
        }

        public static Guid StealthId
        {
            get { return new Guid(Stealth); }
        }

        public static Guid SurvivalId
        {
            get { return new Guid(Survival); }
        }
    }
}
