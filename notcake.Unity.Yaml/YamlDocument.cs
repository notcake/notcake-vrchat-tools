using System.IO;
using notcake.Unity.Yaml.IO;
using notcake.Unity.Yaml.NodeDeserializers;
using notcake.Unity.Yaml.Nodes;
using notcake.Unity.Yaml.ObjectFactories;
using notcake.Unity.Yaml.Style;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace notcake.Unity.Yaml
{
    /// <summary>
    ///     Represents a Unity YAML document.
    /// </summary>
    /// <remarks>
    ///     Unity uses YAML 1.1. See the YAML 1.1 specification at https://yaml.org/spec/1.1/.
    ///     <para/>
    ///     Multiple YAML documents in the same stream are not supported.
    /// </remarks>
    public class YamlDocument
    {
        /// <summary>
        ///     Gets or sets the root node of the YAML document.
        /// </summary>
        public YamlNode RootNode { get; set; }

        /// <summary>
        ///     Gets or sets the line break style of the YAML document.
        /// </summary>
        public LineBreakStyle LineBreakStyle { get; set; }

        /// <summary>
        ///     Gets or sets a boolean indicating whether the YAML document ends with a line break.
        /// </summary>
        public bool TrailingLineBreak { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlDocument"/> class.
        /// </summary>
        /// <param name="rootNode">The root node of the YAML document.</param>
        /// <param name="lineBreakStyle">The line break style of the YAML document.</param>
        /// <param name="trailingLineBreak">
        ///     A boolean indicating whether the YAML document ends with a line break.
        /// </param>
        public YamlDocument(
            YamlNode rootNode,
            LineBreakStyle lineBreakStyle = LineBreakStyle.LineFeed,
            bool trailingLineBreak = true
        )
        {
            this.RootNode = rootNode;
            this.LineBreakStyle = lineBreakStyle;
            this.TrailingLineBreak = trailingLineBreak;
        }

        /// <summary>
        ///     Serializes the YAML document into a string.
        /// </summary>
        /// <returns>The string representing the YAML document.</returns>
        public string Serialize()
        {
            using StringWriter stringWriter = new();
            this.Serialize(stringWriter);
            return stringWriter.ToString();
        }

        /// <summary>
        ///     Serializes the YAML document into a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="Stream"/> into which to serialize the YAML document.
        /// </param>
        /// <inheritdoc cref="Serialize(TextWriter)"/>
        public void Serialize(Stream stream)
        {
            using StreamWriter streamWriter = new(stream, leaveOpen: true);
            this.Serialize(streamWriter);
        }

        /// <summary>
        ///     Serializes the YAML document into a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">
        ///     The <see cref="TextWriter"/> into which to serialize the YAML document.
        /// </param>
        public void Serialize(TextWriter textWriter)
        {
            using YamlWriter yamlWriter = new(textWriter, this.LineBreakStyle);
            this.RootNode.Serialize(yamlWriter, this.TrailingLineBreak);
            if (this.TrailingLineBreak)
            {
                yamlWriter.WriteLineBreakAndIndentation();
            }
        }

        /// <summary>
        ///     Deserializes a YAML document from a string.
        /// </summary>
        /// <param name="string">The string from which to deserialize the YAML document.</param>
        /// <inheritdoc cref="Deserialize(TextReader)"/>
        public static YamlDocument Deserialize(string @string)
        {
            using StringReader stringReader = new(@string);
            return YamlDocument.Deserialize(stringReader);
        }

        /// <summary>
        ///     Deserializes a YAML document from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        ///     The <see cref="Stream"/> from which to deserialize the YAML document.
        /// </param>
        /// <inheritdoc cref="Deserialize(TextReader)"/>
        public static YamlDocument Deserialize(Stream stream)
        {
            using StreamReader streamReader = new(stream, leaveOpen: true);
            return YamlDocument.Deserialize(streamReader);
        }

        /// <summary>
        ///     Deserializes a YAML document from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="textReader">
        ///     The <see cref="TextReader"/> from which to deserialize the YAML document.
        /// </param>
        /// <returns>The deserialized <see cref="YamlDocument"/>.</returns>
        /// <exception cref="InvalidDataException">
        ///     Thrown when the YAML document is malformed.
        /// </exception>
        public static YamlDocument Deserialize(TextReader textReader)
        {
            DeserializerBuilder deserializerBuilder = new();
            IDeserializer deserializer = deserializerBuilder
                .WithNodeDeserializer(
                    new NoOpNodeDeserializer(),
                    x => x.InsteadOf<YamlDotNet.Serialization.NodeDeserializers.NullNodeDeserializer>()
                )
                .WithNodeDeserializer(
                    new ScalarNodeDeserializer(),
                    x => x.InsteadOf<YamlDotNet.Serialization.NodeDeserializers.ScalarNodeDeserializer>()
                )
                .WithNodeDeserializer(
                    new MappingNodeDeserializer(),
                    x => x.Before<YamlDotNet.Serialization.NodeDeserializers.DictionaryNodeDeserializer>()
                )
                .WithNodeDeserializer(
                    new SequenceNodeDeserializer(),
                    x => x.Before<YamlDotNet.Serialization.NodeDeserializers.CollectionNodeDeserializer>()
                )
                .Build();

            using AnalyzingTextReader analyzingTextReader = new(textReader);
            YamlNode yamlNode;
            try
            {
                yamlNode = deserializer.Deserialize<YamlNode?>(analyzingTextReader) ??
                           YamlNull.FromEmptyPresentation();
            }
            catch (YamlException yamlException)
            {
                throw new InvalidDataException(yamlException.Message, yamlException);
            }
            return new YamlDocument(
                yamlNode,
                analyzingTextReader.GetMostCommonLineBreakStyle() ?? LineBreakStyle.LineFeed,
                analyzingTextReader.LastCharactersReadWereLineBreak
            );
        }
    }
}
