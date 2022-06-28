using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Provides methods for validating the content of YAML 1.1 scalars.
    /// </summary>
    public static partial class YamlScalarValidator
    {
        /// <summary>
        ///     Determines whether the given code point is a <c>[ 84] b-l-folded-any(n,s)</c> line
        ///     break.
        /// </summary>
        /// <remarks>
        ///     Folded line breaks are <c>'\n'</c>, <c>' '</c>, <c>'\u2028'</c> and <c>'\u2029'</c>.
        ///     <para/>
        ///     <c>[ 84] b-l-folded-any(n,s)</c> is defined as follows:
        ///     <code>
        ///         [ 28] b-specific                ::= b-line-separator | b-paragraph-separator
        ///         [ 29] b-generic                 ::=   ( b-carriage-return b-line-feed ) /* DOS, Windows */
        ///                                             | b-carriage-return                 /* Macintosh */
        ///                                             | b-line-feed                       /* UNIX */
        ///                                             | b-next-line                       /* Unicode */
        ///         [ 32] b-ignored-generic         ::= b-generic
        ///         [ 81] b-l-folded-specific(n,s)  ::= b-specific l-empty(n,s)*
        ///         [ 82] b-l-folded-as-space       ::= b-generic
        ///         [ 83] b-l-folded-trimmed(n,s)   ::= b-ignored-generic l-empty(n,s)+
        ///         [ 84] b-l-folded-any(n,s)       ::=   b-l-folded-specific(n,s)
        ///                                             | b-l-folded-as-space
        ///                                             | b-l-folded-trimmed(n,s)
        ///     </code>
        /// </remarks>
        /// <param name="c">The code point to check.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="c"/> is a <c>[ 84] b-l-folded-any(n,s)</c> line
        ///     break;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsFoldedLineBreak(char c)
        {
            return Sse2.IsSupported ?
                YamlScalarValidator.IsFoldedLineBreakIntrinsic(c) :
                YamlScalarValidator.IsFoldedLineBreakNonIntrinsic(c);
        }

        /// <inheritdoc cref="IsFoldedLineBreak(char)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsFoldedLineBreakNonIntrinsic(char c)
        {
            return c == '\n' |
                   c == ' ' |
                   (uint)(c - 0x2028) <= 0x2029 - 0x2028;
        }

        /// <inheritdoc cref="IsFoldedLineBreak(char)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsFoldedLineBreakIntrinsic(char c)
        {
            if (!Sse2.IsSupported)
            {
                return YamlScalarValidator.IsFoldedLineBreakNonIntrinsic(c);
            }

            Vector128<ushort> normalizedLineBreaks = Vector128.Create(
                '\n', '\n', '\n',     '\n',
                '\n', ' ',  '\u2028', '\u2029'
            );
            Vector128<ushort> v = Vector128.Create(c);
            int equals = Sse2.MoveMask(Sse2.CompareEqual(normalizedLineBreaks, v).AsByte());
            return equals != 0;
        }

        /// <summary>
        ///     Determines whether the given code point is a <c>[ 31] b-normalized</c> line break.
        /// </summary>
        /// <remarks>
        ///     Normalized line breaks are <c>'\n'</c>, <c>'\u2028'</c> and <c>'\u2029'</c>.
        ///     <para/>
        ///     <c>[ 31] b-normalized</c> is defined as follows:
        ///     <code>
        ///         [ 28] b-specific     ::= b-line-separator | b-paragraph-separator
        ///         [ 29] b-generic      ::=   ( b-carriage-return b-line-feed ) /* DOS, Windows */
        ///                                  | b-carriage-return                 /* Macintosh */
        ///                                  | b-line-feed                       /* UNIX */
        ///                                  | b-next-line                       /* Unicode */
        ///         [ 30] b-as-line-feed ::= b-generic
        ///         [ 31] b-normalized   ::= b-as-line-feed | b-specific
        ///     </code>
        /// </remarks>
        /// <param name="c">The code point to check.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="c"/> is a <c>[ 31] b-normalized</c> line break;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsNormalizedLineBreak(char c)
        {
            return Sse2.IsSupported ?
                YamlScalarValidator.IsNormalizedLineBreakIntrinsic(c) :
                YamlScalarValidator.IsNormalizedLineBreakNonIntrinsic(c);
        }

        /// <inheritdoc cref="IsNormalizedLineBreak(char)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsNormalizedLineBreakNonIntrinsic(char c)
        {
            return c == '\n' |
                   (uint)(c - 0x2028) <= 0x2029 - 0x2028;
        }

        /// <inheritdoc cref="IsNormalizedLineBreak(char)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsNormalizedLineBreakIntrinsic(char c)
        {
            if (!Sse2.IsSupported)
            {
                return YamlScalarValidator.IsNormalizedLineBreakNonIntrinsic(c);
            }

            Vector128<ushort> normalizedLineBreaks = Vector128.Create(
                '\n', '\n', '\n',     '\n',
                '\n', '\n', '\u2028', '\u2029'
            );
            Vector128<ushort> v = Vector128.Create(c);
            int equals = Sse2.MoveMask(Sse2.CompareEqual(normalizedLineBreaks, v).AsByte());
            return equals != 0;
        }

        /// <summary>
        ///     Determines whether the given code point is a valid <c>[ 34] nb-char</c> or
        ///     <c>[ 84] b-l-folded-any(n,s)</c> line break.
        /// </summary>
        /// <remarks>
        ///     <c>[ 34] nb-char</c> is defined as follows:
        ///     <code>
        ///         [  1] c-printable    ::=   #x9 | #xA | #xD | [#x20-#x7E]          /* 8 bit */
        ///                                  | #x85 | [#xA0-#xD7FF] | [#xE000-#xFFFD] /* 16 bit */
        ///                                  | [#x10000-#x10FFFF]                     /* 32 bit */
        ///         [ 27] b-char         ::=   b-line-feed | b-carriage-return | b-next-line
        ///                                  | b-line-separator | b-paragraph-separator
        ///         [ 34] nb-char        ::= c-printable - b-char
        ///     </code>
        ///
        ///     Folded line breaks are <c>'\n'</c>, <c>' '</c>, <c>'\u2028'</c> and
        ///     <c>'\u2029'</c>.
        ///     <para/>
        ///     <c>[ 84] b-l-folded-any(n,s)</c> is defined as follows:
        ///     <code>
        ///         [ 28] b-specific               ::= b-line-separator | b-paragraph-separator
        ///         [ 29] b-generic                ::=   ( b-carriage-return b-line-feed ) /* DOS, Windows */
        ///                                            | b-carriage-return                 /* Macintosh */
        ///                                            | b-line-feed                       /* UNIX */
        ///                                            | b-next-line                       /* Unicode */
        ///         [ 32] b-ignored-generic        ::= b-generic
        ///         [ 81] b-l-folded-specific(n,s) ::= b-specific l-empty(n,s)*
        ///         [ 82] b-l-folded-as-space      ::= b-generic
        ///         [ 83] b-l-folded-trimmed(n,s)  ::= b-ignored-generic l-empty(n,s)+
        ///         [ 84] b-l-folded-any(n,s)      ::=   b-l-folded-specific(n,s)
        ///                                            | b-l-folded-as-space
        ///                                            | b-l-folded-trimmed(n,s)
        ///     </code>
        ///
        ///     Assumes that:<br/>
        ///     <list type="bullet">
        ///         <item>
        ///             The given code point is part of a valid surrogate pair if it is a high
        ///             or low surrogate.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="c">The code point to check.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="c"/> is a valid <c>[ 34] nb-char</c> or
        ///     <c>[ 84] b-l-folded-any(n,s)</c> line break;<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsNonBreakingCharOrFoldedLineBreak(char c)
        {
            return Popcnt.IsSupported && Sse2.IsSupported ?
                YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakIntrinsic(c) :
                YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakNonIntrinsic(c);
        }

        /// <inheritdoc cref="IsNonBreakingCharOrFoldedLineBreak(char)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsNonBreakingCharOrFoldedLineBreakNonIntrinsic(char c)
        {
            return (uint)(c - 0x0009) <= 0x000A - 0x0009 |
                    (uint)(c - 0x0020) <= 0x007E - 0x0020 |
                    (uint)(c - 0x00A0) <= 0xFFFD - 0x00A0;
        }

        /// <inheritdoc cref="IsNonBreakingCharOrFoldedLineBreak(char)"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization
        )]
        internal static bool IsNonBreakingCharOrFoldedLineBreakIntrinsic(char c)
        {
            if (!Popcnt.IsSupported || !Sse2.IsSupported)
            {
                return YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreakNonIntrinsic(c);
            }

            Vector128<short> rangeThresholds = Vector128.Create(
                0x0009 - 1, 0x000A,
                0x0020 - 1, 0x007E,
                0x00A0 - 1, 0xFFFD - 65536,
                0x7FFF,     0x7FFF
            );
            Vector128<short> v = Vector128.Create((short)c);
            Vector128<short> greaterThan = Sse2.CompareGreaterThan(v, rangeThresholds);
            return (Popcnt.PopCount((uint)Sse2.MoveMask(greaterThan.AsByte())) & 2) == 0;
        }
    }
}
