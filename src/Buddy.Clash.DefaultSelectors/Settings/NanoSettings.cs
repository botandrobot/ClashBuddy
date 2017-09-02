namespace Buddy.Clash.DefaultSelectors.Settings
{
    using Buddy.Common;
    using Buddy.Engine.Settings.Attributes;
    using System.ComponentModel;

    internal class NanoSettings : JsonSettings
    {
        internal NanoSettings() : base(SettingsPath, "Settings", "Nano.json")
        {

        }

        public string DatabaseFullpath => System.IO.Path.GetFullPath(DatabaseFolder);

        [SettingsName("Database Folder")]
        [SettingsDescription("The folder containing the Nano Databases.")]
        [DefaultValue("Nano")]
        public string DatabaseFolder { get; set; }

        [SettingsName("Logfile Name")]
        [SettingsDescription("File name for the logfile, might be removed in the future.")]
        [DefaultValue("nano.log")]
        public string LogFileName { get; set; }
        
        [SettingsName("Random Deployment Faktor")]
        [SettingsDescription("Random deployment factor range to let the bot look more human like")]
        [IntegerRangeSettings(100, 0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }
    }
}