﻿// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!
// Last updated: August 2023 (small tweak to API)

using System.Text.RegularExpressions;

namespace SpreadsheetUtilities;

/// <summary>
/// Represents formulas written in standard infix notation using standard precedence
/// rules.  The allowed symbols are non-negative numbers written using double-precision
/// floating-point syntax (without unary preceeding '-' or '+');
/// variables that consist of a letter or underscore followed by
/// zero or more letters, underscores, or digits; parentheses; and the four operator
/// symbols +, -, *, and /.
///
/// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
/// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
/// and "x 23" consists of a variable "x" and a number "23".
///
/// Associated with every formula are two delegates: a normalizer and a validator.  The
/// normalizer is used to convert variables into a canonical form. The validator is used to
/// add extra restrictions on the validity of a variable, beyond the base condition that
/// variables must always be legal: they must consist of a letter or underscore followed
/// by zero or more letters, underscores, or digits.
/// Their use is described in detail in the constructor and method comments.
/// </summary>
public class Formula
{
    private List<string> formula;

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.
    /// </summary>
    public Formula(string formula) :
        this(formula, s => s, s => true)
    {
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.
    ///
    /// If the formula contains a variable v such that normalize(v) is not a legal variable,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    ///
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
        List<string> tokens = GetTokens(formula).ToList();
        int numTokens = tokens.Count();
        int lpCount = 0;
        int rpCount = 0;

        // Makes sure there is at least one token
        if (numTokens <= 0)
            throw new FormulaFormatException("Must have at least One token in Formula");

        // Checks the tokens for legality
        if (!hasValidTokens(formula))
        {
            throw new FormulaFormatException("Illegal tokens contained in formula");
        }
        // Checks the first element in the formula
        string token = tokens[0];
        if (!(token != "(" || double.TryParse(token, out double result)
            || Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z0-9_]*$")))
        {
            throw new FormulaFormatException("First element in the Formula must be a valid " +
                "variable, number, or an opening parenthesis");
        }

        // Checks the last element in the formula
        token = tokens[numTokens - 1];
        if (!(token != ")" || double.TryParse(token, out result)
            || Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z0-9_]*$")))
        {
            throw new FormulaFormatException("Last element in the Formula must be a valid " +
                "variable, number, or an opening parenthesis");
        }

        for (int i = 0; i < numTokens; i++)
        {
            token = tokens[i];

            if (token == "(")
            {
                lpCount++;
            }
            else if (token == ")")
            {
                rpCount++;
                if (rpCount > lpCount)
                {
                    throw new FormulaFormatException("Invalid use of parenthesis in formula");
                }
            }

            else if (Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                tokens[i] = normalize(token);
            }
            else if (double.TryParse(token, out result))
            {

            }
            //else
            //    throw new FormulaFormatException("Illegal tokens contained in formula");

        }
        this.formula = tokens;
    }

    /// <summary>
    /// Evaluates this Formula, using the lookup delegate to determine the values of
    /// variables.  When a variable symbol v needs to be determined, it should be looked up
    /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
    /// the constructor.)
    ///
    /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
    /// in a string to upper case:
    ///
    /// new Formula("x+7", N, s => true).Evaluate(L) is 11
    /// new Formula("x+7").Evaluate(L) is 9
    ///
    /// Given a variable symbol as its parameter, lookup returns the variable's value
    /// (if it has one) or throws an ArgumentException (otherwise).
    ///
    /// If no undefined variables or divisions by zero are encountered when evaluating
    /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
    /// The Reason property of the FormulaError should have a meaningful explanation.
    ///
    /// This method should never throw an exception.
    /// </summary>
    public object Evaluate(Func<string, double> lookup)
    {
        return "";
    }

    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in this
    /// formula.  No normalization may appear more than once in the enumeration, even
    /// if it appears more than once in this Formula.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<string> GetVariables()
    {
        return new List<string>();
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
        return "";
    }

    /// <summary>
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    ///
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" by
    /// using C#'s standard conversion from string to double (and optionally back to a string).
    /// Variable tokens are considered equal if their normalized forms are equal, as
    /// defined by the provided normalizer.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
        return false;
    }

    /// <summary>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return false;
    }

    /// <summary>
    /// Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return false;
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
        return 0;
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a legal variable token;
    /// a double literal; and anything that doesn't match one of those patterns.
    /// There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(string formula)
    {
        // Patterns for individual tokens
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                yield return s;
            }
        }

    }

    private static bool hasValidTokens(string formula)
    {
        List<string> tokens = GetTokens(formula).ToList();
        int numTokens = tokens.Count();

        for (int i = 0; i < numTokens; i++)
        {
            string token = tokens[i];
            if (!(token == "(" || token == ")" || token == "-" || token == "+" || token == "*"
                    || token == "/" || Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z0-9_]*$")
                    || double.TryParse(token, out double result)))
            {
                return false;
            }
        }
        return true;
    }

    private static void SyntaxChecker(string formula)
    {


    }
}

/// <summary>
/// Used to report syntactic errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(string message) : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public struct FormulaError
{
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(string reason) : this()
    {
        Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}