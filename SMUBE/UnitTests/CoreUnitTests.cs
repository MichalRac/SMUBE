using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Core;
using SMUBE.DataStructures;
using SMUBE.Units;
using System;

namespace SMUBE_Utils.UnitTests
{
    [TestClass]
    public class CoreUnitTests
    {

        private BattleCore battleCore;

        public CoreUnitTests()
        {
            battleCore = new BattleCore();
        }

        [TestMethod]
        public void AddUnitToBattle()
        {
            // arrange
            var newUnit = new Unit(0, 0);


            // act
            var success = battleCore.TryAddUnit(newUnit);
            

            // assert
            Assert.IsTrue(battleCore.Units.Contains(newUnit));
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CannotAddSameUnitTwice()
        {
            var newUnit = new Unit(0, 0);


            battleCore.TryAddUnit(newUnit);
            var success = battleCore.TryAddUnit(newUnit);


            Assert.IsFalse(success);
        }

        [TestMethod]
        public void CanAddMultipleUnits()
        {
            var unit1 = new Unit(0, 0);
            var unit2 = new Unit(0, 1);


            var success1 = battleCore.TryAddUnit(unit1);
            var success2 = battleCore.TryAddUnit(unit2);


            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsTrue(battleCore.Units.Contains(unit1) && battleCore.Units.Contains(unit2));
        }

        [TestMethod]
        public void CanRemoveAddedUnit() 
        {
            var addedUnit = new Unit(0, 0);
            var notAddedUnit = new Unit(0, 1);

            battleCore.TryAddUnit(addedUnit);
            var fail = battleCore.TryRemoveUnit(notAddedUnit);


            Assert.IsFalse(fail);
            Assert.IsTrue(battleCore.Units.Count == 1);
            Assert.IsTrue(battleCore.Units.Contains(addedUnit));
            Assert.IsFalse(battleCore.Units.Contains(notAddedUnit));
        }

        [TestMethod]
        public void CannotRemoveNotAddedUnit()
        {
            var newUnit = new Unit(0, 0);


            battleCore.TryAddUnit(newUnit);
            var success = battleCore.TryRemoveUnit(newUnit);


            Assert.IsTrue(success);
            Assert.IsFalse(battleCore.Units.Contains(newUnit));

        }

        [TestMethod]
        public void CanRemoveSpecificUnit()
        {
            var unit1 = new Unit(0, 0);
            var unit2 = new Unit(0, 1);


            battleCore.TryAddUnit(unit1);
            battleCore.TryAddUnit(unit2);
            var success1 = battleCore.TryRemoveUnit(unit1);


            Assert.IsTrue(success1);
            Assert.IsTrue(!battleCore.Units.Contains(unit1) && battleCore.Units.Contains(unit2));
        }

        [TestMethod]
        public void GetUnits()
        {
            var unit1 = new Unit(0, 0);
            var unit2 = new Unit(0, 1);
            var unit3 = new Unit(1, 0);


            battleCore.TryAddUnit(unit1);
            battleCore.TryAddUnit(unit2);
            battleCore.TryRemoveUnit(unit2);
            battleCore.TryAddUnit(unit3);
            var units = battleCore.Units;


            Assert.AreEqual(unit1, units[0]);
            Assert.AreNotEqual(unit2, units[1]);
            Assert.AreEqual(unit3, units[1]);
            Assert.IsTrue(battleCore.Units.Contains(unit1));
            Assert.IsFalse(battleCore.Units.Contains(unit2));
            Assert.IsTrue(battleCore.Units.Contains(unit3));
        }

        [TestMethod]
        public void GetUnitsForTeam()
        {
            var unit1 = new Unit(0, 0);
            var unit2 = new Unit(1, 0);
            var unit3 = new Unit(0, 1);
            var unit4 = new Unit(1, 1);


            battleCore.TryAddUnit(unit1);
            battleCore.TryAddUnit(unit2);
            battleCore.TryAddUnit(unit3);
            battleCore.TryAddUnit(unit4);
            var teamZeroUnits = battleCore.GetTeamUnits(0);
            var teamOneUnits = battleCore.GetTeamUnits(1);


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
            var unit1 = new Unit(0, 0);
            var unit2 = new Unit(1, 0);
            var unit3 = new Unit(0, 1);
            var unit4 = new Unit(1, 1);


            battleCore.TryAddUnit(unit1);
            battleCore.TryAddUnit(unit2);
            battleCore.TryAddUnit(unit3);
            battleCore.TryAddUnit(unit4);
            var incorrectIdentifier = new UnitIdentifier(100, 100);
            var fail1 = battleCore.GetUnit(incorrectIdentifier, out var fetchedFalseUnit);
            var success1 = battleCore.GetUnit(unit3.UnitIdentifier, out var fetchedUnit3);


            Assert.IsFalse(fail1);
            Assert.IsNull(fetchedFalseUnit);
            Assert.IsTrue(success1);

            Assert.IsNotNull(fetchedUnit3);
            Assert.AreEqual(unit3, fetchedUnit3);
        }
    }
}
