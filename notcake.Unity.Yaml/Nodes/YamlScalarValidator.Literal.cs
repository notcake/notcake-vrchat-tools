using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace notcake.Unity.Yaml.Nodes
{
    public static partial class YamlScalarValidator
    {
        /// <summary>
        ///     Provides methods for validating the content of YAML 1.1 literal scalars.
        /// </summary>
        /// <remarks>
        ///     A YAML 1.1 literal scalar is defined by the following rules:
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
        ///         [ 33] b-ignored-any              ::= b-generic | b-specific
        ///         [ 34] nb-char                    ::= c-printable - b-char
        ///         [ 35] s-ignored-space            ::= #x20 /*SP*/
        ///         [ 36] s-white                    ::= #x9 /*TAB*/ | #x20 /*SP*/
        ///         [ 37] s-ignored-white            ::= s-white
        ///         [ 39] ns-dec-digit               ::= [#x30-#x39] /*0-9*/
        ///         [ 66] s-indent(n)                ::= s-ignored-space x n
        ///         [ 76] s-ignored-prefix(n,s)      ::= s = plain   ⇒ s-ignored-prefix-plain(n)
        ///                                              s = double  ⇒ s-ignored-prefix-quoted(n)
        ///                                              s = single  ⇒ s-ignored-prefix-quoted(n)
        ///                                              s = literal ⇒ s-ignored-prefix-block(n)
        ///                                              s = folded  ⇒ s-ignored-prefix-block(n)
        ///         [ 79] s-ignored-prefix-block(n)  ::= s-indent(n)
        ///         [ 80] l-empty(n,s)               ::= ( s-indent(&lt;n) | s-ignored-prefix(n,s) )
        ///                                                 b-normalized
        ///         [164] c-b-block-header(s,m,t)    ::= c-style-indicator(s)
        ///                                              ( ( c-indentation-indicator(m)
        ///                                                  c-chomping-indicator(t) )
        ///                                              | ( c-chomping-indicator(t)
        ///                                                  c-indentation-indicator(m) ) )
        ///                                              s-b-comment
        ///         [165] c-style-indicator(s)       ::= s = literal ⇒ “|”
        ///                                              s = folded  ⇒ “>”
        ///         [166] c-indentation-indicator(m) ::= explicit(m) ⇒ ns-dec-digit - “0”
        ///                                              detect(m)   ⇒ /* empty */
        ///         [167] c-chomping-indicator(t)    ::= t = strip ⇒ “-”
        ///                                              t = clip  ⇒ /* empty */
        ///                                              t = keep  ⇒ “+”
        ///         [168] b-chomped-last(t)          ::= t = strip ⇒ b-strip-last
        ///                                              t = clip  ⇒ b-keep-last
        ///                                              t = keep  ⇒ b-keep-last
        ///         [169] b-strip-last               ::= b-ignored-any
        ///         [170] b-keep-last                ::= b-normalized
        ///         [171] l-chomped-empty(n,t)       ::= t = strip ⇒ l-strip-empty(n)
        ///                                              t = clip  ⇒ l-strip-empty(n)
        ///                                              t = keep  ⇒ l-keep-empty(n)
        ///         [172] l-strip-empty(n)           ::= ( s-indent(≤n) b-ignored-any )* l-trail-comments(n)?
        ///         [173] l-keep-empty(n)            ::= l-empty(n,literal)* l-trail-comments(n)?
        ///         [174] l-trail-comments(n)        ::= s-indent(&lt;n) c-nb-comment-text b-ignored-any
        ///                                              l-comment*
        ///         [175] c-l+literal(n)             ::= c-b-block-header(literal,m,t)
        ///                                              l-literal-content(n+m,t)
        ///         [176] l-nb-literal-text(n)       ::= l-empty(n,block)* s-indent(n) nb-char+
        ///         [177] l-literal-inner(n)         ::= l-nb-literal-text(n) b-normalized
        ///         [178] l-literal-last(n,t)        ::= l-nb-literal-text(n) b-chomped-last(t)
        ///         [179] l-literal-content(n,t)     ::= ( l-literal-inner(n)* l-literal-last(n,t) )?
        ///                                              l-chomped-empty(n,t)?
        ///     </code>
        /// </remarks>
        public static class Literal
        {
            /// <summary>
            ///     Determines whether the given content can be presented as a literal scalar.
            /// </summary>
            /// <param name="content">The literal scalar content to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="content"/> can be presented as a literal
            ///     scalar;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            public static YamlNodeValidity IsValid(string content)
            {
                // [176] l-nb-literal-text(n) ::= l-empty(n,block)* s-indent(n) nb-char+
                for (int i = 0; i < content.Length; i++)
                {
                    char c = content[i];

                    // [ 31] b-normalized ::= b-as-line-feed | b-specific
                    // [ 34] nb-char      ::= c-printable - b-char
                    // Literal scalars can contain `[  1] c-printable`s.
                    // Literal scalars cannot contain non-`[  1] c-printable`s.
                    // Literal scalars can contain tabs.
                    // Literal scalars cannot contain `[ 27] b-char`s.
                    // Literal scalars can contain normalized `[ 30] b-as-line-feed`s and
                    // `[ 28] b-specific` line breaks.
                    if (!YamlScalarValidator.IsNonBreakingCharOrFoldedLineBreak(c))
                    {
                        return YamlNodeValidity.None;
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

                return YamlNodeValidity.Block();
            }
        }
    }
}
