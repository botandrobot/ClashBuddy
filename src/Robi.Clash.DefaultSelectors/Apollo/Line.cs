
namespace Robi.Clash.DefaultSelectors.Apollo
{


    class Line
    {
        private Level danger;
        public Level Danger { get => danger; set => danger = value; }

        private int enemyMinionHP; 
        public int EnemyMinionHP { get => enemyMinionHP; set => enemyMinionHP = value; }

        private int enemyMinionCount;
        public int EnemyMinionCount { get => enemyMinionCount; set => enemyMinionCount = value; }

        private int enemyMinionAtk;
        public int EnemyMinionAtk { get => enemyMinionAtk; set => enemyMinionAtk = value; }

        private int ownMinionHP;
        public int OwnMinionHP { get => ownMinionHP; set => ownMinionHP = value; }

        private int ownMinionCount;
        public int OwnMinionCount { get => ownMinionCount; set => ownMinionCount = value; }

        private int ownMinionAtk;
        public int OwnMinionAtk { get => ownMinionAtk; set => ownMinionAtk = value; }

        private Level ownPtHp;
        public Level OwnPtHp { get => ownPtHp; set => ownPtHp = value; }

        private Level enemyPtHp;
        public Level EnemyPtHp { get => enemyPtHp; set => enemyPtHp = value; }

    }
}
