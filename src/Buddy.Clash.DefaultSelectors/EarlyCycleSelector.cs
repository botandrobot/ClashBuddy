namespace Buddy.Clash.DefaultSelectors
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using Engine;
	using Common;
	using Serilog;

	public class EarlyCycleSelector : IActionSelector
	{
		private static readonly ILogger Logger = LogProvider.CreateLogger<EarlyCycleSelector>();
		private readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();

		public string Name => "Early Cycle Selector";

		public string Description => "This selector implements a simple attack logic that cycles throu cards till it has 3 cards that require more than 3 mana. Then plays two cards at almost the same time.";

		public string Author => "Token";

		public Version Version => new Version(1, 0, 0, 0);
		public Guid Identifier => new Guid("{9996B2EB-8002-4684-BACB-4321FF7D359E}");

		public CastRequest GetNextCast()
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
					var charName = data.Name.Value;
					var isFlying = data.FlyFromGround != 0;
					var attacksAir = data.AttacksAir != 0;
					var attacksGround = data.AttacksGround != 0;
					var collisionRadius = data.CollisionRadius;
					Logger.Verbose("Found Character with name {OwnerIndex} {charName} {StartPosition} {collisionRadius} {isFlying} {attacksAir} {attacksGround}",
						@char.OwnerIndex, charName, @char.StartPosition, collisionRadius, isFlying, attacksAir, attacksGround);
				}
			}

			var towerPos = battle.SummonerTowers[0].StartPosition;

			if (_spellQueue.TryDequeue(out string name)) return new CastRequest(name, towerPos);

			var spells = ClashEngine.Instance.AvailableSpells;

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
	}
}
