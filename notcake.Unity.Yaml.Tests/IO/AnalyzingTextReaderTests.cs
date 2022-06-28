using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.IO;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Tests.IO
{
    /// <summary>
    ///     Tests for <see cref="AnalyzingTextReader"/>.
    /// </summary>
    [TestClass]
    public class AnalyzingTextReaderTests
    {
        /// <summary>
        ///     Tests reading a '\r' character, followed by a '\n' character.
        /// </summary>
        [TestMethod]
        public void ReadCarriageReturnReadLineFeed()
        {
            using StringReader stringReader = new("\r\n");
            using AnalyzingTextReader analyzingTextReader = new(stringReader);

            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(null, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(false, analyzingTextReader.LastCharactersReadWereLineBreak);

            Assert.AreEqual('\r', analyzingTextReader.Read());
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(1UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(LineBreakStyle.CarriageReturn, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(true, analyzingTextReader.LastCharactersReadWereLineBreak);

            Assert.AreEqual('\n', analyzingTextReader.Read());
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(1UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(LineBreakStyle.CarriageReturnLineFeed, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(true, analyzingTextReader.LastCharactersReadWereLineBreak);

            Assert.AreEqual(-1, analyzingTextReader.Read());
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(1UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(LineBreakStyle.CarriageReturnLineFeed, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(true, analyzingTextReader.LastCharactersReadWereLineBreak);
        }

        /// <summary>
        ///     Tests reading an empty line, ending with a "\r\n" line break.
        /// </summary>
        [TestMethod]
        public void ReadLineCarriageReturnLineFeed()
        {
            using StringReader stringReader = new("\r\n");
            using AnalyzingTextReader analyzingTextReader = new(stringReader);

            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(null, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(false, analyzingTextReader.LastCharactersReadWereLineBreak);

            Assert.AreEqual("", analyzingTextReader.ReadLine());
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(1UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(LineBreakStyle.CarriageReturnLineFeed, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(true, analyzingTextReader.LastCharactersReadWereLineBreak);
        }

        /// <summary>
        ///     Tests reading a '\r' character, followed by reading an empty line, ending with a
        ///     '\n' character.
        /// </summary>
        [TestMethod]
        public void ReadCarriageReturnReadLineLineFeed()
        {
            using StringReader stringReader = new("\r\n");
            using AnalyzingTextReader analyzingTextReader = new(stringReader);

            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(null, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(false, analyzingTextReader.LastCharactersReadWereLineBreak);

            Assert.AreEqual('\r', analyzingTextReader.Read());
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(1UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(LineBreakStyle.CarriageReturn, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(true, analyzingTextReader.LastCharactersReadWereLineBreak);

            Assert.AreEqual("", analyzingTextReader.ReadLine());
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.LineFeed));
            Assert.AreEqual(0UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturn));
            Assert.AreEqual(1UL, analyzingTextReader.GetLineBreakCount(LineBreakStyle.CarriageReturnLineFeed));
            Assert.AreEqual(LineBreakStyle.CarriageReturnLineFeed, analyzingTextReader.GetMostCommonLineBreakStyle());
            Assert.AreEqual(true, analyzingTextReader.LastCharactersReadWereLineBreak);
        }
    }
}
