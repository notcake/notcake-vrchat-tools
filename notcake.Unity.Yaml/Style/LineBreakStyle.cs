using System;

namespace notcake.Unity.Yaml.Style
{
    /// <summary>
    ///     Represents the line break style of a YAML stream.
    /// </summary>
    public enum LineBreakStyle
    {
        /// <summary>Indicates that the YAML stream uses "\n" for line breaks.</summary>
        LineFeed               = 0,
        /// <summary>Indicates that the YAML stream uses "\r" for line breaks.</summary>
        CarriageReturn         = 1,
        /// <summary>Indicates that the YAML stream uses "\r\n" for line breaks.</summary>
        CarriageReturnLineFeed = 2,
        /// <summary>Indicates that the YAML stream uses "\u0085" for line breaks.</summary>
        NextLine               = 3,
        /// <summary>Indicates that the YAML stream uses "\u2028" for line breaks.</summary>
        LineSeparator          = 4,
        /// <summary>Indicates that the YAML stream uses "\u2029" for line breaks.</summary>
        ParagraphSeparator     = 5,
    }

    /// <summary>
    ///     Provides extension methods for <see cref="LineBreakStyle"/>.
    /// </summary>
    public static class LineBreakStyleExtensions
    {
        /// <summary>
        ///     Gets the line break associated with the given <see cref="LineBreakStyle"/>.
        /// </summary>
        /// <param name="lineBreakStyle">
        ///     The <see cref="LineBreakStyle"/> associated with the line break to get.
        /// </param>
        /// <returns>
        ///     The line break associated with <paramref name="lineBreakStyle"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="lineBreakStyle"/> is not a valid
        ///     <see cref="LineBreakStyle"/>.
        /// </exception>
        public static string GetString(this LineBreakStyle lineBreakStyle)
        {
            return lineBreakStyle switch
            {
                LineBreakStyle.LineFeed               => "\n",
                LineBreakStyle.CarriageReturn         => "\r",
                LineBreakStyle.CarriageReturnLineFeed => "\r\n",
                LineBreakStyle.NextLine               => "\u0085",
                LineBreakStyle.LineSeparator          => "\u2028",
                LineBreakStyle.ParagraphSeparator     => "\u2029",
                _                                     => throw new ArgumentOutOfRangeException(nameof(lineBreakStyle))
            };
        }
    }
}
