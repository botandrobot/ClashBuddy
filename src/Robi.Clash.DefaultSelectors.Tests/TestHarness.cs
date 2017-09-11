namespace Robi.Clash.DefaultSelectors.Tests
{
	using Behaviors;
	using Xunit;

	public class TestHarness
    {
        [Fact]
        public void ControlTest()
        {
            CardDB.Initialize();
            BoardTester bt = new BoardTester();
            Test behave = new Test();

            Cast bc = behave.GetBestCast(bt.btPlayfield);
            bt.btPlayfield.print();

            //Assert.Equal(0, bc.Position.X);
            //Assert.Equal(0, bc.Position.Y);
        }

	    [Fact]
	    public void ApolloTest()
	    {
		    CardDB.Initialize();
		    BoardTester bt = new BoardTester();
		    Apollo behave = new Apollo();

		    Cast bc = behave.GetBestCast(bt.btPlayfield);
            bt.btPlayfield.print();

            //Assert.Equal(0, bc.Position.X);
		    //Assert.Equal(0, bc.Position.Y);
		}
    }
}
