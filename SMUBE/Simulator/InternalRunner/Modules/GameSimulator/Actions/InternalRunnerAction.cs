namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal abstract class InternalRunnerAction
    {
        protected BattleCoreSimulationWrapper CoreWrapper { get; }

        protected InternalRunnerAction(BattleCoreSimulationWrapper coreWrapper)
        {
            CoreWrapper = coreWrapper;
        }

        public abstract void OnPicked();
    }
}
