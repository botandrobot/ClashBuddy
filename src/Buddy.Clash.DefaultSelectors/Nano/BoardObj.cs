namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;
    
    public enum boardObjType
    {
        NONE,
        BUILDING,
        MOB,
        AOE,
        PROJECTILE
    }

    public enum transportType
    {
        NONE,
        AIR,
        GROUND
    }

    public enum targetType
    {
        NONE,
        ALL,
        GROUND,
        BUILDINGS,
        IGNOREBUILDINGS
    }

    public enum affectType
    {
        NONE,
        ALL,
        ONLY_OWN,
        ONLY_ENEMIES
    }


    public class BoardObj
    {
        public boardObjType type = boardObjType.NONE;        
        public transportType Transport = transportType.NONE; //-Mob (Air, Ground)
        public targetType TargetType = targetType.NONE; //-AttacksAir, TargetOnlyBuildings
        public affectType EffectType = affectType.NONE;
        //-        public bool isHero = false; решить вопрос с башнями - сюда или в бордТип
        public VectorAI Position;
        public bool own = true;
        public int ownerIndex = -1;
        public int pID = 0;
        public int Line = 0; //1-left, 2-right
        
        public CardDB.cardName Name = CardDB.cardName.unknown;
        public CardDB.Card card = CardDB.Instance.unknownCard;
        public uint GId = 0; //-All
        public int cost = 0; //All
        public int DeployTime = 0; //-All
        public int MaxHP = 0; //-All
        public int HP = 0; //-All
        public int Atk = 0; //-All Damage
        public int Shield = 0; //-Mob
        public int Speed = 0; //-Mob
        public int HitSpeed = 0; //-All
        public int MinRange = 0; //-Mob+AreaEffect Radius
        public int Range = 0; //-Mob+AreaEffect Radius
        public int SightRange = 0; //-Mob
        public int MaxTargets = 0; //-All
        public int DamageRadius = 0;
        public bool attacking = false;
        public BoardObj attacker = null;
        public bool attacked = false;
        public BoardObj target = null;
        public int DeathEffect = 0;
        public int Tower = 0; //1,2 - PrincessTower, 11,12 - KingsTower
        public int level = 0;


        public int LifeTime = 0; //-Buildings+AreaEffect LifeDuration
        public int SpawnNumber = 0; //-Mobs+Buildings
        public int SpawnTime = 0; //-Mobs+Buildings SpawnStartTime for balle and SpawnPauseTime for CardDB
        public int SpawnInterval = 0; //-Mobs+Buildings
        public int SpawnCharacterLevel = 0;


        public bool frozen = false;
        public int startFrozen = 0; //-dstatime or %
        public string extraData = "";

        //        public int Boost //-AreaEffect
        //        public int SlowTarget  //-AreaEffect
        //        public	byte	NoEffectToCrownTowers //-AreaEffect
        //        public	byte	OnlyEnemies //-AreaEffect
        //        public	byte	OnlyOwnTroops //-AreaEffect  





        /*
            AttractPercentage
            BuffTime
            DarkMirror
            DestIndicatorEffect
            DurationSeconds
            HealPerSecond
            HitBiggestTargets
            InstantDamage
            InstantHeal
            SummonRadius
            TID_INFO
            DamageSpecial
            Stun Duration

    public	byte	FlyFromGround
    public	Character	AttachedCharacter
    public	Effect	DamageEffect
    public	Effect	DashEffect
    public	Effect	DeathEffect
    public	Effect	FlameEffect1
    public	Effect	LandingEffect
    public	Effect	LoopingEffect
    public	Effect	OneShotEffect
    public	Effect	SpawnCharacterEffect
    public	Effect	SpawnEffect
    public	Effect	TakeDamageEffect
    public	Effect	TargettedDamageEffect1
    public	Projectile	Projectile
    public	byte	CapBuffTimeToAreaEffectTime
    public	byte	Clone
    public	byte	GeneratesMana
    public	byte	HitsAir
    public	byte	JumpEnabled
    public	byte	Mirror
    public	int	ActivationTime
    public	int	AreaBuffRadius
    public	int	AreaBuffTime
    public	int	AreaDamageRadius
    public	int	AttackDashTime
    public	int	AttackShakeTime
    public	int	BuffNumber
    public	int	Burst
    public	int	ChargeRange
    public	int	CollisionRadius
    public	int	CrownTowerDamagePercent
    public	int	DashMinRange
    public	int	DashRadius
    public	int	DeathDamageRadius
    public	int	DeathSpawnCount
    public	int	DeathSpawnDeployTime
    public	int	DeployTimerDelay
    public	int	Height
    public	int	ManaCollectAmount
    public	int	MaximumTargets
    public	int	MinimumRange
    public	int	SpawnCharacterLevelIndex
    public	int	SpawnInitialDelay
    public	int	SpawnInterval
    public	int	SpawnLimit
    public	int	SpawnMaxCount
    public	int	SpawnPauseTime
    public	int	SpawnStartTime
    public	int	SpecialAttackInterval
    public	int	Speed1
    public	int	Speed2
    public	int	StopMovementAfterMs
    public	int	StopTimeAfterAttack
    public	int	StopTimeAfterSpecialAttack
    public	int	SummonCharacterLevelIndex
    public	int	SummonCharacterSecondCount
    public	int	SummonNumber*/
        //  public	int	DeployDelay
        //    public	int	CustomDeployTime


        //        public	Rarity	Rarity
        //   public	Effect	HideEffect
        //   public	Effect	KamikazeEffect
        //    public	Character	DeathSpawnCharacter
        //    public	CharacterBuff	AreaBuff
        //    public	CharacterBuff	Buff
        //    public	Effect	HitEffect
        //    public	Effect	MorphEffect
        //    public	Effect	MoveEffect
        //     public	CharacterBuff	BuffOnDamage
        //     public	Effect	AttackStartEffect
        //     public	Effect	FlameEffect3
        //     public	Effect	SpawnPathfindEffect
        //     public	byte	DeathSpawnPushback
        //     public	byte	IgnorePushback
        //      public	CharacterBuff	StartingBuff
        //      public	Effect	FlameEffect2
        //      public	Effect	ProjectileEffect
        //      public	Effect	TargetedHitEffectSpecial
        //       public	CharacterBuff	BuffType
        //       public	Effect	TargettedDamageEffect2
        //       public	Effect	TargettedDamageEffect3

        //public	Ability	Ability
        //public	AreaEffectObject	AppearAreaObject
        // public	AreaEffectObject	AreaEffect
        //  public	AreaEffectObject	AreaEffectOnDash
        //  public	Effect	_water
        //  public	byte	Kamikaze
        //   public	AreaEffectObject	AreaEffectOnMorph
        //   public	AreaEffectObject	SpawnsAOE
        //   public	AreaEffectObject(MemPtr	address);
        //   public	Arena	UnlockArena
        //   public	Effect	AppearEffect
        //   public	Effect	LoadAttackEffect1
        //   public	NativeString	BlueShieldExportName
        //   public	byte	AllTargetsHit
        //   public	byte	AttacksGround
        //   public	byte	BurstKeepTarget
        //   public	byte	CanDeployOnEnemySide
        //   public	byte	DontStopMoveAnim
        //   public	byte	Field17D
        //   public	byte	HasRotationOnTimeline
        //   public	byte	HideBeforeFirstHit
        //   public	byte	HideRadiusIndicator
        //   public	byte	HidesWhenNotAttacking
        //   public	byte	HitsBiggestTargets
        //   public	byte	IgnoreBuildings
        //   public	byte	IsKingTowerMiddle
        //   public	byte	IsSummonerTower
        //   public	byte	LoadFirstHit
        //   public	byte	NotInUse
        //   public	byte	ProjectilesToCenter
        //   public	byte	RetargetAfterAttack
        //   public	byte	SelfAsAoeCenter
        //   public	byte	SpellAsDeploy
        //   public	int	AppearPushbackRadius
        //   public	int	DashPushBack
        //   public	int	ReleaseDate
        //   public	int	SpawnPushback
        //   public	int	SpawnPushbackRadius
        //   public	int	SpawnRadius
        //    public	AreaEffectObject	DeathAreaEffect
        //    public	AreaEffectObject	SpawnAreaObject
        //    public	Character	Field94
        //    public	Character	SpawnPathfindMorph
        //    public	Character(MemPtr	address);
        //    public	Effect	AttackStartEffectSpecial
        //    public	Effect	ChargeEffect
        //    public	Effect	ContinuousEffect
        //    public	Effect	DamageEffectSpecial
        //    public	Effect	DamageLevelTransitionEffect12
        //    public	Effect	DamageLevelTransitionEffect23
        //    public	Effect	DashStartEffect
        //    public	Effect	LoadAttackEffect2
        //    public	Effect	LoadAttackEffect3
        //    public	Effect	LoadAttackEffectReady
        //    public	ExternalPtr<HealthBar>	HealthBar
        //    public	NativeString	BlueExportName
        //    public	NativeString	DeployBaseAnimExportName
        //    public	NativeString	RedExportName
        //    public	NativeString	RedShieldExportName
        //    public	NativeString	SpawnDeployBaseAnim
        //    public	Projectile	CustomFirstProjectile
        //    public	byte	AffectsHidden
        //    public	byte	ControlsBuff
        //    public	byte	HitsGround
        //    public	byte	ManaCostFromSummonerMana
        //    public	byte	ShowHealthNumber
        //    public	byte	SpecialAttackWhenHidden
        //    public	byte	UseAnimator
        //    public	int	AttachedCharacterHeight
        //    public	int	BuffTimeIncreaseAfterTournamentCap
        //    public	int	BurstDelay
        //    public	int	ChargeSpeedMultiplier
        //    public	int	DashMaxRange
        //    public	int	HealthBarOffsetY
        //    public	int	JumpSpeed
        //    public	int	LoadTime
        //    public	int	ProjectileStartHeight
        //    public	int	PushBack
        //    public	int	ShieldDiePushback
        //    public	int	SightClip
        //    public	int	SightRange
        //    public	int	SpawnAngleShift
        //    public	int	SpawnAreaObjectLevelIndex
        //    public	int	SpawnPathfindSpeed
        //    public	int	StartingBuffTime
        ////    public	int	TurretMovement
        //     public	Character	SummonCharacter
        //     public	Character	SummonCharacterSecond
        //     public	Effect	Effect
        //     public	Effect	ProjectileEffectSpecial
        //     public	Effect	ScaledEffect
        //     public	Effect	ShieldLostEffect
        //     public	Projectile	ProjectileSpecial
        //     public	byte	CanPlaceOnBuildings
        //     public	byte	HealOnMorph
        //     public	byte	StatsUnderInfo
        //     public	int	AppearPushback
        //     public	int	AttackPushBack
        //     public	int	BuffTimeIncreasePerLevel
        //     public	int	DashCooldown
        //     public	int	DeathPushBack
        //     public	int	DeathSpawnRadius
        //     public	int	ElixirProductionStopTime
        //     public	int	Field50
        //     public	int	FlyingHeight
        //     public	int	GrowSize
        //     public	int	GrowTime
        //     public	int	HideTimeMs
        //     public	int	JumpHeight
        //     public	int	LifeDurationIncreaseAfterTournamentCap
        //     public	int	ManaGenerateLimit
        //     public	int	ManaGenerateTimeMs
        //     public	int	MultipleProjectiles
        //     public	int	MultipleTargets
        //     public	int	ProjectileStartRadius
        //     public	int	ProjectileStartZ
        //     public	int	RotateAngleSpeed
        //     public	int	TileSizeOverride
        //     public	int	UpTimeMs
        //     public	int	VisualHitSpeed
        //     public	int	WaitMs
        //     public	int	WalkingSpeedTweakPercentage
        //      public	Character	MorphCharacter
        //      public	Character	SpawnCharacter
        //      public	Effect	TargetedHitEffect
        //      public	int	BuffOnDamageTime
        //      public	int	LifeDurationIncreasePerLevel
        //      public	int	Mass
        //      public	int	ProjectileYOffset
        //      public	int	ShadowX
        //      public	int	ShadowY
        //      public	int	SightClipSide

        public BoardObj()
        {

        }

        public BoardObj(BoardObj bo)
        {
            this.Name = bo.Name;
            this.card = bo.card;
            this.type = bo.type;
            this.Transport = bo.Transport;
            this.EffectType = bo.EffectType;
            this.Position = bo.Position;
            this.own = bo.own;
            this.pID = bo.pID; //-????????
            this.Line = bo.Line;
            this.GId = bo.GId;
            this.cost = bo.cost;
            this.DeployTime = bo.DeployTime;
            this.DamageRadius = bo.DamageRadius;
            this.MaxHP = bo.MaxHP;
            this.HP = bo.HP;
            this.Atk = bo.Atk;
            this.Shield = bo.Shield;
            this.Speed = bo.Speed;
            this.HitSpeed = bo.HitSpeed;
            this.MinRange = bo.MinRange;
            this.Range = bo.Range;
            this.SightRange = bo.SightRange;
            this.TargetType = bo.TargetType;
            this.MaxTargets = bo.MaxTargets;
            this.attacking = bo.attacking;
            this.attacked = bo.attacked;
            this.LifeTime = bo.LifeTime;
            this.SpawnNumber = bo.SpawnNumber;
            this.SpawnInterval = bo.SpawnInterval;
            this.SpawnTime = bo.SpawnTime;
            this.SpawnCharacterLevel = bo.SpawnCharacterLevel;
            this.frozen = bo.frozen;
            this.startFrozen = bo.startFrozen;
            this.attacker = bo.attacker;
            this.target = bo.target;
            this.DeathEffect = bo.DeathEffect;
            this.Tower = bo.Tower;
            this.extraData = bo.extraData;
        }

        public BoardObj(CardDB.cardName cName, int lvl = 0)
        {
            CardDB.Card c = CardDB.Instance.getCardDataFromName(cName, lvl);
            this.card = c;
            this.Name = c.name;
            this.type = c.type;
            this.Transport = c.Transport;
            this.EffectType = c.EffectType;
            //this.Position = c.Position;
            //this.own = c.own;
            //this.pID = c.pID; //-????????
            //this.Line = c.Line;
            //this.GId = c.GId;
            this.cost = c.cost;
            this.DeployTime = c.DeployTime;
            this.DamageRadius = c.DamageRadius;
            this.MaxHP = c.MaxHP;
            this.HP = c.MaxHP;
            this.Atk = c.Atk;
            this.Shield = c.Shield;
            this.Speed = c.Speed;
            this.HitSpeed = c.HitSpeed;
            this.MinRange = c.MinRange;
            this.Range = c.MaxRange;
            this.SightRange = c.SightRange;
            this.TargetType = c.TargetType;
            this.MaxTargets = c.MaxTargets;
            //this.attacking = c.attacking;
            //this.attacked = c.attacked;
            this.LifeTime = c.LifeTime;
            this.SpawnNumber = c.SpawnNumber;
            this.SpawnInterval = c.SpawnInterval;
            this.SpawnTime = c.SpawnTime;
            this.SpawnCharacterLevel = c.SpawnCharacterLevel;
            //this.frozen = c.frozen;
            //this.startFrozen = c.startFrozen;
            //this.attacker = c.attacker;
            //this.target = c.target;
            this.DeathEffect = c.DeathEffect;
            //this.Tower = c.Tower;
        }

        /*
        public BoardObj(Engine.NativeObjects.Logic.GameObjects.Character @char) //TODO: remove this: заполнять карты либо через _carddb.txt либо через менеджер коллекции (спросить Рене где он)
        {
            var data = @char.LogicGameObjectData;

            this.Name = CardDB.Instance.cardNamestringToEnum(data.Name.Value);
            this.level = 1; //TODO: get real value

            this.type = data.DeployTime == 0 ? boardObjType.BUILDING : data.LifeTime > 0 ? boardObjType.BUILDING : boardObjType.MOB;
            this.Transport = data.FlyingHeight > 0 ? transportType.AIR : transportType.GROUND; //TODO: do it correct. To Rene - not wrapped FlyFromGround
            //this.EffectType =

            if (data.IgnorePushback == 1) this.TargetType = targetType.BUILDINGS;
            else if (data.AttacksAir == 1) this.TargetType = targetType.ALL;
            else if (data.AttacksGround == 1) this.TargetType = targetType.GROUND;

            this.Position = @char.StartPosition; //TODO: just for info - comment it
            //this.own = @char.OwnerIndex;
            //this.pID
            this.GId = @char.GlobalId; //TODO: just for info - comment it
            this.cost = @char.Mana; //TODO: for Rene - strange or wrong value - for Rene (but in Spell it is correct data)
            this.DeployTime = data.DeployTime; //TODO: check it - strange or wrong value
            this.MaxHP = @char.HealthComponent.Health;
            this.HP = @char.HealthComponent.CurrentHealth;
            this.Atk = 200; //TODO: for Rene - should be done in core - find this value
            this.Shield = Shield = @char.HealthComponent.ShieldHealth;
            this.Speed = 1200; //TODO: find this value
            this.HitSpeed = data.HitSpeed;
            //this.MinRange = this.MinRange; TODO: проверить надо ли вообще для какого либо bo
            this.Range = data.Range;
            this.SightRange = data.SightRange;
            this.MaxTargets = 1; //TODO: find real value
            //this.attacking = 
            //this.attacked = 
            //this.target = 
            //this.attacker = 
            this.Line = @char.StartPosition.X > 8700 ? 1 : 2;

            if (data.DeployTime == 0)
            {
                switch (this.Name)
                {
                    case CardDB.cardName.princesstower: this.Tower = this.Line; break;
                    case CardDB.cardName.kingtower: this.Tower = 100; break;
                    case CardDB.cardName.kingtowermiddle: this.Tower = 1000; break;
                }
            }

            this.LifeTime = data.LifeTime; //TODO: find real value for battle stage
            this.SpawnNumber = data.SpawnNumber;
            this.SpawnInterval = data.SpawnInterval;
            this.SpawnTime = data.SpawnPauseTime;
            this.SpawnCharacterLevel = data.SpawnCharacterLevelIndex;
            //this.frozen = 
            //this.startFrozen = 
            CardDB.Instance.collectCardInfo(this);
        }


        public BoardObj(Engine.NativeObjects.Logic.GameObjects.AreaEffectObject aoe) //TODO: remove this: заполнять карты либо через _carddb.txt либо через менеджер коллекции (спросить Рене где он)
        {
            var data = aoe.LogicGameObjectData;

            this.Name = CardDB.Instance.cardNamestringToEnum(data.Name.Value);
            this.level = 1; //TODO: get real value

            this.type = boardObjType.AOE;

            //this.Transport = 
            if (data.OnlyOwnTroops == 1) this.EffectType = affectType.ONLY_OWN;
            else if (data.OnlyEnemies == 1) this.EffectType = affectType.ONLY_ENEMIES;

            if (data.IgnoreBuildings == 1) this.TargetType = targetType.IGNOREBUILDINGS;
            else if (data.HitsAir == 1) this.TargetType = targetType.ALL;
            else if (data.HitsGround == 1) this.TargetType = targetType.GROUND;

            this.Position = aoe.StartPosition; //TODO: just for info - comment it
            //this.own = @char.OwnerIndex;
            //this.pID
            this.GId = aoe.GlobalId; //TODO: just for info - comment it
            this.cost = aoe.Mana; //TODO: for Rene - strange or wrong value - for Rene (but in Spell it is correct data)
            //this.DeployTime = 
            //this.MaxHP = 
            //this.HP = 
            this.Atk = 500; //TODO: real value
            //this.Shield = 
            this.Speed = data.Buff.SpeedMultiplier; //TODO: на самом деле там намного больше бафов - проверить их все если этих будет мало
            this.HitSpeed = data.Buff.HitSpeedMultiplier;
            //this.MinRange = this.MinRange; TODO: проверить надо ли вообще для какого либо bo
            this.Range = data.Radius;
            //this.SightRange = 
            //this.MaxTargets = 
            //this.attacking = 
            //this.attacked = 
            //this.target = 
            //this.attacker = 
            this.Line = aoe.StartPosition.X > 8700 ? 1 : 2;
            this.LifeTime = aoe.RemainingTime;
            // this.SpawnNumber = 
            this.SpawnInterval = data.SpawnInterval;
            //this.SpawnTime = 
            this.SpawnCharacterLevel = data.SpawnCharacterLevelIndex;
            //this.frozen = 
            //this.startFrozen = 

            //public int Line = 1; //1-left, 2-right
            //public bool deathEffect = false;
            CardDB.Instance.collectCardInfo(this);
        }
        */
        public string ToString(bool printAll = false)
        {
            switch (this.type)
            {
                case boardObjType.AOE:
                    if (!printAll) return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " " + this.level + " " + this.LifeTime;
                    else return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " lvl:" + this.level + " LifeTime:" + this.LifeTime +
                        " cost:" + this.cost + " EffectType:" + this.EffectType + " TargetType:" + this.TargetType +
                        " buffSpeed:" + this.Speed + " buffHitSpeed:" + this.HitSpeed + " Radius:" + this.Range + " Atk:" + this.Atk +
                        " SpawnInterval:" + this.SpawnInterval + " SpawnCharacterLevel:" + this.SpawnCharacterLevel;
                case boardObjType.MOB:
                    if (!printAll) return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " " + this.level + " " + this.LifeTime;
                    else return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " lvl:" + this.level + " LifeTime:" + this.LifeTime +
                        " cost:" + this.cost + " EffectType:" + this.EffectType + " TargetType:" + this.TargetType +
                        " buffSpeed:" + this.Speed + " buffHitSpeed:" + this.HitSpeed + " Radius:" + this.Range + " Atk:" + this.Atk +
                        " SpawnInterval:" + this.SpawnInterval + " SpawnCharacterLevel:" + this.SpawnCharacterLevel;

            }
            return "Type ERROR " + this.Name + " " + this.GId + " " + this.type;
        }

        public bool aheadOf(BoardObj bo, bool home)
        {
            if ((this.Position.Y > bo.Position.Y) == home) return true;
            else return false;
        }

        public List<attackDef> getImmediateAttackers(Playfield p)
        {
            List<attackDef> attackersList = new List<attackDef>();
            if (this.HP > 0)
            {
                List<BoardObj> enemies = this.own ? p.enemyMinions : p.ownMinions;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef bo1ad = new attackDef(this, e);
                    attackDef bo2ad = new attackDef(e, this);
                    if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                    else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
                }

                enemies = this.own ? p.enemyBuildings : p.ownBuildings;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef bo1ad = new attackDef(this, e);
                    attackDef bo2ad = new attackDef(e, this);
                    if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                    else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
                }

                if (this.Tower == 0)
                {
                    enemies = this.own ? p.enemyTowers : p.ownTowers;
                    foreach (BoardObj e in enemies)
                    {
                        if (e.Line != this.Line) continue;
                        attackDef bo1ad = new attackDef(this, e);
                        attackDef bo2ad = new attackDef(e, this);
                        if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                        else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
                    }
                }

                attackersList.Sort((a, b) => a.time.CompareTo(b.time));
            }
            return attackersList;
        }

        public List<attackDef> getPossibleAttackers(Playfield p)
        {
            List<attackDef> attackersList = new List<attackDef>();
            if (this.HP > 0)
            {
                List<BoardObj> enemies = this.own ? p.enemyMinions : p.ownMinions;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef ad = new attackDef(e, this);
                    if (!ad.empty) attackersList.Add(ad);
                }

                enemies = this.own ? p.enemyBuildings : p.ownBuildings;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef ad = new attackDef(e, this);
                    if (!ad.empty) attackersList.Add(ad);
                }

                enemies = this.own ? p.enemyTowers : p.ownTowers;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef ad = new attackDef(e, this);
                    if (!ad.empty) attackersList.Add(ad);
                }

                attackersList.Sort((a, b) => a.time.CompareTo(b.time));
            }
            return attackersList;
        }

        

        public void minionDied(Playfield p)
        {
            //TODO: possible effects
        }
        
        

    }

}