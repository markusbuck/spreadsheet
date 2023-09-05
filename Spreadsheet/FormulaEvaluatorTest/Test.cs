using FormulaEvaluator;
namespace FormulaEvaluatorTest;

class Test
{
    static void Main(string[] args)
    {
        Console.WriteLine("Test for Eval");
        Console.WriteLine(Evaluator.Evaluate("4 * 3", LookUpNoVar));
        
    }

    public static int LookUpNoVar(String s)
    {
        return 0;
    }
}