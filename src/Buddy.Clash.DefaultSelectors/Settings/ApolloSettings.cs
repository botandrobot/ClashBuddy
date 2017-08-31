using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Buddy.Common;
using Buddy.Engine.Settings.Attributes;

namespace Buddy.Clash.DefaultSelectors.Settings
{
    internal class ApolloSettings : JsonSettings
    {
	    internal ApolloSettings() : base(SettingsPath, "Settings", "Apollo.json")
	    {
		    
	    }

        [DefaultValue(Player.FightStyle.Balanced)]
        [SettingsName("Choose Apollos fight style")]
        [SettingsDescription("Smart balanced, concentrated on the Defense or as an angry rusher?")]
        public Player.FightStyle FightStyle { get; set; }

        [SettingsName("Random Deployment Faktor")]
        [SettingsDescription("Random deployment factor range to let the bot look more human like")]
        [FloatRangeSettings(100, 0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }

        [SettingsName("Enemys KingTower spell damaging mode")]
        [SettingsDescription("Starts to attack the enemys KingTower with all damaging spells at the Health...")]
        [FloatRangeSettings(100, 0, 10000)]
        [DefaultValue(400)]
        public int KingTowerSpellDamagingHealth { get; set; }

        [SettingsName("Spell position correction (friendly chars around)")]
        [SettingsDescription("Don´t correct spell deployment position if at least x friendly characters around the enemy")]
        [FloatRangeSettings(0, 0, 50)]
        [DefaultValue(2)]
        public int SpellCorrectionConditionCharCount { get; set; }

        [SettingsName("Spell deployment decision (minimum anzahl Characters)")]
        [SettingsDescription("How many enemy characters should be at least in the area for an deploy")]
        [FloatRangeSettings(0, 0, 50)]
        [DefaultValue(5)]
        public int SpellDeployConditionCharCount { get; set; }

        [SettingsName("Game-Beginning: Mana-Load")]
        [SettingsDescription("How much Mana till first attack")]
        [FloatRangeSettings(0, 0, 10)]
        [DefaultValue(9)]
        public int ManaTillFirstAttack { get; set; }

        [SettingsName("Tank health")]
        [SettingsDescription("How much health-points to classify an character as a tank")]
        [FloatRangeSettings(0, 0, 10000)]
        [DefaultValue(1200)]
        public int MinHealthAsTank { get; set; }

        //[SettingsGroup("Random Deployment Faktor")]
        //[FloatRangeSettings(100, 0, 10000)]
        //[DefaultValue(200)]
        //public int RandomDeploymentValue { get; set; }
    }
}
