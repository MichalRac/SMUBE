using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;

namespace UnitTests
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
    }
}
