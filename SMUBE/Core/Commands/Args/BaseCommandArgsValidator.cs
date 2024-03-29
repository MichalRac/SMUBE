using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using SMUBE.Units;

namespace SMUBE.Commands.Args
{
    public enum ArgsConstraint
    {
        None = 0,

        Ally = 1,
        Enemy = 2,
        OtherUnit = 3,
        Position = 4,
    }

    public abstract class BaseCommandArgsValidator
    {
        public abstract ArgsConstraint ArgsConstraint { get; }
        public abstract ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel);

        public virtual bool Validate(CommandArgs args, BattleStateModel battleStateModel)
        {
            if (args?.BattleStateModel == null)
            {
                return false;
            }

            if (!ValidateEffectConstraints(battleStateModel.ActiveUnit, args))
            {
                return false;
            }
            
            return true;
        }

        protected static bool ValidateActiveUnit(CommandArgs args, out Unit activeUnit)
        {
            if (args.ActiveUnit == null || args.ActiveUnit.UnitIdentifier == null)
            {
                activeUnit = null;
                return false;
            }
            if (!args.BattleStateModel.TryGetUnit(args.ActiveUnit.UnitIdentifier, out activeUnit))
                return false;
            return true;
        }

        protected static bool ValidateTargetConstraint(ArgsConstraint argsConstraint, UnitIdentifier identifier1, UnitIdentifier identifier2)
        {
            if (argsConstraint != ArgsConstraint.None)
            {
                if(argsConstraint == ArgsConstraint.Ally)
                {
                    return identifier1.TeamId == identifier2.TeamId;
                }
                
                if(argsConstraint == ArgsConstraint.Enemy)
                {
                    return identifier1.TeamId != identifier2.TeamId;
                }

                if (argsConstraint == ArgsConstraint.OtherUnit)
                {
                    return !identifier1.Equals(identifier2);
                }
            }
            return true;
        }

        protected static bool ValidateEffectConstraints(Unit activeUnit, CommandArgs args)
        {
            if (activeUnit.UnitCommandProvider.IsTaunted)
            {
                var viableTargets = activeUnit.UnitCommandProvider.ViableTargets;
                if (viableTargets.Count > 0)
                {
                    var includesTauntTarget = 
                        args.TargetUnits.Any(target => activeUnit.UnitCommandProvider
                            .ViableTargets.Contains(target.UnitIdentifier));

                    if (!includesTauntTarget)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}
