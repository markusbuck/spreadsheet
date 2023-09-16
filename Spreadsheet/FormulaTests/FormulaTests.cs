using SpreadsheetUtilities;
namespace FormulaTests;
/// <summary>
/// Defines the Test for the Fomula Class some test ideas derived from the ps1 tests.
/// </summary>
[TestClass]
public class FormulaTest
{
    // Verifies a Formula with legals and an illegal token 
    [TestMethod()]
    [ExpectedException(typeof(FormulaFormatException))]
    public void VerifyIllegalTokens()
    {
        Formula form = new Formula("X + y + &");
    }

    // Verifies a Formula with multiple Illegal tokens
    [TestMethod()]
    [ExpectedException(typeof(FormulaFormatException))]
    public void VerifyMultipleIllegalTokens()
    {
        Formula form = new Formula("(% + * + &) * 9");
    }
   
    // Verifies a Formula with no tokens throws
    [TestMethod()]
    [ExpectedException(typeof(FormulaFormatException))]
    public void VerifyNoTokens()
    {
        Formula form = new Formula("");
    }

    // Verifies a Formula with no tokens and a big space
    [TestMethod()]
    [ExpectedException(typeof(FormulaFormatException))]
    public void VerifyNoTokensLotsOfWhtieSpace()
    {
        Formula form = new Formula("        ");
    }

    // Verifies a Formula with multiple closing Parenthesis tokens
    [TestMethod()]
    [ExpectedException(typeof(FormulaFormatException))]
    public void VerifyMultipleRightParenthesiTokens()
    {
        Formula form = new Formula("(9 + 10e11))");
    }

    [TestMethod()]
    public void TestSingleNumber()
    {
        Formula form = new Formula("5");
        Assert.AreEqual(5.0, form.Evaluate(s => 0.0));
    }

    [TestMethod()]
    public void TestSingleVariable()
    {
        Formula form = new Formula("X5");
        Assert.AreEqual(13.0, form.Evaluate(s => 13.0));
    }

    [TestMethod()]
    public void TestAddition()
    {
        Formula form = new Formula("5+3");
        Assert.AreEqual(8.0, form.Evaluate(s => 0.0));
    }

    [TestMethod()]
    public void TestSubtraction()
    {
        Formula form = new Formula("18-10");
        Assert.AreEqual(8.0, form.Evaluate(s => 0.0));
    }

    [TestMethod()]
    public void TestMultiplication()
    {
        Formula form = new Formula("2*4");
        Assert.AreEqual(8.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestDivision()
    {
        Formula form = new Formula("16/2");
        Assert.AreEqual(8.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestArithmeticWithVariable()
    {
        Formula form = new Formula("2+X1");
        Assert.AreEqual(6.0, form.Evaluate(s => 4));
    }



    [TestMethod()]
    public void TestLeftToRight()
    {
        Formula form = new Formula("2*6+3");
        Assert.AreEqual(15.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestOrderOperations()
    {
        Formula form = new Formula("2+6*3");
        Assert.AreEqual(20.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestParenthesesTimes()
    {
        Formula form = new Formula("(2+6)*3");
        Assert.AreEqual(24.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestTimesParentheses()
    {
        Formula form = new Formula("2*(3+5)");
        Assert.AreEqual(16.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestPlusParentheses()
    {
        Formula form = new Formula("2+(3+5)");
        Assert.AreEqual(10.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestPlusComplex()
    {
        Formula form = new Formula("2+(3+5*9)");
        Assert.AreEqual(50.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestOperatorAfterParens()
    {
        Formula form = new Formula("(1*1)-2/2");
        Assert.AreEqual(0.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestComplexTimesParentheses()
    {
        Formula form = new Formula("2 + 3 * (3 + 5)");
        Assert.AreEqual(26.0, form.Evaluate(s => 0));
    }

    [TestMethod()]
    public void TestComplexAndParentheses()
    {
        Formula form = new Formula("2+3*5+(3+4*8)*5+2");
        Assert.AreEqual(194.0, form.Evaluate(s => 0));
    }

    //[TestMethod()]
    //[ExpectedException(typeof(ArgumentException))]
    //public void TestDivideByZero()
    //{
    //    Formula form = new Formula("5/0");
    //    Assert.AreEqual(FormulaError(), form.Evaluate(s => 0));
    //}

    [TestMethod()]
    public void TestComplexMultiVar()
    {
        Formula form = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
        Assert.AreEqual(6.0, form.Evaluate(s => (s == "x7") ? 1 : 4));
    }

    [TestMethod()]
    public void TestComplexNestedParensRight()
    {
        Formula form = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
        Assert.AreEqual(6.0, form.Evaluate(s => 1));
    }

    [TestMethod()]
    public void TestComplexNestedParensLeft()
    {
        Formula form = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
        Assert.AreEqual(12.0, form.Evaluate(s => 2));
    }

    [TestMethod()]
    public void TestRepeatedVar()
    {
        Formula form = new Formula("a4-a4*a4/a4");
        Assert.AreEqual(0.0, form.Evaluate(s => 3));
    }

}
