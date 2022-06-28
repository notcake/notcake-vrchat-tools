using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.Tests.Nodes.YamlScalarValidator
{
    using YamlScalarValidator = notcake.Unity.Yaml.Nodes.YamlScalarValidator;

    /// <summary>
    ///     Tests for <see cref="YamlScalarValidator.Plain"/> methods.
    /// </summary>
    [TestClass]
    public class PlainTests : YamlScalarValidatorTests
    {
        /// <summary>
        ///     Tests the validity of plain scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="validSomewhere">
        ///     The expected validity of <paramref name="content"/> as a plain scalar.
        /// </param>
        /// <param name="validAtRoot">
        ///     The expected validity of <paramref name="content"/> as a plain scalar at the root
        ///     of a YAML document.
        /// </param>
        /// <param name="validInFlowOut">
        ///     The expected validity of <paramref name="content"/> as a plain scalar in a
        ///     `flow-out` context.
        /// </param>
        /// <param name="validInFlowIn">
        ///     The expected validity of <paramref name="content"/> as a plain scalar in a `flow-in`
        ///     context.
        /// </param>
        /// <param name="validInFlowKey">
        ///     The expected validity of <paramref name="content"/> as a plain scalar in a
        ///     `flow-key` context.
        /// </param>
        [DataTestMethod]
        // [160] ns-plain-single(c)
        // Plain scalars cannot be empty.
        [DataRow("",             false, false, false, false, false)]
        // [ 34] nb-char
        // Plain scalars can contain `[  1] c-printable`s.
        [DataRow("a\u0020a",     true,  true,  true,  true,  true )]
        [DataRow("a\u007Ea",     true,  true,  true,  true,  true )]
        [DataRow("a\u00A0a",     true,  true,  true,  true,  true )]
        [DataRow("a\uD7FFa",     true,  true,  true,  true,  true )]
        [DataRow("a\uE000a",     true,  true,  true,  true,  true )]
        [DataRow("a\uFFFDa",     true,  true,  true,  true,  true )]
        // [ 34] nb-char
        // Plain scalars cannot contain non-`[  1] c-printable`s.
        [DataRow("a\u0000a",     false, false, false, false, false)]
        [DataRow("a\u001Fa",     false, false, false, false, false)]
        [DataRow("a\u007Fa",     false, false, false, false, false)]
        [DataRow("a\u009Fa",     false, false, false, false, false)]
        [DataRow("a\uFFFEa",     false, false, false, false, false)]
        [DataRow("a\uFFFFa",     false, false, false, false, false)]
        // [ 34] nb-char
        // Plain scalars cannot contain `[ 27] b-char`s.
        [DataRow("a\ra",         false, false, false, false, false)]
        [DataRow("a\u0085a",     false, false, false, false, false)]
        // [ 84] b-l-folded-any(n,s)
        // Plain scalars can contain `[ 84] b-l-folded-any(n,s)` line breaks in a `flow-out` or
        // `flow-in` context.
        [DataRow("a\u0009a",     false, false, false, false, false)]
        [DataRow("a\u000Aa",     true,  true,  true,  true,  false)]
        [DataRow("a\u000Ba",     false, false, false, false, false)]
        [DataRow("a\u2027a",     true,  true,  true,  true,  true )]
        [DataRow("a\u2028a",     true,  true,  true,  true,  false)]
        [DataRow("a\u2029a",     true,  true,  true,  true,  false)]
        [DataRow("a\u202Aa",     true,  true,  true,  true,  true )]
        // [160] ns-plain-single(c)
        // Plain scalars cannot start or end with a `[ 84] b-l-folded-any(n,s)` line break.
        [DataRow("\na",          false, false, false, false, false)]
        [DataRow("\u2028a",      false, false, false, false, false)]
        [DataRow("\u2029a",      false, false, false, false, false)]
        [DataRow("a\n",          false, false, false, false, false)]
        [DataRow("a\u2028",      false, false, false, false, false)]
        [DataRow("a\u2029",      false, false, false, false, false)]
        // [ 77] s-ignored-prefix-plain(n)
        // Plain scalars cannot contain a space directly after a `[ 84] b-l-folded-any(n,s)` line
        // break.
        [DataRow("a\n a",        false, false, false, false, false)]
        [DataRow("a\u2028 a",    false, false, false, false, false)]
        [DataRow("a\u2029 a",    false, false, false, false, false)]
        // [154] nb-plain-char-out
        // Plain scalars cannot contain tabs.
        [DataRow("a\u0008a",     false, false, false, false, false)]
        [DataRow("a\u0009a",     false, false, false, false, false)]
        [DataRow("a\u000Aa",     true,  true,  true,  true,  false)]
        // [154] nb-plain-char-out
        // Plain scalars cannot contain tabs.
        [DataRow("a\n\ta",       false, false, false, false, false)]
        [DataRow("a\u2028\ta",   false, false, false, false, false)]
        [DataRow("a\u2029\ta",   false, false, false, false, false)]
        // [154] nb-plain-char-out
        // Plain scalars cannot contain '#'s not preceded by a non-space.
        [DataRow("#a",           false, false, false, false, false)]
        [DataRow("a#a",          true,  true,  true,  true,  true )]
        [DataRow("a #a",         false, false, false, false, false)]
        // [154] nb-plain-char-out
        // Plain scalars cannot contain ':'s not followed by a non-space.
        [DataRow("a:",           false, false, false, false, false)]
        [DataRow("a:a",          true,  true,  true,  true,  true )]
        [DataRow("a: a",         false, false, false, false, false)]
        // [155] nb-plain-char-in
        // Plain scalars cannot contain ',', '[', ']', '{' or '}' in the `flow-in` and `flow-key`
        // contexts.
        [DataRow("a,a",          true,  true,  true,  false, false)]
        [DataRow("a[a",          true,  true,  true,  false, false)]
        [DataRow("a]a",          true,  true,  true,  false, false)]
        [DataRow("a{a",          true,  true,  true,  false, false)]
        [DataRow("a}a",          true,  true,  true,  false, false)]
        // [ 84] b-l-folded-any(n,s) and [155] nb-plain-char-in
        // Plain scalars can contain `[ 84] b-l-folded-any(n,s)` line breaks in a `flow-out` or
        // `flow-in` context.
        // Plain scalars cannot contain ',', '[', ']', '{' or '}' in the `flow-in` and `flow-key`
        // contexts.
        [DataRow("a\n,a",        true,  true,  true,  false, false)]
        [DataRow("a\u2028,a",    true,  true,  true,  false, false)]
        [DataRow("a\u2029,a",    true,  true,  true,  false, false)]
        [DataRow("a\n[a",        true,  true,  true,  false, false)]
        [DataRow("a\u2028[a",    true,  true,  true,  false, false)]
        [DataRow("a\u2029[a",    true,  true,  true,  false, false)]
        [DataRow("a\n]a",        true,  true,  true,  false, false)]
        [DataRow("a\u2028]a",    true,  true,  true,  false, false)]
        [DataRow("a\u2029]a",    true,  true,  true,  false, false)]
        [DataRow("a\n{a",        true,  true,  true,  false, false)]
        [DataRow("a\u2028{a",    true,  true,  true,  false, false)]
        [DataRow("a\u2029{a",    true,  true,  true,  false, false)]
        [DataRow("a\n}a",        true,  true,  true,  false, false)]
        [DataRow("a\u2028}a",    true,  true,  true,  false, false)]
        [DataRow("a\u2029}a",    true,  true,  true,  false, false)]
        // [157] ns-plain-first-char(c)
        // Plain scalars cannot start with an indicator, unless it is '-', '?' or ':' followed by a
        // non-space.
        [DataRow("-",            false, false, false, false, false)]
        [DataRow("?",            false, false, false, false, false)]
        [DataRow(":",            false, false, false, false, false)]
        [DataRow(",",            false, false, false, false, false)]
        [DataRow("[",            false, false, false, false, false)]
        [DataRow("]",            false, false, false, false, false)]
        [DataRow("{",            false, false, false, false, false)]
        [DataRow("}",            false, false, false, false, false)]
        [DataRow("#",            false, false, false, false, false)]
        [DataRow("&",            false, false, false, false, false)]
        [DataRow("*",            false, false, false, false, false)]
        [DataRow("!",            false, false, false, false, false)]
        [DataRow("|",            false, false, false, false, false)]
        [DataRow(">",            false, false, false, false, false)]
        [DataRow("'",            false, false, false, false, false)]
        [DataRow("\"",           false, false, false, false, false)]
        [DataRow("%",            false, false, false, false, false)]
        [DataRow("@",            false, false, false, false, false)]
        [DataRow("`",            false, false, false, false, false)]
        [DataRow("- a",          false, false, false, false, false)]
        [DataRow("? a",          false, false, false, false, false)]
        [DataRow(": a",          false, false, false, false, false)]
        [DataRow(", a",          false, false, false, false, false)]
        [DataRow("[ a",          false, false, false, false, false)]
        [DataRow("] a",          false, false, false, false, false)]
        [DataRow("{ a",          false, false, false, false, false)]
        [DataRow("} a",          false, false, false, false, false)]
        [DataRow("# a",          false, false, false, false, false)]
        [DataRow("& a",          false, false, false, false, false)]
        [DataRow("* a",          false, false, false, false, false)]
        [DataRow("! a",          false, false, false, false, false)]
        [DataRow("| a",          false, false, false, false, false)]
        [DataRow("> a",          false, false, false, false, false)]
        [DataRow("' a",          false, false, false, false, false)]
        [DataRow("\" a",         false, false, false, false, false)]
        [DataRow("% a",          false, false, false, false, false)]
        [DataRow("@ a",          false, false, false, false, false)]
        [DataRow("` a",          false, false, false, false, false)]
        [DataRow("-a",           true,  true,  true,  true,  true )]
        [DataRow("?a",           true,  true,  true,  true,  true )]
        [DataRow(":a",           true,  true,  true,  true,  true )]
        [DataRow(",a",           false, false, false, false, false)]
        [DataRow("[a",           false, false, false, false, false)]
        [DataRow("]a",           false, false, false, false, false)]
        [DataRow("{a",           false, false, false, false, false)]
        [DataRow("}a",           false, false, false, false, false)]
        [DataRow("#a",           false, false, false, false, false)]
        [DataRow("&a",           false, false, false, false, false)]
        [DataRow("*a",           false, false, false, false, false)]
        [DataRow("!a",           false, false, false, false, false)]
        [DataRow("|a",           false, false, false, false, false)]
        [DataRow(">a",           false, false, false, false, false)]
        [DataRow("'a",           false, false, false, false, false)]
        [DataRow("\"a",          false, false, false, false, false)]
        [DataRow("%a",           false, false, false, false, false)]
        [DataRow("@a",           false, false, false, false, false)]
        [DataRow("`a",           false, false, false, false, false)]
        // [159] l-forbidden-content
        // Plain scalars at the start of a line cannot start with a "---" or "..." unless it is
        // followed by a non-space.
        [DataRow("---",          true,  false, true,  true,  true )]
        [DataRow("...",          true,  false, true,  true,  true )]
        [DataRow("--- a",        true,  false, true,  true,  true )]
        [DataRow("... a",        true,  false, true,  true,  true )]
        [DataRow("---a",         true,  true,  true,  true,  true )]
        [DataRow("...a",         true,  true,  true,  true,  true )]
        // [160] ns-plain-single(c)
        // Plain scalars cannot start or end with a space.
        [DataRow("a",            true,  true,  true,  true,  true )]
        [DataRow("a ",           false, false, false, false, false)]
        [DataRow(" a",           false, false, false, false, false)]
        [DataRow(" ",            false, false, false, false, false)]
        public void IsValid(
            string content,
            bool validSomewhere,
            bool validAtRoot,
            bool validInFlowOut,
            bool validInFlowIn,
            bool validInFlowKey
        )
        {
            YamlNodeValidity yamlNodeValidity = YamlScalarValidator.Plain.IsValid(content);
            Assert.AreEqual(validSomewhere, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                YamlNodeValidity.Flow(validAtRoot, validInFlowOut, validInFlowIn, validInFlowKey),
                yamlNodeValidity
            );
        }

        /// <summary>
        ///     Tests the validity of high and low surrogates in plain scalars.
        /// </summary>
        /// <param name="content">The content of the scalar node.</param>
        /// <param name="valid">
        ///     The expected validity of <paramref name="content"/> as a plain scalar.
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
                YamlScalarValidator.Plain.IsValid(new string(content));
            Assert.AreEqual(valid, yamlNodeValidity.Somewhere);
            Assert.AreEqual(
                valid ? YamlNodeValidity.Flow() : YamlNodeValidity.None,
                yamlNodeValidity
            );
        }

        /// <summary>
        ///     Tests that
        ///     <see cref="YamlScalarValidator.Plain.IsIndicatorNonIntrinsic(char)"/> and
        ///     <see cref="YamlScalarValidator.Plain.IsIndicatorIntrinsic(char)"/> produce the same
        ///     output.
        /// </summary>
        [TestMethod]
        public void IsIndicator()
        {
            this.AssertCharacterSetMembershipTestsEqual(
                YamlScalarValidator.Plain.IsIndicatorNonIntrinsic,
                YamlScalarValidator.Plain.IsIndicatorIntrinsic
            );
        }

        /// <summary>
        ///     Tests that
        ///     <see cref="YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharNonIntrinsic(char)"/>
        ///     and
        ///     <see cref="YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharIntrinsic(char)"/>
        ///     produce the same output.
        /// </summary>
        [TestMethod]
        public void IsValidCharOutOrMultilineChar()
        {
            this.AssertCharacterSetMembershipTestsEqual(
                YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharNonIntrinsic,
                YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharIntrinsic
            );
        }

        /// <summary>
        ///     Tests that
        ///     <see cref="YamlScalarValidator.Plain.IsCharOutValidCharInNonIntrinsic(char)"/> and
        ///     <see cref="YamlScalarValidator.Plain.IsCharOutValidCharInIntrinsic(char)"/> produce
        ///     the same output.
        /// </summary>
        [TestMethod]
        public void IsCharOutValidCharIn()
        {
            this.AssertCharacterSetMembershipTestsEqual(
                YamlScalarValidator.Plain.IsCharOutValidCharInNonIntrinsic,
                YamlScalarValidator.Plain.IsCharOutValidCharInIntrinsic
            );
        }
    }
}
