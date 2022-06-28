using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace notcake.Unity.Yaml.NodeDeserializers
{
    /// <summary>
    ///     An <see cref="INodeDeserializer"/> that never deserializes anything.
    /// </summary>
    internal class NoOpNodeDeserializer : INodeDeserializer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NoOpNodeDeserializer"/> class.
        /// </summary>
        public NoOpNodeDeserializer()
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
            return false;
        }
        #endregion
    }
}
