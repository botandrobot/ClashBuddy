using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Buddy.Common;
using Buddy.Engine.Settings.Attributes;

namespace Buddy.Clash.DefaultSelectors.Settings
{
    internal class ControlSettings : JsonSettings
    {
	    internal ControlSettings() : base(SettingsPath, "Settings", "Control.json")
	    {
		    
	    }

        [Category("Default")]
		[DisplayName("Random Deployment Faktor")]
        [Description("Random deployment factor range to let the bot look more human like")]
        [Range(0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }
	}
}
