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

        [SettingsGroup("Random Deployment Faktor")]
        [FloatRangeSettings(100, 0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }
    }
}
