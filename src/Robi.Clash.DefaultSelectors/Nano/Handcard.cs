namespace Robi.Clash.DefaultSelectors
{
    public class Handcard
    {
        public int position = 0;
        public int lvl = 1;
        public CardDB.Card card;
        public int manacost = 0;
        public string name = "";
        public int val = int.MinValue;

        public Handcard()
        {
            card = CardDB.Instance.unknownCard;
        }

        public Handcard(Handcard hc)
        {
            this.position = hc.position;
            this.lvl = hc.lvl;
            this.card = hc.card;
            this.manacost = hc.manacost;
            this.name = hc.name;
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
            this.card = CardDB.Instance.getCardDataFromName(CardDB.Instance.cardNamestringToEnum(name), lvl);
            this.name = name;
            //this.manacost = c.manacost;
        }
        /* public bool canplayCard(Playfield p, bool own)
         {
             return this.card.canplayCard(p, this.manacost, own);
         }*/

    }

}