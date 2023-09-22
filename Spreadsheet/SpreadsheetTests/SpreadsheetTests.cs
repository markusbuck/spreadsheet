using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests;

[TestClass]
public class SpreadsheetTests
{
    [TestMethod]
    public void TestSetCellContentsFormula()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        IEnumerable<string> cells = spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        foreach(string cell in cells)
        {
            Console.WriteLine(cell);
        }
        cells = spreadsheet.SetCellContents("b1", new Formula("a1 * 2"));
        foreach (string cell in cells)
        {
            Console.WriteLine(cell);
        }
        spreadsheet.SetCellContents("a1", 5.0);
        cells = spreadsheet.SetCellContents("a1", new Formula("7"));
        foreach (string cell in cells)
        {
            Console.WriteLine(cell);
        }

        spreadsheet.SetCellContents("b1", 9);
        cells = spreadsheet.SetCellContents("a1", new Formula("7"));
        foreach (string cell in cells)
        {
            Console.WriteLine(cell);
        }
    }

    [TestMethod]
    public void testEmptyGetCellContents()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        Assert.AreEqual(spreadsheet.GetCellContents("x1"), "");
    }

    [TestMethod]
    public void testSetCellContentsTextBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", "hello world");

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), "hello world");
    }
}
