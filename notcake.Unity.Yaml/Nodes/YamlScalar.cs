using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using notcake.Unity.Yaml.IO;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a scalar node in a YAML document.
    /// </summary>
    public abstract class YamlScalar : YamlNode, IEquatable<YamlScalar>
    {
        /// <summary>
        ///     Gets the YAML presentation of the scalar node as a single multiline string.
        /// </summary>
        public abstract string Presentation { get; }

        /// <summary>
        ///     Gets the YAML presentation of the scalar node as a list of lines, with no
        ///     indentation included.
        /// </summary>
        public abstract IReadOnlyList<string> PresentationLines { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlScalar"/> class.
        /// </summary>
        protected YamlScalar()
        {
        }

        #region Object
        public abstract override bool Equals([NotNullWhen(true)] object? obj);
        public abstract override int GetHashCode();

        public override string ToString()
        {
            return this.Presentation;
        }
        #endregion

        #region YamlNode
        internal override void Serialize(YamlWriter yamlWriter, bool followedByLineBreak)
        {
            yamlWriter.Write(this.Presentation);
        }
        #endregion

        #region IEquatable<YamlScalar>
        public abstract bool Equals(YamlScalar? other);
        #endregion
    }
}
