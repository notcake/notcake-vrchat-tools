using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.Tests.Nodes.YamlScalarValidator
{
    using YamlScalarValidator = notcake.Unity.Yaml.Nodes.YamlScalarValidator;

    /// <summary>
    ///     Tests for <see cref="YamlScalarValidator.Folded"/> methods.
    /// </summary>
    [TestClass]
    public class FoldedTests : YamlScalarValidatorTests
    {
        /// <summary>
        ///     Tests the validity of folded scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="valid">
        ///     The expected validity of <paramref name="content"/> as a folded scalar.
        /// </param>
        [DataTestMethod]
        // [189] l-folded-content(n,t)
        // Folded scalars can be empty.
        [DataRow("",           true )]
        // [ 34] nb-char
        // Folded scalars can contain `[  1] c-printable`s.
        [DataRow("a\u0020a",   true )]
        [DataRow("a\u007Ea",   true )]
        [DataRow("a\u00A0a",   true )]
        [DataRow("a\uD7FFa",   true )]
        [DataRow("a\uE000a",   true )]
        [DataRow("a\uFFFDa",   true )]
        // [ 34] nb-char
        // Folded scalars cannot contain non-`[  1] c-printable`s.
        [DataRow("a\u0000a",   false)]
        [DataRow("a\u001Fa",   false)]
        [DataRow("a\u007Fa",   false)]
        [DataRow("a\u009Fa",   false)]
        [DataRow("a\uFFFEa",   false)]
        [DataRow("a\uFFFFa",   false)]
        // [ 34] nb-char
        // Folded scalars cannot contain `[ 27] b-char`s.
        [DataRow("a\ra",       false)]
        [DataRow("a\u0085a",   false)]
        // [ 84] b-l-folded-any(n,s)
        // Folded scalars can contain `[ 84] b-l-folded-any(n,s)` line breaks.
        [DataRow("a\u0009a",   true )]
        [DataRow("a\u000Aa",   true )]
        [DataRow("a\u000Ba",   false)]
        [DataRow("a\u2027a",   true )]
        [DataRow("a\u2028a",   true )]
        [DataRow("a\u2029a",   true )]
        [DataRow("a\u202Aa",   true )]
        // [ 84] b-l-folded-any(n,s)
        // Folded scalars can start or end with a `[ 84] b-l-folded-any(n,s)` line break.
        [DataRow("\na",        true )]
        [DataRow("\u2028a",    true )]
        [DataRow("\u2029a",    true )]
        [DataRow("a\n",        true )]
        [DataRow("a\u2028",    true )]
        [DataRow("a\u2029",    true )]
        // [ 77] s-ignored-prefix-block(n)
        // Folded scalars can contain a space directly after a `[ 84] b-l-folded-any(n,s)` line
        // break.
        [DataRow("a\n a",      true )]
        [DataRow("a\u2028 a",  true )]
        [DataRow("a\u2029 a",  true )]
        // [ 34] nb-char
        // Folded scalars can contain tabs.
        [DataRow("a\u0008a",   false)]
        [DataRow("a\u0009a",   true )]
        [DataRow("a\u000Aa",   true )]
        // [ 77] s-ignored-prefix-block(n)
        // Folded scalars can contain a tab directly after a `[ 84] b-l-folded-any(n,s)` line break.
        [DataRow("a\n\ta",     true )]
        [DataRow("a\u2028\ta", true )]
        [DataRow("a\u2029\ta", true )]
        // [ 79] s-ignored-prefix-block(n)
        // Folded scalars can start with a tab.
        [DataRow("\ta",        true )]
        // [184] s-nb-spaced-text(n)
        // Folded scalars can start with a space if a `[166] c-indentation-indicator(m)` is
        // present.
        [DataRow(" a",         true )]
        public void IsValid(string content, bool valid)
        {
            YamlNodeValidity yamlNodeValidity = YamlScalarValidator.Folded.IsValid(content);
            Assert.AreEqual(valid, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                valid ? YamlNodeValidity.Block() : YamlNodeValidity.None,
                yamlNodeValidity
            );
        }

        /// <summary>
        ///     Tests the validity of high and low surrogates in folded scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="valid">
        ///     The expected validity of <paramref name="content"/> as a folded scalar.
        /// </param>
        // [  1] c-printable
        // All high and low surrogates must be part of a surrogate pair.
        [DataTestMethod]
        [DataRow(new char[] { '\uD800', '\uDC00' }, true )]
        [DataRow(new char[] { '\uDBFF', '\uDFFF' }, true )]
        [DataRow(new char[] { '\uD800'           }, false)]
        [DataRow(new char[] { '\uD800', 'a'      }, false)]
        [DataRow(new char[] { '\uDBFF'           }, false)]
        [DataRow(new char[] { '\uDBFF', 'a'      }, false)]
        [DataRow(new char[] { '\uDC00'           }, false)]
        [DataRow(new char[] { 'a',      '\uDC00' }, false)]
        [DataRow(new char[] { '\uDFFF'           }, false)]
        [DataRow(new char[] { 'a',      '\uDFFF' }, false)]
        public void IsValidSurrogates(char[] content, bool valid)
        {
            YamlNodeValidity yamlNodeValidity =
                YamlScalarValidator.Folded.IsValid(new string(content));
            Assert.AreEqual(valid, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                valid ? YamlNodeValidity.Block() : YamlNodeValidity.None,
                yamlNodeValidity
            );
        }
    }
}
