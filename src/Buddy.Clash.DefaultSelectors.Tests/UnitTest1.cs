using System;
using Xunit;

namespace Robi.Clash.DefaultSelectors.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Helpfunctions.Instance.setnewLoggFile();
            CardDB cdb = CardDB.Instance;
            BoardTester bt = new BoardTester();
            Behavior behave = new BehaviorControl();//change this to new BehaviorRush() for rush mode

            Cast bc = behave.getBestCast(bt.btPlayfield);
            
        }
    }
}
