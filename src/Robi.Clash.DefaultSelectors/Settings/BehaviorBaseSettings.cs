namespace Robi.Clash.DefaultSelectors.Settings
{
    using Common;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    internal class BehaviorBaseSettings : JsonSettings
    {
        internal BehaviorBaseSettings() : base(SettingsPath, "Settings", "Routine.json")
        {

        }

        [Category("Default")]
        [DefaultValue(RoutineLogLevel.Verbose)]
        [DisplayName("Routine Log Level")]
        [Description("Change the log level of the battle logfile. Default: Verbose")]
        public RoutineLogLevel routineLogLevel { get; set; }
    }
}