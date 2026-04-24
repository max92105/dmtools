using Controllers.VirtualFactories;
using Data.DataModels.MonsterManagerPage;
using Data.VirtualObject;
using System;
using System.Collections.ObjectModel;

namespace Controllers.Controllers
{
    public static class MonsterManagerPageDataController
    {
        public static MonsterManagerPageDataModel Load()
        {
            MonsterManagerPageDataModel monsterManagerPageDataModel = new MonsterManagerPageDataModel();

            monsterManagerPageDataModel.Monsters = new ObservableCollection<DisplayMonster>(new DisplayMonsterFactory().GetObjects());

            return monsterManagerPageDataModel;
        }    

        public static MonsterManagerPageDataModel SearchMonster(String name)
        {
            MonsterManagerPageDataModel monsterManagerPageDataModel = new MonsterManagerPageDataModel();

            monsterManagerPageDataModel.Monsters = new ObservableCollection<DisplayMonster>(new DisplayMonsterFactory().GetObjectsBySearchCriteria(name));

            return monsterManagerPageDataModel;
        }
    }
}
