namespace Robi.Clash.DefaultSelectors.Settings
{
    using Robi.Common;
    using Robi.Engine.Settings.Attributes;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    internal class DefaultRoutineSettings : JsonSettings
    {
        internal DefaultRoutineSettings() : base(SettingsPath, "Settings", "DefaultRoutine.json")
        {

        }
        /*
        public string DatabaseFullpath => System.IO.Path.GetFullPath(DatabaseFolder);
        
        [DisplayName("Database Folder")]
        [Description("The folder containing the DefaultRoutine Databases.")]
        [DefaultValue("DefaultRoutine")]
        public string DatabaseFolder { get; set; }
                
        [DisplayName("Random Deployment Faktor")]
        [Description("Random deployment factor range to let the bot look more human like")]
        [Range(0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }*/
    }
}