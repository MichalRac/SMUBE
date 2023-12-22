namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerSkipForward : InternalRunnerAction
    {
        private readonly bool _log;

        public InternalRunnerSkipForward(BattleCoreSimulationWrapper coreWrapper, bool log = false) : base(coreWrapper)
        {
            _log = log;
        }

        public override void OnPicked()
        {
            var gameAutoResolved = false;
            while (!gameAutoResolved)
            {
                if (_log)
                {
                    CoreWrapper.LogTurnInfo();
                }
                
                CoreWrapper.AutoResolveTurn(_log);
                gameAutoResolved = CoreWrapper.IsFinished(out _);
            }
        }
    }
}
