using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.Yaml.Tests.Nodes.YamlScalarValidator
{
    /// <summary>
    ///     Provides utility methods for <see cref="YamlScalarValidator"/> tests.
    /// </summary>
    public class YamlScalarValidatorTests
    {
        /// <summary>
        ///     Asserts that two character set membership test methods produce the same results for
        ///     all possible <c>char</c> values.
        /// </summary>
        /// <param name="isInCharacterSet1">
        ///     The first character set membership test method to be tested.
        /// </param>
        /// <param name="isInCharacterSet1">
        ///     The second character set membership test method to be tested.
        /// </param>
        /// <exception cref="AssertFailedException">
        ///     Thrown when <paramref name="isInCharacterSet1"/> and
        ///     <paramref name="isInCharacterSet2"/> return different results for the same
        ///     <c>char</c>.
        /// </exception>
        public void AssertCharacterSetMembershipTestsEqual(
            Func<char, bool> isInCharacterSet1,
            Func<char, bool> isInCharacterSet2
        )
        {
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = (char)i;
                Assert.AreEqual(isInCharacterSet1(c), isInCharacterSet2(c));
            }
        }
    }
}
