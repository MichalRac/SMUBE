namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerSkipForward : InternalRunnerAction
    {
        internal static bool DEBUG_DISPLAY_MAP_AFTER_NEXT_TURN = false; // set to true only when debugging
        
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

                if (DEBUG_DISPLAY_MAP_AFTER_NEXT_TURN)
                {
                    DEBUG_DISPLAY_MAP_AFTER_NEXT_TURN = false;
                    new InternalRunnerDisplayMap(CoreWrapper, true).OnPicked();
                }
                
                gameAutoResolved = CoreWrapper.IsFinished(out _);
            }
        }
    }
}
