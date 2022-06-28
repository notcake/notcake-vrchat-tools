using System;
using System.Collections.Generic;
using notcake.Unity.Yaml.IO;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a string node in a YAML document.
    /// </summary>
    public partial class YamlString : YamlScalar<YamlString>
    {
        #region YamlScalar
        public override string Presentation { get; }
        public override IReadOnlyList<string> PresentationLines { get; }
        #endregion

        /// <summary>
        ///     Gets the YAML presentation style of the string node.
        /// </summary>
        public ScalarStyle PresentationStyle { get; }

        /// <summary>
        ///     Gets the value of the string node.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc cref="YamlString(string, ScalarStyle)"/>
        public YamlString(string value) :
            this(value, ScalarStyle.DoubleQuoted)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlString"/> class.
        /// </summary>
        /// <param name="value">The value of the string node.</param>
        /// <param name="presentationStyle">The YAML presentation style of the string node.</param>
        private YamlString(string value, ScalarStyle presentationStyle)
        {
            this.PresentationStyle = presentationStyle;

            this.Value = value;

            this.PresentationLines = presentationStyle switch
            {
                ScalarStyle.Plain        => new string[] { value },
                ScalarStyle.SingleQuoted => new string[] { "'" + value.Replace("'", "''") + "'" },
                ScalarStyle.DoubleQuoted => new string[]
                                            {
                                                "\"" +
                                                value
                                                    .Replace("\\", "\\\\")
                                                    .Replace("\"", "\\\"") +
                                                "\""
                                            },
                ScalarStyle.Literal      => value.EndsWith('\n') ?
                                                new string[] { "|",  value[..^1] } :
                                                new string[] { "|-", value       },
                ScalarStyle.Folded       => value.EndsWith('\n') ?
                                                new string[] { ">",  value[..^1] } :
                                                new string[] { ">-", value       },
                _                        => throw new ArgumentOutOfRangeException(nameof(presentationStyle)),
            };
            this.Presentation = String.Join("\n  ", this.PresentationLines);
        }

        #region Object
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        #endregion

        #region YamlNode
        internal override void Serialize(YamlWriter yamlWriter, bool followedByLineBreak)
        {
            switch (this.PresentationStyle)
            {
                case ScalarStyle.Plain:
                case ScalarStyle.SingleQuoted:
                case ScalarStyle.DoubleQuoted:
                    yamlWriter.Indentation += 2;

                    int currentIndex = 0;

                    static int IndexOfSpace(string presentation, int currentIndex)
                    {
                        // Find the index of the next space that is not followed by a space or tab.
                        int nextSpaceIndex = presentation.IndexOf(' ', currentIndex);
                        while (nextSpaceIndex != -1 &&
                               nextSpaceIndex < presentation.Length - 1 &&
                               (presentation[nextSpaceIndex + 1] == ' ' ||
                                presentation[nextSpaceIndex + 1] == '\t'))
                        {
                            nextSpaceIndex++;
                            nextSpaceIndex = presentation.IndexOf(' ', nextSpaceIndex);
                        }
                        if (nextSpaceIndex == -1) { nextSpaceIndex = presentation.Length; }

                        return nextSpaceIndex;
                    }
                    int nextSpaceIndex = IndexOfSpace(this.Presentation, currentIndex);

                    while (currentIndex < this.Presentation.Length)
                    {
                        yamlWriter.Write(this.Presentation[currentIndex..nextSpaceIndex]);
                        currentIndex = nextSpaceIndex;

                        if (nextSpaceIndex < this.Presentation.Length)
                        {
                            if (yamlWriter.CurrentLineLength >= 80)
                            {
                                yamlWriter.WriteLineBreakAndIndentation();
                            }
                            else
                            {
                                yamlWriter.Write(' ');
                            }
                            currentIndex++;
                        }

                        nextSpaceIndex = IndexOfSpace(this.Presentation, currentIndex);
                    }
                    yamlWriter.Indentation -= 2;
                    break;
                case ScalarStyle.Literal:
                case ScalarStyle.Folded:
                    // If the string is followed by a line break, the line break will be interpreted
                    // as a trailing '\n' at the end of the string in the default chomping mode.
                    yamlWriter.Indentation += 2;
                    yamlWriter.Write(
                        this.PresentationStyle switch
                        {
                            ScalarStyle.Literal => '|',
                            ScalarStyle.Folded  => '>',
                            _                   => throw new InvalidOperationException(),
                        }
                    );
                    // Use the strip chomping mode if the string does not end with '\n' but will be
                    // followed by a line break.
                    if (!this.Value.EndsWith('\n') && followedByLineBreak)
                    {
                        yamlWriter.Write('-');
                    }
                    yamlWriter.WriteLineBreakAndIndentation();
                    if (this.Value.EndsWith('\n') && followedByLineBreak)
                    {
                        // Strip off the '\n'.
                        yamlWriter.Write(this.Value[0..^1]);
                    }
                    else
                    {
                        yamlWriter.Write(this.Value);
                    }
                    yamlWriter.Indentation -= 2;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region YamlScalar<YamlString>
        public override bool Equals(YamlString other)
        {
            return this.Value == other.Value;
        }
        #endregion

        /// <summary>
        ///     Creates a string node with the given value and YAML presentation style.
        /// </summary>
        /// <returns>
        ///     A new string node with the given YAML presentation style, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        /// <inheritdoc cref="YamlString(string, ScalarStyle)"/>
        public static YamlString? FromString(string value, ScalarStyle presentationStyle)
        {
            YamlNodeValidity yamlNodeValidity = presentationStyle switch
            {
                ScalarStyle.Plain        => YamlScalarValidator.Plain.IsValid(value),
                ScalarStyle.SingleQuoted => YamlScalarValidator.SingleQuoted.IsValid(value),
                ScalarStyle.DoubleQuoted => YamlScalarValidator.DoubleQuoted.IsValid(value),
                ScalarStyle.Literal      => YamlScalarValidator.Literal.IsValid(value),
                ScalarStyle.Folded       => YamlScalarValidator.Folded.IsValid(value),
                _                        => throw new ArgumentOutOfRangeException(nameof(value)),
            };

            if (!yamlNodeValidity.Somewhere) { return null; }

            return new YamlString(value, presentationStyle);
        }

        /// <summary>
        ///     Creates a double quoted string node with the given value.
        /// </summary>
        /// <returns>A double quoted string node with the given value.</returns>
        /// <inheritdoc cref="YamlString(string, ScalarStyle)"/>
        public static YamlString DoubleQuoted(string value)
        {
            return new YamlString(value, ScalarStyle.DoubleQuoted);
        }
    }
}
