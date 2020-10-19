using IngameScript;
using NUnit.Framework;
using System;
using System.Linq;

namespace GridOS.UnitTests
{
    // Technically these tests are low-level integration tests, because dependencies aren't mocked, but they are simple enough not to affect the nature of the tests in a practical sense.
    [TestFixture]
    class MenuModelTests
    {
        private IMenuGroup menuRoot;
        private IMenuItem itemInRoot;
        private IMenuGroup groupInRoot;
        private IMenuItem itemInGroup;
        private IMenuCommand commandInRoot;
        private Action commandInRootAction = null;
        private IMenuCommand commandInGroup;
        private Action commandInGroupAction = null;

        [SetUp]
        public void SetUp()
        {
            menuRoot = new MenuGroup("Root");
            
            itemInRoot = new MenuItem("MenuItemInRoot");
            menuRoot.AddChild(itemInRoot);

            groupInRoot = new MenuGroup("MenuGroup");
            menuRoot.AddChild(groupInRoot);

            itemInGroup = new MenuItem("MenuItemInGroup");
            groupInRoot.AddChild(itemInGroup);

            commandInRootAction = null;
            commandInRoot = new MenuCommand("MenuCommandInRoot", (_) => commandInRootAction()); // Action wrapped in lambda; Overridable in test cases.
            menuRoot.AddChild(commandInRoot);

            commandInGroupAction = null;
            commandInGroup = new MenuCommand("MenuCommandInGroup", (_) => commandInGroupAction()); // Action wrapped in lambda; Overridable in test cases.
            groupInRoot.AddChild(commandInGroup);
        }

        [Test]
        public void CurrentTitle_InRoot_EqualsRootLabel()
        {
            var menuModel = new MenuModel(menuRoot, null);

            Assert.AreEqual(menuRoot.Label, menuModel.CurrentTitle);
        }

        [Test]
        public void CurrentTitleChanged__InRoot_WhenRootLabelChanges__ShouldGetInvocation()
        {
            var called = 0;
            var sentValue = "";
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.CurrentTitleChanged += (x) => { called++; sentValue = x; };

            // Act
            menuRoot.Label = "NewLabel";

            Assert.AreEqual(1, called, "Notification event expected to be called once.");
            Assert.AreEqual(menuRoot.Label, sentValue, "The value sent must be the new label of the current group.");
        }

        [Test]
        public void CurrentTitleChanged__InSubGroup_WhenSubGroupLabelChanges__ShouldGetInvocation()
        {
            var called = 0;
            var sentValue = "";
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.Select(groupInRoot); // Navigation also invokes CurrentTitleChanged, so it needs to be placed before the subscription.
            menuModel.CurrentTitleChanged += (x) => { called++; sentValue = x; };

            // Act
            groupInRoot.Label = "NewLabel";

            Assert.AreEqual(1, called, "Notification event expected to be called once.");
            Assert.AreEqual(groupInRoot.Label, sentValue, "The value sent must be the new label of the current group.");
        }

        [Test]
        public void CurrentTitleChanged_WhenNavigatingToNewGroup_ShouldGetInvocation()
        {
            var called = 0;
            var sentValue = "";
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.CurrentTitleChanged += (x) => { called++; sentValue = x; };

            // Act
            menuModel.Select(groupInRoot); // Navigating into another group; thus the newly opened group's title becomes the current view's title.

            Assert.AreEqual(1, called, "Notification event expected to be called once.");
            Assert.AreEqual(groupInRoot.Label, sentValue, "The value sent must be the label of the newly opened group.");
        }

        [Test]
        public void CurrentTitleChanged_WhenGroupIsNotOpen_ShouldNotBeInvoked()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.Select(groupInRoot); // Navigation also invokes CurrentTitleChanged, so it needs to be placed before the subscription.
            menuModel.CurrentTitleChanged += (_) => called++;

            // Act
            menuRoot.Label = "NewLabel";

            Assert.AreEqual(0, called, "Notification must not be sent if the changed group is not the currently open group.");
        }

        [Test]
        public void MenuItemChanged__InRoot_WhenItemChanges__ShouldGetInvocation()
        {
            var called = 0;
            IMenuItem changeOrigin = null;
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.MenuItemChanged += (x) => { called++; changeOrigin = x; };

            // Act
            itemInRoot.Label = "NewLabel";

            Assert.AreEqual(1, called, "One notification call expected when an item's label changes.");
            Assert.AreEqual(itemInRoot, changeOrigin, "The sender indicated in payload must be the item that changed.");
        }

        [Test]
        public void MenuItemChanged__InSubGroup_WhenItemChanges__ShouldGetInvocation()
        {
            var called = 0;
            IMenuItem changeOrigin = null;
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.MenuItemChanged += (x) => { called++; changeOrigin = x; };
            menuModel.Select(groupInRoot);

            // Act
            itemInGroup.Label = "NewLabel";

            Assert.AreEqual(1, called, "One notification call expected when an item's label changes.");
            Assert.AreEqual(itemInGroup, changeOrigin, "The sender indicated in payload must be the item that changed.");
        }

        [Test]
        public void MenuItemChanged_WhenChangedItemIsNotInView_ShouldNotBeInvoked()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.MenuItemChanged += (_) => called++;

            // Navigate away, back, and away again, to see if subscriptions/unsubscriptions are handled properly.
            menuModel.Select(groupInRoot);
            menuModel.MoveBack();
            menuModel.Select(groupInRoot);

            // Act
            itemInRoot.Label = "NewLabel";

            Assert.AreEqual(0, called, "Notification must not be sent for item changes outside of the current view.");
        }

        [Test]
        public void CurrentViewChanged__InRoot_WhenItemAddedAndRemoved__ShouldGetInvocation()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            var child = new MenuItem("NewItem");
            menuModel.CurrentViewChanged += (_) => called++;

            // Act
            menuRoot.AddChild(child);
            menuRoot.RemoveChild(child);

            Assert.AreEqual(2, called, "One notification for adding, and one for removing, was expected.");
        }

        [Test]
        public void CurrentViewChanged__InSubGroup_WhenItemAddedOrRemoved__ShouldGetInvocation()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            var child = new MenuItem("NewItem");
            menuModel.CurrentViewChanged += (_) => called++;
            menuModel.Select(groupInRoot);

            // Act
            groupInRoot.AddChild(child);
            groupInRoot.RemoveChild(child);

            Assert.AreEqual(2, called, "One notification for adding, and one for removing, was expected.");
        }

        [Test]
        public void CurrentViewChanged_WhenChangesAreNotInView_ShouldNotBeInvoked()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            var child = new MenuItem("NewItem");
            menuModel.CurrentViewChanged += (x) => called++;

            // Navigate away, back, and away again, to see if subscriptions/unsubscriptions are handled properly.
            menuModel.Select(groupInRoot);
            menuModel.MoveBack();
            menuModel.Select(groupInRoot);

            // Act
            menuRoot.AddChild(child);
            menuRoot.RemoveChild(child);

            Assert.AreEqual(0, called, "Notification must not be sent if items are added to or removed from a group that is not the current view.");
        }

        [Test]
        public void Select_WhenCommandIsPartOfView_ReturnsTrueAndCommandIsExecuted()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            commandInRootAction = () => called++;

            // Act
            var result = menuModel.Select(commandInRoot); // Command in root.

            Assert.IsTrue(result, "Return value should indicate success.");
            Assert.AreEqual(1, called, "Command action expected to be called once.");
        }

        [Test]
        public void Select_WhenCommandIsNotPartOfView_ReturnsFalseAndCommandIsNotExecuted()
        {
            var called = 0;
            var menuModel = new MenuModel(menuRoot, null);
            commandInGroupAction = () => called++;

            // Act
            var result = menuModel.Select(commandInGroup); // Command in another group.

            Assert.IsFalse(result, "Return value should indicate failure.");
            Assert.AreEqual(0, called, "Command action expected NOT to be called.");
        }

        [Test]
        public void Select_WhenSelectingValidGroup_ReturnsTrueAndNavigationIsCompleted()
        {
            var called = 0;
            NavigationPayload sentPayload = default;
            var menuModel = new MenuModel(menuRoot, null);
            menuModel.NavigatedTo += (x) => { called++; sentPayload = x; };

            // Act
            var result = menuModel.Select(groupInRoot);

            Assert.IsTrue(result, "Return value should indicate success.");
            Assert.AreEqual(1, called, "Navigation notification is expected to be invoked once.");
            Assert.AreEqual(menuRoot, sentPayload.NavigatedFrom, "Notification payload should contain the correct group (root) as the previous group.");
            Assert.AreEqual(groupInRoot, sentPayload.NavigatedTo, "Notification payload should contain the correct group as the newly opened group.");
            Assert.AreEqual(groupInRoot.Label, menuModel.CurrentTitle, "Current title of menu should change to the label of the opened group.");
            CollectionAssert.IsSupersetOf(menuModel.CurrentView, groupInRoot.GetChildren(), "All items of the opened group should be part of the current view.");
        }
    }
}
