using System;
using notcake.Unity.Yaml.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using ScalarStyle = notcake.Unity.Yaml.Style.ScalarStyle;

namespace notcake.Unity.Yaml.NodeDeserializers
{
    /// <summary>
    ///     An <see cref="INodeDeserializer"/> for scalar nodes.
    /// </summary>
    internal class ScalarNodeDeserializer : INodeDeserializer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScalarNodeDeserializer"/> class.
        /// </summary>
        public ScalarNodeDeserializer()
        {
        }

        #region INodeDeserializer
        public bool Deserialize(
            IParser parser,
            Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer,
            out object? value
        )
        {
            value = null;

            if (!expectedType.IsAssignableFrom(typeof(YamlNode))) { return false; }

            if (!parser.Accept<Scalar>(out Scalar? scalar)) { return false; }

            parser.Consume<Scalar>();

            if (scalar.Style == YamlDotNet.Core.ScalarStyle.Plain)
            {
                if (YamlNull.FromPresentation(scalar.Value) is YamlNull yamlNull)
                {
                    value = yamlNull;
                }
                else if (YamlBoolean.FromPresentation(scalar.Value) is YamlBoolean yamlBoolean)
                {
                    value = yamlBoolean;
                }
                else if (YamlInteger.FromPresentation1_1(scalar.Value) is YamlInteger yamlInteger)
                {
                    value = yamlInteger;
                }
                else if (YamlFloat.FromPresentation1_1(scalar.Value) is YamlFloat yamlFloat)
                {
                    value = yamlFloat;
                }
            }

            if (value == null)
            {
                ScalarStyle presentationStyle = scalar.Style switch
                {
                    YamlDotNet.Core.ScalarStyle.Plain        => ScalarStyle.Plain,
                    YamlDotNet.Core.ScalarStyle.SingleQuoted => ScalarStyle.SingleQuoted,
                    YamlDotNet.Core.ScalarStyle.DoubleQuoted => ScalarStyle.DoubleQuoted,
                    YamlDotNet.Core.ScalarStyle.Literal      => ScalarStyle.Literal,
                    YamlDotNet.Core.ScalarStyle.Folded       => ScalarStyle.Folded,
                    _                                        => throw new InvalidOperationException(),
                };
                value = YamlString.FromString(scalar.Value, presentationStyle);
            }

            return true;
        }
        #endregion
    }
}
