using notcake.Unity.Yaml;
using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Provides extension methods for the <see cref="YamlNode"/> class.
    /// </summary>
    internal static class YamlNodeExtensions
    {
        /// <summary>
        ///     Interprets the <see cref="YamlNode"/> as a <see cref="FileID"/>.
        /// </summary>
        /// <param name="yamlNode">
        ///     The <see cref="YamlNode"/> to interpret as a <see cref="FileID"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="FileID"/> represented by the <see cref="YamlNode"/>, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        /// <inheritdoc cref="YamlMappingExtensions.ToFileID(YamlMapping)"/>
        public static FileID? ToFileID(this YamlNode yamlNode)
        {
            return yamlNode is YamlMapping yamlMapping ? yamlMapping.ToFileID() : null;
        }

        /// <summary>
        ///     Gets the root <see cref="YamlMapping"/> for a Unity object.
        /// </summary>
        /// <param name="yamlNode">
        ///     The root <see cref="YamlNode"/> of the Unity object's <see cref="YamlDocument"/>.
        /// </param>
        /// <returns>The root <see cref="YamlMapping"/> for the Unity object.</returns>
        public static YamlMapping? GetUnityObjectMapping(this YamlNode yamlNode)
        {
            if (yamlNode is not YamlMapping yamlMapping) { return null; }
            if (yamlMapping.Count != 1) { return null; }

            return yamlMapping.Values[0] as YamlMapping;
        }
    }
}
