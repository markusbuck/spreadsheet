using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            foreach (string t in substrings)
            {
                int i = variableEvaluator(t);
                string s = t.Trim();
                // if t is an integer
                if (int.TryParse(s, out int operand1)) {
                    if (operators.Count > 0 && operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        if (values.Count > 0)
                        {
                            int operand2 = values.Pop();
                            string sign = operators.Pop();

                            values.Push(MultiplyOrDivide(operand1, operand2, sign));
                        }
                        else
                        {
                            throw new ArgumentException("Invalid number of values");
                        }
                    }
                    else
                    {
                        values.Push(operand1);
                    }
                }
                // add if t is a variable


                // if t is a plus or minus
                else if (s == "+" || s == "-")
                {
                    if (operators.Count > 0 && operators.Peek() == "+" || operators.Peek() == "-")
                    {
                        if (values.Count >= 2)
                        {
                            int addend2 = values.Pop();
                            int addend1 = values.Pop();
                            string sign = operators.Pop();

                            values.Push(AddOrSubtract(addend1, addend2, sign));
                        }
                        else
                        {
                            throw new ArgumentException("Invalid number of values");
                        }
                    }
                }
                else if(s == "*" || s == "/" || s == "(")
                {
                    operators.Push(s);
                }
                else if(s == ")")
                {

                }

            }
            return values.Pop();
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

        private static int AddOrSubtract(int num1, int num2, string sign)
        {
            int product = 0;
            if (sign == "+")
            {
                product = num2 + num1;
            }
            if (sign == "-")
            {
                product = num1 - num2;
            }
            return product;
        }

        private static bool IsVariable(string t)
        {
            char[] charArray = t.ToCharArray();
            foreach(char c in charArray)
            {

            }

        }
    }
}