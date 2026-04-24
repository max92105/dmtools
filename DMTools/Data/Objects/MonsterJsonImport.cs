using System;
using System.Collections.Generic;

namespace Data.Objects
{
    public class MonsterJsonImport
    {
        public String index;
        public String name;
        public String size;
        public String type;
        public String subtype;
        public String alignment;
        public Int16 armor_class;
        public Int16 hit_points;
        public String hit_dice;
        public String speed;
        public Int16 strength;
        public Int16 dexterity;
        public Int16 constitution;
        public Int16 intelligence;
        public Int16 wisdom;
        public Int16 charisma;
        public Int16 strength_save;
        public Int16 dexterity_save;
        public Int16 constitution_save;
        public Int16 intelligence_save;
        public Int16 wisdom_save;
        public Int16 charisma_save;
        public Int16 acrobatics;
        public Int16 arcana;
        public Int16 athletics;
        public Int16 deception;
        public Int16 history;
        public Int16 insight;
        public Int16 intimidation;
        public Int16 investigation;
        public Int16 medicine;
        public Int16 nature;
        public Int16 perception;
        public Int16 performance;
        public Int16 persuasion;
        public Int16 religion;
        public Int16 stealth;
        public Int16 survival;
        public String damage_vulnerabilities;
        public String damage_resistances;
        public String damage_immunities;
        public String condition_immunities;
        public String senses;
        public String languages;
        public Decimal challenge_rating;

        public List<SpecialAbilityImport> special_abilities;
        public List<ActionImport> actions;
        public List<LegendaryAction> legendary_actions;
    }

    public class SpecialAbilityImport
    {
        public String name;
        public String desc;
        public Int16 attack_bonus;
    }

    public class ActionImport
    {
        public String name;
        public String desc;
        public Int16 attack_bonus;
        public String damage_dice;
        public Int16 damage_bonus;
    }

    public class LegendaryAction
    {
        public String name;
        public String desc;
        public Int16 attack_bonus;
    }
}
