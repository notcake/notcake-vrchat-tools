using System.Globalization;

namespace notcake.Unity.Yaml.Nodes
{
    /// <summary>
    ///     Represents a float node in a YAML document.
    /// </summary>
    /// <remarks>
    ///     See http://yaml.org/type/float.html for a list of valid float presentations.
    /// </remarks>
    public class YamlFloat : YamlSingleLineScalar<YamlFloat>
    {
        /// <summary>
        ///     Gets the value of the float node as a double.
        /// </summary>
        public double Value { get; }

        /// <inheritdoc cref="YamlFloat(double, string)"/>
        public YamlFloat(double value) :
            this(value, value.ToString(CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlFloat"/> class.
        /// </summary>
        /// <param name="value">The value of the float node, as a double.</param>
        /// <param name="presentation">The YAML presentation of the float node.</param>
        public YamlFloat(double value, string presentation) :
            base(presentation)
        {
            this.Value = value;
        }

        #region Object
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        #endregion

        #region YamlScalar<YamlFloat>
        public override bool Equals(YamlFloat other)
        {
            return this.Value.Equals(other.Value);
        }
        #endregion

        /// <summary>
        ///     Discards 0 or more digits, <c>/[0-9]*/</c>, from the given string.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="startIndex">
        ///     The index within the string from which to start parsing.
        /// </param>
        /// <param name="allowUnderscore">
        ///     A boolean indicating whether underscores are allowed in the run of digits.
        /// </param>
        /// <returns>The index of the first unparsed character.</returns>
        public static int ParseDigits(string input, int startIndex, bool allowUnderscore)
        {
            int index = startIndex;

            // /[0-9]*/
            while (index < input.Length)
            {
                if (allowUnderscore &&
                    input[index] == '_')
                {
                    index++;
                }
                else if ('0' <= input[index] &&
                         input[index] <= '9')
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        /// <summary>
        ///     Creates a float node with the given YAML 1.1 presentation.
        /// </summary>
        /// <param name="presentation">The YAML 1.1 presentation of the float node.</param>
        /// <returns>
        ///     A float node with the given YAML 1.1 presentation, if valid;<br/>
        ///     <c>null</c> otherwise.
        /// </returns>
        public static YamlFloat? FromPresentation1_1(string presentation)
        {
            //  [-+]?([0-9][0-9_]*)?\.[0-9_]*([eE][-+][0-9]+)? (base 10)
            // |[-+]?[0-9][0-9_]*(:[0-5]?[0-9])+\.[0-9_]* (base 60)
            // |[-+]?\.(inf|Inf|INF) # (infinity)
            // |\.(nan|NaN|NAN) # (not a number)

            int index = 0;

            // Handle NaN, /\.(nan|NaN|NAN)$/
            if (presentation == ".nan" ||
                presentation == ".NaN" ||
                presentation == ".NAN")
            {
                return new YamlFloat(double.NaN, presentation);
            }

            // Now all that's left is:
            //      [-+]?([0-9][0-9_]*)?\.[0-9_]*([eE][-+][0-9]+)? (base 10)
            //     |[-+]?[0-9][0-9_]*(:[0-5]?[0-9])+\.[0-9_]* (base 60)
            //     |[-+]?\.(inf|Inf|INF) # (infinity)

            // Parse the sign, /[-+]?/.
            (index, char? sign) = YamlNumberParser.Sign(presentation, index);
            bool negative = sign == '-';

            if (index >= presentation.Length) { return null; }

            // Handle infinity, /\.(inf|Inf|INF)$/.
            string maybeInfinity = presentation[index..];
            if (maybeInfinity == ".inf" ||
                maybeInfinity == ".Inf" ||
                maybeInfinity == ".INF")
            {
                return new YamlFloat(
                    !negative ? double.PositiveInfinity : double.NegativeInfinity,
                    presentation
                );
            }

            // Now all that's left is:
            //      ([0-9][0-9_]*)?\.[0-9_]*([eE][-+][0-9]+)? (base 10)
            //     |[0-9][0-9_]*(:[0-5]?[0-9])+\.[0-9_]* (base 60)

            // /([0-9][0-9_]*)?/
            bool hasIntegerPart = false;
            double value = 0;
            if ('0' <= presentation[index] &&
                presentation[index] <= '9')
            {
                hasIntegerPart = true;

                do
                {
                    if (presentation[index] == '_')
                    {
                        index++;
                        continue;
                    }

                    ulong digit;
                    if ('0' <= presentation[index] &&
                        presentation[index] <= '9')
                    {
                        digit = (ulong)(presentation[index] - '0');
                        index++;
                    }
                    else
                    {
                        break;
                    }

                    value *= 10;
                    value += digit;
                }
                while (index < presentation.Length);
            }

            if (hasIntegerPart &&
                index < presentation.Length &&
                presentation[index] == ':')
            {
                // Base 60
                while (true)
                {
                    // There is definitely a ':' available here.
                    // /:[0-5]?[0-9]/
                    (int componentEndIndex, ulong? maybeComponent) = YamlNumberParser.Base60Component(
                        presentation,
                        index
                    );

                    if (maybeComponent is not ulong component)
                    {
                        // There was a ':', but the rest was malformed.
                        break;
                    }

                    // The start of the last base 60 component, including the ':'.
                    int componentStartIndex = index;
                    index = componentEndIndex;

                    // Peek for a ':'.
                    if (index < presentation.Length &&
                        presentation[index] == ':')
                    {
                        // There's another base 60 component after this one.
                        value *= 60;
                        value += component;
                    }
                    else
                    {
                        // That was the last base 60 component.
                        // /\./
                        if (index >= presentation.Length) { return null; }
                        if (presentation[index] != '.') { return null; }
                        index++;

                        // /[0-9_]*/
                        index = YamlFloat.ParseDigits(presentation, index, allowUnderscore: true);

                        string lastComponent = presentation[(componentStartIndex + 1)..index];
                        lastComponent = lastComponent.Replace("_", "");
                        value *= 60;
                        value += double.Parse(lastComponent, CultureInfo.InvariantCulture);

                        if (negative)
                        {
                            value = -value;
                        }

                        break;
                    }
                }

                // `value` has now been populated.

                // Fail if there is junk after the end.
                // /$/
                if (index < presentation.Length) { return null; }
            }
            else
            {
                // /\./
                if (index >= presentation.Length) { return null; }
                if (presentation[index] != '.') { return null; }
                index++;

                // /[0-9_]*/
                index = YamlFloat.ParseDigits(presentation, index, allowUnderscore: true);

                // /([eE][-+][0-9]+)?/
                if (index < presentation.Length &&
                    (presentation[index] == 'e' ||
                     presentation[index] == 'E'))
                {
                    index++;

                    // /[-+]/
                    (index, char? exponentSign) = YamlNumberParser.Sign(presentation, index);
                    if (exponentSign == null) { return null; }

                    // /[0-9]+/
                    // There must be characters remaining.
                    if (index >= presentation.Length) { return null; }
                    index = YamlFloat.ParseDigits(presentation, index, allowUnderscore: false);
                    // If no digits were parsed, the remaining characters are junk and the
                    // presentation is invalid.
                }

                // Fail if there is junk after the end.
                // /$/
                if (index < presentation.Length) { return null; }

                // Convert the entire presentation, including the sign.
                string number = presentation;
                number = number.Replace("_", "");
                if (!hasIntegerPart)
                {
                    number = number.Replace(".", "0.");
                }
                value = double.Parse(number, CultureInfo.InvariantCulture);
            }

            return new YamlFloat(value, presentation);
        }
    }
}
