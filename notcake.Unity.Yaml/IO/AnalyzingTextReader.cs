using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using notcake.Unity.Yaml.Style;

namespace notcake.Unity.Yaml.IO
{
    /// <summary>
    ///     Wraps a <see cref="TextReader"/> and analyzes the line breaks in the text being read.
    /// </summary>
    public class AnalyzingTextReader : TextReader
    {
        /// <summary>
        ///     Gets the underlying <see cref="TextReader"/>.
        /// </summary>
        public TextReader TextReader { get; }

        /// <summary>
        ///     Gets a boolean indicating whether the last characters read ended with a line break.
        /// </summary>
        public bool LastCharactersReadWereLineBreak { get; private set; } = false;

        private readonly ulong[] lineBreakCounts =
            new ulong[Enum.GetNames<LineBreakStyle>().Length];

        private bool lastCharacterReadWasCarriageReturn = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AnalyzingTextReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> to wrap.</param>
        public AnalyzingTextReader(TextReader textReader)
        {
            this.TextReader = textReader;
        }

        #region TextReader
        public override int Peek()
        {
            return this.TextReader.Peek();
        }

        public override int Read()
        {
            int characterOrEndOfStream = this.TextReader.Read();

            if (characterOrEndOfStream != -1)
            {
                this.Analyze((char)characterOrEndOfStream);
            }

            return characterOrEndOfStream;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            int charactersRead = this.TextReader.Read(buffer, index, count);
            this.Analyze(buffer.AsSpan(index, charactersRead));
            return charactersRead;
        }

        public override int Read(Span<char> buffer)
        {
            int charactersRead = this.TextReader.Read(buffer);
            this.Analyze(buffer[..charactersRead]);
            return charactersRead;
        }

        public override async Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            int charactersRead = await this.TextReader.ReadAsync(buffer, index, count);
            this.Analyze(buffer.AsSpan(index, charactersRead));
            return charactersRead;
        }

        public override async ValueTask<int> ReadAsync(
            Memory<char> buffer,
            CancellationToken cancellationToken = default
        )
        {
            int charactersRead = await this.TextReader.ReadAsync(buffer, cancellationToken);
            this.Analyze(buffer.Span[..charactersRead]);
            return charactersRead;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            int charactersRead = this.TextReader.ReadBlock(buffer);
            this.Analyze(buffer.AsSpan(index, charactersRead));
            return charactersRead;
        }

        public override int ReadBlock(Span<char> buffer)
        {
            int charactersRead = this.TextReader.ReadBlock(buffer);
            this.Analyze(buffer[..charactersRead]);
            return charactersRead;
        }

        public override async Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            int charactersRead = await this.TextReader.ReadBlockAsync(buffer, index, count);
            this.Analyze(buffer.AsSpan(index, charactersRead));
            return charactersRead;
        }

        public override async ValueTask<int> ReadBlockAsync(
            Memory<char> buffer,
            CancellationToken cancellationToken = default
        )
        {
            int charactersRead = await this.TextReader.ReadBlockAsync(buffer, cancellationToken);
            this.Analyze(buffer.Span[..charactersRead]);
            return charactersRead;
        }

        public override string? ReadLine()
        {
            StringBuilder stringBuilder = new();
            string line;

            int characterOrEndOfStream = this.TextReader.Read();
            while (characterOrEndOfStream != -1)
            {
                char c = (char)characterOrEndOfStream;
                switch (c)
                {
                    case '\r':
                        line = stringBuilder.ToString();
                        this.Analyze(line);
                        if (this.TextReader.Peek() == '\n')
                        {
                            this.Analyze("\r\n");
                            this.TextReader.Read();
                        }
                        else
                        {
                            this.Analyze(c);
                        }
                        return line;
                    case '\n':
                    case '\u0085':
                    case '\u2028':
                    case '\u2029':
                        line = stringBuilder.ToString();
                        this.Analyze(line);
                        this.Analyze(c);
                        return line;
                    default:
                        stringBuilder.Append(c);
                        continue;
                }
            }

            // The end of stream has been hit.
            line = stringBuilder.ToString();
            this.Analyze(line);
            return line.Length > 0 ? line : null;
        }

        public override Task<string?> ReadLineAsync()
        {
            return Task.Run(() => this.ReadLine());
        }

        public override string ReadToEnd()
        {
            string text = this.TextReader.ReadToEnd();
            this.Analyze(text);
            return text;
        }

        public override async Task<string> ReadToEndAsync()
        {
            string text = await this.TextReader.ReadToEndAsync();
            this.Analyze(text);
            return text;
        }
        #endregion

        /// <summary>
        ///     Gets the number of line breaks seen so far with the given style.
        /// </summary>
        /// <param name="lineBreakStyle">
        ///     The <see cref="LineBreakStyle"/> whose number of occurrences are to be retrieved.
        /// </param>
        /// <returns>
        ///     The number of line breaks seen so far with <paramref name="lineBreakStyle"/>.
        /// </returns>
        public ulong GetLineBreakCount(LineBreakStyle lineBreakStyle)
        {
            ulong lineBreakCount = this.lineBreakCounts[(int)lineBreakStyle];
            if (lineBreakStyle == LineBreakStyle.CarriageReturn &&
                this.lastCharacterReadWasCarriageReturn)
            {
                lineBreakCount++;
            }
            return lineBreakCount;
        }

        /// <summary>
        ///     Gets the <see cref="LineBreakStyle"/> most frequently seen so far.
        /// </summary>
        /// <remarks>
        ///     In the event of a tie, any one of the tied
        ///     <see cref="LineBreakStyle">LineBreakStyles</see> is returned.
        /// </remarks>
        /// <returns>
        ///     The <see cref="LineBreakStyle"/> with the most occurrences, if any line breaks have
        ///     been seen;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public LineBreakStyle? GetMostCommonLineBreakStyle()
        {
            LineBreakStyle? mostCommonLineBreakStyle = null;
            ulong maximumCount = 0;

            for (int i = 0; i < this.lineBreakCounts.Length; i++)
            {
                LineBreakStyle lineBreakStyle = (LineBreakStyle)i;
                ulong lineBreakCount = this.GetLineBreakCount(lineBreakStyle);

                if (lineBreakCount > maximumCount)
                {
                    mostCommonLineBreakStyle = lineBreakStyle;
                    maximumCount = lineBreakCount;
                }
            }

            return mostCommonLineBreakStyle;
        }

        private void Analyze(char c)
        {
            ReadOnlySpan<char> buffer = stackalloc char[1] { c };
            this.Analyze(buffer);
        }

        private void Analyze(ReadOnlySpan<char> buffer)
        {
            bool lastCharactersReadWereLineBreak = this.LastCharactersReadWereLineBreak;
            bool lastCharacterReadWasCarriageReturn = this.lastCharacterReadWasCarriageReturn;

            foreach (char c in buffer)
            {
                lastCharactersReadWereLineBreak = true;

                if (lastCharacterReadWasCarriageReturn && c != '\n')
                {
                    this.lineBreakCounts[(int)LineBreakStyle.CarriageReturn]++;
                }

                switch (c)
                {
                    case '\n':
                        if (lastCharacterReadWasCarriageReturn)
                        {
                            this.lineBreakCounts[(int)LineBreakStyle.CarriageReturnLineFeed]++;
                        }
                        else
                        {
                            this.lineBreakCounts[(int)LineBreakStyle.LineFeed]++;
                        }
                        break;
                    case '\r':
                        // This line break may be a \r or a \r\n.
                        // Wait until the next character to count it.
                        break;
                    case '\u0085':
                        this.lineBreakCounts[(int)LineBreakStyle.NextLine]++;
                        break;
                    case '\u2028':
                        this.lineBreakCounts[(int)LineBreakStyle.LineSeparator]++;
                        break;
                    case '\u2029':
                        this.lineBreakCounts[(int)LineBreakStyle.ParagraphSeparator]++;
                        break;
                    default:
                        lastCharactersReadWereLineBreak = false;
                        break;
                }

                lastCharacterReadWasCarriageReturn = c == '\r';
            }

            this.LastCharactersReadWereLineBreak = lastCharactersReadWereLineBreak;
            this.lastCharacterReadWasCarriageReturn = lastCharacterReadWasCarriageReturn;
        }
    }
}
