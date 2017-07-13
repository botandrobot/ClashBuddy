using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Clash.DefaultSelectors
{
	using System;
	using Engine;

	class RandomActionSelector : IActionSelector
    {
	    public string Name => "Random Selector";

	    public string Description => "Will try to cast a random spell every tick.";

	    public string Author => "Token";

	    public Version Version => new Version(1, 0, 0, 0);
	    public Guid Identifier => new Guid("{41989DD3-6F76-4794-8870-4A1E860A0EF5}");

	    private static readonly Random _generator = new Random();

	    public CastRequest GetNextCast()
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
