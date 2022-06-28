using System;
using notcake.Unity.Yaml.Nodes;
using notcake.Unity.Yaml.ObjectFactories;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace notcake.Unity.Yaml.NodeDeserializers
{
    /// <summary>
    ///     An <see cref="INodeDeserializer"/> for sequence nodes.
    /// </summary>
    internal class SequenceNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer =
            new CollectionNodeDeserializer(new ObjectFactory());

        /// <summary>
        ///     Initializes a new instance of the <see cref="SequenceNodeDeserializer"/> class.
        /// </summary>
        public SequenceNodeDeserializer()
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

            if (!expectedType.IsAssignableFrom(typeof(YamlSequence))) { return false; }

            if (!parser.Accept<SequenceStart>(out SequenceStart? sequenceStart))
            {
                return false;
            }

            bool success = this.nodeDeserializer.Deserialize(
                parser,
                sequenceStart.Style == SequenceStyle.Flow ?
                    typeof(IFlowSequence) :
                    typeof(IBlockSequence),
                nestedObjectDeserializer,
                out value
            );

            return success;
        }
        #endregion
    }
}
