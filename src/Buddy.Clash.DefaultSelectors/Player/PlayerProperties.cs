using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Player
{
    enum DeckType
    {

    }

    enum FightStyle
    {
        Defensive,
        Balanced,
        Rusher
    }

    class PlayerProperties
    {
        private static FightStyle fightStyle = FightStyle.Balanced;
        public static FightStyle FightStyle
        {
            get { return fightStyle; }
            set { fightStyle = value; }
        }
    }
}
