using System;
using System.Collections.Generic;
using System.Linq;

namespace Dream.ConsoleApp
{
    public static class CommandLineArgumentsProcessor
    {
        private const char _quoteCharacter = '\"';
        private const char _spaceCharacter = ' ';

        /// <summary>
        /// Parses the arguments passed into the console application for a script, while handling matching pairs of quotes.
        /// Also, trims excess spaces from and between arguments.
        /// </summary>
        public static string[] ParseArguments(string arguments)
        {
            if (string.IsNullOrEmpty(arguments)) return null;

            var handleQuotes = false;
            var processedCommandLine = ProcessCommandLine(arguments, c => QuotesHandler(c, ref handleQuotes)).ToList();
            var trimmedArguments = processedCommandLine.Select(argument => TrimPairsOfQuotes(argument)).ToArray();

            return trimmedArguments;
        }

        private static IEnumerable<string> ProcessCommandLine(string arguments, Func<char, bool> quotesHandler)
        {
            var sectionIndex = 0;

            for (var characterIndex = 0; characterIndex < arguments.Length; characterIndex++)
            {
                // If this character is a part of a pair of quotes, skip it
                if (!quotesHandler(arguments[characterIndex])) continue;

                // Pull out each individual argument and return it
                var argument = arguments.Substring(sectionIndex, characterIndex - sectionIndex);
                yield return argument;

                sectionIndex = characterIndex + 1;
            }

            // Pull out any remaining argument
            var remainingArgument = arguments.Substring(sectionIndex).Trim();
            yield return remainingArgument;
        }

        private static bool QuotesHandler(char characterInArgument, ref bool handleQuotes)
        {
            if (characterInArgument == _quoteCharacter)
            {
                handleQuotes = !handleQuotes;
            }

            var invertedHandleQuotes = !handleQuotes;
            var isCharacterInArgumentSpace = (characterInArgument == _spaceCharacter);

            return invertedHandleQuotes && isCharacterInArgumentSpace;
        }

        private static string TrimPairsOfQuotes(string argument)
        {
            var argumentLength = argument.Length;

            // If the argument is more than two characters long and it is surround by quotes,
            // then trim out the surround quotes and return it
            if ((argumentLength >= 2)
                && (argument[0] == _quoteCharacter)
                && (argument[argumentLength - 1] == _quoteCharacter))
            {
                var argumentWithSurroundQuotesTrimmed = argument.Substring(1, argumentLength - 2);
                return argumentWithSurroundQuotesTrimmed;
            }

            // Otherwise, leave the arguement unaltered
            return argument;
        }
    }
}
