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
    internal class MappingNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer =
            new DictionaryNodeDeserializer(new ObjectFactory());

        /// <summary>
        ///     Initializes a new instance of the <see cref="MappingNodeDeserializer"/> class.
        /// </summary>
        public MappingNodeDeserializer()
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

            if (!expectedType.IsAssignableFrom(typeof(YamlMapping))) { return false; }

            if (!parser.Accept<MappingStart>(out MappingStart? mappingStart))
            {
                return false;
            }

            bool success = this.nodeDeserializer.Deserialize(
                parser,
                mappingStart.Style == MappingStyle.Flow ?
                    typeof(IFlowMapping) :
                    typeof(IBlockMapping),
                nestedObjectDeserializer,
                out value
            );

            return success;
        }
        #endregion
    }
}
