using SMUBE.Units;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal interface IGameSimulatorConfigurator
    {
        List<Unit> GetUnits(bool useSimpleBehaviour);
    }
}