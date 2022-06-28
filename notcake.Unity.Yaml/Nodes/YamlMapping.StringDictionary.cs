using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Nodes
{
    public partial class YamlMapping :
        IDictionary<string, YamlNode>, IReadOnlyDictionary<string, YamlNode>
    {
        #region IEnumerable<KeyValuePair<string, YamlNode>>
        IEnumerator<KeyValuePair<string, YamlNode>> IEnumerable<KeyValuePair<string, YamlNode>>.GetEnumerator()
        {
            return ((IReadOnlyDictionary<string, YamlNode>)this).Keys
                .Select(
                    x => new KeyValuePair<string, YamlNode>(x, this.children[this.stringKeys[x]])
                )
                .GetEnumerator();
        }
        #endregion

        #region IReadOnlyCollection<KeyValuePair<string, YamlNode>>
        int IReadOnlyCollection<KeyValuePair<string, YamlNode>>.Count => this.stringKeys.Count;
        #endregion

        #region ICollection<KeyValuePair<string, YamlNode>>
        int ICollection<KeyValuePair<string, YamlNode>>.Count => this.stringKeys.Count;
        #endregion

        #region ICollection<KeyValuePair<string, YamlNode>>
        public void Add(KeyValuePair<string, YamlNode> item)
        {
            YamlString yamlStringKey = this.CreateStringKey(item.Key);
            this.Add(new KeyValuePair<YamlNode, YamlNode>(yamlStringKey, item.Value));
        }

        public bool Contains(KeyValuePair<string, YamlNode> item)
        {
            YamlString? yamlStringKey = this.GetStringKey(item.Key);
            if (yamlStringKey == null) { return false; }

            return this.Contains(
                new KeyValuePair<YamlNode, YamlNode>(yamlStringKey, item.Value)
            );
        }

        public void CopyTo(KeyValuePair<string, YamlNode>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < this.stringKeys.Count)
            {
                throw new ArgumentException(null, nameof(array));
            }

            int j = 0;
            for (int i = 0; i < this.orderedKeys.Count; i++)
            {
                if (this.orderedKeys[i] is not YamlString yamlStringKey)
                {
                    continue;
                }

                array[arrayIndex + j] = new KeyValuePair<string, YamlNode>(
                    yamlStringKey.Value,
                    this.children[this.orderedKeys[i]]
                );

                j++;
            }
        }

        public bool Remove(KeyValuePair<string, YamlNode> item)
        {
            YamlString? yamlStringKey = this.GetStringKey(item.Key);
            if (yamlStringKey == null) { return false; }

            return this.Remove(
                new KeyValuePair<YamlNode, YamlNode>(yamlStringKey, item.Value)
            );
        }
        #endregion

        #region IReadOnlyDictionary<string, YamlNode>
        IEnumerable<string> IReadOnlyDictionary<string, YamlNode>.Keys =>
            this.Keys
                .Select(x => x as YamlString)
                .Where(x => x != null)
                .Select(x => x!.Value);

        IEnumerable<YamlNode> IReadOnlyDictionary<string, YamlNode>.Values =>
            this.Keys
                .Select(x => x as YamlString)
                .Where(x => x != null)
                .Select(x => this.children[x!]);

        public bool ContainsKey(string key)
        {
            return this.stringKeys.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out YamlNode value)
        {
            YamlString? yamlStringKey = this.GetStringKey(key);

            if (yamlStringKey == null)
            {
                value = null;
                return false;
            }

            return this.TryGetValue(yamlStringKey, out value);
        }
        #endregion

        #region IDictionary<string, YamlNode>
        public YamlNode this[string key]
        {
            get
            {
                return this.children[this.stringKeys[key]];
            }
            set
            {
                YamlString yamlStringKey = this.CreateStringKey(key);
                this.AddKey(yamlStringKey);
                this.children[yamlStringKey] = value;
            }
        }

        ICollection<string> IDictionary<string, YamlNode>.Keys =>
            ((IReadOnlyDictionary<string, YamlNode>)this).Keys.ToArray();
        ICollection<YamlNode> IDictionary<string, YamlNode>.Values =>
            ((IReadOnlyDictionary<string, YamlNode>)this).Values.ToArray();

        public void Add(string key, YamlNode value)
        {
            YamlString yamlStringKey = this.CreateStringKey(key);
            this.Add(yamlStringKey, value);
        }

        public bool Remove(string key)
        {
            YamlString? yamlStringKey = this.GetStringKey(key);
            if (yamlStringKey == null) { return false; }

            return this.Remove(yamlStringKey);
        }
        #endregion

        #region YamlMapping
        /// <inheritdoc cref="TryGetValue{YamlNodeT}(YamlNode, out YamlNodeT)"/>
        public bool TryGetValue<YamlNodeT>(string key, [NotNullWhen(true)] out YamlNodeT? value)
            where YamlNodeT : YamlNode
        {
            value = this.TryGetValue<YamlNodeT>(key);
            return value != null;
        }

        /// <inheritdoc cref="TryGetValue{YamlNodeT}(YamlNode)"/>
        public YamlNodeT? TryGetValue<YamlNodeT>(string key)
            where YamlNodeT : YamlNode
        {
            YamlString? yamlStringKey = this.GetStringKey(key);
            if (yamlStringKey == null) { return null; }

            return this.TryGetValue<YamlNodeT>(yamlStringKey);
        }

        /// <summary>
        ///     Gets the existing <see cref="YamlString"/> key representing the given string or
        ///     creates a new one.
        /// </summary>
        /// <param name="key">
        ///     The string represented by the <see cref="YamlString"/> key to get.
        /// </param>
        /// <returns>A <see cref="YamlString"/> key representing the given string.</returns>
        private YamlString CreateStringKey(string key)
        {
            return this.GetStringKey(key) ??
                   YamlString.FromString(key, ScalarStyle.Plain) ??
                   YamlString.DoubleQuoted(key);
        }

        /// <summary>
        ///     Gets the existing <see cref="YamlString"/> key representing the given string.
        /// </summary>
        /// <param name="key">
        ///     The string represented by the <see cref="YamlString"/> key to get.
        /// </param>
        /// <returns>
        ///     The <see cref="YamlString"/> key representing the given string, if it exists;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        private YamlString? GetStringKey(string key)
        {
            this.stringKeys.TryGetValue(key, out YamlString? yamlStringKey);
            return yamlStringKey;
        }
        #endregion
    }
}
