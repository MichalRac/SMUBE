namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerStepForward : InternalRunnerAction
    {
        public InternalRunnerStepForward(BattleCoreSimulationWrapper coreWrapper) : base(coreWrapper) { }

        public override void OnPicked()
        {
            CoreWrapper.AutoResolveTurn();
        }
    }
}
