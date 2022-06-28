using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Provides methods for parsing YAML 1.1 presentations of numbers.
    /// </summary>
    internal class YamlNumberParser
    {
        /// <summary>
        ///     Parses an optional '+' or '-' sign, <c>/[-+]?/</c>, from the given string.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="startIndex">
        ///     The index within the string from which to start parsing.
        /// </param>
        /// <returns>
        ///     A tuple containing:<br/>
        ///     <list type="bullet">
        ///         <item>The index of the first unparsed character.</item>
        ///         <item>
        ///             The sign, if present;<br/>
        ///             <c>null</c> otherwise.
        ///         </item>
        ///     </list>
        /// </returns>
        public static (int EndIndex, char? Sign) Sign(string input, int startIndex)
        {
            // /[-+]?/
            if (startIndex >= input.Length) { return (startIndex, null); }

            if (input[startIndex] == '+') { return (startIndex + 1, '+'); }
            if (input[startIndex] == '-') { return (startIndex + 1, '-'); }

            return (startIndex, null);
        }

        /// <summary>
        ///     Parses a base 60 component, <c>/:[0-5]?[0-9]/</c>, from the given string.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="startIndex">
        ///     The index within the string from which to start parsing.
        /// </param>
        /// <returns>
        ///     A tuple containing:<br/>
        ///     <list type="bullet">
        ///         <item>The index of the first unparsed character.</item>
        ///         <item>
        ///             The base 60 component if successfully parsed;<br/>
        ///             <c>null</c> otherwise.
        ///         </item>
        ///     </list>
        /// </returns>
        public static (int EndIndex, ulong? Value) Base60Component(string input, int startIndex)
        {
            int index = startIndex;

            if (index >= input.Length) { return (startIndex, null); }

            // /:/
            if (input[index] != ':') { return (startIndex, null); }
            index++;

            ulong component;

            // /[0-9]/
            if (index < input.Length &&
                '0' <= input[index] &&
                input[index] <= '9')
            {
                component = (ulong)(input[index] - '0');
                index++;
            }
            else
            {
                return (startIndex, null);
            }

            // /[0-9]?/
            if (index < input.Length &&
                '0' <= input[index] &&
                input[index] <= '9')
            {
                component *= 10;
                component += (ulong)(input[index] - '0');
                index++;
            }

            if (component >= 60) { return (startIndex, null); }

            return (index, component);
        }
    }
}
