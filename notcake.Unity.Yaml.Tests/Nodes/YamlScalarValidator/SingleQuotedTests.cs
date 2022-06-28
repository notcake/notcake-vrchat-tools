using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.Tests.Nodes.YamlScalarValidator
{
    using YamlScalarValidator = notcake.Unity.Yaml.Nodes.YamlScalarValidator;

    /// <summary>
    ///     Tests for <see cref="YamlScalarValidator.SingleQuoted"/> methods.
    /// </summary>
    [TestClass]
    public class SingleQuotedTests : YamlScalarValidatorTests
    {
        /// <summary>
        ///     Tests the validity of single quoted scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="validSomewhere">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar.
        /// </param>
        /// <param name="validAtRoot">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar at the
        ///     root of a YAML document.
        /// </param>
        /// <param name="validInFlowOut">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar in a
        ///     `flow-out` context.
        /// </param>
        /// <param name="validInFlowIn">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar in a
        ///     `flow-in` context.
        /// </param>
        /// <param name="validInFlowKey">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar in a
        ///     `flow-key` context.
        /// </param>
        [DataTestMethod]
        // [147] nb-single-single(n)
        // Single quoted scalars can be empty.
        [DataRow("",           true,  true,  true,  true,  true )]
        // [ 34] nb-char
        // Single quoted scalars can contain `[  1] c-printable`s.
        [DataRow("a\u0020a",   true,  true,  true,  true,  true )]
        [DataRow("a\u007Ea",   true,  true,  true,  true,  true )]
        [DataRow("a\u00A0a",   true,  true,  true,  true,  true )]
        [DataRow("a\uD7FFa",   true,  true,  true,  true,  true )]
        [DataRow("a\uE000a",   true,  true,  true,  true,  true )]
        [DataRow("a\uFFFDa",   true,  true,  true,  true,  true )]
        // [ 34] nb-char
        // Single quoted scalars cannot contain non-`[  1] c-printable`s.
        [DataRow("a\u0000a",   false, false, false, false, false)]
        [DataRow("a\u001Fa",   false, false, false, false, false)]
        [DataRow("a\u007Fa",   false, false, false, false, false)]
        [DataRow("a\u009Fa",   false, false, false, false, false)]
        [DataRow("a\uFFFEa",   false, false, false, false, false)]
        [DataRow("a\uFFFFa",   false, false, false, false, false)]
        // [ 34] nb-char
        // Single quoted scalars cannot contain `[ 27] b-char`s.
        [DataRow("a\ra",       false, false, false, false, false)]
        [DataRow("a\u0085a",   false, false, false, false, false)]
        // [ 84] b-l-folded-any(n,s)
        // Single quoted scalars can contain `[ 84] b-l-folded-any(n,s)` line breaks in a `flow-out`
        // or `flow-in` context.
        [DataRow("a\u0009a",   true,  true,  true,  true,  true )]
        [DataRow("a\u000Aa",   true,  true,  true,  true,  false)]
        [DataRow("a\u000Ba",   false, false, false, false, false)]
        [DataRow("a\u2027a",   true,  true,  true,  true,  true )]
        [DataRow("a\u2028a",   true,  true,  true,  true,  false)]
        [DataRow("a\u2029a",   true,  true,  true,  true,  false)]
        [DataRow("a\u202Aa",   true,  true,  true,  true,  true )]
        // [149] nb-single-multi(n)
        // Single quoted scalars can start or end with a `[ 84] b-l-folded-any(n,s)` line break in a
        // `flow-out` or `flow-in` context.
        [DataRow("\na",        true,  true,  true,  true,  false)]
        [DataRow("\u2028a",    true,  true,  true,  true,  false)]
        [DataRow("\u2029a",    true,  true,  true,  true,  false)]
        [DataRow("a\n",        true,  true,  true,  true,  false)]
        [DataRow("a\u2028",    true,  true,  true,  true,  false)]
        [DataRow("a\u2029",    true,  true,  true,  true,  false)]
        // [ 77] s-ignored-prefix-quoted(n)
        // Single quoted scalars cannot contain a space directly after a `[ 84] b-l-folded-any(n,s)`
        // line break.
        [DataRow("a\n a",      false, false, false, false, false)]
        [DataRow("a\u2028 a",  false, false, false, false, false)]
        [DataRow("a\u2029 a",  false, false, false, false, false)]
        // [ 34] nb-char
        // Single quoted scalars can contain tabs.
        [DataRow("a\u0008a",   false, false, false, false, false)]
        [DataRow("a\u0009a",   true,  true,  true,  true,  true )]
        [DataRow("a\u000Aa",   true,  true,  true,  true,  false)]
        // [ 77] s-ignored-prefix-quoted(n)
        // Single quoted scalars cannot contain a tab directly after a `[ 84] b-l-folded-any(n,s)`
        // line break.
        [DataRow("a\n\ta",     false, false, false, false, false)]
        [DataRow("a\u2028\ta", false, false, false, false, false)]
        [DataRow("a\u2029\ta", false, false, false, false, false)]
        // [ 34] nb-char
        // Single quoted scalars can contain backslashes and double quotes.
        [DataRow("a\\a",       true,  true,  true,  true,  true )]
        [DataRow("a\"a",       true,  true,  true,  true,  true )]
        // [142] nb-single-char
        // Single quoted scalars can contain single quotes.
        [DataRow("a'a",        true,  true,  true,  true,  true )]
        public void IsValid(
            string content,
            bool validSomewhere,
            bool validAtRoot,
            bool validInFlowOut,
            bool validInFlowIn,
            bool validInFlowKey
        )
        {
            YamlNodeValidity yamlNodeValidity = YamlScalarValidator.SingleQuoted.IsValid(content);
            Assert.AreEqual(validSomewhere, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                YamlNodeValidity.Flow(validAtRoot, validInFlowOut, validInFlowIn, validInFlowKey),
                yamlNodeValidity
            );
        }

        /// <summary>
        ///     Tests the validity of high and low surrogates in single quoted scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="valid">
        ///     The expected validity of <paramref name="content"/> as a single quoted scalar.
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
                YamlScalarValidator.SingleQuoted.IsValid(new string(content));
            Assert.AreEqual(valid, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                valid ? YamlNodeValidity.Flow() : YamlNodeValidity.None,
                yamlNodeValidity
            );
        }
    }
}
