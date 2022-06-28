using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.Yaml.Tests.Nodes.YamlScalarValidator
{
    using YamlScalarValidator = notcake.Unity.Yaml.Nodes.YamlScalarValidator;

    /// <summary>
    ///     Tests for <see cref="YamlScalarValidator"/> methods.
    /// </summary>
    [TestClass]
    public class Tests : YamlScalarValidatorTests
    {
        /// <summary>
        ///     Tests that
        ///     <see cref="YamlScalarValidator.IsFoldedLineBreakNonIntrinsic(char)"/> and
        ///     <see cref="YamlScalarValidator.IsFoldedLineBreakIntrinsic(char)"/> produce
        ///     the same output.
        /// </summary>
        [TestMethod]
        public void IsFoldedLineBreak()
        {
            this.AssertCharacterSetMembershipTestsEqual(
                YamlScalarValidator.IsFoldedLineBreakNonIntrinsic,
                YamlScalarValidator.IsFoldedLineBreakIntrinsic
            );
        }

        /// <summary>
        ///     Tests that
        ///     <see cref="YamlScalarValidator.IsNormalizedLineBreakNonIntrinsic(char)"/> and
        ///     <see cref="YamlScalarValidator.IsNormalizedLineBreakIntrinsic(char)"/> produce
        ///     the same output.
        /// </summary>
        [TestMethod]
        public void IsNormalizedLineBreak()
        {
            this.AssertCharacterSetMembershipTestsEqual(
                YamlScalarValidator.IsNormalizedLineBreakNonIntrinsic,
                YamlScalarValidator.IsNormalizedLineBreakIntrinsic
            );
        }

        /// <summary>
        ///     Tests that
        ///     <see cref="YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakNonIntrinsic(char)"/>
        ///     and
        ///     <see cref="YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakIntrinsic(char)"/>
        ///     produce the same output.
        /// </summary>
        [TestMethod]
        public void IsNonBreakingCharOrFoldedLineBreak()
        {
            this.AssertCharacterSetMembershipTestsEqual(
                YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakNonIntrinsic,
                YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakIntrinsic
            );
        }
    }
}
