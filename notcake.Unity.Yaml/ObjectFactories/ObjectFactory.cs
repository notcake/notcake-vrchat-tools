using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectFactories;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.ObjectFactories
{
    /// <summary>
    ///     An <see cref="IObjectFactory"/> that remaps the dummy <see cref="IBlockMapping"/>,
    ///     <see cref="IFlowMapping"/>, <see cref="IBlockSequence"/> and <see cref="IFlowSequence"/>
    ///     interfaces to <see cref="YamlMapping">YamlMappings</see> and
    ///     <see cref="YamlSequence">YamlSequences</see>.
    /// </summary>
    internal class ObjectFactory : IObjectFactory
    {
        private readonly DefaultObjectFactory defaultObjectFactory = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectFactory"/> class.
        /// </summary>
        public ObjectFactory()
        {
        }

        #region IObjectFactory
        public object Create(Type type)
        {
            if (type == typeof(IBlockMapping))
            {
                return new YamlMapping(flow: false);
            }
            else if (type == typeof(IFlowMapping))
            {
                return new YamlMapping(flow: true);
            }
            else if (type == typeof(IBlockSequence))
            {
                return new YamlSequence(flow: false);
            }
            else if (type == typeof(IFlowSequence))
            {
                return new YamlSequence(flow: true);
            }
            else
            {
                return this.defaultObjectFactory.Create(type);
            }
        }
        #endregion
    }
}
