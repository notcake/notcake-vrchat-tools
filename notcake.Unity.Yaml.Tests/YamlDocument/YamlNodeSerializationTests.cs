using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Tests.YamlDocument
{
    using YamlDocument = notcake.Unity.Yaml.YamlDocument;

    /// <summary>
    ///     Tests for <see cref="YamlDocument"/>'s serialization of
    ///     <see cref="YamlNode">YamlNodes</see>.
    /// </summary>
    [TestClass]
    public class YamlNodeSerializationTests
    {
        /// <summary>
        ///     Tests serialization of <c>null</c> values.
        /// </summary>
        [TestMethod]
        public void Null()
        {
            foreach (string presentation in YamlNull.Presentations)
            {
                YamlNull? yamlNull = YamlNull.FromPresentation(presentation);
                Assert.IsNotNull(yamlNull);

                YamlDocument yamlDocument = new(yamlNull, trailingLineBreak: false);
                Assert.AreEqual(presentation, yamlDocument.Serialize());
            }
        }

        /// <summary>
        ///     Tests serialization of boolean values.
        /// </summary>
        /// <param name="value">The boolean value whose presentations are to be tested.</param>
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Boolean(bool value)
        {
            IEnumerable<string> presentations = value ?
                YamlBoolean.TruePresentations :
                YamlBoolean.FalsePresentations;

            foreach (string presentation in presentations)
            {
                YamlBoolean? yamlBoolean = YamlBoolean.FromPresentation(presentation);
                Assert.IsNotNull(yamlBoolean);

                YamlDocument yamlDocument = new(yamlBoolean, trailingLineBreak: false);
                Assert.AreEqual(presentation, yamlDocument.Serialize());
            }
        }

        /// <summary>
        ///     Tests serialization of integer values.
        /// </summary>
        /// <param name="presentation">The presentation of the integer value.</param>
        [DataTestMethod]
        [DataRow("0")]
        [DataRow("+0")]
        [DataRow("-0")]
        [DataRow("1")]
        [DataRow("+1")]
        [DataRow("-1")]
        [DataRow("0b1")]
        [DataRow("01")]
        [DataRow("0x1")]
        [DataRow("1:0")]
        [DataRow("1_")]
        public void Integer(string presentation)
        {
            YamlInteger? yamlInteger = YamlInteger.FromPresentation1_1(presentation);
            Assert.IsNotNull(yamlInteger);

            YamlDocument yamlDocument = new(yamlInteger, trailingLineBreak: false);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of float values.
        /// </summary>
        /// <param name="presentation">The presentation of the float value.</param>
        [DataTestMethod]
        [DataRow("0.0")]
        [DataRow("+0.0")]
        [DataRow("-0.0")]
        [DataRow("1.0")]
        [DataRow("+1.0")]
        [DataRow("-1.0")]
        [DataRow("1.5")]
        [DataRow("1.")]
        [DataRow("1._")]
        [DataRow(".")]
        [DataRow("1.5e+1")]
        [DataRow("1.5e-1")]
        [DataRow("1.5E+1")]
        [DataRow("1:1.0")]
        [DataRow("1:1.5")]
        [DataRow(".inf")]
        [DataRow("+.inf")]
        [DataRow("-.inf")]
        [DataRow(".nan")]
        public void Float(string presentation)
        {
            YamlFloat? yamlFloat = YamlFloat.FromPresentation1_1(presentation);
            Assert.IsNotNull(yamlFloat);

            YamlDocument yamlDocument = new(yamlFloat, trailingLineBreak: false);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of string values.
        /// </summary>
        /// <param name="presentation">The expected presentation of the string value.</param>
        /// <param name="value">The string value whose presentation is to be tested.</param>
        /// <param name="presentationStyle">The presentation style of the string value.</param>
        [DataTestMethod]
        [DataRow("''",        "",     ScalarStyle.SingleQuoted)]
        [DataRow("\"\"",      "",     ScalarStyle.DoubleQuoted)]
        [DataRow("OK",        "OK",   ScalarStyle.Plain       )]
        [DataRow("'OK'",      "OK",   ScalarStyle.SingleQuoted)]
        [DataRow("\"OK\"",    "OK",   ScalarStyle.DoubleQuoted)]
        [DataRow("|\n  OK",   "OK",   ScalarStyle.Literal     )]
        [DataRow(">\n  OK",   "OK",   ScalarStyle.Folded      )]
        // Test chomping.
        [DataRow("|\n  OK\n", "OK\n", ScalarStyle.Literal     )]
        [DataRow(">\n  OK\n", "OK\n", ScalarStyle.Folded      )]
        // Test escapes.
        [DataRow("'\\'",      "\\",   ScalarStyle.SingleQuoted)]
        [DataRow("''''",      "'",    ScalarStyle.SingleQuoted)]
        [DataRow("\"\\\\\"",  "\\",   ScalarStyle.DoubleQuoted)]
        [DataRow("\"\\\"\"",  "\"",   ScalarStyle.DoubleQuoted)]
        // Test double quoted strings that would otherwise be other scalar types.
        [DataRow("\"null\"",  "null", ScalarStyle.DoubleQuoted)]
        [DataRow("\"true\"",  "true", ScalarStyle.DoubleQuoted)]
        [DataRow("\"false\"", "false",ScalarStyle.DoubleQuoted)]
        [DataRow("\"0\"",     "0",    ScalarStyle.DoubleQuoted)]
        [DataRow("\"0.0\"",   "0.0",  ScalarStyle.DoubleQuoted)]
        public void String(string presentation, string value, ScalarStyle presentationStyle)
        {
            YamlString? yamlString = YamlString.FromString(value, presentationStyle);
            if (yamlString == null) { throw new InvalidOperationException(); }

            YamlDocument yamlDocument = new(yamlString, trailingLineBreak: false);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of block string values.
        /// </summary>
        /// <param name="presentation">The expected presentation of the string value.</param>
        /// <param name="value">The string value whose presentation is to be tested.</param>
        /// <param name="presentationStyle">The presentation style of the string value.</param>
        /// <param name="trailingLineBreak">
        ///     A boolean indicating whether to serialize the YAML document with a trailing line
        ///     break.
        /// </param>
        [DataTestMethod]
        [DataRow("|\n  ",      "",      ScalarStyle.Literal, false)]
        [DataRow("|-\n  \n",   "",      ScalarStyle.Literal, true )]
        [DataRow("|\n  OK",    "OK",    ScalarStyle.Literal, false)]
        [DataRow("|-\n  OK\n", "OK",    ScalarStyle.Literal, true )]
        [DataRow("|\n  OK\n",  "OK\n",  ScalarStyle.Literal, false)]
        [DataRow("|\n  OK\n",  "OK\n",  ScalarStyle.Literal, true )]
        [DataRow(">\n  ",      "",      ScalarStyle.Folded,  false)]
        [DataRow(">-\n  \n",   "",      ScalarStyle.Folded,  true )]
        [DataRow(">\n  OK",    "OK",    ScalarStyle.Folded,  false)]
        [DataRow(">-\n  OK\n", "OK",    ScalarStyle.Folded,  true )]
        [DataRow(">\n  OK\n",  "OK\n",  ScalarStyle.Folded,  false)]
        [DataRow(">\n  OK\n",  "OK\n",  ScalarStyle.Folded,  true )]
        public void BlockString(
            string presentation,
            string value,
            ScalarStyle presentationStyle,
            bool trailingLineBreak
        )
        {
            YamlString? yamlString = YamlString.FromString(value, presentationStyle);
            if (yamlString == null) { throw new InvalidOperationException(); }

            YamlDocument yamlDocument = new(yamlString, trailingLineBreak: trailingLineBreak);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of an empty flow sequence.
        /// </summary>
        [TestMethod]
        public void EmptyFlowSequence()
        {
            YamlSequence yamlSequence = new(flow: true);

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: false);
            Assert.AreEqual("[]", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a flow sequence.
        /// </summary>
        [TestMethod]
        public void FlowSequence()
        {
            YamlSequence yamlSequence = new(flow: true)
            {
                new YamlInteger(1),
            };

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: false);
            Assert.AreEqual("[1]", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a block sequence.
        /// </summary>
        [TestMethod]
        public void BlockSequence()
        {
            YamlSequence yamlSequence = new(flow: false)
            {
                new YamlInteger(1),
            };

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: false);
            Assert.AreEqual("- 1", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a block sequence containing a single <c>null</c> value.
        /// </summary>
        [TestMethod]
        public void BlockSequenceContainingNull()
        {
            YamlSequence yamlSequence = new(flow: false)
            {
                YamlNull.FromEmptyPresentation()
            };

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: false);
            Assert.AreEqual("- ", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a block sequence containing two literal block strings with
        ///     the same value.
        /// </summary>
        /// <param name="presentation">The expected presentation of the block sequence.</param>
        /// <param name="value">The value of both strings.</param>
        /// <param name="trailingLineBreak">
        ///     A boolean indicating whether to serialize the YAML document with a trailing line
        ///     break.
        /// </param>
        [DataTestMethod]
        [DataRow("- |-\n  \n- |\n  ",      "",    false)]
        [DataRow("- |-\n  \n- |-\n  \n",   "",    true )]
        [DataRow("- |-\n  a\n- |\n  a",    "a",   false)]
        [DataRow("- |-\n  a\n- |-\n  a\n", "a",   true )]
        [DataRow("- |\n  a\n- |\n  a\n",   "a\n", false)]
        [DataRow("- |\n  a\n- |\n  a\n",   "a\n", true )]
        public void BlockSequenceContainingBlockString(
            string presentation,
            string value,
            bool trailingLineBreak
        )
        {
            YamlSequence yamlSequence = new(flow: false)
            {
                YamlString.FromString(value, ScalarStyle.Literal) ??
                    throw new InvalidOperationException(),
                YamlString.FromString(value, ScalarStyle.Literal) ??
                    throw new InvalidOperationException(),
            };

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: trailingLineBreak);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a block sequence containing a block sequence.
        /// </summary>
        [TestMethod]
        public void BlockSequenceContainingBlockSequence()
        {
            YamlSequence yamlSequence = new(flow: false)
            {
                new YamlSequence(flow: false)
                {
                    new YamlInteger(1),
                    new YamlInteger(2),
                },
                new YamlSequence(flow: false)
                {
                    new YamlInteger(3),
                    new YamlInteger(4),
                }
            };

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: false);
            Assert.AreEqual(
                "- - 1\n" +
                "  - 2\n" +
                "- - 3\n" +
                "  - 4",
                yamlDocument.Serialize()
            );
        }

        /// <summary>
        ///     Tests serialization of a block sequence containing a block mapping.
        /// </summary>
        [TestMethod]
        public void BlockSequenceContainingBlockMapping()
        {
            YamlSequence yamlSequence = new(flow: false)
            {
                new YamlMapping(flow: false)
                {
                    [
                        YamlString.FromString("b", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(1),
                    [
                        YamlString.FromString("c", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(2),
                },
                new YamlMapping(flow: false)
                {
                    [
                        YamlString.FromString("a", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(3),
                    [
                        YamlString.FromString("d", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(4),
                }
            };

            YamlDocument yamlDocument = new(yamlSequence, trailingLineBreak: false);
            Assert.AreEqual(
                "- b: 1\n" +
                "  c: 2\n" +
                "- a: 3\n" +
                "  d: 4",
                yamlDocument.Serialize()
            );
        }

        /// <summary>
        ///     Tests serialization of an empty flow mapping.
        /// </summary>
        [TestMethod]
        public void EmptyFlowMapping()
        {
            YamlMapping yamlMapping = new(flow: true);

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: false);
            Assert.AreEqual("{}", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a flow mapping.
        /// </summary>
        [TestMethod]
        public void FlowMapping()
        {
            YamlMapping yamlMapping = new(flow: true)
            {
                [
                    YamlString.FromString("a", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlInteger(1),
            };

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: false);
            Assert.AreEqual("{a: 1}", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a flow mapping containing a single <c>null</c> to <c>null</c>
        ///     entry.
        /// </summary>
        [TestMethod]
        public void FlowMappingContainingNull()
        {
            YamlMapping yamlMapping = new(flow: true)
            {
                [YamlNull.FromCanonicalPresentation()] = YamlNull.FromEmptyPresentation(),
            };

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: false);
            Assert.AreEqual("{~: }", yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a block mapping.
        /// </summary>
        [TestMethod]
        public void BlockMapping()
        {
            YamlMapping yamlMapping = new(flow: false)
            {
                [
                    YamlString.FromString("b", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlInteger(1),
                [
                    YamlString.FromString("c", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlInteger(2),
                [
                    YamlString.FromString("a", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlInteger(3),
                [
                    YamlString.FromString("d", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlInteger(4),
            };

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: false);
            Assert.AreEqual(
                "b: 1\n" +
                "c: 2\n" +
                "a: 3\n" +
                "d: 4",
                yamlDocument.Serialize()
            );
        }

        /// <summary>
        ///     Tests serialization of a block mapping containing two literal block strings with the
        ///     same value.
        /// </summary>
        /// <param name="presentation">The expected presentation of the block mapping.</param>
        /// <param name="value">The value of both strings.</param>
        /// <param name="trailingLineBreak">
        ///     A boolean indicating whether to serialize the YAML document with a trailing line
        ///     break.
        /// </param>
        [DataTestMethod]
        [DataRow("b: |-\n  \nc: |\n  ",      "",    false)]
        [DataRow("b: |-\n  \nc: |-\n  \n",   "",    true )]
        [DataRow("b: |-\n  a\nc: |\n  a",    "a",   false)]
        [DataRow("b: |-\n  a\nc: |-\n  a\n", "a",   true )]
        [DataRow("b: |\n  a\nc: |\n  a\n",   "a\n", false)]
        [DataRow("b: |\n  a\nc: |\n  a\n",   "a\n", true )]
        public void BlockMappingContainingBlockString(
            string presentation,
            string value,
            bool trailingLineBreak
        )
        {
            YamlMapping yamlMapping = new(flow: false)
            {
                [
                    YamlString.FromString("b", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = YamlString.FromString(value, ScalarStyle.Literal) ??
                    throw new InvalidOperationException(),
                [
                    YamlString.FromString("c", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = YamlString.FromString(value, ScalarStyle.Literal) ??
                    throw new InvalidOperationException(),
            };

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: trailingLineBreak);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests serialization of a block mapping containing a block sequence.
        /// </summary>
        [TestMethod]
        public void BlockMappingContainingBlockSequence()
        {
            YamlMapping yamlMapping = new(flow: false)
            {
                [
                    YamlString.FromString("b", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlSequence(flow: false)
                {
                    new YamlInteger(1),
                },
                [
                    YamlString.FromString("c", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlSequence(flow: false)
                {
                    new YamlInteger(2),
                },
            };

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: false);
            Assert.AreEqual(
                "b:\n" +
                "- 1\n" +
                "c:\n" +
                "- 2",
                yamlDocument.Serialize()
            );
        }

        /// <summary>
        ///     Tests serialization of a block mapping containing a block mapping.
        /// </summary>
        [TestMethod]
        public void BlockMappingContainingBlockMapping()
        {
            YamlMapping yamlMapping = new(flow: false)
            {
                [
                    YamlString.FromString("b", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlMapping(flow: false)
                {
                    [
                        YamlString.FromString("c", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(2),
                    [
                        YamlString.FromString("d", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(3),
                },
                [
                    YamlString.FromString("a", ScalarStyle.Plain) ??
                        throw new InvalidOperationException()
                ] = new YamlInteger(4),
            };

            YamlDocument yamlDocument = new(yamlMapping, trailingLineBreak: false);
            Assert.AreEqual(
                "b:\n" +
                "  c: 2\n" +
                "  d: 3\n" +
                "a: 4",
                yamlDocument.Serialize()
            );
        }
    }
}
