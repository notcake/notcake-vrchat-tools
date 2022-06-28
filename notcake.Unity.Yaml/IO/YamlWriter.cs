using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.IO
{
    /// <summary>
    ///     A writer for a YAML stream.
    /// </summary>
    internal class YamlWriter : IDisposable
    {
        private static readonly char[] LineBreaks = new char[] { '\r', '\n' };

        /// <summary>
        ///     Gets the underlying <see cref="TextWriter"/>.
        /// </summary>
        public TextWriter TextWriter { get; }

        /// <summary>
        ///     Gets the <see cref="LineBreakStyle"/> in use.
        /// </summary>
        public LineBreakStyle LineBreakStyle { get; }
        private readonly string originalLineBreak;

        /// <summary>
        ///     Gets or sets the current indentation level, in characters.
        /// </summary>
        public int Indentation { get; set; }

        /// <summary>
        ///     Gets the length of the current line, in code points.
        /// </summary>
        public int CurrentLineLength { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to which to write.</param>
        /// <param name="lineBreakStyle">The <see cref="LineBreakStyle"/> to use.</param>
        public YamlWriter(TextWriter textWriter, LineBreakStyle lineBreakStyle)
        {
            this.TextWriter = textWriter;
            this.LineBreakStyle = lineBreakStyle;

            this.Indentation = 0;
            this.CurrentLineLength = 0;

            this.originalLineBreak = textWriter.NewLine;
            this.TextWriter.NewLine = this.LineBreakStyle.GetString();
        }

        #region IDisposable
        public void Dispose()
        {
            this.TextWriter.NewLine = this.originalLineBreak;
        }
        #endregion

        #region YamlWriter
        public void Write(char c)
        {
            if (c == '\r' || c == '\n')
            {
                this.CurrentLineLength = 0;
            }
            else
            {
                this.CurrentLineLength++;
            }

            this.TextWriter.Write(c);
        }

        public void Write(string s)
        {
            int lastLineBreakIndex = s.LastIndexOfAny(YamlWriter.LineBreaks);
            if (lastLineBreakIndex != -1)
            {
                this.CurrentLineLength = s[(lastLineBreakIndex + 1)..].EnumerateRunes().Count();
            }
            else
            {
                this.CurrentLineLength += s.EnumerateRunes().Count();
            }

            this.TextWriter.Write(s);
        }

        public void WriteLineBreakAndIndentation()
        {
            this.TextWriter.WriteLine();
            this.TextWriter.Write(new string(' ', Math.Max(0, this.Indentation)));
            this.CurrentLineLength = this.Indentation;
        }
        #endregion
    }
}
