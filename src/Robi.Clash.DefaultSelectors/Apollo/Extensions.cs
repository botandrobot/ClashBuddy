using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    public static class Extensions
    {
        public static bool IsPositionInArea(this BoardObj bo, Playfield p, VectorAI position)
        {
            int buildingSizeAddition = 1000;

            long a = ((bo.Position.X + buildingSizeAddition - position.X) * (bo.Position.X + buildingSizeAddition - position.X));
            long b = ((bo.Position.Y + buildingSizeAddition - position.Y) * (bo.Position.Y + buildingSizeAddition - position.Y));
            long c = (bo.Range * bo.Range);

            return ((a + b) < c);
        }
    }
}
