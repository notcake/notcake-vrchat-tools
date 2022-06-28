using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.Tests.Nodes.YamlScalarValidator
{
    using YamlScalarValidator = notcake.Unity.Yaml.Nodes.YamlScalarValidator;

    /// <summary>
    ///     Tests for <see cref="YamlScalarValidator.DoubleQuoted"/> methods.
    /// </summary>
    [TestClass]
    public class DoubleQuoted
    {
        /// <summary>
        ///     Tests the validity of double quoted scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="valid">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar.
        /// </param>
        [DataTestMethod]
        // [133] nb-double-single
        // Double quoted scalars can be empty.
        [DataRow("",       true)]
        // [ 34] nb-char
        // Double quoted scalars can contain `[  1] c-printable`s.
        [DataRow("\u0020", true)]
        [DataRow("\u007E", true)]
        [DataRow("\u00A0", true)]
        [DataRow("\uD7FF", true)]
        [DataRow("\uE000", true)]
        [DataRow("\uFFFD", true)]
        // [ 34] nb-char
        // Double quoted scalars can contain escaped non-`[  1] c-printable`s.
        [DataRow("\u0000", true)]
        [DataRow("\u001F", true)]
        [DataRow("\u007F", true)]
        [DataRow("\u009F", true)]
        [DataRow("\uFFFE", true)]
        [DataRow("\uFFFF", true)]
        // [ 65] ns-esc-char
        // Double quoted scalars can contain escaped code points.
        [DataRow("\0",     true)]
        [DataRow("\a",     true)]
        [DataRow("\b",     true)]
        [DataRow("\t",     true)]
        [DataRow("\n",     true)]
        [DataRow("\v",     true)]
        [DataRow("\f",     true)]
        [DataRow("\r",     true)]
        [DataRow("\u001B", true)]
        [DataRow(" ",      true)]
        [DataRow("\"",     true)]
        [DataRow("\\",     true)]
        [DataRow("\u0085", true)]
        [DataRow("\u00A0", true)]
        [DataRow("\u2028", true)]
        [DataRow("\u2029", true)]
        public void IsValid(string content, bool valid)
        {
            YamlNodeValidity yamlNodeValidity = YamlScalarValidator.DoubleQuoted.IsValid(content);
            Assert.AreEqual(valid, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                valid ? YamlNodeValidity.Flow() : YamlNodeValidity.None,
                yamlNodeValidity
            );
        }

        /// <summary>
        ///     Tests the validity of high and low surrogates in double quoted scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="valid">
        ///     The expected validity of <paramref name="content"/> as a double quoted scalar.
        /// </param>
        // Anything can be escaped, so anything goes.
        [DataTestMethod]
        [DataRow(new char[] { '\uD800', '\uDC00' }, true)]
        [DataRow(new char[] { '\uDBFF', '\uDFFF' }, true)]
        [DataRow(new char[] { '\uD800'           }, true)]
        [DataRow(new char[] { '\uD800', 'a'      }, true)]
        [DataRow(new char[] { '\uDBFF'           }, true)]
        [DataRow(new char[] { '\uDBFF', 'a'      }, true)]
        [DataRow(new char[] { '\uDC00'           }, true)]
        [DataRow(new char[] { 'a',      '\uDC00' }, true)]
        [DataRow(new char[] { '\uDFFF'           }, true)]
        [DataRow(new char[] { 'a',      '\uDFFF' }, true)]
        public void IsValidSurrogates(char[] content, bool valid)
        {
            YamlNodeValidity yamlNodeValidity =
                YamlScalarValidator.DoubleQuoted.IsValid(new string(content));
            Assert.AreEqual(valid, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                valid ? YamlNodeValidity.Flow() : YamlNodeValidity.None,
                yamlNodeValidity
            );
        }
    }
}
