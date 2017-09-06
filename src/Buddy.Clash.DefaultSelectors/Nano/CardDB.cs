using System.Linq;

namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;
    using System.IO;


    public struct targett
    {
        public int target;
        public int targetEntity;

        public targett(int targ, int ent)
        {
            this.target = targ;
            this.targetEntity = ent;
        }
    }
    
    public class CardDB
    {
        // Data is stored in hearthstone-folder -> data->win cardxml0
        //(data-> cardxml0 seems outdated (blutelfkleriker has 3hp there >_>)

        public enum cardtrigers
        {
            newtriger,
            getBattlecryEffect,
            onAHeroGotHealedTrigger,
            onAMinionGotHealedTrigger,
            onAuraEnds,
            onAuraStarts,
            onCardIsGoingToBePlayed,
            onCardPlay,
            onCardWasPlayed,
            onDeathrattle,
            onEnrageStart,
            onEnrageStop,
            onMinionDiedTrigger,
            onMinionGotDmgTrigger,
            onMinionIsSummoned,
            onMinionWasSummoned,
            onSecretPlay,
            onTurnEndsTrigger,
            onTurnStartTrigger,
            triggerInspire
        }
        
        public enum cardName //-replace " ", ".", lower case
        {
            unknown,
            //Troops
            angrybarbarian,
            archer,
            assassin,
            axeman, //executioner
            babydragon,
            balloon,
            barbarian,
            bat,
            battleram,
            blowdartgoblin,
            bomber,
            bowler,
            brokencannon,
            darkprince,
            darkwitch,
            dartbarrell,
            electrowizard,
            firespirits,
            giant,
            giantskeleton,
            goblin,
            golem,
            golemite,
            hogrider,
            icegolemite,
            icespirits,
            icewizard,
            infernodragon,
            knight,
            lavahound,
            lavapups,
            megaknight,
            megaminion,
            miner,
            minion,
            minipekka,
            movingcannon,
            musketeer,
            pekka,
            prince,
            princess,
            ragebarbarian,
            royalgiant,
            skeleton,
            skeletonballoon,
            skeletonwarrior,
            speargoblin,
            towerprincess,
            valkyrie,
            witch,
            wizard,
            zapmachine,
            
            //-Buildings
            balloonbomb,
            barbarianhut,
            bombtower,
            cannon,
            elixircollector,
            firespirithut,
            giantskeletonbomb,
            goblinhut,
            infernotower,
            kingtower,
            kingtowermiddle,
            mortar,
            princesstower,
            ragebarbarianbottle,
            skeletoncontainer,
            tesla,
            tombstone,
            xbow,

            //-Spels/AOE
            barbarianrage,
            clone,
            freeze,
            freezeicegolemite,
            graveyard,
            heal,
            lightning,
            poison,
            rage,
            tornado,
            zap,
            
            //Projectiles
            archerarrow,
            arrowsspell,
            arrowsspelldeco,
            axemanprojectile,
            babydragonprojectile,
            batprojectile,
            blowdartgoblinprojectile,
            bombskeletonprojectile,
            bombtowerprojectile,
            bowlerprojectile,
            chr_wizardprojectile,
            dartbarrellprojectile,
            fireballspell,
            firespiritsprojectile,
            goblinbarrelspell,
            ice_wizardprojectile,
            icespiritsprojectile,
            kingprojectile,
            lavahoundprojectile,
            lavapupprojectile,
            lighningspell,
            logprojectile,
            logprojectilerolling,
            megaknightappear,
            megaminionspit,
            minionspit,
            mortarprojectile,
            movingcannonprojectile,
            musketeerprojectile,
            princessprojectile,
            princessprojectiledeco,
            rocketspell,
            royalgiantprojectile,
            speargoblinprojectile,
            towercannonball,
            towerprincessprojectile,
            witchprojectile,
            xbow_projectile,
            zapmachineprojectile,

            //Not in use
            not_in_use,
            notinuse1,
            notinuse2,
            notinuse3,
            notinuse4,
            notinuse5,
            notinuse8,
            notinuse9,
            notinuse21,
            notinuse22,


            //TODO: check names below
            //troops
            bandit,
            dartgoblin,
            elitebarbarian,
            executioner, //axeman
            goblingang,
            guards,
            icegolem,
            icespirit,
            lumberjack,
            minionhorde,
            //nightwitch,
            skeletonarmy,
            sparky,
            threemusketeers, 
            //-Spels
            arrows,
            clonespell,
            fireball,
            goblinbarrel,
            mirror,
            rocket,
            thelog,
            //-Buildings
            furnace,
        }

        public cardName cardNamestringToEnum(string s)
        {
            CardDB.cardName NameEnum;
            if (Enum.TryParse<cardName>(s.ToLower().Replace(" ", ""), false, out NameEnum)) return NameEnum; //TODO: improve
            else
            {
                Helpfunctions.Instance.logg("!!!NEW NAME: " + s);
                return CardDB.cardName.unknown;
            }
        }
        
        public class Card
        {
            public CardDB.cardName name = CardDB.cardName.unknown;
            public string stringName = "";
            public boardObjType type = boardObjType.NONE;
            public transportType Transport = transportType.NONE; //-Mob (Air, Ground)
            public targetType TargetType = targetType.NONE; //-AttacksAir, TargetOnlyBuildings
            public affectType affectType = affectType.NONE;

            public int cost = 0; //All
            public int DeployTime = 0; //-mob,buildings
            public int DeployDelay = 0; //-mob,buildings
            public int MaxHP = 0; //-All
            public int Atk = 0; //-All Damage
            public int Shield = 0; //-Mob
            public int Speed = 0; //-Mob
            public int HitSpeed = 0; //-All
            public int MinRange = 0; //-Mob+AreaEffect Radius
            public int MaxRange = 0; //-Mob+AreaEffect Radius
            public int SightRange = 0;
            public int SightClip = 0;
            public int MultipleTargets = 0; //- only ElectroWizard
            public int MultipleProjectiles = 0; //- only Princess
            public int DeathEffect = 0; //TODO:deathEffects/deathSpawn
            public string Rarity = "";
            public int Level = 0;
            public int DamageRadius = 0;
            public bool aoeGround = false; //-projectile
            public bool aoeAir = false; //-projectile
            public int CollisionRadius = 0; //-Mobs+Buildings
            public int towerDamage = 0; //aoe

            public int LifeTime = 0; //-Buildings; AreaEffect=LifeDuration
            public int SpawnNumber = 0; //-Mobs+Buildings
            public int SpawnPause = 0; //-Mobs+Buildings
            public int SpawnInterval = 0; //-Mobs+Buildings
            public int SpawnCharacterLevel = 0;
            

            public bool needUpdate = true;
            public int numDuplicates = 0;
            public int numDifferences = 0;
            public List<cardtrigers> trigers;






            //stuff for test and seech
            public string srcTargettedDamageEffect3 = "";
            public string srcTargettedDamageEffect2 = "";
            public string srcTargettedDamageEffect1 = "";
            public string srcTargetedHitEffectSpecial = "";
            public string srcTargetedHitEffect = "";
            public string srcTakeDamageEffect = "";
            public string srcTID = "";
            public string srcStartingBuff = "";
            public string srcSpecialReadyEffect = "";
            public string srcSpawnProjectile = "";
            public string srcSpawnPathfindMorph = "";
            public string srcSpawnPathfindEffect = "";
            public string srcSpawnEffect = "";
            public string srcSpawnDeployBaseAnim = "";
            public string srcSpawnCharacterEffect = "";
            public string srcSpawnCharacter = "";
            public string srcSpawnAreaObject = "";
            public string srcShieldLostEffect = "";
            public string srcShadowCustomLow = "";
            public string srcShadowCustom = "";
            public string srcRedTopExportName = "";
            public string srcRedShieldExportName = "";
            public string srcRedExportName = "";
            public string srcRarity = "";
            public string srcProjectileSpecial = "";
            public string srcProjectileEffectSpecial = "";
            public string srcProjectileEffect = "";
            public string srcProjectile = "";
            public string srcName = "";
            public string srcMoveEffect = "";
            public string srcMorphEffect = "";
            public string srcMorphCharacter = "";
            public string srcLoopingFilter = "";
            public string srcLoadAttackEffectReady = "";
            public string srcLoadAttackEffect3 = "";
            public string srcLoadAttackEffect2 = "";
            public string srcLoadAttackEffect1 = "";
            public string srcLandingEffect = "";
            public string srcKamikazeEffect = "";
            public string srcHideEffect = "";
            public string srcHealthBar = "";
            public string srcFlameEffect3 = "";
            public string srcFlameEffect2 = "";
            public string srcFlameEffect1 = "";
            public string srcFileName = "";
            public string srcDeployBaseAnimExportName = "";
            public string srcDeathSpawnProjectile = "";
            public string srcDeathSpawnCharacter = "";
            public string srcDeathEffect = "";
            public string srcDeathAreaEffect = "";
            public string srcDashStartEffect = "";
            public string srcDashFilter = "";
            public string srcDashEffect = "";
            public string srcDamageLevelTransitionEffect23 = "";
            public string srcDamageLevelTransitionEffect12 = "";
            public string srcDamageExportName = "";
            public string srcDamageEffectSpecial = "";
            public string srcDamageEffect = "";
            public string srcCustomFirstProjectile = "";
            public string srcContinuousEffect = "";
            public string srcChargeEffect = "";
            public string srcBuffOnDamage = "";
            public string srcBlueTopExportName = "";
            public string srcBlueShieldExportName = "";
            public string srcBlueExportName = "";
            public string srcAttackStartEffectSpecial = "";
            public string srcAttackStartEffect = "";
            public string srcAttachedCharacter = "";
            public string srcAreaEffectOnMorph = "";
            public string srcAreaEffectOnDash = "";
            public string srcAreaBuff = "";
            public string srcAppearEffect = "";
            public string srcAppearAreaObject = "";
            public string srcAbility = "";
            public int? srcWalkingSpeedTweakPercentage = 0;
            public int? srcWaitMS = 0;
            public int? srcVisualHitSpeed = 0;
            public int? srcVariableDamageTransitionTime = 0;
            public int? srcVariableDamageTime2 = 0;
            public int? srcVariableDamageTime1 = 0;
            public int? srcVariableDamage3 = 0;
            public int? srcVariableDamage2 = 0;
            public int? srcUpTimeMs = 0;
            public int? srcTurretMovement = 0;
            public int? srcTileSizeOverride = 0;
            public int? srcTargetEffectY = 0;
            public int? srcStopTimeAfterSpecialAttack = 0;
            public int? srcStopTimeAfterAttack = 0;
            public int? srcStopMovementAfterMS = 0;
            public int? srcStartingBuffTime = 0;
            public int? srcSpeed = 0;
            public int? srcSpecialRange = 0;
            public int? srcSpecialMinRange = 0;
            public int? srcSpecialLoadTime = 0;
            public int? srcSpecialAttackInterval = 0;
            public int? srcSpawnStartTime = 0;
            public int? srcSpawnRadius = 0;
            public int? srcSpawnPushbackRadius = 0;
            public int? srcSpawnPushback = 0;
            public int? srcSpawnPauseTime = 0;
            public int? srcSpawnPathfindSpeed = 0;
            public int? srcSpawnNumber = 0;
            public int? srcSpawnLimit = 0;
            public int? srcSpawnInterval = 0;
            public int? srcSpawnCharacterLevelIndex = 0;
            public int? srcSpawnAreaObjectLevelIndex = 0;
            public int? srcSpawnAngleShift = 0;
            public int? srcSightRange = 0;
            public int? srcSightClipSide = 0;
            public int? srcSightClip = 0;
            public int? srcShieldHitpoints = 0;
            public int? srcShieldDiePushback = 0;
            public int? srcShadowY = 0;
            public int? srcShadowX = 0;
            public int? srcShadowSkew = 0;
            public int? srcShadowScaleY = 0;
            public int? srcShadowScaleX = 0;
            public int? srcScale = 0;
            public int? srcRotateAngleSpeed = 0;
            public int? srcRange = 0;
            public int? srcPushback = 0;
            public int? srcProjectileYOffset = 0;
            public int? srcProjectileStartZ = 0;
            public int? srcProjectileStartRadius = 0;
            public int? srcNoDeploySizeW = 0;
            public int? srcNoDeploySizeH = 0;
            public int? srcMultipleTargets = 0;
            public int? srcMultipleProjectiles = 0;
            public int? srcMorphTime = 0;
            public int? srcMinimumRange = 0;
            public int? srcMass = 0;
            public int? srcManaGenerateTimeMs = 0;
            public int? srcManaGenerateLimit = 0;
            public int? srcManaCollectAmount = 0;
            public int? srcLoadTime = 0;
            public int? srcLifeTime = 0;
            public int? srcKamikazeTime = 0;
            public int? srcJumpSpeed = 0;
            public int? srcJumpHeight = 0;
            public int? srcHitpoints = 0;
            public int? srcHitSpeed = 0;
            public int? srcHideTimeMs = 0;
            public int? srcHealthBarOffsetY = 0;
            public int? srcGrowTime = 0;
            public int? srcGrowSize = 0;
            public int? srcFlyingHeight = 0;
            public int? srcDeployTimerDelay = 0;
            public int? srcDeployTime = 0;
            public int? srcDeployDelay = 0;
            public int? srcDeathSpawnRadius = 0;
            public int? srcDeathSpawnMinRadius = 0;
            public int? srcDeathSpawnDeployTime = 0;
            public int? srcDeathSpawnCount = 0;
            public int? srcDeathPushBack = 0;
            public int? srcDeathDamageRadius = 0;
            public int? srcDeathDamage = 0;
            public int? srcDashRadius = 0;
            public int? srcDashPushBack = 0;
            public int? srcDashMinRange = 0;
            public int? srcDashMaxRange = 0;
            public int? srcDashLandingTime = 0;
            public int? srcDashImmuneToDamageTime = 0;
            public int? srcDashDamage = 0;
            public int? srcDashCooldown = 0;
            public int? srcDashConstantTime = 0;
            public int? srcDamageSpecial = 0;
            public int? srcDamage = 0;
            public int? srcCrownTowerDamagePercent = 0;
            public int? srcCollisionRadius = 0;
            public int? srcChargeSpeedMultiplier = 0;
            public int? srcChargeRange = 0;
            public int? srcBurstDelay = 0;
            public int? srcBurst = 0;
            public int? srcBuffOnDamageTime = 0;
            public int? srcAttackShakeTime = 0;
            public int? srcAttackPushBack = 0;
            public int? srcAttackDashTime = 0;
            public int? srcAttachedCharacterHeight = 0;
            public int? srcAreaDamageRadius = 0;
            public int? srcAreaBuffTime = 0;
            public int? srcAreaBuffRadius = 0;
            public int? srcAppearPushbackRadius = 0;
            public int? srcAppearPushback = 0;
            public int? srcActivationTime = 0;
            public bool? srcVariableDamageLifeTime = false;
            public bool? srcUseAnimator = false;
            public bool? srcTargetOnlyBuildings = false;
            public bool? srcSpecialAttackWhenHidden = false;
            public bool? srcSpawnConstPriority = false;
            public bool? srcShowHealthNumber = false;
            public bool? srcSelfAsAoeCenter = false;
            public bool? srcRetargetAfterAttack = false;
            public bool? srcMorphKeepTarget = false;
            public bool? srcLoopMoveEffect = false;
            public bool? srcLoadFirstHit = false;
            public bool? srcKamikaze = false;
            public bool? srcJumpEnabled = false;
            public bool? srcIsSummonerTower = false;
            public bool? srcIgnorePushback = false;
            public bool? srcHidesWhenNotAttacking = false;
            public bool? srcHideBeforeFirstHit = false;
            public bool? srcHealOnMorph = false;
            public bool? srcHasRotationOnTimeline = false;
            public bool? srcFlyFromGround = false;
            public bool? srcFlyDirectPaths = false;
            public bool? srcDontStopMoveAnim = false;
            public bool? srcDeathSpawnPushback = false;
            public bool? srcDeathInheritIgnoreList = false;
            public bool? srcCrowdEffects = false;
            public bool? srcBurstKeepTarget = false;
            public bool? srcBuildingTarget = false;
            public bool? srcAttacksGround = false;
            public bool? srcAttacksAir = false;
            public bool? srcAllTargetsHit = false;







            //aoe
            public string srcSpawnsAEO = "";
            public string srcScaledEffect = "";
            public string srcOneShotEffect = "";
            public string srcLoopingEffect = "";
            public string srcHitEffect = "";
            public string srcBuff = "";
            public int? srcSpawnTime = 0;
            public int? srcSpawnMaxCount = 0;
            public int? srcSpawnInitialDelay = 0;
            public int? srcRadius = 0;
            public int? srcProjectileStartHeight = 0;
            public int? srcMaximumTargets = 0;
            public int? srcLifeDurationIncreasePerLevel = 0;
            public int? srcLifeDurationIncreaseAfterTournamentCap = 0;
            public int? srcLifeDuration = 0;
            public int? srcBuffTimeIncreasePerLevel = 0;
            public int? srcBuffTimeIncreaseAfterTournamentCap = 0;
            public int? srcBuffTime = 0;
            public int? srcBuffNumber = 0;
            public int? srcAttractPercentage = 0;
            public bool? srcProjectilesToCenter = false;
            public bool? srcOnlyOwnTroops = false;
            public bool? srcOnlyEnemies = false;
            public bool? srcNoEffectToCrownTowers = false;
            public bool? srcIgnoreBuildings = false;
            public bool? srcHitsGround = false;
            public bool? srcHitsAir = false;
            public bool? srcHitBiggestTargets = false;
            public bool? srcControlsBuff = false;
            public bool? srcClone = false;
            public bool? srcCapBuffTimeToAreaEffectTime = false;
            public bool? srcAffectsHidden = false;



            //buildings

            //projectiles

            public string srcTrailEffect = "";
            public string srcTargettedEffect = "";
            public string srcTargetBuff = "";
            public string srcSpawnAreaEffectObject = "";
            public string srcShadowExportName = "";
            public string srcScatter = "";
            public string srcRedShadowExportName = "";
            public string srcPingpongDeathEffect = "";
            public string srcHitSoundWhenParentAlive = "";
            public string srcExportName = "";
            public string srcChainedHitEndEffect = "";
            public int? srcSpawnCharacterDeployTime = 0;
            public int? srcSpawnCharacterCount = 0;
            public int? srcRandomDistance = 0;
            public int? srcRandomAngle = 0;
            public int? srcRadiusY = 0;
            public int? srcProjectileRange = 0;
            public int? srcProjectileRadiusY = 0;
            public int? srcProjectileRadius = 0;
            public int? srcPingpongVisualTime = 0;
            public int? srcMinDistance = 0;
            public int? srcMaxDistance = 0;
            public int? srcHeal = 0;
            public int? srcGravity = 0;
            public int? srcDragMargin = 0;
            public int? srcDragBackSpeed = 0;
            public int? srcCrownTowerHealPercent = 0;
            public int? srcConstantHeight = 0;
            public int? srcChainedHitRadius = 0;
            public bool? srcUse360Frames = false;
            public bool? srcTargetToEdge = false;
            public bool? srcShakesTargets = false;
            public bool? srcShadowDisableRotate = false;
            public bool? srcPushbackAll = false;
            public bool? srcHoming = false;
            public bool? srcHeightFromTargetRadius = false;
            public bool? srcAoeToGround = false;
            public bool? srcAoeToAir = false;


            //CharacterBuff
            public string srcPortalSpell = "";
            public string srcNegatesBuffs = "";
            public string srcMarkEffect = "";
            public string srcImmunityToBuffs = "";
            public string srcIconFileName = "";
            public string srcIconExportName = "";
            public string srcFilterFile = "";
            public string srcFilterExportName = "";
            public string srcEffect = "";
            public int? srcSpeedMultiplier = 0;
            public int? srcSpawnSpeedMultiplier = 0;
            public int? srcSizeMultiplier = 0;
            public int? srcHitSpeedMultiplier = 0;
            public int? srcHitFrequency = 0;
            public int? srcHealPerSecond = 0;
            public int? srcDamageReduction = 0;
            public int? srcDamagePerSecond = 0;
            public int? srcDamageMultiplier = 0;
            public int? srcAudioPitchModifier = 0;
            public bool? srcStaticTarget = false;
            public bool? srcRemoveOnHeal = false;
            public bool? srcRemoveOnAttack = false;
            public bool? srcPanic = false;
            public bool? srcInvisible = false;
            public bool? srcImmuneToAntiMagic = false;
            public bool? srcIgnorePushBack = false;
            public bool? srcFilterInheritLifeDuration = false;
            public bool? srcFilterAffectsTransformation = false;
            public bool? srcEnableStacking = false;
            public bool? srcControlledByParent = false;
            public bool? srcChangeControl = false;

            



            public Card()
            {

            }

            public Card(Card c)
            {
                //this.entityID = c.entityID;
                this.Rarity = c.Rarity;
                //TODO
            }
            
            public int getManaCost(Playfield p, int currentcost)//-calculates mana from current mana
            {
                int retval = currentcost;
                                                
                return retval;
            }

            public bool canplayCard(Playfield p, int manacost, bool own)
            {
                //if (p.mana < this.getManaCost(p, manacost)) return false;
                //if (this.getTargetsForCard(p, false, own).Count == 0) return false;
                return true;
            }

        }

        Dictionary<int, Card> forTestBase = new Dictionary<int, Card>();
        Dictionary<cardName, Card> cardNameToCardList = new Dictionary<cardName, Card>();
        List<string> allCardIDS = new List<string>();
        public Card unknownCard;
        public bool installedWrong = false;

        public Card teacherminion;
        public Card illidanminion;
        public Card lepergnome;
        public Card burlyrockjaw;
        private static CardDB instance;

        public static CardDB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardDB();
                }
                return instance;
            }
        }

        private CardDB()
        {

            this.cardNameToCardList.Clear();
            Card c = new Card();
            foreach (var e in Buddy.Clash.Engine.Csv.CsvLogic.Characters.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName);
                c.type = boardObjType.MOB;
                c.Transport = e.FlyingHeight > 0 ? transportType.AIR : transportType.GROUND;

                if (e.TargetOnlyBuildings != null && (bool)e.TargetOnlyBuildings) c.TargetType = targetType.BUILDINGS;
                else if (e.AttacksAir != null && (bool)e.AttacksAir) c.TargetType = targetType.ALL;
                else if (e.AttacksGround != null && (bool)e.AttacksGround) c.TargetType = targetType.GROUND;
                
                //c.affectType = //-aoe
                c.cost = -1; //TODO: dig this value in !Characters
                if (e.DeployTime != null) c.DeployTime = (int)e.DeployTime; //-mob,buildings
                if (e.DeployDelay != null) c.DeployDelay = (int)e.DeployDelay; //-mob,buildings
                if (e.Hitpoints != null) c.MaxHP = (int)e.Hitpoints; //-All
                if (e.Damage != null) c.Atk = (int)e.Damage; //-All Damage
                if (e.ShieldHitpoints != null) c.Shield = (int)e.ShieldHitpoints; //-Mob
                if (e.Speed != null) c.Speed = (int)e.Speed; //-Mob, Projectile
                if (e.HitSpeed != null) c.HitSpeed = (int)e.HitSpeed; //-Mob,aoe,building
                if (e.MinimumRange != null) c.MinRange = (int)e.MinimumRange; //-only Mortar
                if (e.Range != null) c.MaxRange = (int)e.Range; //-Mob,building
                if (e.SightRange != null) c.SightRange = (int)e.SightRange; //-Mob,building
                if (e.SightClip != null) c.SightClip = (int)e.SightClip; //-Mob,building
                if (e.MultipleTargets != null) c.MultipleTargets = (int)e.MultipleTargets; //-only ElectroWizard
                if (e.MultipleProjectiles != null) c.MultipleProjectiles = (int)e.MultipleProjectiles; //- only Princess
                if (e.Rarity != null) c.Rarity = e.Rarity;
                c.Level = 0;
                if (e.AreaDamageRadius != null) c.DamageRadius = (int)e.AreaDamageRadius; //mob,aoe,projectile=Radius
                //c.aoeGround = false; //-projectile
                //c.aoeAir = false; //-projectile
                if (e.CollisionRadius != null) c.CollisionRadius = (int)e.CollisionRadius; //-Mobs+Buildings          
                if (e.LifeTime != null) c.LifeTime = (int)e.LifeTime; //-Buildings; AreaEffect=LifeDuration
                if (e.SpawnPauseTime != null) c.SpawnPause = (int)e.SpawnPauseTime; //-Mobs+Buildings DataTime
                if (e.SpawnNumber != null) c.SpawnNumber = (int)e.SpawnNumber; //-Mobs+Buildings
                if (e.SpawnInterval != null) c.SpawnInterval = (int)e.SpawnInterval; //-Mobs+Buildings > 0 if SpawnNumber > 1
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = (int)e.SpawnCharacterLevelIndex;

                /*//TODO:explore real value on field for below
                 * ProjectileRange don't use, just set val to Log
                BowlerProjectile 6000 - Bowler 4500
                LogProjectileRolling 11100
                AxeManProjectile 6000 axeman 4500*/

                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Helpfunctions.Instance.ErrorLog("#####ERR. Duplicate name:" + c.name);
            }


            foreach (var e in Buddy.Clash.Engine.Csv.CsvLogic.Buildings.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName);
                c.type = boardObjType.BUILDING;
                c.Transport = e.FlyingHeight > 0 ? transportType.AIR : transportType.GROUND;

                if (e.TargetOnlyBuildings != null && (bool)e.TargetOnlyBuildings) c.TargetType = targetType.BUILDINGS;
                else if (e.AttacksAir != null && (bool)e.AttacksAir) c.TargetType = targetType.ALL;
                else if (e.AttacksGround != null && (bool)e.AttacksGround) c.TargetType = targetType.GROUND;
                
                //c.affectType = //-aoe
                c.cost = -1; //TODO: dig this value in !Characters
                if (e.DeployTime != null) c.DeployTime = (int)e.DeployTime; //-mob,buildings
                if (e.DeployDelay != null) c.DeployDelay = (int)e.DeployDelay; //-mob,buildings
                if (e.Hitpoints != null) c.MaxHP = (int)e.Hitpoints; //-All
                if (e.Damage != null) c.Atk = (int)e.Damage; //-All Damage
                if (e.ShieldHitpoints != null) c.Shield = (int)e.ShieldHitpoints; //-Mob
                if (e.Speed != null) c.Speed = (int)e.Speed; //-Mob, Projectile
                if (e.HitSpeed != null) c.HitSpeed = (int)e.HitSpeed; //-Mob,aoe,building
                if (e.MinimumRange != null) c.MinRange = (int)e.MinimumRange; //-only Mortar
                if (e.Range != null) c.MaxRange = (int)e.Range; //-Mob,building
                if (e.SightRange != null) c.SightRange = (int)e.SightRange; //-Mob,building
                if (e.SightClip != null) c.SightClip = (int)e.SightClip; //-Mob,building
                if (e.MultipleTargets != null) c.MultipleTargets = (int)e.MultipleTargets; //-only ElectroWizard
                if (e.MultipleProjectiles != null) c.MultipleProjectiles = (int)e.MultipleProjectiles; //- only Princess
                if (e.Rarity != null) c.Rarity = e.Rarity;
                c.Level = 0;
                if (e.AreaDamageRadius != null) c.DamageRadius = (int)e.AreaDamageRadius; //mob,aoe,projectile=Radius TODO: match troop and projectile Заполняем сначала минионов, здания, аое - потом проджектил и оттуда кейсом тупо сопоставляем с минионом
                //c.aoeGround = false; //-projectile
                //c.aoeAir = false; //-projectile
                if (e.CollisionRadius != null) c.CollisionRadius = (int)e.CollisionRadius; //-Mobs+Buildings          
                if (e.LifeTime != null) c.LifeTime = (int)e.LifeTime; //-Buildings; AreaEffect=LifeDuration
                if (e.SpawnPauseTime != null) c.SpawnPause = (int)e.SpawnPauseTime; //-Mobs+Buildings DataTime
                if (e.SpawnNumber != null) c.SpawnNumber = (int)e.SpawnNumber; //-Mobs+Buildings
                if (e.SpawnInterval != null) c.SpawnInterval = (int)e.SpawnInterval; //-Mobs+Buildings > 0 if SpawnNumber > 1
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = (int)e.SpawnCharacterLevelIndex;
                
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Helpfunctions.Instance.ErrorLog("#####ERR. Duplicate name:" + c.name);
            }
            
            foreach (var e in Buddy.Clash.Engine.Csv.CsvLogic.AreaEffectObjects.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName);
                c.type = boardObjType.AOE;
                if (e.IgnoreBuildings != null) c.TargetType = (bool)e.IgnoreBuildings ? targetType.IGNOREBUILDINGS : targetType.ALL;

                if (e.OnlyEnemies != null)
                {
                    if ((bool)e.OnlyEnemies)
                    {
                        if (e.OnlyOwnTroops != null && (bool)e.OnlyOwnTroops) c.affectType = affectType.ALL;
                        else c.affectType = affectType.ONLY_ENEMIES;
                    }
                }
                else if (e.OnlyOwnTroops != null && (bool)e.OnlyOwnTroops) c.affectType = affectType.ONLY_OWN;
                
                c.cost = -1; //TODO: dig this value in !Characters
                //c.DeployTime =  //-mob,buildings
                //c.DeployDelay = //-mob,buildings
                //c.MaxHP = 
                //c.Atk = (int)e.Damage; //no one real values :(
                //c.Shield = 
                //c.Speed = 
                if (e.HitSpeed != null) c.HitSpeed = (int)e.HitSpeed; //-Mob,aoe,building
                //c.MinRange = 
                //c.MaxRange = 
                //c.SightRange = 
                //c.SightClip = 
                //c.MultipleTargets = 
                //c.MultipleProjectiles = 
                //c.Rarity = e.Rarity;
                c.Level = 0;
                if (e.Radius != null) c.DamageRadius = (int)e.Radius; //mob,aoe,projectile=Radius
                //c.aoeGround = false; //-projectile
                //c.aoeAir = false; //-projectile
                //c.CollisionRadius = (        
                if (e.LifeDuration != null) c.LifeTime = (int)e.LifeDuration; //-Buildings; AreaEffect=LifeDuration
                //c.SpawnPause = 
                //c.SpawnNumber =  //-Mobs+Buildings
                if (e.SpawnInterval != null) c.SpawnInterval = (int)e.SpawnInterval; //-Mobs+Buildings > 0 if SpawnNumber > 1
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = (int)e.SpawnCharacterLevelIndex;

                switch (c.name)
                {
                    case cardName.poison: c.Atk = 57; c.towerDamage = 23; break;
                    case cardName.tornado: c.Atk = 44; break;
                    case cardName.heal: c.Atk = -100; break;
                    case cardName.zap: c.Atk = 75; c.towerDamage = 30; break;
                    case cardName.graveyard: c.SpawnNumber = 15; break;
                    case cardName.lightning: c.Atk = 650; c.towerDamage = 260; break;
                        

                }

                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Helpfunctions.Instance.ErrorLog("#####ERR. Duplicate name:" + c.name);
            }


            foreach (var e in Buddy.Clash.Engine.Csv.CsvLogic.Projectiles.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName);
                c.type = boardObjType.PROJECTILE;
                c.TargetType = targetType.ALL;

                if (e.OnlyEnemies != null)
                {
                    if ((bool)e.OnlyEnemies)
                    {
                        if (e.OnlyOwnTroops != null && (bool)e.OnlyOwnTroops) c.affectType = affectType.ALL;
                        else c.affectType = affectType.ONLY_ENEMIES;
                    }
                }
                else if (e.OnlyOwnTroops != null && (bool)e.OnlyOwnTroops) c.affectType = affectType.ONLY_OWN;

                c.cost = -1; //TODO: dig this value
                if (e.Damage != null) c.Atk = (int)e.Damage; //-All Damage
                if (e.Speed != null) c.Speed = (int)e.Speed; //-Mob, Projectile
                if (e.Rarity != null) c.Rarity = e.Rarity;
                c.Level = 0;
                if (e.Radius != null) c.DamageRadius = (int)e.Radius; //mob,aoe,projectile=Radius
                if (e.AoeToGround != null) c.aoeGround = (bool)e.AoeToGround; //-projectile
                if (e.AoeToAir != null) c.aoeAir = (bool)e.AoeToAir; //-projectile
                if (e.SpawnCharacterCount != null) c.SpawnNumber = (int)e.SpawnCharacterCount; //-Mobs+Buildings
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = (int)e.SpawnCharacterLevelIndex;

                /*//TODO:explore real value on field for below
                 * ProjectileRange don't use, just set val to Log
                BowlerProjectile 6000 - Bowler 4500
                LogProjectileRolling 11100
                AxeManProjectile 6000 axeman 4500*/

                switch (c.stringName)
                {
                    case "AxeManProjectile": if (cardNameToCardList.ContainsKey(cardName.axeman)) { cardNameToCardList[cardName.axeman].Atk = c.Atk; cardNameToCardList[cardName.axeman].DamageRadius = c.DamageRadius; } break;
                    case "ice_wizardProjectile": if (cardNameToCardList.ContainsKey(cardName.icewizard)) { cardNameToCardList[cardName.icewizard].Atk = c.Atk; cardNameToCardList[cardName.icewizard].DamageRadius = c.DamageRadius; } break;
                    case "chr_wizardProjectile": if (cardNameToCardList.ContainsKey(cardName.wizard)) { cardNameToCardList[cardName.wizard].Atk = c.Atk; cardNameToCardList[cardName.wizard].DamageRadius = c.DamageRadius; } break;
                    case "BombSkeletonProjectile": if (cardNameToCardList.ContainsKey(cardName.bomber)) { cardNameToCardList[cardName.bomber].Atk = c.Atk; cardNameToCardList[cardName.bomber].DamageRadius = c.DamageRadius; } break;
                    case "ZapMachineProjectile": if (cardNameToCardList.ContainsKey(cardName.zapmachine)) { cardNameToCardList[cardName.zapmachine].Atk = c.Atk; cardNameToCardList[cardName.zapmachine].DamageRadius = c.DamageRadius; } break;
                    case "BowlerProjectile": if (cardNameToCardList.ContainsKey(cardName.bowler)) { cardNameToCardList[cardName.bowler].Atk = c.Atk; cardNameToCardList[cardName.bowler].DamageRadius = c.DamageRadius; } break;
                    case "MovingCannonProjectile":
                        if (cardNameToCardList.ContainsKey(cardName.movingcannon)) { cardNameToCardList[cardName.movingcannon].Atk = c.Atk; cardNameToCardList[cardName.movingcannon].DamageRadius = c.DamageRadius; }
                        if (cardNameToCardList.ContainsKey(cardName.brokencannon)) { cardNameToCardList[cardName.brokencannon].Atk = c.Atk; cardNameToCardList[cardName.brokencannon].DamageRadius = c.DamageRadius; }
                        break;
                    case "MegaKnightAppear": if (cardNameToCardList.ContainsKey(cardName.megaknight)) { cardNameToCardList[cardName.megaknight].Atk = c.Atk; cardNameToCardList[cardName.megaknight].DamageRadius = c.DamageRadius; } break;
                    case "KingProjectile": if (cardNameToCardList.ContainsKey(cardName.kingtower)) { cardNameToCardList[cardName.kingtower].Atk = c.Atk; cardNameToCardList[cardName.kingtower].DamageRadius = c.DamageRadius; } break;
                    case "TowerCannonball": if (cardNameToCardList.ContainsKey(cardName.cannon)) { cardNameToCardList[cardName.cannon].Atk = c.Atk; cardNameToCardList[cardName.cannon].DamageRadius = c.DamageRadius; } break;
                    case "MortarProjectile": if (cardNameToCardList.ContainsKey(cardName.mortar)) { cardNameToCardList[cardName.mortar].Atk = c.Atk; cardNameToCardList[cardName.mortar].DamageRadius = c.DamageRadius; } break;
                    case "BombTowerProjectile": if (cardNameToCardList.ContainsKey(cardName.bombtower)) { cardNameToCardList[cardName.bombtower].Atk = c.Atk; cardNameToCardList[cardName.bombtower].DamageRadius = c.DamageRadius; } break;
                    case "xbow_projectile": if (cardNameToCardList.ContainsKey(cardName.xbow)) { cardNameToCardList[cardName.xbow].Atk = c.Atk; cardNameToCardList[cardName.xbow].DamageRadius = c.DamageRadius; } break;
                    default:
                        if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                        else Helpfunctions.Instance.ErrorLog("#####ERR. Duplicate name:" + c.name);
                        break;
                }
            }
            
            Helpfunctions.Instance.ErrorLog("CardList:" + cardNameToCardList.Count);
        }

        public affectType stringToAffectType(string s)
        {
            affectType type;
            if (Enum.TryParse<affectType>(s, false, out type)) return type;
            else return affectType.NONE;
        }

        public targetType stringToTargetType(string s)
        {
            targetType type;
            if (Enum.TryParse<targetType>(s, false, out type)) return type;
            else return targetType.NONE;
        }

        public transportType stringToTransportType(string s)
        {
            transportType type;
            if (Enum.TryParse<transportType>(s, false, out type)) return type;
            else return transportType.NONE;
        }

        public boardObjType stringToBoardObjType(string s)
        {
            boardObjType type;
            if (Enum.TryParse<boardObjType>(s, false, out type)) return type;
            else return boardObjType.NONE;
        }


        public void collectCardInfo(BoardObj bo)
        {
            if (cardNameToCardList.ContainsKey(bo.Name))
            {
                Card c = cardNameToCardList[bo.Name];
                if (c.needUpdate)
                {
                    c.type = bo.type;
                    c.Transport = bo.Transport;
                    c.TargetType = bo.TargetType;
                    c.affectType = bo.affectOn;
                    c.cost = bo.cost;
                    c.DeployTime = bo.DeployTime;
                    c.DamageRadius = bo.DamageRadius;
                    c.MaxHP = bo.MaxHP;
                    c.Atk = bo.Atk;
                    c.Shield = bo.Shield;
                    c.Speed = bo.Speed;
                    c.HitSpeed = bo.HitSpeed;
                    c.MinRange = bo.MinRange;
                    c.MaxRange = bo.Range;
                    c.SightRange = bo.SightRange;
                    c.MultipleTargets = bo.MaxTargets;
                    c.DeathEffect = bo.DeathEffect;
                    c.Level = bo.level;
                    c.LifeTime = bo.LifeTime;
                    c.SpawnNumber = bo.SpawnNumber;
                    c.SpawnPause = bo.SpawnTime;
                    c.SpawnInterval = bo.SpawnInterval;
                    c.SpawnCharacterLevel = bo.SpawnCharacterLevel;
                    c.needUpdate = false;
                }
            }
            else
            {                
                //numDuplicates = 0;
                //numDifferences = 0;
            }
        }

        public void uploadCardInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1000000);
            
            /*
            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(Nano.Settings.DatabaseFullpath,"_carddb_upd.txt")))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch
            {
                //TODO: other way to inform about this problem (m.b. line in main bot-log)
            }*/
        }



        public Card getCardDataFromName(CardDB.cardName cardname, int lvl)
        {
            return this.cardNameToCardList.ContainsKey(cardname) ? this.cardNameToCardList[cardname] : this.unknownCard;
        }       
        

    }

}