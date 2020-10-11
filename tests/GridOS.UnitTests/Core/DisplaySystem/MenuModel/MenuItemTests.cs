using IngameScript;
using NUnit.Framework;

namespace GridOS.UnitTests
{
    [TestFixture]
    class MenuItemTests
    {
        [Test]
        public void Label_Changed_SendsNotification()
        {
            var callCount = 0;
            var menuItem = new MenuItem("String");
            menuItem.LabelChanged += (_) => callCount++;

            // Act
            menuItem.Label = "New String";

            Assert.AreEqual(1, callCount, "Change notification event expected to be invoked once.");
        }

        [Test]
        public void Label_Changed_SendsCorrectSender()
        {
            IMenuItem sender = null;
            var menuItem = new MenuItem("String");
            menuItem.LabelChanged += (x) => sender = x;

            // Act
            menuItem.Label = "New String";

            Assert.AreEqual(menuItem, sender, "Change notification must contain the origin of change as payload.");
        }
    }
}
