using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.Engine.NativeObjects.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    class PositionHelper
    {
        public static Vector2f AddYInIndexDirection(Vector2f position, uint ownerIndex, int y = 1000)
        {
            Vector2f moveVector = new Vector2(0, y);

            switch (GameStateHandling.CurrentGameMode)
            {
                case GameMode.ONE_VERSUS_ONE:
                    if (ownerIndex == 0)
                        return position - moveVector;
                    else
                        return position + moveVector;
                case GameMode.TWO_VERSUS_TWO:
                    if (ownerIndex == 0 || ownerIndex == 1)
                        return position - moveVector;
                    else
                        return position + moveVector;
                case GameMode.NOT_IMPLEMENTED:
                    return position;
                default:
                    return position;
            }
        }
    }
}
