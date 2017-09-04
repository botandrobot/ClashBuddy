namespace Buddy.Clash.DefaultSelectors
{
	using System;
	using Engine;
    // Making sure that we have a change for a demo :)
	public class SequenceActionSelector : ActionSelectorBase
	{
	    public override string Name => "Sequence Selector";

	    public override string Description => "Will try to cast a spell in sequence every tick.";

	    public override string Author => "Token";

	    public override Version Version => new Version(1, 0, 0, 0);
	    public override Guid Identifier => new Guid("{7110C08C-283B-4D2F-AF26-982DD2246024}");

	    private int _currentSpell = 0;

	    public override CastRequest GetNextCast()
	    {
		    var battle = ClashEngine.Instance.Battle;
		    if (battle == null || !battle.IsValid) return null;

		    var spells = ClashEngine.Instance.AvailableSpells;
		    if (_currentSpell >= 4 || _currentSpell < 0)
			    _currentSpell = 0;
			
			var spell = spells[_currentSpell++];
			if (spell == null || !spell.IsValid) return null;
			return new CastRequest(spell.Name.Value, battle.SummonerTowers[0].StartPosition);
	    }
    }
}
