using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Core;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;

namespace SMUBE_Utils.UnitTests
{
    [TestClass]
    public class CoreUnitTests
    {
        public BattleCore battleCore;

        [TestMethod]
        public void SetupCore()
        {
            // arrange
            var teamId0 = 0;
            var teamId1 = 1;
            var initalUnits = new List<Unit>()
            {
                UnitHelper.CreateUnit<Hunter>(teamId0),
                UnitHelper.CreateUnit<Scholar>(teamId1),
                UnitHelper.CreateUnit<Squire>(teamId0),
                UnitHelper.CreateUnit<Hunter>(teamId1),
            };

            // act
            battleCore = new BattleCore(initalUnits);

            // assert
            Assert.AreEqual(initalUnits, battleCore.currentStateModel.Units);
        }

        [TestMethod]
        public void CannotSetupCoreWithNoInitialUnits()
        {
            var emptyInitialUnits = new List<Unit>();

            Action action = () => { new BattleCore(null); };
            Action action2 = () => { new BattleCore(emptyInitialUnits); };

            Assert.ThrowsException<ArgumentException>(action);
            Assert.ThrowsException<ArgumentException>(action2);
        }
    }
}
