using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace notcake.Unity.Yaml.Nodes
{
    public partial class YamlMapping :
        IDictionary<YamlNode, YamlNode>, IReadOnlyDictionary<YamlNode, YamlNode>
    {
        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region IEnumerable<KeyValuePair<YamlNode, YamlNode>>
        public IEnumerator<KeyValuePair<YamlNode, YamlNode>> GetEnumerator()
        {
            return this.orderedKeys.Select(
                x => new KeyValuePair<YamlNode, YamlNode>(x, this.children[x])
            ).GetEnumerator();
        }
        #endregion

        #region IReadOnlyCollection<KeyValuePair<YamlNode, YamlNode>>
        public int Count => this.children.Count;
        #endregion

        #region ICollection<KeyValuePair<YamlNode, YamlNode>>
        public bool IsReadOnly =>
            ((ICollection<KeyValuePair<YamlNode, YamlNode>>)this.children).IsReadOnly;

        public void Add(KeyValuePair<YamlNode, YamlNode> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.children.Clear();
            this.orderedKeys.Clear();
            this.stringKeys.Clear();
        }

        public bool Contains(KeyValuePair<YamlNode, YamlNode> item)
        {
            return this.children.Contains(item);
        }

        public void CopyTo(KeyValuePair<YamlNode, YamlNode>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < this.Count)
            {
                throw new ArgumentException(null, nameof(array));
            }

            for (int i = 0; i < this.orderedKeys.Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<YamlNode, YamlNode>(
                    this.orderedKeys[i],
                    this.children[this.orderedKeys[i]]
                );
            }
        }

        public bool Remove(KeyValuePair<YamlNode, YamlNode> item)
        {
            bool success =
                ((ICollection<KeyValuePair<YamlNode, YamlNode>>)this.children).Remove(item);

            if (success)
            {
                this.RemoveKey(item.Key);
            }

            return success;
        }
        #endregion

        #region IReadOnlyDictionary<YamlNode, YamlNode>
        IEnumerable<YamlNode> IReadOnlyDictionary<YamlNode, YamlNode>.Keys   => this.Keys;
        IEnumerable<YamlNode> IReadOnlyDictionary<YamlNode, YamlNode>.Values => this.Values;

        public bool ContainsKey(YamlNode key)
        {
            return this.children.ContainsKey(key);
        }

        public bool TryGetValue(YamlNode key, [MaybeNullWhen(false)] out YamlNode value)
        {
            return this.children.TryGetValue(key, out value);
        }
        #endregion

        #region IDictionary<YamlNode, YamlNode>
        public YamlNode this[YamlNode key]
        {
            get { return this.children[key]; }
            set
            {
                this.AddKey(key);
                this.children[key] = value;
            }
        }

        ICollection<YamlNode> IDictionary<YamlNode, YamlNode>.Keys   => this.Keys.ToArray();
        ICollection<YamlNode> IDictionary<YamlNode, YamlNode>.Values => this.Values.ToArray();

        public void Add(YamlNode key, YamlNode value)
        {
            this.AddKey(key);
            this.children.Add(key, value);
        }

        public bool Remove(YamlNode key)
        {
            bool success = this.children.Remove(key);

            if (success)
            {
                this.RemoveKey(key);
            }

            return success;
        }
        #endregion

        #region YamlMapping
        /// <param name="value">
        ///     <inheritdoc cref="TryGetValue{YamlNodeT}(YamlNode)" path="/returns"/>
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <see cref="YamlMapping"/> contains a value with the given key
        ///     and type;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        /// <inheritdoc cref="TryGetValue{YamlNodeT}(YamlNode)"/>
        public bool TryGetValue<YamlNodeT>(YamlNode key, [NotNullWhen(true)] out YamlNodeT? value)
            where YamlNodeT : YamlNode
        {
            value = this.TryGetValue<YamlNodeT>(key);
            return value != null;
        }

        /// <summary>
        ///      Gets the value associated with the given key.
        /// </summary>
        /// <typeparam name="YamlNodeT">The type of value to get.</typeparam>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>
        ///     The value associated with <paramref name="key"/>, if it exists and is a
        ///     <typeparamref name="YamlNodeT"/>;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public YamlNodeT? TryGetValue<YamlNodeT>(YamlNode key)
            where YamlNodeT : YamlNode
        {
            return this.children.TryGetValue(key, out YamlNode? yamlNode) ?
                yamlNode as YamlNodeT :
                null;
        }
        #endregion
    }
}
