using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using notcake.Unity.Yaml.IO;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a mapping node in a YAML document.
    /// </summary>
    public partial class YamlMapping : YamlNode
    {
        /// <summary>
        ///     Gets a boolean indicating whether the mapping's presentation uses flow style.
        /// </summary>
        public bool Flow { get; }

        public IReadOnlyList<YamlNode> Keys => this.orderedKeys;
        public IReadOnlyList<YamlNode> Values { get; }

        private readonly Dictionary<YamlNode, YamlNode> children = new();
        private readonly List<YamlNode> orderedKeys = new();
        private readonly Dictionary<string, YamlString> stringKeys = new();

        /// <inheritdoc cref="YamlMapping(bool)"/>
        public YamlMapping() :
            this(flow: false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlMapping"/> class.
        /// </summary>
        /// <param name="flow">
        ///     A boolean indicating whether the mapping's presentation uses flow style.
        /// </param>
        public YamlMapping(bool flow)
        {
            this.Values = new ValueCollection(this);

            this.Flow = flow;
        }

        #region YamlNode
        public override bool PresentationEquals(YamlNode other)
        {
            if (other is not YamlMapping yamlMapping) { return false; }

            if (this.Flow != yamlMapping.Flow) { return false; }
            if (this.Count != yamlMapping.Count) { return false; }

            for (int i = 0; i < this.Count; i++)
            {
                if (!this.Keys  [i].PresentationEquals(yamlMapping.Keys  [i])) { return false; }
                if (!this.Values[i].PresentationEquals(yamlMapping.Values[i])) { return false; }
            }

            return true;
        }

        internal override void Serialize(YamlWriter yamlWriter, bool followedByLineBreak)
        {
            if (this.Flow)
            {
                yamlWriter.Write('{');
                yamlWriter.Indentation += 2;

                for (int i = 0; i < this.orderedKeys.Count; i++)
                {
                    YamlNode key = this.orderedKeys[i];
                    YamlNode value = this.children[this.orderedKeys[i]];

                    if (i > 0) { yamlWriter.Write(','); }

                    if (yamlWriter.CurrentLineLength > 80)
                    {
                        yamlWriter.WriteLineBreakAndIndentation();
                    }
                    else
                    {
                        if (i > 0) { yamlWriter.Write(' '); }
                    }

                    key.Serialize(yamlWriter, false);
                    yamlWriter.Write(": ");
                    value.Serialize(yamlWriter, false);
                }

                yamlWriter.Indentation -= 2;
                yamlWriter.Write('}');
            }
            else
            {
                for (int i = 0; i < this.orderedKeys.Count; i++)
                {
                    YamlNode key = this.orderedKeys[i];
                    YamlNode value = this.children[this.orderedKeys[i]];

                    if (i > 0) { yamlWriter.WriteLineBreakAndIndentation(); }

                    bool isLastEntry = i == this.orderedKeys.Count - 1;

                    key.Serialize(yamlWriter, false);
                    yamlWriter.Write(':');
                    if ((value is YamlSequence yamlSequence && !yamlSequence.Flow) ||
                        (value is YamlMapping yamlMapping && !yamlMapping.Flow))
                    {
                        if (value is YamlMapping)
                        {
                            yamlWriter.Indentation += 2;
                        }
                        yamlWriter.WriteLineBreakAndIndentation();
                        value.Serialize(yamlWriter, !isLastEntry || followedByLineBreak);
                        if (value is YamlMapping)
                        {
                            yamlWriter.Indentation -= 2;
                        }
                    }
                    else
                    {
                        yamlWriter.Write(' ');
                        // It's `value`'s responsibility to bump the indentation level if necessary.
                        value.Serialize(yamlWriter, !isLastEntry || followedByLineBreak);
                    }
                }
            }
        }
        #endregion

        #region YamlMapping
        /// <summary>
        ///     Adds a key to the ordered list of keys, if it is not already present.
        /// </summary>
        /// <param name="key">The key to add.</param>
        private void AddKey(YamlNode key)
        {
            if (this.children.ContainsKey(key)) { return; }

            this.orderedKeys.Add(key);

            if (key is YamlString yamlStringKey)
            {
                this.stringKeys[yamlStringKey.Value] = yamlStringKey;
            }
        }

        /// <summary>
        ///     Removes a key from the ordered list of keys.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        private void RemoveKey(YamlNode key)
        {
            this.orderedKeys.Remove(key);

            if (key is YamlString yamlStringKey)
            {
                this.stringKeys.Remove(yamlStringKey.Value);
            }
        }
        #endregion
    }
}
