namespace Robi.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;

    public enum boardObjType
    {
        NONE,
        BUILDING,
        MOB,
        AOE,
        PROJECTILE
    }

    public enum transportType
    {
        NONE,
        AIR,
        GROUND
    }

    public enum targetType
    {
        NONE,
        ALL,
        GROUND,
        BUILDINGS,
        IGNOREBUILDINGS
    }

    public enum affectType
    {
        NONE,
        ALL,
        ONLY_OWN,
        ONLY_ENEMIES
    }

    public enum deployDirectionAbsolute
    {
        none, //the position of the object itself
        //Absolute directions:
        behindKingsTowerCenter, //the Archers will be divided in to the different directions in this position
        behindKingsTowerLine1, //troops will go in the specified direction (Line 1)
        behindKingsTowerLine2, //troops will go in the specified direction (Line 2)
        cornerLine1, //corner on the board on Line 1
        cornerLine2, //corner on the board on Line 2
        bridgeLine1,
        bridgeLine2,
        betweenBridges, //on my side, the Archers will be divided in to the different directions in this position
        borderBridgeLine1, //border near the bridge on my side
        borderBridgeLine2, //border near the bridge on my side
        ownPrincessTowerLine1,
        ownPrincessTowerLine2,
        enemyPrincessTowerLine1,
        enemyPrincessTowerLine2,
    }

    public enum deployDirectionRelative
    {
        none, //the position of the object itself

        //Relative directions:
        Up, //angle0,
        RightUp, //angle45,
        Right, //angle90,
        RightDown, //angle135,
        Down, //angle180,
        LeftDown, //angle225,
        Left, //angle270,
        LeftUp, //angle315,
        borderSideUp, //angle 45 or 315 to the nearest border
        borderSideMiddle, //angle 90 or 270 to the nearest border
        borderSideDown, //angle 135 or 225 to the nearest border
        centerSideUp, //angle 45 or 315 closer to the center
        centerSideMiddle, //angle 90 or 270 closer to the center
        centerSideDown, //angle 135 or 225 closer to the center
        lineCorner, //bottom left or right corner on the board
        //customAngle, TODO if it is needed
    }

    public enum RoutineLogLevel
    {
        Verbose,
        Compact
    }

}