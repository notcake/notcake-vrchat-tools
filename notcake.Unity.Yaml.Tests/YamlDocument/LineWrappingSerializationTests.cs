using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using notcake.Unity.Yaml.Nodes;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Tests.YamlDocument
{
    using YamlDocument = notcake.Unity.Yaml.YamlDocument;

    /// <summary>
    ///     Tests for <see cref="YamlDocument"/>'s line wrapping during serialization.
    /// </summary>
    [TestClass]
    public class LineWrappingSerializationTests
    {
        /// <summary>
        ///     Tests line wrapping of flow mappings during serialization.
        /// </summary>
        [TestMethod]
        public void FlowMapping()
        {
            string presentation =
                "PrefabInstance:\n" +
                "  m_Modification:\n" +
                "    m_Modifications:\n" +
                "    - target: {fileID: 2234567890123456, guid: f8337e58742c5a64c8846b82a8083d4f, type: 3}\n" +
                "    - target: {fileID: 22345678901234567, guid: f8337e58742c5a64c8846b82a8083d4f,\n" +
                "        type: 3}";

            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }

        /// <summary>
        ///     Tests line wrapping of plain scalar strings during serialization.
        /// </summary>
        /// <param name="presentation">The YAML document to round trip.</param>
        [DataTestMethod]
        [DataRow(
            "GameObject:\n" +
            "  m_Name: The quick brown fox jumps over the lazy dog. 123456789012345678901234 abc"
        )]
        [DataRow(
            "GameObject:\n" +
            "  m_Name: The quick brown fox jumps over the lazy dog. 1234567890123456789012345\n" +
            "    abc"
        )]
        public void PlainString(string presentation)
        {
            YamlDocument yamlDocument = YamlDocument.Deserialize(presentation);
            Assert.AreEqual(presentation, yamlDocument.Serialize());
        }
    }
}
