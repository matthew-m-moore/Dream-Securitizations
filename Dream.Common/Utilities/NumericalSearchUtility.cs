using System;

namespace Dream.Common.Utilities
{
    public static class NumericalSearchUtility
    {
        private const int _numberOfTriesToFindBracket = 75;
        private const double _bracketFindingFactor = 1.13;
        private const double _maximumPrecision = 1e-14;

        /// <summary>
        /// Conducts a Newton-Raphson numerical search with bisection. Returns Double.NaN if no solution is found
        /// within the maximum number of allowable iterations.
        /// </summary>
        public static double NewtonRaphsonWithBisection(
            Func<double, double> functionToSearch,
            double searchTargetValue,
            double targetPrecision,
            double? floorValue = null,
            double? ceilingValue = null,
            int maximumIterations = 100,
            bool acceptBestGuess = true)
        {
            if (targetPrecision < _maximumPrecision)
            {
                throw new Exception("ERROR: Cannot perform a numercal search to a precision less 14 digits.");
            }

            // Adjust the function by the target value, so that we can search for a value equal to zero
            Func<double, double> numericalSearchFunction = (x => functionToSearch(x) - searchTargetValue);

            double highEndOfSearchBracket;
            double lowEndOfSearchBracket;

            var canFindSearchBracket = FindSearchBracket(
                numericalSearchFunction, 
                DoesFunctionCrossZero,
                targetPrecision, 
                floorValue,
                ceilingValue,
                out lowEndOfSearchBracket, 
                out highEndOfSearchBracket);

            // Return Not-a-Number and move on if no bracket can be found
            if (!canFindSearchBracket) return double.NaN;

            var functionEvaluationLow = numericalSearchFunction(lowEndOfSearchBracket);
            var functionEvaluationHigh = numericalSearchFunction(highEndOfSearchBracket);

            if (!DoesFunctionCrossZero(functionEvaluationLow, functionEvaluationHigh)) return double.NaN;

            // It may so happen that the new ends of the bracket satisfy the numerical search already
            if (Math.Abs(functionEvaluationLow) < targetPrecision) return lowEndOfSearchBracket;
            if (Math.Abs(functionEvaluationHigh) < targetPrecision) return highEndOfSearchBracket;

            // The search below depends on the low end of the bracket evaluating to less than zero
            if (functionEvaluationLow > 0)
            {
                var valueToSwap = lowEndOfSearchBracket;
                lowEndOfSearchBracket = highEndOfSearchBracket;
                highEndOfSearchBracket = valueToSwap;
            }

            var result = ConductNewtonRaphsonNumericalSearch(
                numericalSearchFunction,
                lowEndOfSearchBracket,
                highEndOfSearchBracket,
                targetPrecision,
                maximumIterations,
                acceptBestGuess);

            return result;
        }

        public static double BisectionWithNotANumber(
            Func<double, double> functionToSearch,
            double targetPrecision,
            out double inputValueAtSuccess,
            double? floorValue = null,
            double? ceilingValue = null,
            int maximumIterations = 100)
        {
            double highEndOfSearchBracket;
            double lowEndOfSearchBracket;

            var canFindSearchBracket = FindSearchBracket(
                functionToSearch,
                DoesFunctionSpanNotANumber,
                targetPrecision,
                floorValue,
                ceilingValue,
                out lowEndOfSearchBracket,
                out highEndOfSearchBracket);

            // Return Not-a-Number and move on if no bracket can be found
            if (!canFindSearchBracket)
            {
                inputValueAtSuccess = double.NaN;
                return double.NaN;
            }

            var functionEvaluationLow = functionToSearch(lowEndOfSearchBracket);
            var functionEvaluationHigh = functionToSearch(highEndOfSearchBracket);

            // This is the criterion sufficient to end the search
            if (Math.Abs(lowEndOfSearchBracket - highEndOfSearchBracket) < targetPrecision)
            {
                if (!double.IsNaN(functionEvaluationLow))
                {
                    inputValueAtSuccess = lowEndOfSearchBracket;
                    return functionEvaluationLow;
                };
                if (!double.IsNaN(functionEvaluationHigh))
                {
                    inputValueAtSuccess = highEndOfSearchBracket;
                    return functionEvaluationHigh;
                };
            }

            var guess = 0.0;
            for (var iterationCounter = 0; iterationCounter < maximumIterations; iterationCounter++)
            {
                var stepSize = Math.Abs(lowEndOfSearchBracket - highEndOfSearchBracket) / 2.0;

                // Be careful to step in the right direction, toward the double.NaN
                if (!double.IsNaN(functionEvaluationLow))
                {
                    guess = lowEndOfSearchBracket + stepSize;
                    var functionEvaluation = functionToSearch(guess);

                    if (double.IsNaN(functionEvaluation)) highEndOfSearchBracket = guess;
                    else lowEndOfSearchBracket = guess;
                }
                else
                {
                    guess = highEndOfSearchBracket - stepSize;
                    var functionEvaluation = functionToSearch(guess);

                    if (double.IsNaN(functionEvaluation)) lowEndOfSearchBracket = guess;
                    else highEndOfSearchBracket = guess;
                }

                functionEvaluationLow = functionToSearch(lowEndOfSearchBracket);
                functionEvaluationHigh = functionToSearch(highEndOfSearchBracket);

                if (Math.Abs(lowEndOfSearchBracket - highEndOfSearchBracket) < targetPrecision)
                {
                    if (!double.IsNaN(functionEvaluationLow))
                    {
                        inputValueAtSuccess = lowEndOfSearchBracket;
                        return functionEvaluationLow;
                    }
                    if (!double.IsNaN(functionEvaluationHigh))
                    {
                        inputValueAtSuccess = highEndOfSearchBracket;
                        return functionEvaluationHigh;
                    }
                }
            }

            inputValueAtSuccess = double.NaN;
            return double.NaN;
        }

        private static bool FindSearchBracket(
            Func<double, double> numericalSearchFunction,
            Func<double, double, bool> bracketIdentifyingFunction, 
            double targetPrecision,
            double? floorValue,
            double? ceilingValue,
            out double lowEndOfSearchBracket, 
            out double highEndOfSearchBracket)
        {
            highEndOfSearchBracket = ceilingValue ?? 1.5;
            lowEndOfSearchBracket = floorValue ?? -0.5;

            if (ceilingValue.HasValue && floorValue.HasValue)
            {
                if (ceilingValue.Value < floorValue.Value)
                {
                    throw new Exception("ERROR: For a numerical search, the floor value must be below the celing value.");
                }

                return true;
            }

            var functionEvaluationLow = numericalSearchFunction(lowEndOfSearchBracket);
            var functionEvaluationHigh = numericalSearchFunction(highEndOfSearchBracket);            

            // Case 1: One of the intial values supplied for the bracket happens to satisify the search
            if (Math.Abs(functionEvaluationLow) < targetPrecision || 
                Math.Abs(functionEvaluationHigh) < targetPrecision)
            {
                return true;
            }

            // Case 2: The intial values supplied for the bracket already encapsulates a range where the function equals zero
            if (bracketIdentifyingFunction(functionEvaluationLow, functionEvaluationHigh)) return true;

            // Case 3: Search for a bracket that works
            for (var tryCounter = 0; tryCounter < _numberOfTriesToFindBracket; tryCounter++)
            {
                if (Math.Abs(functionEvaluationLow) < Math.Abs(functionEvaluationHigh))
                {
                    lowEndOfSearchBracket += _bracketFindingFactor * (lowEndOfSearchBracket - highEndOfSearchBracket);
                    if (floorValue.HasValue) lowEndOfSearchBracket = Math.Max(lowEndOfSearchBracket, floorValue.Value);
                    functionEvaluationLow = numericalSearchFunction(lowEndOfSearchBracket);
                }
                else
                {
                    highEndOfSearchBracket += _bracketFindingFactor * (highEndOfSearchBracket - lowEndOfSearchBracket);
                    functionEvaluationHigh = numericalSearchFunction(highEndOfSearchBracket);
                }

                if (bracketIdentifyingFunction(functionEvaluationLow, functionEvaluationHigh)) return true;
            }

            return false;
        }

        private static bool DoesFunctionCrossZero(double functionEvaluationLow, double functionEvaluationHigh)
        {
            var isThereSignChange = (functionEvaluationLow * functionEvaluationHigh) <= 0;
            return isThereSignChange;
        }

        private static bool DoesFunctionSpanNotANumber(double functionEvaluationLow, double functionEvaluationHigh)
        {
            var doesSpanNotANumber = (double.IsNaN(functionEvaluationLow) && !double.IsNaN(functionEvaluationHigh)) ||
                                     (!double.IsNaN(functionEvaluationLow) && double.IsNaN(functionEvaluationHigh));
            return doesSpanNotANumber;
        }

        // The following is from a "Numerical Recipes in C" book, a section on Newton-Raphson
        private static double ConductNewtonRaphsonNumericalSearch(
            Func<double, double> numericalSearchFunction,
            double lowEndOfSearchBracket,
            double highEndOfSearchBracket,
            double targetPrecision,
            int maximumIterations,
            bool acceptBestGuess)
        {
            // Start with an initial guess
            var guess = (lowEndOfSearchBracket + highEndOfSearchBracket) / 2.0;
            var previousGuess = guess;

            var stepSize = Math.Abs(highEndOfSearchBracket - lowEndOfSearchBracket);
            var previousStepSize = stepSize;

            var functionEvaluation = numericalSearchFunction(guess);
            var previousFunctionEvaluation = functionEvaluation;

            var evaluationQuotient = functionEvaluation / guess;
            var initiated = false;

            for (var iterationCounter = 0; iterationCounter < maximumIterations; iterationCounter++)
            {
                previousFunctionEvaluation = functionEvaluation;
                previousStepSize = stepSize;
                previousGuess = guess;

                var highEndApproximation = ((guess - highEndOfSearchBracket) * evaluationQuotient) - functionEvaluation;
                var lowEndApproximation = ((guess - lowEndOfSearchBracket) * evaluationQuotient) - functionEvaluation;

                var isOutOfRange = (highEndApproximation * lowEndApproximation) > 0;
                var isNotGettingCloser = Math.Abs(2.0 * functionEvaluation) > Math.Abs(previousStepSize * evaluationQuotient);

                if (isOutOfRange || isNotGettingCloser)
                {
                    stepSize = Math.Abs(lowEndOfSearchBracket - highEndOfSearchBracket) / 2.0;

                    // Be careful to step in the right direction, depending on the sign of low versus high
                    if (highEndOfSearchBracket > lowEndOfSearchBracket)
                    {
                        guess = lowEndOfSearchBracket + stepSize;

                        if (initiated &&
                            Math.Abs(lowEndOfSearchBracket - guess) < _maximumPrecision) return double.NaN;
                    }
                    else
                    {
                        guess = highEndOfSearchBracket + stepSize;

                        if (initiated &&
                            Math.Abs(highEndOfSearchBracket - guess) < _maximumPrecision) return double.NaN;
                    }                
                }
                else
                {
                    stepSize = functionEvaluation / evaluationQuotient;
                    guess -= stepSize;

                    // If the guesses are no longer within maximum precision, we can terminate with the best possible guess
                    // Of course, choice of this behavior is optional
                    if (initiated &&
                        Math.Abs(previousGuess - guess) < _maximumPrecision) return acceptBestGuess ? guess : double.NaN;
                }

                functionEvaluation = numericalSearchFunction(guess);
                if (Math.Abs(functionEvaluation) < targetPrecision) return guess;

                if (guess - previousGuess > 0)
                {
                    evaluationQuotient = (functionEvaluation - previousFunctionEvaluation) / (guess - previousGuess);
                }
                else
                {
                    evaluationQuotient = functionEvaluation - previousFunctionEvaluation;
                }

                // This logic will keep the bracket around the solution
                if (functionEvaluation < 0)
                {
                    lowEndOfSearchBracket = guess;
                }
                else
                {
                    highEndOfSearchBracket = guess;
                }

                // This will initiate the loop after the first iteration
                if (!initiated) initiated = true;
            }

            return double.NaN;
        }
    }
}
