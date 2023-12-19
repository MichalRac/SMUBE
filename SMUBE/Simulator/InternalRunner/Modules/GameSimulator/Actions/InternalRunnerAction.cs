namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal abstract class InternalRunnerAction
    {
        protected BattleCoreSimulationWrapper Core { get; }

        public InternalRunnerAction(BattleCoreSimulationWrapper core)
        {
            Core = core;
        }

        public abstract void OnPicked();
    }
}
