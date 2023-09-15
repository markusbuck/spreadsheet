using SpreadsheetUtilities;
namespace FormulaTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Formula form = new Formula("A235", s => "penis", s => true);
        Console.WriteLine("Test complete");
    }
}
