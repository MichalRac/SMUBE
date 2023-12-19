using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.ActionContexts
{
    internal abstract class InternalRunnerActionContext
    {
        public abstract string ContextDescription { get; }
        public abstract List<InternalRunnerAction> ContextActions { get; }
    }
}
