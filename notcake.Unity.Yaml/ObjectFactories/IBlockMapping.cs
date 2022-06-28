using System.Collections.Generic;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.ObjectFactories
{
    /// <summary>
    ///     A dummy interface representing a block <see cref="YamlMapping"/>.
    /// </summary>
    internal interface IBlockMapping :
        IDictionary<YamlNode, YamlNode>, IReadOnlyDictionary<YamlNode, YamlNode>
    {
    }
}
