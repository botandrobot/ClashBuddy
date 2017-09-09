using System;
using System.Collections.Generic;
using System.Text;

namespace Robi.Clash.DefaultSelectors
{
    public interface IBehavior
    {
	    Cast GetBestCast(Playfield p);
	    float GetPlayfieldValue(Playfield p);
	    int GetBoValue(BoardObj bo, Playfield p);
	    int GetPlayCardPenalty(CardDB.Card card, Playfield p);
    }
}
