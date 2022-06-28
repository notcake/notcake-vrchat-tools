using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Tests.YamlDocument
{
    using YamlDocument = notcake.Unity.Yaml.YamlDocument;

    /// <summary>
    ///     Tests for <see cref="YamlDocument"/>'s identification of styles during deserialization.
    /// </summary>
    [TestClass]
    public class StyleDeserializationTests
    {
        /// <summary>
        ///     Tests identification of line break styles.
        /// </summary>
        /// <param name="lineBreakStyle">
        ///     The <see cref="Style.LineBreakStyle"/> whose identification is to be tested.
        /// </param>
        [DataTestMethod]
        [DataRow(Style.LineBreakStyle.LineFeed              )]
        [DataRow(Style.LineBreakStyle.CarriageReturn        )]
        [DataRow(Style.LineBreakStyle.CarriageReturnLineFeed)]
        [DataRow(Style.LineBreakStyle.NextLine              )]
        [DataRow(Style.LineBreakStyle.LineSeparator         )]
        [DataRow(Style.LineBreakStyle.ParagraphSeparator    )]
        public void LineBreakStyle(LineBreakStyle lineBreakStyle)
        {
            string presentation = $"1{lineBreakStyle.GetString()}";
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            Assert.AreEqual(lineBreakStyle, yamlDocument.LineBreakStyle);
        }

        /// <summary>
        ///     Tests identification of line break styles when there are no line breaks present.
        /// </summary>
        [TestMethod]
        public void NoLineBreakStyle()
        {
            string presentation = $"1";
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            Assert.AreEqual(Style.LineBreakStyle.LineFeed, yamlDocument.LineBreakStyle);
        }

        /// <summary>
        ///     Tests detection of trailing line breaks at the end of a YAML stream.
        /// </summary>
        /// <param name="presentation">The YAML stream to test.</param>
        /// <param name="trailingLineBreak">
        ///     A boolean indicating whether a trailing line break is present in the YAML stream.
        /// </param>
        [DataTestMethod]
        [DataRow("1", false)]
        [DataRow("1\n", true)]
        public void TrailingLineBreak(string presentation, bool trailingLineBreak)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            Assert.AreEqual(trailingLineBreak, yamlDocument.TrailingLineBreak);
        }
    }
}
