using System;
using Xunit;

namespace Buddy.Clash.DefaultSelectors.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Helpfunctions.Instance.setnewLoggFile();
            CardDB cdb = CardDB.Instance;
            BoardTester bt = new BoardTester();

            group g = bt.btPlayfield.getGroup(false, 85);
            Handcard hc = KnowledgeBase.Instance.getOppositeCard(bt.btPlayfield, g);

            Behavior behave = new BehaviorControl();//change this to new BehaviorRush() for rush mode

            Cast bc = behave.getBestCast(bt.btPlayfield);

            Assert.Equal(0, bc.Position.X);
            Assert.Equal(0, bc.Position.Y);
        }
    }
}
