using System.Collections.Generic;
using System.Linq;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a boolean node in a YAML document.
    /// </summary>
    /// <remarks>
    ///     See http://yaml.org/type/bool.html for a list of valid boolean presentations.
    /// </remarks>
    public class YamlBoolean : YamlSingleLineScalar<YamlBoolean>
    {
        /// <summary>Gets the canonical YAML presentation for <c>true</c> nodes.</summary>
        public static string TrueCanonicalPresentation => "y";
        /// <summary>Gets the canonical YAML presentation for <c>false</c> nodes.</summary>
        public static string FalseCanonicalPresentation => "n";

        /// <summary>
        ///     Gets a set of valid YAML presentations for <c>true</c> nodes.
        /// </summary>
        /// <remarks>
        ///     See http://yaml.org/type/bool.html for a list of valid <c>true</c> presentations.
        /// </remarks>
        public static IReadOnlySet<string> TruePresentations { get; } = new HashSet<string>
        {
            "y",
            "Y",
            "yes",
            "YES",
            "Yes",
            "true",
            "TRUE",
            "True",
            "on",
            "ON",
            "On",
        };

        /// <summary>
        ///     Gets a set of valid YAML presentations for <c>false</c> nodes.
        /// </summary>
        /// <remarks>
        ///     See http://yaml.org/type/bool.html for a list of valid <c>false</c> presentations.
        /// </remarks>
        public static IReadOnlySet<string> FalsePresentations { get; } = new HashSet<string>
        {
            "n",
            "N",
            "no",
            "NO",
            "No",
            "false",
            "FALSE",
            "False",
            "off",
            "OFF",
            "Off",
        };

        /// <summary>
        ///     Gets a set of valid YAML presentations for boolean nodes.
        /// </summary>
        /// <remarks>
        ///     See http://yaml.org/type/bool.html for a list of valid boolean presentations.
        /// </remarks>
        public static IReadOnlySet<string> Presentations { get; } =
            new HashSet<string>(TruePresentations.Concat(FalsePresentations));

        /// <summary>
        ///     Gets the canonical YAML presentation of the boolean node.
        /// </summary>
        public string CanonicalPresentation => this.Value ?
            YamlBoolean.TrueCanonicalPresentation :
            YamlBoolean.FalseCanonicalPresentation;

        /// <summary>
        ///     Gets the value of the boolean node.
        /// </summary>
        public bool Value { get; }

        /// <inheritdoc cref="YamlNull(bool, string)"/>
        public YamlBoolean(bool value) :
            this(
                value,
                value ?
                    YamlBoolean.TrueCanonicalPresentation :
                    YamlBoolean.FalseCanonicalPresentation
            )
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlBoolean"/> class.
        /// </summary>
        /// <param name="value">The value of the boolean node.</param>
        /// <param name="presentation">The YAML presentation of the boolean node.</param>
        private YamlBoolean(bool value, string presentation) :
            base(presentation)
        {
            this.Value = value;
        }

        #region Object
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        #endregion

        #region YamlScalar<YamlBoolean>
        public override bool Equals(YamlBoolean other)
        {
            return this.Value == other.Value;
        }
        #endregion

        /// <summary>
        ///     Creates a boolean node with the given YAML presentation.
        /// </summary>
        /// <param name="presentation">The YAML presentation of the boolean node.</param>
        /// <returns>
        ///     A boolean node with the given YAML presentation, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static YamlBoolean? FromPresentation(string presentation)
        {
            if (YamlBoolean.TruePresentations.Contains(presentation))
            {
                return new YamlBoolean(true, presentation);
            }
            else if (YamlBoolean.FalsePresentations.Contains(presentation))
            {
                return new YamlBoolean(false, presentation);
            }

            return null;
        }
    }
}
