using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    public static class Extensions
    {
        public static bool IsPositionInArea(this BoardObj bo, Playfield p, VectorAI position)
        {
            bool isInArea = position.X >= bo.Position.X - bo.Range &&
                            position.X <= bo.Position.X + bo.Range &&
                            position.Y >= bo.Position.Y - bo.Range &&
                            position.Y <= bo.Position.Y + bo.Range;

            return isInArea;
        }
    }
}
