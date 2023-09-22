using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests;

[TestClass]
public class SpreadsheetTests
{
    /// <summary>
    ///  Tests the getCellContents method when the cell is empty 
    /// </summary>
    [TestMethod]
    public void testEmptyGetCellContents()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        Assert.AreEqual(spreadsheet.GetCellContents("x1"), "");
    }

    /// <summary>
    /// Tests the setCellContents method when inserting text into the cell
    /// </summary>
    [TestMethod]
    public void testSetCellContentsTextBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", "hello world");

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), "hello world");
    }

    /// <summary>
    /// Tests the setCellContents method when inserting a double into the cell
    /// </summary>
    [TestMethod]
    public void testSetCellContentsDoubleBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", 5.2);

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"),5.200);
    }


    /// <summary>
    /// Tests the setCellContents method when inserting a Formula into the cell 
    /// </summary>
    [TestMethod]
    public void testSetCellContentsFormulaBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", new Formula("5+4"));

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), new Formula("5+4"));
    }


    /// <summary>
    /// Tests that the setCellContents method that takes in a formula will
    /// throw an exception when the name of the cell is invalid
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void testSetCellContentsFormulaInvalidName()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("2x", new Formula("5+4"));
    }

    /// <summary>
    /// Tests that the setCellContents method that takes in text will
    /// throw an exception when the name of the cell is invalid
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void testSetCellContentsTextInvalidName()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("$", "Hello");

    }

    /// <summary>
    /// Tests that the setCellContents method that takes in a double will
    /// throw an exception when the name of the cell is invalid
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void testSetCellContentsDoubleInvalidName()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("25", 5.787);
    }

    /// <summary>
    /// This test method tests the set cell contents method with a cell that
    /// is resetting the contents of an already named and created cell.
    /// </summary>
    [TestMethod]
    public void testSetCellContentsTextAlreadyNamed()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", "hello world");
        spreadsheet.SetCellContents("b1", "New Text");

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "hello world");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), "New Text");
    }

    /// <summary>
    /// This test method tests the set cell contents method with a cell that
    /// is resetting the contents of an already named and created cell.
    /// </summary>
    [TestMethod]
    public void testSetCellContentsDoubleAlreadyNamed()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", 5.2);
        spreadsheet.SetCellContents("b1", 2.4);

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), 5.200);
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), 2.4);
    }

    /// <summary>
    /// This test method tests the set cell contents method with a cell that
    /// is resetting the contents of an already named and created cell.
    /// </summary>
    [TestMethod]
    public void testSetCellContentsFormulaAlreadyNamed()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("b1", new Formula("5+4"));
        spreadsheet.SetCellContents("b1", new Formula("3+2"));

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), new Formula("5+4"));
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), new Formula("3+2"));
    }

    /// <summary>
    /// Tests the list that setCellContents method returns when calling it for a
    /// formula
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsFormulaComplex()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", new Formula("a1 * 2"));
        IEnumerable<string> cells = spreadsheet.SetCellContents("a1", 5.0);
        IEnumerable<string> returnVal = new List<string>
        {
            "a1",
            "b1",
            "c1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i ++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
    }

    /// <summary>
    /// Tests the list that setCellContents method returns when calling it for text
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsTextComplex()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", new Formula("a1 * 2"));
        IEnumerable<string> cells = spreadsheet.SetCellContents("a1", 5.0);
        IEnumerable<string> returnVal = new List<string>
        {
            "a1",
            "b1",
            "c1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", "hello world");
        cells = spreadsheet.SetCellContents("a1", 5.0);
        returnVal = new List<string>
        {
            "a1",
            "c1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
    }

    /// <summary>
    /// Tests the list that setCellContents method returns when calling it for a double
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsDoubleComplex()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", new Formula("a1 * 2"));
        IEnumerable<string> cells = spreadsheet.SetCellContents("a1", 5.0);
        IEnumerable<string> returnVal = new List<string>
        {
            "a1",
            "b1",
            "c1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", 4.875);
        cells = spreadsheet.SetCellContents("a1", 5.0);
        returnVal = new List<string>
        {
            "a1",
            "c1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
    }

    /// <summary>
    /// Tests the GetNamesOfAllNonEmptyCells method 
    /// </summary>
    [TestMethod()]
    public void TestGetNamesOfAllNonEmptyCells()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", new Formula("a1 * 2"));
        spreadsheet.SetCellContents("a1", 5.0);

        IEnumerable<string> cells = spreadsheet.GetNamesOfAllNonemptyCells();

        IEnumerable<string> returnVal = new List<string>
        {
            "c1",
            "b1",
            "a1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
    }

    /// <summary>
    /// Tests that the methods involved make it so a cell whos contents is an empty
    /// string is not in the collection return when GetNamesOfAllNonEmptyCells is called
    /// </summary>
    [TestMethod()]
    public void TestGetNamesOfAllNonEmptyCellsWithEmptyCells()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetCellContents("c1", new Formula("b1 + a1"));
        spreadsheet.SetCellContents("b1", new Formula("a1 * 2"));
        spreadsheet.SetCellContents("a1", 5.0);

        IEnumerable<string> cells = spreadsheet.GetNamesOfAllNonemptyCells();

        IEnumerable<string> returnVal = new List<string>
        {
            "c1",
            "b1",
            "a1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
        spreadsheet.SetCellContents("c1", "");

        cells = spreadsheet.GetNamesOfAllNonemptyCells();

        returnVal = new List<string>
        {
            "b1",
            "a1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }

        spreadsheet.SetCellContents("b1", "");

        cells = spreadsheet.GetNamesOfAllNonemptyCells();

        returnVal = new List<string>
        {
            "a1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }
    }
}
