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

		[DefaultValue(true)]
	    [SettingsName("Enable Awesomeness")]
	    [SettingsDescription("Having this enabled, you get super awesome!")]
	    public bool IsAwesome { get; set; }

	    [SettingsGroup("Pwn Range")]
	    [FloatRangeSettings(100f, 0f, 3000f)]
	    [DefaultValue(500f)]
	    public float PwnRange { get; set; }
	}
}
