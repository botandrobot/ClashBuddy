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
			assassin, //bandit
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
			clonespell,
			mirror,
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
				Logger.Debug("!!!NEW NAME: {s}", s);
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

			this.cardNameToCardList.Clear();
			Card c = new Card();
			foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.Characters.Entries)
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
				else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
			}


			foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.Buildings.Entries)
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
				else Logger.Error("#####ERR. Duplicate name: {name}", c.name);
			}

			foreach (var e in Robi.Clash.Engine.Csv.CsvLogic.AreaEffectObjects.Entries)
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

				string tmp = "";
				switch (c.stringName)
				{
					case "DartBarrellProjectile": if (cardNameToCardList.ContainsKey(cardName.dartbarrell)) { cardNameToCardList[cardName.dartbarrell].Atk = c.Atk; cardNameToCardList[cardName.dartbarrell].DamageRadius = c.DamageRadius; } break;
					case "BatProjectile": if (cardNameToCardList.ContainsKey(cardName.bat)) { cardNameToCardList[cardName.bat].Atk = c.Atk; cardNameToCardList[cardName.bat].DamageRadius = c.DamageRadius; } break;
					case "MegaMinionSpit": if (cardNameToCardList.ContainsKey(cardName.megaminion)) { cardNameToCardList[cardName.megaminion].Atk = c.Atk; cardNameToCardList[cardName.megaminion].DamageRadius = c.DamageRadius; } break;
					case "MinionSpit": if (cardNameToCardList.ContainsKey(cardName.minion)) { cardNameToCardList[cardName.minion].Atk = c.Atk; cardNameToCardList[cardName.minion].DamageRadius = c.DamageRadius; } break;
					case "BabyDragonProjectile": if (cardNameToCardList.ContainsKey(cardName.babydragon)) { cardNameToCardList[cardName.babydragon].Atk = c.Atk; cardNameToCardList[cardName.babydragon].DamageRadius = c.DamageRadius; } break;
					case "LavaPupProjectile": if (cardNameToCardList.ContainsKey(cardName.lavapups)) { cardNameToCardList[cardName.lavapups].Atk = c.Atk; cardNameToCardList[cardName.lavapups].DamageRadius = c.DamageRadius; } break;
					case "LavaHoundProjectile": if (cardNameToCardList.ContainsKey(cardName.lavahound)) { cardNameToCardList[cardName.lavahound].Atk = c.Atk; cardNameToCardList[cardName.lavahound].DamageRadius = c.DamageRadius; } break;
					case "RoyalGiantProjectile": if (cardNameToCardList.ContainsKey(cardName.royalgiant)) { cardNameToCardList[cardName.royalgiant].Atk = c.Atk; cardNameToCardList[cardName.royalgiant].DamageRadius = c.DamageRadius; } break;
					case "ArcherArrow": if (cardNameToCardList.ContainsKey(cardName.archer)) { cardNameToCardList[cardName.archer].Atk = c.Atk; cardNameToCardList[cardName.archer].DamageRadius = c.DamageRadius; } break;
					case "MusketeerProjectile": if (cardNameToCardList.ContainsKey(cardName.musketeer)) { cardNameToCardList[cardName.musketeer].Atk = c.Atk; cardNameToCardList[cardName.musketeer].DamageRadius = c.DamageRadius; } break;
					case "SpearGoblinProjectile": if (cardNameToCardList.ContainsKey(cardName.speargoblin)) { cardNameToCardList[cardName.speargoblin].Atk = c.Atk; cardNameToCardList[cardName.speargoblin].DamageRadius = c.DamageRadius; } break;
					case "BlowdartGoblinProjectile": if (cardNameToCardList.ContainsKey(cardName.blowdartgoblin)) { cardNameToCardList[cardName.blowdartgoblin].Atk = c.Atk; cardNameToCardList[cardName.blowdartgoblin].DamageRadius = c.DamageRadius; } break;
					case "WitchProjectile": if (cardNameToCardList.ContainsKey(cardName.witch)) { cardNameToCardList[cardName.witch].Atk = c.Atk; cardNameToCardList[cardName.witch].DamageRadius = c.DamageRadius; } break;
					case "TowerPrincessProjectile": if (cardNameToCardList.ContainsKey(cardName.towerprincess)) { cardNameToCardList[cardName.towerprincess].Atk = c.Atk; cardNameToCardList[cardName.towerprincess].DamageRadius = c.DamageRadius; } break;
					case "IceSpiritsProjectile": if (cardNameToCardList.ContainsKey(cardName.icespirits)) { cardNameToCardList[cardName.icespirits].Atk = c.Atk; cardNameToCardList[cardName.icespirits].DamageRadius = c.DamageRadius; } break;
					case "FireSpiritsProjectile": if (cardNameToCardList.ContainsKey(cardName.firespirits)) { cardNameToCardList[cardName.firespirits].Atk = c.Atk; cardNameToCardList[cardName.firespirits].DamageRadius = c.DamageRadius; } break;
					case "PrincessProjectile": if (cardNameToCardList.ContainsKey(cardName.princess)) { cardNameToCardList[cardName.princess].Atk = c.Atk; cardNameToCardList[cardName.princess].DamageRadius = c.DamageRadius; } break;
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
						else Logger.Error("#####ERR. Duplicate name: {name}", c.name); ;
						break;
				}
			}

			Logger.Debug("CardList: {Count}", cardNameToCardList.Count);
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


        public Card collectNewCards(Robi.Clash.Engine.NativeObjects.LogicData.Spell spell)
        {
            //try to fill missing data
            var data = spell.SummonCharacter;
            StringBuilder sb = new StringBuilder(10000);

            Card c = new Card();
            c.stringName = spell.Name.Value;
            c.name = cardNamestringToEnum(c.stringName);
            sb.Append("name:").Append(c.name).Append(" ");
            sb.Append("stringName:").Append(c.stringName).Append(" ");
            sb.Append("type:").Append("-").Append(" ");
            sb.Append("Transport:").Append("-").Append(" ");

            c.TargetType = targetType.NONE;
            if (data.IgnorePushback == 1) c.TargetType = targetType.BUILDINGS;
            else if (data.AttacksAir == 1) c.TargetType = targetType.ALL;
            else if (data.AttacksGround == 1) c.TargetType = targetType.GROUND;
            sb.Append("TargetType:").Append(c.TargetType);
            sb.Append("affectType:").Append(spell.OnlyEnemies).Append("-").Append(spell.OnlyOwnTroops).Append(" ");
            c.cost = spell.ManaCost;
            sb.Append("cost:").Append(c.cost).Append(" ");
            c.DeployTime = data.DeployTime;
            sb.Append("DeployTime:").Append(data.DeployTime).Append(" ");
            c.DeployDelay = data.DeployDelay;
            sb.Append("DeployDelay:").Append(data.DeployDelay).Append(" ");
            c.MaxHP = 100;
            c.Atk = 200;
            sb.Append("MaxHP:").Append("-").Append(" ");
            sb.Append("Atk:").Append("-").Append(" ");
            sb.Append("Shield:").Append("-").Append(" ");
            sb.Append("Speed:").Append("-").Append(" ");
            c.HitSpeed = data.HitSpeed;
            sb.Append("HitSpeed:").Append(data.HitSpeed).Append(" ");
            sb.Append("MinRange:").Append("-").Append(" ");
            c.MaxRange = data.Range;
            sb.Append("MaxRange:").Append(data.Range).Append(" ");
            c.SightRange = data.SightRange;
            sb.Append("SightRange:").Append(data.SightRange).Append(" ");
            c.SightClip = data.SightClip;
            sb.Append("SightClip:").Append(data.SightClip).Append(" ");
            c.MultipleTargets = data.MultipleTargets;
            sb.Append("MultipleTargets:").Append(data.MultipleTargets).Append(" ");
            c.MultipleProjectiles = spell.MultipleProjectiles;
            sb.Append("MultipleProjectiles:").Append(spell.MultipleProjectiles).Append(" ");
            sb.Append("DeathEffect:").Append(data.DeathEffect.Name).Append(" ");
            c.Rarity = spell.Rarity.Name.Value;
            sb.Append("Rarity:").Append(spell.Rarity).Append(" ");
            sb.Append("Level:").Append("-").Append(" ");
            c.DamageRadius = spell.Radius;
            sb.Append("DamageRadius:").Append(spell.Radius).Append(" ");
            //c.aoeGround = spell.Projectile.AoeToGround;
            sb.Append("aoeGround:").Append(spell.Projectile.AoeToGround).Append(" ");
            //c.aoeAir = spell.Projectile.AoeToAir;
            sb.Append("aoeAir:").Append(spell.Projectile.AoeToAir).Append(" ");
            c.CollisionRadius = data.CollisionRadius;
            sb.Append("CollisionRadius:").Append(data.CollisionRadius).Append(" ");
            sb.Append("towerDamage:").Append("-").Append(" ");
            c.LifeTime = data.LifeTime;
            sb.Append("LifeTime:").Append(data.LifeTime).Append(" ");
            c.SpawnNumber = data.SpawnNumber;
            sb.Append("SpawnNumber:").Append(data.SpawnNumber).Append(" ");
            c.SpawnPause = data.SpawnPauseTime;
            sb.Append("SpawnPause:").Append(data.SpawnPauseTime).Append(" ");
            c.SpawnInterval = data.SpawnInterval;
            sb.Append("SpawnInterval:").Append(data.SpawnInterval).Append(" ");
            c.SpawnCharacterLevel = data.SpawnCharacterLevelIndex;
            sb.Append("SpawnCharacterLevel:").Append(data.SpawnCharacterLevelIndex).Append(" ");

            sb.Append("Extra_Spell_Data:").Append("*************** ");

            sb.Append("dataLifeTime:").Append(data.LifeTime).Append(" ");
            sb.Append("dataDeployTime:").Append(data.DeployTime).Append(" ");
            sb.Append("dataFlyingHeight:").Append(data.FlyingHeight).Append(" ");
            sb.Append("CanDeployOnEnemySide:").Append(spell.CanDeployOnEnemySide).Append(" ");
            sb.Append("CustomDeployTime:").Append(spell.CustomDeployTime).Append(" ");
            sb.Append("CanDeployOnEnemySide:").Append(spell.CanDeployOnEnemySide).Append(" ");
            sb.Append("ManaCostFromSummonerMana:").Append(spell.ManaCostFromSummonerMana).Append(" ");
            sb.Append("SpellAsDeploy:").Append(spell.SpellAsDeploy).Append(" ");
            sb.Append("CanDeployOnEnemySide:").Append(spell.CanDeployOnEnemySide).Append(" ");
            sb.Append("CanPlaceOnBuildings:").Append(spell.CanPlaceOnBuildings).Append(" ");
            sb.Append("ElixirProductionStopTime:").Append(spell.ElixirProductionStopTime).Append(" ");
            sb.Append("MultipleProjectiles:").Append(spell.MultipleProjectiles).Append(" ");
            sb.Append("Height:").Append(spell.Height).Append(" ");
            sb.Append("Radius:").Append(spell.Radius).Append(" ");
            sb.Append("Pushback:").Append(spell.Pushback).Append(" ");
            sb.Append("SummonNumber:").Append(spell.SummonNumber).Append(" ");
            sb.Append("StatsUnderInfo:").Append(spell.StatsUnderInfo).Append(" ");
            sb.Append("Field80:").Append(spell.Field80).Append(" ");
            sb.Append("Field84:").Append(spell.Field84).Append(" ");
            sb.Append("Field88:").Append(spell.Field88).Append(" ");
            sb.Append("Field50:").Append(spell.Field50).Append(" ");
            
            try
            {
                using (StreamWriter sw = File.AppendText("_carddb_upd.txt"))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch { return c; }

            return c;
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
			StringBuilder sb = new StringBuilder(1000000);
			foreach (var kvp in cardNameToCardList)
			{
				sb.Clear();
				Card c = kvp.Value;
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
				sb.Append("SpawnNumber:").Append(c.SpawnNumber).Append(" ");
				sb.Append("SpawnPause:").Append(c.SpawnPause).Append(" ");
				sb.Append("SpawnInterval:").Append(c.SpawnInterval).Append(" ");
				sb.Append("SpawnCharacterLevel:").Append(c.SpawnCharacterLevel).Append(" ");

				try
				{
					using (StreamWriter sw = File.AppendText("_carddb_upd.txt"))
					{
						sw.WriteLine(sb.ToString());
					}
				}
				catch { }
			}
		}



		public Card getCardDataFromName(CardDB.cardName cardname, int lvl)
		{
			if (this.cardNameToCardList.ContainsKey(cardname)) return this.cardNameToCardList[cardname];
			else
			{
				Logger.Information("!!!NEW CardName: {cardname}", cardname);
				return this.unknownCard;
			}
		}


	}

}