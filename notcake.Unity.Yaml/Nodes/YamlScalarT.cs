using System;
using System.Diagnostics.CodeAnalysis;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a scalar node in a YAML document.
    /// </summary>
    /// <typeparam name="Self">The concrete type of the scalar node.</typeparam>
    public abstract class YamlScalar<Self> : YamlScalar, IEquatable<Self>
        where Self : YamlScalar<Self>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlScalar{Self}"/> class.
        /// </summary>
        protected YamlScalar()
        {
        }

        #region Object
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Self self) { return false; }

            return this.Equals(self);
        }

        public abstract override int GetHashCode();
        #endregion

        #region YamlNode
        public override bool PresentationEquals(YamlNode other)
        {
            if (other is not Self yamlScalar) { return false; }

            return this.Presentation == yamlScalar.Presentation;
        }
        #endregion

        #region IEquatable<YamlScalar>
        public override bool Equals([NotNullWhen(true)] YamlScalar? other)
        {
            if (other is not Self self) { return false; }

            return this.Equals(self);
        }
        #endregion

        #region IEquatable<Self>
        bool IEquatable<Self>.Equals(Self? other)
        {
            if (other is not Self self) { return false; }

            return this.Equals(self);
        }
        #endregion

        #region YamlScalar<Self>
        public abstract bool Equals(Self other);
        #endregion
    }
}
