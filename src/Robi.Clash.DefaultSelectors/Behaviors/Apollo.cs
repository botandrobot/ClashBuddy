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

	public class Apollo : BehaviorBase
	{
		private static readonly ILogger Logger = LogProvider.CreateLogger<Apollo>();
		public static bool GameBeginning = true;

		#region
		public override string Name => "Apollo";

		public override string Description => "1vs1; Please lean back and let me Apollo do the work...";

		public override string Author => "Peros_";

		public override Version Version => new Version(1, 4, 0, 0);
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

        public override Cast GetBestCast(Playfield p)
        {
            Cast bc = null;
            Logger.Debug("Home = {Home}", p.home);
            // Peros: Contains mobs, buildings and towers
            //group ownGroup = p.getGroup(true, 85, boPriority.byTotalNumber, 3000);

            #region Apollo Magic
            Logger.Debug("Part: Get CurrentSituation");
            FightState currentSituation = CurrentFightState(p);
            Logger.Debug("Part: SpellMagic");
            Handcard hc = SpellMagic(p, currentSituation);

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

        private static Handcard SpellMagic(Playfield p, FightState currentSituation)
        {
            //opposite oppo = KnowledgeBase.Instance.getOppositeCardToAll(p, myObj);
            //Handcard opposite = KnowledgeBase.Instance.getOppositeCard(Playfield p, BoardObj attacker, bool canWait = true);
            switch (currentSituation)
            {
                case FightState.UAKT:
                case FightState.UALPT:
                case FightState.UARPT:
                    return GetBestUnderAttackCard(p);
                case FightState.AKT:
                case FightState.ALPT:
                case FightState.ARPT:
                    return GetBestAttackCard(p);
                case FightState.DKT:
                case FightState.DLPT:
                case FightState.DRPT:
                    return GetBestDefenseCard(p);
                case FightState.START:
                    return null;
                case FightState.WAIT:
                    return null;
                default:
                    return GetBestAttackCard(p);
            }
        }

        #region Get Best Card To Deploy
        private static Handcard GetBestUnderAttackCard(Playfield p)
        {
            // TODO: Find most important char to counter
            // Priority1: highest atk

            // ## Old
            //IOrderedEnumerable<BoardObj> enemies = p.enemyMinions.OrderBy(n => n.Atk + ((n.level * 0.1) * n.Atk));
            //BoardObj enemy = enemies.FirstOrDefault();

            //if (enemy != null)
            //{
            //    Logger.Debug("Enemy in GetBestUnderAttackCard");
            //    Logger.Debug(enemy.ToString());
            //    Handcard spell = KnowledgeBase.Instance.getOppositeCard(p, enemy);

            //    if (spell != null)
            //    {
            //        if (spell.missingMana > 0)
            //            return null;
            //        else
            //            return spell;
            //    }
            //    else
            //        return DefenseTroop(p);
            //}
            //else
            //    return DefenseTroop(p);

            Logger.Debug("Path: Spell - GetBestUnderAttackCard");
            BoardObj defender = GetBestDefender(p);

            if (defender == null)
                return null;
            else if (defender.Name.ToString() == "princesstower" || defender.Name.ToString() == "kingtower")
                return DefenseTroop(p);

            Logger.Debug("BestDefender: {Defender}", defender.Name);
            opposite spell = KnowledgeBase.Instance.getOppositeToAll(p, defender, true);

            if (spell != null && spell.hc != null)
            {
                Logger.Debug("Spell: {Sp} - MissingMana: {MM}",  spell.hc.name, spell.hc.missingMana);
                if (spell.hc.missingMana > 0)
                    return null;
                else
                    return spell.hc;
            }
            else
                return DefenseTroop(p);
        }

        private static BoardObj GetBestDefender(Playfield p)
        {
            Logger.Debug("Path: Spell - GetBestDefender");
            int count = 0;
            BoardObj enemy = EnemyCharacterWithTheMostEnemiesAround(p, out count);

            if (enemy == null)
                return p.ownKingsTower;

            if (enemy.Line == 2)
                return p.ownPrincessTower2;
            else if (enemy.Line == 1)
                return p.ownPrincessTower1;
            else
                return p.ownKingsTower;
        }

        private static Handcard GetBestAttackCard(Playfield p)
        {
            Logger.Debug("Path: Spell - GetBestAttackCard");
            // TODO: Find most important char to counter
            BoardObj defender = GetBestDefender(p);

            if (defender == null)
                return null;
            else if (defender.Name.ToString() == "princesstower" || defender.Name.ToString() == "kingtower")
                return All(p);

            opposite spell = KnowledgeBase.Instance.getOppositeToAll(p, defender, true);

            if (spell != null && spell.hc != null)
            {
                Logger.Debug("Spell: {Sp} - MissingMana: {MM}", spell.hc.name, spell.hc.missingMana);
                if (spell.hc.missingMana > 0)
                    return null;
                else
                    return spell.hc;
            }
            else
                return All(p);
        }

        private static Handcard GetBestDefenseCard(Playfield p)
        {
            Logger.Debug("Path: Spell - GetBestDefenseCard");
            BoardObj defender = GetBestDefender(p);

            if (defender == null)
                return null;
            else if (defender.Name.ToString() == "princesstower" || defender.Name.ToString() == "kingtower")
                return Defense(p);

            opposite spell = KnowledgeBase.Instance.getOppositeToAll(p, defender, true);

            if (spell != null && spell.hc != null)
            {
                Logger.Debug("Spell: {Sp} - MissingMana: {MM}", spell.hc.name, spell.hc.missingMana);
                if (spell.hc.missingMana > 0)
                    return null;
                else
                    return spell.hc;
            }
            else
                return Defense(p);
        }
        #endregion

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
            catch(Exception e)
            {
                return GetCurrentFightStateBalanced(p);
            }

        }

        private static FightState GetCurrentFightStateBalanced(Playfield p)
        {
            FightState fightState = FightState.WAIT;

            if (GameBeginning)
                return GameBeginningDecision(p);

            if (!p.noEnemiesOnMySide())
                fightState = EnemyIsOnOurSideDecision(p);
            else if (p.ownMana >= Settings.ManaTillAttack || (p.ownMinions.Count > 0))
            {
                if (p.enemyMinions.Count > 2) // ToDo: CHeck more (Health, Damage etc) 
                    fightState = EnemyHasCharsOnTheFieldDecision(p);
                else
                    fightState = AttackDecision(p);
            }

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

        #region Decisions
        private static FightState DefenseDecision(Playfield p)
        {
            if (p.ownTowers.Count < 3)
                return FightState.DKT;

            BoardObj princessTower = p.enemyTowers.OrderBy(n => n.HP).FirstOrDefault();

            if (princessTower.Line == 2)
                return FightState.DRPT;
            else
                return FightState.DLPT;
        }

        private static FightState EnemyHasCharsOnTheFieldDecision(Playfield p)
        {
            if (p.enemyTowers.Count > 2)
            {
                BoardObj obj = GetNearestEnemy(p);

                if (obj.Line == 2)
                    return FightState.DRPT;
                else
                    return FightState.DLPT;
            }
            else
            {
                return FightState.DKT;
            }
        }

        private static FightState EnemyIsOnOurSideDecision(Playfield p)
        {
            if (p.ownTowers.Count > 2) // Question: If in ownTowers the KT is not included change to > 1
            {
                BoardObj obj = GetNearestEnemy(p);

                if (obj.Line == 2)
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


            BoardObj princessTower = p.enemyTowers.OrderBy(n => n.HP).FirstOrDefault();

            if (princessTower.Line == 2)
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
            catch(Exception e)
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

        private static Handcard All(Playfield p)
        {
            Logger.Debug("Path: Spell - All");
            IOrderedEnumerable<Handcard> troopCycleSpells = cycleCard(p);
            IEnumerable<Handcard> damagingSpells = p.ownHandCards.Where(s => s != null && s.card.type == boardObjType.AOE);
            IEnumerable<Handcard> troopPowerSpells = p.ownHandCards.Where(s => s != null && s.card.type == boardObjType.AOE);

            Handcard resultHC = new Handcard();

            if (DamagingSpellDecision(p))
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    //return new CardSpell(damagingSpell.Name.Value, SpellType.SpellDamaging);
                    return new Handcard(damagingSpell.name, damagingSpell.lvl);
            }

            if (IsAOEAttackNeeded(p))
            {
                var atkAOE = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB).FirstOrDefault(); // Todo: just AOE-Attack
                if (atkAOE == null)
                    atkAOE = p.ownHandCards.Where(n => n.card.TargetType == targetType.GROUND).FirstOrDefault();

                if (atkAOE != null)
                    //return new CardCharacter(spell.Name.Value, TroopType.AOEAttackGround);
                    return new Handcard(atkAOE.name, atkAOE.lvl);
            }

            if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
            {
                var atkFlying = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB && n.card.TargetType == targetType.ALL).FirstOrDefault(); // Peros: Not sure if targetType.All is right
                if (atkFlying != null)
                    return new Handcard(atkFlying.name, atkFlying.lvl);
            }

            if (DeployBuildingDecision())
            {
                // ToDo: Take right building and set right Building-Type
                var buildingCard = p.ownHandCards.Where(n => n.card.type == boardObjType.BUILDING).FirstOrDefault();
                if (buildingCard != null)
                    return new Handcard(buildingCard.name, buildingCard.lvl);
            }

            if (cycleCard(p).Count() > 1)
            {
                var troopCycle = troopCycleSpells.FirstOrDefault();
                if (troopCycle != null)
                    return new Handcard(troopCycle.name, troopCycle.lvl);
            }

            var powerSpell = powerCard(p).FirstOrDefault();
            if (powerSpell != null)
                return new Handcard(powerSpell.name, powerSpell.lvl);

            return p.ownHandCards.FirstOrDefault();
        }

        private static Handcard DefenseTroop(Playfield p)
        {
            Logger.Debug("Path: Spell - DefenseTroop");

            if (IsAOEAttackNeeded(p))
            {
                var atkAOE = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB).FirstOrDefault(); // Todo: just AOE-Attack
                if (atkAOE == null)
                    atkAOE = p.ownHandCards.Where(n => n.card.TargetType == targetType.GROUND).FirstOrDefault();

                if (atkAOE != null)
                    //return new CardCharacter(spell.Name.Value, TroopType.AOEAttackGround);
                    return new Handcard(atkAOE.name, atkAOE.lvl);
            }

            if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
            {
                var atkFlying = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB && n.card.TargetType == targetType.ALL).FirstOrDefault(); // Peros: Not sure if targetType.All is right
                if (atkFlying != null)
                    return new Handcard(atkFlying.name, atkFlying.lvl);
            }

            var powerSpell = powerCard(p).FirstOrDefault();
            if (powerSpell != null)
                return new Handcard(powerSpell.name, powerSpell.lvl);

            return cycleCard(p).FirstOrDefault();
        }

        private static Handcard Defense(Playfield p)
        {
            Logger.Debug("Path: Spell - Defense");
            IEnumerable<Handcard> damagingSpells = p.ownHandCards.Where(s => s != null && s.card.type == boardObjType.AOE);

            if (DamagingSpellDecision(p))
            {
                var damagingSpell = damagingSpells.FirstOrDefault();

                if (damagingSpell != null)
                    //return new CardSpell(damagingSpell.Name.Value, SpellType.SpellDamaging);
                    return new Handcard(damagingSpell.name, damagingSpell.lvl);
            }

            if (IsAOEAttackNeeded(p))
            {
                var atkAOE = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB).FirstOrDefault(); // Todo: just AOE-Attack
                if (atkAOE == null)
                    atkAOE = p.ownHandCards.Where(n => n.card.TargetType == targetType.GROUND).FirstOrDefault();

                if (atkAOE != null)
                    //return new CardCharacter(spell.Name.Value, TroopType.AOEAttackGround);
                    return new Handcard(atkAOE.name, atkAOE.lvl);
            }

            if (p.enemyMinions.Where(n => n.Transport == transportType.AIR).Count() > 0)
            {
                var atkFlying = p.ownHandCards.Where(n => n.card.type == boardObjType.MOB && n.card.TargetType == targetType.ALL).FirstOrDefault(); // Peros: Not sure if targetType.All is right
                if (atkFlying != null)
                    return new Handcard(atkFlying.name, atkFlying.lvl);
            }

            var cycleSpell = cycleCard(p).FirstOrDefault();
            if (cycleSpell != null)
                return new Handcard(cycleSpell.name, cycleSpell.lvl);

            return p.ownHandCards.FirstOrDefault();
        }


        private static Handcard Building(Playfield p)
        {
            Logger.Debug("Path: Spell - Building");
            var buildingCard = p.ownHandCards.Where(n => n.card.type == boardObjType.BUILDING).FirstOrDefault();
            if (buildingCard != null)
                return new Handcard(buildingCard.name, buildingCard.lvl);

            return null;
        }


        public static bool DamagingSpellDecision(Playfield p)
        {
            int count = 0;
            EnemyCharacterWithTheMostEnemiesAround(p, out count);

            /*
            Logger.Debug("enemyWhithTheMostEnemiesAround-Count: {count} enemy-Name {name}", count
                         , enemy.LogicGameObjectData.Name.Value);
                         */
            if (count > Settings.SpellDeployConditionCharCount)
                return true;

            return false;
        }

        public static bool DeployBuildingDecision()
        {
            // ToDo: Find gut conditions
            return true;
        }

        private static bool IsAOEAttackNeeded(Playfield p)
        {
            int biggestEnemieGroupCount;
            BoardObj obj = EnemyCharacterWithTheMostEnemiesAround(p, out biggestEnemieGroupCount);

            if (biggestEnemieGroupCount > 3)
                return true;

            return false;
        }
    #endregion

        #region Which Card

        #endregion

        #region Which Position

        public static VectorAI GetNextSpellPosition(FightState gameState, Handcard hc, Playfield p)
        {
            #region Randomise
            Random rnd = new Random();
            int rndX = rnd.Next(-Settings.RandomDeploymentValue, Settings.RandomDeploymentValue);
            int rndY = rnd.Next(-Settings.RandomDeploymentValue, Settings.RandomDeploymentValue);
            VectorAI rndAddVector = new VectorAI(rndX, rndY);
            #endregion

            if (hc == null || hc.card == null)
                return null;

            VectorAI choosedPosition = new VectorAI(0, 0), nextPosition;


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
            Vector2 v = (choosedPosition.ToVector2() + rndAddVector.ToVector2());
            nextPosition = new VectorAI(v.X, v.Y);
            //Logger.Debug("nextPosition: " + nextPosition);

            return nextPosition;
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
                if (hc.card.MaxHP >= Settings.MinHealthAsTank)
                {
                    //Logger.Debug("DKT Troop-Name {0} ; CartType GroundAttack, Flying or Tank", cardToDeploy.Name);
                    if (GetNearestEnemy(p)?.Line == 2)
                    {
                        //VectorAI v = new VectorAI(p.ownKingsTower.Position.X + 1000, p.enemyKingsTower.Position.Y);
                        VectorAI v = p.getDeployPosition(p.ownKingsTower.Position, deployDirection.RightUp, 100);
                        return v;
                    }
                    else
                    {
                        //VectorAI v = new VectorAI(p.ownKingsTower.Position.X - 1000, p.enemyKingsTower.Position.Y);
                        VectorAI v = p.getDeployPosition(p.ownKingsTower.Position, deployDirection.LeftUp, 100);
                        return v;
                    }
                }
                else
                {
                    //p.ownKingsTower.Position.AddYInDirection(p);
                    //VectorAI position = p.getDeployPosition(p.ownKingsTower.Position, deployDirection.Down, 1000);

                    if (GetNearestEnemy(p)?.Line == 2)
                    {
                        //VectorAI position = new VectorAI(position.X + 300, position.Y);
                        VectorAI position = p.getDeployPosition(deployDirection.behindKingsTowerLine2);
                        return position;
                    }
                    else
                    {
                        //VectorAI position = new VectorAI(position.X - 300, position.Y);
                        VectorAI position = p.getDeployPosition(deployDirection.behindKingsTowerLine1);
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
            else if(hc.card.type == boardObjType.AOE || hc.card.type == boardObjType.PROJECTILE)
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

            //Logger.Debug("DLPT: LeftPrincessTower = " + lPT.ToString());
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

            if (p.enemyTowers?.Count() > 2)
                //Logger.Debug("Bug: NoPrincessTowerDown-State in Attack-King-Tower-State!");
                return p.getDeployPosition(deployDirection.enemyPrincessTowerLine1);

            if (p.enemyTowers?.Where(n => n.Line == 1).Count() == 0)
                //Logger.Debug("LPTD");
                return p.getDeployPosition(deployDirection.enemyPrincessTowerLine1);

            if (p.enemyTowers?.Where(n => n.Line == 2).Count() == 0)
                //Logger.Debug("RPTD");
                return p.getDeployPosition(deployDirection.enemyPrincessTowerLine2);

            if (p.enemyTowers?.Count() == 1)
                //Logger.Debug("BPTD");
                return p.enemyKingsTower?.Position;

            return p.enemyKingsTower?.Position;
        }
        private static VectorAI ALPT(Playfield p)
        {
            Logger.Debug("ALPT");

            VectorAI lPT = p.getDeployPosition(deployDirection.enemyPrincessTowerLine1, 70);
            return lPT;
        }
        private static VectorAI ARPT(Playfield p)
        {
            Logger.Debug("ARPT");

            VectorAI rPT = p.getDeployPosition(deployDirection.enemyPrincessTowerLine2, 70);
            return rPT;
        }
        #endregion

        public static VectorAI GetPositionOfTheBestDamagingSpellDeploy(Playfield p)
        {
            // Prio1: Hit Enemy King Tower if health is low
            // Prio2: Every damaging spell if there is a big group of enemies
            Logger.Debug("GetPositionOfTheBestDamaingSpellDeploy");

            if (p.enemyKingsTower?.HP < Settings.KingTowerSpellDamagingHealth)
                return p.enemyKingsTower?.Position;
            else
            {
                int count;
                BoardObj enemy = EnemyCharacterWithTheMostEnemiesAround(p, out count);

                if (enemy != null && enemy.Position != null)
                {
                    //LC: also you can try group Group = new group(false, enemy.Position, p.enemyMinions, lowHPlimit, false, radius); and get the full characteristics of the group for deciding whether the spell will be effective against this group or not (hint: set lowHPlimit = hc.card.Atk, radius = hc.card.DamageRadius)
                    if (HowManyCharactersAroundCharacter(p, enemy) >= Settings.SpellCorrectionConditionCharCount)
                        return enemy.Position;
                    else
                    {
                        //enemy.Position.AddYInDirection(p, 3000); // Position Correction
                        VectorAI result = p.getDeployPosition(enemy.Position, deployDirection.Down, 2000); // TODO: Have to look about target speed and dircetion
                        return result;
                    }
                }
                Logger.Debug("enemy = null?{enemy} ; enemy.position = null?{position}", enemy == null, enemy.Position == null);
            }

            
            return new VectorAI(0, 0);
        }

        public static VectorAI GetPositionOfTheBestBuildingDeploy(Playfield p)
        {
            // ToDo: Find the best position
            VectorAI betweenBridges = p.getDeployPosition(deployDirection.betweenBridges,0);
            VectorAI result = p.getDeployPosition(betweenBridges, deployDirection.Down, 4000);
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
                    return p.getDeployPosition(position, deployDirection.Up, 100);
                }
                else
                {
                    //position.AddYInDirection(p);
                    return p.getDeployPosition(position, deployDirection.Down, 2000);
                }
            }
            else if (hc.card.type == boardObjType.BUILDING)
                return GetPositionOfTheBestBuildingDeploy(p);
            else
                Logger.Debug("Tower Correction: No Correction!!!");

            return position;
        }
        #endregion


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

        public static BoardObj EnemyCharacterWithTheMostEnemiesAround(Playfield p, out int count)
        {
            int boarderX = 1000;
            int boarderY = 1000;
            IEnumerable<BoardObj> enemies = p.enemyMinions;
            IEnumerable<BoardObj> enemiesAroundTemp;
            BoardObj enemy = null;
            count = 0;

            foreach (var item in enemies)
            {
                enemiesAroundTemp = enemies.Where(n => n.Position.X > item.Position.X - boarderX
                                                && n.Position.X < item.Position.X + boarderX &&
                                                n.Position.Y > item.Position.Y - boarderY &&
                                                n.Position.Y < item.Position.Y + boarderY);

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
}