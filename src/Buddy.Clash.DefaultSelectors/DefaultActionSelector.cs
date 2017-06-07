namespace Buddy.Clash.DefaultSelectors
{
	using System;
	using Engine;

	public class DefaultActionSelector : IActionSelector
	{
		public string Name => "Default Action Selector";

		public string Description => "Plays the first spell of the spell buttons.";

		public string Author => "Token";

		public Version Version => new Version(1, 0, 0, 0);
		public Guid Identifier => new Guid("{53FB4573-C7F7-43CA-9052-C05E88925795}");

		public CastRequest GetNextCast()
		{
			var battle = ClashEngine.Instance.Battle;
			if (battle == null || !battle.IsValid) return null;

			var spells = ClashEngine.Instance.AvailableSpells;
			for (var spellIndex = 0; spellIndex < 4; spellIndex++)
			{
				var spell = spells[spellIndex];
				if (spell == null || !spell.IsValid) continue;
				return new CastRequest(spell.Name.Value, battle.SummonerTowers[0].StartPosition);
			}
			return null;
		}
	}
}