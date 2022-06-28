using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Tests.YamlDocument
{
    using YamlDocument = notcake.Unity.Yaml.YamlDocument;

    /// <summary>
    ///     Tests for <see cref="YamlDocument"/>'s deserialization of
    ///     <see cref="YamlNode">YamlNodes</see>.
    /// </summary>
    [TestClass]
    public class YamlNodeDeserializationTests
    {
        /// <summary>
        ///     Tests deserialization of <c>null</c> values.
        /// </summary>
        [TestMethod]
        public void Null()
        {
            foreach (string presentation in YamlNull.Presentations)
            {
                YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
                YamlNode yamlNode = yamlDocument.RootNode;
                Assert.IsTrue(
                    YamlNull.FromPresentation(presentation)?.PresentationEquals(yamlNode)
                );

                YamlNull? yamlNull = yamlNode as YamlNull;
                Assert.IsNotNull(yamlNull);
                Assert.AreEqual(presentation, yamlNull.Presentation);
            }
        }

        /// <summary>
        ///     Tests deserialization of strings that are almost valid <c>null</c> values.
        /// </summary>
        /// <param name="presentation">The presentation of the string value.</param>
        [DataTestMethod]
        [DataRow("nULL")]
        [DataRow("NulL")]
        public void NotNull(string presentation)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                YamlString.FromString(presentation, ScalarStyle.Plain)?.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of boolean values.
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
                YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
                YamlNode yamlNode = yamlDocument.RootNode;
                Assert.IsTrue(
                    YamlBoolean.FromPresentation(presentation)?.PresentationEquals(yamlNode)
                );

                YamlBoolean? yamlBoolean = yamlNode as YamlBoolean;
                Assert.IsNotNull(yamlBoolean);
                Assert.AreEqual(presentation, yamlBoolean.Presentation);
                Assert.AreEqual(value, yamlBoolean.Value);
            }
        }

        /// <summary>
        ///     Tests deserialization of strings that are incorrectly capitalized boolean values.
        /// </summary>
        /// <param name="presentation">The presentation of the string value.</param>
        [DataTestMethod]
        [DataRow("nO")]
        [DataRow("fALSE")]
        [DataRow("FalsE")]
        [DataRow("oFF")]
        [DataRow("OfF")]
        [DataRow("yES")]
        [DataRow("YeS")]
        [DataRow("tRUE")]
        [DataRow("TruE")]
        [DataRow("oN")]
        public void NotBoolean(string presentation)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                YamlString.FromString(presentation, ScalarStyle.Plain)?.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of integer values.
        /// </summary>
        /// <param name="presentation">The presentation of the integer value.</param>
        /// <param name="int64Value">The expected integer value, as a signed 64-bit integer.</param>
        /// <param name="uint64Value">
        ///     The expected integer value, as an unsigned 64-bit integer.
        /// </param>
        [DataTestMethod]
        // Test zero.
        [DataRow( "0",                                           0L,                   0UL)]
        [DataRow("+0",                                           0L,                   0UL)]
        [DataRow("-0",                                           0L,                   0UL)]
        // Test signs.
        [DataRow( "1",                                           1L,                   1UL)]
        [DataRow("+1",                                           1L,                   1UL)]
        [DataRow("-1",                                          -1L,                  null)]
        // Test base 2.
        [DataRow( "0b10",                                        2L,                   2UL)]
        [DataRow("+0b10",                                        2L,                   2UL)]
        [DataRow("-0b10",                                       -2L,                  null)]
        [DataRow( "0b11",                                        3L,                   3UL)]
        [DataRow("+0b11",                                        3L,                   3UL)]
        [DataRow("-0b11",                                       -3L,                  null)]
        // Test base 8.
        [DataRow( "012",                                        10L,                  10UL)]
        [DataRow("+012",                                        10L,                  10UL)]
        [DataRow("-012",                                       -10L,                  null)]
        // Test base 10.
        [DataRow( "12",                                         12L,                  12UL)]
        [DataRow("+12",                                         12L,                  12UL)]
        [DataRow("-12",                                        -12L,                  null)]
        // Test base 16.
        [DataRow( "0x12",                                       18L,                  18UL)]
        [DataRow("+0x12",                                       18L,                  18UL)]
        [DataRow("-0x12",                                      -18L,                  null)]
        [DataRow( "0xff",                                      255L,                 255UL)]
        [DataRow("+0xff",                                      255L,                 255UL)]
        [DataRow("-0xff",                                     -255L,                  null)]
        [DataRow( "0xFF",                                      255L,                 255UL)]
        [DataRow("+0xFF",                                      255L,                 255UL)]
        [DataRow("-0xFF",                                     -255L,                  null)]
        // Test base 60.
        [DataRow( "1:0",                                        60L,                  60UL)]
        [DataRow("+1:0",                                        60L,                  60UL)]
        [DataRow("-1:0",                                       -60L,                  null)]
        [DataRow( "1:00",                                       60L,                  60UL)]
        [DataRow("+1:00",                                       60L,                  60UL)]
        [DataRow("-1:00",                                      -60L,                  null)]
        [DataRow( "1:1",                                        61L,                  61UL)]
        [DataRow("+1:1",                                        61L,                  61UL)]
        [DataRow("-1:1",                                       -61L,                  null)]
        [DataRow( "1:9",                                        69L,                  69UL)]
        [DataRow("+1:9",                                        69L,                  69UL)]
        [DataRow("-1:9",                                       -69L,                  null)]
        [DataRow( "1:59",                                      119L,                 119UL)]
        [DataRow("+1:59",                                      119L,                 119UL)]
        [DataRow("-1:59",                                     -119L,                  null)]
        [DataRow( "1:0:0",                                    3600L,                3600UL)]
        [DataRow("+1:0:0",                                    3600L,                3600UL)]
        [DataRow("-1:0:0",                                   -3600L,                  null)]
        // Test only underscores.
        [DataRow( "0b_",                                         0L,                   0UL)]
        [DataRow("+0b_",                                         0L,                   0UL)]
        [DataRow("-0b_",                                         0L,                   0UL)]
        [DataRow( "0_",                                          0L,                   0UL)]
        [DataRow("+0_",                                          0L,                   0UL)]
        [DataRow("-0_",                                          0L,                   0UL)]
        [DataRow( "0x_",                                         0L,                   0UL)]
        [DataRow("+0x_",                                         0L,                   0UL)]
        [DataRow("-0x_",                                         0L,                   0UL)]
        // Test trailing underscores.
        [DataRow( "0b1_",                                        1L,                   1UL)]
        [DataRow("+0b1_",                                        1L,                   1UL)]
        [DataRow("-0b1_",                                       -1L,                  null)]
        [DataRow( "01_",                                         1L,                   1UL)]
        [DataRow("+01_",                                         1L,                   1UL)]
        [DataRow("-01_",                                        -1L,                  null)]
        [DataRow( "1_",                                          1L,                   1UL)]
        [DataRow("+1_",                                          1L,                   1UL)]
        [DataRow("-1_",                                         -1L,                  null)]
        [DataRow( "0x1_",                                        1L,                   1UL)]
        [DataRow("+0x1_",                                        1L,                   1UL)]
        [DataRow("-0x1_",                                       -1L,                  null)]
        // Test (u)int64 limits.
        [DataRow( "0b0000000000000000000000000000000000000000000000000000000000000000",   0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("+0b0000000000000000000000000000000000000000000000000000000000000000",   0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("-0b0000000000000000000000000000000000000000000000000000000000000000",   0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow( "0b0111111111111111111111111111111111111111111111111111111111111111",   0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("+0b0111111111111111111111111111111111111111111111111111111111111111",   0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("-0b0111111111111111111111111111111111111111111111111111111111111111",  -0x7FFFFFFF_FFFFFFFFL,                  null)]
        [DataRow( "0b1000000000000000000000000000000000000000000000000000000000000000",                   null, 0x80000000_00000000UL)]
        [DataRow("+0b1000000000000000000000000000000000000000000000000000000000000000",                   null, 0x80000000_00000000UL)]
        [DataRow("-0b1000000000000000000000000000000000000000000000000000000000000000",  -0x80000000_00000000L,                  null)]
        [DataRow( "0b1111111111111111111111111111111111111111111111111111111111111111",                   null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("+0b1111111111111111111111111111111111111111111111111111111111111111",                   null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("-0b1111111111111111111111111111111111111111111111111111111111111111",                   null,                  null)]
        [DataRow( "0b00000000000000000000000000000000000000000000000000000000000000000",  0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("+0b00000000000000000000000000000000000000000000000000000000000000000",  0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("-0b00000000000000000000000000000000000000000000000000000000000000000",  0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow( "0b10000000000000000000000000000000000000000000000000000000000000000",                  null,                  null)]
        [DataRow("+0b10000000000000000000000000000000000000000000000000000000000000000",                  null,                  null)]
        [DataRow("-0b10000000000000000000000000000000000000000000000000000000000000000",                  null,                  null)]
        [DataRow( "00000000000000000000000",   0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("+00000000000000000000000",   0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("-00000000000000000000000",   0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow( "00777777777777777777777",   0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("+00777777777777777777777",   0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("-00777777777777777777777",  -0x7FFFFFFF_FFFFFFFFL,                  null)]
        [DataRow( "01000000000000000000000",                   null, 0x80000000_00000000UL)]
        [DataRow("+01000000000000000000000",                   null, 0x80000000_00000000UL)]
        [DataRow("-01000000000000000000000",  -0x80000000_00000000L,                  null)]
        [DataRow( "01777777777777777777777",                   null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("+01777777777777777777777",                   null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("-01777777777777777777777",                   null,                  null)]
        [DataRow( "000000000000000000000000",  0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("+000000000000000000000000",  0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("-000000000000000000000000",  0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow( "020000000000000000000000",                  null,                  null)]
        [DataRow("+020000000000000000000000",                  null,                  null)]
        [DataRow("-020000000000000000000000",                  null,                  null)]
        [DataRow( "9223372036854775807",       0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("+9223372036854775807",       0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("-9223372036854775807",      -0x7FFFFFFF_FFFFFFFFL,                  null)]
        [DataRow( "9223372036854775808",                       null, 0x80000000_00000000UL)]
        [DataRow("+9223372036854775808",                       null, 0x80000000_00000000UL)]
        [DataRow("-9223372036854775808",      -0x80000000_00000000L,                  null)]
        [DataRow( "18446744073709551615",                      null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("+18446744073709551615",                      null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("-18446744073709551615",                      null,                  null)]
        [DataRow( "18446744073709551616",                      null,                  null)]
        [DataRow("+18446744073709551616",                      null,                  null)]
        [DataRow("-18446744073709551616",                      null,                  null)]
        [DataRow( "0x00000000_00000000",       0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("+0x00000000_00000000",       0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("-0x00000000_00000000",       0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow( "0x7FFFFFFF_FFFFFFFF",       0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("+0x7FFFFFFF_FFFFFFFF",       0x7FFFFFFF_FFFFFFFFL, 0x7FFFFFFF_FFFFFFFFUL)]
        [DataRow("-0x7FFFFFFF_FFFFFFFF",      -0x7FFFFFFF_FFFFFFFFL,                  null)]
        [DataRow( "0x80000000_00000000",                       null, 0x80000000_00000000UL)]
        [DataRow("+0x80000000_00000000",                       null, 0x80000000_00000000UL)]
        [DataRow("-0x80000000_00000000",      -0x80000000_00000000L,                  null)]
        [DataRow( "0xFFFFFFFF_FFFFFFFF",                       null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("+0xFFFFFFFF_FFFFFFFF",                       null, 0xFFFFFFFF_FFFFFFFFUL)]
        [DataRow("-0xFFFFFFFF_FFFFFFFF",                       null,                  null)]
        [DataRow( "0x0_00000000_00000000",     0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("+0x0_00000000_00000000",     0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow("-0x0_00000000_00000000",     0x00000000_00000000L, 0x00000000_00000000UL)]
        [DataRow( "0x1_00000000_00000000",                     null,                  null)]
        [DataRow("+0x1_00000000_00000000",                     null,                  null)]
        [DataRow("-0x1_00000000_00000000",                     null,                  null)]
        public void Integer(string presentation, long? int64Value, ulong? uint64Value)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(YamlInteger.FromPresentation1_1(presentation)?.PresentationEquals(yamlNode));

            YamlInteger? yamlInteger = yamlNode as YamlInteger;
            Assert.IsNotNull(yamlInteger);
            Assert.AreEqual(presentation, yamlInteger.Presentation);
            Assert.AreEqual(int64Value, yamlInteger.Int64Value);
            Assert.AreEqual(uint64Value, yamlInteger.UInt64Value);
        }

        /// <summary>
        ///     Tests deserialization of strings that are almost valid integer values.
        /// </summary>
        /// <param name="presentation">The presentation of the string value.</param>
        [DataTestMethod]
        // Test uppercase base character.
        [DataRow("0B0")]
        [DataRow("0X0")]
        // Test missing digits.
        [DataRow("+")]
        // "-" would be a sequence containing a single `null`.
        [DataRow("0b")]
        [DataRow("0x")]
        // Test underscores.
        [DataRow("_")]
        [DataRow("+_")]
        [DataRow("-_")]
        // Test junk after the end.
        [DataRow("0b1z")]
        [DataRow("07z")]
        [DataRow("0x1z")]
        [DataRow("1:1z")]
        // Test out-of-range base character.
        [DataRow("0b2")]
        [DataRow("08")]
        [DataRow("0xg")]
        [DataRow("0xG")]
        [DataRow("1:60")]
        // Test unacceptable base 60 integers.
        [DataRow(":0")]
        [DataRow("0:0")]
        [DataRow("1:z")]
        [DataRow("1:000")]
        [DataRow("1:-60")]
        [DataRow("1::0")]
        public void NotInteger(string presentation)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                YamlString.FromString(presentation, ScalarStyle.Plain)?.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of float values.
        /// </summary>
        /// <param name="presentation">The presentation of the float value.</param>
        /// <param name="value">The expected float value, as a double.</param>
        [DataTestMethod]
        // Test zero.
        [DataRow( "0.0",      0.0 )]
        [DataRow("+0.0",      0.0 )]
        [DataRow("-0.0",     -0.0 )]
        // Test signs.
        [DataRow( "1.0",      1.0 )]
        [DataRow("+1.0",      1.0 )]
        [DataRow("-1.0",     -1.0 )]
        // Test base 10 with fractional part.
        [DataRow( "1.5",      1.5 )]
        [DataRow("+1.5",      1.5 )]
        [DataRow("-1.5",     -1.5 )]
        // Test base 10 with no fractional part.
        [DataRow( "1.",       1.0 )]
        [DataRow("+1.",       1.0 )]
        [DataRow("-1.",      -1.0 )]
        // Test base 10 with underscore fractional part.
        [DataRow( "1._",      1.0 )]
        [DataRow("+1._",      1.0 )]
        [DataRow("-1._",     -1.0 )]
        // Test base 10 with no integer part.
        [DataRow( ".0",       0.0 )]
        [DataRow("+.0",       0.0 )]
        [DataRow("-.0",      -0.0 )]
        // Test base 10 with no integer or fractional part.
        [DataRow( ".",        0.0 )]
        [DataRow("+.",        0.0 )]
        [DataRow("-.",       -0.0 )]
        // Test positive exponent.
        [DataRow( "1.5e+1",  15.0 )]
        [DataRow("+1.5e+1",  15.0 )]
        [DataRow("-1.5e+1", -15.0 )]
        // Test negative exponent.
        [DataRow( "1.5e-1",   0.15)]
        [DataRow("+1.5e-1",   0.15)]
        [DataRow("-1.5e-1",  -0.15)]
        // Test uppercase 'E'.
        [DataRow( "1.5E+1",  15.0 )]
        [DataRow("+1.5E+1",  15.0 )]
        [DataRow("-1.5E+1", -15.0 )]
        // Test base 60.
        [DataRow( "1:1.0",   61.0 )]
        [DataRow("+1:1.0",   61.0 )]
        [DataRow("-1:1.0",  -61.0 )]
        // Test base 60 with fractional part.
        [DataRow( "1:1.5",   61.5 )]
        [DataRow("+1:1.5",   61.5 )]
        [DataRow("-1:1.5",  -61.5 )]
        // Test base 60 with no fractional part.
        [DataRow( "1:1.",    61.0 )]
        [DataRow("+1:1.",    61.0 )]
        [DataRow("-1:1.",   -61.0 )]
        // Test base 60 with underscore fractional part.
        [DataRow( "1:1._",   61.0 )]
        [DataRow("+1:1._",   61.0 )]
        [DataRow("-1:1._",  -61.0 )]
        // Test infinities.
        [DataRow( ".inf", double.PositiveInfinity)]
        [DataRow("+.inf", double.PositiveInfinity)]
        [DataRow("-.inf", double.NegativeInfinity)]
        [DataRow( ".Inf", double.PositiveInfinity)]
        [DataRow("+.Inf", double.PositiveInfinity)]
        [DataRow("-.Inf", double.NegativeInfinity)]
        [DataRow( ".INF", double.PositiveInfinity)]
        [DataRow("+.INF", double.PositiveInfinity)]
        [DataRow("-.INF", double.NegativeInfinity)]
        // Test NaNs.
        [DataRow( ".nan", double.NaN)]
        [DataRow( ".NaN", double.NaN)]
        [DataRow( ".NAN", double.NaN)]
        // Test closest double.
        [DataRow("1.00000000000000000000000000000000000000000000000000000",   1.0000000000000000)]
        [DataRow("1.00000000000000011102230246251565404236316680908203125",   1.0000000000000000)]
        [DataRow("1.00000000000000011102230246251565404236316680908203126",   1.0000000000000002)]
        [DataRow("1.00000000000000022204460492503130808472633361816406250",   1.0000000000000002)]
        [DataRow("0:1.00000000000000000000000000000000000000000000000000000", 1.0000000000000000)]
        [DataRow("0:1.00000000000000011102230246251565404236316680908203125", 1.0000000000000000)]
        [DataRow("0:1.00000000000000011102230246251565404236316680908203126", 1.0000000000000002)]
        [DataRow("0:1.00000000000000022204460492503130808472633361816406250", 1.0000000000000002)]
        // Test for double rounding.
        //  60.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
        // + 0.000000000000003552713678800500929355621337890625
        // + 0.00000000000000000000000000000039443045261050590270586428264139311483660321755451150238513946533203125
        // =60.00000000000000355271367880050132378607394839652770586428264139311483660321755451150238513946533203125
        //  0b111100.00000000000000000000000000000000000000000000000_00000000000000000000000000000000000000000000000000000_0
        // +0b000000.00000000000000000000000000000000000000000000000_10000000000000000000000000000000000000000000000000000_0
        // +0b000000.00000000000000000000000000000000000000000000000_00000000000000000000000000000000000000000000000000000_1
        // =0b111100.00000000000000000000000000000000000000000000000_10000000000000000000000000000000000000000000000000000_1
        //    ^----------------------------------------------------^ ^---------------------------------------------------^
        //                           53 bits                                                53 bits
        // The underscores are points where rounding may happen.
        [DataRow( "60.00000000000000355271367880050132378607394839652770586428264139311483660321755451150238513946533203125", 60.00000000000001)]
        [DataRow("1:0.00000000000000355271367880050132378607394839652770586428264139311483660321755451150238513946533203125", 60.00000000000001)]
        [DataRow(
            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "000000000",
            0.0
        )]
        [DataRow(
            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000024703" +
              "2822920623272088284396434110686182529901307162382212792841250337753635104375932649" +
              "9181808179961898982823477228588654633283551779698981993873980053909390631503565951" +
              "5570226392290858392449105184435931802849936536152500319370457678249219365623669863" +
              "6584807570015857692699037063119282795585513329278343384093519780155312465972635795" +
              "7462276646527282722005637400648549997709659947045402082816622623785739345073633900" +
              "7967761930577506740176324673600968951340535537458516661134223766678604162159680461" +
              "9144672918403005300575308490487653917113865916462395249126236538818796362393732804" +
              "2389101867234849766823508986338858792562830275599565752445550725518931369083625477" +
              "9186948667994968324049705821028513185451396213837722826145437693412532098591327667" +
              "236328125",
            0.0
        )]
        [DataRow(
            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000024703" +
              "2822920623272088284396434110686182529901307162382212792841250337753635104375932649" +
              "9181808179961898982823477228588654633283551779698981993873980053909390631503565951" +
              "5570226392290858392449105184435931802849936536152500319370457678249219365623669863" +
              "6584807570015857692699037063119282795585513329278343384093519780155312465972635795" +
              "7462276646527282722005637400648549997709659947045402082816622623785739345073633900" +
              "7967761930577506740176324673600968951340535537458516661134223766678604162159680461" +
              "9144672918403005300575308490487653917113865916462395249126236538818796362393732804" +
              "2389101867234849766823508986338858792562830275599565752445550725518931369083625477" +
              "9186948667994968324049705821028513185451396213837722826145437693412532098591327667" +
              "236328126",
            5e-324
        )]
        [DataRow(
            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
              "0000000000000000000000000000000000000000000000000000000000000000000000000000049406" +
              "5645841246544176568792868221372365059802614324764425585682500675507270208751865299" +
              "8363616359923797965646954457177309266567103559397963987747960107818781263007131903" +
              "1140452784581716784898210368871863605699873072305000638740915356498438731247339727" +
              "3169615140031715385398074126238565591171026658556686768187039560310624931945271591" +
              "4924553293054565444011274801297099995419319894090804165633245247571478690147267801" +
              "5935523861155013480352649347201937902681071074917033322268447533357208324319360923" +
              "8289345836806010601150616980975307834227731832924790498252473077637592724787465608" +
              "4778203734469699533647017972677717585125660551199131504891101451037862738167250955" +
              "8373897335989936648099411642057026370902792427675445652290875386825064197182655334" +
              "472656250",
            5e-324
        )]
        public void Float(string presentation, double value)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(YamlFloat.FromPresentation1_1(presentation)?.PresentationEquals(yamlNode));

            YamlFloat? yamlFloat = yamlNode as YamlFloat;
            Assert.IsNotNull(yamlFloat);
            Assert.AreEqual(presentation, yamlFloat.Presentation);
            Assert.AreEqual(value, yamlFloat.Value);

            // Check sign of zeros.
            Assert.AreEqual(1.0 / value, 1.0 / yamlFloat.Value);
        }

        /// <summary>
        ///     Tests deserialization of strings that are almost valid float values.
        /// </summary>
        /// <param name="presentation">The presentation of the string value.</param>
        [DataTestMethod]
        // Test leading underscore.
        [DataRow("_0.0")]
        // Test underscore-only integer part.
        [DataRow("_.0")]
        // Test underscores in exponents.
        [DataRow("1.5e1_1")]
        // Test truncated exponents.
        [DataRow("1.5e")]
        [DataRow("1.5e+")]
        [DataRow("1.5e-")]
        // Test truncated exponents with junk after the end.
        [DataRow("1.5ez")]
        [DataRow("1.5e+z")]
        [DataRow("1.5e-z")]
        // Test junk after the end.
        [DataRow("0.0z")]
        [DataRow("0:0.0z")]
        [DataRow(".inf0")]
        [DataRow(".infz")]
        [DataRow(".nan0")]
        [DataRow(".nanz")]
        // Test incorrectly capitalized infinities.
        [DataRow(".iNF")]
        [DataRow(".InF")]
        // Test incorrectly capitalized NaNs.
        [DataRow(".nAN")]
        [DataRow(".Nan")]
        // Test signed NaNs.
        [DataRow("+.nan")]
        [DataRow("-.nan")]
        [DataRow("+.NaN")]
        [DataRow("-.NaN")]
        [DataRow("+.NAN")]
        [DataRow("-.NAN")]
        public void NotFloat(string presentation)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                YamlString.FromString(presentation, ScalarStyle.Plain)?.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of string values.
        /// </summary>
        /// <param name="presentation">The presentation of the string value.</param>
        /// <param name="value">The expected string value.</param>
        /// <param name="presentationStyle">
        ///     The expected presentation style of the string value.
        /// </param>
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
        [DataRow("|\n  OK\r", "OK\n", ScalarStyle.Literal     )]
        [DataRow(">\n  OK\r", "OK\n", ScalarStyle.Folded      )]
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
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                YamlString.FromString(value, presentationStyle)?.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of block string values.
        /// </summary>
        /// <param name="presentation">The presentation of the string value.</param>
        /// <param name="value">The expected string value.</param>
        /// <param name="presentationStyle">
        ///     The expected presentation style of the string value.
        /// </param>
        [DataTestMethod]
        [DataRow("|\n  ",      "",      ScalarStyle.Literal)]
        [DataRow("|-\n  ",     "",      ScalarStyle.Literal)]
        [DataRow("|-\n  \n",   "",      ScalarStyle.Literal)]
        [DataRow("|\n  OK",    "OK",    ScalarStyle.Literal)]
        [DataRow("|-\n  OK\n", "OK",    ScalarStyle.Literal)]
        [DataRow("|\n  OK\n",  "OK\n",  ScalarStyle.Literal)]
        [DataRow(">\n  ",      "",      ScalarStyle.Folded )]
        [DataRow(">-\n  \n",   "",      ScalarStyle.Folded )]
        [DataRow(">\n  OK",    "OK",    ScalarStyle.Folded )]
        [DataRow(">-\n  OK\n", "OK",    ScalarStyle.Folded )]
        [DataRow(">\n  OK\n",  "OK\n",  ScalarStyle.Folded )]
        public void BlockString(string presentation, string value, ScalarStyle presentationStyle)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                YamlString.FromString(value, presentationStyle)?.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of an empty flow sequence.
        /// </summary>
        [TestMethod]
        public void EmptyFlowSequence()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("[]");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(new YamlSequence(flow: true).PresentationEquals(yamlNode));
        }

        /// <summary>
        ///     Tests deserialization of a flow sequence.
        /// </summary>
        [TestMethod]
        public void FlowSequence()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("[1]");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlSequence(flow: true)
                {
                    new YamlInteger(1),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block sequence.
        /// </summary>
        [TestMethod]
        public void BlockSequence()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("- 1");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlSequence(flow: false)
                {
                    new YamlInteger(1),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block sequence containing a single <c>null</c> value.
        /// </summary>
        [TestMethod]
        public void BlockSequenceContainingNull()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("- ");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlSequence(flow: false)
                {
                    YamlNull.FromEmptyPresentation(),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block sequence containing two literal block strings with
        ///     the same value.
        /// </summary>
        /// <param name="presentation">The presentation of the block sequence.</param>
        /// <param name="value">The expected value of both strings.</param>
        [DataTestMethod]
        [DataRow("- |-\n  \n- |\n  ",      ""   )]
        [DataRow("- |-\n  \n- |-\n  \n",   ""   )]
        [DataRow("- |-\n  a\n- |\n  a",    "a"  )]
        [DataRow("- |-\n  a\n- |-\n  a\n", "a"  )]
        [DataRow("- |\n  a\n- |\n  a\n",   "a\n")]
        public void BlockSequenceContainingBlockString(string presentation, string value)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlSequence(flow: false)
                {
                    YamlString.FromString(value, ScalarStyle.Literal) ??
                        throw new InvalidOperationException(),
                    YamlString.FromString(value, ScalarStyle.Literal) ??
                        throw new InvalidOperationException(),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block sequence containing a block sequence.
        /// </summary>
        [TestMethod]
        public void BlockSequenceContainingBlockSequence()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(
                "- - 1\n" +
                "  - 2\n" +
                "- - 3\n" +
                "  - 4"
            );
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlSequence(flow: false)
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
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block sequence containing a block mapping.
        /// </summary>
        [TestMethod]
        public void BlockSequenceContainingBlockMapping()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(
                "- b: 1\n" +
                "  c: 2\n" +
                "- a: 3\n" +
                "  d: 4"
            );
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlSequence(flow: false)
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
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of an empty flow mapping.
        /// </summary>
        [TestMethod]
        public void EmptyFlowMapping()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("{}");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(new YamlMapping(flow: true).PresentationEquals(yamlNode));
        }

        /// <summary>
        ///     Tests deserialization of a flow mapping.
        /// </summary>
        [TestMethod]
        public void FlowMapping()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("{a: 1}");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlMapping(flow: true)
                {
                    [
                        YamlString.FromString("a", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(1),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a flow mapping containing a single <c>null</c> to
        ///     <c>null</c> entry.
        /// </summary>
        [TestMethod]
        public void FlowMappingContainingNull()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize("{~: }");
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlMapping(flow: true)
                {
                    [YamlNull.FromCanonicalPresentation()] = YamlNull.FromEmptyPresentation(),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block mapping.
        /// </summary>
        [TestMethod]
        public void BlockMapping()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(
                "b: 1\n" +
                "c: 2\n" +
                "a: 3\n" +
                "d: 4"
            );
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
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
                    [
                        YamlString.FromString("a", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(3),
                    [
                        YamlString.FromString("d", ScalarStyle.Plain) ??
                            throw new InvalidOperationException()
                    ] = new YamlInteger(4),
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block mapping containing two literal block strings with
        ///     the same value.
        /// </summary>
        /// <param name="presentation">The presentation of the block mapping.</param>
        /// <param name="value">The expected value of both strings.</param>
        [DataTestMethod]
        [DataRow("b: |-\n  \nc: |\n  ",      ""   )]
        [DataRow("b: |-\n  \nc: |-\n  \n",   ""   )]
        [DataRow("b: |-\n  a\nc: |\n  a",    "a"  )]
        [DataRow("b: |-\n  a\nc: |-\n  a\n", "a"  )]
        [DataRow("b: |\n  a\nc: |\n  a\n",   "a\n")]
        public void BlockMappingContainingBlockString(string presentation, string value)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlMapping(flow: false)
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
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block mapping containing a block sequence.
        /// </summary>
        [TestMethod]
        public void BlockMappingContainingBlockSequence()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(
                "b:\n" +
                "- 1\n" +
                "c:\n" +
                "- 2"
            );
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlMapping(flow: false)
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
                }.PresentationEquals(yamlNode)
            );
        }

        /// <summary>
        ///     Tests deserialization of a block mapping containing a block mapping.
        /// </summary>
        [TestMethod]
        public void BlockMappingContainingBlockMapping()
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(
                "b:\n" +
                "  c: 2\n" +
                "  d: 3\n" +
                "a: 4"
            );
            YamlNode yamlNode = yamlDocument.RootNode;
            Assert.IsTrue(
                new YamlMapping(flow: false)
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
                }.PresentationEquals(yamlNode)
            );
        }
    }
}
