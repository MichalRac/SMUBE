namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerStepForward : InternalRunnerAction
    {
        public InternalRunnerStepForward(BattleCoreSimulationWrapper core) : base(core) { }

        public override void OnPicked()
        {
            Core.AutoResolveTurn();
        }
    }
}
