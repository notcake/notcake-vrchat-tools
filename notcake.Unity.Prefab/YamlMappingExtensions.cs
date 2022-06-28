using notcake.Unity.Yaml.Nodes;

namespace notcake.Unity.Prefab
{
    /// <summary>
    ///     Provides extension methods for the <see cref="YamlMapping"/> class.
    /// </summary>
    internal static class YamlMappingExtensions
    {
        /// <summary>
        ///     Gets a value in a <see cref="YamlMapping"/> and interprets it as a
        ///     <see cref="FileID"/>.
        /// </summary>
        /// <remarks>
        ///     Only values that are <see cref="YamlMapping">YamlMappings</see> with a <c>fileID</c>
        ///     entry containing a signed 64-bit integer value can be interpreted as valid
        ///     <see cref="FileID">FileIDs</see>.
        /// </remarks>
        /// <param name="yamlMapping">
        ///     The <see cref="YamlMapping"/> containing the key and value.
        /// </param>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <returns>
        ///     The <see cref="FileID"/> represented by value associated with
        ///     <paramref name="key"/>, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static FileID? GetFileIDValue(this YamlMapping yamlMapping, string key)
        {
            return yamlMapping.TryGetValue<YamlMapping>(key)?.ToFileID();
        }

        /// <summary>
        ///     Interprets the <see cref="YamlMapping"/> as a <see cref="FileID"/>.
        /// </summary>
        /// <remarks>
        ///     Only <see cref="YamlMapping">YamlMappings</see> with a <c>fileID</c> entry
        ///     containing a signed 64-bit integer value can be interpreted as valid
        ///     <see cref="FileID">FileIDs</see>.
        /// </remarks>
        /// <param name="yamlMapping">
        ///     The <see cref="YamlMapping"/> to interpret as a <see cref="FileID"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="FileID"/> represented by the <see cref="YamlMapping"/>, if
        ///     valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static FileID? ToFileID(this YamlMapping yamlMapping)
        {
            if (yamlMapping.TryGetValue<YamlInteger>("fileID") is YamlInteger fileIDInteger &&
                fileIDInteger.Int64Value is long fileID)
            {
                return new FileID(fileID);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Interprets the <see cref="YamlMapping"/> as a GUID and <see cref="FileID"/> pair.
        /// </summary>
        /// <remarks>
        ///     Only <see cref="YamlMapping">YamlMappings</see> with a <c>fileID</c> entry
        ///     containing a signed 64-bit integer value and a <c>guid</c> entry containing a string
        ///     can be interpreted as valid GUID and <see cref="FileID"/> pairs.
        /// </remarks>
        /// <param name="yamlMapping">
        ///     The <see cref="YamlMapping"/> to interpret as a GUID and <see cref="FileID"/> pair.
        /// </param>
        /// <returns>
        ///     The GUID and <see cref="FileID"/> pair represented by the <see cref="YamlMapping"/>,
        ///     if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static (string, FileID)? ToGuidFileID(this YamlMapping yamlMapping)
        {
            if (yamlMapping.TryGetValue<YamlString>("guid") is YamlString guidString && 
                yamlMapping.TryGetValue<YamlInteger>("fileID") is YamlInteger fileIDInteger &&
                fileIDInteger.Int64Value is long fileID)
            {
                return (guidString.Value, new FileID(fileID));
            }
            else
            {
                return null;
            }
        }
    }
}
