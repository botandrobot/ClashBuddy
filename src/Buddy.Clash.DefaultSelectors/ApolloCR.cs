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

    // Just 1v1
    public class ApolloCR : IActionSelector
    {
        #region
        private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();

        public string Name => "Apollo";

        public string Description => "Arena 1-4; 1vs1; Please lean back and let me Apollo do the work...";

        public string Author => "Token";

        public Version Version => new Version(1, 0, 0, 0);
        public Guid Identifier => new Guid("{669f976f-23ce-4b97-9105-a21595a394bf}");
        #endregion
        private static PositionHandling positionHandling = new PositionHandling();
        private static CharacterHandling characterHandling = new CharacterHandling();
        private static OwnCardHandling cardHandling = new OwnCardHandling();

        public CastRequest GetNextCast()
        {
            #region battle valid check
            var battle = ClashEngine.Instance.Battle;
            if (battle == null || !battle.IsValid)
            {
                GameStateHandling.GameBeginning = true;
                return null;
            }
            #endregion

            #region Just for logging
            Log.Debug("Avatar-Count: " + ClashEngine.Instance.Battle.AvatarCount);
            Log.Debug("Avatar1-StartPos: " + ClashEngine.Instance.Battle.AvatarLocations1.StartPosition);

            Log.Debug("OwnerIndex: " + ClashEngine.Instance.LocalPlayer.OwnerIndex);
            Logger.Debug("IsEnemyCharOnOurSide: " + CharacterHandling.IsEnemyOnOurSide());
            characterHandling.LogCharInformations();
            #endregion Just for logging

            #region GameState and next position
            GameState gameState = GameStateHandling.CurrentGameState;

            Vector2f nextPosition = positionHandling.GetNextSpellPosition(gameState);
            #endregion

            return CastHandling.SpellMagic(nextPosition, gameState);
        }
    }
}
