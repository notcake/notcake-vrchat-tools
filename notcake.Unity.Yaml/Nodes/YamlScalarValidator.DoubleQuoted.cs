using System.Diagnostics.CodeAnalysis;

namespace notcake.Unity.Yaml.Nodes
{
    public static partial class YamlScalarValidator
    {
        /// <summary>
        ///     Provides methods for validating the content of YAML 1.1 double quoted scalars.
        /// </summary>
        /// <remarks>
        ///     A YAML 1.1 double quoted scalar is defined by the following rules:
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
        ///         [ 39] ns-dec-digit               ::= [#x30-#x39] /*0-9*/
        ///         [ 40] ns-hex-digit               ::= ns-dec-digit | [#x41-#x46] /*A-F*/ | [#x61-#x66] /*a-f*/
        ///         [ 46] ns-esc-null                ::= “\” “0”
        ///         [ 47] ns-esc-bell                ::= “\” “a”
        ///         [ 48] ns-esc-backspace           ::= “\” “b”
        ///         [ 49] ns-esc-horizontal-tab      ::= “\” “t” | “\” #x9
        ///         [ 50] ns-esc-line-feed           ::= “\” “n”
        ///         [ 51] ns-esc-vertical-tab        ::= “\” “v”
        ///         [ 52] ns-esc-form-feed           ::= “\” “f”
        ///         [ 53] ns-esc-carriage-return     ::= “\” “r”
        ///         [ 54] ns-esc-escape              ::= “\” “e”
        ///         [ 55] ns-esc-space               ::= “\” #x20
        ///         [ 56] ns-esc-double-quote        ::= “\” “"”
        ///         [ 57] ns-esc-backslash           ::= “\” “\”
        ///         [ 58] ns-esc-next-line           ::= “\” “N”
        ///         [ 59] ns-esc-non-breaking-space  ::= “\” “_”
        ///         [ 60] ns-esc-line-separator      ::= “\” “L”
        ///         [ 61] ns-esc-paragraph-separator ::= “\” “P”
        ///         [ 62] ns-esc-8-bit               ::= “\” “x” ( ns-hex-digit x 2 )
        ///         [ 63] ns-esc-16-bit              ::= “\” “u” ( ns-hex-digit x 4 )
        ///         [ 64] ns-esc-32-bit              ::= “\” “U” ( ns-hex-digit x 8 )
        ///         [ 65] ns-esc-char                ::=   ns-esc-null | ns-esc-bell | ns-esc-backspace
        ///                                              | ns-esc-horizontal-tab | ns-esc-line-feed
        ///                                              | ns-esc-vertical-tab | ns-esc-form-feed
        ///                                              | ns-esc-carriage-return | ns-esc-escape | ns-esc-space
        ///                                              | ns-esc-double-quote | ns-esc-backslash
        ///                                              | ns-esc-next-line | ns-esc-non-breaking-space
        ///                                              | ns-esc-line-separator | ns-esc-paragraph-separator
        ///                                              | ns-esc-8-bit | ns-esc-16-bit | ns-esc-32-bit
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
        ///         [128] nb-double-char             ::= ( nb-char - “\” - “"” ) | ns-esc-char
        ///         [129] ns-double-char             ::= nb-double-char - s-white
        ///         [130] c-double-quoted(n,c)       ::= “"” nb-double-text(n,c) “"”
        ///         [131] nb-double-text(n,c)        ::= c = flow-out ⇒ nb-double-any(n)
        ///                                              c = flow-in  ⇒ nb-double-any(n)
        ///                                              c = flow-key ⇒ nb-double-single
        ///         [132] nb-double-any(n)           ::= nb-double-single | nb-double-multi(n)
        ///         [133] nb-double-single           ::= nb-double-char*
        ///         [134] s-l-double-folded(n)       ::= s-ignored-white* b-l-folded-any(n,double)
        ///         [135] s-l-double-escaped(n)      ::= s-white* “\” b-ignored-any
        ///                                              l-empty(n,double)*
        ///         [136] s-l-double-break(n)        ::= s-l-double-folded(n) | s-l-double-escaped(n)
        ///         [137] nb-double-multi(n)         ::= nb-l-double-first(n)
        ///                                              l-double-inner(n)*
        ///                                              s-nb-double-last(n)
        ///         [138] nb-l-double-first(n)       ::= ( nb-double-char* ns-double-char )?
        ///                                              s-l-double-break(n)
        ///         [139] l-double-inner(n)          ::= s-ignored-prefix(n,double) ns-double-char
        ///                                              ( nb-double-char* ns-double-char )?
        ///                                              s-l-double-break(n)
        ///         [140] s-nb-double-last(n)        ::= s-ignored-prefix(n,double)
        ///                                              ( ns-double-char nb-double-char* )?
        ///     </code>
        /// </remarks>
        public static class DoubleQuoted
        {
            /// <summary>
            ///     Determines whether the given content can be presented as a double quoted scalar.
            /// </summary>
            /// <param name="content">The double quoted scalar content to check.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="content"/> can be presented as a double quoted
            ///     scalar;<br/>
            ///     <c>false</c> otherwise.
            /// </returns>
            [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
            public static YamlNodeValidity IsValid(string content)
            {
                return YamlNodeValidity.Flow();
            }
        }
    }
}
