using System.Collections;
using System.Collections.Generic;
using notcake.Unity.Yaml.IO;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a sequence node in a YAML document.
    /// </summary>
    public class YamlSequence : YamlNode, IList<YamlNode>, IReadOnlyList<YamlNode>
    {
        /// <summary>
        ///     Gets a boolean indicating whether the sequence's presentation uses flow style.
        /// </summary>
        public bool Flow { get; }

        private readonly List<YamlNode> children = new();

        /// <inheritdoc cref="YamlSequence(bool)"/>
        public YamlSequence() :
            this(flow: false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlSequence"/> class.
        /// </summary>
        /// <param name="flow">
        ///     A boolean indicating whether the sequence's presentation uses flow style.
        /// </param>
        public YamlSequence(bool flow)
        {
            this.Flow = flow;
        }

        #region YamlNode
        public override bool PresentationEquals(YamlNode other)
        {
            if (other is not YamlSequence yamlSequence) { return false; }

            if (this.Flow != yamlSequence.Flow) { return false; }
            if (this.Count != yamlSequence.Count) { return false; }

            for (int i = 0; i < this.Count; i++)
            {
                if (!this[i].PresentationEquals(yamlSequence[i])) { return false; }
            }

            return true;
        }

        internal override void Serialize(YamlWriter yamlWriter, bool followedByLineBreak)
        {
            if (this.Flow)
            {
                yamlWriter.Write('[');
                yamlWriter.Indentation += 2;

                for (int i = 0; i < this.children.Count; i++)
                {
                    YamlNode childNode = this.children[i];

                    if (i > 0) { yamlWriter.Write(','); }

                    if (yamlWriter.CurrentLineLength > 80)
                    {
                        yamlWriter.WriteLineBreakAndIndentation();
                    }
                    else
                    {
                        if (i > 0) { yamlWriter.Write(' '); }
                    }

                    childNode.Serialize(yamlWriter, false);
                }
                yamlWriter.Indentation -= 2;
                yamlWriter.Write(']');
            }
            else
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    YamlNode childNode = this.children[i];

                    if (i > 0) { yamlWriter.WriteLineBreakAndIndentation(); }

                    bool isLastEntry = i == this.children.Count - 1;

                    yamlWriter.Write("- ");
                    if (childNode is YamlMapping || childNode is YamlSequence)
                    {
                        yamlWriter.Indentation += 2;
                    }
                    childNode.Serialize(yamlWriter, !isLastEntry || followedByLineBreak);
                    if (childNode is YamlMapping || childNode is YamlSequence)
                    {
                        yamlWriter.Indentation -= 2;
                    }
                }
            }
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.children.GetEnumerator();
        }
        #endregion

        #region IEnumerable<YamlNode>
        public IEnumerator<YamlNode> GetEnumerator()
        {
            return this.children.GetEnumerator();
        }
        #endregion

        #region IReadOnlyCollection<YamlNode>
        public int Count => this.children.Count;
        #endregion

        #region ICollection<YamlNode>
        public bool IsReadOnly => ((ICollection<YamlNode>)this.children).IsReadOnly;

        public void Add(YamlNode item)
        {
            this.children.Add(item);
        }

        public void Clear()
        {
            this.children.Clear();
        }

        public bool Contains(YamlNode item)
        {
            return this.children.Contains(item);
        }

        public void CopyTo(YamlNode[] array, int arrayIndex)
        {
            this.children.CopyTo(array, arrayIndex);
        }

        public bool Remove(YamlNode item)
        {
            return this.children.Remove(item);
        }
        #endregion

        #region IList<YamlNode>
        public YamlNode this[int index]
        {
            get { return this.children[index]; }
            set { this.children[index] = value; }
        }

        public int IndexOf(YamlNode item)
        {
            return this.children.IndexOf(item);
        }

        public void Insert(int index, YamlNode item)
        {
            this.children.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.children.RemoveAt(index);
        }
        #endregion
    }
}
