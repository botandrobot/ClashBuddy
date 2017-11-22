namespace Robi.Clash.DefaultSelectors.Behaviors
{
	using Common;
	using Serilog;
	using System;
	using System.Collections.Generic;

	public class LogRecorder : BehaviorBase
	{
		private static readonly ILogger Logger = LogProvider.CreateLogger<Test>();

		public override string Name { get; } = "LogRecorder";
		public override string Description { get; } = "Only records the battlefield.";
		public override string Author { get; } = "LogRecorder";
		public override Version Version { get; } = new Version(1, 0, 0, 0);
		public override Guid Identifier { get; } = new Guid("{16AA8911-90CE-4701-AA8A-ADBE8B1B6E62}");

        public override Cast GetBestCast(Playfield p)
		{
			return null;
		}

		public override float GetPlayfieldValue(Playfield p)
		{
			return 0;
		}

		public override int GetBoValue(BoardObj bo, Playfield p)
		{
			return 0;
		}

		public override int GetPlayCardPenalty(CardDB.Card card, Playfield p)
		{
			return 0;
		}
	}
}