using System;

namespace Data.Constants
{
    public sealed class Characteristics
    {
        public static String Strenght = "101e14dc-3839-43a0-8a1f-93c37ac68991";
        public static String Dexterity = "8cd29246-1cf6-4a2f-99b9-f12d65194367";
        public static String Constitution = "d0143a99-0c0a-42bd-b03c-3b1ee942f2c1";
        public static String Intelligence = "399c5b0e-8ca9-40f7-8725-d32d91e05e27";
        public static String Wisdom = "2a992f03-0789-4000-a2d7-880e46f81c8a";
        public static String Charisma = "2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5";

        public static Guid StrenghtId
        {
            get { return new Guid(Strenght); }
        }

        public static Guid DexterityId
        {
            get { return new Guid(Dexterity); }
        }

        public static Guid ConstitutionId
        {
            get { return new Guid(Constitution); }
        }

        public static Guid IntelligenceId
        {
            get { return new Guid(Intelligence); }
        }

        public static Guid WisdomId
        {
            get { return new Guid(Wisdom); }
        }

        public static Guid CharismaId
        {
            get { return new Guid(Charisma); }
        }
    }
}
