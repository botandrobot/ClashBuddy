using Robi.Clash.DefaultSelectors.Game;
using Robi.Clash.DefaultSelectors.Logic;
using Robi.Clash.Engine.NativeObjects.Logic.GameObjects;
using Robi.Clash.Engine.NativeObjects.Native;
using Robi.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Utilities
{
    class PositionHelper
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<PositionHelper>();


        public static Vector2f AddYInDirection(Vector2f position, Position fieldPosition, int y = 1000)
        {
            Vector2f moveVector = new Vector2(0, y);
            Logger.Debug("PlayerPosition: {0}", fieldPosition);

                    if (fieldPosition == Position.Down)
                        return position - (moveVector*4);
                    else
                        return position + moveVector;
        }

        public static Vector2f SubtractYInDirection(Vector2f position, Position fieldPosition, int y = 1000)
        {
            Vector2f moveVector = new Vector2(0, y);
            Logger.Debug("PlayerPosition: {0}", fieldPosition);

                    if (fieldPosition == Position.Down)
                        return position + (moveVector*4);
                    else
                        return position - moveVector;
        }
    }
}
