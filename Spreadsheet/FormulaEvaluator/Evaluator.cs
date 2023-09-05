using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class provides an evaluator for infix expressions
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// This delegate sets up a method that can be passed into another method parameter in
        /// order to find the value of a variable.
        /// </summary>
        /// <param name="v"></param>
        /// <returns> an int</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// This method does the evaluation of the passed in infix expression which is given as a string
        /// the method passed into this method will be used to lookup the value of a variable in the expression. Will throw
        /// multiple exceptions for a number of different reasons. 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns> an int value of the expression </returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Private helper method used to complete the multiplication or division needed
        /// in the evaluate method. Throws an exception if a user tries to divide by zero. 
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="sign"></param>
        /// <returns> result of the two values being multiplied or divided </returns>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// A private helper method used to complete the addition or subtraction in the evaluate method.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="sign"></param>
        /// <returns> the result of two values being added or subtracted </returns>
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

        /// <summary>
        /// Private helper method used to tell if a given string is a variable with the proper format. 
        /// </summary>
        /// <param name="t"></param>
        /// <returns> a boolean value of whether or not the given string is a variable </returns>
        /// <exception cref="ArgumentException"></exception>
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