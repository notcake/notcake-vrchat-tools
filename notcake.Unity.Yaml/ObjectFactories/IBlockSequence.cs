using System.Collections.Generic;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.ObjectFactories
{
    /// <summary>
    ///     A dummy interface representing a block <see cref="YamlSequence"/>.
    /// </summary>
    internal interface IBlockSequence : IList<YamlNode>, IReadOnlyList<YamlNode>
    {
    }
}
