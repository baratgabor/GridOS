using IngameScript;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridOS.UnitTests
{
    // This class also includes tests pertaining to menu display which are functionally related to some non-abstracted components used by the Menu class.
    // For the sake of simplicity, I'm considering these functionalities as a single unit to be tested. But if/when the complexity requires it, the corresponding tests will be extracted to target the specific dependencies.
    [TestFixture]
    class MenuTests
    {
        const char menuSelectionMarker = '►';
        const int lineHeight = 5;
        const int lineLength = 25;
        readonly Mock<IMenuModel> mockModel = new Mock<IMenuModel>();
        MainConfig config;
        readonly MenuItem firstMenuItem = new MenuItem("Item1");
        readonly MenuItem seventhMenuItem = new MenuItem("item7");

        [SetUp]
        public void SetUp()
        {
            config = new MainConfig() { LineHeight = lineHeight, LineLength = lineLength, SelectionMarker = menuSelectionMarker, PaddingLeft = 0 };

            mockModel.Setup(x => x.CurrentView)
                .Returns(new List<IMenuItem>() {
                    firstMenuItem,
                    new MenuItem("Item2"),
                    new MenuItem("Item3"),
                    new MenuItem("Item4"),
                    new MenuItem("Item5"),
                    new MenuItem("Item6"),
                    seventhMenuItem,
                    new MenuItem("Item8"),
                });
        }

        [Test]
        public void ModelChange_InInitialState_ShouldInvokeRedrawRequired()
        {
            var called = 0;
            var sut = new Menu(mockModel.Object, config);
            sut.RedrawRequired += (_) => called++;

            // Act
            mockModel.Raise(x => x.MenuItemChanged += null, (object)null);

            Assert.AreEqual(1, called, "Invocation must happen once, irrespective of what changed, because content is not initialized.");
        }

        [Test]
        public void ModelChange_IfItemIsVisible_ShouldInvokeRedrawRequired()
        {
            var called = 0;
            var sut = new Menu(mockModel.Object, config);
            sut.RedrawRequired += (_) => called++;

            // Act
            mockModel.Raise(x => x.MenuItemChanged += null, (object)firstMenuItem);

            Assert.AreEqual(1, called, "Invocation must happen once, because this item is visible in the viewport.");
        }

        [Test]
        public void ModelChange_IfItemIsNotVisible_ShouldNotRequestRedraw()
        {
            var called = 0;
            var sut = new Menu(mockModel.Object, config);
            sut.RedrawRequired += (_) => called++;
            sut.GetContent(); // Initializing content. Without this it will invoke the redraw request, because it cannot determine the item's visiblity. This is expected behavior.

            // Act
            mockModel.Raise(x => x.MenuItemChanged += null, (object)seventhMenuItem);

            Assert.AreEqual(0, called, "Invocation must not happen, because this item is outside of the viewport.");
        }

        [Test]
        public void ModelListChange_AtAllConditions_ShouldInvokeRedrawRequired()
        {
            var called = 0;
            var sut = new Menu(mockModel.Object, config);
            sut.RedrawRequired += (_) => called++;

            // Act
            mockModel.Raise(x => x.CurrentViewChanged += null, (object)null);

            Assert.AreEqual(1, called, "Redraw invocation must happen once when an item is added to or removed from the currently visible list.");
        }

        [Test]
        public void Navigating_ShouldInvokeNotifications()
        {
            var redrawCalled = 0;
            var pathChangedCalled = 0;
            var sut = new Menu(mockModel.Object, config);
            sut.RedrawRequired += (_) => redrawCalled++;
            sut.NavigationPathChanged += (_) => pathChangedCalled++;

            // Act
            mockModel.Raise(x => x.NavigatedTo += null, (object)null);

            Assert.AreEqual(1, redrawCalled, "Redraw invocation must happen once when model is navigated to a new group.");
            Assert.AreEqual(1, redrawCalled, "Path changed invocation must happen once when model is navigated to a new group.");
        }

        [Test]
        public void Navigating_ForwardNavigation_ShouldResetLineSelection()
        {
            var sut = new Menu(mockModel.Object, config);
            sut.MoveDown(); // Selection moved down.
            sut.MoveDown();
            StringAssert.DoesNotStartWith(menuSelectionMarker.ToString(), sut.GetContent().ToString(), "Arrange failed to create necessary pre-test condition. Selection was expected to be moved down from the first element.");

            // Act
            mockModel.Raise(x => x.NavigatedTo += null, (object)null);

            StringAssert.StartsWith(menuSelectionMarker.ToString(), sut.GetContent().ToString(), "Line selection must be reset to top after forward navigation.");
        }

        [Test]
        public void Navigating_BackNavigation_ShouldRestoreLineSelection()
        {
            config.LineLength = 10;
            var expectedSelectedGroup = new MenuGroup("Group - long multi-line item to test if selection will be on the first line");
            var anotherGroup = new MenuGroup("AnotherGroup");
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() {
                new MenuItem("Item"),
                new MenuItem("Item"),
                new MenuItem("Item"),
                expectedSelectedGroup
            });
            mockModel.Setup(x => x.GetIndexOf(expectedSelectedGroup)).Returns(3); // Tells SUT the position of the item in the menu.
            var sut = new Menu(mockModel.Object, config);
            StringAssert.StartsWith(menuSelectionMarker.ToString(), sut.GetContent().ToString(), "Arrange failed to create necessary pre-test condition. First line was expected to be selected.");

            // Act
            mockModel.Raise(x => x.NavigatedTo += null, new NavigationPayload() { NavigatedFrom = expectedSelectedGroup, NavigatedTo = anotherGroup } );

            var lineOfGroup = sut.GetContent().ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None)[3];
            StringAssert.StartsWith(config.Prefixes_Selected.Group + " Group", lineOfGroup, "The group was expected to be selected. After navigating backwards from a previously selected group, the selection must be restored.");
        }

        [Test]
        public void MoveUp_WhenSelectionIsAtTop_ShouldNotThrow()
        {
            var sut = new Menu(mockModel.Object, config);

            // Act
            TestDelegate act = () =>
            {
                // Technically one would be enough, because we're at the top initially.
                sut.MoveUp();
                sut.MoveUp();
                sut.MoveUp();
            };

            Assert.DoesNotThrow(act, "Must not throw, because invoking moving when we're already at boundary is expected user behavior.");
        }

        [Test]
        public void MoveDown_WhenSelectionIsAtTop_ShouldNotThrow()
        {
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { 
                new MenuItem("Item1"),
                new MenuItem("Item2"),
            });
            var sut = new Menu(mockModel.Object, config);

            // Act
            TestDelegate act = () =>
            {
                // Number of moving downs clearly exceeds the number of items set in Arrange.
                sut.MoveDown();
                sut.MoveDown();
                sut.MoveDown();
                sut.MoveDown();
            };

            Assert.DoesNotThrow(act, "Must not throw, because invoking moving when we're already at boundary is expected user behavior.");
        }

        [Test]
        public void MoveDown_WhenScrolling_ReachesCorrectStateAtBottom()
        {
            config.LineHeight = 3;
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() {
                new MenuItem("Item1"),
                new MenuItem("Item2"),
                new MenuItem("Item3"),
                new MenuItem("Item4"),
                new MenuItem("Item5"),
            });
            var sut = new Menu(mockModel.Object, config);

            // Act
            sut.MoveDown();
            sut.MoveDown();
            sut.MoveDown();
            sut.MoveDown();

            var lines = sut.GetContent().ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.AreEqual(3, lines.Length, "Number of lines expected to be equal to the number set in config.");
            Assert.AreEqual("Item4", lines[0].Trim(), "First menu line is expected to contain the next-to-last menu item, without anything else.");
            StringAssert.Contains(menuSelectionMarker + " Item5", lines[1], "Second menu line is expected to contain the last menu item, preceded by a selection mark.");
            Assert.AreEqual(string.Empty, lines[2], "Last menu line is expected to be empty. This is to visually communicate that the end of the menu is reached.");
        }

        [Test]
        public void GetContent_WithBlankMenu_ShouldStillReturnCorrectNumberOfEmptyLines()
        {
            config.LineHeight = 5;
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { /* Blank */ });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.AreEqual(5, lines.Length);
            Assert.IsTrue(lines.All(x => x == string.Empty));
        }

        [Test]
        public void GetContent_WithOneMenuItem_ShouldReturnItemPlusEmptyLines()
        {
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuItem("First") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.AreEqual($"{menuSelectionMarker} First", lines.First());
        }

        [TestCase (3)]
        [TestCase (4)]
        [TestCase (5)]
        [TestCase(20)]
        public void GetContent_WithDifferingLineHeights_AlwaysReturnsExpectedNumberOfLines(int expectedLineNumber)
        {
            config.LineHeight = expectedLineNumber;
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.AreEqual(expectedLineNumber, lines.Length);
        }

        [Test]
        public void GetContent_AtInitialState_FirstLineShouldStartWithSelectionMarker()
        {
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.AreEqual(menuSelectionMarker, lines.First().Trim()[0]);
        }

        [Test]
        public void GetContent_WithItemSelected_ReturnsCorrectSelectionMarker()
        {
            config.Prefixes_Selected = new AffixConfig() { Item = '~' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuItem("Item") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            Assert.AreEqual('~', result[0], "Selected item must have the correct marker/prefix.");
        }

        [Test]
        public void GetContent_WithCommandSelected_ReturnsCorrectSelectionMarker()
        {
            config.Prefixes_Selected = new AffixConfig() { Command = '~' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuCommand("Command", null)});
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            Assert.AreEqual('~', result[0], "Selected command must have the correct marker/prefix.");
        }

        [Test]
        public void GetContent_WithGroupSelected_ReturnsCorrectSelectionMarker()
        {
            config.Prefixes_Selected = new AffixConfig() { Group = '~' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuGroup("Group") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            Assert.AreEqual('~', result[0], "Selected group must have the correct marker/prefix.");
        }

        [Test]
        public void GetContent_WithItemNOTSelected_ReturnsCorrectPrefix()
        {
            config.Prefixes_Unselected = new AffixConfig() { Item = '~' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuItem("Item"), new MenuItem("Item2") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            sut.MoveDown(); // Unselects first item.
            var result = sut.GetContent().ToString();

            Assert.AreEqual('~', result[0], "Non-selected item must have the correct marker/prefix.");
        }

        [Test]
        public void GetContent_WithCommandNOTSelected_ReturnsCorrectPrefix()
        {
            config.Prefixes_Unselected = new AffixConfig() { Command = '~' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuCommand("Command", null), new MenuItem("Item2") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            sut.MoveDown(); // Unselects first item.
            var result = sut.GetContent().ToString();

            Assert.AreEqual('~', result[0], "Non-selected command must have the correct marker/prefix.");
        }

        [Test]
        public void GetContent_WithGroupNOTSelected_ReturnsCorrectSelectionMarker()
        {
            config.Prefixes_Unselected = new AffixConfig() { Group = '~' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuGroup("Group"), new MenuItem("Item2") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            sut.MoveDown(); // Unselects first item.
            var result = sut.GetContent().ToString();

            Assert.AreEqual('~', result[0], "Non-selected group must have the correct marker/prefix.");
        }

        [Test]
        public void GetContent_ItemWithSuffix_ReturnsCorrectSuffix()
        {
            config.Suffixes = new AffixConfig() { Item = '%' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuItem("Item") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var firstLine = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
            StringAssert.EndsWith("Item %", firstLine, "Menu line must end with the item name followed by the suffix.");
        }

        [Test]
        public void GetContent_GroupWithSuffix_ReturnsCorrectSuffix()
        {
            config.Suffixes = new AffixConfig() { Group = '%' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuGroup("Group") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var firstLine = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
            StringAssert.EndsWith("Group %", firstLine, "Menu line must end with the group name followed by the suffix.");
        }

        [Test]
        public void GetContent_CommandWithSuffix_ReturnsCorrectSuffix()
        {
            config.Suffixes = new AffixConfig() { Command = '%' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuCommand("Command", null) });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var firstLine = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
            StringAssert.EndsWith("Command %", firstLine, "Menu line must end with the command name followed by the suffix.");
        }

        [Test]
        public void GetContent_ItemWithEmptySuffix_HasTheSuffixCutOff()
        {
            config.Suffixes = new AffixConfig() { Item = ' ' };
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() { new MenuItem("Item") });
            var sut = new Menu(mockModel.Object, config);

            // Act
            var result = sut.GetContent().ToString();

            var firstLine = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
            StringAssert.EndsWith("Item", firstLine, "Suffix must be removed (no trailing spaces) for items that have no suffix set.");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void GetContent_UnderAllConditions_SelectedLineShouldStartWithSelectionMarker (int expectedSelectedLineIndex)
        {
            config.LineHeight = 20;
            var sut = new Menu(mockModel.Object, config);
            sut.GetContent();
            for (int i = 0; i < expectedSelectedLineIndex; i++)
            {
                sut.MoveDown(); // Moving down means that the next line should be selected.
            }

            // Act
            var result = sut.GetContent().ToString();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.AreEqual(menuSelectionMarker, lines[expectedSelectedLineIndex].Trim()[0]);
        }

        [Test]
        public void Select_ExecutesCorrectCommand()
        {
            var expectedCommand = new MenuCommand("Command", null);
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() {
                new MenuItem("Item1"),
                new MenuCommand("Command1",  null),
                expectedCommand,
                new MenuCommand("Command2",  null)
            });
            var sut = new Menu(mockModel.Object, config);

            // Act
            sut.MoveDown();
            sut.MoveDown();
            sut.Select();

            mockModel.Verify(mock => mock.Select(expectedCommand), Times.Once);
        }

        [Test]
        public void Select_ExecutesCorrectGroup()
        {
            var expectedGroup = new MenuGroup("Group");
            mockModel.Setup(x => x.CurrentView).Returns(new List<IMenuItem>() {
                new MenuItem("Item1"),
                new MenuGroup("Group1"),
                expectedGroup,
                new MenuGroup("Group2")
            });
            var sut = new Menu(mockModel.Object, config);

            // Act
            sut.MoveDown();
            sut.MoveDown();
            sut.Select();

            mockModel.Verify(mock => mock.Select(expectedGroup), Times.Once);
        }
    }
}
