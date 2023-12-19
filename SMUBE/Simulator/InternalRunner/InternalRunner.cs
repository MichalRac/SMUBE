using SMUBE_Utils.Simulator.InternalRunner.Modules;

namespace SMUBE_Utils.Simulator.InternalRunner
{
    internal class InternalRunner
    {
        public void Run()
        {
            var modulePicker = new InternalRunnerModulePicker();
            var module = modulePicker.ChooseModule();

            while (module != default)
            {
                module.Run();
                module = modulePicker.ChooseModule();
            }
        }
    }
}
