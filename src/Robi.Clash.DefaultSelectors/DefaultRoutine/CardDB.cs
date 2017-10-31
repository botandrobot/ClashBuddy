using Robi.Common;
using Serilog;

namespace Robi.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;


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
        private static readonly ILogger Logger = LogProvider.CreateLogger<CardDB>();

        public enum cardParamInt
        {
            cost,
            DeployTime,
            DeployDelay,
            MaxHP,
            Atk,
            Shield,
            SpawnDamage,
            Speed,
            HitSpeed,
            MinRange,
            MaxRange,
            SightRange,
            SightClip,
            MultipleTargets,
            MultipleProjectiles,
            Level,
            DamageRadius,
            CollisionRadius,
            towerDamage,
            LifeTime,
            SummonNumber,
            SpawnNumber,
            SpawnPause,
            SpawnInterval,
            SpawnCharacterLevel
        }

        public enum cardName //-replace " ", ".", lower case
        {
            unknown,
            //Troops
            angrybarbarian, //elitebarbarian
            angrybarbarians, // -
            archer,
            assassin, //bandit
            axeman, //executioner
            babydragon,
            balloon,
            barbarian,
            barbarians,
            bat,
            battleram,
            blowdartgoblin, //dartgoblin
            bomber,
            bowler,
            brokencannon, //CannonCart
            darkprince,
            darkwitch, //nightwitch
            dartbarrell, //FlyingMachine
            electrowizard,
            firespirits,
            giant,
            giantskeleton,
            goblin,
            goblins,
            goblingang,
            golem,
            golemite,
            hogrider,
            icegolemite, //icegolem ?
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
            minionhorde,
            minions,
            minipekka,
            movingcannon, //CannonCart
            musketeer,
            threemusketeers,
            pekka,
            prince,
            princess,
            ragebarbarian, //Lumberjack
            royalgiant,
            skeleton,
            skeletonarmy,
            skeletonballoon,
            skeletons,
            skeletonwarrior, //Guards
            speargoblin,
            speargoblins,
            towerprincess,
            valkyrie,
            witch,
            wizard,
            zapmachine, //sparky

            //-Buildings
            balloonbomb,
            barbarianhut,
            bombtower,
            cannon,
            elixircollector,
            firespirithut, //Furnace
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
            clone, //clonespell
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
            arrows,
            fireball,
            goblinbarrel,
            rocket,
            archerarrow,
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
            firespiritsprojectile,
            ice_wizardprojectile,
            icespiritsprojectile,
            kingprojectile,
            lavahoundprojectile,
            lavapupprojectile,
            log, //logprojectile,
            logprojectilerolling,
            megaknightappear,
            megaminionspit,
            minionspit,
            mortarprojectile,
            movingcannonprojectile,
            musketeerprojectile,
            princessprojectile,
            princessprojectiledeco,
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
            mirror, //TODO:find real params
            //thelog, TODO:check real card

            //troops
            //dartgoblin, //blowdartgoblin
            //elitebarbarian, //angrybarbarian
            //executioner, //axeman
            guards, //skeletonwarrior
            icegolem, //icegolemite
            // icespirit, //icespirits
            lumberjack, //ragebarbarian
            //nightwitch, //darkwitch
            sparky, //zapmachine
            //-Spels
            //clonespell, //clone
            //-Buildings
            //furnace, //firespirithut
        }

        public cardName cardNamestringToEnum(string s, string sender)
        {
            CardDB.cardName NameEnum;
            if (Enum.TryParse<cardName>(s.ToLower().Replace(" ", ""), false, out NameEnum)) return NameEnum; //TODO: improve
            else
            {
                Logger.Debug("!!!NEW NAME: {s} sender:{sender}", s, sender);
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
            public int SpawnDamage = 0; //-Mob
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
            public int Level = 1;
            public int DamageRadius = 0;
            public bool aoeGround = false; //-projectile
            public bool aoeAir = false; //-projectile
            public int CollisionRadius = 0; //-Mobs+Buildings
            public int towerDamage = 0; //aoe

            public int LifeTime = 0; //-Buildings; AreaEffect=LifeDuration
            public int SummonNumber = 0;
            public int SpawnNumber = 0; //-Mobs+Buildings
            public int SpawnPause = 0; //-Mobs+Buildings
            public int SpawnInterval = 0; //-Mobs+Buildings
            public string SpawnCharacter = ""; //-Mobs+Buildings
            public int SpawnCharacterLevel = 0;

            //Internal use
            public bool needUpdate = true;



            public Card()
            {

            }

            public Card(Card c)
            {
                this.name = c.name;
                this.stringName = c.stringName;
                this.type = c.type;
                this.Transport = c.Transport;
                this.TargetType = c.TargetType;
                this.affectType = c.affectType;

                this.cost = c.cost;
                this.DeployTime = c.DeployTime;
                this.DeployDelay = c.DeployDelay;
                this.MaxHP = c.MaxHP;
                this.Atk = c.Atk;
                this.Shield = c.Shield;
                this.SpawnDamage = c.SpawnDamage;
                this.Speed = c.Speed;
                this.HitSpeed = c.HitSpeed;
                this.MinRange = c.MinRange;
                this.MaxRange = c.MaxRange;
                this.SightRange = c.SightRange;
                this.SightClip = c.SightClip;
                this.MultipleTargets = c.MultipleTargets;
                this.MultipleProjectiles = c.MultipleProjectiles;
                this.DeathEffect = c.DeathEffect;
                this.Rarity = c.Rarity;
                this.Level = c.Level;
                this.DamageRadius = c.DamageRadius;
                this.aoeGround = c.aoeGround;
                this.aoeAir = c.aoeAir;
                this.CollisionRadius = c.CollisionRadius;
                this.towerDamage = c.towerDamage;

                this.LifeTime = c.LifeTime;
                this.SummonNumber = c.SummonNumber;
                this.SpawnNumber = c.SpawnNumber;
                this.SpawnPause = c.SpawnPause;
                this.SpawnInterval = c.SpawnInterval;
                this.SpawnCharacter = c.SpawnCharacter;
                this.SpawnCharacterLevel = c.SpawnCharacterLevel;
            }


            public int getParamByNameInt(cardParamInt param)
            {
                switch (param)
                {
                    case cardParamInt.cost: return this.cost;
                    case cardParamInt.DeployTime: return this.DeployTime;
                    case cardParamInt.DeployDelay: return this.DeployDelay;
                    case cardParamInt.MaxHP: return this.MaxHP;
                    case cardParamInt.Atk: return this.Atk;
                    case cardParamInt.Shield: return this.Shield;
                    case cardParamInt.SpawnDamage: return this.SpawnDamage;
                    case cardParamInt.Speed: return this.Speed;
                    case cardParamInt.HitSpeed: return this.HitSpeed;
                    case cardParamInt.MinRange: return this.MinRange;
                    case cardParamInt.MaxRange: return this.MaxRange;
                    case cardParamInt.SightRange: return this.SightRange;
                    case cardParamInt.SightClip: return this.SightClip;
                    case cardParamInt.MultipleTargets: return this.MultipleTargets;
                    case cardParamInt.MultipleProjectiles: return this.MultipleProjectiles;
                    case cardParamInt.Level: return this.Level;
                    case cardParamInt.DamageRadius: return this.DamageRadius;
                    case cardParamInt.CollisionRadius: return this.CollisionRadius;
                    case cardParamInt.towerDamage: return this.towerDamage;
                    case cardParamInt.LifeTime: return this.LifeTime;
                    case cardParamInt.SummonNumber: return this.SummonNumber;
                    case cardParamInt.SpawnNumber: return this.SpawnNumber;
                    case cardParamInt.SpawnPause: return this.SpawnPause;
                    case cardParamInt.SpawnInterval: return this.SpawnInterval;
                    case cardParamInt.SpawnCharacterLevel: return this.SpawnCharacterLevel;
                    default: return int.MinValue;
                }
            }
        }

        Dictionary<int, Card> forTestBase = new Dictionary<int, Card>();
        Dictionary<cardName, Card> cardNameToCardList = new Dictionary<cardName, Card>();
        Dictionary<cardName, List<Card>> cardsAdjustmentDB = new Dictionary<cardName, List<Card>>();
        Dictionary<cardParamInt, Dictionary<int, int>> cardsAdjustmentContainerInt = new Dictionary<cardParamInt, Dictionary<int, int>>();
        int updCardsMeasure = 20;
        List<string> allCardIDS = new List<string>();
        public Card unknownCard = new Card();
        public bool installedWrong = false;

        public Card teacherminion;
        public Card illidanminion;
        public Card lepergnome;
        public Card burlyrockjaw;
        private static CardDB instance;

        public static CardDB Instance => instance;

        public static void Initialize()
        {
            if (instance == null)
            {
                instance = new CardDB();
            }
        }

        private CardDB()
        {
            cardNameToCardList.Clear();
            cardsAdjustmentDB.Clear();
            initBasicDBfromString();
            initCardsAdjustmentContainerInt();

            foreach (var pair in cardNameToCardList) cardsAdjustmentDB.Add(pair.Key, new List<Card>() { pair.Value });
            Logger.Debug("CardList count: {0}", cardNameToCardList.Count);
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

        public void uploadCardInfo()
        {
            foreach (var kvp in cardNameToCardList)
            {
                try
                {
                    using (StreamWriter sw = File.AppendText("_carddb_upd.txt"))
                    {
                        sw.WriteLine(cardToString(kvp.Value));
                    }
                }
                catch { }
            }
        }

        public string cardToString(Card c)
        {
            StringBuilder sb = new StringBuilder(2000);
            sb.Append("name:").Append(c.name).Append(" ");
            sb.Append("stringName:").Append(c.stringName).Append(" ");
            sb.Append("type:").Append(c.type).Append(" ");
            sb.Append("Transport:").Append(c.Transport).Append(" ");
            sb.Append("TargetType:").Append(c.TargetType).Append(" ");
            sb.Append("affectType:").Append(c.affectType).Append(" ");
            sb.Append("cost:").Append(c.cost).Append(" ");
            sb.Append("DeployTime:").Append(c.DeployTime).Append(" ");
            sb.Append("DeployDelay:").Append(c.DeployDelay).Append(" ");
            sb.Append("MaxHP:").Append(c.MaxHP).Append(" ");
            sb.Append("Atk:").Append(c.Atk).Append(" ");
            sb.Append("Shield:").Append(c.Shield).Append(" ");
            sb.Append("SpawnDamage:").Append(c.SpawnDamage).Append(" ");
            sb.Append("Speed:").Append(c.Speed).Append(" ");
            sb.Append("HitSpeed:").Append(c.HitSpeed).Append(" ");
            sb.Append("MinRange:").Append(c.MinRange).Append(" ");
            sb.Append("MaxRange:").Append(c.MaxRange).Append(" ");
            sb.Append("SightRange:").Append(c.SightRange).Append(" ");
            sb.Append("SightClip:").Append(c.SightClip).Append(" ");
            sb.Append("MultipleTargets:").Append(c.MultipleTargets).Append(" ");
            sb.Append("MultipleProjectiles:").Append(c.MultipleProjectiles).Append(" ");
            sb.Append("DeathEffect:").Append(c.DeathEffect).Append(" ");
            sb.Append("Rarity:").Append(c.Rarity).Append(" ");
            sb.Append("Level:").Append(c.Level).Append(" ");
            sb.Append("DamageRadius:").Append(c.DamageRadius).Append(" ");
            sb.Append("aoeGround:").Append(c.aoeGround).Append(" ");
            sb.Append("aoeAir:").Append(c.aoeAir).Append(" ");
            sb.Append("CollisionRadius:").Append(c.CollisionRadius).Append(" ");
            sb.Append("towerDamage:").Append(c.towerDamage).Append(" ");
            sb.Append("LifeTime:").Append(c.LifeTime).Append(" ");
            sb.Append("SummonNumber:").Append(c.SummonNumber).Append(" ");
            sb.Append("SpawnNumber:").Append(c.SpawnNumber).Append(" ");
            sb.Append("SpawnPause:").Append(c.SpawnPause).Append(" ");
            sb.Append("SpawnInterval:").Append(c.SpawnInterval).Append(" ");
            sb.Append("SpawnCharacter:").Append(c.SpawnCharacter).Append(" ");
            sb.Append("SpawnCharacterLevel:").Append(c.SpawnCharacterLevel).Append(" ");
            return sb.ToString();
        }


        public Card getCardDataFromName(CardDB.cardName cardname, int lvl)
        {
            if (this.cardNameToCardList.ContainsKey(cardname)) return this.cardNameToCardList[cardname];
            else
            {
                Logger.Debug("!!!NEW CardName: {cardname}", cardname);
                return this.unknownCard;
            }
        }

        private void initBasicDBfromCsv()
        {
            Card c = new Card();
            foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.Characters.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName, "7");
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
                if (e.Damage != null) c.Atk = (int)e.Damage; //-all damage
                if (e.ShieldHitpoints != null) c.Shield = (int)e.ShieldHitpoints; //-mob
                //TODO: if need - SpawnDamage
                if (e.Speed != null) c.Speed = (int)e.Speed; //-mob, projectile
                if (e.HitSpeed != null) c.HitSpeed = (int)e.HitSpeed; //-mob,aoe,building
                if (e.MinimumRange != null) c.MinRange = (int)e.MinimumRange; //-only mortar
                if (e.Range != null) c.MaxRange = (int)e.Range; //-mob,building
                if (e.SightRange != null) c.SightRange = (int)e.SightRange; //-mob,building
                if (e.SightClip != null) c.SightClip = (int)e.SightClip; //-mob,building
                if (e.MultipleTargets != null) c.MultipleTargets = (int)e.MultipleTargets; //-only ElectroWizard
                if (e.MultipleProjectiles != null) c.MultipleProjectiles = (int)e.MultipleProjectiles; //- only Princess
                if (e.Rarity != null) c.Rarity = e.Rarity;
                c.Level = 1;
                if (e.AreaDamageRadius != null) c.DamageRadius = (int)e.AreaDamageRadius; //mob,aoe,projectile=Radius
                                                                                          //c.aoeGround = false; //-projectile
                                                                                          //c.aoeAir = false; //-projectile
                if (e.CollisionRadius != null) c.CollisionRadius = (int)e.CollisionRadius; //-Mobs+Buildings          
                if (e.LifeTime != null) c.LifeTime = (int)e.LifeTime; //-Buildings; AreaEffect=LifeDuration
                if (e.SpawnPauseTime != null) c.SpawnPause = (int)e.SpawnPauseTime; //-Mobs+Buildings DataTime
                //TODO:find somewhere SummonNumber
                if (e.SpawnNumber != null) c.SpawnNumber = (int)e.SpawnNumber; //-Mobs+Buildings
                if (e.SpawnInterval != null) c.SpawnInterval = (int)e.SpawnInterval; //-Mobs+Buildings > 0 if SpawnNumber > 1
                //TODO: if need - SpawnCharacter
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = 1 + (int)e.SpawnCharacterLevelIndex;

                /*TODO:explore real value on field for below
                ProjectileRange don't use, just set val to Log
                BowlerProjectile 6000 - Bowler 4500
                LogProjectileRolling 11100
                AxeManProjectile 6000 axeman 4500*/

                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }


            foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.Buildings.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName, "8");
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
                //TODO: if need - SpawnDamage
                if (e.Speed != null) c.Speed = (int)e.Speed; //-Mob, Projectile
                if (e.HitSpeed != null) c.HitSpeed = (int)e.HitSpeed; //-Mob,aoe,building
                if (e.MinimumRange != null) c.MinRange = (int)e.MinimumRange; //-only Mortar
                if (e.Range != null) c.MaxRange = (int)e.Range; //-Mob,building
                if (e.SightRange != null) c.SightRange = (int)e.SightRange; //-Mob,building
                if (e.SightClip != null) c.SightClip = (int)e.SightClip; //-Mob,building
                if (e.MultipleTargets != null) c.MultipleTargets = (int)e.MultipleTargets; //-only ElectroWizard
                if (e.MultipleProjectiles != null) c.MultipleProjectiles = (int)e.MultipleProjectiles; //- only Princess
                if (e.Rarity != null) c.Rarity = e.Rarity;
                c.Level = 1;
                if (e.AreaDamageRadius != null) c.DamageRadius = (int)e.AreaDamageRadius; //mob,aoe,projectile=Radius TODO: match troop and projectile Заполняем сначала минионов, здания, аое - потом проджектил и оттуда кейсом тупо сопоставляем с минионом
                                                                                          //c.aoeGround = false; //-projectile
                                                                                          //c.aoeAir = false; //-projectile
                if (e.CollisionRadius != null) c.CollisionRadius = (int)e.CollisionRadius; //-Mobs+Buildings          
                if (e.LifeTime != null) c.LifeTime = (int)e.LifeTime; //-Buildings; AreaEffect=LifeDuration
                if (e.SpawnPauseTime != null) c.SpawnPause = (int)e.SpawnPauseTime; //-Mobs+Buildings DataTime
                if (e.SpawnNumber != null) c.SpawnNumber = (int)e.SpawnNumber; //-Mobs+Buildings
                if (e.SpawnInterval != null) c.SpawnInterval = (int)e.SpawnInterval; //-Mobs+Buildings > 0 if SpawnNumber > 1
                //TODO: if need - SpawnCharacter
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = 1 + (int)e.SpawnCharacterLevelIndex;

                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }

            foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.AreaEffectObjects.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                c.name = cardNamestringToEnum(c.stringName, "9");
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
                c.Level = 1;
                if (e.Radius != null) c.DamageRadius = (int)e.Radius; //mob,aoe,projectile=Radius
                                                                      //c.aoeGround = false; //-projectile
                                                                      //c.aoeAir = false; //-projectile
                                                                      //c.CollisionRadius = (        
                if (e.LifeDuration != null) c.LifeTime = (int)e.LifeDuration; //-Buildings; AreaEffect=LifeDuration
                                                                              //c.SpawnPause = 
                                                                              //c.SpawnNumber =  //-Mobs+Buildings
                if (e.SpawnInterval != null) c.SpawnInterval = (int)e.SpawnInterval; //-Mobs+Buildings > 0 if SpawnNumber > 1
                //TODO: if need - SpawnCharacter
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = 1 + (int)e.SpawnCharacterLevelIndex;

                switch (c.name)
                {
                    case cardName.poison: c.Atk = 57; c.towerDamage = 23; break;
                    case cardName.tornado: c.Atk = 44; break;
                    case cardName.heal: c.Atk = -100; break;
                    case cardName.zap: c.Atk = 75; c.towerDamage = 30; break;
                    case cardName.graveyard: c.SummonNumber = 15; break;
                    case cardName.lightning: c.Atk = 650; c.towerDamage = 260; break;


                }

                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }


            foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.Projectiles.Entries)
            {
                c = new Card();
                c.stringName = e.Name;
                switch (c.stringName)
                {
                    case "FireballSpell": c.stringName = "Fireball"; break;
                    case "ArrowsSpell": c.stringName = "Arrows"; break;
                    case "RocketSpell": c.stringName = "Rocket"; break;
                    case "GoblinBarrelSpell": c.stringName = "GoblinBarrel"; break;
                    case "LighningSpell": c.stringName = "Lightning"; break;
                }
                c.name = cardNamestringToEnum(c.stringName, "10");
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
                c.Level = 1;
                if (e.Radius != null) c.DamageRadius = (int)e.Radius; //mob,aoe,projectile=Radius
                if (e.AoeToGround != null) c.aoeGround = (bool)e.AoeToGround; //-projectile
                if (e.AoeToAir != null) c.aoeAir = (bool)e.AoeToAir; //-projectile
                if (e.SpawnCharacterCount != null) c.SpawnNumber = (int)e.SpawnCharacterCount; //-Mobs+Buildings
                if (e.SpawnCharacterLevelIndex != null) c.SpawnCharacterLevel = 1 + (int)e.SpawnCharacterLevelIndex;

                /*TODO:explore real value on field for below
                ProjectileRange don't use, just set val to Log
                BowlerProjectile 6000 - Bowler 4500
                LogProjectileRolling 11100
                AxeManProjectile 6000 axeman 4500*/

                switch (c.stringName)
                {
                    case "DartBarrellProjectile": if (cardNameToCardList.ContainsKey(cardName.dartbarrell)) { cardNameToCardList[cardName.dartbarrell].Atk = c.Atk; cardNameToCardList[cardName.dartbarrell].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.dartbarrell].aoeGround = c.aoeGround; cardNameToCardList[cardName.dartbarrell].aoeAir = c.aoeAir; } break;
                    case "BatProjectile": if (cardNameToCardList.ContainsKey(cardName.bat)) { cardNameToCardList[cardName.bat].Atk = c.Atk; cardNameToCardList[cardName.bat].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.bat].aoeGround = c.aoeGround; cardNameToCardList[cardName.bat].aoeAir = c.aoeAir; } break;
                    case "MegaMinionSpit": if (cardNameToCardList.ContainsKey(cardName.megaminion)) { cardNameToCardList[cardName.megaminion].Atk = c.Atk; cardNameToCardList[cardName.megaminion].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.megaminion].aoeGround = c.aoeGround; cardNameToCardList[cardName.megaminion].aoeAir = c.aoeAir; } break;
                    case "MinionSpit": if (cardNameToCardList.ContainsKey(cardName.minion)) { cardNameToCardList[cardName.minion].Atk = c.Atk; cardNameToCardList[cardName.minion].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.minion].aoeGround = c.aoeGround; cardNameToCardList[cardName.minion].aoeAir = c.aoeAir; } break;
                    case "BabyDragonProjectile": if (cardNameToCardList.ContainsKey(cardName.babydragon)) { cardNameToCardList[cardName.babydragon].Atk = c.Atk; cardNameToCardList[cardName.babydragon].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.babydragon].aoeGround = c.aoeGround; cardNameToCardList[cardName.babydragon].aoeAir = c.aoeAir; } break;
                    case "LavaPupProjectile": if (cardNameToCardList.ContainsKey(cardName.lavapups)) { cardNameToCardList[cardName.lavapups].Atk = c.Atk; cardNameToCardList[cardName.lavapups].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.lavapups].aoeGround = c.aoeGround; cardNameToCardList[cardName.lavapups].aoeAir = c.aoeAir; } break;
                    case "LavaHoundProjectile": if (cardNameToCardList.ContainsKey(cardName.lavahound)) { cardNameToCardList[cardName.lavahound].Atk = c.Atk; cardNameToCardList[cardName.lavahound].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.lavahound].aoeGround = c.aoeGround; cardNameToCardList[cardName.lavahound].aoeAir = c.aoeAir; } break;
                    case "RoyalGiantProjectile": if (cardNameToCardList.ContainsKey(cardName.royalgiant)) { cardNameToCardList[cardName.royalgiant].Atk = c.Atk; cardNameToCardList[cardName.royalgiant].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.royalgiant].aoeGround = c.aoeGround; cardNameToCardList[cardName.royalgiant].aoeAir = c.aoeAir; } break;
                    case "ArcherArrow": if (cardNameToCardList.ContainsKey(cardName.archer)) { cardNameToCardList[cardName.archer].Atk = c.Atk; cardNameToCardList[cardName.archer].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.archer].aoeGround = c.aoeGround; cardNameToCardList[cardName.archer].aoeAir = c.aoeAir; } break;
                    case "MusketeerProjectile": if (cardNameToCardList.ContainsKey(cardName.musketeer)) { cardNameToCardList[cardName.musketeer].Atk = c.Atk; cardNameToCardList[cardName.musketeer].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.musketeer].aoeGround = c.aoeGround; cardNameToCardList[cardName.musketeer].aoeAir = c.aoeAir; } break;
                    case "SpearGoblinProjectile": if (cardNameToCardList.ContainsKey(cardName.speargoblin)) { cardNameToCardList[cardName.speargoblin].Atk = c.Atk; cardNameToCardList[cardName.speargoblin].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.speargoblin].aoeGround = c.aoeGround; cardNameToCardList[cardName.speargoblin].aoeAir = c.aoeAir; } break;
                    case "BlowdartGoblinProjectile": if (cardNameToCardList.ContainsKey(cardName.blowdartgoblin)) { cardNameToCardList[cardName.blowdartgoblin].Atk = c.Atk; cardNameToCardList[cardName.blowdartgoblin].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.blowdartgoblin].aoeGround = c.aoeGround; cardNameToCardList[cardName.blowdartgoblin].aoeAir = c.aoeAir; } break;
                    case "WitchProjectile": if (cardNameToCardList.ContainsKey(cardName.witch)) { cardNameToCardList[cardName.witch].Atk = c.Atk; cardNameToCardList[cardName.witch].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.witch].aoeGround = c.aoeGround; cardNameToCardList[cardName.witch].aoeAir = c.aoeAir; } break;
                    case "TowerPrincessProjectile": if (cardNameToCardList.ContainsKey(cardName.towerprincess)) { cardNameToCardList[cardName.towerprincess].Atk = c.Atk; cardNameToCardList[cardName.towerprincess].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.towerprincess].aoeGround = c.aoeGround; cardNameToCardList[cardName.towerprincess].aoeAir = c.aoeAir; } break;
                    case "IceSpiritsProjectile": if (cardNameToCardList.ContainsKey(cardName.icespirits)) { cardNameToCardList[cardName.icespirits].Atk = c.Atk; cardNameToCardList[cardName.icespirits].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.icespirits].aoeGround = c.aoeGround; cardNameToCardList[cardName.icespirits].aoeAir = c.aoeAir; } break;
                    case "FireSpiritsProjectile": if (cardNameToCardList.ContainsKey(cardName.firespirits)) { cardNameToCardList[cardName.firespirits].Atk = c.Atk; cardNameToCardList[cardName.firespirits].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.firespirits].aoeGround = c.aoeGround; cardNameToCardList[cardName.firespirits].aoeAir = c.aoeAir; } break;
                    case "PrincessProjectile": if (cardNameToCardList.ContainsKey(cardName.princess)) { cardNameToCardList[cardName.princess].Atk = c.Atk; cardNameToCardList[cardName.princess].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.princess].aoeGround = c.aoeGround; cardNameToCardList[cardName.princess].aoeAir = c.aoeAir; } break;
                    case "AxeManProjectile": if (cardNameToCardList.ContainsKey(cardName.axeman)) { cardNameToCardList[cardName.axeman].Atk = c.Atk; cardNameToCardList[cardName.axeman].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.axeman].aoeGround = c.aoeGround; cardNameToCardList[cardName.axeman].aoeAir = c.aoeAir; } break;
                    case "ice_wizardProjectile": if (cardNameToCardList.ContainsKey(cardName.icewizard)) { cardNameToCardList[cardName.icewizard].Atk = c.Atk; cardNameToCardList[cardName.icewizard].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.icewizard].aoeGround = c.aoeGround; cardNameToCardList[cardName.icewizard].aoeAir = c.aoeAir; } break;
                    case "chr_wizardProjectile": if (cardNameToCardList.ContainsKey(cardName.wizard)) { cardNameToCardList[cardName.wizard].Atk = c.Atk; cardNameToCardList[cardName.wizard].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.wizard].aoeGround = c.aoeGround; cardNameToCardList[cardName.wizard].aoeAir = c.aoeAir; } break;
                    case "BombSkeletonProjectile": if (cardNameToCardList.ContainsKey(cardName.bomber)) { cardNameToCardList[cardName.bomber].Atk = c.Atk; cardNameToCardList[cardName.bomber].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.bomber].aoeGround = c.aoeGround; cardNameToCardList[cardName.bomber].aoeAir = c.aoeAir; } break;
                    case "ZapMachineProjectile": if (cardNameToCardList.ContainsKey(cardName.zapmachine)) { cardNameToCardList[cardName.zapmachine].Atk = c.Atk; cardNameToCardList[cardName.zapmachine].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.zapmachine].aoeGround = c.aoeGround; cardNameToCardList[cardName.zapmachine].aoeAir = c.aoeAir; } break;
                    case "BowlerProjectile": if (cardNameToCardList.ContainsKey(cardName.bowler)) { cardNameToCardList[cardName.bowler].Atk = c.Atk; cardNameToCardList[cardName.bowler].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.bowler].aoeGround = c.aoeGround; cardNameToCardList[cardName.bowler].aoeAir = c.aoeAir; } break;
                    case "MovingCannonProjectile":
                        if (cardNameToCardList.ContainsKey(cardName.movingcannon)) { cardNameToCardList[cardName.movingcannon].Atk = c.Atk; cardNameToCardList[cardName.movingcannon].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.movingcannon].aoeGround = c.aoeGround; cardNameToCardList[cardName.movingcannon].aoeAir = c.aoeAir; }
                        if (cardNameToCardList.ContainsKey(cardName.brokencannon)) { cardNameToCardList[cardName.brokencannon].Atk = c.Atk; cardNameToCardList[cardName.brokencannon].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.brokencannon].aoeGround = c.aoeGround; cardNameToCardList[cardName.brokencannon].aoeAir = c.aoeAir; }
                        break;
                    case "MegaKnightAppear": if (cardNameToCardList.ContainsKey(cardName.megaknight)) { cardNameToCardList[cardName.megaknight].Atk = c.Atk; cardNameToCardList[cardName.megaknight].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.megaknight].aoeGround = c.aoeGround; cardNameToCardList[cardName.megaknight].aoeAir = c.aoeAir; } break;
                    case "KingProjectile": if (cardNameToCardList.ContainsKey(cardName.kingtower)) { cardNameToCardList[cardName.kingtower].Atk = c.Atk; cardNameToCardList[cardName.kingtower].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.kingtower].aoeGround = c.aoeGround; cardNameToCardList[cardName.kingtower].aoeAir = c.aoeAir; } break;
                    case "TowerCannonball": if (cardNameToCardList.ContainsKey(cardName.cannon)) { cardNameToCardList[cardName.cannon].Atk = c.Atk; cardNameToCardList[cardName.cannon].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.cannon].aoeGround = c.aoeGround; cardNameToCardList[cardName.cannon].aoeAir = c.aoeAir; } break;
                    case "MortarProjectile": if (cardNameToCardList.ContainsKey(cardName.mortar)) { cardNameToCardList[cardName.mortar].Atk = c.Atk; cardNameToCardList[cardName.mortar].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.mortar].aoeGround = c.aoeGround; cardNameToCardList[cardName.mortar].aoeAir = c.aoeAir; } break;
                    case "BombTowerProjectile": if (cardNameToCardList.ContainsKey(cardName.bombtower)) { cardNameToCardList[cardName.bombtower].Atk = c.Atk; cardNameToCardList[cardName.bombtower].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.bombtower].aoeGround = c.aoeGround; cardNameToCardList[cardName.bombtower].aoeAir = c.aoeAir; } break;
                    case "xbow_projectile": if (cardNameToCardList.ContainsKey(cardName.xbow)) { cardNameToCardList[cardName.xbow].Atk = c.Atk; cardNameToCardList[cardName.xbow].DamageRadius = c.DamageRadius; cardNameToCardList[cardName.xbow].aoeGround = c.aoeGround; cardNameToCardList[cardName.xbow].aoeAir = c.aoeAir; } break;

                    default:
                        if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                        else Logger.Error("#####ERR. Duplicate name: {name}", c.name); ;
                        break;
                }
            }

            //add groups
            if (cardNameToCardList.ContainsKey(cardName.skeleton))
            {
                cardNameToCardList[cardName.skeleton].SightClip = 1000;
                c = new Card(cardNameToCardList[cardName.skeleton]);
                c.stringName = "SkeletonArmy";
                c.name = cardNamestringToEnum(c.stringName, "11");
                c.cost = 3;
                c.SummonNumber = 14;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);

                c = new Card(cardNameToCardList[cardName.skeleton]);
                c.stringName = "Skeletons";
                c.name = cardNamestringToEnum(c.stringName, "12");
                c.cost = 1;
                c.SummonNumber = 3;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }

            if (cardNameToCardList.ContainsKey(cardName.goblin))
            {
                cardNameToCardList[cardName.goblin].SightClip = 1000;
                c = new Card(cardNameToCardList[cardName.goblin]);
                c.stringName = "Goblins";
                c.name = cardNamestringToEnum(c.stringName, "13");
                c.cost = 2;
                c.SummonNumber = 3;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }

            if (cardNameToCardList.ContainsKey(cardName.speargoblin))
            {
                cardNameToCardList[cardName.speargoblin].SightClip = 1000;
                c = new Card(cardNameToCardList[cardName.speargoblin]);
                c.stringName = "SpearGoblins";
                c.name = cardNamestringToEnum(c.stringName, "14");
                c.cost = 2;
                c.SummonNumber = 3;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }

            if (cardNameToCardList.ContainsKey(cardName.barbarian))
            {
                cardNameToCardList[cardName.barbarian].SightClip = 1000;
                c = new Card(cardNameToCardList[cardName.barbarian]);
                c.stringName = "Barbarians";
                c.name = cardNamestringToEnum(c.stringName, "15");
                c.cost = 5;
                c.SummonNumber = 4;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }

            if (cardNameToCardList.ContainsKey(cardName.minion))
            {
                cardNameToCardList[cardName.minion].SightClip = 1000;
                c = new Card(cardNameToCardList[cardName.minion]);
                c.stringName = "MinionHorde";
                c.name = cardNamestringToEnum(c.stringName, "16");
                c.cost = 5;
                c.SummonNumber = 6;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);

                c = new Card(cardNameToCardList[cardName.minion]);
                c.stringName = "Minions";
                c.name = cardNamestringToEnum(c.stringName, "17");
                c.cost = 3;
                c.SummonNumber = 3;
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }

            Logger.Debug("CardList: {Count}", cardNameToCardList.Count);
        }

        private void initBasicDBfromString()
        {
            string basicCards = " cName:rage stringName:Rage type:AOE Transport:NONE TargetType:NONE affectType:ONLY_OWN cost:2 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:300 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:5000 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:0 LifeTime:6000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:freeze stringName:Freeze type:AOE Transport:NONE TargetType:NONE affectType:ONLY_ENEMIES cost:4 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:3000 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:0 LifeTime:4000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:lightning stringName:Lightning type:AOE Transport:NONE TargetType:NONE affectType:ONLY_ENEMIES cost:6 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:650 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:460 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:3500 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:260 LifeTime:1500 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:zap stringName:Zap type:AOE Transport:NONE TargetType:NONE affectType:ONLY_ENEMIES cost:2 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:75 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:2500 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:30 LifeTime:1 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:poison stringName:Poison type:AOE Transport:NONE TargetType:NONE affectType:ONLY_ENEMIES cost:4 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:57 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:250 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:3500 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:23 LifeTime:8000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:barbarianrage stringName:BarbarianRage type:AOE Transport:NONE TargetType:NONE affectType:ONLY_OWN cost:-1 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:300 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity: Level:1 DamageRadius:5000 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:7500 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:graveyard stringName:Graveyard type:AOE Transport:NONE TargetType:NONE affectType:ONLY_ENEMIES cost:5 DeployTime:1500 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:5000 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:9000 SummonNumber:15 SpawnNumber:0 SpawnPause:0 SpawnInterval:500 SpawnCharacter:skeleton SpawnCharacterLevel:9 cName:freezeicegolemite stringName:FreezeIceGolemite type:AOE Transport:NONE TargetType:NONE affectType:ONLY_ENEMIES cost:-1 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity: Level:1 DamageRadius:2000 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:2000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:tornado stringName:Tornado type:AOE Transport:NONE TargetType:IGNOREBUILDINGS affectType:ONLY_ENEMIES cost:3 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:44 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:50 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:5500 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:0 LifeTime:2500 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:clone stringName:Clone type:AOE Transport:NONE TargetType:IGNOREBUILDINGS affectType:ONLY_OWN cost:3 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:3000 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:0 LifeTime:1000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:not_in_use stringName:NOT_IN_USE type:AOE Transport:NONE TargetType:IGNOREBUILDINGS affectType:ONLY_OWN cost:-1 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity: Level:1 DamageRadius:3000 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:1000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:heal stringName:Heal type:AOE Transport:NONE TargetType:IGNOREBUILDINGS affectType:ONLY_OWN cost:3 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:-100 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:50 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:3000 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:0 LifeTime:2500 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:balloonbomb stringName:BalloonBomb type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:-1 DeployTime:3000 DeployDelay:0 MaxHP:0 Atk:205 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1500 aoeGround:True aoeAir:True CollisionRadius:450 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:barbarianhut stringName:BarbarianHut type:BUILDING Transport:GROUND TargetType:GROUND affectType:NONE cost:7 DeployTime:1000 DeployDelay:0 MaxHP:1100 Atk:75 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:60000 SummonNumber:0 SpawnNumber:2 SpawnPause:14000 SpawnInterval:500 SpawnCharacter:barbarian SpawnCharacterLevel:3 cName:tombstone stringName:Tombstone type:BUILDING Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:240 Atk:32 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:40000 SummonNumber:0 SpawnNumber:1 SpawnPause:2950 SpawnInterval:0 SpawnCharacter:skeleton SpawnCharacterLevel:3 cName:goblinhut stringName:GoblinHut type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:735 Atk:24 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:60000 SummonNumber:0 SpawnNumber:1 SpawnPause:4900 SpawnInterval:0 SpawnCharacter:speargoblin SpawnCharacterLevel:3 cName:firespirithut stringName:FirespiritHut type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:570 Atk:80 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:True aoeAir:True CollisionRadius:1000 towerDamage:0 LifeTime:50000 SummonNumber:0 SpawnNumber:2 SpawnPause:10000 SpawnInterval:500 SpawnCharacter:firespirits SpawnCharacterLevel:3 cName:bombtower stringName:BombTower type:BUILDING Transport:GROUND TargetType:GROUND affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:950 Atk:100 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:1600 MinRange:0 MaxRange:6000 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:1500 aoeGround:True aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:40000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:cannon stringName:Cannon type:BUILDING Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:350 Atk:60 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:800 MinRange:0 MaxRange:5500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:30000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:elixircollector stringName:ElixirCollector type:BUILDING Transport:GROUND TargetType:NONE affectType:NONE cost:6 DeployTime:1000 DeployDelay:0 MaxHP:580 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:70000 SummonNumber:0 SpawnNumber:0 SpawnPause:8500 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:giantskeletonbomb stringName:GiantSkeletonBomb type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:-1 DeployTime:3000 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:450 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:infernotower stringName:InfernoTower type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:800 Atk:20 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:400 MinRange:0 MaxRange:6000 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:40000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:kingtower stringName:KingTower type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:0 DeployTime:0 DeployDelay:0 MaxHP:2400 Atk:50 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:1000 MinRange:0 MaxRange:7000 SightRange:7000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1400 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:kingtowermiddle stringName:KingTowerMiddle type:BUILDING Transport:GROUND TargetType:NONE affectType:NONE cost:0 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1400 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:mortar stringName:Mortar type:BUILDING Transport:GROUND TargetType:GROUND affectType:NONE cost:4 DeployTime:3500 DeployDelay:0 MaxHP:600 Atk:108 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:5000 MinRange:3000 MaxRange:11000 SightRange:11000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:2000 aoeGround:True aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:30000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse21 stringName:NOTINUSE21 type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:-1 DeployTime:0 DeployDelay:0 MaxHP:2400 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:1000 MinRange:0 MaxRange:7000 SightRange:7000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1400 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse22 stringName:NOTINUSE22 type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:800 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:10000 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:40000 SummonNumber:0 SpawnNumber:4 SpawnPause:6500 SpawnInterval:150 SpawnCharacter: SpawnCharacterLevel:8 cName:princesstower stringName:PrincessTower type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:0 DeployTime:0 DeployDelay:0 MaxHP:1400 Atk:50 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:800 MinRange:0 MaxRange:7500 SightRange:7500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:ragebarbarianbottle stringName:RageBarbarianBottle type:BUILDING Transport:GROUND TargetType:NONE affectType:NONE cost:-1 DeployTime:500 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:skeletoncontainer stringName:SkeletonContainer type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:-1 DeployTime:600 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:tesla stringName:Tesla type:BUILDING Transport:GROUND TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:450 Atk:64 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:800 MinRange:0 MaxRange:5500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:40000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:xbow stringName:Xbow type:BUILDING Transport:GROUND TargetType:GROUND affectType:NONE cost:6 DeployTime:3500 DeployDelay:0 MaxHP:1000 Atk:20 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:250 MinRange:0 MaxRange:11500 SightRange:11500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:40000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:angrybarbarians stringName:AngryBarbarian type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:6 DeployTime:1000 DeployDelay:400 MaxHP:458 Atk:120 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1500 MinRange:0 MaxRange:1000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:2 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:angrybarbarian stringName:AngryBarbarian type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:6 DeployTime:1000 DeployDelay:400 MaxHP:458 Atk:120 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1500 MinRange:0 MaxRange:1000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:archer stringName:Archer type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:120 Atk:41 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1200 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:2 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:assassin stringName:Assassin type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:780 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:axeman stringName:AxeMan type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:5 DeployTime:1000 DeployDelay:400 MaxHP:760 Atk:106 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:2400 MinRange:0 MaxRange:4500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:2 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1000 aoeGround:True aoeAir:True CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:babydragon stringName:BabyDragon type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:800 Atk:100 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1600 MinRange:0 MaxRange:3500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1200 aoeGround:True aoeAir:True CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:balloon stringName:Balloon type:MOB Transport:AIR TargetType:BUILDINGS affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:1050 Atk:600 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:3000 MinRange:0 MaxRange:100 SightRange:7700 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:barbarian stringName:Barbarian type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:0 DeployTime:1000 DeployDelay:400 MaxHP:300 Atk:75 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:700 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:barbarians stringName:Barbarians type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:5 DeployTime:1000 DeployDelay:400 MaxHP:300 Atk:75 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:700 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:4 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:bat stringName:Bat type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:2 DeployTime:1000 DeployDelay:400 MaxHP:32 Atk:32 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1000 MinRange:0 MaxRange:1200 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:5 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:battleram stringName:BattleRam type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:430 Atk:140 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:400 MinRange:0 MaxRange:500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:2 SpawnPause:0 SpawnInterval:0 SpawnCharacter:barbarian SpawnCharacterLevel:3 cName:blowdartgoblin stringName:BlowdartGoblin type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:123 Atk:53 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:650 MinRange:0 MaxRange:6500 SightRange:7500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:bomber stringName:Bomber type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:147 Atk:128 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1900 MinRange:0 MaxRange:4500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:1500 aoeGround:True aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:bowler stringName:Bowler type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:1200 Atk:180 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:2500 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1800 aoeGround:True aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:brokencannon stringName:BrokenCannon type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:5 DeployTime:1000 DeployDelay:400 MaxHP:524 Atk:153 Shield:524 SpawnDamage:0 Speed:0 HitSpeed:1200 MinRange:0 MaxRange:5500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:20000 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:darkprince stringName:DarkPrince type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:735 Atk:155 Shield:200 SpawnDamage:0 Speed:60 HitSpeed:1400 MinRange:0 MaxRange:1050 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1000 aoeGround:True aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:darkwitch stringName:DarkWitch type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:260 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:1850 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:2 SpawnPause:7000 SpawnInterval:0 SpawnCharacter:bat SpawnCharacterLevel:9 cName:dartbarrell stringName:DartBarrell type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:400 MaxHP:290 Atk:81 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:6000 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:electrowizard stringName:ElectroWizard type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:590 Atk:100 Shield:0 SpawnDamage:159 Speed:90 HitSpeed:1800 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:0 MultipleTargets:2 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:firespirits stringName:FireSpirits type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:2 DeployTime:1000 DeployDelay:400 MaxHP:43 Atk:80 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:300 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:1500 aoeGround:True aoeAir:True CollisionRadius:400 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:giant stringName:Giant type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:1900 Atk:120 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:1500 MinRange:0 MaxRange:1250 SightRange:7500 SightClip:2000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:giantskeleton stringName:GiantSkeleton type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:6 DeployTime:1000 DeployDelay:0 MaxHP:2000 Atk:130 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:800 SightRange:5000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:goblin stringName:Goblin type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:0 DeployTime:1000 DeployDelay:400 MaxHP:80 Atk:50 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1100 MinRange:0 MaxRange:500 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:goblins stringName:Goblins type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:2 DeployTime:1000 DeployDelay:400 MaxHP:80 Atk:50 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1100 MinRange:0 MaxRange:500 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:goblingang stringName:GoblinGang type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:80 Atk:50 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1100 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:5 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:golem stringName:Golem type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:8 DeployTime:3000 DeployDelay:0 MaxHP:3200 Atk:195 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:2500 MinRange:0 MaxRange:750 SightRange:7000 SightClip:2000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:2 SpawnPause:0 SpawnInterval:0 SpawnCharacter:golemite SpawnCharacterLevel:1 cName:golemite stringName:Golemite type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:0 DeployTime:1000 DeployDelay:0 MaxHP:650 Atk:40 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:2500 MinRange:0 MaxRange:250 SightRange:7000 SightClip:2000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:hogrider stringName:HogRider type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:4 DeployTime:1000 DeployDelay:400 MaxHP:800 Atk:150 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1500 MinRange:0 MaxRange:800 SightRange:9500 SightClip:4000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:icegolemite stringName:IceGolemite type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:2 DeployTime:1000 DeployDelay:0 MaxHP:595 Atk:40 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:2500 MinRange:0 MaxRange:750 SightRange:7000 SightClip:2000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:700 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:icespirits stringName:IceSpirits type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:1 DeployTime:1000 DeployDelay:400 MaxHP:90 Atk:45 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:300 MinRange:0 MaxRange:2500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:1500 aoeGround:True aoeAir:True CollisionRadius:400 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:icewizard stringName:IceWizard type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:665 Atk:69 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1700 MinRange:0 MaxRange:5500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:1000 aoeGround:True aoeAir:True CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:infernodragon stringName:InfernoDragon type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:1070 Atk:30 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:400 MinRange:0 MaxRange:4000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:knight stringName:Knight type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:660 Atk:75 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1100 MinRange:0 MaxRange:1000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:lavahound stringName:LavaHound type:MOB Transport:AIR TargetType:BUILDINGS affectType:NONE cost:7 DeployTime:1000 DeployDelay:0 MaxHP:3000 Atk:45 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:1300 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:6 SpawnPause:0 SpawnInterval:0 SpawnCharacter:lavapups SpawnCharacterLevel:1 cName:lavapups stringName:LavaPups type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:0 DeployTime:1000 DeployDelay:0 MaxHP:179 Atk:45 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1000 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:450 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:megaknight stringName:MegaKnight type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:7 DeployTime:1000 DeployDelay:0 MaxHP:3300 Atk:240 Shield:0 SpawnDamage:480 Speed:60 HitSpeed:1800 MinRange:0 MaxRange:1000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:2500 aoeGround:True aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:megaminion stringName:MegaMinion type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:395 Atk:147 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:miner stringName:Miner type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:1000 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1200 MinRange:0 MaxRange:1300 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:minion stringName:Minion type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:0 DeployTime:1000 DeployDelay:400 MaxHP:90 Atk:40 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:minionhorde stringName:MinionHorde type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:5 DeployTime:1000 DeployDelay:400 MaxHP:90 Atk:40 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:6 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:minions stringName:Minions type:MOB Transport:AIR TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:90 Atk:40 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:2000 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:minipekka stringName:MiniPekka type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:600 Atk:340 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1800 MinRange:0 MaxRange:1050 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:450 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:movingcannon stringName:MovingCannon type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:5 DeployTime:1000 DeployDelay:400 MaxHP:524 Atk:153 Shield:524 SpawnDamage:0 Speed:90 HitSpeed:1200 MinRange:0 MaxRange:5500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:musketeer stringName:Musketeer type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:4 DeployTime:1000 DeployDelay:300 MaxHP:340 Atk:100 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1100 MinRange:0 MaxRange:6000 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:threemusketeers stringName:ThreeMusketeers type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:9 DeployTime:1000 DeployDelay:300 MaxHP:340 Atk:100 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1100 MinRange:0 MaxRange:6000 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse1 stringName:NOTINUSE1 type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse2 stringName:NOTINUSE2 type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse3 stringName:NOTINUSE3 type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse4 stringName:NOTINUSE4 type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse5 stringName:NOTINUSE5 type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse8 stringName:NOTINUSE8 type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:-1 DeployTime:1000 DeployDelay:400 MaxHP:320 Atk:55 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:800 MinRange:0 MaxRange:500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:notinuse9 stringName:NOTINUSE9 type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:-1 DeployTime:1000 DeployDelay:0 MaxHP:750 Atk:160 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:750 SightRange:6000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:600 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:pekka stringName:Pekka type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:7 DeployTime:1000 DeployDelay:0 MaxHP:2600 Atk:510 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:1800 MinRange:0 MaxRange:750 SightRange:5000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:prince stringName:Prince type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:1100 Atk:245 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:1850 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:650 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:princess stringName:Princess type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:216 Atk:140 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:3000 MinRange:0 MaxRange:9000 SightRange:9500 SightClip:0 MultipleTargets:0 MultipleProjectiles:5 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:2000 aoeGround:True aoeAir:True CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:ragebarbarian stringName:RageBarbarian type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:4 DeployTime:1000 DeployDelay:400 MaxHP:990 Atk:200 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:700 MinRange:0 MaxRange:700 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:royalgiant stringName:RoyalGiant type:MOB Transport:GROUND TargetType:BUILDINGS affectType:NONE cost:6 DeployTime:2000 DeployDelay:0 MaxHP:1200 Atk:75 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:1700 MinRange:0 MaxRange:6500 SightRange:7500 SightClip:2000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:750 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:skeleton stringName:Skeleton type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:0 DeployTime:1000 DeployDelay:400 MaxHP:32 Atk:32 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:500 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:skeletonarmy stringName:SkeletonArmy type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:32 Atk:32 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:500 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:14 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter:skeleton SpawnCharacterLevel:1 cName:skeletons stringName:Skeletons type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:1 DeployTime:1000 DeployDelay:400 MaxHP:32 Atk:32 Shield:0 SpawnDamage:0 Speed:90 HitSpeed:1000 MinRange:0 MaxRange:500 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter:skeleton SpawnCharacterLevel:1 cName:skeletonballoon stringName:SkeletonBalloon type:MOB Transport:AIR TargetType:BUILDINGS affectType:NONE cost:3 DeployTime:1000 DeployDelay:0 MaxHP:300 Atk:0 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:300 MinRange:0 MaxRange:350 SightRange:7700 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:8 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:skeletonwarrior stringName:SkeletonWarrior type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:3 DeployTime:1000 DeployDelay:400 MaxHP:65 Atk:65 Shield:150 SpawnDamage:0 Speed:90 HitSpeed:1200 MinRange:0 MaxRange:1600 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:speargoblin stringName:SpearGoblin type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:2 DeployTime:1000 DeployDelay:400 MaxHP:52 Atk:24 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1300 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:speargoblins stringName:SpearGoblins type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:2 DeployTime:1000 DeployDelay:400 MaxHP:52 Atk:24 Shield:0 SpawnDamage:0 Speed:120 HitSpeed:1300 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:1000 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:3 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:towerprincess stringName:TowerPrincess type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:-1 DeployTime:1000 DeployDelay:400 MaxHP:125 Atk:50 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1200 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:valkyrie stringName:Valkyrie type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:4 DeployTime:1000 DeployDelay:0 MaxHP:880 Atk:120 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1500 MinRange:0 MaxRange:1000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:2000 aoeGround:True aoeAir:False CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:witch stringName:Witch type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:524 Atk:52 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:700 MinRange:0 MaxRange:5000 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1100 aoeGround:True aoeAir:True CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:3 SpawnPause:7000 SpawnInterval:300 SpawnCharacter:skeleton SpawnCharacterLevel:6 cName:wizard stringName:Wizard type:MOB Transport:GROUND TargetType:ALL affectType:NONE cost:5 DeployTime:1000 DeployDelay:0 MaxHP:340 Atk:130 Shield:0 SpawnDamage:0 Speed:60 HitSpeed:1400 MinRange:0 MaxRange:5500 SightRange:5500 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:1200 aoeGround:True aoeAir:True CollisionRadius:500 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:zapmachine stringName:ZapMachine type:MOB Transport:GROUND TargetType:GROUND affectType:NONE cost:6 DeployTime:1000 DeployDelay:0 MaxHP:1200 Atk:1300 Shield:0 SpawnDamage:0 Speed:45 HitSpeed:5000 MinRange:0 MaxRange:4500 SightRange:5000 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:1800 aoeGround:True aoeAir:False CollisionRadius:1000 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:fireball stringName:Fireball type:PROJECTILE Transport:NONE TargetType:ALL affectType:ONLY_ENEMIES cost:4 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:325 Shield:0 SpawnDamage:0 Speed:600 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:2500 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:130 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:arrows stringName:Arrows type:PROJECTILE Transport:NONE TargetType:ALL affectType:ONLY_ENEMIES cost:3 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:115 Shield:0 SpawnDamage:0 Speed:800 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:4000 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:46 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:arrowsspelldeco stringName:ArrowsSpellDeco type:PROJECTILE Transport:NONE TargetType:ALL affectType:ONLY_ENEMIES cost:-1 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:800 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Common Level:1 DamageRadius:0 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:rocket stringName:Rocket type:PROJECTILE Transport:NONE TargetType:ALL affectType:ONLY_ENEMIES cost:6 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:700 Shield:0 SpawnDamage:0 Speed:350 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Rare Level:1 DamageRadius:2000 aoeGround:True aoeAir:True CollisionRadius:0 towerDamage:280 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:goblinbarrel stringName:GoblinBarrel type:PROJECTILE Transport:NONE TargetType:GROUND affectType:ONLY_ENEMIES cost:3 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:400 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:1500 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:3 SpawnPause:0 SpawnInterval:0 SpawnCharacter:goblin SpawnCharacterLevel:6 cName:princessprojectiledeco stringName:PrincessProjectileDeco type:PROJECTILE Transport:NONE TargetType:ALL affectType:ONLY_ENEMIES cost:-1 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:450 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:log stringName:Log type:PROJECTILE Transport:NONE TargetType:GROUND affectType:ONLY_ENEMIES cost:2 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:240 Shield:0 SpawnDamage:0 Speed:360 HitSpeed:0 MinRange:0 MaxRange:11100 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:1950 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:96 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:logprojectilerolling stringName:LogProjectileRolling type:PROJECTILE Transport:NONE TargetType:ALL affectType:ONLY_ENEMIES cost:2 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:240 Shield:0 SpawnDamage:0 Speed:360 HitSpeed:0 MinRange:0 MaxRange:11100 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Legendary Level:1 DamageRadius:1950 aoeGround:True aoeAir:False CollisionRadius:0 towerDamage:96 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0 cName:mirror stringName:Mirror type:NONE Transport:NONE TargetType:NONE affectType:NONE cost:-1 DeployTime:0 DeployDelay:0 MaxHP:0 Atk:0 Shield:0 SpawnDamage:0 Speed:0 HitSpeed:0 MinRange:0 MaxRange:0 SightRange:0 SightClip:0 MultipleTargets:0 MultipleProjectiles:0 DeathEffect:0 Rarity:Epic Level:1 DamageRadius:0 aoeGround:False aoeAir:False CollisionRadius:0 towerDamage:0 LifeTime:0 SummonNumber:0 SpawnNumber:0 SpawnPause:0 SpawnInterval:0 SpawnCharacter: SpawnCharacterLevel:0";
            string[] cardsTxt = basicCards.Split(new string[] { " cName:" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ct in cardsTxt)
            {
                string[] paramsTxt = ct.Split(' ');
                Card c = new Card();
                c.name = cardNamestringToEnum(paramsTxt[0], "18");
                int count = paramsTxt.Length;
                for (int i = 1; i < count; i++)
                {
                    string[] oneParam = paramsTxt[i].Split(':');
                    switch (oneParam[0])
                    {
                        case "stringName": c.stringName = oneParam[1]; continue;
                        case "type": c.type = boardObjTypeStringToEnum(oneParam[1]); continue;
                        case "Transport": c.Transport = transportTypeStringToEnum(oneParam[1]); continue;
                        case "TargetType": c.TargetType = targetTypeStringToEnum(oneParam[1]); continue;
                        case "affectType": c.affectType = affectTypeStringToEnum(oneParam[1]); continue;
                        case "cost": c.cost = Convert.ToInt32(oneParam[1]); continue;
                        case "DeployTime": c.DeployTime = Convert.ToInt32(oneParam[1]); continue;
                        case "DeployDelay": c.DeployDelay = Convert.ToInt32(oneParam[1]); continue;
                        case "MaxHP": c.MaxHP = Convert.ToInt32(oneParam[1]); continue;
                        case "Atk": c.Atk = Convert.ToInt32(oneParam[1]); continue;
                        case "Shield": c.Shield = Convert.ToInt32(oneParam[1]); continue;
                        case "SpawnDamage": c.SpawnDamage = Convert.ToInt32(oneParam[1]); continue;
                        case "Speed": c.Speed = Convert.ToInt32(oneParam[1]); continue;
                        case "HitSpeed": c.HitSpeed = Convert.ToInt32(oneParam[1]); continue;
                        case "MinRange": c.MinRange = Convert.ToInt32(oneParam[1]); continue;
                        case "MaxRange": c.MaxRange = Convert.ToInt32(oneParam[1]); continue;
                        case "SightRange": c.SightRange = Convert.ToInt32(oneParam[1]); continue;
                        case "SightClip": c.SightClip = Convert.ToInt32(oneParam[1]); continue;
                        case "MultipleTargets": c.MultipleTargets = Convert.ToInt32(oneParam[1]); continue;
                        case "MultipleProjectiles": c.MultipleProjectiles = Convert.ToInt32(oneParam[1]); continue;
                        case "DeathEffect": c.DeathEffect = Convert.ToInt32(oneParam[1]); continue;
                        case "Rarity": c.Rarity = oneParam[1]; continue;
                        case "Level": c.Level = Convert.ToInt32(oneParam[1]); continue;
                        case "DamageRadius": c.DamageRadius = Convert.ToInt32(oneParam[1]); continue;
                        case "aoeGround": c.aoeGround = oneParam[1] == "True"; continue;
                        case "aoeAir": c.aoeAir = oneParam[1] == "True"; continue;
                        case "CollisionRadius": c.CollisionRadius = Convert.ToInt32(oneParam[1]); continue;
                        case "towerDamage": c.towerDamage = Convert.ToInt32(oneParam[1]); continue;
                        case "LifeTime": c.LifeTime = Convert.ToInt32(oneParam[1]); continue;
                        case "SummonNumber": c.SummonNumber = Convert.ToInt32(oneParam[1]); continue;
                        case "SpawnNumber": c.SpawnNumber = Convert.ToInt32(oneParam[1]); continue;
                        case "SpawnPause": c.SpawnPause = Convert.ToInt32(oneParam[1]); continue;
                        case "SpawnInterval": c.SpawnInterval = Convert.ToInt32(oneParam[1]); continue;
                        case "SpawnCharacter": c.SpawnCharacter = oneParam[1]; continue;
                        case "SpawnCharacterLevel": c.SpawnCharacterLevel = Convert.ToInt32(oneParam[1]); continue;
                    }
                }
                if (!cardNameToCardList.ContainsKey(c.name)) cardNameToCardList.Add(c.name, c);
                else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
            }
        }

        private void initCardsAdjustmentContainerInt(bool onlyClear = false)
        {
            if (!onlyClear || cardsAdjustmentContainerInt.Count == 0)
            {
                cardsAdjustmentContainerInt = new Dictionary<cardParamInt, Dictionary<int, int>> {
                    {cardParamInt.cost, new Dictionary<int, int>()},
                    {cardParamInt.DeployTime, new Dictionary<int, int>()},
                    {cardParamInt.DeployDelay, new Dictionary<int, int>()},
                    {cardParamInt.MaxHP, new Dictionary<int, int>()},
                    {cardParamInt.Atk, new Dictionary<int, int>()},
                    {cardParamInt.Shield, new Dictionary<int, int>()},
                    {cardParamInt.SpawnDamage, new Dictionary<int, int>()},
                    {cardParamInt.Speed, new Dictionary<int, int>()},
                    {cardParamInt.HitSpeed, new Dictionary<int, int>()},
                    {cardParamInt.MinRange, new Dictionary<int, int>()},
                    {cardParamInt.MaxRange, new Dictionary<int, int>()},
                    {cardParamInt.SightRange, new Dictionary<int, int>()},
                    {cardParamInt.SightClip, new Dictionary<int, int>()},
                    {cardParamInt.MultipleTargets, new Dictionary<int, int>()},
                    {cardParamInt.MultipleProjectiles, new Dictionary<int, int>()},
                    {cardParamInt.Level, new Dictionary<int, int>()},
                    {cardParamInt.DamageRadius, new Dictionary<int, int>()},
                    {cardParamInt.CollisionRadius, new Dictionary<int, int>()},
                    {cardParamInt.LifeTime, new Dictionary<int, int>()},
                    {cardParamInt.SummonNumber, new Dictionary<int, int>()},
                    {cardParamInt.SpawnNumber, new Dictionary<int, int>()},
                    {cardParamInt.SpawnPause, new Dictionary<int, int>()},
                    {cardParamInt.SpawnInterval, new Dictionary<int, int>()},
                    {cardParamInt.SpawnCharacterLevel, new Dictionary<int, int>()}
                };
            }
            else
            {
                foreach (var pair in cardsAdjustmentContainerInt) pair.Value.Clear();
            }
        }

        public boardObjType boardObjTypeStringToEnum(string s)
        {
            boardObjType retval;
            if (Enum.TryParse<boardObjType>(s, false, out retval)) return retval;
            else
            {
                Logger.Debug("!!!NEW boardObjType: {s}", s);
                return boardObjType.NONE;
            }
        }

        public transportType transportTypeStringToEnum(string s)
        {
            transportType retval;
            if (Enum.TryParse<transportType>(s, false, out retval)) return retval;
            else
            {
                Logger.Debug("!!!NEW transportType: {s}", s);
                return transportType.NONE;
            }
        }

        public targetType targetTypeStringToEnum(string s)
        {
            targetType retval;
            if (Enum.TryParse<targetType>(s, false, out retval)) return retval;
            else
            {
                Logger.Debug("!!!NEW targetType: {s}", s);
                return targetType.NONE;
            }
        }

        public affectType affectTypeStringToEnum(string s)
        {
            affectType retval;
            if (Enum.TryParse<affectType>(s, false, out retval)) return retval;
            else
            {
                Logger.Debug("!!!NEW affectType: {s}", s);
                return affectType.NONE;
            }
        }


        public void cardsAdjustment(Clash.Engine.NativeObjects.LogicData.Spell spell)
        {
            if (spell == null || !spell.IsValid) return;
            cardName cName = cardNamestringToEnum(spell.Name.Value, "19");
            if (cName == cardName.unknown) return;

            if (cardsAdjustmentDB.ContainsKey(cName))
            {
                List<Card> list = cardsAdjustmentDB[cName];
                if (list[0].needUpdate)
                {
                    Card c = collectNewCards(spell, false);
                    list.Add(c);
                    Logger.Debug("Add {0} {1}", c.name, list.Count);
                    if (list.Count >= updCardsMeasure)
                    {
                        //updateCardData(list);
                    }
                }
            }
        }

        public void cardsAdjustment(Clash.Engine.NativeObjects.Logic.GameObjects.Character @char)
        {
            if (@char == null || !@char.IsValid) return;
            var LogicDataCharacter = @char.LogicGameObjectData;
            if (!LogicDataCharacter.IsValid) return;

            cardName cName = cardNamestringToEnum(LogicDataCharacter.Name.Value, "6");
            if (cName == cardName.unknown) return;

            if (cardsAdjustmentDB.ContainsKey(cName))
            {
                List<Card> list = cardsAdjustmentDB[cName];
                if (list[0].needUpdate)
                {
                    Card c = collectNewCards(@char, false);
                    list.Add(c);
                    Logger.Debug("Add {0} {1}", c.name, list.Count);
                    if (list.Count >= updCardsMeasure)
                    {
                        updateCardData(list, "char");
                    }
                }
            }
        }

        private void updateCardData(List<Card> list, string sender)
        {
            if (list == null) return;
            int count = list.Count;
            if (count < 2) return;

            Card baseCard = list[0];
            baseCard.needUpdate = false;
            Logger.Debug("Total after {0} {1} {2}", list.Count, baseCard.name, cardNameToCardList[baseCard.name].needUpdate);

            initCardsAdjustmentContainerInt(true);
            Card c;
            for (int i = 1; i < count; i++)
            {
                c = list[i];
                switch (sender)
                {
                    case "char":
                        updCardsAdjustmentContainerInt(cardParamInt.MaxHP, c.MaxHP);
                        updCardsAdjustmentContainerInt(cardParamInt.Shield, c.Shield);
                        updCardsAdjustmentContainerInt(cardParamInt.Speed, c.Speed);
                        updCardsAdjustmentContainerInt(cardParamInt.Level, c.Level);
                        updCardsAdjustmentContainerInt(cardParamInt.DamageRadius, c.DamageRadius);
                        continue;
                    default:
                        Logger.Debug("[AI] Wrong updateCardData sender: {0}", sender);
                        continue;
                }

                /*
                updCardsAdjustmentContainerInt("cost", c.cost);
                updCardsAdjustmentContainerInt("DeployTime", c.DeployTime);
                updCardsAdjustmentContainerInt("DeployDelay", c.DeployDelay);
                updCardsAdjustmentContainerInt("Atk", c.Atk);
                updCardsAdjustmentContainerInt("SpawnDamage", c.SpawnDamage);
                updCardsAdjustmentContainerInt("HitSpeed", c.HitSpeed);
                updCardsAdjustmentContainerInt("MinRange", c.MinRange);
                updCardsAdjustmentContainerInt("MaxRange", c.MaxRange);
                updCardsAdjustmentContainerInt("SightRange", c.SightRange);
                updCardsAdjustmentContainerInt("SightClip", c.SightClip);
                updCardsAdjustmentContainerInt("MultipleTargets", c.MultipleTargets);
                updCardsAdjustmentContainerInt("MultipleProjectiles", c.MultipleProjectiles);
                updCardsAdjustmentContainerInt("CollisionRadius", c.CollisionRadius);
                updCardsAdjustmentContainerInt("LifeTime", c.LifeTime);
                updCardsAdjustmentContainerInt("SummonNumber", c.SummonNumber);
                updCardsAdjustmentContainerInt("SpawnNumber", c.SpawnNumber);
                updCardsAdjustmentContainerInt("SpawnPause", c.SpawnPause);
                updCardsAdjustmentContainerInt("SpawnInterval", c.SpawnInterval);
                updCardsAdjustmentContainerInt("SpawnCharacterLevel", c.SpawnCharacterLevel);
                */

            }

            foreach (var pair in cardsAdjustmentContainerInt)
            {
                int baseVal = baseCard.getParamByNameInt(pair.Key);
                int moda = int.MinValue;
                int maxNum = 0;
                foreach (var vals in pair.Value)
                {
                    if (vals.Value > maxNum)
                    {
                        if (baseVal > 0)
                        {
                            if (vals.Key < baseVal / 2 || vals.Key > baseVal * 5) continue;
                        }
                        else if (vals.Key > 1000000 || vals.Key < 0) continue;

                        maxNum = vals.Value;
                        moda = vals.Key;
                    }
                }
                Logger.Debug("adj: {0} moda:{1} : {2} Old val:{3}", pair.Key, moda, maxNum, baseVal);
                if (moda != int.MinValue)
                {
                    Logger.Debug("adj:!!!!!! moda:{0}", moda);
                }
            }
        }

        private void updCardsAdjustmentContainerInt(cardParamInt param, int val)
        {
            if (cardsAdjustmentContainerInt[param].ContainsKey(val)) cardsAdjustmentContainerInt[param][val]++;
            else cardsAdjustmentContainerInt[param].Add(val, 1);
        }

        public Card collectNewCards(Robi.Clash.Engine.NativeObjects.LogicData.Spell spell, bool needLog = true)
        {
            //try to fill missing data
            var SummonCharacter = spell.SummonCharacter;
            var Projectile = spell.Projectile;
            var AreaEffect = spell.AreaEffectObject;

            Card c = new Card();
            c.stringName = spell.Name.Value;
            c.name = cardNamestringToEnum(c.stringName, "20");
            c.cost = spell.ManaCost;
            c.MaxHP = 1;
            c.Atk = 1;
            c.MultipleProjectiles = spell.MultipleProjectiles;
            c.Rarity = spell.Rarity.Name.Value;
            c.DamageRadius = spell.Radius;
            c.SummonNumber = spell.SummonNumber;
            //c.Shield =
            //c.Speed =
            //c.Level =

            if (spell.Projectile.IsValid)
            {
                c.aoeGround = spell.Projectile.AoeToGround == 1;
                c.aoeAir = spell.Projectile.AoeToAir == 1;
            }
            if (SummonCharacter.IsValid)
            {
                c.TargetType = targetType.NONE;
                c.type = boardObjType.MOB; //TODO: divide with Buildings (is not enough data)
                if (SummonCharacter.IgnorePushback == 1) c.TargetType = targetType.BUILDINGS;
                else if (SummonCharacter.AttacksAir == 1) c.TargetType = targetType.ALL;
                else if (SummonCharacter.AttacksGround == 1) c.TargetType = targetType.GROUND;

                c.Transport = SummonCharacter.FlyingHeight > 0 ? transportType.AIR : transportType.GROUND;
                //TODO: find SpawnDamage
                c.DeployTime = SummonCharacter.DeployTime;
                c.DeployDelay = SummonCharacter.DeployDelay;
                c.HitSpeed = SummonCharacter.HitSpeed;
                c.MaxRange = SummonCharacter.Range;
                c.SightRange = SummonCharacter.SightRange;
                c.SightClip = SummonCharacter.SightClip;
                c.MultipleTargets = SummonCharacter.MultipleTargets;
                c.CollisionRadius = SummonCharacter.CollisionRadius;
                c.LifeTime = SummonCharacter.LifeTime;
                c.SpawnNumber = SummonCharacter.SpawnNumber;
                c.SpawnPause = SummonCharacter.SpawnPauseTime;
                c.SpawnInterval = SummonCharacter.SpawnInterval;
                if (SummonCharacter.SpawnCharacter.IsValid) c.SpawnCharacter = SummonCharacter.SpawnCharacter.Name.Value;
                if (c.SpawnNumber > 0) c.SpawnCharacterLevel = 1 + SummonCharacter.SpawnCharacterLevelIndex;
                else c.SpawnCharacterLevel = SummonCharacter.SpawnCharacterLevelIndex;
            }
            else if (Projectile.IsValid)
            {
                c.type = boardObjType.PROJECTILE;
                c.TargetType = targetType.ALL;

                if (Projectile.OnlyEnemies > 0)
                {
                    if (Projectile.OnlyOwnTroops > 0) c.affectType = affectType.ALL;
                    else c.affectType = affectType.ONLY_ENEMIES;
                }
                else if (Projectile.OnlyOwnTroops > 0) c.affectType = affectType.ONLY_OWN;

                c.Speed = Projectile.Speed;
                c.DamageRadius = Projectile.Radius;
                c.aoeGround = Projectile.AoeToGround > 0;
                c.aoeAir = Projectile.AoeToAir > 0;
                if (Projectile.SpawnCharacter.IsValid)
                {
                    c.SpawnCharacter = Projectile.SpawnCharacter.Name.Value;
                    c.SpawnCharacterLevel = 1 + Projectile.SpawnCharacterLevelIndex;
                }
                else c.SpawnCharacterLevel = Projectile.SpawnCharacterLevelIndex;
            }
            else if (AreaEffect.IsValid)
            {
                c.type = boardObjType.AOE;
                c.TargetType = AreaEffect.IgnoreBuildings > 0 ? targetType.IGNOREBUILDINGS : targetType.ALL;

                if (AreaEffect.OnlyEnemies > 0)
                {
                    if (AreaEffect.OnlyOwnTroops > 0) c.affectType = affectType.ALL;
                    else c.affectType = affectType.ONLY_ENEMIES;
                }
                else if (AreaEffect.OnlyOwnTroops > 0) c.affectType = affectType.ONLY_OWN;

                c.HitSpeed = AreaEffect.HitSpeed;
                c.DamageRadius = AreaEffect.Radius;
                c.LifeTime = AreaEffect.LifeDuration;
                c.SpawnInterval = AreaEffect.SpawnInterval;
                if (AreaEffect.SpawnCharacter.IsValid)
                {
                    c.SpawnCharacter = AreaEffect.SpawnCharacter.Name.Value;
                    c.SpawnCharacterLevel = 1 + AreaEffect.SpawnCharacterLevelIndex;
                }
                else c.SpawnCharacterLevel = AreaEffect.SpawnCharacterLevelIndex;
            }

            if (needLog)
            {
                StringBuilder sb = new StringBuilder(10000);
                sb.Append(" Extra_Spell_Data:").Append("*************** ");

                sb.Append("CanDeployOnEnemySide:").Append(spell.CanDeployOnEnemySide).Append(" ");
                sb.Append("CustomDeployTime:").Append(spell.CustomDeployTime).Append(" ");
                sb.Append("ManaCostFromSummonerMana:").Append(spell.ManaCostFromSummonerMana).Append(" ");
                sb.Append("SpellAsDeploy:").Append(spell.SpellAsDeploy).Append(" ");
                sb.Append("CanPlaceOnBuildings:").Append(spell.CanPlaceOnBuildings).Append(" ");
                sb.Append("ElixirProductionStopTime:").Append(spell.ElixirProductionStopTime).Append(" ");
                sb.Append("MultipleProjectiles:").Append(spell.MultipleProjectiles).Append(" ");
                sb.Append("Height:").Append(spell.Height).Append(" ");
                sb.Append("Radius:").Append(spell.Radius).Append(" ");
                sb.Append("Pushback:").Append(spell.Pushback).Append(" ");
                sb.Append("StatsUnderInfo:").Append(spell.StatsUnderInfo).Append(" ");
                //sb.Append("Field80:").Append(spell.Field80).Append(" ");
                //sb.Append("Field84:").Append(spell.Field84).Append(" ");
                //sb.Append("Field88:").Append(spell.Field88).Append(" ");
                //sb.Append("Field50:").Append(spell.Field50).Append(" ");

                if (SummonCharacter.IsValid)
                {
                    sb.Append(" sc_SpawnCharacterLevelIndex:").Append(1 + SummonCharacter.SpawnCharacterLevelIndex);
                    sb.Append(" sc_LoadTime:").Append(SummonCharacter.LoadTime);
                    sb.Append(" sc_GrowTime:").Append(SummonCharacter.GrowTime);
                    sb.Append(" sc_DeployTimerDelay:").Append(SummonCharacter.DeployTimerDelay);
                    sb.Append(" sc_DeployTime:").Append(SummonCharacter.DeployTime);
                    sb.Append(" sc_DeployDelay:").Append(SummonCharacter.DeployDelay);
                    sb.Append(" sc_Mass:").Append(SummonCharacter.Mass);
                    sb.Append(" sc_CrownTowerDamagePercent:").Append(SummonCharacter.CrownTowerDamagePercent);
                    sb.Append(" sc_IsSummonerTower:").Append(SummonCharacter.IsSummonerTower);
                    sb.Append(" sc_FlyFromGround:").Append(SummonCharacter.FlyFromGround);
                    sb.Append(" sc_FlyDirectPaths:").Append(SummonCharacter.FlyDirectPaths);
                    sb.Append(" sc_AttacksGround:").Append(SummonCharacter.AttacksGround);
                    sb.Append(" sc_AttacksAir:").Append(SummonCharacter.AttacksAir);
                    sb.Append(" sc_AllTargetsHit:").Append(SummonCharacter.AllTargetsHit);
                    sb.Append(" sc_SpawnProjectile:").Append(SummonCharacter.SpawnProjectile.Name.Value);
                    //sb.Append(" sc_SpawnProjectile:").Append(SummonCharacter.SpawnProjectile.);
                    sb.Append(" sc_Projectile:").Append(SummonCharacter.Projectile.Name.Value);
                    //sb.Append(" sc_Projectile:").Append(SummonCharacter.Projectile.Name.damage);
                    //sb.Append(" sc_DeathSpawnCharacter:").Append(ch isValid SummonCharacter.DeathSpawnCharacter.Name.Value);
                    //sb.Append(" sc_Field10:").Append(SummonCharacter.Field10);
                    //sb.Append(" sc_Field14:").Append(SummonCharacter.Field14);
                    //sb.Append(" sc_Field1C:").Append(SummonCharacter.Field1C);
                }
                else sb.Append("!spell.SummonCharacter.IsValid");

                if (Projectile.IsValid)
                {
                }

                if (AreaEffect.IsValid)
                {
                }

                try
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine("Logs", "_carddb_upd.txt")))
                    {
                        sw.WriteLine(cardToString(c) + sb.ToString());
                    }
                }
                catch { return c; }
            }

            return c;
        }

        public Card collectNewCards(Robi.Clash.Engine.NativeObjects.Logic.GameObjects.Character @char, bool needLog = true)
        {
            //try to fill missing data
            var LogicDataCharacter = @char.LogicGameObjectData;
            if (!LogicDataCharacter.IsValid) return null;

            Card c = new Card();
            c.stringName = LogicDataCharacter.Name.Value;
            c.name = cardNamestringToEnum(c.stringName, "4");

            c.type = boardObjType.MOB; //TODO: divide with Buildings (is not enough data)
            c.Transport = LogicDataCharacter.FlyingHeight > 0 ? transportType.AIR : transportType.GROUND;
            c.TargetType = targetType.NONE;
            if (LogicDataCharacter.TargetOnlyBuildings > 0) c.TargetType = targetType.BUILDINGS;
            else if (LogicDataCharacter.AttacksAir > 0) c.TargetType = targetType.ALL;
            else if (LogicDataCharacter.AttacksGround > 0) c.TargetType = targetType.GROUND;

            c.cost = @char.Mana;
            c.DeployTime = LogicDataCharacter.DeployTime;
            c.DeployDelay = LogicDataCharacter.DeployDelay;
            c.MaxHP = @char.HealthComponent.Health;
            c.Atk = 1;
            c.Shield = @char.HealthComponent.ShieldHealth;
            //c.Speed
            c.HitSpeed = LogicDataCharacter.HitSpeed;
            c.MaxRange = LogicDataCharacter.Range;
            c.SightRange = LogicDataCharacter.SightRange;
            c.SightClip = LogicDataCharacter.SightClip;
            c.MultipleTargets = LogicDataCharacter.MultipleTargets;
            c.MultipleProjectiles = LogicDataCharacter.MultipleProjectiles;
            //c.DeathEffect
            //c.Rarity
            if (@char.HealthComponent.IsValid && @char.HealthComponent.POwner.IsValid) c.Level = 1 + (int)@char.HealthComponent.POwner.TowerLevel;
            c.DamageRadius = LogicDataCharacter.AreaDamageRadius;
            //c.aoeGround
            //c.aoeAir
            c.CollisionRadius = LogicDataCharacter.CollisionRadius;
            c.LifeTime = LogicDataCharacter.LifeTime;
            c.SpawnNumber = LogicDataCharacter.SpawnNumber;
            c.SpawnPause = LogicDataCharacter.SpawnPauseTime;
            c.SpawnInterval = LogicDataCharacter.SpawnInterval;
            if (LogicDataCharacter.SpawnCharacter.IsValid) c.SpawnCharacter = LogicDataCharacter.SpawnCharacter.Name.Value;
            if (c.SpawnNumber > 0) c.SpawnCharacterLevel = 1 + LogicDataCharacter.SpawnCharacterLevelIndex;
            else c.SpawnCharacterLevel = LogicDataCharacter.SpawnCharacterLevelIndex;

            var Projectile = LogicDataCharacter.Projectile;
            if (Projectile.IsValid)
            {
                if (Projectile.OnlyEnemies > 0)
                {
                    if (Projectile.OnlyOwnTroops > 0) c.affectType = affectType.ALL;
                    else c.affectType = affectType.ONLY_ENEMIES;
                }
                else if (Projectile.OnlyOwnTroops > 0) c.affectType = affectType.ONLY_OWN;

                c.Speed = Projectile.Speed;
                c.DamageRadius = Projectile.Radius;
                c.aoeGround = Projectile.AoeToGround > 0;
                c.aoeAir = Projectile.AoeToAir > 0;
                if (c.SpawnCharacter == "" && Projectile.SpawnCharacter.IsValid) c.SpawnCharacter = Projectile.SpawnCharacter.Name.Value;
                c.SpawnCharacterLevel = 1 + Projectile.SpawnCharacterLevelIndex;
            }

            if (needLog)
            {
                StringBuilder sb = new StringBuilder(10000);
                sb.Append("Projectile.Field10:").Append(Projectile.Field10).Append(" ");
                sb.Append("Projectile.Field14:").Append(Projectile.Field14).Append(" ");
                sb.Append("Projectile.Field1C:").Append(Projectile.Field1C).Append(" ");

                try
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine("Logs", "_carddb_upd.txt")))
                    {
                        sw.WriteLine(cardToString(c) + sb.ToString());
                    }
                }
                catch { return c; }
            }

            return c;
        }

        public Card collectNewCards(Robi.Clash.Engine.NativeObjects.Logic.GameObjects.AreaEffectObject aoe, bool needLog = true)
        {
            //try to fill missing data
            var LogicDataAOE = aoe.LogicGameObjectData;
            if (!LogicDataAOE.IsValid) return null;

            Card c = new Card();
            c.stringName = LogicDataAOE.Name.Value;
            c.name = cardNamestringToEnum(c.stringName, "4a");

            c.cost = aoe.Mana;

            c.type = boardObjType.AOE;
            c.TargetType = LogicDataAOE.IgnoreBuildings > 0 ? targetType.IGNOREBUILDINGS : targetType.ALL;

            if (LogicDataAOE.OnlyEnemies > 0)
            {
                if (LogicDataAOE.OnlyOwnTroops > 0) c.affectType = affectType.ALL;
                else c.affectType = affectType.ONLY_ENEMIES;
            }
            else if (LogicDataAOE.OnlyOwnTroops > 0) c.affectType = affectType.ONLY_OWN;

            c.HitSpeed = LogicDataAOE.HitSpeed;
            c.DamageRadius = LogicDataAOE.Radius;
            c.LifeTime = LogicDataAOE.LifeDuration;
            c.SpawnInterval = LogicDataAOE.SpawnInterval;
            c.SpawnNumber = LogicDataAOE.SpawnMaxCount;

            if (LogicDataAOE.SpawnCharacter.IsValid)
            {
                c.SpawnCharacter = LogicDataAOE.SpawnCharacter.Name.Value;
                c.SpawnCharacterLevel = 1 + LogicDataAOE.SpawnCharacterLevelIndex;
            }
            else c.SpawnCharacterLevel = LogicDataAOE.SpawnCharacterLevelIndex;

            if (aoe.HealthComponent.IsValid && aoe.HealthComponent.POwner.IsValid) c.Level = 1 + (int)aoe.HealthComponent.POwner.TowerLevel;

            if (needLog)
            {
                StringBuilder sb = new StringBuilder(10000);
                sb.Append(" Extra_AOE_Data:").Append("*************** ");
                sb.Append("Field18:").Append(aoe.Field18).Append(" ");
                sb.Append("Field18:").Append(aoe.Field1C).Append(" ");
                sb.Append("Field18:").Append(aoe.Field20).Append(" ");
                sb.Append("Field18:").Append(aoe.Field34).Append(" ");
                sb.Append("Field18:").Append(aoe.Field5C).Append(" ");
                sb.Append("Field18:").Append(aoe.Field64).Append(" ");
                sb.Append("Field18:").Append(aoe.Field68).Append(" ");
                sb.Append("Field18:").Append(aoe.FieldC).Append(" ");
                sb.Append("LogicDataAOE.Field10:").Append(LogicDataAOE.Field10).Append(" ");
                sb.Append("LogicDataAOE.Field14:").Append(LogicDataAOE.Field14).Append(" ");
                sb.Append("LogicDataAOE.Field1C:").Append(LogicDataAOE.Field1C).Append(" ");

                try
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine("Logs", "_carddb_upd.txt")))
                    {
                        sw.WriteLine(cardToString(c) + sb.ToString());
                    }
                }
                catch { return c; }
            }

            return c;
        }

        public Card collectNewCards(Robi.Clash.Engine.NativeObjects.Logic.GameObjects.Projectile proj)
        {
            //try to fill missing data
            StringBuilder sb = new StringBuilder(10000);
            Card c = new Card();

            if (!proj.LogicGameObjectData.IsValid) return c;

            sb.Append("Name:").Append(proj.LogicGameObjectData.Name.Value);
            sb.Append(" OwnerIndex:").Append(proj.OwnerIndex);
            sb.Append(" Own:").Append(Robi.Clash.Engine.ClashEngine.Instance.LocalPlayer.OwnerIndex);
            sb.Append(" GlobalId:").Append(proj.GlobalId);
            sb.Append(" bt:").Append(Robi.Clash.Engine.ClashEngine.Instance.Battle.BattleTime);
            if (proj.Parent.IsValid)
            {
                sb.Append(" ParentGId:").Append(proj.Parent.GlobalId);
                if (proj.Parent.LogicGameObjectData.IsValid) sb.Append(" ParentName:").Append(proj.Parent.LogicGameObjectData.Name);
            }
            sb.Append(" Mana:").Append(proj.Mana);
            sb.Append(" StartPosition:").Append(proj.StartPosition);
            if (proj.SpeedComponent.IsValid && proj.SpeedComponent.POwner.IsValid) sb.Append(" TowerLevel:").Append(1 + proj.SpeedComponent.POwner.TowerLevel);

            sb.Append(" ComponentsCount:").Append(proj.ComponentsCount);
            sb.Append(" Field18:").Append(proj.Field18);
            sb.Append(" Field1C:").Append(proj.Field1C);
            sb.Append(" Field20:").Append(proj.Field20);
            sb.Append(" Field34:").Append(proj.Field34);
            sb.Append(" Field5C:").Append(proj.Field5C);
            sb.Append(" Field60:").Append(proj.Field60);
            sb.Append(" Field64:").Append(proj.Field64);
            sb.Append(" Field68:").Append(proj.Field68);
            sb.Append(" Field70:").Append(proj.Field70);
            sb.Append(" Field74:").Append(proj.Field74);
            sb.Append(" Field7C:").Append(proj.Field7C);
            sb.Append(" Field80:").Append(proj.Field80);
            sb.Append(" Field84:").Append(proj.Field84);
            sb.Append(" Field85:").Append(proj.Field85);
            sb.Append(" Field86:").Append(proj.Field86);
            sb.Append(" Field87:").Append(proj.Field87);
            sb.Append(" Field88:").Append(proj.Field88);
            sb.Append(" Field8C:").Append(proj.Field8C);
            sb.Append(" Field90:").Append(proj.Field90);
            sb.Append(" Field94:").Append(proj.Field94);
            sb.Append(" Field98:").Append(proj.Field98);
            sb.Append(" FieldC:").Append(proj.FieldC);

            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine("Logs", "_carddb_upd.txt")))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch { return c; }

            return c;
        }
    }
}