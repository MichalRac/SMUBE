using System;
using System.Collections.Generic;
using SMUBE_Utils.Simulator.Utils.MapPrinter;
using SMUBE.Units.CharacterTypes;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerDisplayMap : InternalRunnerAction
    {
        private readonly bool _withDescriptors;

        public InternalRunnerDisplayMap(BattleCoreSimulationWrapper coreWrapper, bool withDescriptors) 
            : base(coreWrapper)
        {
            _withDescriptors = withDescriptors;
        }

        public override void OnPicked()
        {
            var activeUnitCoords = CoreWrapper.Core.currentStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates;
            var gridMapGenericDisplayData = new List<GridMapGenericDisplayData>()
            {
                new GridMapGenericDisplayData()
                {
                    Coordinates = activeUnitCoords,
                    Label = "(active)",
                    Color = ConsoleColor.Yellow
                }
            };

            if (_withDescriptors)
            {
                foreach (var unit in CoreWrapper.Core.currentStateModel.Units)
                {
                    gridMapGenericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Coordinates = unit.UnitData.BattleScenePosition.Coordinates,
                        Label = $"{Enum.GetName(typeof(BaseCharacterType), unit.UnitData.UnitStats.BaseCharacter.BaseCharacterType)}",
                        Color = ConsoleColor.Gray
                    });

                    
                    gridMapGenericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Coordinates = unit.UnitData.BattleScenePosition.Coordinates,
                        Label = $"HP:{unit.UnitData.UnitStats.CurrentHealth}/{unit.UnitData.UnitStats.MaxHealth}",
                        Color = ConsoleColor.Gray
                    });
                    
                    gridMapGenericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Coordinates = unit.UnitData.BattleScenePosition.Coordinates,
                        Label = $"MP:{unit.UnitData.UnitStats.CurrentMana}/{unit.UnitData.UnitStats.MaxMana}",
                        Color = ConsoleColor.Gray
                    });
                    
                    gridMapGenericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Coordinates = unit.UnitData.BattleScenePosition.Coordinates,
                        Label = $"SP:{unit.UnitData.UnitStats.CurrentStamina}/{unit.UnitData.UnitStats.MaxStamina}",
                        Color = ConsoleColor.Gray
                    });
                    
                    foreach (var appliedEffect in unit.UnitData.UnitStats.PersistentEffects)
                    {
                        gridMapGenericDisplayData.Add(new GridMapGenericDisplayData()
                        {
                            Coordinates = unit.UnitData.BattleScenePosition.Coordinates,
                            Label = $"E: {appliedEffect.GetDescriptor()}:t{appliedEffect.GetPersistence()}",
                            Color = ConsoleColor.DarkGray
                        });
                    }
                }
            }
            GridMapPrinter.DefaultGridPrinter(CoreWrapper.Core.currentStateModel.BattleSceneState, gridMapGenericDisplayData).PrintMap();
        }
    }
}