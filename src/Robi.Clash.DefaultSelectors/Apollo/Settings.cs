using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors.Apollo
{
    static class Setting
    {
        public static FightStyle FightStyle { get; set; }

        public static int KingTowerSpellDamagingHealth { get; set; }

        public static int SpellCorrectionConditionCharCount { get; set; }

        public static int ManaTillFirstAttack { get; set; }

        public static int ManaTillDeploy { get; set; }

        public static int MinHealthAsTank { get; set; }

        public static Level DangerSensitivity { get; set; }

        public static Level ChanceSensitivity { get; set; }

        //[SettingsGroup(" Deployment Faktor")]
        //[FloatRangeSettings(100, 0, 10000)]
        //[DefaultValue(200)]
        //public int DeploymentValue { get; set; }
    }
}
