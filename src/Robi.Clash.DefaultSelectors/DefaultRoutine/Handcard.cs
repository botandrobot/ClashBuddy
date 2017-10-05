namespace Robi.Clash.DefaultSelectors
{
    public class Handcard
    {
        public int position = 0;
        public int lvl = 1;
        public CardDB.Card card;
        public int manacost = 0;
        public string name = "";
        public bool mirror = false;
        public int missingMana = 100;
        public int val = int.MinValue;
        public double extraVal = 0;

        public Handcard()
        {
            card = CardDB.Instance.unknownCard;
        }

        public Handcard(CardDB.Card c)
        {
            this.position = 0;
            this.lvl = 1;
            this.card = c;
            //this.name =
            //this.manacost = c.manacost;
        }

        public Handcard(string name, int level)
        {
            this.position = 0;
            this.lvl = level;
            this.card = CardDB.Instance.getCardDataFromName(CardDB.Instance.cardNamestringToEnum(name, "5"), lvl);
            this.name = name;
            //this.manacost = c.manacost;
        }

        public Handcard(Handcard hc)
        {
            this.position = hc.position;
            this.lvl = hc.lvl;
            this.card = hc.card;
            this.manacost = hc.manacost;
            this.name = hc.name;
            this.mirror = hc.mirror;
        }

        public void transformTo(Handcard hc)
        {
            this.position = hc.position;
            this.lvl = hc.lvl;
            this.card = hc.card;
            this.manacost = hc.manacost;
            this.name = hc.name;
            this.mirror = hc.mirror;
        }
    }
}