using System;
using Newtonsoft.Json;
using SMUBE.Commands;
using SMUBE.Commands.Args;

namespace SMUBE.AI.QLearning
{
    [Serializable]
    public class QValueActionPair
    {
        public float QValue { get; set; }

        public readonly ArgsPreferences ArgsPreferences;
        public readonly CommandId CommandId;
        
        [JsonConstructor]
        public QValueActionPair(float qValue, ArgsPreferences argsPreferences, CommandId commandId)
        {
            QValue = qValue;
            ArgsPreferences = argsPreferences;
            CommandId = commandId;
        }
        
        public QValueActionPair(float qValue, BaseCommand baseCommand)
        {
            QValue = qValue;
            CommandId = baseCommand.CommandId;
            ArgsPreferences = baseCommand.ArgsPreferences;
        }
        
        public bool MatchesBaseCommand(BaseCommand command)
        {
            return CommandId == command.CommandId 
                   && ArgsPreferences.TargetingPreference == command.ArgsPreferences.TargetingPreference 
                   && ArgsPreferences.MovementTargetingPreference == command.ArgsPreferences.MovementTargetingPreference 
                   && ArgsPreferences.PositionTargetingPreference == command.ArgsPreferences.PositionTargetingPreference;
        }
    }
}