using Buddy.Clash.DefaultSelectors.Settings;
using Buddy.Engine.Settings;

namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Engine;
    using Common;
    using Serilog;
    using Engine.NativeObjects.Native;
    using System.Collections.Generic;
    using Engine.NativeObjects.Logic.GameObjects;
    using Utilities;
    using Buddy.Clash.DefaultSelectors.Player;
    using Buddy.Clash.DefaultSelectors.Game;
    using Buddy.Clash.DefaultSelectors.Logic;

    // Just 1v1
    public class ApolloCR : ActionSelectorBase
	{
        #region
        private static readonly ILogger Logger = LogProvider.CreateLogger<ApolloCR>();

        public override string Name => "Apollo";

        public override string Description => "1vs1; Please lean back and let me Apollo do the work...";

        public override string Author => "Peros_";

        public override Version Version => new Version(1, 1, 0, 0);
        public override Guid Identifier => new Guid("{669f976f-23ce-4b97-9105-a21595a394bf}");
        #endregion

        private static GameHandling gameHandling = new GameHandling();

		internal static ApolloSettings Settings => SettingsManager.GetSetting<ApolloSettings>("Apollo");

		public override CastRequest GetNextCast()
        {
            #region battle valid check
            var battle = ClashEngine.Instance.Battle;
            if (battle == null || !battle.IsValid)
            {
                return null;
            }
            #endregion

            if (StaticValues.Player.Mana < 2)
                return null;


            if (Clash.Engine.ClashEngine.Instance.Battle.BattleTime.TotalSeconds < 1)
                gameHandling.IniGame(Settings);


            gameHandling.IniRound();
            Vector2f nextPosition = gameHandling.GetSpellPosition();
            FightState fightState = gameHandling.FightState;

            if (PlayerCardClassifying.TroopRanger.FirstOrDefault() != null)
            {
                Logger.Debug("TroopRanger: " + PlayerCardClassifying.TroopRanger.FirstOrDefault().Name.Value);
                Logger.Debug("AOEToAir: " + PlayerCardClassifying.TroopRanger.FirstOrDefault().Projectile.AoeToAir);
                Logger.Debug("AOEToGround: " + PlayerCardClassifying.TroopRanger.FirstOrDefault().Projectile.AoeToGround);
            }

            if (PlayerCardClassifying.TroopTank.FirstOrDefault() != null)
            {
                Logger.Debug("TroopTank: " + PlayerCardClassifying.TroopTank.FirstOrDefault().Name.Value);
            }

            //foreach (var item in Engine.Csv.CsvLogic.Characters.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //    if (item.Projectile != null)
            //        Logger.Debug("Projectile: " + item.Projectile);
            //    if (item.Hitpoints != null)
            //        Logger.Debug("Hitpoints: " +item.Hitpoints.Value);
            //    if (item.HitSpeed != null)
            //        Logger.Debug("HitSpeed: " + item.HitSpeed.Value);
            //    if (item.Speed != null)
            //        Logger.Debug("Speed: " + item.Speed.Value);
            //    if (item.Range != null)
            //        Logger.Debug("Range: " + item.Range.Value);
            //    if(item.Damage != null)
            //        Logger.Debug("Damage: " + item.Damage.Value);
            //    if (item.ProjectileEffect != null)
            //        Logger.Debug("ProjectileEffect: " + item.ProjectileEffect);
            //}

            //Logger.Debug("SpellsBuildings: ");
            //foreach (var item in Engine.Csv.CsvLogic.SpellsBuildings.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //}

            //Logger.Debug("SpellsCharacters: ");
            //foreach (var item in Engine.Csv.CsvLogic.SpellsCharacters.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //}

            //Logger.Debug("SpellsOther: ");
            //foreach (var item in Engine.Csv.CsvLogic.SpellsOther.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //    if (item.InstantDamage != null)
            //        Logger.Debug("InstantDamage: " + item.InstantDamage.Value);
            //}

            //Logger.Debug("Buildings: ");
            //foreach (var item in Engine.Csv.CsvLogic.Buildings.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //}

            //Logger.Debug("CharacterBuffs: ");
            //foreach (var item in Engine.Csv.CsvLogic.CharacterBuffs.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //}

            //Logger.Debug("AreaEffectObjects: ");
            //foreach (var item in Engine.Csv.CsvLogic.AreaEffectObjects.Entries)
            //{
            //    if (item.Name != null)
            //        Logger.Debug("Name: " + item.Name);
            //}

            //

            return CastHandling.SpellMagic(nextPosition, fightState);
        }

		public override void Initialize()
		{
			SettingsManager.RegisterSettings(Name, new ApolloSettings());
		}

		public override void Deinitialize()
		{
			
			SettingsManager.UnregisterSettings(Name);
		}
	}
}
