using System.Collections.Generic;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a <c>null</c> node in a YAML document.
    /// </summary>
    /// <remarks>
    ///     See http://yaml.org/type/null.html for a list of valid <c>null</c> presentations.
    /// </remarks>
    public class YamlNull : YamlSingleLineScalar<YamlNull>
    {
        /// <summary>
        ///     Gets the canonical YAML presentation for <c>null</c> nodes.
        /// </summary>
        public static string CanonicalPresentation => "~";

        /// <summary>
        ///     Gets the empty YAML presentation for <c>null</c> nodes.
        /// </summary>
        public static string EmptyPresentation => "";

        /// <summary>
        ///     Gets a set of valid YAML presentations for <c>null</c> nodes.
        /// </summary>
        /// <remarks>
        ///     See http://yaml.org/type/null.html for a list of valid <c>null</c> presentations.
        /// </remarks>
        public static IReadOnlySet<string> Presentations { get; } = new HashSet<string>
        {
            "",
            "~",
            "null",
            "NULL",
            "Null",
        };

        /// <inheritdoc cref="YamlNull(string)"/>
        public YamlNull() :
            this(YamlNull.CanonicalPresentation)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlNull"/> class.
        /// </summary>
        /// <param name="presentation">The YAML presentation of the <c>null</c> node.</param>
        private YamlNull(string presentation) :
            base(presentation)
        {
        }

        #region Object
        public override int GetHashCode()
        {
            return 4;
        }
        #endregion

        #region YamlScalar<YamlNull>
        public override bool Equals(YamlNull other)
        {
            return true;
        }
        #endregion

        /// <summary>
        ///     Creates a <c>null</c> node with the canonical YAML presentation.
        /// </summary>
        /// <returns>A <c>null</c> node with the canonical YAML presentation.</returns>
        public static YamlNull FromCanonicalPresentation()
        {
            return new YamlNull("~");
        }

        /// <summary>
        ///     Creates a <c>null</c> node with the empty YAML presentation.
        /// </summary>
        /// <returns>A <c>null</c> node with the empty YAML presentation.</returns>
        public static YamlNull FromEmptyPresentation()
        {
            return new YamlNull("");
        }

        /// <summary>
        ///     Creates a <c>null</c> node with the given YAML presentation.
        /// </summary>
        /// <param name="presentation">The YAML presentation of the <c>null</c> node.</param>
        /// <returns>
        ///     A <c>null</c> node with the given YAML presentation, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static YamlNull? FromPresentation(string presentation)
        {
            if (!YamlNull.Presentations.Contains(presentation)) { return null; }

            return new YamlNull(presentation);
        }
    }
}
