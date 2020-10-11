using IngameScript;
using NUnit.Framework;
using System.Collections.Generic;

namespace GridOS.UnitTests
{
    [TestFixture]
    class MenuCommandTests
    {
        [Test]
        public void Execute_WithNullAction_DoesNotThrow()
        {
            MenuCommand menuCommand = null;
            Assert.DoesNotThrow(
                () => menuCommand = new MenuCommand("Label", null));

            // Act
            Assert.DoesNotThrow(
                () => menuCommand.Execute(), "Command execution is not supposed to throw when action is null, to support the disabling of commands.");
        }

        [Test]
        public void Execute_DoesInvokeAction()
        {
            var called = 0;
            var menuCommand = new MenuCommand(
                label: "Label",
                action: () => called++
            );

            // Act
            menuCommand.Execute();

            Assert.AreEqual(1, called, "Command execution must invoke the command action once.");
        }

        [Test]
        public void Execute_InvokesNotificationsInCorrectOrder()
        {
            var expectedOrder = new List<string>() { "before", "command", "after"};

            var actualOrder = new List<string>();
            var menuCommand = new MenuCommand(
                label: "Label",
                action: () => actualOrder.Add("command"));
            menuCommand.BeforeExecute += (x) => actualOrder.Add("before");
            menuCommand.Executed += (x) => actualOrder.Add("after");

            // Act
            menuCommand.Execute();

            CollectionAssert.AreEqual(expectedOrder, actualOrder);
        }
    }
}
