namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerLogUnitSummary : InternalRunnerAction
    {
        public InternalRunnerLogUnitSummary(BattleCoreSimulationWrapper coreWrapper) : base(coreWrapper)
        {
        }

        public override void OnPicked()
        {
            CoreWrapper.LogUnitSummary();
        }
    }
}