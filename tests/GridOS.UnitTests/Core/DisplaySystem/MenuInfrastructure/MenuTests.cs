using IngameScript;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Castle.DynamicProxy.Generators.Emitters;

namespace GridOS.UnitTests
{
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
            sut.GetContent(); // Initializing content.

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
            sut.GetContent(); // Initializing content.

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

            Assert.AreEqual(1, called, "Redraw invocation must happen when an item is added to or removed from the currently visible list.");
        }

        [Test]
        public void MoveUp_WhenSelectionIsAtTop_ShouldNotThrow()
        {
            var sut = new Menu(mockModel.Object, config);
            sut.GetContent(); // Initializing content.

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
            sut.GetContent(); // Initializing content.

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
    }
}
