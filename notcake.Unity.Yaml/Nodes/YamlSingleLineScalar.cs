using System.Collections.Generic;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a single line scalar node in a YAML document.
    /// </summary>
    /// <typeparam name="Self">The concrete type of the scalar node.</typeparam>
    public abstract class YamlSingleLineScalar<Self> : YamlScalar<Self>
        where Self : YamlSingleLineScalar<Self>
    {
        #region YamlScalar
        public override string Presentation { get; }
        public override IReadOnlyList<string> PresentationLines { get; }
        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlSingleLineScalar{Self}"/> class.
        /// </summary>
        /// <param name="presentation">The YAML presentation of the scalar node.</param>
        public YamlSingleLineScalar(string presentation)
        {
            this.Presentation = presentation;
            this.PresentationLines = new string[] { presentation };
        }
    }
}
