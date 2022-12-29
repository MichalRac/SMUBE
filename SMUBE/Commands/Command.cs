using SMUBE.Commands.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public abstract class Command<TArgs, TResults> 
        where TArgs : CommandArgs 
        where TResults : CommandResults
    {
        protected TArgs commandArgs;
        protected TResults commandResults;
        public abstract TResults GetCommandResults();
    }
}
