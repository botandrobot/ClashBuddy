using System;
using System.Collections.Generic;
using System.Text;
using Buddy.Clash.Engine.NativeObjects.Native;
using Buddy.Common;
using Serilog;

namespace Buddy.Clash.DefaultSelectors.Utilities
{
    public static class VectorHelper
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();

        public static void GetCoordinates(this Vector2 v, out int x, out int y)
        {
            try
            {
                //Logger.Debug("GetCoordinates-Vector-Param: " + v);
                String[] vectorString = v.ToString().Split('/');
                x = int.Parse(vectorString[0].Replace("{",""));
                y = int.Parse(vectorString[1].Replace("}",""));
            }
            catch (Exception e)
            {
                //Logger.Debug("GetCoordinates--Exception: " + e.Message);
                x = 0;
                y = 0;
            }
        }

        public static int GetY(this Vector2 v)
        {
            int x, y;
            GetCoordinates(v, out x, out y);

            return y;
        }

        public static int GetX(this Vector2 v)
        {
            int x, y;
            GetCoordinates(v, out x, out y);

            return x;
        }
    }
}
