namespace Robi.Clash.DefaultSelectors.Behaviors
{
    using Common;
    using Engine.NativeObjects.Native;
    using Robi.Engine.Settings;
    using Serilog;
    using Settings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class Apollo : BehaviorBase
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<Apollo>();

        #region
        public override string Name => "Apollo";

        public override string Description => "1vs1; Please lean back and let me Apollo do the work...";

        public override string Author => "Peros_";

        public override Version Version => new Version(1, 6, 0, 0);
        public override Guid Identifier => new Guid("{669f976f-23ce-4b97-9105-a21595a394bf}");
        #endregion

        private static ApolloSettings Settings => SettingsManager.GetSetting<ApolloSettings>("Apollo");

        public override void Initialize()
        {
            base.Initialize();
            SettingsManager.RegisterSettings(Name, new ApolloSettings());
        }

        public override void Deinitialize()
        {
            SettingsManager.UnregisterSettings(Name);
            base.Deinitialize();
        }

        private static bool StartLoadedDeploy = false;

        #region enums
        public enum FightState
        {
            DLPT,       // Defense LeftPrincessTower
            DKT,        // Defense KingTower
            DRPT,       // Defense RightPrincessTower
            UALPT,      // UnderAttack LeftPrincessTower
            UAKT,       // UnderAttack KingTower
            UARPT,      // UnderAttack RightPrincessTower
            ALPT,       // Attack LeftPrincessTower
            AKT,        // Attack KingTower
            ARPT,        // Attack RightPrincessTower
            START,
            WAIT
        };

        enum CardTypeOld
        {
            Defense,
            All,
            Troop,
            Buildings,
            NONE
        };

        enum DeployDecision
        {
            DamagingSpell,
            AOEAttack,
            AttacksFlying,
            Buildings,
            CycleSpell,
            PowerSpell
        };

        enum FightStyle
        {
            Defensive,
            Balanced,
            Rusher
        };

        enum SpecificCardType
        {
            All,


            // Mobs
            MobsTank,
            MobsDamageDealer,
            MobsBuildingAttacker,
            MobsRanger,
            MobsAOEGround,
            MobsAOEAll,
            MobsFlyingAttack,

            // Buildings
            BuildingsDefense,
            BuildingsAttack,
            BuildingsSpawning,
            BuildingsMana,

            // Spells
            SpellsDamaging,
            SpellsNonDamaging
        };
        #endregion enums

        private static FightState currentSituation;

        public override Cast GetBestCast(Playfield p)
        {
            //DebugThings(p);
            Cast bc = null;
            Logger.Debug("Home = {Home}", p.home);

            #region Apollo Magic
            Logger.Debug("Part: Get CurrentSituation");
            currentSituation = CurrentFightState(p);
            Logger.Debug("Part: GetOppositeCard");
            Handcard hc = GetOppositeCard(p, currentSituation);

            if (hc == null)
            {
                Logger.Debug("Part: SpellApolloWay");
                VectorAI choosedPosition;
                Handcard hcApollo = SpellMagic(p, currentSituation, out choosedPosition);

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
            VectorAI nextPosition = GetNextSpellPosition(currentSituation, hc, p);
            bc = new Cast(hc.name, nextPosition, hc);
            #endregion

            if (bc != null) Logger.Debug("BestCast:" + bc.SpellName + " " + bc.Position.ToString());
            else Logger.Debug("BestCast: null");

            return bc;
        }

        private static void DebugThings(Playfield p)
        {

            IEnumerable<Handcard> damagingSpells = GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);
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

            int abc = 10;
            Logger.Debug("test");
        }



        private static Handcard GetOppositeCard(Playfield p, FightState currentSituation)
        {
            try
            {
                if (p.enemyKingsTower.HP < Settings.KingTowerSpellDamagingHealth)
                {
                    Handcard hc = AttackKingTowerWithSpell(p);

                    if (hc != null)
                        return hc;
                }
            }
            catch (Exception){ }

            switch (currentSituation)
            {
                case FightState.UAKT:
                case FightState.UALPT:
                case FightState.UARPT:
                case FightState.AKT:
                case FightState.ALPT:
                case FightState.ARPT:
                case FightState.DKT:
                case FightState.DLPT:
                case FightState.DRPT:
                {
                        BoardObj defender = GetBestDefender(p);

                        if (defender == null)
                            return null;

                        Logger.Debug("BestDefender: {Defender}", defender.ToString());
                        opposite spell = KnowledgeBase.Instance.getOppositeToAll(p, defender, canWaitDecision(p));

                        if (spell != null && spell.hc != null)
                        {
                            Logger.Debug("Spell: {Sp} - MissingMana: {MM}", spell.hc.name, spell.hc.missingMana);
                            if (spell.hc.missingMana == 100) // Oposite-Card is already on the field
                                return null;
                            else if (spell.hc.missingMana > 0)
                                return null;
                            else
                                return spell.hc;
                        }
                }
                break; 
            }
            return null;
        }

        private static Handcard SpellMagic(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        {
            choosedPosition = null;
            switch (currentSituation)
            {
                case FightState.UAKT:
                case FightState.UALPT:
                case FightState.UARPT:
                case FightState.DKT:
                case FightState.DLPT:
                case FightState.DRPT:
                case FightState.AKT:
                case FightState.ALPT:
                case FightState.ARPT:
                    return All(p, currentSituation, out choosedPosition);
                case FightState.START:
                    return null;
                case FightState.WAIT:
                    return null;
                default:
                    return All(p, currentSituation, out choosedPosition);
            }
        }

        private static BoardObj GetBestDefender(Playfield p)
        {
            // TODO: Find better condition
            Logger.Debug("Path: Spell - GetBestDefender");
            int count = 0;
            BoardObj enemy = EnemyCharacterWithTheMostEnemiesAround(p, out count, transportType.NONE);

            if (enemy == null)
                return p.ownKingsTower;

            if (enemy.Line == 2)
                return p.ownPrincessTower2;
            else if (enemy.Line == 1)
                return p.ownPrincessTower1;
            else
                return p.ownKingsTower;

        }

        #region Analyse Current Situation
        public static FightState CurrentFightState(Playfield p)
        {
            try
            {
                switch ((FightStyle)Settings.FightStyle)
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
            catch (Exception e)
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
                return GameBeginningDecision(p);
            }

            //if (!p.noEnemiesOnMySide())
            //{
            //    StartLoadedDeploy = false;
            //    fightState = EnemyIsOnOurSideDecision(p);
            //}

            
            int dangerOrAttackLine = GetDangerOrBestAttackingLine(p);

            if (dangerOrAttackLine > 0)
            {
                Logger.Debug("Danger");
                StartLoadedDeploy = false;
                fightState = DangerousSituationDecision(p, dangerOrAttackLine);
            }
            else if (dangerOrAttackLine < 0)
            {
                Logger.Debug("Chance");
                StartLoadedDeploy = false;
                fightState = GoodAttackChanceDecision(p, dangerOrAttackLine * (-1));
            }
            else if (p.ownMana >= Settings.ManaTillDeploy)
            {
                StartLoadedDeploy = true;
                fightState = DefenseDecision(p);
            }
            else if (StartLoadedDeploy)
                fightState = DefenseDecision(p);

            //Logger.Debug("FightSate = {0}", fightState.ToString());
            return fightState;
        }

        private static FightState GetCurrentFightStateDefensive(Playfield p)
        {
            if (GameBeginning)
                return GameBeginningDecision(p);

            if (!p.noEnemiesOnMySide())
                return EnemyIsOnOurSideDecision(p);
            else if (p.enemyMinions.Count > 1)
                return EnemyHasCharsOnTheFieldDecision(p);
            else
                return DefenseDecision(p);
        }

        private static FightState GetCurrentFightStateRusher(Playfield p)
        {
            if (!p.noEnemiesOnMySide())
                return EnemyIsOnOurSideDecision(p);
            else
                return AttackDecision(p);
        }

        #endregion

        private static int GetDangerOrBestAttackingLine(Playfield p) // Good chance for an attack?
        {
            // TODO: Make a fusion of this and GetDangerLine

            #region Danger analyses
            IEnumerable<BoardObj> enemyMinionsL1 = p.enemyMinions.Where(n => n.Line == 1);
            IEnumerable<BoardObj> enemyMinionsL2 = p.enemyMinions.Where(n => n.Line == 2);

            #region enemy sums (atk and health; Line 1 and 2)
            int atkSumL1 = enemyMinionsL1.Sum(n => n.Atk);
            int healthSumL1 = enemyMinionsL1.Sum(n => n.HP);
            int atkSumL2 = enemyMinionsL2.Sum(n => n.Atk);
            int healthSumL2 = enemyMinionsL2.Sum(n => n.HP);
            #endregion

            if (p.ownKingsTower.Line == 1 && 
                (healthSumL1 > p.ownKingsTower.Atk * 2 && atkSumL1 > (p.ownKingsTower.MaxHP / 20)
                || enemyMinionsL1.Count() > 4 || 
                healthSumL1 >= p.ownKingsTower.Atk * 3))
                return 3;
            if (p.ownKingsTower.Line == 2 && 
                (healthSumL2 > p.ownKingsTower.Atk * 2 && atkSumL2 > (p.ownKingsTower.MaxHP / 20)
                || enemyMinionsL2.Count() > 4 || 
                healthSumL2 >= p.ownKingsTower.Atk * 3))
                return 3;
            if (healthSumL1 > p.ownPrincessTower1.Atk * 2 && atkSumL1 > p.ownPrincessTower1.MaxHP / 10
                || healthSumL1 > p.ownPrincessTower1.Atk * 4
                || enemyMinionsL1.Count() > 5)
                return 1;
            if (healthSumL2 > p.ownPrincessTower2.Atk * 2 && atkSumL2 > p.ownPrincessTower2.MaxHP / 10
                || healthSumL2 > p.ownPrincessTower2.Atk * 4
                || enemyMinionsL2.Count() > 5)
                return 2;

            #region just as comments (.attacker is not implemented atm)
            // Check if building attacks Tower (.attacker is not implemented atm)
            //if (p.ownKingsTower?.attacker?.type == boardObjType.BUILDING)
            //    return 3;
            //if (p.ownPrincessTower1?.attacker?.type == boardObjType.BUILDING)
            //    return 1;
            //if (p.ownPrincessTower2?.attacker?.type == boardObjType.BUILDING)
            //    return 2;
            #endregion

            #region check if buildings can attack own towers
            List<BoardObj> enemyBuildings = p.enemyBuildings;

            if (enemyBuildings?.Count() > 0)
            {
                BoardObj bKT = enemyBuildings.Where(n => n.IsPositionInArea(p, p.ownKingsTower.Position)).FirstOrDefault();
                BoardObj bPT1 = enemyBuildings.Where(n => n.IsPositionInArea(p, p.ownPrincessTower1.Position)).FirstOrDefault();
                BoardObj bPT2 = enemyBuildings.Where(n => n.IsPositionInArea(p, p.ownPrincessTower2.Position)).FirstOrDefault();

                if (bKT != null)
                    return 3;

                if (bPT1 != null)
                    return 1;

                if (bPT2 != null)
                    return 2;
            }
            #endregion
            #endregion

            #region Chance for good attack analyses
            #region check which tower is down and which cards are playable
            if (p.enemyKingsTower.Line == 1) // PT with line 1 is down
            {
                // Analyse own HandCards
                IEnumerable<Handcard> mobCards = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB);
                IEnumerable<Handcard> tankCards = mobCards.Where(n => n.card.MaxHP > Settings.MinHealthAsTank);
                IEnumerable<Handcard> destroyerCard = mobCards.OrderBy(n => (n.card.Atk * n.card.SummonNumber) > 100);  // use n.card.SummonNumber

                if (p.enemyKingsTower.HP < 1000)
                {
                    // Destroyer Card
                    if (destroyerCard.FirstOrDefault() != null && destroyerCard.FirstOrDefault().manacost -p.ownMana <= 0)
                        return -1;
                }

                if (tankCards.FirstOrDefault() != null && tankCards.FirstOrDefault().manacost -p.ownMana <= 0)
                {
                    if (mobCards.Where(n => n.manacost - p.ownMana <= 0).Count() > 0)
                        return -1;
                }

            }
            else if (p.enemyKingsTower.Line == 2) // PT with line 2 is down
            {
                // Analyse own HandCards
                IEnumerable<Handcard> mobCards = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB);
                IEnumerable<Handcard> tankCards = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsTank);
                IEnumerable<Handcard> destroyerCard = mobCards.OrderBy(n => (n.card.Atk * n.card.SummonNumber) > 100);  // use n.card.SummonNumber

                if (p.enemyKingsTower.HP < 1000)
                {
                    // Destroyer Card
                    if (destroyerCard.FirstOrDefault() != null && destroyerCard.FirstOrDefault().manacost - p.ownMana <= 0)
                        return -2;
                }

                if (tankCards.FirstOrDefault() != null && tankCards.FirstOrDefault().manacost - p.ownMana <= 0)
                {
                    if (mobCards.Where(n => n.manacost - p.ownMana <= 0).Count() > 0)
                        return -2;
                }
            }
            #endregion

            #region Own Minions
            IEnumerable<BoardObj> ownMinionsL1 = p.ownMinions.Where(n => n.Line == 1);
            IEnumerable<BoardObj> ownMinionsL2 = p.ownMinions.Where(n => n.Line == 2);

            int healthSumOwnL1 = ownMinionsL1.Sum(n => n.HP);
            int atkSumOwnL1 = ownMinionsL1.Sum(n => n.Atk);

            int healthSumOwnL2 = ownMinionsL2.Sum(n => n.HP);
            int atkSumOwnL2 = ownMinionsL2.Sum(n => n.Atk);

            if (healthSumOwnL1 > 300 || atkSumOwnL1 > 150)
                return -1;
            if (healthSumOwnL2 > 300 || atkSumOwnL2 > 150)
                return -2;

            #endregion
            #endregion
            return 0;
        }

        #region Decisions
        private static bool canWaitDecision(Playfield p)
        {
            if (p.noEnemiesOnMySide())
            {
                switch (currentSituation)
                {
                    //case FightState.DLPT:
                    //case FightState.DRPT:
                    //    break;
                    //case FightState.DKT:
                    //    break;
                    case FightState.ALPT:
                        {
                            if (p.BattleTime.TotalSeconds < 20)
                                return false;
                            else if (p.enemyPrincessTower1.HP < 300)
                                return false;
                            break;
                        }
                    case FightState.ARPT:
                        {
                            if (p.BattleTime.TotalSeconds < 20)
                                return false;
                            else if (p.enemyPrincessTower2.HP < 300)
                                return false;
                            break;
                        }
                    case FightState.AKT:
                        {
                            if (p.BattleTime.TotalSeconds < 30)
                                return false;
                            else if (p.enemyKingsTower.HP < 300)
                                return false;
                            break;
                        }
                }
            }
            else
            {
                if (p.BattleTime.TotalSeconds < 20)
                    return false;
                if (p.ownKingsTower.HP < 1000)
                    return false;
                if (p.ownKingsTower.attacked)
                    return false;
            }

            return true;
        }
        private static FightState DefenseDecision(Playfield p)
        {
            if (p.ownTowers.Count < 3)
                return FightState.DKT;

            BoardObj princessTower = p.enemyPrincessTowers.OrderBy(n => n.HP).FirstOrDefault(); // Because they are going to attack this tower

            if (princessTower.Line == 2)
                return FightState.DRPT;
            else
                return FightState.DLPT;
        }

        private static FightState EnemyHasCharsOnTheFieldDecision(Playfield p)
        {
            if (p.ownTowers.Count > 2)
            {
                //BoardObj obj = GetNearestEnemy(p);

                // ToDo: Get most dangeroust group
                group mostDangeroustGroup = p.getGroup(false, 200, boPriority.byTotalBuildingsDPS, 3000);

                if (mostDangeroustGroup == null)
                {
                    Logger.Debug("mostDangeroustGroup = null");
                    return FightState.DKT;
                }
                int line = mostDangeroustGroup.Position.X > 8700 ? 2 : 1;
                Logger.Debug("mostDangeroustGroup.Position.X = {0} ; line = {1}", mostDangeroustGroup?.Position?.X, line);


                if (line == 2)
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
            else
            {
                return FightState.DKT;
            }
        }

        private static FightState DangerousSituationDecision(Playfield p, int line)
        {
            if (p.ownTowers.Count > 2)
            {
                if (line == 2)
                    return FightState.UARPT;
                else if (line == 1)
                    return FightState.UALPT;
                else
                    return FightState.UAKT;
            }
            else
            {
                return FightState.UAKT;
            }
        }

        private static FightState EnemyIsOnOurSideDecision(Playfield p)
        {
            Logger.Debug("Enemy is on our Side!!");
            if (p.ownTowers.Count > 2) // Question: If in ownTowers the KT is not included change to > 1
            {
                BoardObj obj = GetNearestEnemy(p);

                if (obj != null && obj.Line == 2)
                    return FightState.UARPT;
                else
                    return FightState.UALPT;
            }
            else
            {
                return FightState.UAKT;
            }
        }

        private static FightState AttackDecision(Playfield p)
        {
            if (p.enemyTowers.Count < 3)
                return FightState.AKT;


            BoardObj princessTower = p.enemyPrincessTowers.OrderBy(n => n.HP).FirstOrDefault();

            if (princessTower.Line == 2)
                return FightState.ARPT;
            else
                return FightState.ALPT;
        }

        private static FightState GoodAttackChanceDecision(Playfield p, int line)
        {
            if (p.enemyTowers.Count < 3)
                return FightState.AKT;

            if (line == 2)
                return FightState.ARPT;
            else
                return FightState.ALPT;
        }

        private static FightState GameBeginningDecision(Playfield p)
        {
            bool StartFirstAttack = true;

            try
            {
                StartFirstAttack = (p.ownMana < Settings.ManaTillFirstAttack);
            }
            catch (Exception e)
            {

            }


            if (StartFirstAttack)
            {
                if (!p.noEnemiesOnMySide())
                    GameBeginning = false;

                return FightState.START;
            }
            else
            {
                GameBeginning = false;
                BoardObj obj = GetNearestEnemy(p);

                if (obj.Line == 2)
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
        }
        #endregion

        //#region Choosing Spell Card Old
        //private static Handcard SpellMagic(Playfield p, FightState currentSituation)
        //{
        //    CardTypeOld cardTypeToPlay = ChooseCardType(currentSituation);

        //    switch (cardTypeToPlay)
        //    {
        //        case CardTypeOld.All:
        //            return All(p);
        //        case CardTypeOld.Defense:
        //            return Defense(p);
        //        case CardTypeOld.Troop:
        //            return DefenseTroop(p);
        //        case CardTypeOld.Buildings:
        //            return Building(p);
        //        case CardTypeOld.NONE:
        //            return null;
        //    }
        //    return null;
        //}

        //private static CardTypeOld ChooseCardType(FightState currentSituation)
        //{
        //    switch (currentSituation)
        //    {
        //        case FightState.UAKT:
        //        case FightState.UALPT:
        //        case FightState.UARPT:
        //            return CardTypeOld.Defense;
        //        case FightState.AKT:
        //        case FightState.ALPT:
        //        case FightState.ARPT:
        //            return CardTypeOld.All;
        //        case FightState.DKT:
        //        case FightState.DLPT:
        //        case FightState.DRPT:
        //            return CardTypeOld.Troop;
        //        case FightState.START:
        //            return CardTypeOld.NONE;
        //        case FightState.WAIT:
        //            return CardTypeOld.NONE;
        //        default:
        //            return CardTypeOld.All;
        //    }
        //}
        //#endregion

        #region Which Card Old
        private static IOrderedEnumerable<Handcard> cycleCard(Playfield p)
        {
            return p.ownHandCards.Where(s => s != null && s.manacost <= 3 && s.card.type == boardObjType.MOB).OrderBy(s => s.manacost);
        }

        private static IOrderedEnumerable<Handcard> powerCard(Playfield p)
        {
            return p.ownHandCards.Where(s => s != null && s.manacost > 3 && s.card.type == boardObjType.MOB).OrderBy(s => s.manacost);
        }

        private static Handcard AttackKingTowerWithSpell(Playfield p)
        {
            IEnumerable<Handcard> spells = p.ownHandCards.Where(n => n.card.type == boardObjType.AOE || n.card.type == boardObjType.PROJECTILE);

            if (spells != null)
                return spells.FirstOrDefault();

            return null;
        }


        private static Handcard All(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        {
            // TODO: Use more current situation
            Logger.Debug("Path: Spell - All");

            Handcard damagingSpell = DamagingSpellDecision(p, out choosedPosition);
            if (damagingSpell != null)
                return damagingSpell;

            Handcard aoeCard = AOEDecision(p, out choosedPosition ,currentSituation);
            if (aoeCard != null)
                return aoeCard;

            if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
            {
                Logger.Debug("AttackFlying Needed");
                Handcard atkFlying = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
                if (atkFlying != null)
                    return atkFlying;
            }

            if ((int)currentSituation < 3) // Just for Defense
            {
                if (DeployBuildingDecision(p, out choosedPosition))
                {
                    // ToDo: Take right building and set right Building-Type
                    var buildingCard = p.ownHandCards.Where(n => n.card.type == boardObjType.BUILDING).FirstOrDefault();
                    if (buildingCard != null)
                        return new Handcard(buildingCard.name, buildingCard.lvl);
                }
                choosedPosition = null;
            }

            if ((int)currentSituation < 3 || (int)currentSituation > 5) // Just not at Under Attack
            {
                var tank = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsTank).OrderBy(n => n.card.MaxHP);
                if (tank.LastOrDefault() != null && tank.LastOrDefault().manacost <= p.ownMana)
                    return tank.LastOrDefault();
            }
            else
            {
                Handcard hc = p.ownHandCards.Where(n => n.manacost - p.ownMana <= 0).OrderBy(n => n.manacost).FirstOrDefault();
            }

            var rangerCard = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsRanger).FirstOrDefault();
            if (rangerCard != null && rangerCard.manacost <= p.ownMana)
                return rangerCard;

            var damageDealerCard = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsDamageDealer).FirstOrDefault();    
            if(damageDealerCard != null && damageDealerCard.manacost <= p.ownMana)
                return damageDealerCard;

            Logger.Debug("Wait - No card selected...");
            return null;
        }

        //private static Handcard DefenseTroop(Playfield p)
        //{
        //    Logger.Debug("Path: Spell - DefenseTroop");

        //    if (IsAOEAttackNeeded(p))
        //    {
        //        var atkAOE = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEGround).FirstOrDefault(); // Todo: just AOE-Attack

        //        if (atkAOE != null)
        //            return new Handcard(atkAOE.name, atkAOE.lvl);
        //    }

        //    if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
        //    {
        //        var atkFlying = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
        //        if (atkFlying != null)
        //            return new Handcard(atkFlying.name, atkFlying.lvl);
        //    }

        //    var powerSpell = powerCard(p).FirstOrDefault();
        //    if (powerSpell != null)
        //        return new Handcard(powerSpell.name, powerSpell.lvl);

        //    return cycleCard(p).FirstOrDefault();
        //}

        //private static Handcard Defense(Playfield p, FightState currentSituation, out VectorAI choosedPosition)
        //{
        //    Logger.Debug("Path: Spell - Defense");
        //    choosedPosition = null;
        //    IEnumerable<Handcard> damagingSpells = GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);


        //    Handcard damagingSpell = DamagingSpellDecision(p, out choosedPosition);
        //    if (damagingSpell != null)
        //        return new Handcard(damagingSpell.name, damagingSpell.lvl);

        //    Handcard aoeCard = AOEDecision(p, out choosedPosition, currentSituation);
        //    if (aoeCard != null)
        //        return aoeCard;

        //    if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
        //    {
        //        Handcard atkFlying = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsFlyingAttack).FirstOrDefault();
        //        if (atkFlying != null)
        //            return atkFlying;
        //    }

        //    var powerSpell = powerCard(p).FirstOrDefault();
        //    if (powerSpell != null)
        //        return new Handcard(powerSpell.name, powerSpell.lvl);

        //    return p.ownHandCards.FirstOrDefault();
        //}


        private static Handcard Building(Playfield p)
        {
            Logger.Debug("Path: Spell - Building");
            var buildingCard = p.ownHandCards.Where(n => n.card.type == boardObjType.BUILDING).FirstOrDefault();
            if (buildingCard != null)
                return new Handcard(buildingCard.name, buildingCard.lvl);

            return null;
        }


        public static Handcard DamagingSpellDecision(Playfield p, out VectorAI choosedPosition)
        {
            choosedPosition = null;

            IEnumerable<Handcard> damagingSpells = GetOwnHandCards(p, boardObjType.AOE, SpecificCardType.SpellsDamaging);
            
            if(damagingSpells.FirstOrDefault() == null)
                return null;

            Logger.Debug("Damaging-Spell: tower damage first card = " + damagingSpells.FirstOrDefault().card?.towerDamage);

            if (p.suddenDeath)
            {
                IEnumerable<Handcard> ds3 = damagingSpells.Where(n => (n.card.towerDamage >= p.enemyPrincessTower1.HP));
                IEnumerable<Handcard> ds4 = damagingSpells.Where(n => (n.card.towerDamage >= p.enemyPrincessTower2.HP));
                IEnumerable<Handcard> ds5 = damagingSpells.Where(n => (n.card.towerDamage >= p.enemyKingsTower.HP));


                if (ds3.FirstOrDefault() != null)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds3.FirstOrDefault().card.towerDamage, 
                        p.enemyPrincessTower1.HP);
                    choosedPosition = p.enemyPrincessTower1.Position;
                    return ds3.FirstOrDefault();
                }

                if (ds4.FirstOrDefault() != null)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds4.FirstOrDefault().card.towerDamage, 
                        p.enemyPrincessTower2.HP);
                    choosedPosition = p.enemyPrincessTower2.Position;
                    return ds4.FirstOrDefault();
                }

                if (ds5.FirstOrDefault() != null)
                {
                    Logger.Debug("towerDamage: {td} ; pt1.hp: {pt1hp}", ds5.FirstOrDefault().card.towerDamage, 
                        p.enemyKingsTower.HP);
                    choosedPosition = p.enemyKingsTower.Position;
                    return ds5.FirstOrDefault();
                }
            }

            IOrderedEnumerable<Handcard> radiusOrderedDS = damagingSpells.OrderBy(n => n.card.DamageRadius);

            group Group = p.getGroup(false, 200, boPriority.byTotalNumber, radiusOrderedDS.LastOrDefault().card.DamageRadius);
            int hpSum = (Group.lowHPboHP + Group.avgHPboHP + Group.hiHPboHP);
            IEnumerable<Handcard> ds1 = damagingSpells.Where(n => n.card.DamageRadius > 3 && n.card.Atk * 5 <= hpSum);

            if (ds1.FirstOrDefault() != null)
            {
                Logger.Debug("Damaging-Spell-Decision: HP-Sum of group = " + hpSum);
                choosedPosition = p.getDeployPosition(Group.Position, deployDirectionRelative.Down, 1000);
                return ds1.FirstOrDefault();
            }

            IEnumerable<Handcard> ds2 = damagingSpells.Where(n => n.card.DamageRadius <= 3 && n.card.Atk * 4 <= hpSum);

            if (ds2.FirstOrDefault() != null)
            {
                Logger.Debug("Damaging-Spell-Decision: HP-Sum of group = " + hpSum);
                choosedPosition = p.getDeployPosition(Group.Position, deployDirectionRelative.Down, 1000);
                return ds2.FirstOrDefault();
            }

            return null;

            
        }

        public static bool DeployBuildingDecision(Playfield p, out VectorAI choosedPosition)
        {
            choosedPosition = p.getDeployPosition(p.ownKingsTower, deployDirectionRelative.Up, 4000);
            return IsAnEnemyObjectInArea(p, choosedPosition, 3000, boardObjType.MOB);
        }


        private static Handcard AOEDecision(Playfield p, out VectorAI choosedPosition, FightState currentSituation)
        {
            int biggestEnemieGroupCount;
            choosedPosition = null;
            Handcard aoeGround = null, aoeAir = null;

            BoardObj objGround = EnemyCharacterWithTheMostEnemiesAround(p, out biggestEnemieGroupCount, transportType.GROUND);
            if (biggestEnemieGroupCount > 3)
                aoeGround = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEGround).FirstOrDefault();

            BoardObj objAir = EnemyCharacterWithTheMostEnemiesAround(p, out biggestEnemieGroupCount, transportType.AIR);
            if (biggestEnemieGroupCount > 3)
                aoeAir = GetOwnHandCards(p, boardObjType.MOB, SpecificCardType.MobsAOEGround).FirstOrDefault();

            switch (currentSituation)
            {
                case FightState.DLPT:
                case FightState.UALPT:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine1);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.ALPT:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.DRPT:
                case FightState.UARPT:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine2);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.ARPT:
                    choosedPosition = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);
                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
                case FightState.DKT:
                case FightState.UAKT:
                    if (aoeAir != null)
                    {
                        choosedPosition = objAir.Line == 1 ? p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine1) 
                            : p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine2);
                        return aoeAir;

                    }

                    if (aoeGround != null)
                    {
                        choosedPosition = objGround.Line == 1 ? p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine1)
                            : p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine2);
                    }
                    return aoeGround;
                case FightState.AKT:
                    choosedPosition = p.enemyKingsTower.Line == 3 ? p.enemyKingsTower.Position
                        : p.enemyKingsTower.Line == 1 ? p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1)
                        : p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);

                    if (aoeAir != null)
                        return aoeAir;

                    return aoeGround;
            }
            return null;
        }
        #endregion


        private static bool IsAnEnemyObjectInArea(Playfield p, VectorAI position, int areaSize, boardObjType type)
        {
            Func<BoardObj, bool> whereClause = n => n.Position.X >= position.X - areaSize && n.Position.X <= position.X + areaSize &&
                                                    n.Position.Y >= position.Y - areaSize && n.Position.Y <= position.Y + areaSize;


            if(type == boardObjType.MOB)
                return p.enemyMinions.Where(whereClause).Count() > 0;
            else if(type == boardObjType.BUILDING)
                return p.enemyBuildings.Where(whereClause).Count() > 0;
            else if (type == boardObjType.AOE)
                return p.enemyAreaEffects.Where(whereClause).Count() > 0;

            return false;
        }
        #region Which Card

        #endregion

        #region Which Position

        public static VectorAI GetNextSpellPosition(FightState gameState, Handcard hc, Playfield p)
        {
            if (hc == null || hc.card == null)
                return null;

            VectorAI choosedPosition = null;


            if (hc.card.type == boardObjType.AOE || hc.card.type == boardObjType.PROJECTILE)
            {
                Logger.Debug("AOE or PROJECTILE");
                return GetPositionOfTheBestDamagingSpellDeploy(p);
            }

            // ToDo: Handle Defense Gamestates
            switch (gameState)
            {
                case FightState.UAKT:
                    choosedPosition = UAKT(p, hc);
                    break;
                case FightState.UALPT:
                    choosedPosition = UALPT(p, hc);
                    break;
                case FightState.UARPT:
                    choosedPosition = UARPT(p, hc);
                    break;
                case FightState.AKT:
                    choosedPosition = AKT(p);
                    break;
                case FightState.ALPT:
                    choosedPosition = ALPT(p);
                    break;
                case FightState.ARPT:
                    choosedPosition = ARPT(p);
                    break;
                case FightState.DKT:
                    choosedPosition = DKT(p, hc);
                    break;
                case FightState.DLPT:
                    choosedPosition = DLPT(p, hc);
                    break;
                case FightState.DRPT:
                    choosedPosition = DRPT(p, hc);
                    break;
                default:
                    //Logger.Debug("GameState unknown");
                    break;
            }

            if (choosedPosition == null)
                return null;

            //Logger.Debug("GameState: {GameState}", gameState.ToString());
            //Logger.Debug("nextPosition: " + nextPosition);

            return choosedPosition;
        }

        #region UnderAttack
        private static VectorAI UAKT(Playfield p, Handcard hc)
        {
            return DKT(p, hc);
        }

        private static VectorAI UALPT(Playfield p, Handcard hc)
        {
            return DLPT(p, hc);
        }
        private static VectorAI UARPT(Playfield p, Handcard hc)
        {
            return DRPT(p, hc);
        }
        #endregion

        #region Defense
        private static VectorAI DKT(Playfield p, Handcard hc)
        {
            if (hc.card.type == boardObjType.MOB)
            {
                try
                {
                    if (hc.card.MaxHP >= Settings.MinHealthAsTank)
                    {

                        // TODO: Analyse which is the most dangerous line
                        if (GetNearestEnemy(p)?.Line == 2)
                        {
                            Logger.Debug("KT RightUp");
                            VectorAI v = p.getDeployPosition(p.ownKingsTower.Position, deployDirectionRelative.RightUp, 100);
                            return v;
                        }
                        else
                        {
                            Logger.Debug("KT LeftUp");
                            VectorAI v = p.getDeployPosition(p.ownKingsTower.Position, deployDirectionRelative.LeftUp, 100);
                            return v;
                        }
                    }
                }
                catch (Exception) { }
            
                if (hc.card.Transport == transportType.AIR)
                {
                    // TODO: Analyse which is the most dangerous line
                    if (GetNearestEnemy(p)?.Line == 2)
                        return p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine2);
                    else
                        return p.getDeployPosition(deployDirectionAbsolute.ownPrincessTowerLine1);
                }
                else
                {
                    if (GetNearestEnemy(p)?.Line == 2)
                    {
                        Logger.Debug("BehindKT: Line2");
                        VectorAI position = p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine2);
                        return position;
                    }
                    else
                    {
                        Logger.Debug("BehindKT: Line1");
                        VectorAI position = p.getDeployPosition(deployDirectionAbsolute.behindKingsTowerLine1);
                        return position;
                    }
                }
            }
            else if (hc.card.type == boardObjType.BUILDING)
            {
                //switch ((cardToDeploy as CardBuilding).Type)
                //{
                //    case BuildingType.BuildingDefense:
                //    case BuildingType.BuildingSpawning:
                return GetPositionOfTheBestBuildingDeploy(p);
                //}
            }
            else if (hc.card.type == boardObjType.AOE || hc.card.type == boardObjType.PROJECTILE)
                return GetPositionOfTheBestDamagingSpellDeploy(p);
            else
            {
                Logger.Debug("DKT: Handcard equals NONE!");
                return p.ownKingsTower?.Position;
            }

        }
        private static VectorAI DLPT(Playfield p, Handcard hc)
        {
            BoardObj lPT = p.ownPrincessTower1;

            if (lPT == null || lPT.Position == null)
                return DKT(p, hc);

            VectorAI lPTP = lPT.Position;
            VectorAI correctedPosition = PrincessTowerCharacterDeploymentCorrection(lPTP, p, hc);
            return correctedPosition;
        }
        private static VectorAI DRPT(Playfield p, Handcard hc)
        {
            BoardObj rPT = p.ownPrincessTower2;

            if (rPT == null && rPT.Position == null)
                return DKT(p, hc);

            VectorAI rPTP = rPT.Position;
            VectorAI correctedPosition = PrincessTowerCharacterDeploymentCorrection(rPTP, p, hc);
            return correctedPosition;
        }
        #endregion

        #region Attack
        private static VectorAI AKT(Playfield p)
        {
            Logger.Debug("AKT");

            if (p.enemyPrincessTowers.Count == 2)
            {
                if (p.enemyPrincessTower1.HP < p.enemyPrincessTower2.HP)
                    return p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);
                else
                    return p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);
            }

            if (p.enemyPrincessTower1.HP == 0)
                return p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);

            if (p.enemyPrincessTower2.HP == 0)
                return p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);

            if (p.enemyPrincessTowers.Count == 0)
                return p.enemyKingsTower?.Position;

            return p.enemyKingsTower?.Position;
        }
        private static VectorAI ALPT(Playfield p)
        {
            Logger.Debug("ALPT");

            VectorAI lPT = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine1);
            return lPT;
        }
        private static VectorAI ARPT(Playfield p)
        {
            Logger.Debug("ARPT");

            VectorAI rPT = p.getDeployPosition(deployDirectionAbsolute.enemyPrincessTowerLine2);
            return rPT;
        }
        #endregion

        public static VectorAI GetPositionOfTheBestDamagingSpellDeploy(Playfield p)
        {
            // Prio1: Hit Enemy King Tower if health is low
            // Prio2: Every damaging spell if there is a big group of enemies
            Logger.Debug("GetPositionOfTheBestDamaingSpellDeploy");

            try
            {
                if (p.enemyKingsTower?.HP < Settings.KingTowerSpellDamagingHealth || (p.enemyMinions.Count + p.enemyBuildings.Count) < 1)
                    return p.enemyKingsTower?.Position;
            } catch(Exception)
            
            {
                int count;
                BoardObj enemy = EnemyCharacterWithTheMostEnemiesAround(p, out count, transportType.NONE);

                if (enemy != null && enemy.Position != null)
                {
                    try
                    {
                        if (HowManyNFCharactersAroundCharacter(p, enemy) >= Settings.SpellCorrectionConditionCharCount)
                        {
                            Logger.Debug("enemy.Name = {Name}", enemy.Name);
                            if (enemy.Position != null) Logger.Debug("enemy.Position = {position}", enemy.Position);

                            return enemy.Position;
                        }
                    } catch(Exception)

                    {
                        //enemy.Position.AddYInDirection(p, 3000); // Position Correction
                        VectorAI result = p.getDeployPosition(enemy.Position, deployDirectionRelative.Down, 500);

                        Logger.Debug("enemy.Name = {Name}", enemy.Name);
                        if (enemy.Position != null) Logger.Debug("enemy.Position = {position}", enemy.Position);
                        Logger.Debug("result = {position}", result);

                        return result;
                    }
                }
                Logger.Debug("enemy = null?{enemy} ; enemy.position = null?{position}", enemy == null, enemy.Position == null);
            }

            Logger.Debug("Error: 0/0");
            return new VectorAI(0, 0);
        }

        public static VectorAI GetPositionOfTheBestBuildingDeploy(Playfield p)
        {
            // ToDo: Find the best position
            VectorAI betweenBridges = p.getDeployPosition(deployDirectionAbsolute.betweenBridges);
            VectorAI result = p.getDeployPosition(betweenBridges, deployDirectionRelative.Down, 4000);
            return result;
        }

        private static VectorAI PrincessTowerCharacterDeploymentCorrection(VectorAI position, Playfield p, Handcard hc)
        {
            if (hc == null || hc.card == null || position == null)
                return null;

            //Logger.Debug("PT Characer Position Correction: Name und Typ {0} " + cardToDeploy.Name, (cardToDeploy as CardCharacter).Type);
            if (hc.card.type == boardObjType.MOB)
            {
                if (hc.card.MaxHP >= Settings.MinHealthAsTank)
                {
                    //position.SubtractYInDirection(p);
                    return p.getDeployPosition(position, deployDirectionRelative.Up, 100);
                }
                else
                {
                    //position.AddYInDirection(p);
                    return p.getDeployPosition(position, deployDirectionRelative.Down, 2000);
                }
            }
            else if (hc.card.type == boardObjType.BUILDING)
                return GetPositionOfTheBestBuildingDeploy(p);
            else
                Logger.Debug("Tower Correction: No Correction!!!");

            return position;
        }
        #endregion


        #region Helper
        public static int? HowManyCharactersAroundCharacter(Playfield p, BoardObj obj)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> playerCharacter = p.ownMinions;
            IEnumerable<BoardObj> characterAround;

            characterAround = playerCharacter.Where(n => n.Position.X > obj.Position.X - boarderX
                                            && n.Position.X < obj.Position.X + boarderX &&
                                            n.Position.Y > obj.Position.Y - boarderY &&
                                            n.Position.Y < obj.Position.Y + boarderY);

            if (characterAround == null)
                return null;

            return characterAround.Count();
        }

        // NF = not flying
        public static int? HowManyNFCharactersAroundCharacter(Playfield p, BoardObj obj)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> playerCharacter = p.ownMinions;
            IEnumerable<BoardObj> characterAround;

            characterAround = playerCharacter.Where(n => n.Position.X > obj.Position.X - boarderX
                                            && n.Position.X < obj.Position.X + boarderX &&
                                            n.Position.Y > obj.Position.Y - boarderY &&
                                            n.Position.Y < obj.Position.Y + boarderY && n.card.Transport == transportType.GROUND);

            if (characterAround == null)
                return null;

            return characterAround.Count();
        }

        public static BoardObj EnemyCharacterWithTheMostEnemiesAround(Playfield p, out int count, transportType tP)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> enemies = p.enemyMinions;
            IEnumerable<BoardObj> enemiesAroundTemp;
            BoardObj enemy = null;
            count = 0;

            foreach (var item in enemies)
            {
                if (tP != transportType.NONE)
                {
                    enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                                    && n.Position.X < item.Position.X + boarderX &&
                                                    n.Position.Y > item.Position.Y - boarderY &&
                                                    n.Position.Y < item.Position.Y + boarderY && n.Transport == tP);
                }else
                {
                    enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                && n.Position.X < item.Position.X + boarderX &&
                                n.Position.Y > item.Position.Y - boarderY &&
                                n.Position.Y < item.Position.Y + boarderY);
                }

                if (enemiesAroundTemp?.Count() > count)
                {
                    count = enemiesAroundTemp.Count();
                    enemy = item;
                }
            }

            return enemy;
        }

        public static BoardObj GetNearestEnemy(Playfield p)
        {
            var nearestChar = p.enemyMinions;

            var orderedChar = nearestChar.OrderBy(n => n.Position.Y);

            if (p.home)
                return orderedChar.FirstOrDefault();
            else
                return orderedChar.LastOrDefault();
        }
        #endregion

        #region Classification
        private static IEnumerable<Handcard> GetOwnHandCards(Playfield p, boardObjType cardType, SpecificCardType sCardType)
        {
            IEnumerable<Handcard> cardsOfType = p.ownHandCards.Where(n => n.card.type == cardType);

            switch (sCardType)
            {
                case SpecificCardType.All:
                    return cardsOfType;

                // Mobs
                case SpecificCardType.MobsTank:
                    return cardsOfType.Where(n => n.card.MaxHP >= Settings.MinHealthAsTank);
                case SpecificCardType.MobsDamageDealer:
                    return cardsOfType.Where(n => (n.card.Atk * n.card.SummonNumber) > 100);
                case SpecificCardType.MobsBuildingAttacker:
                    return cardsOfType.Where(n => n.card.TargetType == targetType.BUILDINGS);

                case SpecificCardType.MobsRanger:
                    return cardsOfType.Where(n => n.card.MaxRange >= 3);
                case SpecificCardType.MobsAOEGround:
                    return cardsOfType.Where(n => n.card.aoeGround);
                case SpecificCardType.MobsAOEAll:
                    return cardsOfType.Where(n => n.card.aoeAir);
                case SpecificCardType.MobsFlyingAttack:
                    return cardsOfType.Where(n => n.card.TargetType == targetType.ALL);

                // Buildings
                case SpecificCardType.BuildingsDefense:
                    return cardsOfType.Where(n => n.card.Atk > 0); // TODO: Define
                case SpecificCardType.BuildingsAttack:
                    return cardsOfType.Where(n => n.card.Atk > 0); // TODO: Define
                case SpecificCardType.BuildingsSpawning:
                    return cardsOfType.Where(n => n.card.SpawnNumber > 0);
                case SpecificCardType.BuildingsMana:
                    break; // TODO: ManaProduction


                // Spells
                case SpecificCardType.SpellsDamaging:
                    return cardsOfType.Where(n => n.card.DamageRadius > 0);
                case SpecificCardType.SpellsNonDamaging:
                    return cardsOfType.Where(n => n.card.DamageRadius == 0);

            }

            return null;
        }
        #endregion

        // Question: For what is this?
        public override float GetPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            int retval = 0;
            return retval;
        }

        // Question: For what is this?
        public override int GetBoValue(BoardObj bo, Playfield p)
        {
            int retval = 5;
            return retval;
        }

        // Question: the penality depends on the card and the actual playfield situation?
        public override int GetPlayCardPenalty(CardDB.Card card, Playfield p)
        {
            return 0;
        }
    }

    public static class BoardObjExtension
    {
        public static bool IsPositionInArea(this BoardObj bo, Playfield p, VectorAI position)
        {
            Log.Debug("Building: Name = {0} Range = {1}", bo.Name, bo.Range);
            bool isInArea = position.X >= bo.Position.X - bo.Range &&
                            position.X <= bo.Position.X + bo.Range &&
                            position.Y >= bo.Position.Y - bo.Range &&
                            position.Y <= bo.Position.Y + bo.Range;

            return isInArea;
        }
    }
}