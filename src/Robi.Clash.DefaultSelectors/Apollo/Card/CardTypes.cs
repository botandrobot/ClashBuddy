using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Card
{
    enum TroopType
    {
        Tank,
        Damager,
        Ranger,
        AirAttack,
        GroundAttack,
        AOEAttackGround,
        AOEAttackFlying,
        Flying
    };

    enum SpellType
    {
        SpellDamaging,
        SpellOther
    };

    enum BuildingType
    {
        BuildingDefense,
        BuildingSpawning
    };

    enum CardTypeOld
    {
        Defense,
        All,
        Troop,
        Buildings,
        NONE
    };
}
