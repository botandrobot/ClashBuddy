using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors
{
	using System;
	using Engine;

	class RandomActionSelector : ActionSelectorBase
	{
	    public override string Name => "Random Selector";

	    public override string Description => "Will try to cast a random spell every tick.";

	    public override string Author => "Token";

	    public override Version Version => new Version(1, 0, 0, 0);
	    public override Guid Identifier => new Guid("{41989DD3-6F76-4794-8870-4A1E860A0EF5}");

	    private static readonly Random _generator = new Random();

	    public override CastRequest GetNextCast()
	    {
		    var battle = ClashEngine.Instance.Battle;
		    if (battle == null || !battle.IsValid) return null;

		    var spells = ClashEngine.Instance.AvailableSpells;

		    var spell = spells[_generator.Next(4)];
		    if (spell == null || !spell.IsValid) return null;
		    return new CastRequest(spell.Name.Value, battle.SummonerTowers[0].StartPosition);
	    }
    }
}
