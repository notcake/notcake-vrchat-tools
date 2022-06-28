using notcake.Unity.Yaml.IO;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a node in a YAML document.
    /// </summary>
    public abstract class YamlNode
    {
        /// <summary>
        ///     Determines whether the given <see cref="YamlNode"/> has the same presentation as
        ///     the current <see cref="YamlNode"/>.
        /// </summary>
        /// <param name="other">The other <see cref="YamlNode"/>.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="other"/> has the same presentation as the current
        ///     <see cref="YamlNode"/>;<br/>
        ///     <c>false</c> otherwise.
        ///  </returns>
        public abstract bool PresentationEquals(YamlNode other);

        /// <summary>
        ///     Serializes the <see cref="YamlNode"/>.
        /// </summary>
        /// <param name="yamlWriter">The <see cref="YamlWriter"/> into which to write.</param>
        internal abstract void Serialize(YamlWriter yamlWriter, bool followedByLineBreak);
    }
}
