namespace Buddy.Clash.DefaultSelectors.Settings
{
    using Buddy.Common;
    using Buddy.Engine.Settings.Attributes;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    internal class DefaultRoutineSettings : JsonSettings
    {
        internal DefaultRoutineSettings() : base(SettingsPath, "Settings", "DefaultRoutine.json")
        {

        }

        public enum Behavior
        {
            BehaviorApollo,
            BehaviorControl
        };

        [Category("Default")]
        [DefaultValue(Behavior.BehaviorApollo)]
        [DisplayName("Behavior")]
        [Description("")]
        public Behavior SelectedBehavior { get; set; }

        [Category("Default")]
        [DefaultValue(Player.FightStyle.Balanced)]
        [DisplayName("Fight Style")]
        [Description("Choose Apollos fight style. Smart balanced, concentrated on the Defense or as an angry rusher?")]
        public Player.FightStyle FightStyle { get; set; }

        [Category("Default")]
        [DisplayName("Random Deployment Faktor")]
        [Description("Random deployment factor range to let the bot look more human like")]
        [Range(0, 10000)]
        [DefaultValue(200)]
        public int RandomDeploymentValue { get; set; }

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

        [Category("Default")]
        [DisplayName("Spell deployment decision (minimum anzahl Characters)")]
        [Description("How many enemy characters should be at least in the area for an deploy")]
        [Range(0, 50)]
        [DefaultValue(5)]
        public int SpellDeployConditionCharCount { get; set; }

        [Category("Game Start")]
        [DisplayName("Mana-Load")]
        [Description("How much Mana till first attack")]
        [Range(0, 10)]
        [DefaultValue(9)]
        public int ManaTillFirstAttack { get; set; }

        [Category("Attack")]
        [DisplayName("Mana-Load")]
        [Description("How much Mana till attack")]
        [Range(0, 10)]
        [DefaultValue(7)]
        public int ManaTillAttack { get; set; }

        [Category("Default")]
        [DisplayName("Tank health")]
        [Description("How much health-points to classify an character as a tank")]
        [Range(0, 10000)]
        [DefaultValue(1200)]
        public int MinHealthAsTank { get; set; }

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