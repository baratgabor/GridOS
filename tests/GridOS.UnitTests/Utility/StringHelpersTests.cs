using NUnit.Framework;
using System.Linq;

namespace GridOS.UnitTests
{
    [TestFixture]
    public class StringHelpersTests
    {
        [Test]
        public void WordWrap_StringShorterThanLineLength_SameAsInput()
        {
            var test = "Test Test Test";
            
            var res = IngameScript.Program.StringHelpers.WordWrap(test, test.Length + 10, new[] { ' ' });

            Assert.Multiple(() => {
                Assert.AreEqual(1, res.Count(), "Single line expected.");
                Assert.AreEqual(test, res.First().ToString(), "Result must be the same as the input.");
            });
        }

        [Test]
        public void WordWrap_StringEqualToLineLength_SameAsInput()
        {
            var test = "Test Test Test";

            var res = IngameScript.Program.StringHelpers.WordWrap(test, test.Length, new[] { ' ' });

            Assert.Multiple(() => {
                Assert.AreEqual(1, res.Count(), "Single line expected.");
                Assert.AreEqual(test, res.First().ToString(), "Result must be the same as the input.");
            });
        }

        [Test]
        public void WordWrap_StringLongerThanLineLength_BrokenIntoTwoAtWordBoundary()
        {
            var test = "Test Test Test Test Test";
            var lineLength = test.Length - 1;

            var res = IngameScript.Program.StringHelpers.WordWrap(test, lineLength, new[] { ' ' });

            Assert.AreEqual(2, res.Count(), "Two lines expected.");
            Assert.Multiple(() =>
            {
                Assert.IsTrue(res.ElementAt(0).ToString().Length <= lineLength, "First line's length must not exceed line length.");
                Assert.IsTrue(res.ElementAt(1).ToString().Length <= lineLength, "Second line's length must not exceed line length.");
                Assert.AreEqual("Test Test Test Test ", res.ElementAt(0).ToString());
                Assert.AreEqual("Test", res.ElementAt(1).ToString());
            });
        }

        [Test]
        public void WordWrap_StringWithNewLine_NewLineIsPreserved()
        {
            var test = "Test1\r\nTest2";

            var res = IngameScript.Program.StringHelpers.WordWrap(test, test.Length + 10, new[] { ' ' });

            Assert.AreEqual(2, res.Count(), "Two lines are expected.");
            Assert.Multiple(() => {
                Assert.AreEqual("Test1", res.ElementAt(0).ToString());
                Assert.AreEqual("Test2", res.ElementAt(1).ToString());
            });
        }

        [Test]
        public void WordWrap_StringWithMultipleNewLines_NewLinesArePreserved()
        {
            var test = "Test1\r\n\r\n\r\n\r\nTest2";

            var res = IngameScript.Program.StringHelpers.WordWrap(test, test.Length + 10, new[] { ' ' });

            Assert.AreEqual(5, res.Count(), "Five lines are expected.");
            Assert.Multiple(() => {
                Assert.AreEqual("Test1", res.First().ToString());
                Assert.AreEqual(string.Empty, res.ElementAt(1).ToString());
                Assert.AreEqual(string.Empty, res.ElementAt(2).ToString());
                Assert.AreEqual(string.Empty, res.ElementAt(3).ToString());
                Assert.AreEqual("Test2", res.Last().ToString());
            });
        }

        [Test]
        public void WordWrap_StringWithVeryLongWord_LongWordIsForceBroken()
        {
            var test = "ABCDEFGHIJKLMNOPQRST";

            var res = IngameScript.Program.StringHelpers.WordWrap(test, 15, new[] { ' ' });

            Assert.AreEqual(2, res.Count(), "Two lines are expected.");
            Assert.Multiple(() => {
                Assert.AreEqual("ABCDEFGHIJKLMNO", res.First().ToString());
                Assert.AreEqual("PQRST", res.Last().ToString());
            });
        }

        [Test]
        public void WordWrap_StringWithVeryVeryLongWord_LongWordIsForceBrokenIntoMultipleLines()
        {
            var test = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var res = IngameScript.Program.StringHelpers.WordWrap(test, 10, new[] { ' ' });

            Assert.AreEqual(3, res.Count(), "Three lines are expected.");
            Assert.Multiple(() => {
                Assert.AreEqual("ABCDEFGHIJ", res.ElementAt(0).ToString());
                Assert.AreEqual("KLMNOPQRST", res.ElementAt(1).ToString());
                Assert.AreEqual("UVWXYZ"    , res.ElementAt(2).ToString());
            });
        }

        [Test]
        public void WordWrap_ComplexString_WrappedProperly()
        {
            var test = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\n\r\n\r\nWord Word Word 0123456789 Word1-Word2";

            var res = IngameScript.Program.StringHelpers.WordWrap(test, 10, new[] { ' ', '-' });

            Assert.AreEqual(10, res.Count(), "Ten lines are expected.");
            Assert.Multiple(() => {
                Assert.AreEqual("ABCDEFGHIJ", res.ElementAt(0).ToString());
                Assert.AreEqual("KLMNOPQRST", res.ElementAt(1).ToString());
                Assert.AreEqual("UVWXYZ"    , res.ElementAt(2).ToString());
                Assert.AreEqual(""          , res.ElementAt(3).ToString());
                Assert.AreEqual(""          , res.ElementAt(4).ToString());
                Assert.AreEqual("Word Word ", res.ElementAt(5).ToString());
                Assert.AreEqual("Word "     , res.ElementAt(6).ToString());
                Assert.AreEqual("0123456789 ", res.ElementAt(7).ToString()); // TODO: Decide if trailing space should be trimmed in edge-case where word length equals line length.
                Assert.AreEqual("Word1-"    , res.ElementAt(8).ToString());
                Assert.AreEqual("Word2"     , res.ElementAt(9).ToString());
            });
        }
    }
}
