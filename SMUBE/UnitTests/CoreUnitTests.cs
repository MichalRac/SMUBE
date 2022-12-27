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
            var initalUnits = new List<Unit>()
            {
                new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>()),
                new Unit(1, 0, UnitHelper.CreateCharacter<Hunter>()),
                new Unit(2, 0, UnitHelper.CreateCharacter<Hunter>()),

                new Unit(0, 1, UnitHelper.CreateCharacter<Hunter>()),
                new Unit(1, 1, UnitHelper.CreateCharacter<Hunter>()),
                new Unit(2, 1, UnitHelper.CreateCharacter<Hunter>()),
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
