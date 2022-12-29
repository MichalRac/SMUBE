using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Core;
using SMUBE.DataStructures;
using SMUBE.DataStructures.Units;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;

namespace SMUBE_Utils.UnitTests
{
    [TestClass]
    public class BattleStateModelUnitTests
    {

        private BattleStateModel battleStateModel;

        public BattleStateModelUnitTests()
        {
            battleStateModel = new BattleStateModel();
        }

        [TestMethod]
        public void AddUnitToBattleState()
        {
            // arrange
            var newUnit = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());


            // act
            var successResult = battleStateModel.TryAddUnit(newUnit);
            

            // assert
            Assert.IsTrue(battleStateModel.Units.Contains(newUnit));
            Assert.IsTrue(successResult);
        }

        [TestMethod]
        public void CannotAddSameUnitTwice()
        {
            var newUnit = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(newUnit);
            var successResult = battleStateModel.TryAddUnit(newUnit);


            Assert.IsFalse(successResult);
        }

        [TestMethod]
        public void CanAddMultipleUnits()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());


            var successResult1 = battleStateModel.TryAddUnit(unit1);
            var successResult2 = battleStateModel.TryAddUnit(unit2);


            Assert.IsTrue(successResult1);
            Assert.IsTrue(successResult2);
            Assert.IsTrue(battleStateModel.Units.Contains(unit1) && battleStateModel.Units.Contains(unit2));
        }

        [TestMethod]
        public void CanRemoveAddedUnit() 
        {
            var addedUnit = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var notAddedUnit = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());

            battleStateModel.TryAddUnit(addedUnit);
            var successResult = battleStateModel.TryRemoveUnit(notAddedUnit);


            Assert.IsFalse(successResult);
            Assert.IsTrue(battleStateModel.Units.Count == 1);
            Assert.IsTrue(battleStateModel.Units.Contains(addedUnit));
            Assert.IsFalse(battleStateModel.Units.Contains(notAddedUnit));
        }

        [TestMethod]
        public void CannotRemoveNotAddedUnit()
        {
            var newUnit = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(newUnit);
            var successResult = battleStateModel.TryRemoveUnit(newUnit);


            Assert.IsTrue(successResult);
            Assert.IsFalse(battleStateModel.Units.Contains(newUnit));

        }

        [TestMethod]
        public void CanRemoveSpecificUnit()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(unit1);
            battleStateModel.TryAddUnit(unit2);
            var successResult = battleStateModel.TryRemoveUnit(unit1);


            Assert.IsTrue(successResult);
            Assert.IsTrue(!battleStateModel.Units.Contains(unit1) && battleStateModel.Units.Contains(unit2));
        }

        [TestMethod]
        public void GetUnits()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());
            var unit3 = new Unit(1, 0, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(unit1);
            battleStateModel.TryAddUnit(unit2);
            battleStateModel.TryRemoveUnit(unit2);
            battleStateModel.TryAddUnit(unit3);
            var units = battleStateModel.Units;


            Assert.AreEqual(unit1, units[0]);
            Assert.AreNotEqual(unit2, units[1]);
            Assert.AreEqual(unit3, units[1]);
            Assert.IsTrue(battleStateModel.Units.Contains(unit1));
            Assert.IsFalse(battleStateModel.Units.Contains(unit2));
            Assert.IsTrue(battleStateModel.Units.Contains(unit3));
        }

        [TestMethod]
        public void GetUnitsForTeam()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(1, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit3 = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());
            var unit4 = new Unit(1, 1, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(unit1);
            battleStateModel.TryAddUnit(unit2);
            battleStateModel.TryAddUnit(unit3);
            battleStateModel.TryAddUnit(unit4);
            var teamZeroUnits = battleStateModel.GetTeamUnits(0);
            var teamOneUnits = battleStateModel.GetTeamUnits(1);


            Assert.IsTrue(teamZeroUnits.Contains(unit1));
            Assert.IsTrue(teamZeroUnits.Contains(unit2));
            Assert.IsFalse(teamZeroUnits.Contains(unit3));
            Assert.IsFalse(teamZeroUnits.Contains(unit4));
            Assert.IsTrue(teamZeroUnits.Count == 2);

            Assert.IsFalse(teamOneUnits.Contains(unit1));
            Assert.IsFalse(teamOneUnits.Contains(unit2));
            Assert.IsTrue(teamOneUnits.Contains(unit3));
            Assert.IsTrue(teamOneUnits.Contains(unit4));
            Assert.IsTrue(teamOneUnits.Count == 2);
        }

        [TestMethod]
        public void GetSpecificUnit()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(1, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit3 = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());
            var unit4 = new Unit(1, 1, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(unit1);
            battleStateModel.TryAddUnit(unit2);
            battleStateModel.TryAddUnit(unit3);
            battleStateModel.TryAddUnit(unit4);
            var incorrectIdentifier = new UnitIdentifier(100, 100);
            var fail1 = battleStateModel.TryGetUnit(incorrectIdentifier, out var fetchedFalseUnit);
            var successResult = battleStateModel.TryGetUnit(unit3.UnitIdentifier, out var fetchedUnit3);


            Assert.IsFalse(fail1);
            Assert.IsNull(fetchedFalseUnit);
            Assert.IsTrue(successResult);

            Assert.IsNotNull(fetchedUnit3);
            Assert.AreEqual(unit3, fetchedUnit3);
        }

        [TestMethod]
        public void FailGetNextUnit_EmptyBattle()
        {
            var successResult = battleStateModel.GetNextActiveUnit(out var nextUnit);

            Assert.IsFalse(successResult);
            Assert.IsNull(nextUnit);
        }

        [TestMethod]
        public void GetNextActiveUnit()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(1, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit3 = new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>());
            var unit4 = new Unit(1, 1, UnitHelper.CreateCharacter<Hunter>());


            battleStateModel.TryAddUnit(unit1);
            battleStateModel.TryAddUnit(unit2);
            battleStateModel.TryAddUnit(unit3);
            battleStateModel.TryAddUnit(unit4);


            var successResult = battleStateModel.GetNextActiveUnit(out var nextUnit);
            Assert.IsTrue(successResult);
            Assert.IsNotNull(nextUnit);
            Assert.IsTrue((nextUnit == unit1) || (nextUnit == unit2) || (nextUnit == unit3) || (nextUnit == unit4));
        }

        [TestMethod]
        public void GetFullUnitQueue_InitialUnitsOnly()
        {
            //var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(1, 0, UnitHelper.CreateCharacter<Scholar>());
            var unit3 = new Unit(0, 1, UnitHelper.CreateCharacter<Squire>());
            var unit4 = new Unit(1, 1, UnitHelper.CreateCharacter<Hunter>());
            
            List<Unit> units = new List<Unit>
            {
                unit2,
                unit3,
                unit4
            };
            battleStateModel = new BattleStateModel(units);

            var successResult = battleStateModel.GetUnitQueueShallowCopy(out var unitQueue);

            Assert.IsTrue(successResult);
            Assert.AreEqual(units.Count, unitQueue.Count);
            Assert.AreEqual(unitQueue.Dequeue(), unit4);
            Assert.AreEqual(unitQueue.Dequeue(), unit2);
            Assert.AreEqual(unitQueue.Dequeue(), unit3);

        }

        [TestMethod]
        public void GetFullUnitQueue_AdditionalUnits()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(1, 0, UnitHelper.CreateCharacter<Scholar>());
            var unit3 = new Unit(0, 1, UnitHelper.CreateCharacter<Squire>());
            var unit4 = new Unit(1, 1, UnitHelper.CreateCharacter<Hunter>());
            List<Unit> units = new List<Unit>
            {
                unit3,
                unit4
            };
            battleStateModel = new BattleStateModel(units);


            battleStateModel.TryAddUnit(unit1);
            battleStateModel.TryAddUnit(unit2);


            var successResult = battleStateModel.GetUnitQueueShallowCopy(out var unitQueue);

            Assert.IsTrue(successResult);
            Assert.AreEqual(4, unitQueue.Count);
            Assert.AreEqual(unitQueue.Dequeue(), unit4);
            Assert.AreEqual(unitQueue.Dequeue(), unit3);
            Assert.AreEqual(unitQueue.Dequeue(), unit1);
            Assert.AreEqual(unitQueue.Dequeue(), unit2);
        }

    }
}
