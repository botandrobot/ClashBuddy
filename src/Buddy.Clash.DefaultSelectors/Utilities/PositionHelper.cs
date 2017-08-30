using Buddy.Clash.DefaultSelectors.Game;
using Buddy.Clash.DefaultSelectors.Logic;
using Buddy.Clash.Engine.NativeObjects.Logic.GameObjects;
using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    class PositionHelper
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<PositionHelper>();


        public static Vector2f AddYInDirection(Vector2f position, Position fieldPosition, int y = 500)
        {
            Vector2f moveVector = new Vector2(0, y);
            Logger.Debug("PlayerPosition: {0}", fieldPosition);

            switch (GameStateHandling.CurrentGameMode)
            {
                case GameMode.ONE_VERSUS_ONE:
                    if (fieldPosition == Position.Down)
                        return position - moveVector;
                    else
                        return position + moveVector;
                case GameMode.TWO_VERSUS_TWO:
                    if (fieldPosition == Position.Down)
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
