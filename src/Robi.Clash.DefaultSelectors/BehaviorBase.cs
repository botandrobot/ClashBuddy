

using System.IO;
using Serilog.Core;
using Serilog.Events;

namespace Robi.Clash.DefaultSelectors
{
	using Common;
	using Engine;
	using Robi.Engine;
	using Robi.Engine.Settings;
	using Serilog;
	using Settings;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public abstract class BehaviorBase : ActionSelectorBase, IBehavior
	{
		private static ILogger Logger { get; } = LogProvider.CreateLogger<BehaviorBase>();

		public abstract Cast GetBestCast(Playfield p);
		public abstract float GetPlayfieldValue(Playfield p);
		public abstract int GetBoValue(BoardObj bo, Playfield p);
		public abstract int GetPlayCardPenalty(CardDB.Card card, Playfield p);

		private static BehaviorBaseSettings Settings { get; } = new BehaviorBaseSettings();

		private ILogEventSink _battleLogger;
		public override void BattleStart()
		{
			base.BattleStart();
			var battleLogName = Path.Combine(LogProvider.LogPath, "BattleLog", $"battle-{DateTime.Now:yyyyMMddHHmmss}.log");
			Logger.Information($"BattleLog: {battleLogName}");
			_battleLogger = new LoggerConfiguration()
				.Filter.ByExcluding(e =>
				{
					if (!e.Properties.ContainsKey("SourceContext")) return true;
					var ctx = ((Serilog.Events.ScalarValue)e.Properties["SourceContext"]).Value as string;
					if (string.IsNullOrWhiteSpace(ctx)) return true;
					return !(ctx.StartsWith("Robi.Clash.DefaultSelectors") || ctx.StartsWith("Robi.Engine.PerformanceTimer"));
				}).WriteTo.File(battleLogName,  outputTemplate: "{Message}{NewLine}{Exception}").CreateLogger();
			LogProvider.AttachSink(_battleLogger);
		}

		public override void BattleEnd()
		{
			LogProvider.DetachSink(_battleLogger);
			((Logger)_battleLogger).Dispose();
			_battleLogger = null;
			base.BattleEnd();
		}

		public override void Initialize()
		{
			SettingsManager.RegisterSettings("Base Behavior", Settings);
			CardDB.Initialize();
		}

		public override void Deinitialize()
		{
			SettingsManager.UnregisterSettings("Base Behavior");
			CardDB.Instance.uploadCardInfo();
		}

		public sealed override CastRequest GetNextCast()
		{
			List<BoardObj> ownMinions = new List<BoardObj>();
			List<BoardObj> enemyMinions = new List<BoardObj>();

			List<BoardObj> ownAreaEffects = new List<BoardObj>();
			List<BoardObj> enemyAreaEffects = new List<BoardObj>();

			List<BoardObj> ownBuildings = new List<BoardObj>();
			List<BoardObj> enemyBuildings = new List<BoardObj>();

			BoardObj ownKingsTower = new BoardObj();
			BoardObj ownPrincessTower1 = new BoardObj();
			BoardObj ownPrincessTower2 = new BoardObj();
			BoardObj enemyKingsTower = new BoardObj();
			BoardObj enemyPrincessTower1 = new BoardObj();
			BoardObj enemyPrincessTower2 = new BoardObj();

			List<Handcard> ownHandCards = new List<Handcard>();

			var battle = ClashEngine.Instance.Battle;
			if (battle == null || !battle.IsValid) return null;
			var om = ClashEngine.Instance.ObjectManager;
			if (om == null) return null;
			var lp = ClashEngine.Instance.LocalPlayer;
			if (lp == null || !lp.IsValid) return null;
			var spells = ClashEngine.Instance.AvailableSpells;
			if (spells == null) return null;

			StringBuilder sb = new StringBuilder();

			using (new PerformanceTimer("GetNextCast entrance"))
			{
				foreach (var spell in spells)
				{
					if (spell != null && spell.IsValid)
					{
						int lvl = 1;
						Handcard hc = new Handcard(spell.Name.Value, lvl); //hc.lvl = ??? TODO
						hc.manacost = spell.ManaCost;
                        if (hc.card.name == CardDB.cardName.unknown) hc.card = CardDB.Instance.collectNewCards(spell);
						//hc.position = ??? TODO
						ownHandCards.Add(hc);
					}
				}

				var aoes = om.OfType<Clash.Engine.NativeObjects.Logic.GameObjects.AreaEffectObject>();
				foreach (var aoe in aoes)
				{
					if (aoe != null && aoe.IsValid)
					{
						//TODO: get static data for all objects
						//Here we get dynamic data only
						BoardObj bo = new BoardObj(CardDB.Instance.cardNamestringToEnum(aoe.LogicGameObjectData.Name.Value));
						bo.GId = aoe.GlobalId;
						bo.Position = new VectorAI(aoe.StartPosition);
						bo.Line = bo.Position.X > 8700 ? 2 : 1;
						//bo.level = TODO real value
						//bo.Atk = TODO real value
						bo.LifeTime = aoe.HealthComponent.RemainingTime; //TODO check this value

						bo.ownerIndex = (int)aoe.OwnerIndex;
						bool own = bo.ownerIndex == lp.OwnerIndex ? true : false; //TODO: replace it on Friendly (for 2x2 mode)
						bo.own = own;
						if (own) ownAreaEffects.Add(bo);
						else enemyAreaEffects.Add(bo);
                        //hc.position = ??? TODO

                        //if (hc.card.name == CardDB.cardName.unknown) hc.card = CardDB.Instance.collectNewCards(spell); //TODO: same for all objects

                    }

                }

				var chars = om.OfType<Clash.Engine.NativeObjects.Logic.GameObjects.Character>();
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
						bo.Line = bo.Position.X > 8700 ? 2 : 1;
						//bo.level = TODO real value
						//bo.Atk = TODO real value
						//this.frozen = TODO
						//this.startFrozen = TODO
						bo.HP = @char.HealthComponent.CurrentHealth; //TODO: check it
						bo.Shield = @char.HealthComponent.CurrentShieldHealth; //TODO: check it
						bo.LifeTime =
							@char.HealthComponent.LifeTime -
							@char.HealthComponent.RemainingTime; //TODO: check it of data.LifeTime, - find real value for battle stage

						bo.ownerIndex = (int)@char.OwnerIndex;
						bool own = bo.ownerIndex == lp.OwnerIndex ? true : false; //TODO: replace it on Friendly (for 2x2 mode)

                        bo.own = own;
                        int tower = 0;
						switch (bo.Name)
						{
							case CardDB.cardName.princesstower:
								tower = bo.Line;
								if (bo.own)
								{
									if (tower == 1) ownPrincessTower1 = bo;
									else ownPrincessTower2 = bo;
								}
								else
								{
									if (tower == 1) enemyPrincessTower1 = bo;
									else enemyPrincessTower2 = bo;
								}
								break;
							case CardDB.cardName.kingtower:
								tower = 10 + bo.Line;
								if (bo.own)
								{
									if (lp.OwnerIndex == bo.ownerIndex) ownKingsTower = bo;
								}
								else enemyKingsTower = bo;
								break;
							case CardDB.cardName.kingtowermiddle:
								tower = 100;
								break;
                            default:
                                if (own)
                                {
                                    switch (bo.type)
                                    {
                                        case boardObjType.MOB:
                                            ownMinions.Add(bo);
                                            break;
                                        case boardObjType.BUILDING:
                                            ownBuildings.Add(bo);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (bo.type)
                                    {
                                        case boardObjType.MOB:
                                            enemyMinions.Add(bo);
                                            break;
                                        case boardObjType.BUILDING:
                                            enemyBuildings.Add(bo);
                                            break;
                                    }
                                }
                                break;
                        }
					}
                    //if (hc.card.name == CardDB.cardName.unknown) hc.card = CardDB.Instance.collectNewCards(spell); //TODO: same for all objects

                }
            }

			Playfield p;

			using (new PerformanceTimer("Initialize playfield."))
			{
				p = new Playfield
				{
					BattleTime = ClashEngine.Instance.Battle.BattleTime,
					ownerIndex = (int)lp.OwnerIndex,
					ownMana = (int)lp.Mana,
					ownHandCards = ownHandCards,
					ownAreaEffects = ownAreaEffects,
					ownMinions = ownMinions,
					ownBuildings = ownBuildings,
					ownKingsTower = ownKingsTower,
					ownPrincessTower1 = ownPrincessTower1,
					ownPrincessTower2 = ownPrincessTower2,
					enemyAreaEffects = enemyAreaEffects,
					enemyMinions = enemyMinions,
					enemyBuildings = enemyBuildings,
					enemyKingsTower = enemyKingsTower,
					enemyPrincessTower1 = enemyPrincessTower1,
					enemyPrincessTower2 = enemyPrincessTower2,
					//TODO: Add next card
					//nextCard =
				};

				p.home = p.ownKingsTower.Position.Y < 15250 ? true : false;

                if (p.ownPrincessTower1.Position == null) p.ownPrincessTower1.Position = p.getDeployPosition(deployDirection.ownPrincessTowerLine1);
                if (p.ownPrincessTower2.Position == null) p.ownPrincessTower2.Position = p.getDeployPosition(deployDirection.ownPrincessTowerLine2);
                if (p.enemyPrincessTower1.Position == null) p.enemyPrincessTower1.Position = p.getDeployPosition(deployDirection.enemyPrincessTowerLine1);
                if (p.enemyPrincessTower2.Position == null) p.enemyPrincessTower2.Position = p.getDeployPosition(deployDirection.enemyPrincessTowerLine2);

                p.initTowers();

				int i = 0;
				foreach (BoardObj t in p.ownTowers) if (t.Tower < 10) i += t.Line;
				int kingsLine = 0;
				switch (i)
				{
					case 0:
						kingsLine = 3;
						break;
					case 1:
						kingsLine = 2;
						break;
					case 2:
						kingsLine = 1;
						break;
				}
				foreach (BoardObj t in p.ownTowers) if (t.Tower > 9) t.Line = kingsLine;

				p.print();
			}

			Cast bc;
			using (new PerformanceTimer("GetBestCast"))
			{
				bc = this.GetBestCast(p);

				CastRequest retval = null;
				if (bc != null && bc.Position != null)
				{
					Logger.Information("Cast {bc}", bc.ToString());
					retval = new CastRequest(bc.SpellName, bc.Position.ToVector2());
				}
				else Logger.Information("Waiting for cast, maybe next tick...");

				return retval;
			}
		}
    }
}