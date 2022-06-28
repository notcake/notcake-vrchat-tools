using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents an integer node in a YAML document.
    /// </summary>
    /// <remarks>
    ///     See http://yaml.org/type/int.html for a list of valid integer presentations.
    /// </remarks>
    public class YamlInteger : YamlSingleLineScalar<YamlInteger>
    {
        /// <summary>
        ///     Gets the value of the integer node as a signed 64-bit integer.
        /// </summary>
        public long? Int64Value { get; }

        /// <summary>
        ///     Gets the value of the integer node as an unsigned 64-bit integer.
        /// </summary>
        public ulong? UInt64Value { get; }

        /// <inheritdoc cref="YamlInteger(long?, ulong?, string)"/>
        /// <param name="value">
        ///     <inheritdoc cref="YamlInteger(long?, ulong?, string)"
        ///                 path="/param[@name='int64Value']"/>
        /// </param>
        public YamlInteger(long value) :
            this(
                value,
                value >= 0 ? (ulong?)value : null,
                value.ToString(CultureInfo.InvariantCulture)
            )
        {
        }

        /// <inheritdoc cref="YamlInteger(long?, ulong?, string)"/>
        /// <param name="value">
        ///     <inheritdoc cref="YamlInteger(long?, ulong?, string)"
        ///                 path="/param[@name='uint64Value']"/>
        /// </param>
        public YamlInteger(ulong value) :
            this(
                value < 0x80000000_00000000 ? (long?)value : null,
                value,
                value.ToString(CultureInfo.InvariantCulture)
            )
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlInteger"/> class.
        /// </summary>
        /// <param name="int64Value">
        ///     The value of the integer node, as a signed 64-bit integer.
        /// </param>
        /// <param name="uint64Value">
        ///     The value of the integer node, as an unsigned 64-bit integer.
        /// </param>
        /// <param name="presentation">The YAML presentation of the integer node.</param>
        private YamlInteger(long? int64Value, ulong? uint64Value, string presentation) :
            base(presentation)
        {
            this.Int64Value  = int64Value;
            this.UInt64Value = uint64Value;
        }

        #region Object
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Int64Value, this.UInt64Value);
        }
        #endregion

        #region YamlScalar<YamlInteger>
        public override bool Equals(YamlInteger other)
        {
            return this.Int64Value == other.Int64Value &&
                   this.UInt64Value == other.UInt64Value;
        }
        #endregion

        /// <summary>
        ///     Parses 0 or more digits, <c>/[0-9a-zA-Z_]*/</c>, from the given string in the given
        ///     base.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="startIndex">
        ///     The index within the string from which to start parsing.
        /// </param>
        /// <param name="base">
        ///     The base of the digits.
        ///     <para/>
        ///     Must be at most 36.
        /// </param>
        /// <returns>
        ///     A tuple containing:<br/>
        ///     <list type="bullet">
        ///         <item>The index of the first unparsed character.</item>
        ///         <item>The parsed value as an unsigned 64-bit integer.</item>
        ///         <item>
        ///             A boolean indicating whether parsing overflowed the unsigned 64-bit integer.
        ///         </item>
        ///     </list>
        /// </returns>
        public static (int EndIndex, ulong Value, bool Overflowed) ParseDigits(
            string input,
            int startIndex,
            ulong @base
        )
        {
            int index = startIndex;

            bool overflowed = false;
            ulong value = 0;

            // /[0-9a-zA-Z_]*/
            do
            {
                if (input[index] == '_')
                {
                    index++;
                    continue;
                }

                ulong digit;
                if ('0' <= input[index] &&
                    input[index] <= '9')
                {
                    digit = (ulong)(input[index] - '0');
                }
                else if ('a' <= input[index] &&
                         input[index] <= 'z')
                {
                    digit = (ulong)(input[index] - 'a' + 10);
                }
                else if ('A' <= input[index] &&
                         input[index] <= 'Z')
                {
                    digit = (ulong)(input[index] - 'A' + 10);
                }
                else
                {
                    break;
                }

                if (digit >= @base)
                {
                    break;
                }

                // Only advance if we accepted the digit.
                index++;

                overflowed |= value > 0xFFFFFFFF_FFFFFFFF / @base;
                value *= @base;
                overflowed |= 0xFFFFFFFF_FFFFFFFF - value < digit;
                value += digit;
            }
            while (index < input.Length);

            return (index, value, overflowed);
        }

        /// <summary>
        ///     Creates an integer node with the given YAML 1.1 presentation.
        /// </summary>
        /// <param name="presentation">The YAML 1.1 presentation of the integer node.</param>
        /// <returns>
        ///     An integer node with the given YAML 1.1 presentation, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static YamlInteger? FromPresentation1_1(string presentation)
        {
            //  [-+]?0b[0-1_]+ # (base 2)
            // |[-+]?0[0-7_]+ # (base 8)
            // |[-+]?(0|[1-9][0-9_]*) # (base 10)
            // |[-+]?0x[0-9a-fA-F_]+ # (base 16)
            // |[-+]?[1-9][0-9_]*(:[0-5]?[0-9])+ # (base 60)

            int index = 0;

            // Parse the sign, /[-+]?/.
            (index, char? sign) = YamlNumberParser.Sign(presentation, index);
            bool negative = sign == '-';

            if (index >= presentation.Length) { return null; }

            ulong value;
            bool overflowed;
            if (presentation[index] == '0')
            {
                // 0
                index++;

                if (index >= presentation.Length)
                {
                    // $
                    return new YamlInteger(0, 0, presentation);
                }
                else if (presentation[index] == 'b')
                {
                    // Base 2
                    // b[0-1_]+
                    index++;

                    if (index >= presentation.Length) { return null; }
                    (index, value, overflowed) = YamlInteger.ParseDigits(presentation, index, 2);
                }
                else if (presentation[index] == 'x')
                {
                    // Base 16
                    // x[0-9a-fA-F_]+
                    index++;

                    if (index >= presentation.Length) { return null; }
                    (index, value, overflowed) = YamlInteger.ParseDigits(presentation, index, 16);
                }
                else
                {
                    // Base 8
                    // [0-7_]+
                    if (index >= presentation.Length) { return null; }
                    (index, value, overflowed) = YamlInteger.ParseDigits(presentation, index, 8);
                }
            }
            else
            {
                // Base 10
                // [1-9][0-9_]*
                // A check for the end of the string is unnecessary here, since it has already been
                // checked above.

                // The leading digit must be [1-9].
                if (presentation[index] < '1' ||
                    presentation[index] > '9')
                {
                    return null;
                }

                (index, value, overflowed) = YamlInteger.ParseDigits(presentation, index, 10);

                // Base 60
                // (:[0-5]?[0-9])+
                (index, ulong? maybeDigit) = YamlNumberParser.Base60Component(presentation, index);
                while (maybeDigit is ulong digit)
                {
                    overflowed |= value > 0xFFFFFFFF_FFFFFFFF / 60;
                    value *= 60;
                    overflowed |= 0xFFFFFFFF_FFFFFFFF - value < digit;
                    value += digit;

                    (index, maybeDigit) = YamlNumberParser.Base60Component(presentation, index);
                }
            }

            // Fail if there is junk after the end.
            // /$/
            if (index < presentation.Length) { return null; }

            ulong? uint64Value;
            long? int64Value;
            if (overflowed)
            {
                uint64Value = null;
                int64Value  = null;
            }
            else if (negative)
            {
                uint64Value = value == 0 ? 0 : null;
                int64Value  = value <= 0x80000000_00000000 ? -(long)value : null;
            }
            else
            {
                uint64Value = value;
                int64Value  = value < 0x80000000_00000000 ? (long)value : null;
            }

            return new YamlInteger(int64Value, uint64Value, presentation);
        }
    }
}
