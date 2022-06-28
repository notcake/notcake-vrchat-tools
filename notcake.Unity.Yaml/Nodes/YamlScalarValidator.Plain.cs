using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace notcake.Unity.Yaml.Nodes
{
    public static partial class YamlScalarValidator
    {
        /// <summary>
        ///     Provides methods for validating the content of YAML 1.1 plain scalars.
        /// </summary>
        /// <remarks>
        ///     A YAML 1.1 plain scalar is defined by the following rules:
        ///     <code>
        ///         [  1] c-printable               ::=   #x9 | #xA | #xD | [#x20-#x7E]          /* 8 bit */
        ///                                             | #x85 | [#xA0-#xD7FF] | [#xE000-#xFFFD] /* 16 bit */
        ///                                             | [#x10000-#x10FFFF]                     /* 32 bit */
        ///         [ 21] c-indicator               ::=   “-” | “?” | “:” | “,” | “[” | “]” | “{” | “}”
        ///                                             | “#” | “&amp;” | “*” | “!” | “|” | “>” | “'” | “"”
        ///                                             | “%” | “@” | “`”
        ///         [ 27] b-char                    ::=   b-line-feed | b-carriage-return | b-next-line
        ///                                             | b-line-separator | b-paragraph-separator
        ///         [ 28] b-specific                ::= b-line-separator | b-paragraph-separator
        ///         [ 29] b-generic                 ::=   ( b-carriage-return b-line-feed ) /* DOS, Windows */
        ///                                             | b-carriage-return                 /* Macintosh */
        ///                                             | b-line-feed                       /* UNIX */
        ///                                             | b-next-line                       /* Unicode */
        ///         [ 30] b-as-line-feed            ::= b-generic
        ///         [ 31] b-normalized              ::= b-as-line-feed | b-specific
        ///         [ 32] b-ignored-generic         ::= b-generic
        ///         [ 34] nb-char                   ::= c-printable - b-char
        ///         [ 35] s-ignored-space           ::= #x20 /*SP*/
        ///         [ 36] s-white                   ::= #x9 /*TAB*/ | #x20 /*SP*/
        ///         [ 37] s-ignored-white           ::= s-white
        ///         [ 66] s-indent(n)               ::= s-ignored-space x n
        ///         [ 76] s-ignored-prefix(n,s)     ::= s = plain   ⇒ s-ignored-prefix-plain(n)
        ///                                             s = double  ⇒ s-ignored-prefix-quoted(n)
        ///                                             s = single  ⇒ s-ignored-prefix-quoted(n)
        ///                                             s = literal ⇒ s-ignored-prefix-block(n)
        ///                                             s = folded  ⇒ s-ignored-prefix-block(n)
        ///         [ 77] s-ignored-prefix-plain(n) ::= s-indent(n) s-ignored-space*
        ///         [ 80] l-empty(n,s)              ::= ( s-indent(&lt;n) | s-ignored-prefix(n,s) )
        ///                                             b-normalized
        ///         [ 81] b-l-folded-specific(n,s)  ::= b-specific l-empty(n,s)*
        ///         [ 82] b-l-folded-as-space       ::= b-generic
        ///         [ 83] b-l-folded-trimmed(n,s)   ::= b-ignored-generic l-empty(n,s)+
        ///         [ 84] b-l-folded-any(n,s)       ::=   b-l-folded-specific(n,s)
        ///                                             | b-l-folded-as-space
        ///                                             | b-l-folded-trimmed(n,s)
        ///         [153] nb-plain-char(c)          ::= c = flow-out ⇒ nb-plain-char-out
        ///                                             c = flow-in  ⇒ nb-plain-char-in
        ///                                             c = flow-key ⇒ nb-plain-char-in
        ///         [154] nb-plain-char-out         ::=   ( nb-char - “:” - “#” - #x9 /*TAB*/ )
        ///                                             | ( ns-plain-char(flow-out) “#” )
        ///                                             | ( “:” ns-plain-char(flow-out) )
        ///         [155] nb-plain-char-in          ::= nb-plain-char-out - “,” - “[” - “]” - “{” - “}”
        ///         [156] ns-plain-char(c)          ::= nb-plain-char(c) - #x20 /*SP*/
        ///         [157] ns-plain-first-char(c)    ::=   ( ns-plain-char(c) - c-indicator )
        ///                                             | ( ( “-” | “?” | “:” ) ns-plain-char(c) )
        ///         [158] ns-plain(n,c)             ::= c = flow-out ⇒ ns-plain-multi(n,c)?
        ///                                             c = flow-in  ⇒ ns-plain-multi(n,c)?
        ///                                             c = flow-key ⇒ ns-plain-single(c)
        ///         [159] l-forbidden-content       ::= /* start of line */
        ///                                             ( “---” | “...” )
        ///                                             /* space or end of line */
        ///         [160] ns-plain-single(c)        ::= ( ns-plain-first-char(c) ( nb-plain-char(c)* ns-plain-char(c) )? )
        ///                                             - l-forbidden-content
        ///         [161] s-l-plain-break(n)        ::= s-ignored-white* b-l-folded-any(n,plain)
        ///         [162] ns-plain-multi(n,c)       ::= ns-plain-single(c) s-ns-plain-more(n,c)*
        ///         [163] s-ns-plain-more(n,c)      ::= s-l-plain-break(n) s-ignored-prefix(n,plain)
        ///                                             ns-plain-char(c) ( nb-plain-char(c)* ns-plain-char(c) )?
        ///     </code>
        /// </remarks>
        public static class Plain
        {
            /// <summary>
            ///     Determines whether the given content can be presented as a plain scalar.
            /// </summary>
            /// <param name="content">The plain scalar content to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="content"/> can be presented as a plain
            ///     scalar;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            public static YamlNodeValidity IsValid(string content)
            {
                // [160] ns-plain-single(c) ::= ( ns-plain-first-char(c) ( nb-plain-char(c)* ns-plain-char(c) )? )
                //                              - l-forbidden-content
                // Plain scalars cannot be empty.
                if (content.Length == 0) { return YamlNodeValidity.None; }

                // [160] ns-plain-single(c) ::= ( ns-plain-first-char(c) ( nb-plain-char(c)* ns-plain-char(c) )? )
                //                              - l-forbidden-content
                // The first character has to be a valid `[157] ns-plain-first-char(c)`.
                // The middle characters have to be valid `[153] nb-plain-char(c)`s.
                // The last character has to be a valid `[157] ns-plain-char(c)`.

                // [156] ns-plain-char(c)       ::= nb-plain-char(c) - #x20 /*SP*/
                // [157] ns-plain-first-char(c) ::=   ( ns-plain-char(c) - c-indicator )
                //                                  | ( ( “-” | “?” | “:” ) ns-plain-char(c) )
                // [160] ns-plain-single(c)     ::= ( ns-plain-first-char(c) ( nb-plain-char(c)* ns-plain-char(c) )? )
                //                                  - l-forbidden-content
                // Plain scalars cannot start or end with a space.
                // [161] s-l-plain-break(n)     ::= s-ignored-white* b-l-folded-any(n,plain)
                // [162] ns-plain-multi(n,c)    ::= ns-plain-single(c) s-ns-plain-more(n,c)*
                // [163] s-ns-plain-more(n,c)   ::= s-l-plain-break(n) s-ignored-prefix(n,plain)
                //                                  ns-plain-char(c) ( nb-plain-char(c)* ns-plain-char(c) )?
                // Plain scalars cannot start or end with a `[ 84] b-l-folded-any(n,s)` line break.
                // The check for `[153] nb-plain-char(c)`s happens later.
                if (YamlScalarValidator.IsFoldedLineBreak(content[0]) ||
                    YamlScalarValidator.IsFoldedLineBreak(content[^1]))
                {
                    return YamlNodeValidity.None;
                }

                // [159] l-forbidden-content ::= /* start of line */
                //                               ( “---” | “...” )
                //                               /* space or end of line */
                // Plain scalars at the start of a line cannot start with a "---" or "..." unless it
                // is followed by a non-space.
                bool containsForbiddenContent = (content.StartsWith("---") || content.StartsWith("...")) &&
                                                (content.Length <= 3 || content[3] == ' ');

                // [ 21] c-indicator            ::=   “-” | “?” | “:” | “,” | “[” | “]” | “{” | “}”
                //                                  | “#” | “&” | “*” | “!” | “|” | “>” | “'” | “"”
                //                                  | “%” | “@” | “`”
                // [156] ns-plain-char(c)       ::= nb-plain-char(c) - #x20 /*SP*/
                // [157] ns-plain-first-char(c) ::=   ( ns-plain-char(c) - c-indicator )
                //                                  | ( ( “-” | “?” | “:” ) ns-plain-char(c) )
                // Plain scalars cannot start with an indicator, unless it is '-', '?' or ':'
                // followed by a non-space.
                // Assume all code points are `[153] nb-plain-char(c)`s and the first and last code
                // points are `[156] ns-plain-char(c)`s.
                if (!YamlScalarValidator.Plain.HasValidFirstChar(content))
                {
                    return YamlNodeValidity.None;
                }

                // [160] ns-plain-single(c) ::= ( ns-plain-first-char(c) ( nb-plain-char(c)* ns-plain-char(c) )? )
                //                              - l-forbidden-content
                bool allCodePointsAreValidMultilineCharOuts = true;
                bool allCodePointsAreValidSingleLineChars   = true;
                bool allCodePointsAreValidCharIns           = true;
                for (int i = 0; i < content.Length; i++)
                {
                    char c = content[i];

                    // [ 34] nb-char             ::= c-printable - b-char
                    // [154] nb-plain-char-out   ::=   ( nb-char - “:” - “#” - #x9 /*TAB*/ )
                    //                               | ( ns-plain-char(flow-out) “#” )
                    //                               | ( “:” ns-plain-char(flow-out) )
                    // [ 84] b-l-folded-any(n,s) ::=   b-l-folded-specific(n,s)
                    //                               | b-l-folded-as-space
                    //                               | b-l-folded-trimmed(n,s)
                    // Plain scalars can contain `[  1] c-printable`s.
                    // Plain scalars cannot contain non-`[  1] c-printable`s.
                    // Plain scalars cannot contain `[ 27] b-char`s.
                    // Plain scalars cannot contain tabs.
                    allCodePointsAreValidMultilineCharOuts &=
                        YamlScalarValidator.Plain.IsValidCharOutOrMultilineChar(c);

                    // [155] nb-plain-char-in ::= nb-plain-char-out - “,” - “[” - “]” - “{” - “}”
                    // Plain scalars cannot contain ',', '[', ']', '{' or '}' in the `flow-in` and
                    // `flow-key` contexts.
                    allCodePointsAreValidCharIns &=
                        YamlScalarValidator.Plain.IsCharOutValidCharIn(c);

                    // [154] nb-plain-char-out ::=   ( nb-char - “:” - “#” - #x9 /*TAB*/ )
                    //                             | ( ns-plain-char(flow-out) “#” )
                    //                             | ( “:” ns-plain-char(flow-out) )
                    // Plain scalars cannot contain '#'s not preceded by a non-space.
                    // Plain scalars cannot contain ':'s not followed by a non-space.
                    if ((c == '#' && (i == 0 || content[i - 1] == ' ')) ||
                        (c == ':' && (i == content.Length - 1 || content[i + 1] == ' ')))
                    {
                        allCodePointsAreValidMultilineCharOuts = false;
                    }

                    if (YamlScalarValidator.IsNormalizedLineBreak(c))
                    {
                        // [ 84] b-l-folded-any(n,s)
                        // Plain scalars can contain `[ 84] b-l-folded-any(n,s)` line breaks in a
                        // `flow-out` or `flow-in` context.
                        allCodePointsAreValidSingleLineChars = false;

                        // [ 77] s-ignored-prefix-plain(n)
                        // Plain scalars cannot contain spaces after a folded line break.
                        if (i < content.Length - 1 && content[i + 1] == ' ')
                        {
                            allCodePointsAreValidMultilineCharOuts = false;
                        }
                    }

                    // [  1] c-printable ::=   #x9 | #xA | #xD | [#x20-#x7E]          /* 8 bit */
                    //                       | #x85 | [#xA0-#xD7FF] | [#xE000-#xFFFD] /* 16 bit */
                    //                       | [#x10000-#x10FFFF]                     /* 32 bit */
                    // All high and low surrogates must be part of a surrogate pair.
                    if ((char.IsHighSurrogate(c) &&
                         (i == content.Length - 1 || !char.IsLowSurrogate(content[i + 1]))) ||
                        (char.IsLowSurrogate(c) &&
                         (i == 0 || !char.IsHighSurrogate(content[i - 1]))))
                    {
                        allCodePointsAreValidMultilineCharOuts = false;
                    }

                    if (!allCodePointsAreValidMultilineCharOuts)
                    {
                        // Don't bother checking that the rest of the code points satisfy
                        // `nb-plain-char-in`, since the plain scalar is invalid anyway.
                        break;
                    }
                }

                if (!allCodePointsAreValidMultilineCharOuts)
                {
                    return YamlNodeValidity.None;
                }

                return YamlNodeValidity.Flow(
                    // Scalars at the document root have the `flow-out` context
                    atRoot:    !containsForbiddenContent,
                    inFlowOut: true,
                    inFlowIn:  allCodePointsAreValidCharIns,
                    inFlowKey: allCodePointsAreValidCharIns & allCodePointsAreValidSingleLineChars
                );
            }

            /// <summary>
            ///     Determines whether the first code point of the given plain scalar content is
            ///     valid according to <c>[157] ns-plain-first-char(c)</c>.
            /// </summary>
            /// <remarks>
            ///     <c>[157] ns-plain-first-char(c)</c> is defined as follows:
            ///     <code>
            ///         [ 21] c-indicator            ::=   “-” | “?” | “:” | “,” | “[” | “]” | “{” | “}”
            ///                                          | “#” | “&amp;” | “*” | “!” | “|” | “>” | “'” | “"”
            ///                                          | “%” | “@” | “`”
            ///         [156] ns-plain-char(c)       ::= nb-plain-char(c) - #x20 /*SP*/
            ///         [157] ns-plain-first-char(c) ::=   ( ns-plain-char(c) - c-indicator )
            ///                                          | ( ( “-” | “?” | “:” ) ns-plain-char(c) )
            ///     </code>
            ///
            ///     Assumes that:<br/>
            ///     <list type="bullet">
            ///         <item>
            ///             All code points are valid <c>[153] nb-plain-char(c)</c>s or
            ///             <c>[ 84] b-l-folded-any(n,s)</c> line breaks.
            ///         </item>
            ///         <item>
            ///             The first and last code points satisfy <c>[156] ns-plain-char(c)</c>, as
            ///             described by <c>[160] ns-plain-single(c)</c>.
            ///         </item>
            ///     </list>
            /// </remarks>
            /// <param name="content">The plain scalar content to check.</param>
            /// <returns>
            ///     <c>true</c> if the first code point of <paramref name="content"/> satisfies
            ///     <c>[157] ns-plain-first-char(c)</c>;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            internal static bool HasValidFirstChar(string content)
            {
                // [157] ns-plain-first-char(c) ::=   ( ns-plain-char(c) - c-indicator )
                //                                  | ( ( “-” | “?” | “:” ) ns-plain-char(c) )
                if (content.Length == 0) { return false; }

                // Check for `- c-indicator`
                char firstCodePoint = content[0];
                bool firstCodePointIsForbiddenIndicator =
                    YamlScalarValidator.Plain.IsIndicator(firstCodePoint);

                // Re-allow `( ( “-” | “?” | “:” ) ns-plain-char(c) )`
                const ulong indicatorsAllowedBeforeNonSpace = 1UL << '-' | 1UL << ':' | 1UL << '?';
                if (firstCodePointIsForbiddenIndicator &&
                    firstCodePoint < 64 &&
                    ((1UL << firstCodePoint) & indicatorsAllowedBeforeNonSpace) != 0 &&
                    content.Length > 1 &&
                    content[1] != ' ')
                {
                    firstCodePointIsForbiddenIndicator = false;
                }

                return !firstCodePointIsForbiddenIndicator;
            }

            /// <summary>
            ///     Determines whether the given code point is a <c>[ 21] c-indicator</c>.
            /// </summary>
            /// <remarks>
            ///     <c>[ 21] c-indicator</c> is defined as follows:
            ///     <code>
            ///         [ 21] c-indicator ::=   “-” | “?” | “:” | “,” | “[” | “]” | “{” | “}”
            ///                               | “#” | “&amp;” | “*” | “!” | “|” | “>” | “'” | “"”
            ///                               | “%” | “@” | “`”
            ///     </code>
            /// </remarks>
            /// <param name="c">The code point to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="c"/> is a <c>[ 21] c-indicator</c>;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsIndicator(char c)
            {
                return Sse2.IsSupported ?
                    YamlScalarValidator.Plain.IsIndicatorIntrinsic(c) :
                    YamlScalarValidator.Plain.IsIndicatorNonIntrinsic(c);
            }

            /// <inheritdoc cref="IsIndicator(char)"/>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsIndicatorNonIntrinsic(char c)
            {
                if (c > 127) { return false; }

                const ulong indicatorsLow = 1UL << '!' | 1UL << '"' | 1UL << '#' |
                                            1UL << '%' | 1UL << '&' | 1UL << '\'' |
                                            1UL << '*' |
                                            1UL << ',' | 1UL << '-' |
                                            1UL << ':' |
                                            1UL << '>' | 1UL << '?';
                const ulong indicatorsHigh = 1UL << ('@' - 64) |
                                             1UL << ('[' - 64) |
                                             1UL << (']' - 64) |
                                             1UL << ('`' - 64) |
                                             1UL << ('{' - 64) | 1UL << ('|' - 64) | 1UL << ('}' - 64);

                ulong indicators = c < 64 ? indicatorsLow : indicatorsHigh;
                return ((1UL << (c & 63)) & indicators) != 0;
            }

            /// <inheritdoc cref="IsIndicator(char)"/>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsIndicatorIntrinsic(char c)
            {
                if (!Avx2.IsSupported)
                {
                    return YamlScalarValidator.Plain.IsIndicatorNonIntrinsic(c);
                }

                if (c > 127) { return false; }

                Vector256<byte> indicatorCodePoints = Vector256.Create(
                    (byte)'!', (byte)'"', (byte)'#', (byte)'%',
                    (byte)'&', (byte)'\'', (byte)'*', (byte)',',
                    (byte)'-', (byte)':', (byte)'>', (byte)'?',
                    (byte)'@', (byte)'[', (byte)']', (byte)'`',
                    (byte)'{', (byte)'|', (byte)'}', (byte)'}',
                    (byte)'}', (byte)'}', (byte)'}', (byte)'}',
                    (byte)'}', (byte)'}', (byte)'}', (byte)'}',
                    (byte)'}', (byte)'}', (byte)'}', (byte)'}'
                );
                Vector256<byte> v = Vector256.Create((byte)c);
                int equals = Avx2.MoveMask(Avx2.CompareEqual(indicatorCodePoints, v));
                return equals != 0;
            }

            /// <summary>
            ///     Determines whether the given code point is a valid
            ///     <c>[154] nb-plain-char-out</c> or <c>[ 84] b-l-folded-any(n,s)</c> line break.
            /// </summary>
            /// <remarks>
            ///     <c>[154] nb-plain-char-out</c> is defined as follows:
            ///     <code>
            ///         [  1] c-printable              ::=   #x9 | #xA | #xD | [#x20-#x7E]          /* 8 bit */
            ///                                            | #x85 | [#xA0-#xD7FF] | [#xE000-#xFFFD] /* 16 bit */
            ///                                            | [#x10000-#x10FFFF]                     /* 32 bit */
            ///         [ 27] b-char                   ::=   b-line-feed | b-carriage-return | b-next-line
            ///                                            | b-line-separator | b-paragraph-separator
            ///         [ 34] nb-char                  ::= c-printable - b-char
            ///         [154] nb-plain-char-out        ::=   ( nb-char - “:” - “#” - #x9 /*TAB*/ )
            ///                                            | ( ns-plain-char(flow-out) “#” )
            ///                                            | ( “:” ns-plain-char(flow-out) )
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
            ///             The given code point is preceded by a valid
            ///             <c>[156] ns-plain-char(flow-out)</c> if it is a '#'.
            ///         </item>
            ///         <item>
            ///             The given code point is succeeded by a valid
            ///             <c>[156] ns-plain-char(flow-out)</c> if it is a ':'.
            ///         </item>
            ///         <item>
            ///             The given code point is part of a valid surrogate pair if it is a high
            ///             or low surrogate.
            ///         </item>
            ///     </list>
            /// </remarks>
            /// <param name="c">The code point to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="c"/> is a valid <c>[154] nb-plain-char-out</c> or
            ///     <c>[ 84] b-l-folded-any(n,s)</c> line break;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsValidCharOutOrMultilineChar(char c)
            {
                return Popcnt.IsSupported && Sse2.IsSupported ?
                    YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharIntrinsic(c) :
                    YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharNonIntrinsic(c);
            }

            /// <inheritdoc cref="IsValidCharOutOrMultilineChar(char)"/>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsValidCharOutOrMultilineCharNonIntrinsic(char c)
            {
                return c == '\n' |
                       (uint)(c - 0x0020) <= 0x007E - 0x0020 |
                       (uint)(c - 0x00A0) <= 0xFFFD - 0x00A0;
            }

            /// <inheritdoc cref="IsValidCharOutOrMultilineChar(char)"/>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsValidCharOutOrMultilineCharIntrinsic(char c)
            {
                if (!Popcnt.IsSupported || !Sse2.IsSupported)
                {
                    return YamlScalarValidator.Plain.IsValidCharOutOrMultilineCharNonIntrinsic(c);
                }

                Vector128<short> rangeThresholds = Vector128.Create(
                    0x000A - 1, 0x000A,
                    0x0020 - 1, 0x007E,
                    0x00A0 - 1, 0xFFFD - 65536,
                    0x7FFF,     0x7FFF
                );
                Vector128<short> v = Vector128.Create((short)c);
                Vector128<short> greaterThan = Sse2.CompareGreaterThan(v, rangeThresholds);
                return (Popcnt.PopCount((uint)Sse2.MoveMask(greaterThan.AsByte())) & 2) == 0;
            }

            /// <summary>
            ///     Determines whether the given <c>[154] nb-plain-char-out</c> is a valid
            ///     <c>[155] nb-plain-char-in</c>.
            /// </summary>
            /// <remarks>
            ///     <c>[155] nb-plain-char-in</c> is defined as follows:
            ///     <code>
            ///         [155] nb-plain-char-in ::= nb-plain-char-out - “,” - “[” - “]” - “{” - “}”
            ///     </code>
            ///
            ///     Assumes that:<br/>
            ///     <list type="bullet">
            ///         <item>
            ///             The given code point already satisfies <c>[154] nb-plain-char-out</c>.
            ///         </item>
            ///     </list>
            /// </remarks>
            /// <param name="c">The code point to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="c"/> is a valid
            ///     <c>[155] nb-plain-char-in</c>;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsCharOutValidCharIn(char c)
            {
                return Sse2.IsSupported ?
                    YamlScalarValidator.Plain.IsCharOutValidCharInIntrinsic(c) :
                    YamlScalarValidator.Plain.IsCharOutValidCharInNonIntrinsic(c);
            }

            /// <inheritdoc cref="IsCharOutValidCharIn(char)"/>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsCharOutValidCharInNonIntrinsic(char c)
            {
                if (c > 127) { return true; }

                if (c < 64) { return c != ','; }

                const ulong forbiddenCodePointsHigh = 1UL << ('[' - 64) | 1UL << (']' - 64) |
                                                      1UL << ('{' - 64) | 1UL << ('}' - 64);
                return ((1UL << (c & 63)) & forbiddenCodePointsHigh) == 0;
            }

            /// <inheritdoc cref="IsCharOutValidCharIn(char)"/>
            [MethodImpl(
                MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization
            )]
            internal static bool IsCharOutValidCharInIntrinsic(char c)
            {
                if (!Sse2.IsSupported)
                {
                    return YamlScalarValidator.Plain.IsCharOutValidCharInNonIntrinsic(c);
                }

                Vector128<ushort> forbiddenCodePoints = Vector128.Create(
                    ',', ',', ',', ',',
                    '[', ']', '{', '}'
                );
                Vector128<ushort> v = Vector128.Create(c);
                int equals = Sse2.MoveMask(Sse2.CompareEqual(forbiddenCodePoints, v).AsByte());
                return equals == 0;
            }
        }
    }
}
