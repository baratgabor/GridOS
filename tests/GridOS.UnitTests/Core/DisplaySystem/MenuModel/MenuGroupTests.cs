using IngameScript;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GridOS.UnitTests
{
    [TestFixture]
    class MenuGroupTests
    {
        [Test]
        public void AddChild_WithNewChild_ActuallyAddsChild()
        {
            var menuGroup = new MenuGroup("Group");
            var child = new MenuItem("Item");
            
            // Act
            menuGroup.AddChild(child);

            Assert.AreEqual(child, menuGroup.GetChildren().FirstOrDefault());
        }

        [Test]
        public void AddChild_WithNewChild_SendsNotification()
        {
            var called = 0;
            var menuGroup = new MenuGroup("Group");
            menuGroup.ChildrenChanged += (_) => called++;

            // Act
            menuGroup.AddChild(new MenuItem("Item"));

            Assert.AreEqual(1, called, "Notification event expected to be invoked once.");
        }

        [Test]
        public void AddChild_WithExistingChild_DoesNotAddAndDoesNotNotify()
        {
            var called = 0;
            var menuGroup = new MenuGroup("Group");
            var child = new MenuItem("Item");
            menuGroup.AddChild(child);
            menuGroup.ChildrenChanged += (_) => called++;

            // Act
            menuGroup.AddChild(child);

            Assert.AreEqual(1, menuGroup.GetChildren().Count(), "An already added child item must not be added the second time.");
            Assert.AreEqual(0, called, "Notification must not be sent if child already present in group.");
        }

        [Test]
        public void RemoveChild_WithExistingChild_RemovesCorrectChild()
        {
            var menuGroup = new MenuGroup("Group");
            var child1 = new MenuItem("Child1");
            var child2 = new MenuItem("Child2");
            var child3 = new MenuItem("Child3");
            menuGroup.AddChild(child1);
            menuGroup.AddChild(child2);
            menuGroup.AddChild(child3);

            // Act
            menuGroup.RemoveChild(child2);

            CollectionAssert.AreEqual(new List<IMenuItem>() { child1, child3 }, menuGroup.GetChildren());
        }

        [Test]
        public void RemoveChild_WithExistingChild_SendsNotification()
        {
            var called = 0;
            var menuGroup = new MenuGroup("Group");
            var child = new MenuItem("Item");
            menuGroup.AddChild(child);
            menuGroup.ChildrenChanged += (_) => called++; // Subscribed after child was already added.

            // Act
            menuGroup.RemoveChild(child);

            Assert.AreEqual(1, called, "Notification event expected to be invoked once.");
        }

        [Test]
        public void ChildLabelChanged_WhenChildLabelChanges_IsInvoked()
        {
            var called = 0;
            IMenuItem senderItem = null;
            var menuGroup = new MenuGroup("Group");
            var child = new MenuItem("Child");
            menuGroup.ChildLabelChanged += (x) => { called++; senderItem = x; };
            menuGroup.AddChild(child);

            // Act
            child.Label = "New Label";

            Assert.AreEqual(1, called);
            Assert.AreEqual(child, senderItem);
        }

        [Test]
        public void ChildLabelChanged_WhenRemovedChildChanges_IsNotInvoked()
        {
            var called = 0;
            IMenuItem senderItem = null;
            var menuGroup = new MenuGroup("Group");
            var child = new MenuItem("Child");
            menuGroup.AddChild(child);
            menuGroup.ChildLabelChanged += (x) => { called++; senderItem = x; };
            menuGroup.RemoveChild(child);

            // Act
            child.Label = "New Label";

            Assert.AreEqual(0, called);
            Assert.AreEqual(null, senderItem);
        }

        [Test]
        public void Open_WhenCalledMultipleTimes_DoesNotThrow()
        {
            var menuGroup = new MenuGroup("Group");

            // Act
            TestDelegate act = () =>
            {
                menuGroup.Open();
                menuGroup.Open();
                menuGroup.Open();
            };

            Assert.DoesNotThrow(act, $"Calling {nameof(MenuGroup.Open)}() multiple times should not throw, to support opening groups in multiple displays."); // Granted, this is a very lousy way to handle it.
        }

        [Test]
        public void Close_WhenCalledBeforeOpen_Throws()
        {
            var menuGroup = new MenuGroup("Group");

            // Act
            TestDelegate act = () =>
            {
                menuGroup.Close();
            };

            Assert.Throws<Exception>(act, "Closing before Open is expected to throw exception.");
        }
    }
}
