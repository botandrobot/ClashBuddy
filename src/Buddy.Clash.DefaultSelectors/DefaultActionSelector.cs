namespace Buddy.Clash.DefaultSelectors
{
	using System;
	using Engine;

    // Test

	public class DefaultActionSelector : ActionSelectorBase
	{
		public override string Name => "Default Action Selector";

		public override string Description => "Plays the first spell of the spell buttons.";

		public override string Author => "Token";

		public override Version Version => new Version(1, 0, 0, 0);
		public override Guid Identifier => new Guid("{53FB4573-C7F7-43CA-9052-C05E88925795}");

		public override CastRequest GetNextCast()
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