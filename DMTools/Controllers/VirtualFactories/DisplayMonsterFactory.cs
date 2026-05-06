using Controllers.Helpers;
using Data.Constants;
using Data.Objects;
using Data.VirtualObject;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.VirtualFactories
{
    public class DisplayMonsterFactory
    {
        public List<DisplayMonster> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                var monsters = collection.FindAll().OrderBy(m => m.Name).ToList();

                return monsters.Select(m => new DisplayMonster
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Subtype = m.Subtype,
                    ArmorClass = m.ArmorClass,
                    HitPoints = m.HitPoints,
                    ChallengeRating = m.ChallengeRating
                }).ToList();
            }
        }

        public List<DisplayMonster> GetObjectsBySearchCriteria(String name)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                var monsters = collection.Find(m => m.Name.Contains(name)).OrderBy(m => m.Name).ToList();

                return monsters.Select(m => new DisplayMonster
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Subtype = m.Subtype,
                    ArmorClass = m.ArmorClass,
                    HitPoints = m.HitPoints,
                    ChallengeRating = m.ChallengeRating
                }).ToList();
            }
        }
    }
}