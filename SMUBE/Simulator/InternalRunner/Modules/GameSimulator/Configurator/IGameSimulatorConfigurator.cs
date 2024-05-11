using System.Collections.Concurrent;
using SMUBE.Units;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal interface IGameSimulatorConfigurator
    {
        ConcurrentBag<Unit> GetUnits(bool useSimpleBehaviour);
    }
}