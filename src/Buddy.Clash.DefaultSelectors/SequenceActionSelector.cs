namespace Buddy.Clash.DefaultSelectors
{
	using System;
	using Engine;

	public class SequenceActionSelector : IActionSelector
    {
	    public string Name => "Sequence Selector";

	    public string Description => "Will try to cast a spell in sequence every tick.";

	    public string Author => "Token";

	    public Version Version => new Version(1, 0, 0, 0);
	    public Guid Identifier => new Guid("{7110C08C-283B-4D2F-AF26-982DD2246024}");

	    private int currentSpell = 0;

	    public CastRequest GetNextCast()
	    {
		    var battle = ClashEngine.Instance.Battle;
		    if (battle == null || !battle.IsValid) return null;

		    var spells = ClashEngine.Instance.AvailableSpells;
		    if (currentSpell >= 4 || currentSpell < 0)
			    currentSpell = 0;
			
			var spell = spells[currentSpell++];
			if (spell == null || !spell.IsValid) return null;
			return new CastRequest(spell.Name.Value, battle.SummonerTowers[0].StartPosition);
	    }
    }
}
