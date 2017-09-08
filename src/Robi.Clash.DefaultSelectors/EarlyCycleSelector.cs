using System.IO;
using NSwag.SwaggerGeneration.WebApi.Infrastructure;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.RollingFile;

namespace Robi.Clash.DefaultSelectors
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using Engine;
	using Common;
	using Serilog;
	using Robi.Clash.DefaultSelectors.Utilities;

	public class EarlyCycleSelector : ActionSelectorBase
	{
		private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();
		private static readonly LoggerConfiguration LoggerConfiguration = new LoggerConfiguration().Filter.ByIncludingOnly(l => l.Properties.TryGetValue("SourceContext", out LogEventPropertyValue ctx) && ctx.ToString().Trim('"').StartsWith("Buddy.Clash.DefaultSelectors.EarlyCycleSelector", StringComparison.OrdinalIgnoreCase));
		private static readonly ILogEventSink Sink = LoggerConfiguration.WriteTo.Sink(new RollingFileSink($"{ Path.Combine(LogProvider.LogPath, "EarlyActionSelector") }-{{HalfHour}}.log", new MessageTemplateTextFormatter("{Timestamp:HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message:I}{NewLine}{Exception}", null), null, null)).CreateLogger();
		private readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();

		public override string Name => "Early Cycle Selector";

		public override string Description => "This selector implements a simple attack logic that cycles throu cards till it has 3 cards that require more than 3 mana. Then plays two cards at almost the same time.";

		public override string Author => "Token";

		public override Version Version => new Version(1, 0, 0, 0);

		public override Guid Identifier => new Guid("{9996B2EB-8002-4684-BACB-4321FF7D359E}");

		public override void Initialize()
		{
			
			LogProvider.AttachSink(Sink);
		}

		public override void Deinitialize()
		{
			LogProvider.DetachSink(Sink);
		}

		public override CastRequest GetNextCast()
		{
			var om = ClashEngine.Instance.ObjectManager;

			var battle = ClashEngine.Instance.Battle;
			if (battle == null || !battle.IsValid) return null;

			var chars = om.OfType<Engine.NativeObjects.Logic.GameObjects.Character>();

			foreach (var @char in chars)
			{
				var data = @char.LogicGameObjectData;
				if (data != null && data.IsValid)
				{
					var log = new
					{
						OwnerIndex = @char.OwnerIndex,
						Position = @char.StartPosition.ToString(),
					};
					var summonCharacter = data;
					var charLog = new
					{
						GameObject = log,
						Name = summonCharacter.Name.Value.ToString(),
						AttacksAir = summonCharacter.AttacksAir,
						AttacksGround = summonCharacter.AttacksGround,
						CollisionRadius = summonCharacter.CollisionRadius,
						FlyDirectPaths = summonCharacter.FlyDirectPaths,
						FlyFromGround = summonCharacter.FlyFromGround,
						AbilityIsValid = summonCharacter.Ability != null && summonCharacter.Ability.IsValid,
						ActivationTime = summonCharacter.ActivationTime,
						AllTargetsHit = summonCharacter.AllTargetsHit,
						AppearEffectIsValid = summonCharacter.AppearEffect != null && summonCharacter.AppearEffect.IsValid,
						AppearPushback = summonCharacter.AppearPushback,
						AppearPushbackRadius = summonCharacter.AppearPushbackRadius,
						AreaBuffIsValid = summonCharacter.AreaBuff != null && summonCharacter.AreaBuff.IsValid,
						AreaBuffRadius = summonCharacter.AreaBuffRadius,
						AreaDamageRadius = summonCharacter.AreaDamageRadius,
						AreaEffectOnDashIsValid = summonCharacter.AreaEffectOnDash != null && summonCharacter.AreaEffectOnDash.IsValid,
						AreaEffectOnMorphIsValid =
						summonCharacter.AreaEffectOnMorph != null && summonCharacter.AreaEffectOnMorph.IsValid,
						AttachedCharacterHeight = summonCharacter.AttachedCharacterHeight,
						MultipleTargets = summonCharacter.MultipleTargets
					};
					Logger.Debug("{@charLog}", charLog);
				}
			}

			var towerPos = battle.SummonerTowers[0].StartPosition;

			if (_spellQueue.TryDequeue(out string name)) return new CastRequest(name, towerPos);

			var spells = ClashEngine.Instance.AvailableSpells;

			foreach (var spell in spells)
			{
				if (spell != null && spell.IsValid)
				{
					var log = new
					{
						Name = spell.Name.Value.ToString(),
						ManaCost = spell.ManaCost,
						MultipleProjectiles = spell.MultipleProjectiles,
						SummonCharacterLevelIndex = spell.SummonCharacterLevelIndex,
						SummonNumber = spell.SummonNumber,
						SummonCharacterSecondCount = spell.SummonCharacterSecondCount,
						BuffNumber = spell.BuffNumber,
						CanDeployOnEnemySide = spell.CanDeployOnEnemySide,
						CanPlaceOnBuildings = spell.CanPlaceOnBuildings,
						CustomDeployTime = spell.CustomDeployTime,
						OnlyEnemies = spell.OnlyEnemies,
						OnlyOwnTroops = spell.OnlyOwnTroops,
						Height = spell.Height,
						HideRadiusIndicator = spell.HideRadiusIndicator,
						Mirror = spell.Mirror,
						Pushback = spell.Pushback,
						Radius = spell.Radius,
						SpellAsDeploy = spell.SpellAsDeploy,
						ProjectileIsValid = spell.Projectile != null && spell.Projectile.IsValid,
						SummonCharacterIsValid = spell.SummonCharacter != null && spell.SummonCharacter.IsValid,
						AreaEffectIsValid = spell.AreaEffect != null && spell.AreaEffect.IsValid,
						BuffOnDamageIsValid = spell.BuffOnDamage != null && spell.BuffOnDamage.IsValid,
						BuffTypeIsValid = spell.BuffType != null && spell.BuffType.IsValid,
						EffectIsValid = spell.Effect != null && spell.Effect.IsValid,
						StatsUnderInfo = spell.StatsUnderInfo,
					};
					if (spell.SummonCharacter != null && spell.SummonCharacter.IsValid)
					{
						var summonCharacter = spell.SummonCharacter;
						var charLog = new
						{
							Spell = log,
							Name = summonCharacter.Name.Value.ToString(),
							AttacksAir = summonCharacter.AttacksAir,
							AttacksGround = summonCharacter.AttacksGround,
							CollisionRadius = summonCharacter.CollisionRadius,
							FlyDirectPaths = summonCharacter.FlyDirectPaths,
							FlyFromGround = summonCharacter.FlyFromGround,
							AbilityIsValid = summonCharacter.Ability != null && summonCharacter.Ability.IsValid,
							ActivationTime = summonCharacter.ActivationTime,
							AllTargetsHit = summonCharacter.AllTargetsHit,
							AppearEffectIsValid = summonCharacter.AppearEffect != null && summonCharacter.AppearEffect.IsValid,
							AppearPushback = summonCharacter.AppearPushback,
							AppearPushbackRadius = summonCharacter.AppearPushbackRadius,
							AreaBuffIsValid = summonCharacter.AreaBuff != null && summonCharacter.AreaBuff.IsValid,
							AreaBuffRadius = summonCharacter.AreaBuffRadius,
							AreaDamageRadius = summonCharacter.AreaDamageRadius,
							AreaEffectOnDashIsValid = summonCharacter.AreaEffectOnDash != null && summonCharacter.AreaEffectOnDash.IsValid,
							AreaEffectOnMorphIsValid = summonCharacter.AreaEffectOnMorph != null && summonCharacter.AreaEffectOnMorph.IsValid,
							AttachedCharacterHeight = summonCharacter.AttachedCharacterHeight,
							MultipleTargets = summonCharacter.MultipleTargets
						};
						Logger.Debug("{@charLog}", charLog);
					}
					else if (spell.Projectile != null && spell.Projectile.IsValid)
					{
						var projectile = spell.Projectile;
						var projectileLog = new
						{
							Spell = log,
							Name = projectile.Name.Value.ToString(),
							AoeToAir = projectile.AoeToAir,
							AoeToGround = projectile.AoeToGround,
							SpawnCharacterLevelIndex = projectile.SpawnCharacterLevelIndex,
						};
						Logger.Debug("{@projectileLog}", projectileLog);
					}
					else
					{
						Logger.Debug("{@log}", log);
					}
				}
			}

			var cycleSpells = spells.Where(s => s != null && s.IsValid && s.ManaCost <= 3).OrderBy(s => s.ManaCost);
			var powerSpells = spells.Where(s => s != null && s.IsValid && s.ManaCost > 3).OrderByDescending(s => s.ManaCost);

			if (cycleSpells.Count() > 1)
			{
				var spell = cycleSpells.FirstOrDefault();
				return new CastRequest(spell.Name.Value, towerPos);
			}

			if (StaticValues.Player == null || StaticValues.Player.Mana < 9) return null;

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
	}
}
