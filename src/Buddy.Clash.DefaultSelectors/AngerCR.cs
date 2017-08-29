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

    // Just 1v1
    public class AngerCR : ActionSelectorBase
    {
        #region
        private static readonly ILogger Logger = LogProvider.CreateLogger<ApolloCR>();

        public override string Name => "Anger";

        public override string Description => "Arena 1-x; 1vs1; Let me destroy them fast!";

        public override string Author => "Peros_";

        public override Version Version => new Version(1, 0, 0, 0);
        public override Guid Identifier => new Guid("{cc525fda-c4a9-4f1e-b0ea-098b5350ece7}");
        #endregion

        private static GameHandling gameHandling = new GameHandling();

        public override CastRequest GetNextCast()
        {
            #region battle valid check
            var battle = ClashEngine.Instance.Battle;
            if (battle == null || !battle.IsValid)
            {
                return null;
            }
            #endregion

            if (StaticValues.Player.Mana < 1)
                return null;

            if (Clash.Engine.ClashEngine.Instance.Battle.BattleTime.Seconds < 1)
                gameHandling.IniGame(FightStyle.Rusher);

            gameHandling.IniRound();
            Vector2f nextPosition = gameHandling.GetSpellPosition();
            FightState fightState = gameHandling.FightState;

            return PlayerCastHandling.SpellMagic(nextPosition, fightState);
        }
    }
}
