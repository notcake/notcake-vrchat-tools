using System.Collections.Generic;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Yaml.ObjectFactories
{
    /// <summary>
    ///     A dummy interface representing a flow <see cref="YamlSequence"/>.
    /// </summary>
    internal interface IFlowSequence : IList<YamlNode>, IReadOnlyList<YamlNode>
    {
    }
}
