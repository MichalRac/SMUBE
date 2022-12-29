using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;

namespace SMUBE_Utils.UnitTests
{
    [TestClass]
    public class UnitUnitTests
    {
        [TestMethod]
        public Unit CreateUnit()
        {
            var id = 0;
            var teamId = 0;

            var unit = new Unit(id, teamId, UnitHelper.CreateCharacter<Hunter>());

            Assert.IsNotNull(unit);
            Assert.Equals(unit.UnitIdentifier.Id, id);
            Assert.Equals(unit.UnitIdentifier.TeamId, teamId);

            return new Unit(id, teamId, UnitHelper.CreateCharacter<Hunter>());
        }

        [TestMethod]
        public void GetUnitViableCommands()
        {
            var unit1 = new Unit(0, 0, UnitHelper.CreateCharacter<Hunter>());
            var unit2 = new Unit(0, 1, UnitHelper.CreateCharacter<Squire>());

            var viableCommands1 = unit1.GetViableCommands();
            var viableCommands2 = unit2.GetViableCommands();


        }
    }
}
