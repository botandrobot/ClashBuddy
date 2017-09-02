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

    public class Nano : ActionSelectorBase
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<Nano>();
        private readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();

        internal static NanoSettings Settings { get; } = new NanoSettings();

        Helpfunctions help = Helpfunctions.Instance;

        DateTime starttime = DateTime.Now;
        
        //Behavior behave = new BehaviorControl();

        #region Implementation of IAuthored

        /// <summary> The name of the routine. </summary>
        public override string Name
        {
            get { return "Nano"; }
        }

        /// <summary> The description of the routine. </summary>
        public override string Description
        {
            get { return "The default routine for Clash Royale."; }
        }

        /// <summary>The author of this routine.</summary>
        public override string Author
        {
            get { return "Vlad"; }
        }

        /// <summary>The version of this routine.</summary>
        public override Version Version
        {
            get { return new Version(0, 0, 0, 7); }
        }

        /// <summary>Unique Identifier.</summary>
        public override Guid Identifier
        {
            get { return new Guid("{591611D1-B5F2-4483-AF4F-B154153C40F7}"); }
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
            help.logg("###################test############### " + DateTime.Now);

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
            
            Behavior behave = new BehaviorControl();//change this to new Behavior
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

            
            //17400/2300
            Engine.NativeObjects.Native.Vector2 castPos = new Engine.NativeObjects.Native.Vector2() { X = 17400, Y = 2300 };
            foreach (Handcard hc in ownHandCards)
            {
                //Handcard hc = new Handcard(CardDB.Instance.cardNamestringToEnum(spell.Name.Value), lvl); //hc.lvl = ??? TODO
                if (lp.Mana >= hc.manacost) return new CastRequest(hc.name, castPos);
            }

            return null;

            var towerPos = battle.SummonerTowers[0].StartPosition;

            string name = "";
            if (_spellQueue.TryDequeue(out name)) return new CastRequest(name, towerPos);




            var cycleSpells = spells.Where(s => s != null && s.IsValid && s.ManaCost <= 3).OrderBy(s => s.ManaCost);
            var powerSpells = spells.Where(s => s != null && s.IsValid && s.ManaCost > 3).OrderByDescending(s => s.ManaCost);

            if (cycleSpells.Count() > 1)
            {
                var spell = cycleSpells.FirstOrDefault();
                return new CastRequest(spell.Name.Value, towerPos);
            }

            var player = ClashEngine.Instance.LocalPlayer;

            if (player == null || player.Mana < 9) return null;

            foreach (var s in powerSpells)
            {
                if (_spellQueue.Count < 1)
                {
                    _spellQueue.Enqueue(s.Name.Value);
                }
                else
                {
                    return new CastRequest(s.Name.Value, towerPos);
                }
            }

            return null;
        }

        /* public string GetNextCast()
         {
             var om = ClashEngine.Instance.ObjectManager;

             var battle = ClashEngine.Instance.Battle;
             if (battle == null || !battle.IsValid) return null;

             var chars = om.OfType<Engine.NativeObjects.Logic.GameObjects.Character>();

             StringBuilder sb = new StringBuilder();

             int i = 0;
             foreach (var @char in chars)
             {
                 i++;
                 var data = @char.LogicGameObjectData;
                 if (data != null && data.IsValid)
                 {



                     sb.Clear();
                     sb.Append("***** ").Append(i);

                     sb.Append("own:").Append(@char.OwnerIndex);
                     sb.Append(" name:").Append(data.Name.Value);
                     sb.Append(" TowerLevel:").Append(@char.TowerLevel);
                    // + sb.Append(" HideTimeMs:").Append(@char.HideTimeMs);
                    // + sb.Append(" HasSpeedComponent:").Append(@char.HasSpeedComponent);
                     //+ sb.Append(" GrowTime:").Append(@char.GrowTime);

                     sb.Append(" GlobalId:").Append(@char.GlobalId);
                     sb.Append(" Mana:").Append(@char.Mana);
                     sb.Append(" Parent:").Append(@char.Parent);
                     sb.Append(" Vtable:").Append(@char.Vtable);

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

                     string Line1 = sb.ToString();
                     sb.Clear();
                     sb.Append("***** ").Append(i);

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


                     string Line2 = sb.ToString();
                     sb.Clear();
                     sb.Append("***** ").Append(i);

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


                     string Line3 = sb.ToString();
                     sb.Clear();
                     sb.Append("***** ").Append(i);


                     Logger.Verbose(Line1);
                     Logger.Verbose(Line2);
                     Logger.Verbose(Line3);

                     Helpfunctions.Instance.logg(Line1);
                     Helpfunctions.Instance.logg(Line2);
                     Helpfunctions.Instance.logg(Line3);
                     //Logger.Verbose(sb.ToString());
                 }
             }

             var towerPos = battle.SummonerTowers[0].StartPosition;

             string name = "";
             if (_spellQueue.TryDequeue(out name)) return new CastRequest(name, towerPos);

             var spells = ClashEngine.Instance.AvailableSpells;

             foreach (var spell in spells)
             {
                 if (spell != null && spell.IsValid)
                     Logger.Verbose("Available Spell {Name}, {ManaCost}", spell.Name, spell.ManaCost);
             }

             var cycleSpells = spells.Where(s => s != null && s.IsValid && s.ManaCost <= 3).OrderBy(s => s.ManaCost);
             var powerSpells = spells.Where(s => s != null && s.IsValid && s.ManaCost > 3).OrderByDescending(s => s.ManaCost);

             if (cycleSpells.Count() > 1)
             {
                 var spell = cycleSpells.FirstOrDefault();
                 return new CastRequest(spell.Name.Value, towerPos);
             }

             var player = ClashEngine.Instance.LocalPlayer;

             if (player == null || player.Mana < 9) return null;

             foreach (var s in powerSpells)
             {
                 if (_spellQueue.Count < 1)
                 {
                     _spellQueue.Enqueue(s.Name.Value);
                 }
                 else
                 {
                     return new CastRequest(s.Name.Value, towerPos);
                 }
             }

             return null;
         }*/
                }
            }

/*
//!CompilerOption|AddRef|IronPython.dll
//!CompilerOption|AddRef|IronPython.Modules.dll
//!CompilerOption|AddRef|Microsoft.Scripting.dll
//!CompilerOption|AddRef|Microsoft.Dynamic.dll
//!CompilerOption|AddRef|Microsoft.Scripting.Metadata.dll
using Triton.Game.Mapping;

using Logger = Triton.Common.LogUtilities.Logger;

namespace HREngine.Bots
{
    public class DefaultRoutine : IActionSelector
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private readonly ScriptManager _scriptManager = new ScriptManager();

        private readonly List<Tuple<string, string>> _mulliganRules = new List<Tuple<string, string>>();

        private int dirtyTargetSource = -1;
        private int stopAfterWins = 30;
        private int concedeLvl = 5; // the rank, till you want to concede
        private int dirtytarget = -1;
        private int dirtychoice = -1;
        private string choiceCardId = "";
        DateTime starttime = DateTime.Now;
        bool enemyConcede = false;

        public bool learnmode = false;
        public bool printlearnmode = true;

        Silverfish sf = Silverfish.Instance;

        //uncomment the desired option, or leave it as is to select via the interface
        Behavior behave = new BehaviorControl();
        //Behavior behave = new BehaviorRush();


        public DefaultRoutine()
        {
            // Global rules. Never keep a 4+ minion, unless it's Bolvar Fordragon (paladin).
            _mulliganRules.Add(new Tuple<string, string>("True", "card.Entity.Cost >= 4 and card.Entity.Id != \"GVG_063\""));

            // Never keep Tracking.
            _mulliganRules.Add(new Tuple<string, string>("mulliganData.UserClass == TAG_CLASS.HUNTER", "card.Entity.Id == \"DS1_184\""));

            // Example rule for self.
            //_mulliganRules.Add(new Tuple<string, string>("mulliganData.UserClass == TAG_CLASS.MAGE", "card.Cost >= 5"));

            // Example rule for opponents.
            //_mulliganRules.Add(new Tuple<string, string>("mulliganData.OpponentClass == TAG_CLASS.MAGE", "card.Cost >= 3"));

            // Example rule for matchups.
            //_mulliganRules.Add(new Tuple<string, string>("mulliganData.userClass == TAG_CLASS.HUNTER && mulliganData.OpponentClass == TAG_CLASS.DRUID", "card.Cost >= 2"));

            bool concede = false;
            bool teststuff = false;
            // set to true, to run a testfile (requires test.txt file in folder where _cardDB.txt file is located)
            bool printstuff = false; // if true, the best board of the tested file is printet stepp by stepp

            Helpfunctions.Instance.ErrorLog("----------------------------");
            Helpfunctions.Instance.ErrorLog("you are running uai V" + Silverfish.Instance.versionnumber);
            Helpfunctions.Instance.ErrorLog("----------------------------");

            if (teststuff)
            {
                Ai.Instance.autoTester(printstuff);
            }
        }
        


        #region Implementation of IBase

        /// <summary>Initializes this routine.</summary>
        public void Initialize()
        {
            _scriptManager.Initialize(null,
                new List<string>
                {
                    "Triton.Game",
                    "Triton.Bot",
                    "Triton.Common",
                    "Triton.Game.Mapping",
                    "Triton.Game.Abstraction"
                });
        }

        /// <summary>Deinitializes this routine.</summary>
        public void Deinitialize()
        {
            _scriptManager.Deinitialize();
        }

        #endregion

        #region Implementation of IRunnable

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
                    !VerifyCondition(tuple.Item1, new List<string> {"mulliganData"}, out ex))
                {
                    Log.ErrorFormat("[Start] There is an error with a mulligan execution condition [{1}]: {0}.", ex,
                        tuple.Item1);
                    BotManager.Stop();
                }

                if (
                    !VerifyCondition(tuple.Item2, new List<string> {"mulliganData", "card"},
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

        #endregion

        #region Implementation of IConfigurable

        /// <summary> The routine's settings control. This will be added to the Hearthbuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                using (var fs = new FileStream(@"Routines\DefaultRoutine\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl) XamlReader.Load(fs);

                    // Your settings binding here.

                    // ArenaPreferredClass1
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass1ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass1ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass1ComboBox",
                            "ArenaPreferredClass1", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass1ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass2
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass2ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass2ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass2ComboBox",
                            "ArenaPreferredClass2", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass2ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass3
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass3ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass3ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass3ComboBox",
                            "ArenaPreferredClass3", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass3ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass4
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass4ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass4ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass4ComboBox",
                            "ArenaPreferredClass4", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass4ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass5
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass5ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass5ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass5ComboBox",
                            "ArenaPreferredClass5", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass5ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
                    
                    // defaultBehaviorComboBox1
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "defaultBehaviorComboBox1", "AllBehav",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'defaultBehaviorComboBox1'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "defaultBehaviorComboBox1",
                            "DefaultBehavior", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'defaultBehaviorComboBox1'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
                    // Your settings event handlers here.

                    return root;
                }
            }
        }

        /// <summary>The settings object. This will be registered in the current configuration.</summary>
        public JsonSettings Settings
        {
            get { return DefaultRoutineSettings.Instance; }
        }

        #endregion

        #region Implementation of IRoutine

        /// <summary>
        /// Sends data to the routine with the associated name.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        /// <param name="param">The data passed for the configuration.</param>
        public void SetConfiguration(string name, params object[] param)
        {
        }

        /// <summary>
        /// Requests data from the routine with the associated name.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        /// <returns>Data from the routine.</returns>
        public object GetConfiguration(string name)
        {
            return null;
        }

        /// <summary>
        /// The routine's coroutine logic to execute.
        /// </summary>
        /// <param name="type">The requested type of logic to execute.</param>
        /// <param name="context">Data sent to the routine from the bot for the current logic.</param>
        /// <returns>true if logic was executed to handle this type and false otherwise.</returns>
        public async Task<bool> Logic(string type, object context)
        {

            

            // The bot is requesting mulligan logic.
            if (type == "mulligan")
            {
                await MulliganLogic(context as MulliganData);
                return true;
            }

            // The bot is requesting emote logic.
            if (type == "emote")
            {
                await EmoteLogic(context as EmoteData);
                return true;
            }

            // The bot is requesting our turn logic.
            if (type == "our_turn")
            {
                await OurTurnLogic();
                return true;
            }

            // The bot is requesting opponent turn logic.
            if (type == "opponent_turn")
            {
                await OpponentTurnLogic();
                return true;
            }

	        // The bot is requesting our turn logic.
	        if (type == "our_turn_combat")
	        {
		        await OurTurnCombatLogic();
		        return true;
	        }

	        // The bot is requesting opponent turn logic.
	        if (type == "opponent_turn_combat")
	        {
		        await OpponentTurnCombatLogic();
		        return true;
	        }

			// The bot is requesting arena draft logic.
			if (type == "arena_draft")
            {
                await ArenaDraftLogic(context as ArenaDraftData);
                return true;
            }

            // The bot is requesting quest handling logic.
            if (type == "handle_quests")
            {
                await HandleQuestsLogic(context as QuestData);
                return true;
            }

            // Whatever the current logic type is, this routine doesn't implement it.
            return false;
        }

        #region Mulligan

        private int RandomMulliganThinkTime()
        {
            var random = Client.Random;
            var type = random.Next(0, 100)%4;

            if (type == 0) return random.Next(800, 1200);
            if (type == 1) return random.Next(1200, 2500);
            if (type == 2) return random.Next(2500, 3700);
            return 0;
        }

        /// <summary>
        /// This task implements custom mulligan choosing logic for the bot.
        /// The user is expected to set the Mulligans list elements to true/false 
        /// to signal to the bot which cards should/shouldn't be mulliganed. 
        /// This task should also implement humanization factors, such as hovering 
        /// over cards, or delaying randomly before returning, as the mulligan 
        /// process takes place as soon as the task completes.
        /// </summary>
        /// <param name="mulliganData">An object that contains relevant data for the mulligan process.</param>
        /// <returns></returns>
        public async Task MulliganLogic(MulliganData mulliganData)
        {
            Log.InfoFormat("[Mulligan] {0} vs {1}.", mulliganData.UserClass, mulliganData.OpponentClass);
            var count = mulliganData.Cards.Count;

            if (this.behave.BehaviorName() != DefaultRoutineSettings.Instance.DefaultBehavior)
            {
                behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
            }
            if (!Mulligan.Instance.getHoldList(mulliganData, this.behave))
            {
                for (var i = 0; i < count; i++)
                {
                    var card = mulliganData.Cards[i];

                    try
                    {
                        foreach (var tuple in _mulliganRules)
                        {
                            if (GetCondition(tuple.Item1,
                                new List<RegisterScriptVariableDelegate>
                            {
                                scope => scope.SetVariable("mulliganData", mulliganData)
                            }))
                            {
                                if (GetCondition(tuple.Item2,
                                    new List<RegisterScriptVariableDelegate>
                                {
                                    scope => scope.SetVariable("mulliganData", mulliganData),
                                    scope => scope.SetVariable("card", card)
                                }))
                                {
                                    mulliganData.Mulligans[i] = true;
                                    Log.InfoFormat(
                                        "[Mulligan] {0} should be mulliganed because it matches the user's mulligan rule: [{1}] ({2}).",
                                        card.Entity.Id, tuple.Item2, tuple.Item1);
                                }
                            }
                            else
                            {
                                Log.InfoFormat(
                                    "[Mulligan] The mulligan execution check [{0}] is false, so the mulligan criteria [{1}] will not be evaluated.",
                                    tuple.Item1, tuple.Item2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("[Mulligan] An exception occurred: {0}.", ex);
                        BotManager.Stop();
                        return;
                    }
                }
            }

            var thinkList = new List<KeyValuePair<int, int>>();
            for (var i = 0; i < count; i++)
            {
                thinkList.Add(new KeyValuePair<int, int>(i%count, RandomMulliganThinkTime()));
            }
            thinkList.Shuffle();

            foreach (var entry in thinkList)
            {
                var card = mulliganData.Cards[entry.Key];

                Log.InfoFormat("[Mulligan] Now thinking about mulliganing {0} for {1} ms.", card.Entity.Id, entry.Value);

                // Instant think time, skip the card.
                if (entry.Value == 0)
                    continue;

                Client.MouseOver(card.InteractPoint);

                await Coroutine.Sleep(entry.Value);
            }
        }

        #endregion

        #region Emote

        /// <summary>
        /// This task implements player emote detection logic for the bot.
        /// </summary>
        /// <param name="data">An object that contains relevant data for the emote event.</param>
        /// <returns></returns>
        public async Task EmoteLogic(EmoteData data)
        {
            Log.InfoFormat("[Emote] The enemy is using the emote [{0}].", data.Emote);

            if (data.Emote == EmoteType.GREETINGS)
            {
            }
            else if (data.Emote == EmoteType.WELL_PLAYED)
            {
            }
            else if (data.Emote == EmoteType.OOPS)
            {
            }
            else if (data.Emote == EmoteType.THREATEN)
            {
            }
            else if (data.Emote == EmoteType.THANKS)
            {
            }
            else if (data.Emote == EmoteType.SORRY)
            {
            }
        }

        #endregion

        #region Turn

	    public async Task OurTurnCombatLogic()
	    {
            Log.InfoFormat("[OurTurnCombatLogic]");
            await Coroutine.Sleep(555 + makeChoice());
            switch (dirtychoice)
            {
                case 0: TritonHs.ChooseOneClickMiddle(); break;
                case 1: TritonHs.ChooseOneClickLeft(); break;
                case 2: TritonHs.ChooseOneClickRight(); break;
            }

            dirtychoice = -1;
            await Coroutine.Sleep(555);
            Silverfish.Instance.lastpf = null;
            return;
		}

	    public async Task OpponentTurnCombatLogic()
	    {
		    Log.Info("[OpponentTurnCombatLogic]");
	    }

		/// <summary>
		/// Under construction.
		/// </summary>
		/// <returns></returns>
		public async Task OurTurnLogic()
        {
            if (this.behave.BehaviorName() != DefaultRoutineSettings.Instance.DefaultBehavior)
            {
                behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
                Silverfish.Instance.lastpf = null;
            }

            if (this.learnmode && (TritonHs.IsInTargetMode() || TritonHs.IsInChoiceMode()))
            {
                await Coroutine.Sleep(50);
                return;
            }

            if (TritonHs.IsInTargetMode())
            {
                if (dirtytarget >= 0)
                {
                    Log.Info("targeting...");
                    HSCard source = null;
                    if (dirtyTargetSource == 9000) // 9000 = ability
                    {
                        source = TritonHs.OurHeroPowerCard;
                    }
                    else
                    {
                        source = getEntityWithNumber(dirtyTargetSource);
                    }
                    HSCard target = getEntityWithNumber(dirtytarget);

                    if (target == null)
                    {
                        Log.Error("target is null...");
                        TritonHs.CancelTargetingMode();
                        return;
                    }

                    dirtytarget = -1;
                    dirtyTargetSource = -1;

                    if (source == null) await TritonHs.DoTarget(target);
                    else await source.DoTarget(target);

                    await Coroutine.Sleep(555);

                    return;
                }

                Log.Error("target failure...");
                TritonHs.CancelTargetingMode();
                return;
            }

            if (TritonHs.IsInChoiceMode())
            {
                await Coroutine.Sleep(555 + makeChoice());
                switch (dirtychoice)
                {
                    case 0: TritonHs.ChooseOneClickMiddle(); break;
                    case 1: TritonHs.ChooseOneClickLeft(); break;
                    case 2: TritonHs.ChooseOneClickRight(); break;
                }

                dirtychoice = -1;
                await Coroutine.Sleep(555);
                return;
            }

            bool sleepRetry = false;
            bool templearn = Silverfish.Instance.updateEverything(behave, 0, out sleepRetry);
            if (sleepRetry)
            {
                Log.Error("[AI] Readiness error. Attempting recover...");
                await Coroutine.Sleep(500);
                templearn = Silverfish.Instance.updateEverything(behave, 1, out sleepRetry);
            }

            if (templearn == true) this.printlearnmode = true;
            
            if (this.learnmode)
            {
                if (this.printlearnmode)
                {
                    Ai.Instance.simmulateWholeTurnandPrint();
                }
                this.printlearnmode = false;

                //do nothing
                await Coroutine.Sleep(50);
                return;
            }

            var moveTodo = Ai.Instance.bestmove;
            if (moveTodo == null || moveTodo.actionType == actionEnum.endturn || Ai.Instance.bestmoveValue < -9999)
            {
                bool doEndTurn = false;
                bool doConcede = false;
                if (Ai.Instance.bestmoveValue > -10000) doEndTurn = true;
                else if (HREngine.Bots.Settings.Instance.concedeMode != 0) doConcede = true;
                else
                {
                    if (new Playfield().ownHeroHasDirectLethal())
                    {
                        Playfield lastChancePl = Ai.Instance.bestplay;
                        bool lastChance = false;
                        if (lastChancePl.owncarddraw > 0)
                        {
                            foreach (Handmanager.Handcard hc in lastChancePl.owncards)
                            {
                                if (hc.card.name == CardDB.cardName.unknown) lastChance = true;
                            }
                            if (!lastChance) doConcede = true;
                        }
                        else doConcede = true;

                        if (doConcede)
                        {
                            foreach (Minion m in lastChancePl.ownMinions)
                            {
                                if (!m.playedThisTurn) continue;
                                switch (m.handcard.card.name)
                                {
                                    case CardDB.cardName.cthun: lastChance = true; break;
                                    case CardDB.cardName.nzoththecorruptor: lastChance = true; break;
                                    case CardDB.cardName.yoggsaronhopesend: lastChance = true; break;
                                    case CardDB.cardName.sirfinleymrrgglton: lastChance = true; break;
                                    case CardDB.cardName.ragnarosthefirelord: if (lastChancePl.enemyHero.Hp < 9) lastChance = true; break;
                                    case CardDB.cardName.barongeddon: if (lastChancePl.enemyHero.Hp < 3) lastChance = true; break;
                                }
                            }
                        }
                        if (lastChance) doConcede = false;
                    }
                    else if (moveTodo == null || moveTodo.actionType == actionEnum.endturn) doEndTurn = true;
                }
                if (doEndTurn)
                {
                    Helpfunctions.Instance.ErrorLog("end turn");
                    await TritonHs.EndTurn();
                    return;
                }
                else if (doConcede)
                {
                    Helpfunctions.Instance.ErrorLog("Lethal detected. Concede...");
                    Helpfunctions.Instance.logg("Concede... Lethal detected###############################################");
                    TritonHs.Concede(true);
                    return;
                }
            }
            Helpfunctions.Instance.ErrorLog("play action");
            if (moveTodo == null)
            {
                Helpfunctions.Instance.ErrorLog("moveTodo == null. EndTurn");
                await TritonHs.EndTurn();
                return;
            }

            //play the move#########################################################################
            /*if (HREngine.Bots.Settings.Instance.speedupLevel != 0)
            {
                //moveTodo.print();
                await DoActionList(Ai.Instance.ActionList);
            }
            else
            {
                moveTodo.print();

                //play a card form hand
                if (moveTodo.actionType == actionEnum.playcard)
                {
                    Questmanager.Instance.updatePlayedCardFromHand(moveTodo.card);
                    HSCard cardtoplay = getCardWithNumber(moveTodo.card.entity);
                    if (cardtoplay == null)
                    {
                        Helpfunctions.Instance.ErrorLog("[Tick] cardtoplay == null");
                        return;
                    }

                    if (moveTodo.target != null)
                    {
                        HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                        if (target != null)
                        {
                            Helpfunctions.Instance.ErrorLog("play: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") target: " + target.Name + " (" + target.EntityId + ")");
                            Helpfunctions.Instance.logg("play: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") target: " + target.Name + " (" + target.EntityId + ") choice: " + moveTodo.druidchoice);
						    if (moveTodo.druidchoice >= 1)
                            {
                                dirtytarget = moveTodo.target.entitiyID;
                                dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                                choiceCardId = moveTodo.card.card.cardIDenum.ToString();
                            }

                            //safe targeting stuff for hsbuddy
                            dirtyTargetSource = moveTodo.card.entity;
                            dirtytarget = moveTodo.target.entitiyID;
                            
                            await cardtoplay.Pickup();

                            if (moveTodo.card.card.type == CardDB.cardtype.MOB)
                            {
                                await cardtoplay.UseAt(moveTodo.place);
                            }
                            else if (moveTodo.card.card.type == CardDB.cardtype.WEAPON) // This fixes perdition's blade
                            {
                                await cardtoplay.UseOn(target.Card);
                            }
                            else if (moveTodo.card.card.type == CardDB.cardtype.SPELL)
                            {
                                await cardtoplay.UseOn(target.Card);
                            }
                            else
                            {
                                await cardtoplay.UseOn(target.Card);
                            }
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] Target is missing. Attempting recover...");
                            Helpfunctions.Instance.logg("[AI] Target " + moveTodo.target.entitiyID + "is missing. Attempting recover...");
                        }
                        await Coroutine.Sleep(500);

                        return;
                    }

                    Helpfunctions.Instance.ErrorLog("play: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") target nothing");
                    Helpfunctions.Instance.logg("play: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") choice: " + moveTodo.druidchoice);
                    if (moveTodo.druidchoice >= 1)
                    {
                        dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                        choiceCardId = moveTodo.card.card.cardIDenum.ToString();
                    }

                    dirtyTargetSource = -1;
                    dirtytarget = -1;

                    await cardtoplay.Pickup();

                    if (moveTodo.card.card.type == CardDB.cardtype.MOB)
                    {
                        await cardtoplay.UseAt(moveTodo.place);
                    }
                    else
                    {
                        await cardtoplay.Use();
                    }
                    await Coroutine.Sleep(500);

                    return;
                }

                //attack with minion
                if (moveTodo.actionType == actionEnum.attackWithMinion)
                {
                    HSCard attacker = getEntityWithNumber(moveTodo.own.entitiyID);
                    HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                    if (attacker != null)
                    {
                        if (target != null)
                        {
                            Helpfunctions.Instance.ErrorLog("minion attack: " + attacker.Name + " target: " + target.Name);
                            Helpfunctions.Instance.logg("minion attack: " + attacker.Name + " target: " + target.Name);

                            //-Helpfunctions.Instance.logg("time before move: " + DateTime.Now.ToString("HH:mm:ss.ffff"));
                            await attacker.DoAttack(target);
                            //-Helpfunctions.Instance.logg("time before move: " + DateTime.Now.ToString("HH:mm:ss.ffff"));
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] Target is missing. Attempting recover...");
                            Helpfunctions.Instance.logg("[AI] Target " + moveTodo.target.entitiyID + "is missing. Attempting recover...");
                        }
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("[AI] Attacker is missing. Attempting recover...");
                        Helpfunctions.Instance.logg("[AI] Attacker " + moveTodo.own.entitiyID + "is missing. Attempting recover...");
                    }
                    await Coroutine.Sleep(250);
                    return;
                }
                //attack with hero
                if (moveTodo.actionType == actionEnum.attackWithHero)
                {
                    HSCard attacker = getEntityWithNumber(moveTodo.own.entitiyID);
                    HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                    if (attacker != null)
                    {
                        if (target != null)
                        {
                            dirtytarget = moveTodo.target.entitiyID;
                            Helpfunctions.Instance.ErrorLog("heroattack: " + attacker.Name + " target: " + target.Name);
                            Helpfunctions.Instance.logg("heroattack: " + attacker.Name + " target: " + target.Name);

                            //safe targeting stuff for hsbuddy
                            dirtyTargetSource = moveTodo.own.entitiyID;
                            dirtytarget = moveTodo.target.entitiyID;
                            await attacker.DoAttack(target);
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] Target is missing. Attempting recover...");
                            Helpfunctions.Instance.logg("[AI] Target " + moveTodo.target.entitiyID + "is missing. Attempting recover...");
                        }
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("[AI] Attacker is missing. Attempting recover...");
                        Helpfunctions.Instance.logg("[AI] Attacker " + moveTodo.own.entitiyID + "is missing. Attempting recover...");
                    }
				    await Coroutine.Sleep(250);
                    return;
                }

                //use ability
                if (moveTodo.actionType == actionEnum.useHeroPower)
                {
                    HSCard cardtoplay = TritonHs.OurHeroPowerCard;
                
                    if (moveTodo.target != null)
                    {
                        HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                        if (target != null)
                        {
                            dirtyTargetSource = 9000;
                            dirtytarget = moveTodo.target.entitiyID;

                            Helpfunctions.Instance.ErrorLog("use ablitiy: " + cardtoplay.Name + " target " + target.Name);
                            Helpfunctions.Instance.logg("use ablitiy: " + cardtoplay.Name + " target " + target.Name);

                            await cardtoplay.Pickup();
                            await cardtoplay.UseOn(target.Card);
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] Target is missing. Attempting recover...");
                            Helpfunctions.Instance.logg("[AI] Target " + moveTodo.target.entitiyID + "is missing. Attempting recover...");
                        }
                        await Coroutine.Sleep(500);
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("use ablitiy: " + cardtoplay.Name + " target nothing");
                        Helpfunctions.Instance.logg("use ablitiy: " + cardtoplay.Name + " target nothing");

                        await cardtoplay.Pickup();
                    }

                    return;
                }
            }

            await TritonHs.EndTurn();
        }

        private int makeChoice()
        {
            if (dirtychoice < 1)
            {
                var ccm = ChoiceCardMgr.Get();
                var lscc = ccm.m_lastShownChoiceState;
                GAME_TAG choiceMode = GAME_TAG.CHOOSE_ONE;
                int sourceEntityId = -1;
                CardDB.cardIDEnum sourceEntityCId = CardDB.cardIDEnum.None;
                if (lscc != null)
                {
                    sourceEntityId = lscc.m_sourceEntityId;
                    Entity entity = GameState.Get().GetEntity(lscc.m_sourceEntityId);
                    sourceEntityCId = CardDB.Instance.cardIdstringToEnum(entity.GetCardId());
                    if (entity != null)
                    {
                        var sourceCard = entity.GetCard();
                        if (sourceCard != null)
                        {
                            if (sourceCard.GetEntity().HasReferencedTag(GAME_TAG.DISCOVER))
                            {
                                choiceMode = GAME_TAG.DISCOVER;
                                dirtychoice = -1;
                            }
                            else if (sourceCard.GetEntity().HasReferencedTag(GAME_TAG.ADAPT))
                            {
                                choiceMode = GAME_TAG.ADAPT;
                                dirtychoice = -1;
                            }
                        }
                    }
                }

                Ai ai = Ai.Instance;
                List<Handmanager.Handcard> discoverCards = new List<Handmanager.Handcard>();
                float bestDiscoverValue = -2000000;
                var choiceCardMgr = ChoiceCardMgr.Get();
                var cards = choiceCardMgr.GetFriendlyCards();
        
                for (int i = 0; i < cards.Count; i++)
                {
                    var hc = new Handmanager.Handcard();
                    hc.card = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(cards[i].GetCardId()));
                    hc.position = 100 + i;
                    hc.entity = cards[i].GetEntityId();
                    hc.manacost = hc.card.calculateManaCost(ai.nextMoveGuess);
                    discoverCards.Add(hc);
                }

                int sirFinleyChoice = -1;
                if (ai.bestmove == null) Log.ErrorFormat("[Tick] Can't get cards. ChoiceCardMgr is empty");
                else if (ai.bestmove.actionType == actionEnum.playcard && ai.bestmove.card.card.name == CardDB.cardName.sirfinleymrrgglton)
                {
                    sirFinleyChoice = ai.botBase.getSirFinleyPriority(discoverCards);
                }
                if (choiceMode != GAME_TAG.DISCOVER) sirFinleyChoice = -1;

                DateTime tmp = DateTime.Now;
                if (sirFinleyChoice != -1) dirtychoice = sirFinleyChoice;
                else
                {
                    int dirtyTwoTurnSim = ai.mainTurnSimulator.getSecondTurnSimu();
                    ai.mainTurnSimulator.setSecondTurnSimu(true, 50);
                    using (TritonHs.Memory.ReleaseFrame(true))
                    {
                        Playfield testPl = new Playfield();
                        Playfield basePlf = new Playfield(ai.nextMoveGuess);
                        for (int i = 0; i < 3; i++)
                        {
                            Playfield tmpPlf = new Playfield(basePlf);
                            tmpPlf.isLethalCheck = false;
                            float bestval = bestDiscoverValue;
                            switch (choiceMode)
                            {
                                case GAME_TAG.DISCOVER:
                                    tmpPlf.owncards[tmpPlf.owncards.Count - 1] = discoverCards[i];
                                    bestval = ai.mainTurnSimulator.doallmoves(tmpPlf);
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodimp) bestval -= 20;
                                    break;
                                case GAME_TAG.ADAPT:
                                    bool found = false;
                                    foreach (Minion m in tmpPlf.ownMinions)
                                    {
                                        if (m.entitiyID == sourceEntityId)
                                        {
                                            bool forbidden = false;
                                            switch (discoverCards[i].card.cardIDenum)
                                            {
                                                case CardDB.cardIDEnum.UNG_999t5: if (m.cantBeTargetedBySpellsOrHeroPowers) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t6: if (m.taunt) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t7: if (m.windfury) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t8: if (m.divineshild) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t10: if (m.stealth) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t13: if (m.poisonous) forbidden = true; break;
                                            }
                                            if (forbidden) bestval = -2000000;
                                            else
                                            {
                                                discoverCards[i].card.sim_card.onCardPlay(tmpPlf, true, m, 0);
                                                bestval = ai.mainTurnSimulator.doallmoves(tmpPlf);
                                            }
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) Log.ErrorFormat("[AI] sourceEntityId is missing");
                                    break;
                            }
                            if (bestDiscoverValue <= bestval)
                            {
                                bestDiscoverValue = bestval;
                                dirtychoice = i;
                            }
                        }
                    }
                    ai.mainTurnSimulator.setSecondTurnSimu(true, dirtyTwoTurnSim);
                }
                if (sourceEntityCId == CardDB.cardIDEnum.UNG_035) dirtychoice = new Random().Next(0, 2);
                if (dirtychoice == 0) dirtychoice = 1;
                else if (dirtychoice == 1) dirtychoice = 0;
                int ttf = (int)(DateTime.Now - tmp).TotalMilliseconds;
                Helpfunctions.Instance.logg("discover card: " + dirtychoice + " " + discoverCards[1].card.cardIDenum + " " + discoverCards[0].card.cardIDenum + " " + discoverCards[2].card.cardIDenum);
                if (ttf < 3000) return (new Random().Next(ttf < 1300 ? 1300 - ttf : 0, 3100 - ttf));
            }
            else
            {
                Helpfunctions.Instance.logg("chooses the card: " + dirtychoice);
                return (new Random().Next(1100, 3200));
            }
            return 0;
        }

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <returns></returns>
        public async Task OpponentTurnLogic()
        {
            Log.InfoFormat("[OpponentTurn]");


        }

        #endregion

        #region ArenaDraft

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ArenaDraftLogic(ArenaDraftData data)
        {
            Log.InfoFormat("[ArenaDraft]");

            // We don't have a hero yet, so choose one.
            if (data.Hero == null)
            {
                Log.InfoFormat("[ArenaDraft] Hero: [{0} ({3}) | {1} ({4}) | {2} ({5})].",
                    data.Choices[0].EntityDef.CardId, data.Choices[1].EntityDef.CardId, data.Choices[2].EntityDef.CardId,
                    data.Choices[0].EntityDef.Name, data.Choices[1].EntityDef.Name, data.Choices[2].EntityDef.Name);

                // Quest support logic!
                var questIds = TritonHs.CurrentQuests.Select(q => q.Id).ToList();
                foreach (var choice in data.Choices)
                {
                    var @class = choice.EntityDef.Class;
                    foreach (var questId in questIds)
                    {
                        if (TritonHs.IsQuestForClass(questId, @class))
                        {
                            data.Selection = choice;
                            Log.InfoFormat(
                                "[ArenaDraft] Choosing hero \"{0}\" because it matches a current quest.",
                                data.Selection.EntityDef.Name);
                            return;
                        }
                    }
                }

                // TODO: I'm sure there's a better way to do this, but w/e, no time to waste right now.

                // #1
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass1)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the first preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #2
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass2)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the second preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #3
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass3)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the third preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #4
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass4)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the fourth preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #5
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass5)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the fifth preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // Choose a random hero.
                data.RandomSelection();

                Log.InfoFormat(
                    "[ArenaDraft] Choosing hero \"{0}\" because no other preferred arena classes were available.",
                    data.Selection.EntityDef.Name);

                return;
            }

            // Normal card choices.
            Log.InfoFormat("[ArenaDraft] Card: [{0} ({3}) | {1} ({4}) | {2} ({5})].", data.Choices[0].EntityDef.CardId,
                data.Choices[1].EntityDef.CardId, data.Choices[2].EntityDef.CardId, data.Choices[0].EntityDef.Name,
                data.Choices[1].EntityDef.Name, data.Choices[2].EntityDef.Name);

            /*Log.InfoFormat("[ArenaDraft] Current Deck:");
            foreach (var entry in data.Deck)
            {
                Log.InfoFormat("[ArenaDraft] {0} ({1})", entry.CardId, entry.Name);
            }

            var actor =
                data.Choices.Where(c => ArenavaluesReader.Get.ArenaValues.ContainsKey(c.EntityDef.CardId))
                    .OrderByDescending(c => ArenavaluesReader.Get.ArenaValues[c.EntityDef.CardId]).FirstOrDefault();
            if (actor != null)
            {
                data.Selection = actor;
            }
            else
            {
                data.RandomSelection();
            }
        }

        #endregion

        #region Handle Quests

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task HandleQuestsLogic(QuestData data)
        {
            Log.InfoFormat("[HandleQuests]");

            // Loop though all quest tiles.
            foreach (var questTile in data.QuestTiles)
            {
                // If we can't cancel a quest, we shouldn't try to.
                if (questTile.IsCancelable)
                {
	                if (DefaultRoutineSettings.Instance.QuestIdsToCancel.Contains(questTile.Achievement.Id))
	                {
						// Mark the quest tile to be canceled.
						questTile.ShouldCancel = true;

                        StringBuilder questsInfo = new StringBuilder("", 1000);
                        questsInfo.Append("[HandleQuests] List of quests: ");
                        int qNum = data.QuestTiles.Count;
                        for (int i = 0; i < qNum; i++ )
                        {
                            var q = data.QuestTiles[i].Achievement;
                            if (q.RewardData.Count > 0)
                            {
                                questsInfo.Append("[").Append(q.RewardData[0].Count).Append("x ").Append(q.RewardData[0].Type).Append("] ");
                            }
                            questsInfo.Append(q.Name);
                            if (i < qNum - 1) questsInfo.Append(", ");
                        }
                        questsInfo.Append(". Try to cancel: ").Append(questTile.Achievement.Name);
                        Log.InfoFormat(questsInfo.ToString());
                        await Coroutine.Sleep(new Random().Next(4000, 8000));
						return;
					}
                    // We never want to do this specific quest, if we've not started it.
                    // User logic may vary though, but this is just an example.
                    /*if (questTile.Achievement.Id == 33 && questTile.Achievement.CurProgress == 0)
                    {
                        // Mark the quest tile to be canceled.
                        questTile.ShouldCancel = true;
                        // We can only cancel *1* quest, so no point trying to process the rest.
                        return;
                    }
                }
                else if (DefaultRoutineSettings.Instance.QuestIdsToCancel.Count > 0)
                {
                    Log.InfoFormat("We can't cancel the quest.");
                }
            }
        }

        #endregion

        #endregion

        #region Override of Object

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        #endregion

        private void GameEventManagerOnGameOver(object sender, GameOverEventArgs gameOverEventArgs)
        {
            Log.InfoFormat("[GameEventManagerOnGameOver] {0}{2} => {1}.", gameOverEventArgs.Result,
                GameEventManager.Instance.LastGamePresenceStatus, gameOverEventArgs.Conceded ? " [conceded]" : "");
        }

        private void GameEventManagerOnNewGame(object sender, NewGameEventArgs newGameEventArgs)
        {
            Log.InfoFormat("[Set new log file:] Start");
            Hrtprozis prozis = Hrtprozis.Instance;
            prozis.clearAllNewGame();
            Silverfish.Instance.setnewLoggFile();
            Log.InfoFormat("[Set new log file:] End");
        }

        private void GameEventManagerOnQuestUpdate(object sender, QuestUpdateEventArgs questUpdateEventArgs)
        {
            Log.InfoFormat("[GameEventManagerOnQuestUpdate]");
            foreach (var quest in TritonHs.CurrentQuests)
            {
                Log.InfoFormat("[GameEventManagerOnQuestUpdate][{0}]{1}: {2} ({3} / {4}) [{6}x {5}]", quest.Id, quest.Name, quest.Description, quest.CurProgress,
                    quest.MaxProgress, quest.RewardData[0].Type, quest.RewardData[0].Count);
            }
        }

        private void GameEventManagerOnArenaRewards(object sender, ArenaRewardsEventArgs arenaRewardsEventArgs)
        {
            Log.InfoFormat("[GameEventManagerOnArenaRewards]");
            foreach (var reward in arenaRewardsEventArgs.Rewards)
            {
                Log.InfoFormat("[GameEventManagerOnArenaRewards] {1}x {0}.", reward.Type, reward.Count);
            }
        }        

        private HSCard getEntityWithNumber(int number)
        {
            foreach (HSCard e in getallEntitys())
            {
                if (number == e.EntityId) return e;
            }
            return null;
        }

        private HSCard getCardWithNumber(int number)
        {
            foreach (HSCard e in getallHandCards())
            {
                if (number == e.EntityId) return e;
            }
            return null;
        }

        private List<HSCard> getallEntitys()
        {
            var result = new List<HSCard>();
            HSCard ownhero = TritonHs.OurHero;
            HSCard enemyhero = TritonHs.EnemyHero;
            HSCard ownHeroAbility = TritonHs.OurHeroPowerCard;
            List<HSCard> list2 = TritonHs.GetCards(CardZone.Battlefield, true);
            List<HSCard> list3 = TritonHs.GetCards(CardZone.Battlefield, false);

            result.Add(ownhero);
            result.Add(enemyhero);
            result.Add(ownHeroAbility);

            result.AddRange(list2);
            result.AddRange(list3);

            return result;
        }

        private List<HSCard> getallHandCards()
        {
            List<HSCard> list = TritonHs.GetCards(CardZone.Hand, true);
            return list;
        }
    }
}
*/
