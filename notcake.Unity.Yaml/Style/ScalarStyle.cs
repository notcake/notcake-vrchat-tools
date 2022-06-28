namespace notcake.Unity.Yaml.Style
{
    /// <summary>
    ///     Represents the YAML presentation style of a scalar node.
    /// </summary>
    public enum ScalarStyle
    {
        /// <summary>
        ///     Indicates that the scalar value is presented as-is.
        /// </summary>
        Plain,
        /// <summary>
        ///     Indicates that the scalar value is presented in single quotes (').
        /// </summary>
        /// <remarks>
        ///     Within single quoted values, single quotes are escaped by doubling them ("''").
        /// </remarks>
        SingleQuoted,
        /// <summary>
        ///     Indicates that the scalar value is presented in double quotes (").
        /// </summary>
        DoubleQuoted,
        /// <summary>
        ///     Indicates that the scalar value is presented in literal block style (|).
        /// </summary>
        /// <remarks>
        ///     In literal block style, line breaks are replaced with '\n' during parsing.
        /// </remarks>
        Literal,
        /// <summary>
        ///     Indicates that the scalar value is presented in folded block style (>).
        /// </summary>
        /// <remarks>
        ///     In literal block style, line breaks are replaced with spaces (' ') during parsing,
        ///     except for line breaks followed by one or more empty lines, which are replaced with
        ///     one or more '\n's.
        /// </remarks>
        Folded,
    }
}
