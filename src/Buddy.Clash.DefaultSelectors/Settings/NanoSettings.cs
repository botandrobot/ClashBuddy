namespace Buddy.Clash.DefaultSelectors.Settings
{
    using Buddy.Common;
    using Buddy.Engine.Settings.Attributes;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    internal class NanoSettings : JsonSettings
    {
        internal NanoSettings() : base(SettingsPath, "Settings", "Nano.json")
        {

        }
        /*
        public string DatabaseFullpath => System.IO.Path.GetFullPath(DatabaseFolder);
        
        [DisplayName("Database Folder")]
        [Description("The folder containing the Nano Databases.")]
        [DefaultValue("Nano")]
        public string DatabaseFolder { get; set; }
                
        [DisplayName("Random Deployment Faktor")]
        [Description("Random deployment factor range to let the bot look more human like")]
        [Range(0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }*/
    }
}