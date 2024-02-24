using System;
using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToEveryArgsPicker : ArgsPicker
    {
        public OneToEveryArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
        {
        }

        public override void Submit()
        {
            var args = GetCommandArgs();
            IsResolved = true;
            ArgsConfirmed?.Invoke(args);
        }

        public override void Return()
        {
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
        }

        public override bool IsAnyValid()
        {
            return GetAllValidTargets().Count > 0;
        }

        public override CommandArgs GetCommandArgs()
        {
            var state = BattleStateModel;
            var args = new CommonArgs(state.ActiveUnit.UnitData, GetAllValidTargets(), state);
            return args;
        }

        private List<UnitData> GetAllValidTargets()
        {
            var result = new List<UnitData>();
            var constraint = Command.CommandArgsValidator.ArgsConstraint;
            var activeUnitTeamId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;

            foreach (var unit in BattleStateModel.Units)
            {
                switch (constraint)
                {
                    case ArgsConstraint.None:
                        result.Add(unit.UnitData);
                        break;
                    case ArgsConstraint.Ally:
                        if (activeUnitTeamId == unit.UnitData.UnitIdentifier.TeamId)
                        {
                            result.Add(unit.UnitData);
                        }
                        break;
                    case ArgsConstraint.Enemy:
                        if (activeUnitTeamId != unit.UnitData.UnitIdentifier.TeamId)
                        {
                            result.Add(unit.UnitData);
                        }
                        break;
                    case ArgsConstraint.Position:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            return result;
        }

        public override void Up()
        {
        }

        public override void Down()
        {
        }

        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override string GetPickerInfo()
        {
            return "Confirm or cancel command on all targets";
        }

        public override string GetPickerState()
        {
            return "Confirm or cancel command on all targets";
        }

    }
}