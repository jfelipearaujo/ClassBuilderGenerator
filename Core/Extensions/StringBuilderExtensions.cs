using System.Text;

namespace ClassBuilderGenerator.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// This extension method will add the value to the builder when condition is 'true'
        /// </summary>
        /// <param name="builder">StringBuilder that is been used</param>
        /// <param name="condition">This condition controls if the value will be added to the builder</param>
        /// <param name="value">Value to be added to the builder</param>
        /// <returns></returns>
        public static StringBuilder AppendWhenTrue(this StringBuilder builder, bool condition, string value)
        {
            if (condition)
            {
                builder.Append(value);
            }

            return builder;
        }

        /// <summary>
        /// This extension method will add the value to the builder when condition is 'true' or 'false'
        /// </summary>
        /// <param name="builder">StringBuilder that is been used</param>
        /// <param name="condition">This condition controls if the value will be added to the builder</param>
        /// <param name="whenConditionIsTrue">Value to be added to the builder when the condition is 'true'</param>
        /// <param name="whenConditionIsFalse">Value to be added to the builder when the condition is 'false'</param>
        /// <returns></returns>
        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, string whenConditionIsTrue, string whenConditionIsFalse)
        {
            if (condition)
            {
                builder.Append(whenConditionIsTrue);
            }
            else
            {
                builder.Append(whenConditionIsFalse);
            }

            return builder;
        }
    }
}
