namespace Robi.Clash.DefaultSelectors.Behaviors
{
    using Common;
    using Robi.Clash.DefaultSelectors.Apollo;
    using Robi.Engine.Settings;
    using Serilog;
    using Settings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Apollo : BehaviorBase
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<Apollo>();

        #region
        public override string Name => "Apollo";

        public override string Description => "1vs1; Please lean back and let me Apollo do the work...";

        public override string Author => "Peros_";

        public override Version Version => new Version(1, 7, 0, 0);
        public override Guid Identifier => new Guid("{669f976f-23ce-4b97-9105-a21595a394bf}");

        private static ApolloSettings Settings => SettingsManager.GetSetting<ApolloSettings>("Apollo");

        public override void Initialize()
        {
            base.Initialize();
            SettingsManager.RegisterSettings(Name, new ApolloSettings());
            FillSettings();
        }

        public override void Deinitialize()
        {
            SettingsManager.UnregisterSettings(Name);
            base.Deinitialize();
        }
        #endregion

        private static bool StartLoadedDeploy = false;
        private static FightState currentSituation;

        public override Cast GetBestCast(Playfield p)
        {
            //DebugThings(p);
            Cast bc = null;
            Logger.Debug("Home = {Home}", p.home);

            #region Apollo Magic
            PlayfieldAnalyse.AnalyseLines(p);
            currentSituation = GetCurrentFightState(p);
            Handcard hc = CardChoosing.GetOppositeCard(p, currentSituation);

            if (hc == null)
            {
                Logger.Debug("Part: SpellApolloWay");
                Handcard hcApollo = SpellMagic(p, currentSituation, out  VectorAI choosedPosition);

                if (hcApollo != null)
                {
                    hc = hcApollo;

                    if (choosedPosition != null)
                        return new Cast(hcApollo.name, choosedPosition, hcApollo);
                }

            }

            if (hc == null)
                return null;

            Logger.Debug("Part: GetSpellPosition");
            VectorAI nextPosition = PositionChoosing.GetNextSpellPosition(currentSituation, hc, p);
            bc = new Cast(hc.name, nextPosition, hc);
            #endregion

            if (bc != null) Logger.Debug("BestCast:" + bc.SpellName + " " + bc.Position.ToString());
            else Logger.Debug("BestCast: null");

            return bc;
        }

        

        private static void DebugThings(Playfield p)
        {
            Type t = typeof(Line);
            PropertyInfo[] properties = t.GetProperties();

            PlayfieldAnalyse.AnalyseLines(p);
            Line[] line = PlayfieldAnalyse.lines;

            foreach (PropertyInfo nP in properties)
            {
                Console.WriteLine(nP.GetValue(line).ToString());
            }

            IEnumerable<Handcard> damagingSpells = Classification.GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);
            if (damagingSpells != null)
            {
                IOrderedEnumerable<Handcard> radiusOrderedDS = damagingSpells.OrderBy(n => n.card.DamageRadius);
                group Group = p.getGroup(false, 200, boPriority.byTotalNumber, radiusOrderedDS.FirstOrDefault().card.DamageRadius);
            }

            Logger.Debug("Name: " + p.ownKingsTower.Name);
            Logger.Debug("Name: " + p.ownPrincessTower1.Name);
            Logger.Debug("Name: " + p.ownPrincessTower2.Name);

            int i1 = p.ownKingsTower.HP;
            int i2 = p.ownPrincessTower1.HP;
            int i3 = p.ownPrincessTower2.HP;

            Logger.Debug("test");
        }

        private static Handcard SpellMagic(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        {
            choosedPosition = null;
            switch (currentSituation)
            {
                case FightState.UAKT:
                case FightState.UAPTL1:
                case FightState.UAPTL2:
                case FightState.DKT:
                case FightState.DPTL1:
                case FightState.DPTL2:
                case FightState.AKT:
                case FightState.APTL1:
                case FightState.APTL2:
                    return CardChoosing.All(p, currentSituation, out choosedPosition);
                case FightState.START:
                    return null;
                case FightState.WAIT:
                    return null;
                default:
                    return CardChoosing.All(p, currentSituation, out choosedPosition);
            }
        }

        public static FightState GetCurrentFightState(Playfield p)
        {
            // Debugging: try - catch is just for debugging
            try
            {
                switch (Setting.FightStyle)
                {
                    case FightStyle.Defensive:
                        return GetCurrentFightStateDefensive(p);
                    case FightStyle.Balanced:
                        return GetCurrentFightStateBalanced(p);
                    case FightStyle.Rusher:
                        return GetCurrentFightStateRusher(p);
                    default:
                        return FightState.DKT;
                }
            }
            catch (Exception)
            {
                return GetCurrentFightStateBalanced(p);
            }

        }

        private static FightState GetCurrentFightStateBalanced(Playfield p)
        {
            FightState fightState = FightState.WAIT;

            if (GameBeginning)
            {
                StartLoadedDeploy = false;
                return Decision.GameBeginningDecision(p, out GameBeginning);
            }

            int dangerOrAttackLine = Decision.DangerOrBestAttackingLine(p);

            if (dangerOrAttackLine < 0)
            {
                Logger.Debug("Danger");
                StartLoadedDeploy = false;
                fightState = Decision.DangerousSituationDecision(p, dangerOrAttackLine * (-1));
            }
            else if (dangerOrAttackLine > 0)
            {
                Logger.Debug("Chance");
                StartLoadedDeploy = false;
                fightState = Decision.GoodAttackChanceDecision(p, dangerOrAttackLine);
            }
            else
            {
                // Debugging: try - catch is just for debugging
                try
                {
                    if (p.ownMana >= Setting.ManaTillDeploy)
                    {
                        StartLoadedDeploy = true;
                        fightState = Decision.DefenseDecision(p);
                    }
                }
                catch (Exception) { StartLoadedDeploy = true; }

                if (StartLoadedDeploy)
                    fightState = Decision.DefenseDecision(p);
            }

            //Logger.Debug("FightSate = {0}", fightState.ToString());
            return fightState;
        }

        private static FightState GetCurrentFightStateDefensive(Playfield p)
        {
            if (GameBeginning)
                return Decision.GameBeginningDecision(p, out GameBeginning);

            if (!p.noEnemiesOnMySide())
                return Decision.EnemyIsOnOurSideDecision(p);
            else if (p.enemyMinions.Count > 1)
                return Decision.EnemyHasCharsOnTheFieldDecision(p);
            else
                return Decision.DefenseDecision(p);
        }

        private static FightState GetCurrentFightStateRusher(Playfield p)
        {
            if (!p.noEnemiesOnMySide())
                return Decision.EnemyIsOnOurSideDecision(p);
            else
                return Decision.AttackDecision(p);
        }

        private static void FillSettings()
        {
            Setting.FightStyle = Settings.FightStyle; 
            Setting.KingTowerSpellDamagingHealth = Settings.KingTowerSpellDamagingHealth;
            Setting.ManaTillDeploy = Settings.ManaTillDeploy;
            Setting.ManaTillFirstAttack = Settings.ManaTillFirstAttack;
            Setting.MinHealthAsTank = Settings.MinHealthAsTank;
            Setting.SpellCorrectionConditionCharCount = Settings.SpellCorrectionConditionCharCount;
            Setting.DangerSensitivity = Settings.DangerSensitivity;
            Setting.ChanceSensitivity = Settings.ChanceSensitivity;
        }

        public override float GetPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            int retval = 0;
            return retval;
        }

        public override int GetBoValue(BoardObj bo, Playfield p)
        {
            int retval = 5;
            return retval;
        }

        public override int GetPlayCardPenalty(CardDB.Card card, Playfield p)
        {
            return 0;
        }
    }
}