using Commands;
using Commands.Skills;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMUBE.Commands.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace SMUBE_Utils.UnitTests
{
    
    /*
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void CreateDamageEffect()
        {
            //Arrange
            var damageValue = 10;

            //Act
            var damageEffect = new DamageEffect(damageValue, null);

            //Assert
            Assert.AreEqual(damageValue, damageEffect.Value);
        }

        [TestMethod]
        public void CreateSkill()
        {
            var skill = new Skill();
        }

        [TestMethod]
        public void ValidateCommandId_AreUnique()
        {
            var enums = Enum.GetValues(typeof(CommandId));

            Dictionary<int, List<CommandId>> usedValues = new Dictionary<int, List<CommandId>>();

            foreach (var e in enums)
            {
                var commanIdValue = (CommandId)e;
                var commandIdRawValue = (int)e;

                var firstOccurance = !usedValues.ContainsKey(commandIdRawValue);
                if (firstOccurance)
                {
                    usedValues.Add(commandIdRawValue, new List<CommandId>() { commanIdValue } );
                }

                Assert.IsTrue(firstOccurance, $"The value {commandIdRawValue} is duplicated! Case: CommandId.{commanIdValue}");
            }
        }
    }
*/
}
