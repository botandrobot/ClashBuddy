using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    public enum FightState
    {
        DLPT,       // Defense LeftPrincessTower
        DKT,        // Defense KingTower
        DRPT,       // Defense RightPrincessTower
        UALPT,      // UnderAttack LeftPrincessTower
        UAKT,       // UnderAttack KingTower
        UARPT,      // UnderAttack RightPrincessTower
        ALPT,       // Attack LeftPrincessTower
        AKT,        // Attack KingTower
        ARPT,        // Attack RightPrincessTower
        START,
        WAIT
    };

    enum CardTypeOld
    {
        Defense,
        All,
        Troop,
        Buildings,
        NONE
    };

    enum DeployDecision
    {
        DamagingSpell,
        AOEAttack,
        AttacksFlying,
        Buildings,
        CycleSpell,
        PowerSpell
    };

    enum FightStyle
    {
        Defensive,
        Balanced,
        Rusher
    };

    enum SpecificCardType
    {
        All,


        // Mobs
        MobsTank,
        MobsDamageDealer,
        MobsBuildingAttacker,
        MobsRanger,
        MobsAOEGround,
        MobsAOEAll,
        MobsFlyingAttack,

        // Buildings
        BuildingsDefense,
        BuildingsAttack,
        BuildingsSpawning,
        BuildingsMana,

        // Spells
        SpellsDamaging,
        SpellsNonDamaging
    };

    public enum Level
    {
        ZERO,
        LOW,
        MEDIUM,
        HIGH
    };
}
