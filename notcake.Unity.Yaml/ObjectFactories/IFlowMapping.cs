using System.Collections.Generic;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.ObjectFactories
{
    /// <summary>
    ///     A dummy interface representing a flow <see cref="YamlMapping"/>.
    /// </summary>
    internal interface IFlowMapping :
        IDictionary<YamlNode, YamlNode>, IReadOnlyDictionary<YamlNode, YamlNode>
    {
    }
}
