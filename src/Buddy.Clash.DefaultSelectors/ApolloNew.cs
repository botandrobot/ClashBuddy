namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Text;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Engine;
    using Common;
    using Serilog;
    using System.IO;
    using Buddy.Engine.Settings;
    using Buddy.Clash.DefaultSelectors.Settings;

    public class ApolloNew : ActionSelectorBase
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<DefaultRoutine>();
        private readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();
        public static bool GameBeginning = true;

        internal static ApolloSettings Settings { get; } = new ApolloSettings();

        Helpfunctions help = Helpfunctions.Instance;

        DateTime starttime = DateTime.Now;
        
        //Behavior behave = new BehaviorControl();

        #region Implementation of IAuthored

        /// <summary> The name of the routine. </summary>
        public override string Name
        {
            get { return "ApolloNew"; }
        }

        /// <summary> The description of the routine. </summary>
        public override string Description
        {
            get { return "The default routine for Clash Royale."; }
        }

        /// <summary>The author of this routine.</summary>
        public override string Author
        {
            get { return "Peros"; }
        }

        /// <summary>The version of this routine.</summary>
        public override Version Version
        {
            get { return new Version(0, 0, 0, 1); }
        }

        /// <summary>Unique Identifier.</summary>
        public override Guid Identifier
        {
            get { return new Guid("{e5b7756b-36e8-4c7f-a97b-b910318ec3e1}"); }
        }

        #endregion

        #region Implementation of IRunnable
        /*
        /// <summary> The routine start callback. Do any initialization here. </summary>
        public void Start()
        {
            GameEventManager.NewGame += GameEventManagerOnNewGame;
            GameEventManager.GameOver += GameEventManagerOnGameOver;
            GameEventManager.QuestUpdate += GameEventManagerOnQuestUpdate;
            GameEventManager.ArenaRewards += GameEventManagerOnArenaRewards;

            if (Hrtprozis.Instance.settings == null)
            {
                Hrtprozis.Instance.setInstances();
                ComboBreaker.Instance.setInstances();
                PenalityManager.Instance.setInstances();
            }
            behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
            foreach (var tuple in _mulliganRules)
            {
                Exception ex;
                if (
                    !VerifyCondition(tuple.Item1, new List<string> { "mulliganData" }, out ex))
                {
                    Log.ErrorFormat("[Start] There is an error with a mulligan execution condition [{1}]: {0}.", ex,
                        tuple.Item1);
                    BotManager.Stop();
                }

                if (
                    !VerifyCondition(tuple.Item2, new List<string> { "mulliganData", "card" },
                        out ex))
                {
                    Log.ErrorFormat("[Start] There is an error with a mulligan card condition [{1}]: {0}.", ex,
                        tuple.Item2);
                    BotManager.Stop();
                }
            }
        }

        /// <summary> The routine tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The routine stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            GameEventManager.NewGame -= GameEventManagerOnNewGame;
            GameEventManager.GameOver -= GameEventManagerOnGameOver;
            GameEventManager.QuestUpdate -= GameEventManagerOnQuestUpdate;
            GameEventManager.ArenaRewards -= GameEventManagerOnArenaRewards;
        }
        */
        #endregion



        public override void Initialize()
        { 
            SettingsManager.RegisterSettings(Name, Settings);

            foreach(var e in Buddy.Clash.Engine.Csv.CsvLogic.Characters.Entries)
            {
                Logger.Information("{TID}: {Name} has {ShieldHitpoints}", e.TID, e.Name, e.ShieldHitpoints);
            }
            help.logg("-----------------Initialize");
            CardDB cdb = CardDB.Instance;
        }


        public override void BattleStart()
        {
            help.logg("-----------------BattleStart");
            GameBeginning = true;

        }

        public override void BattleEnd()
        {
            help.logg("-----------------BattleEnd");
        }

        public override void Deinitialize()
        {
            SettingsManager.UnregisterSettings(Name);
            help.logg("-----------------Deinitialize");
            CardDB.Instance.uploadCardInfo();
        }


        //it just concept for ActionSelector  (TODO: in future connect it with NN or other DB)
        public override CastRequest GetNextCast()
        {
            //var ssdf  = Buddy.Clash.Engine.Csv.CsvLogic.SpellsCharacters

            help.logg("###################entrance############### " + DateTime.Now);

            List<BoardObj> ownMinions = new List<BoardObj>();
            List<BoardObj> enemyMinions = new List<BoardObj>();

            List<BoardObj> ownAreaEffects = new List<BoardObj>();
            List<BoardObj> enemyAreaEffects = new List<BoardObj>();

            List<BoardObj> ownBuildings = new List<BoardObj>();
            List<BoardObj> enemyBuildings = new List<BoardObj>();

            List<BoardObj> ownTowers = new List<BoardObj>();
            List<BoardObj> enemyTowers = new List<BoardObj>();

            List<Handcard> ownHandCards = new List<Handcard>();


            var om = ClashEngine.Instance.ObjectManager;

            var battle = ClashEngine.Instance.Battle;
            if (battle == null || !battle.IsValid) return null;


            StringBuilder sb = new StringBuilder();
                        
            var lp = ClashEngine.Instance.LocalPlayer;
            var spells = ClashEngine.Instance.AvailableSpells;
            foreach (var spell in spells)
            {
                if (spell != null && spell.IsValid)
                {
                    int lvl = 1;
                    Handcard hc = new Handcard(spell.Name.Value, lvl); //hc.lvl = ??? TODO
                    hc.manacost = spell.ManaCost;
                    //hc.position = ??? TODO
                    ownHandCards.Add(hc);
                }

                /*i++;
                if (spell != null && spell.IsValid)
                {

                    var summonCharacter = spell.SummonCharacter;
                    var projectile = spell.Projectile;
                    var AreaEffect = spell.AreaEffect;
                    var BuffType = spell.BuffType;
                    var BuffOnDamage = spell.BuffType;
                    var Effect = spell.Effect;

                   /*sb.Clear();
                    sb.Append(" numN:").Append(i);
                    sb.Append(" Name:").Append(spell.Name.Value.ToString());
                    sb.Append(" ManaCost:").Append(spell.ManaCost);
                    sb.Append(" MultipleProjectiles:").Append(spell.MultipleProjectiles);
                    sb.Append(" SummonCharacterLevelIndex:").Append(spell.SummonCharacterLevelIndex);
                    sb.Append(" SummonNumber:").Append(spell.SummonNumber);
                    sb.Append(" SummonCharacterSecondCount:").Append(spell.SummonCharacterSecondCount);
                    sb.Append(" BuffNumber:").Append(spell.BuffNumber);
                    sb.Append(" CanDeployOnEnemySide:").Append(spell.CanDeployOnEnemySide);
                    sb.Append(" CanPlaceOnBuildings:").Append(spell.CanPlaceOnBuildings);
                    sb.Append(" CustomDeployTime:").Append(spell.CustomDeployTime);
                    sb.Append(" OnlyEnemies:").Append(spell.OnlyEnemies);
                    sb.Append(" OnlyOwnTroops:").Append(spell.OnlyOwnTroops);
                    sb.Append(" Height:").Append(spell.Height);
                    sb.Append(" HideRadiusIndicator:").Append(spell.HideRadiusIndicator);
                    sb.Append(" Mirror:").Append(spell.Mirror);
                    sb.Append(" Pushback:").Append(spell.Pushback);
                    sb.Append(" Radius:").Append(spell.Radius);
                    sb.Append(" SpellAsDeploy:").Append(spell.SpellAsDeploy);
                    sb.Append(" ProjectileIsValid:").Append(spell.Projectile != null && spell.Projectile.IsValid);
                    sb.Append(" SummonCharacterIsValid:").Append(spell.SummonCharacter != null && spell.SummonCharacter.IsValid);
                    sb.Append(" AreaEffectIsValid:").Append(spell.AreaEffect != null && spell.AreaEffect.IsValid);
                    sb.Append(" BuffOnDamageIsValid:").Append(spell.BuffOnDamage != null && spell.BuffOnDamage.IsValid);
                    sb.Append(" BuffTypeIsValid:").Append(spell.BuffType != null && spell.BuffType.IsValid);
                    sb.Append(" EffectIsValid:").Append(spell.Effect != null && spell.Effect.IsValid);
                    sb.Append(" StatsUnderInfo:").Append(spell.StatsUnderInfo);
                    sb.Append(" ElixirProductionStopTime:").Append(spell.ElixirProductionStopTime);
                    sb.Append(" NotInUse:").Append(spell.NotInUse);
                    sb.Append(" ManaCostFromSummonerMana:").Append(spell.ManaCostFromSummonerMana);


                    if (summonCharacter != null && summonCharacter.IsValid)
                    {
                        sb.Append("********SummonCharacter");
                        sb.Append(" Name:").Append(summonCharacter.Name.Value.ToString());
                        sb.Append(" AttacksAir:").Append(summonCharacter.AttacksAir);
                        sb.Append(" AttacksGround:").Append(summonCharacter.AttacksGround);
                        sb.Append(" CollisionRadius:").Append(summonCharacter.CollisionRadius);
                        sb.Append(" FlyDirectPaths:").Append(summonCharacter.FlyDirectPaths);
                        sb.Append(" FlyFromGround:").Append(summonCharacter.FlyFromGround);
                        sb.Append(" AbilityIsValid:").Append(summonCharacter.Ability != null && summonCharacter.Ability.IsValid);
                        sb.Append(" ActivationTime:").Append(summonCharacter.ActivationTime);
                        sb.Append(" AllTargetsHit:").Append(summonCharacter.AllTargetsHit);
                        sb.Append(" AppearEffectIsValid:").Append(summonCharacter.AppearEffect != null && summonCharacter.AppearEffect.IsValid);
                        sb.Append(" AppearPushback:").Append(summonCharacter.AppearPushback);
                        sb.Append(" AppearPushbackRadius:").Append(summonCharacter.AppearPushbackRadius);
                        sb.Append(" AreaBuffIsValid:").Append(summonCharacter.AreaBuff != null && summonCharacter.AreaBuff.IsValid);
                        sb.Append(" AreaBuffRadius:").Append(summonCharacter.AreaBuffRadius);
                        sb.Append(" AreaDamageRadius:").Append(summonCharacter.AreaDamageRadius);
                        sb.Append(" AreaEffectOnDashIsValid:").Append(summonCharacter.AreaEffectOnDash != null && summonCharacter.AreaEffectOnDash.IsValid);
                        sb.Append(" AreaEffectOnMorphIsValid:").Append(summonCharacter.AreaEffectOnMorph != null && summonCharacter.AreaEffectOnMorph.IsValid);
                        sb.Append(" AttachedCharacterHeight:").Append(summonCharacter.AttachedCharacterHeight);
                        sb.Append(" MultipleTargets:").Append(summonCharacter.MultipleTargets);
                    }

                    if (projectile != null && projectile.IsValid)
                    {
                        sb.Append("********Projectile");
                        sb.Append(" Name:").Append(projectile.Name.Value.ToString());
                        sb.Append(" AoeToAir:").Append(projectile.AoeToAir);
                        sb.Append(" AoeToGround:").Append(projectile.AoeToGround);
                        sb.Append(" SpawnCharacterLevelIndex:").Append(projectile.SpawnCharacterLevelIndex);
                    }

                    if (AreaEffect != null && AreaEffect.IsValid)
                    {
                        sb.Append("********AreaEffect");
                        sb.Append(" Name:").Append(AreaEffect.Name.Value.ToString());
                        sb.Append(" Radius:").Append(AreaEffect.Radius);
                        sb.Append(" LifeDuration:").Append(AreaEffect.LifeDuration);
                        sb.Append(" LifeDurationIncreasePerLevel:").Append(AreaEffect.LifeDurationIncreasePerLevel);
                        sb.Append(" NoEffectToCrownTowers:").Append(AreaEffect.NoEffectToCrownTowers);
                        sb.Append(" CrownTowerDamagePercent:").Append(AreaEffect.CrownTowerDamagePercent);
                        sb.Append(" MaximumTargets:").Append(AreaEffect.MaximumTargets);
                        sb.Append(" OneShotEffect:").Append(AreaEffect.OneShotEffect);
                        sb.Append(" OnlyEnemies:").Append(AreaEffect.OnlyEnemies);
                        sb.Append(" OnlyOwnTroops:").Append(AreaEffect.OnlyOwnTroops);
                        sb.Append(" Rarity_LevelCount:").Append(AreaEffect.Rarity.LevelCount);
                        sb.Append(" SpawnCharacterLevelIndex:").Append(AreaEffect.SpawnCharacterLevelIndex);
                        sb.Append(" HitsAir:").Append(AreaEffect.HitsAir);
                        sb.Append(" HitsGround:").Append(AreaEffect.HitsGround);
                        sb.Append(" HitSpeed:").Append(AreaEffect.HitSpeed);
                    }

                    if (BuffType != null && BuffType.IsValid)
                    {
                        sb.Append("********BuffType");
                        sb.Append(" Name:").Append(BuffType.Name.Value.ToString());
                        sb.Append(" SpeedMultiplier:").Append(BuffType.SpeedMultiplier);
                        sb.Append(" SpawnSpeedMultiplier:").Append(BuffType.SpawnSpeedMultiplier);
                    }

                    if (BuffOnDamage != null && BuffOnDamage.IsValid)
                    {
                        sb.Append("********BuffOnDamage");
                        sb.Append(" Name:").Append(BuffOnDamage.Name.Value.ToString());
                        sb.Append(" SpeedMultiplier:").Append(BuffOnDamage.SpeedMultiplier);
                        sb.Append(" SpawnSpeedMultiplier:").Append(BuffOnDamage.SpawnSpeedMultiplier);
                    }

                    if (Effect != null && Effect.IsValid)
                    {
                        sb.Append(" starter:").Append("********Effect");
                        sb.Append(" Name:").Append(Effect.Name.Value.ToString());
                        sb.Append(" Loop:").Append(Effect.Loop);
                    }

                    //help.logg(sb.ToString());


                }*/
            }

            //-sb.Append(" BattleSize:").Append(battle.BattleSize.ToString()); BattleSize:{36/64}
            //-sb.Append(" BattleLog:").Append(battle.BattleLog); BattleLog:3093098792 
            //-sb.Append(" ArenaTowers:").Append(battle.ArenaTowers.ToString()); не рассматривал т.к. там массив объектов
            //-sb.Append(" AvatarCount:").Append(battle.AvatarCount); AvatarCount: 2
            //-foreach (var id in battle.AvatarIds) sb.Append(" AvatarIds:").Append(id.ToString()); AvatarIds: 19 - 13798480 AvatarIds: 28 - 14257555 AvatarIds: 4294967294 - 4294967294 AvatarIds: 4294967294 - 4294967294
            //-sb.Append(" BattleSize:").Append(battle.Child); -
            //-sb.Append(" BattleSize:").Append(battle.Decks); -
            //-sb.Append(" BattleSize:").Append(battle.Location); -
            //-sb.Append(" BattleSize:").Append(battle.Npc); -
            //-sb.Append(" BattleSize:").Append(battle.NumberArray); -
            //-sb.Append(" BattleSize:").Append(battle.SummonerTowers); -

            
            var aoes = om.OfType<Engine.NativeObjects.Logic.GameObjects.AreaEffectObject>();
            foreach (var aoe in aoes)
            {
                if (aoe != null && aoe.IsValid)
                {
                    //TODO: get static data for all objects
                    //Here we get dynamic data only
                    BoardObj bo = new BoardObj(CardDB.Instance.cardNamestringToEnum(aoe.LogicGameObjectData.Name.Value));
                    bo.GId = aoe.GlobalId;
                    bo.Position = new VectorAI(aoe.StartPosition);
                    bo.Line = bo.Position.X > 8700 ? 1 : 2;
                    //bo.level = TODO real value
                    //bo.Atk = TODO real value
                    bo.LifeTime = aoe.HealthComponent.RemainingTime; //TODO check this value

                    //BoardObj bo = new BoardObj(aoe);
                    /*BoardObj bo = new BoardObj() {
                        Name = CardDB.Instance.cardNamestringToEnum(aoe.LogicGameObjectData.Name.Value),
                        Position = aoe.StartPosition,
                        GId = aoe.GlobalId,
                        //bo.level = TODO
                        type = boardObjType.AOE,
                        LifeTime = aoe.RemainingTime,
                        Line = aoe.StartPosition.X > 8700 ? 1 : 2,
                    };*/
                    bo.ownerIndex = (int)aoe.OwnerIndex;
                    bool own = bo.ownerIndex == lp.OwnerIndex ? true : false; //TODO: replace it on Friendly (for 2x2 mode)
                    bo.own = own;
                    if (own) ownAreaEffects.Add(bo);
                    else enemyAreaEffects.Add(bo);
                    //hc.position = ??? TODO

                    /*
                    sb.Clear();
                    sb.Append(" Health:").Append(aoe.HealthComponent.Health);
                    sb.Append(" CurrentHealth:").Append(aoe.HealthComponent.CurrentHealth);
                    sb.Append(" ShieldHealth:").Append(aoe.HealthComponent.ShieldHealth);
                    sb.Append(" CurrentShieldHealth:").Append(aoe.HealthComponent.CurrentShieldHealth);
                    sb.Append(" LifeTime:").Append(aoe.HealthComponent.LifeTime);
                    sb.Append(" RemainingTime:").Append(aoe.RemainingTime);
                    sb.Append(" RemainingTimeHC:").Append(aoe.HealthComponent.RemainingTime);
                    sb.Append(" Field10:").Append(aoe.HealthComponent.Field10);
                    */
                    //help.logg(bo, sb.ToString());

                }

                /*sb.Clear();
                var data = aoe.LogicGameObjectData;
                sb.Append(" OwnerIndex:").Append(aoe.OwnerIndex);
                sb.Append(" name:").Append(data.Name.Value);
                sb.Append(" StartPosition:").Append(aoe.StartPosition);
                sb.Append(" RemainingTime:").Append(aoe.RemainingTime);
                sb.Append(" Field5C:").Append(aoe.Field5C);
                sb.Append(" Field64:").Append(aoe.Field64);
                sb.Append(" Field68:").Append(aoe.Field68);
                sb.Append(" SpawnTime:").Append(data.SpawnTime);
                sb.Append(" SpawnMaxCount:").Append(data.SpawnMaxCount);
                sb.Append(" SpawnInterval:").Append(data.SpawnInterval);
                sb.Append(" SpawnInitialDelay:").Append(data.SpawnInitialDelay);
                sb.Append(" SpawnCharacterLevelIndex:").Append(data.SpawnCharacterLevelIndex);
                sb.Append(" Radius:").Append(data.Radius);
                sb.Append(" Pushback:").Append(data.Pushback);
                sb.Append(" ProjectileStartHeight:").Append(data.ProjectileStartHeight);
                sb.Append(" MaximumTargets:").Append(data.MaximumTargets);
                sb.Append(" LifeDurationIncreasePerLevel:").Append(data.LifeDurationIncreasePerLevel);
                sb.Append(" LifeDurationIncreaseAfterTournamentCap:").Append(data.LifeDurationIncreaseAfterTournamentCap);
                sb.Append(" LifeDuration:").Append(data.LifeDuration);
                sb.Append(" HitSpeed:").Append(data.HitSpeed);
                sb.Append(" CrownTowerDamagePercent:").Append(data.CrownTowerDamagePercent);
                sb.Append(" BuffTimeIncreasePerLevel:").Append(data.BuffTimeIncreasePerLevel);
                sb.Append(" BuffTimeIncreaseAfterTournamentCap:").Append(data.BuffTimeIncreaseAfterTournamentCap);
                sb.Append(" ProjectilesToCenter:").Append(data.ProjectilesToCenter);
                sb.Append(" OnlyOwnTroops:").Append(data.OnlyOwnTroops);
                sb.Append(" OnlyEnemies:").Append(data.OnlyEnemies);
                sb.Append(" NoEffectToCrownTowers:").Append(data.NoEffectToCrownTowers);
                sb.Append(" IgnoreBuildings:").Append(data.IgnoreBuildings);
                sb.Append(" HitsGround:").Append(data.HitsGround);
                sb.Append(" HitsAir:").Append(data.HitsAir);
                sb.Append(" HitBiggestTargets:").Append(data.HitBiggestTargets);
                sb.Append(" ControlsBuff:").Append(data.ControlsBuff);
                sb.Append(" Clone:").Append(data.Clone);
                sb.Append(" CapBuffTimeToAreaEffectTime:").Append(data.CapBuffTimeToAreaEffectTime);
                sb.Append(" AffectsHidden:").Append(data.AffectsHidden);

                var buff = data.Buff;
                sb.Append(" SpeedMultiplier:").Append(buff.SpeedMultiplier);
                sb.Append(" SpawnSpeedMultiplier:").Append(buff.SpawnSpeedMultiplier);
                sb.Append(" SizeMultiplier:").Append(buff.SizeMultiplier);
                sb.Append(" Scale:").Append(buff.Scale);
                sb.Append(" HitSpeedMultiplier:").Append(buff.HitSpeedMultiplier);
                sb.Append(" HitFrequency:").Append(buff.HitFrequency);
                sb.Append(" DamageReduction:").Append(buff.DamageReduction);
                sb.Append(" CrownTowerDamagePercent:").Append(buff.CrownTowerDamagePercent);
                sb.Append(" AudioPitchModifier:").Append(buff.AudioPitchModifier);
                sb.Append(" AttractPercentage:").Append(buff.AttractPercentage);
                sb.Append(" StaticTarget:").Append(buff.StaticTarget);
                sb.Append(" RemoveOnHeal:").Append(buff.RemoveOnHeal);
                sb.Append(" RemoveOnAttack:").Append(buff.RemoveOnAttack);
                sb.Append(" Panic:").Append(buff.Panic);
                sb.Append(" Invisible:").Append(buff.Invisible);
                sb.Append(" IgnorePushBack:").Append(buff.IgnorePushBack);
                sb.Append(" FilterInheritLifeDuration:").Append(buff.FilterInheritLifeDuration);
                sb.Append(" FilterAffectsTransformation:").Append(buff.FilterAffectsTransformation);
                sb.Append(" EnableStacking:").Append(buff.EnableStacking);
                sb.Append(" ControlledByParent:").Append(buff.ControlledByParent);
                sb.Append(" Clone:").Append(buff.Clone);
                sb.Append(" ChangeControl:").Append(buff.ChangeControl);
                
                var @proj = data.Projectile;
                sb.Append(" proj_AoeToAir:").Append(@proj.AoeToAir);
                sb.Append(" proj_AoeToGround:").Append(@proj.AoeToGround);
                sb.Append(" proj_BuffTimeIncreasePerLevel:").Append(@proj.BuffTimeIncreasePerLevel);
                sb.Append(" proj_ChainedHitRadius:").Append(@proj.ChainedHitRadius);
                sb.Append(" proj_ConstantHeight:").Append(@proj.ConstantHeight);
                sb.Append(" proj_CrownTowerDamagePercent:").Append(@proj.CrownTowerDamagePercent);
                sb.Append(" proj_CrownTowerHealPercent:").Append(@proj.CrownTowerHealPercent);
                sb.Append(" proj_DragBackSpeed:").Append(@proj.DragBackSpeed);
                sb.Append(" proj_DragMargin:").Append(@proj.DragMargin);
                sb.Append(" proj_Gravity:").Append(@proj.Gravity);
                sb.Append(" proj_HeightFromTargetRadius:").Append(@proj.HeightFromTargetRadius);
                sb.Append(" proj_Homing:").Append(@proj.Homing);
                sb.Append(" proj_MaxDistance:").Append(@proj.MaxDistance);
                sb.Append(" proj_MaximumTargets:").Append(@proj.MaximumTargets);
                sb.Append(" proj_MinDistance:").Append(@proj.MinDistance);
                sb.Append(" proj_OnlyEnemies:").Append(@proj.OnlyEnemies);
                sb.Append(" proj_OnlyOwnTroops:").Append(@proj.OnlyOwnTroops);
                sb.Append(" proj_PingpongVisualTime:").Append(@proj.PingpongVisualTime);
                sb.Append(" proj_ProjectileRadius:").Append(@proj.ProjectileRadius);
                sb.Append(" proj_ProjectileRadiusY:").Append(@proj.ProjectileRadiusY);
                sb.Append(" proj_Pushback:").Append(@proj.Pushback);
                sb.Append(" proj_PushbackAll:").Append(@proj.PushbackAll);
                sb.Append(" proj_Radius:").Append(@proj.Radius);
                sb.Append(" proj_RadiusY:").Append(@proj.RadiusY);
                sb.Append(" proj_RandomAngle:").Append(@proj.RandomAngle);
                sb.Append(" proj_RandomDistance:").Append(@proj.RandomDistance);
                sb.Append(" proj_Scale:").Append(@proj.Scale);
                sb.Append(" proj_ShadowDisableRotate:").Append(@proj.ShadowDisableRotate);
                sb.Append(" proj_ShakesTargets:").Append(@proj.ShakesTargets);
                sb.Append(" proj_SpawnCharacterDeployTime:").Append(@proj.SpawnCharacterDeployTime);
                sb.Append(" proj_SpawnCharacterDeployTime1:").Append(@proj.SpawnCharacterDeployTime1);
                sb.Append(" proj_SpawnCharacterLevelIndex:").Append(@proj.SpawnCharacterLevelIndex);
                sb.Append(" proj_SpawnConstPriority:").Append(@proj.SpawnConstPriority);
                sb.Append(" proj_Speed:").Append(@proj.Speed);
                sb.Append(" proj_TargetToEdge:").Append(@proj.TargetToEdge);
                sb.Append(" proj_Use360Frames:").Append(@proj.Use360Frames);

                //help.logg(sb.ToString());*/
            }


            var chars = om.OfType<Engine.NativeObjects.Logic.GameObjects.Character>();
            foreach (var @char in chars)
            {
                //sb.Clear();
                //i++;
                //BoardObj bo = new BoardObj();

                var data = @char.LogicGameObjectData;

                if (data != null && data.IsValid)
                {
                    //TODO: get static data for all objects
                    //Here we get dynamic data only

                    BoardObj bo = new BoardObj(CardDB.Instance.cardNamestringToEnum(data.Name.Value));
                    bo.GId = @char.GlobalId;
                    bo.Position = new VectorAI(@char.StartPosition);
                    bo.Line = bo.Position.X > 8700 ? 1 : 2;
                    //bo.level = TODO real value
                    //bo.Atk = TODO real value
                    //this.frozen = TODO
                    //this.startFrozen = TODO
                    bo.HP = @char.HealthComponent.CurrentHealth; //TODO: check it
                    bo.Shield = @char.HealthComponent.CurrentShieldHealth; //TODO: check it
                    bo.LifeTime = @char.HealthComponent.LifeTime - @char.HealthComponent.RemainingTime; //TODO: check it of data.LifeTime, - find real value for battle stage
                    switch (bo.Name)
                    {
                        case CardDB.cardName.princesstower: bo.Tower = bo.Line; break;
                        case CardDB.cardName.kingtower: bo.Tower = 10 + bo.Line; break;
                        case CardDB.cardName.kingtowermiddle: bo.Tower = 100; break;
                    }

                    //BoardObj bo = new BoardObj(@char);
                    /*Uncomment this then were DBs
                    BoardObj bo = new BoardObj()
                    {
                        Name = CardDB.Instance.cardNamestringToEnum(data.Name.Value),
                        Position = @char.StartPosition,
                        GId = @char.GlobalId,
                        Atk = 100, //Atk = ???????????? //TODO: no real value
                        HP = @char.HealthComponent.CurrentHealth,
                        Shield = @char.HealthComponent.ShieldHealth,
                        //bo.level = no real value
                        //this.frozen = TODO
                        //this.startFrozen = TODO
                        Tower = data.DeployTime == 0 ? 100 : 0, //TODO: values for towers
                        type = data.DeployTime == 0 ? boardObjType.BUILDING : data.LifeTime > 0 ? boardObjType.BUILDING : boardObjType.MOB,
                        LifeTime = data.LifeTime, //TODO: find real value for battle stage
                        Line = @char.StartPosition.X > 8700 ? 1 : 2,
                    };*/
                    bo.ownerIndex = (int)@char.OwnerIndex;
                    bool own = bo.ownerIndex == lp.OwnerIndex ? true : false; //TODO: replace it on Friendly (for 2x2 mode)
                    bo.own = own;
                    if (own)
                    {
                        switch (bo.type)
                        {
                            case boardObjType.MOB: ownMinions.Add(bo); break;
                            case boardObjType.BUILDING:
                                if (bo.Tower > 0) ownTowers.Add(bo);
                                else ownBuildings.Add(bo);
                                break;
                        }
                    }
                    else
                    {
                        switch (bo.type)
                        {
                            case boardObjType.MOB: enemyMinions.Add(bo); continue;
                            case boardObjType.BUILDING:
                                if (bo.Tower > 0) enemyTowers.Add(bo);
                                else enemyBuildings.Add(bo);
                                break;
                        }
                    }

                    /*
                    sb.Clear();
                    sb.Append(" Health:").Append(@char.HealthComponent.Health);
                    sb.Append(" CurrentHealth:").Append(@char.HealthComponent.CurrentHealth);
                    sb.Append(" ShieldHealth:").Append(@char.HealthComponent.ShieldHealth);
                    sb.Append(" CurrentShieldHealth:").Append(@char.HealthComponent.CurrentShieldHealth);
                    sb.Append(" LifeTime:").Append(@char.HealthComponent.LifeTime);
                    sb.Append(" RemainingTime:").Append(@char.HealthComponent.RemainingTime);
                    sb.Append(" Field10:").Append(@char.HealthComponent.Field10);
                    sb.Append(" Name:").Append(data.Name);
                    */

                    //help.logg(bo, sb.ToString());

                    /*var log = new
                    {
                        //numN = i,
                        OwnerIndex = @char.OwnerIndex,
                        Position = @char.StartPosition.ToString(),
                    };
                    var summonCharacter = data;
                    var charLog = new
                    {
                        starter = "********char",
                        GameObject = log,
                        Name = summonCharacter.Name.Value.ToString(),
                        AttacksAir = summonCharacter.AttacksAir,
                        AttacksGround = summonCharacter.AttacksGround,
                    };
                    Logger.Debug("{@charLog}", charLog);

                    sb.Append("own:").Append(@char.OwnerIndex);
                    sb.Append(" name:").Append(data.Name.Value);
                    sb.Append(" StartPosition:").Append(@char.StartPosition);

                    sb.Append(" TowerLevel:").Append(@char.TowerLevel);

                    sb.Append(" HideTimeMs:").Append(@char.HideTimeMs);
                    sb.Append(" HasSpeedComponent:").Append(@char.HasSpeedComponent);
                    sb.Append(" GrowTime:").Append(@char.GrowTime);

                    sb.Append(" GlobalId:").Append(@char.GlobalId);
                    sb.Append(" Mana:").Append(@char.Mana);
                    //sb.Append(" Parent:").Append(@char.Parent);
                    sb.Append(" Vtable:").Append(@char.Vtable);


                    sb.Append(" FieldCC:").Append(@char.FieldCC);
                    sb.Append(" FieldC8:").Append(@char.FieldC8);
                    sb.Append(" FieldC4:").Append(@char.FieldC4);
                    sb.Append(" FieldC0:").Append(@char.FieldC0);
                    sb.Append(" FieldBC:").Append(@char.FieldBC);
                    sb.Append(" FieldB8:").Append(@char.FieldB8);
                    sb.Append(" FieldB4:").Append(@char.FieldB4);
                    sb.Append(" FieldB0:").Append(@char.FieldB0);
                    sb.Append(" FieldAC:").Append(@char.FieldAC);
                    sb.Append(" FieldA4:").Append(@char.FieldA4);
                    sb.Append(" Field98:").Append(@char.Field98);
                    sb.Append(" Field94:").Append(@char.Field94);
                    sb.Append(" Field88:").Append(@char.Field88);
                    sb.Append(" Field7C:").Append(@char.Field7C);
                    sb.Append(" Field74:").Append(@char.Field74);
                    sb.Append(" Field68:").Append(@char.Field68);
                    sb.Append(" Field64:").Append(@char.Field64);
                    sb.Append(" FieldAA:").Append(@char.FieldAA);
                    sb.Append(" FieldA9:").Append(@char.FieldA9);
                    sb.Append(" FieldA8:").Append(@char.FieldA8);
                    sb.Append(" Field5C:").Append(@char.Field5C);




                    sb.Append(" ComponentsCount:").Append(@char.ComponentsCount);
                    sb.Append(" DeployTime1:").Append(@char.DeployTime1);
                    sb.Append(" DeployTime2:").Append(@char.DeployTime2);
                    sb.Append(" IsValid:").Append(@char.IsValid);
                    sb.Append(" SpawnLimit:").Append(@char.SpawnLimit);
                    sb.Append(" SpawnStartTime:").Append(@char.SpawnStartTime);

                    //-какая то хрень sb.Append(" HealthBar:").Append(@char.LogicGameObjectData.HealthBar);

                    sb.Append(" ActivationTime:").Append(@char.LogicGameObjectData.ActivationTime);
                    sb.Append(" AllTargetsHit:").Append(@char.LogicGameObjectData.AllTargetsHit);
                    sb.Append(" AppearPushback:").Append(@char.LogicGameObjectData.AppearPushback);
                    sb.Append(" AppearPushbackRadius:").Append(@char.LogicGameObjectData.AppearPushbackRadius);
                    sb.Append(" AreaBuffRadius:").Append(@char.LogicGameObjectData.AreaBuffRadius);
                    sb.Append(" AreaBuffTime:").Append(@char.LogicGameObjectData.AreaBuffTime);
                    sb.Append(" AreaDamageRadius:").Append(@char.LogicGameObjectData.AreaDamageRadius);
                    sb.Append(" AttachedCharacterHeight:").Append(@char.LogicGameObjectData.AttachedCharacterHeight);
                    sb.Append(" AttackDashTime:").Append(@char.LogicGameObjectData.AttackDashTime);
                    sb.Append(" AttackPushBack:").Append(@char.LogicGameObjectData.AttackPushBack);
                    sb.Append(" AttacksAir:").Append(@char.LogicGameObjectData.AttacksAir);
                    sb.Append(" AttacksGround:").Append(@char.LogicGameObjectData.AttacksGround);
                    sb.Append(" AttackShakeTime:").Append(@char.LogicGameObjectData.AttackShakeTime);
                    sb.Append(" BuffOnDamageTime:").Append(@char.LogicGameObjectData.BuffOnDamageTime);
                    sb.Append(" BuildingTarget:").Append(@char.LogicGameObjectData.BuildingTarget);
                    sb.Append(" Burst:").Append(@char.LogicGameObjectData.Burst);
                    sb.Append(" BurstDelay:").Append(@char.LogicGameObjectData.BurstDelay);
                    sb.Append(" BurstKeepTarget:").Append(@char.LogicGameObjectData.BurstKeepTarget);
                    sb.Append(" ChargeRange:").Append(@char.LogicGameObjectData.ChargeRange);
                    sb.Append(" ChargeSpeedMultiplier:").Append(@char.LogicGameObjectData.ChargeSpeedMultiplier);
                    sb.Append(" CollisionRadius:").Append(@char.LogicGameObjectData.CollisionRadius);
                    sb.Append(" CrownTowerDamagePercent:").Append(@char.LogicGameObjectData.CrownTowerDamagePercent);
                    sb.Append(" DashConstantTime:").Append(@char.LogicGameObjectData.DashConstantTime);
                    sb.Append(" DashCooldown:").Append(@char.LogicGameObjectData.DashCooldown);
                    sb.Append(" DashImmuneToDamageTime:").Append(@char.LogicGameObjectData.DashImmuneToDamageTime);
                    sb.Append(" DashLandingTime:").Append(@char.LogicGameObjectData.DashLandingTime);
                    sb.Append(" DashMaxRange:").Append(@char.LogicGameObjectData.DashMaxRange);
                    sb.Append(" DashMinRange:").Append(@char.LogicGameObjectData.DashMinRange);
                    sb.Append(" DashPushBack:").Append(@char.LogicGameObjectData.DashPushBack);
                    sb.Append(" DashRadius:").Append(@char.LogicGameObjectData.DashRadius);
                    sb.Append(" DeathDamageRadius:").Append(@char.LogicGameObjectData.DeathDamageRadius);
                    sb.Append(" DeathInheritIgnoreList:").Append(@char.LogicGameObjectData.DeathInheritIgnoreList);
                    sb.Append(" DeathPushBack:").Append(@char.LogicGameObjectData.DeathPushBack);
                    sb.Append(" DeathSpawnCount:").Append(@char.LogicGameObjectData.DeathSpawnCount);
                    sb.Append(" DeathSpawnDeployTime:").Append(@char.LogicGameObjectData.DeathSpawnDeployTime);
                    sb.Append(" DeathSpawnMinRadius:").Append(@char.LogicGameObjectData.DeathSpawnMinRadius);
                    sb.Append(" DeathSpawnPushback:").Append(@char.LogicGameObjectData.DeathSpawnPushback);
                    sb.Append(" DeathSpawnRadius:").Append(@char.LogicGameObjectData.DeathSpawnRadius);
                    sb.Append(" DeployDelay:").Append(@char.LogicGameObjectData.DeployDelay);
                    sb.Append(" DeployTime:").Append(@char.LogicGameObjectData.DeployTime);
                    sb.Append(" DeployTimerDelay:").Append(@char.LogicGameObjectData.DeployTimerDelay);
                    sb.Append(" DontStopMoveAnim:").Append(@char.LogicGameObjectData.DontStopMoveAnim);
                    sb.Append(" FlyDirectPaths:").Append(@char.LogicGameObjectData.FlyDirectPaths);


                    sb.Append(" FlyFromGround:").Append(@char.LogicGameObjectData.FlyFromGround);
                    sb.Append(" FlyingHeight:").Append(@char.LogicGameObjectData.FlyingHeight);
                    sb.Append(" GrowSize:").Append(@char.LogicGameObjectData.GrowSize);
                    sb.Append(" GrowTime:").Append(@char.LogicGameObjectData.GrowTime);
                    sb.Append(" HasRotationOnTimeline:").Append(@char.LogicGameObjectData.HasRotationOnTimeline);
                    sb.Append(" HealOnMorph:").Append(@char.LogicGameObjectData.HealOnMorph);
                    sb.Append(" HealthBarOffsetY:").Append(@char.LogicGameObjectData.HealthBarOffsetY);
                    sb.Append(" HideBeforeFirstHit:").Append(@char.LogicGameObjectData.HideBeforeFirstHit);
                    sb.Append(" HidesWhenNotAttacking:").Append(@char.LogicGameObjectData.HidesWhenNotAttacking);
                    sb.Append(" HideTimeMs:").Append(@char.LogicGameObjectData.HideTimeMs);
                    sb.Append(" HitSpeed:").Append(@char.LogicGameObjectData.HitSpeed);
                    sb.Append(" IgnorePushback:").Append(@char.LogicGameObjectData.IgnorePushback);
                    sb.Append(" IsSummonerTower:").Append(@char.LogicGameObjectData.IsSummonerTower);
                    sb.Append(" JumpEnabled:").Append(@char.LogicGameObjectData.JumpEnabled);
                    sb.Append(" JumpHeight:").Append(@char.LogicGameObjectData.JumpHeight);
                    sb.Append(" JumpSpeed:").Append(@char.LogicGameObjectData.JumpSpeed);
                    sb.Append(" Kamikaze:").Append(@char.LogicGameObjectData.Kamikaze);
                    sb.Append(" KamikazeTime:").Append(@char.LogicGameObjectData.KamikazeTime);
                    sb.Append(" KingTowerMiddle:").Append(@char.LogicGameObjectData.KingTowerMiddle);
                    sb.Append(" LifeTime:").Append(@char.LogicGameObjectData.LifeTime);
                    sb.Append(" LoadFirstHit:").Append(@char.LogicGameObjectData.LoadFirstHit);
                    sb.Append(" LoadTime:").Append(@char.LogicGameObjectData.LoadTime);
                    sb.Append(" LoopMoveEffect:").Append(@char.LogicGameObjectData.LoopMoveEffect);
                    sb.Append(" ManaCollectAmount:").Append(@char.LogicGameObjectData.ManaCollectAmount);
                    sb.Append(" ManaGenerateLimit:").Append(@char.LogicGameObjectData.ManaGenerateLimit);
                    sb.Append(" ManaGenerateTimeMs:").Append(@char.LogicGameObjectData.ManaGenerateTimeMs);
                    sb.Append(" Mass:").Append(@char.LogicGameObjectData.Mass);
                    sb.Append(" MinimumRange:").Append(@char.LogicGameObjectData.MinimumRange);
                    sb.Append(" MorphKeepTarget:").Append(@char.LogicGameObjectData.MorphKeepTarget);
                    sb.Append(" MorphTime:").Append(@char.LogicGameObjectData.MorphTime);
                    sb.Append(" MultipleProjectiles:").Append(@char.LogicGameObjectData.MultipleProjectiles);
                    sb.Append(" MultipleTargets:").Append(@char.LogicGameObjectData.MultipleTargets);
                    sb.Append(" ProjectileStartRadius:").Append(@char.LogicGameObjectData.ProjectileStartRadius);
                    sb.Append(" ProjectileStartZ:").Append(@char.LogicGameObjectData.ProjectileStartZ);
                    sb.Append(" ProjectileYOffset:").Append(@char.LogicGameObjectData.ProjectileYOffset);
                    sb.Append(" Pushback:").Append(@char.LogicGameObjectData.Pushback);
                    sb.Append(" Range:").Append(@char.LogicGameObjectData.Range);
                    sb.Append(" RetargetAfterAttack:").Append(@char.LogicGameObjectData.RetargetAfterAttack);
                    sb.Append(" RotateAngleSpeed:").Append(@char.LogicGameObjectData.RotateAngleSpeed);
                    sb.Append(" SelfAsAoeCenter:").Append(@char.LogicGameObjectData.SelfAsAoeCenter);
                    sb.Append(" ShieldDiePushback:").Append(@char.LogicGameObjectData.ShieldDiePushback);
                    sb.Append(" ShowHealthNumber:").Append(@char.LogicGameObjectData.ShowHealthNumber);
                    sb.Append(" SightClip:").Append(@char.LogicGameObjectData.SightClip);
                    sb.Append(" SightClipSide:").Append(@char.LogicGameObjectData.SightClipSide);
                    sb.Append(" SightRange:").Append(@char.LogicGameObjectData.SightRange);
                    sb.Append(" SpawnAngleShift:").Append(@char.LogicGameObjectData.SpawnAngleShift);
                    sb.Append(" SpawnAreaObjectLevelIndex:").Append(@char.LogicGameObjectData.SpawnAreaObjectLevelIndex);
                    sb.Append(" SpawnCharacterLevelIndex:").Append(@char.LogicGameObjectData.SpawnCharacterLevelIndex);
                    sb.Append(" SpawnConstPriority:").Append(@char.LogicGameObjectData.SpawnConstPriority);
                

                    sb.Append(" SpawnInterval:").Append(@char.LogicGameObjectData.SpawnInterval);
                    sb.Append(" SpawnLimit:").Append(@char.LogicGameObjectData.SpawnLimit);
                    sb.Append(" SpawnNumber:").Append(@char.LogicGameObjectData.SpawnNumber);
                    sb.Append(" SpawnPathfindSpeed:").Append(@char.LogicGameObjectData.SpawnPathfindSpeed);
                    sb.Append(" SpawnPauseTime:").Append(@char.LogicGameObjectData.SpawnPauseTime);
                    sb.Append(" SpawnPushback:").Append(@char.LogicGameObjectData.SpawnPushback);
                    sb.Append(" SpawnPushbackRadius:").Append(@char.LogicGameObjectData.SpawnPushbackRadius);
                    sb.Append(" SpawnRadius:").Append(@char.LogicGameObjectData.SpawnRadius);
                    sb.Append(" SpawnStartTime:").Append(@char.LogicGameObjectData.SpawnStartTime);
                    sb.Append(" SpecialAttackWhenHidden:").Append(@char.LogicGameObjectData.SpecialAttackWhenHidden);
                    sb.Append(" SpecialLoadTime:").Append(@char.LogicGameObjectData.SpecialLoadTime);
                    sb.Append(" SpecialMinRange:").Append(@char.LogicGameObjectData.SpecialMinRange);
                    sb.Append(" SpecialRange:").Append(@char.LogicGameObjectData.SpecialRange);
                    sb.Append(" StartingBuffTime:").Append(@char.LogicGameObjectData.StartingBuffTime);
                    sb.Append(" StopMovementAfterMS:").Append(@char.LogicGameObjectData.StopMovementAfterMS);
                    sb.Append(" StopTimeAfterAttack:").Append(@char.LogicGameObjectData.StopTimeAfterAttack);
                    sb.Append(" StopTimeAfterSpecialAttack:").Append(@char.LogicGameObjectData.StopTimeAfterSpecialAttack);
                    sb.Append(" TargetOnlyBuildings:").Append(@char.LogicGameObjectData.TargetOnlyBuildings);
                    sb.Append(" TileSizeOverride:").Append(@char.LogicGameObjectData.TileSizeOverride);
                    sb.Append(" TurretMovement:").Append(@char.LogicGameObjectData.TurretMovement);
                    sb.Append(" UpTimeMs:").Append(@char.LogicGameObjectData.UpTimeMs);
                    sb.Append(" UseAnimator:").Append(@char.LogicGameObjectData.UseAnimator);
                    sb.Append(" VariableDamageLifeTime:").Append(@char.LogicGameObjectData.VariableDamageLifeTime);
                    sb.Append(" VariableDamageTransitionTime:").Append(@char.LogicGameObjectData.VariableDamageTransitionTime);
                    sb.Append(" VisualHitSpeed:").Append(@char.LogicGameObjectData.VisualHitSpeed);
                    sb.Append(" WaitMS:").Append(@char.LogicGameObjectData.WaitMS);
                    sb.Append(" WalkingSpeedTweakPercentage:").Append(@char.LogicGameObjectData.WalkingSpeedTweakPercentage);

                    var @proj = @char.LogicGameObjectData.Projectile;

                    sb.Append(" proj_AoeToAir:").Append(@proj.AoeToAir);
                    sb.Append(" proj_AoeToGround:").Append(@proj.AoeToGround);
                    sb.Append(" proj_BuffTimeIncreasePerLevel:").Append(@proj.BuffTimeIncreasePerLevel);
                    sb.Append(" proj_ChainedHitRadius:").Append(@proj.ChainedHitRadius);
                    sb.Append(" proj_ConstantHeight:").Append(@proj.ConstantHeight);
                    sb.Append(" proj_CrownTowerDamagePercent:").Append(@proj.CrownTowerDamagePercent);
                    sb.Append(" proj_CrownTowerHealPercent:").Append(@proj.CrownTowerHealPercent);
                    sb.Append(" proj_DragBackSpeed:").Append(@proj.DragBackSpeed);
                    sb.Append(" proj_DragMargin:").Append(@proj.DragMargin);
                    sb.Append(" proj_Gravity:").Append(@proj.Gravity);
                    sb.Append(" proj_HeightFromTargetRadius:").Append(@proj.HeightFromTargetRadius);
                    sb.Append(" proj_Homing:").Append(@proj.Homing);
                    sb.Append(" proj_MaxDistance:").Append(@proj.MaxDistance);
                    sb.Append(" proj_MaximumTargets:").Append(@proj.MaximumTargets);
                    sb.Append(" proj_MinDistance:").Append(@proj.MinDistance);
                    sb.Append(" proj_OnlyEnemies:").Append(@proj.OnlyEnemies);
                    sb.Append(" proj_OnlyOwnTroops:").Append(@proj.OnlyOwnTroops);
                    sb.Append(" proj_PingpongVisualTime:").Append(@proj.PingpongVisualTime);
                    sb.Append(" proj_ProjectileRadius:").Append(@proj.ProjectileRadius);
                    sb.Append(" proj_ProjectileRadiusY:").Append(@proj.ProjectileRadiusY);
                    sb.Append(" proj_Pushback:").Append(@proj.Pushback);
                    sb.Append(" proj_PushbackAll:").Append(@proj.PushbackAll);
                    sb.Append(" proj_Radius:").Append(@proj.Radius);
                    sb.Append(" proj_RadiusY:").Append(@proj.RadiusY);
                    sb.Append(" proj_RandomAngle:").Append(@proj.RandomAngle);
                    sb.Append(" proj_RandomDistance:").Append(@proj.RandomDistance);
                    sb.Append(" proj_Scale:").Append(@proj.Scale);
                    sb.Append(" proj_ShadowDisableRotate:").Append(@proj.ShadowDisableRotate);
                    sb.Append(" proj_ShakesTargets:").Append(@proj.ShakesTargets);
                    sb.Append(" proj_SpawnCharacterDeployTime:").Append(@proj.SpawnCharacterDeployTime);
                    sb.Append(" proj_SpawnCharacterDeployTime1:").Append(@proj.SpawnCharacterDeployTime1);
                    sb.Append(" proj_SpawnCharacterLevelIndex:").Append(@proj.SpawnCharacterLevelIndex);
                    sb.Append(" proj_SpawnConstPriority:").Append(@proj.SpawnConstPriority);
                    sb.Append(" proj_Speed:").Append(@proj.Speed);
                    sb.Append(" proj_TargetToEdge:").Append(@proj.TargetToEdge);
                    sb.Append(" proj_Use360Frames:").Append(@proj.Use360Frames);


                    //help.logg(sb.ToString());*/

                    // string Line3 = sb.ToString();
                    //sb.Clear();
                    //sb.Append("***** ").Append(i);


                    // Logger.Verbose(Line1);
                    // Logger.Verbose(Line2);
                    //  Logger.Verbose(Line3);

                    //  help.logg(Line1);
                    //  help.logg(Line2);
                    //help.logg(Line3);
                }

            }

            Playfield p = new Playfield();
            p.BattleTime = ClashEngine.Instance.Battle.BattleTime;
            p.ownerIndex = (int)lp.OwnerIndex;
            p.ownMana = (int)lp.Mana;
            //p.nextCard = TODO:

            p.ownHandCards = ownHandCards;
            p.ownAreaEffects = ownAreaEffects;
            p.ownMinions = ownMinions;
            p.ownBuildings = ownBuildings;
            p.ownTowers = ownTowers;
            
            p.enemyAreaEffects = enemyAreaEffects;
            p.enemyMinions = enemyMinions;
            p.enemyBuildings = enemyBuildings;
            p.enemyTowers = enemyTowers;

            p.home = p.ownTowers[0].Position.Y < 15250 ? true : false; int i = 0;

            foreach (BoardObj t in p.ownTowers) if (t.Tower < 10) i += t.Line;
            int kingsLine = 0;
            switch (i)
            {
                case 0: kingsLine = 3; break;
                case 1: kingsLine = 2; break;
                case 2: kingsLine = 1; break;
            }
            foreach (BoardObj t in p.ownTowers) if (t.Tower > 9) t.Line = kingsLine;

            p.print();
            help.logg("###Start_calc: " + DateTime.Now + "\r\n");
            
            Behavior behave = new BehaviorApollo();//change this to new Behavior
            Cast bc = behave.getBestCast(p);
            CastRequest retval = null;
            if (bc != null)
            {
                help.logg("Cast " + bc.ToString());
                retval = new CastRequest(bc.SpellName, bc.Position.ToVector2());
            }
            else help.logg("Waiting for...");

            help.logg("###End_calc: " + DateTime.Now + "\r\n");

            return retval;
            
        }

    }
}

