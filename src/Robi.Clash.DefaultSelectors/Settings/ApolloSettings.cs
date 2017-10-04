using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Robi.Common;
using Robi.Engine.Settings.Attributes;

namespace Robi.Clash.DefaultSelectors.Settings
{
    internal class ApolloSettings : JsonSettings
    {
        internal ApolloSettings() : base(SettingsPath, "Settings", "Apollo.json")
        {

        }

        [Category("Default")]
        [DefaultValue(Apollo.FightStyle.Balanced)]
        [DisplayName("Fight Style")]
        [Description("Choose Apollos fight style. Smart balanced, concentrated on the Defense or as an angry rusher?")]
        public Apollo.FightStyle FightStyle { get; set; }

        [Category("Default")]
        [DefaultValue(Apollo.Level.MEDIUM)]
        [DisplayName("Danger Sensitivity")]
        [Description("Sensitivity of danger analysis?")]
        public Apollo.Level DangerSensitivity { get; set; }

        [Category("Default")]
        [DefaultValue(Apollo.Level.MEDIUM)]
        [DisplayName("Chance Sensitivity")]
        [Description("Sensitivity of chance analysis?")]
        public Apollo.Level ChanceSensitivity { get; set; }

        [Category("Default")]
        [DisplayName("Enemys KingTower spell damaging mode")]
        [Description("Starts to attack the enemys KingTower with all damaging spells at the Health...")]
        [Range(0, 10000)]
        [DefaultValue(400)]
        public int KingTowerSpellDamagingHealth { get; set; }

        [Category("Default")]
        [DisplayName("Spell position correction (friendly chars around)")]
        [Description("Don´t correct spell deployment position if at least x friendly characters around the enemy")]
        [Range(0, 50)]
        [DefaultValue(2)]
        public int SpellCorrectionConditionCharCount { get; set; }

        [Category("Game Start")]
        [DisplayName("Mana-Load")]
        [Description("How much Mana till first attack")]
        [Range(0, 10)]
        [DefaultValue(10)]
        public int ManaTillFirstAttack { get; set; }

        [Category("Attack")]
        [DisplayName("Mana-Load")]
        [Description("How much Mana till deploying characters if no enemy on our side")]
        [Range(0, 10)]
        [DefaultValue(10)]
        public int ManaTillDeploy { get; set; }

        [Category("Default")]
        [DisplayName("Tank health")]
        [Description("How much health-points to classify an character as a tank")]
        [Range(0, 10000)]
        [DefaultValue(1200)]
        public int MinHealthAsTank { get; set; }

        //[SettingsGroup(" Deployment Faktor")]
        //[FloatRangeSettings(100, 0, 10000)]
        //[DefaultValue(200)]
        //public int DeploymentValue { get; set; }
    }
}
