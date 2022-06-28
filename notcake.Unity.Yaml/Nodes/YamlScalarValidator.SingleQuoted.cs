using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace notcake.Unity.Yaml.Nodes
{
    public static partial class YamlScalarValidator
    {
        /// <summary>
        ///     Provides methods for validating the content of YAML 1.1 single quoted scalars.
        /// </summary>
        /// <remarks>
        ///     A YAML 1.1 single quoted scalar is defined by the following rules:
        ///     <code>
        ///         [  1] c-printable                ::=   #x9 | #xA | #xD | [#x20-#x7E]          /* 8 bit */
        ///                                              | #x85 | [#xA0-#xD7FF] | [#xE000-#xFFFD] /* 16 bit */
        ///                                              | [#x10000-#x10FFFF]                     /* 32 bit */
        ///         [ 27] b-char                     ::=   b-line-feed | b-carriage-return | b-next-line
        ///                                              | b-line-separator | b-paragraph-separator
        ///         [ 28] b-specific                 ::= b-line-separator | b-paragraph-separator
        ///         [ 29] b-generic                  ::=   ( b-carriage-return b-line-feed ) /* DOS, Windows */
        ///                                              | b-carriage-return                 /* Macintosh */
        ///                                              | b-line-feed                       /* UNIX */
        ///                                              | b-next-line                       /* Unicode */
        ///         [ 30] b-as-line-feed             ::= b-generic
        ///         [ 31] b-normalized               ::= b-as-line-feed | b-specific
        ///         [ 32] b-ignored-generic          ::= b-generic
        ///         [ 34] nb-char                    ::= c-printable - b-char
        ///         [ 35] s-ignored-space            ::= #x20 /*SP*/
        ///         [ 36] s-white                    ::= #x9 /*TAB*/ | #x20 /*SP*/
        ///         [ 37] s-ignored-white            ::= s-white
        ///         [ 66] s-indent(n)                ::= s-ignored-space x n
        ///         [ 76] s-ignored-prefix(n,s)      ::= s = plain   ⇒ s-ignored-prefix-plain(n)
        ///                                              s = double  ⇒ s-ignored-prefix-quoted(n)
        ///                                              s = single  ⇒ s-ignored-prefix-quoted(n)
        ///                                              s = literal ⇒ s-ignored-prefix-block(n)
        ///                                              s = folded  ⇒ s-ignored-prefix-block(n)
        ///         [ 78] s-ignored-prefix-quoted(n) ::= s-indent(n) s-ignored-white*
        ///         [ 80] l-empty(n,s)               ::= ( s-indent(&lt;n) | s-ignored-prefix(n,s) )
        ///                                              b-normalized
        ///         [ 81] b-l-folded-specific(n,s)   ::= b-specific l-empty(n,s)*
        ///         [ 82] b-l-folded-as-space        ::= b-generic
        ///         [ 83] b-l-folded-trimmed(n,s)    ::= b-ignored-generic l-empty(n,s)+
        ///         [ 84] b-l-folded-any(n,s)        ::=   b-l-folded-specific(n,s)
        ///                                              | b-l-folded-as-space
        ///                                              | b-l-folded-trimmed(n,s)
        ///         [141] c-quoted-quote             ::= “'” “'”
        ///         [142] nb-single-char             ::= ( nb-char - “"” ) | c-quoted-quote
        ///         [143] ns-single-char             ::= nb-single-char - s-white
        ///         [144] c-single-quoted(n,c)       ::= “'” nb-single-text(n,c) “'”
        ///         [145] nb-single-text(n,c)        ::= c = flow-out ⇒ nb-single-any(n)
        ///                                              c = flow-in  ⇒ nb-single-any(n)
        ///                                              c = flow-key ⇒ nb-single-single(n)
        ///         [146] nb-single-any(n)           ::= nb-single-single(n) | nb-single-multi(n)
        ///         [147] nb-single-single(n)        ::= nb-single-char*
        ///         [148] s-l-single-break(n)        ::= s-ignored-white* b-l-folded-any(n,single)
        ///         [149] nb-single-multi(n)         ::= nb-l-single-first(n)
        ///                                              l-single-inner(n)*
        ///                                              s-nb-single-last(n)
        ///         [150] nb-l-single-first(n)       ::= ( nb-single-char* ns-single-char )?
        ///                                              s-l-single-break(n)
        ///         [151] l-single-inner(n)          ::= s-ignored-prefix(n, single) ns-single-char
        ///                                              (nb-single-char* ns-single-char )?
        ///                                              s-l-single-break(n)
        ///         [152] s-nb-single-last(n)        ::= s-ignored-prefix(n, single)
        ///                                              (ns-single-char nb-single-char* )?
        ///     </code>
        /// </remarks>
        public static class SingleQuoted
        {
            /// <summary>
            ///     Determines whether the given content can be presented as a single quoted scalar.
            /// </summary>
            /// <param name="content">The single quoted scalar content to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="content"/> can be presented as a single quoted
            ///     scalar;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            public static YamlNodeValidity IsValid(string content)
            {
                bool allCodePointsAreValidSingleLineChars = true;

                // [147] nb-single-single(n) ::= nb-single-char*
                for (int i = 0; i < content.Length; i++)
                {
                    char c = content[i];

                    // [ 34] nb-char             ::= c-printable - b-char
                    // [142] nb-single-char      ::= ( nb-char - “"” ) | c-quoted-quote
                    // [ 84] b-l-folded-any(n,s) ::=   b-l-folded-specific(n,s)
                    //                               | b-l-folded-as-space
                    //                               | b-l-folded-trimmed(n,s)
                    // Single quoted scalars can contain `[  1] c-printable`s.
                    // Single quoted scalars cannot contain non-`[  1] c-printable`s.
                    // Single quoted scalars cannot contain `[ 27] b-char`s.
                    // Single quoted scalars can contain tabs.
                    // Single quoted scalars can contain backslashes and double quotes.
                    // Single quoted scalars can contain single quotes.
                    if (!YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreak(c))
                    {
                        return YamlNodeValidity.None;
                    }

                    if (YamlScalarValidator.IsNormalizedLineBreak(c))
                    {
                        // [ 84] b-l-folded-any(n,s)
                        // Single quoted can contain `[ 84] b-l-folded-any(n,s)` line breaks in a
                        // `flow-out` or `flow-in` context.
                        allCodePointsAreValidSingleLineChars = false;

                        // [ 78] s-ignored-prefix-quoted(n)
                        // Single quoted scalars cannot contain spaces or tabs after a folded line
                        // break.
                        if (i < content.Length - 1 &&
                            (content[i + 1] == ' ' | content[i + 1] == '\t'))
                        {
                            return YamlNodeValidity.None;
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
                        return YamlNodeValidity.None;
                    }
                }

                return YamlNodeValidity.Flow(
                    atRoot:    true,
                    inFlowOut: true,
                    inFlowIn:  true,
                    inFlowKey: allCodePointsAreValidSingleLineChars
                );
            }
        }
    }
}
