using Robi.Common;
using Serilog;

namespace Robi.Clash.DefaultSelectors
{
#if DEBUG
    class AIDebugCommand : Robi.Engine.FrontendCommands.RootCommand
    {
        public static void Register()
        {
            Robi.Engine.Controllers.ActionController.RegisterCommand(new AIDebugCommand());
        }
        public AIDebugCommand()
        {
            Identifier = "AIDebug";
            Name = "AIDebug";
            Order = 0;
            Icon = "bug";
            Color = Robi.Engine.FrontendCommands.CommandColor.DarkCyan;
            Dropdown = null;
        }

        private static ILogger Logger = LogProvider.CreateLogger<AIDebugCommand>();

        /// <inheritdoc />
        public override bool CanExecute { get; } = true;

        /// <inheritdoc />
        public override void Execute()
        {
            Logger.Error("======================== AI LOG MARK ========================");
        }
    }
#endif
}