using Robi.Clash.DefaultSelectors.Settings;
using Robi.Engine.Settings;

namespace Robi.Clash.DefaultSelectors
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
    using Robi.Clash.DefaultSelectors.Player;
    using Robi.Clash.DefaultSelectors.Game;
    using Robi.Clash.DefaultSelectors.Logic;
    using Robi.Clash.DefaultSelectors.Card;

    // Just 1v1
    public class ApolloCR : ActionSelectorBase
    {
        #region
        private static readonly ILogger Logger = LogProvider.CreateLogger<ApolloCR>();

        public override string Name => "Apollo";

        public override string Description => "1vs1; Please lean back and let me Apollo do the work...";

        public override string Author => "Peros_";

        public override Version Version => new Version(1, 3, 0, 0);
        public override Guid Identifier => new Guid("{669f976f-23ce-4b97-9105-a21595a394bf}");
        #endregion

        private static GameHandling gameHandling = new GameHandling();

        internal static ApolloSettings Settings => SettingsManager.GetSetting<ApolloSettings>("Apollo");
        private static bool NewBattle = true;

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

            if (NewBattle)
            {
                gameHandling.IniGame(Settings);
                NewBattle = false;
            }

            gameHandling.IniRound();
            FightState fightState = gameHandling.FightState;
            ICard spell = CastDeploymentHandling.SpellMagic(fightState);

            if (spell == null)
                return null;

            Vector2f nextPosition = CastPositionHandling.GetNextSpellPosition(fightState, spell);

            return new CastRequest(spell.Name, nextPosition);
        }

        public override void BattleStart()
        {
            Logger.Debug("-----------------BattleStart");
            NewBattle = true;
        }

        public override void BattleEnd()
        {
            Logger.Debug("-----------------BattleEnd");
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

