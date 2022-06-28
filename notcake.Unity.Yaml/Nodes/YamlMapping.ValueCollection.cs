using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace notcake.Unity.Yaml.Nodes
{
    public partial class YamlMapping
    {
        /// <summary>
        ///     An <see cref="IReadOnlyList{T}"/> of ordered values in a <see cref="YamlMapping"/>.
        /// </summary>
        private class ValueCollection : IReadOnlyList<YamlNode>
        {
            private readonly YamlMapping yamlMapping;

            /// <summary>
            ///     Initializes a new instance of the <see cref="ValueCollection"/> class.
            /// </summary>
            /// <param name="yamlMapping">
            ///     The <see cref="YamlMapping"/> whose ordered values are to be exposed.
            /// </param>
            public ValueCollection(YamlMapping yamlMapping)
            {
                this.yamlMapping = yamlMapping;
            }

            #region IEnumerable
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

            #region IEnumerable<YamlNode>
            public IEnumerator<YamlNode> GetEnumerator()
            {
                IEnumerable<YamlNode> values =
                    this.yamlMapping.orderedKeys.Select(x => this.yamlMapping.children[x]);

                return values.GetEnumerator();
            }
            #endregion

            #region IReadOnlyCollection<YamlNode>
            public int Count => this.yamlMapping.Count;
            #endregion

            #region IReadOnlyList<YamlNode>
            public YamlNode this[int index] =>
                this.yamlMapping.children[this.yamlMapping.orderedKeys[index]];
            #endregion
        }
    }
}
