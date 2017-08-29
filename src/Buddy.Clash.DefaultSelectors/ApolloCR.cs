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
    public class ApolloCR : ActionSelectorBase
	{
        #region
        private static readonly ILogger Logger = LogProvider.CreateLogger<ApolloCR>();

        public override string Name => "Apollo";

        public override string Description => "Arena 1-4; 1vs1; Please lean back and let me Apollo do the work...";

        public override string Author => "Peros_";

        public override Version Version => new Version(1, 0, 0, 0);
        public override Guid Identifier => new Guid("{669f976f-23ce-4b97-9105-a21595a394bf}");
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

            if (StaticValues.Player.Mana < 2)
                return null;


            gameHandling.IniRound();
            Vector2f nextPosition = gameHandling.GetSpellPosition();
            FightState fightState = gameHandling.FightState;

            return PlayerCastHandling.SpellMagic(nextPosition, fightState);
        }
    }
}
