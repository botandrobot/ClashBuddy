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
        
        public enum cardName //-заменяем пробелы, убираем . и - потом переводим в нижний регистр
        {
            unknown,
            kingtower,
            kingtowermiddle,
            princesstower,
            //-Troops
            archer,
            babydragon,
            balloon,
            balloonbomb,
            bandit,
            barbarian,
            battleram,
            bomber,
            bowler,
            darkprince,
            dartgoblin,
            electrowizard,
            elitebarbarian,
            executioner,
            firespirits,
            firespirithut,
            giant,
            giantskeleton,
            giantskeletonbomb,
            goblingang,
            goblin,
            golem,
            guards,
            hogrider,
            icegolem,
            icespirit,
            icewizard,
            infernodragon,
            knight,
            lavahound,
            lavapups,
            lumberjack,
            megaminion,
            miner,
            minionhorde,
            minion,
            minipekka,
            musketeer,
            nightwitch,
            pekka,
            prince,
            princess,
            royalgiant,
            skeletonarmy,
            skeleton,
            sparky,
            speargoblin,
            threemusketeers, //TODO: check it
            valkyrie,
            witch,
            wizard,

            //-Spels
            arrows,
            clonespell,
            fireball,
            freeze,
            goblinbarrel,
            graveyard,
            heal,
            lightning,
            mirror,
            poison,
            rage,
            rocket,
            thelog,
            tornado,
            zap,

            //-Buildings
            barbarianhut,
            bombtower,
            cannon,
            elixircollector,
            furnace,
            goblinhut,
            infernotower,
            mortar,
            tesla,
            tombstone,
            xbow
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
            public boardObjType type = boardObjType.NONE;
            public transportType Transport = transportType.NONE; //-Mob (Air, Ground)
            public targetType TargetType = targetType.NONE; //-AttacksAir, TargetOnlyBuildings
            public affectType EffectType = affectType.NONE;

            public int cost = 0; //All
            public int DeployTime = 0; //-All
            public int MaxHP = 0; //-All
            public int Atk = 0; //-All Damage
            public int Shield = 0; //-Mob
            public int Speed = 0; //-Mob
            public int HitSpeed = 0; //-All
            public int MinRange = 0; //-Mob+AreaEffect Radius
            public int MaxRange = 0; //-Mob+AreaEffect Radius
            public int SightRange = 0;
            public int MaxTargets = 0; //-All
            public int DeathEffect = 0;
            public int Rarity = 0;
            public int Level = 0;
            public int DamageRadius = 0;

            public int LifeTime = 0; //-Buildings+AreaEffect LifeDuration
            public int SpawnNumber = 0; //-Mobs+Buildings
            public int SpawnTime = 0; //-Mobs+Buildings DataTime
            public int SpawnInterval = 0; //-Mobs+Buildings
            public int SpawnCharacterLevel = 0;


            public bool needUpdate = true;
            public int numDuplicates = 0;
            public int numDifferences = 0;
            public List<cardtrigers> trigers;



            /*


            //stuff for test and seech
            public string TargettedDamageEffect3 = "";
            public string TargettedDamageEffect2 = "";
            public string TargettedDamageEffect1 = "";
            public string TargetedHitEffectSpecial = "";
            public string TargetedHitEffect = "";
            public string TakeDamageEffect = "";
            public string TID = "";
            public string StartingBuff = "";
            public string SpecialReadyEffect = "";
            public string SpawnProjectile = "";
            public string SpawnPathfindMorph = "";
            public string SpawnPathfindEffect = "";
            public string SpawnEffect = "";
            public string SpawnDeployBaseAnim = "";
            public string SpawnCharacterEffect = "";
            public string SpawnCharacter = "";
            public string SpawnAreaObject = "";
            public string ShieldLostEffect = "";
            public string ShadowCustomLow = "";
            public string ShadowCustom = "";
            public string RedTopExportName = "";
            public string RedShieldExportName = "";
            public string RedExportName = "";
            public string Rarity = "";
            public string ProjectileSpecial = "";
            public string ProjectileEffectSpecial = "";
            public string ProjectileEffect = "";
            public string Projectile = "";
            public string Name = "";
            public string MoveEffect = "";
            public string MorphEffect = "";
            public string MorphCharacter = "";
            public string LoopingFilter = "";
            public string LoadAttackEffectReady = "";
            public string LoadAttackEffect3 = "";
            public string LoadAttackEffect2 = "";
            public string LoadAttackEffect1 = "";
            public string LandingEffect = "";
            public string KamikazeEffect = "";
            public string HideEffect = "";
            public string HealthBar = "";
            public string FlameEffect3 = "";
            public string FlameEffect2 = "";
            public string FlameEffect1 = "";
            public string FileName = "";
            public string DeployBaseAnimExportName = "";
            public string DeathSpawnProjectile = "";
            public string DeathSpawnCharacter = "";
            public string DeathEffect = "";
            public string DeathAreaEffect = "";
            public string DashStartEffect = "";
            public string DashFilter = "";
            public string DashEffect = "";
            public string DamageLevelTransitionEffect23 = "";
            public string DamageLevelTransitionEffect12 = "";
            public string DamageExportName = "";
            public string DamageEffectSpecial = "";
            public string DamageEffect = "";
            public string CustomFirstProjectile = "";
            public string ContinuousEffect = "";
            public string ChargeEffect = "";
            public string BuffOnDamage = "";
            public string BlueTopExportName = "";
            public string BlueShieldExportName = "";
            public string BlueExportName = "";
            public string AttackStartEffectSpecial = "";
            public string AttackStartEffect = "";
            public string AttachedCharacter = "";
            public string AreaEffectOnMorph = "";
            public string AreaEffectOnDash = "";
            public string AreaBuff = "";
            public string AppearEffect = "";
            public string AppearAreaObject = "";
            public string Ability = "";
            public int WalkingSpeedTweakPercentage = 0;
            public int WaitMS = 0;
            public int VisualHitSpeed = 0;
            public int VariableDamageTransitionTime = 0;
            public int VariableDamageTime2 = 0;
            public int VariableDamageTime1 = 0;
            public int VariableDamage3 = 0;
            public int VariableDamage2 = 0;
            public int UpTimeMs = 0;
            public int TurretMovement = 0;
            public int TileSizeOverride = 0;
            public int TargetEffectY = 0;
            public int StopTimeAfterSpecialAttack = 0;
            public int StopTimeAfterAttack = 0;
            public int StopMovementAfterMS = 0;
            public int StartingBuffTime = 0;
            public int Speed = 0;
            public int SpecialRange = 0;
            public int SpecialMinRange = 0;
            public int SpecialLoadTime = 0;
            public int SpecialAttackInterval = 0;
            public int SpawnStartTime = 0;
            public int SpawnRadius = 0;
            public int SpawnPushbackRadius = 0;
            public int SpawnPushback = 0;
            public int SpawnPauseTime = 0;
            public int SpawnPathfindSpeed = 0;
            public int SpawnNumber = 0;
            public int SpawnLimit = 0;
            public int SpawnInterval = 0;
            public int SpawnCharacterLevelIndex = 0;
            public int SpawnAreaObjectLevelIndex = 0;
            public int SpawnAngleShift = 0;
            public int SightRange = 0;
            public int SightClipSide = 0;
            public int SightClip = 0;
            public int ShieldHitpoints = 0;
            public int ShieldDiePushback = 0;
            public int ShadowY = 0;
            public int ShadowX = 0;
            public int ShadowSkew = 0;
            public int ShadowScaleY = 0;
            public int ShadowScaleX = 0;
            public int Scale = 0;
            public int RotateAngleSpeed = 0;
            public int Range = 0;
            public int Pushback = 0;
            public int ProjectileYOffset = 0;
            public int ProjectileStartZ = 0;
            public int ProjectileStartRadius = 0;
            public int NoDeploySizeW = 0;
            public int NoDeploySizeH = 0;
            public int MultipleTargets = 0;
            public int MultipleProjectiles = 0;
            public int MorphTime = 0;
            public int MinimumRange = 0;
            public int Mass = 0;
            public int ManaGenerateTimeMs = 0;
            public int ManaGenerateLimit = 0;
            public int ManaCollectAmount = 0;
            public int LoadTime = 0;
            public int LifeTime = 0;
            public int KamikazeTime = 0;
            public int JumpSpeed = 0;
            public int JumpHeight = 0;
            public int Hitpoints = 0;
            public int HitSpeed = 0;
            public int HideTimeMs = 0;
            public int HealthBarOffsetY = 0;
            public int GrowTime = 0;
            public int GrowSize = 0;
            public int FlyingHeight = 0;
            public int DeployTimerDelay = 0;
            public int DeployTime = 0;
            public int DeployDelay = 0;
            public int DeathSpawnRadius = 0;
            public int DeathSpawnMinRadius = 0;
            public int DeathSpawnDeployTime = 0;
            public int DeathSpawnCount = 0;
            public int DeathPushBack = 0;
            public int DeathDamageRadius = 0;
            public int DeathDamage = 0;
            public int DashRadius = 0;
            public int DashPushBack = 0;
            public int DashMinRange = 0;
            public int DashMaxRange = 0;
            public int DashLandingTime = 0;
            public int DashImmuneToDamageTime = 0;
            public int DashDamage = 0;
            public int DashCooldown = 0;
            public int DashConstantTime = 0;
            public int DamageSpecial = 0;
            public int Damage = 0;
            public int CrownTowerDamagePercent = 0;
            public int CollisionRadius = 0;
            public int ChargeSpeedMultiplier = 0;
            public int ChargeRange = 0;
            public int BurstDelay = 0;
            public int Burst = 0;
            public int BuffOnDamageTime = 0;
            public int AttackShakeTime = 0;
            public int AttackPushBack = 0;
            public int AttackDashTime = 0;
            public int AttachedCharacterHeight = 0;
            public int AreaDamageRadius = 0;
            public int AreaBuffTime = 0;
            public int AreaBuffRadius = 0;
            public int AppearPushbackRadius = 0;
            public int AppearPushback = 0;
            public int ActivationTime = 0;
            public bool VariableDamageLifeTime = false;
            public bool UseAnimator = false;
            public bool TargetOnlyBuildings = false;
            public bool SpecialAttackWhenHidden = false;
            public bool SpawnConstPriority = false;
            public bool ShowHealthNumber = false;
            public bool SelfAsAoeCenter = false;
            public bool RetargetAfterAttack = false;
            public bool MorphKeepTarget = false;
            public bool LoopMoveEffect = false;
            public bool LoadFirstHit = false;
            public bool Kamikaze = false;
            public bool JumpEnabled = false;
            public bool IsSummonerTower = false;
            public bool IgnorePushback = false;
            public bool HidesWhenNotAttacking = false;
            public bool HideBeforeFirstHit = false;
            public bool HealOnMorph = false;
            public bool HasRotationOnTimeline = false;
            public bool FlyFromGround = false;
            public bool FlyDirectPaths = false;
            public bool DontStopMoveAnim = false;
            public bool DeathSpawnPushback = false;
            public bool DeathInheritIgnoreList = false;
            public bool CrowdEffects = false;
            public bool BurstKeepTarget = false;
            public bool BuildingTarget = false;
            public bool AttacksGround = false;
            public bool AttacksAir = false;
            public bool AllTargetsHit = false;




            */







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
            
            //foreach (string actName in Enum.GetNames(typeof(actionEnum)))
           /* foreach (var e in Buddy.Clash.Engine.Csv.CsvLogic.Characters.Entries)
            {
                Logger.Information("{TID}: {Name} has {ShieldHitpoints}", e.TID, e.Name, e.ShieldHitpoints);
            }
            help.logg("-----------------Initialize");
            CardDB cdb = CardDB.Instance;*/

            //-TODO: заглушка - just for test - просто имена - потом создать реальные значения
            /*
            foreach (var s in Enum.GetNames(typeof(cardName)))
            {
                cardName cName = this.cardNamestringToEnum(s);
                Card c = new Card() { Name = cName };
                cardNameToCardList.Add(cName, c);
            }
                */

            string[] lines = new string[0] { };
            string fileName = Path.Combine(Nano.Settings.DatabaseFullpath, "data", "_carddb.txt");
            if (File.Exists(fileName))
            {
                lines = System.IO.File.ReadAllLines(fileName);
                Helpfunctions.Instance.ErrorLog("read carddb.txt " + lines.Length + " lines");
            }
            else
            {
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                Helpfunctions.Instance.ErrorLog($"cant find _carddb.txt at {fileName}");
                Helpfunctions.Instance.ErrorLog("or read error");
                return;
            }
            this.cardNameToCardList.Clear();
            this.unknownCard = new Card { name = cardName.unknown, cost = 1000 };
            /*
            foreach (string s in lines)
            {
                Card c = new Card();
                string[] tmp = s.Split(' ');
                foreach (string ss in tmp)
                {
                    string[] param = ss.Split(':');
                    switch (param[0])
                    {
                        case "Name": c.name = cardNamestringToEnum(param[1]); continue;
                        case "type": c.type = stringToBoardObjType(param[1]); continue;
                        case "Transport": c.Transport = stringToTransportType(param[1]); continue;
                        case "TargetType": c.TargetType = stringToTargetType(param[1]); continue;
                        case "EffectType": c.EffectType = stringToAffectType(param[1]); continue;
                        case "cost": c.cost = Convert.ToInt32(param[1]); continue;
                        case "deployTime": c.DeployTime = Convert.ToInt32(param[1]); continue;
                        case "MaxHP": c.MaxHP = Convert.ToInt32(param[1]); continue;
                        case "Atk": c.Atk = Convert.ToInt32(param[1]); continue;
                        case "Shield": c.Shield = Convert.ToInt32(param[1]); continue;
                        case "Speed": c.Speed = Convert.ToInt32(param[1]); continue;
                        case "DamageRadius": c.DamageRadius = Convert.ToInt32(param[1]); continue;
                        case "HitSpeed": c.HitSpeed = Convert.ToInt32(param[1]); continue;
                        case "MinRange": c.MinRange = Convert.ToInt32(param[1]); continue;
                        case "MaxRange": c.MaxRange = Convert.ToInt32(param[1]); continue;
                        case "SightRange": c.SightRange = Convert.ToInt32(param[1]); continue;
                        case "MaxTargets": c.MaxTargets = Convert.ToInt32(param[1]); continue;
                        case "DeathEffect": c.DeathEffect = Convert.ToInt32(param[1]); continue;
                        case "Rarity": c.Rarity = Convert.ToInt32(param[1]); continue;
                        case "Level": c.Level = Convert.ToInt32(param[1]); continue;
                        case "LifeTime": c.LifeTime = Convert.ToInt32(param[1]); continue;
                        case "SpawnNumber": c.SpawnNumber = Convert.ToInt32(param[1]); continue;
                        case "SpawnTime": c.SpawnTime = Convert.ToInt32(param[1]); continue;
                        case "SpawnInterval": c.SpawnInterval = Convert.ToInt32(param[1]); continue;
                        case "SpawnCharacterLevel": c.SpawnCharacterLevel = Convert.ToInt32(param[1]); continue;
                        case "needUpdate": c.needUpdate = param[1] == "False" ? false : true; continue;
                        case "numDuplicates": c.numDuplicates = Convert.ToInt32(param[1]); continue;
                        case "numDifferences": c.numDifferences = Convert.ToInt32(param[1]); continue;
                    }
                }
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Helpfunctions.Instance.ErrorLog("#####ERR. Duplicate name:" + c.name);
            }*/
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
                    c.EffectType = bo.EffectType;
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
                    c.MaxTargets = bo.MaxTargets;
                    c.DeathEffect = bo.DeathEffect;
                    c.Level = bo.level;
                    c.LifeTime = bo.LifeTime;
                    c.SpawnNumber = bo.SpawnNumber;
                    c.SpawnTime = bo.SpawnTime;
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
            System.Text.StringBuilder sb = new System.Text.StringBuilder(20000);
            foreach (var kvp in cardNameToCardList)
            {
                Card c = kvp.Value;
                sb.Append("Name:").Append(c.name).Append(" ");
                sb.Append("type:").Append(c.type).Append(" ");
                sb.Append("Transport:").Append(c.Transport).Append(" ");
                sb.Append("TargetType:").Append(c.TargetType).Append(" ");
                sb.Append("EffectType:").Append(c.EffectType).Append(" ");
                sb.Append("cost:").Append(c.cost).Append(" ");
                sb.Append("deployTime:").Append(c.DeployTime).Append(" ");
                sb.Append("DamageRadius:").Append(c.DamageRadius).Append(" ");
                sb.Append("MaxHP:").Append(c.MaxHP).Append(" ");
                sb.Append("Atk:").Append(c.Atk).Append(" ");
                sb.Append("Shield:").Append(c.Shield).Append(" ");
                sb.Append("Speed:").Append(c.Speed).Append(" ");
                sb.Append("HitSpeed:").Append(c.HitSpeed).Append(" ");
                sb.Append("MinRange:").Append(c.MinRange).Append(" ");
                sb.Append("MaxRange:").Append(c.MaxRange).Append(" ");
                sb.Append("SightRange:").Append(c.SightRange).Append(" ");
                sb.Append("MaxTargets:").Append(c.MaxTargets).Append(" ");
                sb.Append("DeathEffect:").Append(c.DeathEffect).Append(" ");
                sb.Append("Rarity:").Append(c.Rarity).Append(" ");
                sb.Append("Level:").Append(c.Level).Append(" ");
                sb.Append("LifeTime:").Append(c.LifeTime).Append(" ");
                sb.Append("SpawnNumber:").Append(c.SpawnNumber).Append(" ");
                sb.Append("SpawnTime:").Append(c.SpawnTime).Append(" ");
                sb.Append("SpawnInterval:").Append(c.SpawnInterval).Append(" ");
                sb.Append("SpawnCharacterLevel:").Append(c.SpawnCharacterLevel).Append(" ");
                sb.Append("needUpdate:").Append(c.needUpdate).Append(" ");
                sb.Append("numDuplicates:").Append(c.numDuplicates).Append(" ");
                sb.Append("numDifferences:").Append(c.numDifferences);
                sb.Append("\r\n");
            }

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
            }
        }



        public Card getCardDataFromName(CardDB.cardName cardname, int lvl)
        {
            return this.cardNameToCardList.ContainsKey(cardname) ? this.cardNameToCardList[cardname] : this.unknownCard;
        }

        /*
        public SimTemplate getSimCard(cardIDEnum id)
        {
            switch (id)
            {
                case cardIDEnum.None:
                    return new Sim_None();
                case cardIDEnum.NAX3_02_TB:
                    return new Sim_NAX3_02_TB();
                case cardIDEnum.NAX12_02H_2_TB:
                    return new Sim_NAX12_02H_2_TB();
                case cardIDEnum.NAX11_02H_2_TB:
                    return new Sim_NAX11_02H_2_TB();
                case cardIDEnum.BRMA17_5_TB:
                    return new Sim_BRMA17_5_TB();
                case cardIDEnum.BRMA14_10H_TB:
                    return new Sim_BRMA14_10H_TB();
                case cardIDEnum.BRMA13_4_2_TB:
                    return new Sim_BRMA13_4_2_TB();
                case cardIDEnum.UNG_999t7:
                    return new Sim_UNG_999t7();
                case cardIDEnum.UNG_999t8:
                    return new Sim_UNG_999t8();
            }

            return new SimTemplate();
        }*/
        
        

    }

}