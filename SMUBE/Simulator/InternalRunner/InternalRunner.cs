using System.Threading.Tasks;
using SMUBE_Utils.Simulator.InternalRunner.Modules;

namespace SMUBE_Utils.Simulator.InternalRunner
{
    internal class InternalRunner
    {
        public async Task Run()
        {
            var modulePicker = new InternalRunnerModulePicker();
            var module = modulePicker.ChooseModule();

            while (module != default)
            {
                await module.Run();
                module = modulePicker.ChooseModule();
            }
        }
    }
}
