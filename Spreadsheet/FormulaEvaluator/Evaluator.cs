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
                string s = t.Trim();
                
                if (int.TryParse(s, out int operand1)) {
                    if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                    {
                        if (values.Count > 0)
                        {
                            int operand2 = values.Pop();
                            string sign = operators.Pop();

                            values.Push(MultiplyOrDivide(operand1, operand2, sign));
                        }
                        else
                        
                            throw new ArgumentException("Invalid number of values");
                        
                    }
                    else
                    
                        values.Push(operand1);
                   
                }
                else if (IsVariable(s))
                {
                    operand1 = variableEvaluator(t);

                    if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
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
                        values.Push(operand1);
                }
                else if (s == "+" || s == "-")
                {
                    if (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"))
                    {
                        if (values.Count >= 2)
                        {
                            int addend2 = values.Pop();
                            int addend1 = values.Pop();
                            string sign = operators.Pop();

                            values.Push(AddOrSubtract(addend1, addend2, sign));
                        }
                        else
                            throw new ArgumentException("Invalid number of values");
                    }
                    operators.Push(s);
                }
                else if (s == "*" || s == "/" || s == "(")
                {
                    operators.Push(s);
                }
                else if (s == ")")
                {
                    if(operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"))
                    {
                        if (values.Count >= 2)
                        {
                            int addend2 = values.Pop();
                            int addend1 = values.Pop();
                            string sign = operators.Pop();

                            values.Push(AddOrSubtract(addend1, addend2, sign));
                        }
                        else                   
                            throw new ArgumentException("Invalid number of values");                       
                    }
                    if (operators.Count == 0 || (operators.Pop() != "("))
                    {
                        throw new ArgumentException("Inavlid use of paranthesis");
                    }
                    if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                    {
                        if (values.Count >= 2)
                        {
                            int value = values.Pop();
                            int operand = values.Pop();
                            string sign = operators.Pop();

                            values.Push(MultiplyOrDivide(operand, value, sign));
                        }
                        else
                            throw new ArgumentException("Invalid number of values");
                    }
                }

            }
            if(operators.Count > 0)
            {
                if (operators.Count != 1)
                    throw new ArgumentException("Invalid number of Operators remain");
                if (operators.Peek() == "+" || operators.Peek() == "-")
                {
                    if (values.Count == 2)
                    {
                        int addend2 = values.Pop();
                        int addend1 = values.Pop();
                        string sign = operators.Pop();

                        return AddOrSubtract(addend1, addend2, sign);
                    }
                    else
                        throw new ArgumentException("Invalid number of Values");
                  
                }
                else
                    throw new ArgumentException("Invalid Operator Usage");
            }
            else
            {
                if (values.Count != 1)
                    throw new ArgumentException("Invalid number of values remaining");
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
            if (charArray.Length > 0 && (char.IsLetter(charArray[0])))
            {
                bool LettersRemain = true;
                foreach (char c in charArray)
                {
                    if (char.IsNumber(c))
                    {
                        LettersRemain = false;
                    }
                    if ((!char.IsLetterOrDigit(c)) || (char.IsLetter(c) && LettersRemain == false))
                    {
                        throw new ArgumentException("Incorrect Variable Name");
                    }
                }
                return true;
            }
            return false;
        }
    }
}