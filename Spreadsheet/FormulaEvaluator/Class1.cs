using System.Text.RegularExpressions;

namespace FormulaEvaluator
{

    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return 1;
        }

    }

}