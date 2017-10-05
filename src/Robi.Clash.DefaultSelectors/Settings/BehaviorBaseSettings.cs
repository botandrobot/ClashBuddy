namespace Robi.Clash.DefaultSelectors.Settings
{
    using Common;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    internal class BehaviorBaseSettings : JsonSettings
    {
        internal BehaviorBaseSettings() : base(SettingsPath, "Settings", "BehaviorBaseSettings.json")
        {

        }

        public string DatabaseFullpath => System.IO.Path.GetFullPath(DatabaseFolder);

        [DisplayName("Routine version")]
        [Description("Routine version.")]
        [DefaultValue("0.8.0")]
        public string DatabaseFolder { get; }
        
    }
}