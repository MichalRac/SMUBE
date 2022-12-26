using Commands.Skills;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Commands.Effects;
using System;

namespace SMUBE_Utils.UnitTests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void CreateDamageEffect()
        {
            //Arrange
            var damageValue = 10;

            //Act
            var damageEffect = new DamageEffect(damageValue);

            //Assert
            Assert.AreEqual(damageValue, damageEffect.Value);
        }

        [TestMethod]
        public void CreateSkill()
        {
            var skill = new Skill();
        }
    }
}
