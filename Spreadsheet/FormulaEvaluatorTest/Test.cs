﻿using FormulaEvaluator;
namespace FormulaEvaluatorTest;

/// <summary>
/// Class used to test the formula evaluator program
/// </summary>
class Test
{
    /// <summary>
    /// Empty Lookup method used to call the evaluate method.
    /// </summary>
    /// <param name="s"></param>
    /// <returns> int = 0 </returns>
    public static int LookUpNoVar(String s)
    {
        return 0;
    }

    /// <summary>
    /// Lookup method used to evaluate given variables, this method just returns
    /// one number instead of actually looking up values.
    /// </summary>
    /// <param name="s"></param>
    /// <returns> int = 1</returns>
    public static int LookUpWithVar(String s)
    {
        return 1;
    }
    /// <summary>
    /// Main method that contains tests for the evaluate program. 
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Console.WriteLine("Test for Eval");

        Console.WriteLine("Test Multiply, should return 12, actual value: "
            + Evaluator.Evaluate("4 * 3", LookUpNoVar));

        Console.WriteLine("Test divide, should return 2, actual value: "
            + Evaluator.Evaluate("4 / 2", LookUpNoVar));

        Console.WriteLine("Test add, should return 15, actual value: "
            + Evaluator.Evaluate("3 + 12", LookUpNoVar));

        Console.WriteLine("Test subtract, should return 2, actual value: "
            + Evaluator.Evaluate("3 - 1 ", LookUpNoVar));

        Console.WriteLine("Test order of operatations, should return 30, actual: "
            + Evaluator.Evaluate("(3 * 8) + 6", LookUpNoVar));

        Console.WriteLine("Test order of operations, should return 30, actual: "
            + Evaluator.Evaluate("(3 * 8) + 6", LookUpNoVar));

        Console.WriteLine("Test order of  multiple operations, should return 18, actual: "
            + Evaluator.Evaluate("20 + (1 * 3) / 3 - 3", LookUpNoVar));

        Console.WriteLine("Test order of operatations, should return 48, actual: "
            + Evaluator.Evaluate("(3 * 8) + (3 * 8)", LookUpNoVar));

        Console.WriteLine("Test for extra whitespace, should return 18, actual: "
            + Evaluator.Evaluate("20   +   (  1  *  3 )  /  3  -  3", LookUpNoVar));

        Console.WriteLine("Test for no whitespace, should return 18, actual: "
            + Evaluator.Evaluate("20+(1*3)/3-3", LookUpNoVar));

        Console.WriteLine("Test for multiple additions, should return 50, actual: "
            + Evaluator.Evaluate("30 + 12 + 3 + 5 ", LookUpNoVar));

        Console.WriteLine("Test for multiple subtractions, should return 15, actual: "
            + Evaluator.Evaluate("30 - 12 - 3 - 0 ", LookUpNoVar));

        Console.WriteLine("Test for multiple multiplications, should return 24, actual: "
            + Evaluator.Evaluate("1 * 2 * 3 * 4 ", LookUpNoVar));

        Console.WriteLine("Test for multiple divisions, should return 1, actual: "
            + Evaluator.Evaluate("24 / 4 / 3 / 2", LookUpNoVar));

        Console.WriteLine("Test for variables additions, should return 21, actual: "
            + Evaluator.Evaluate("A1 + 12 + 3 + 5 ", LookUpWithVar));

        Console.WriteLine("Test for proper variable names, should return 1, actual: "
            + Evaluator.Evaluate("aaaa1111", LookUpWithVar));

        Console.WriteLine("Test for variable additions, should return 60, actual: "
            + Evaluator.Evaluate("f87 * 60 ", LookUpWithVar));

        Console.WriteLine("Test for no operations, should return 3, actual: "
            + Evaluator.Evaluate("3", LookUpNoVar));

        // The following tests throw exceptions uncomment to test.
        //Console.WriteLine("Test divide by zero, should return exception, actual value: + Evaluator.Evaluate("4 / 0", LookUpNoVar));
        //Console.WriteLine("Test no values, should return exception, actual value: " + Evaluator.Evaluate("()", LookUpNoVar));
        //Console.WriteLine("Test for negative numbers, should return an exception, actual: " + Evaluator.Evaluate("-30 + 12 ", LookUpNoVar));
        //Console.WriteLine("Test for invalid parenthesis, should return Exception, actual: " + Evaluator.Evaluate("30) + 12 + 3 + 5 ", LookUpNoVar));
        //Console.WriteLine("Test for improper additions, should return exception, actual: " + Evaluator.Evaluate("30 + 12 + 3 +  ", LookUpNoVar));
    }
}