namespace Robi.Clash.DefaultSelectors
{
    public abstract class Behavior
    {
        public virtual Cast getBestCast(Playfield p)
        {
            return null;
        }

        public virtual float getPlayfieldValue(Playfield p)
        {
            return 0;
        }

        public virtual int getBoValue(BoardObj bo, Playfield p)
        {
            return 0;
        }

        public virtual string BehaviorName()
        {
            return "None";
        }

        public virtual int getPlayCardPenality(CardDB.Card card, Playfield p)
        {
            return 0;
        }
    }
}