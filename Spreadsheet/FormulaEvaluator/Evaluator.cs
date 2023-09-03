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

                            value.Push(MultiplyOrDivide(operand1, operand2, sign));
                        }
                    }
                    else
                    {
                        value.Push(operand1);
                    }
                }


            }
            return value.Pop();
        }

        private static int MultiplyOrDivide(int num1, int num2, string sign)
        {
            int product = 0;
            if (sign == "*")
            {
                product = num2 * num1;
            }
            if (sign == "/")
            {
                if (num1 == 0)
                {
                    throw new ArgumentException("Cannot Divide by Zero");
                }
                else
                {
                    product = num2 / num1;
                }
            }
            return product;
        }

    }

}