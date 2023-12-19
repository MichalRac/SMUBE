namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerSkipForward : InternalRunnerAction
    {
        public InternalRunnerSkipForward(BattleCoreSimulationWrapper core) : base(core)
        {
        }

        public override void OnPicked()
        {
            var gameAutoResolved = false;
            while (!gameAutoResolved)
            {
                Core.AutoResolveTurn();
                gameAutoResolved = Core.IsFinished(out _);
                if (!gameAutoResolved)
                {
                    Core.LogTurnInfo();
                }
            }
        }
    }
}
