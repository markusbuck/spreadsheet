using System.Text.RegularExpressions;

namespace FormulaEvaluator
{

    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            Stack<int> value = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            foreach(String t in substrings)
            {
                // if t is an integer
                if (int.TryParse(t, out int operand1)){
                    String topOfStack = operators.Peek();
                    if (operators.Count > 0 && topOfStack == "*" || topOfStack == "/")
                    {
                        if(value.Count > 0)
                        {
                            int operand2 = value.Pop();
                            string sign = operators.Pop();
                            int product = 0;
                            if(sign == "*")
                            {
                                product = operand2 * operand1;
                            }
                            if(sign == "/")
                            {
                                product = operand2 / operand1;
                            }
                            value.Push(product);
                            
                        }
                    }
                    else
                    {
                        value.Push(operand1);
                    }
                }
            }
            return 1;
        }

    }

}