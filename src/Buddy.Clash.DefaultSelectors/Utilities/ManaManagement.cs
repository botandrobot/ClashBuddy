using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Utilities
{
    class ManaManagement
    {
        public const float BasicManaGainRate = 2.8f;

        public bool isDoubleElixirActive = false;
        public bool IsDoubleElixirActive
        {
            get
            {
                if (isDoubleElixirActive)
                    return true;
                
               
                //isDoubleElixirActive = Clash.Engine.ClashEngine.Instance.Battle.IsDoubleElixirActive;
                return isDoubleElixirActive;
            }
        }
    }
}
