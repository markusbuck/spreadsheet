// This file contains the tests for the spreadsheet program
// Author: Markus Buckwalter
// Date: September 22, 2023
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
    ///  Tests the getCellContents method when the cell has an invalid name 
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void testGetCellContents()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        Assert.AreEqual(spreadsheet.GetCellContents("$"), "");
    }


    /// <summary>
    /// Tests the setCellContents method when inserting text into the cell
    /// </summary>
    [TestMethod]
    public void testSetCellContentsTextBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("b1", "hello world");

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
        spreadsheet.SetContentsOfCell("b1", "5.2");

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), 5.200);
    }


    /// <summary>
    /// Tests the setCellContents method when inserting a Formula into the cell 
    /// </summary>
    [TestMethod]
    public void testSetCellContentsFormulaBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("b1", "=5+4");

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), new Formula("5+4"));
    }

    /// <summary>
    /// Tests the setCellContents method throws a circular dependency Exception when it is
    /// dependent on its self.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void testSetCellContentsCirularBasic()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("b1", "=5+4");

        Assert.AreNotEqual(spreadsheet.GetCellContents("b1"), "");
        Assert.AreEqual(spreadsheet.GetCellContents("b1"), new Formula("5+4"));
        spreadsheet.SetContentsOfCell("b1", "= b1");
    }

    /// <summary>
    /// Tests the setCellContents method throws a circular dependency Exception when it is
    /// dependent on its indirect dependents.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void testSetCellContentsCirularComplex()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("b1", "= a1 + 3");
        spreadsheet.SetContentsOfCell("a1", "= c1 * 8");
        IEnumerable<string> cells = spreadsheet.SetContentsOfCell("c1", "bruh");

        IEnumerable<string> returnVal = new List<string>
        {
            "c1",
            "a1",
            "b1"
        };
        Assert.AreEqual(cells.Count(), returnVal.Count());
        for (int i = 0; i < cells.Count(); i++)
        {
            Assert.AreEqual(cells.ElementAt(i), returnVal.ElementAt(i));
        }

        spreadsheet.SetContentsOfCell("c1", "= 7/b1");
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
        spreadsheet.SetContentsOfCell("2x", "= 5+4");
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
        spreadsheet.SetContentsOfCell("$", "Hello");

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
        spreadsheet.SetContentsOfCell("25", "5.787");
    }

    /// <summary>
    /// This test method tests the set cell contents method with a cell that
    /// is resetting the contents of an already named and created cell.
    /// </summary>
    [TestMethod]
    public void testSetCellContentsTextAlreadyNamed()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("b1", "hello world");
        spreadsheet.SetContentsOfCell("b1", "New Text");

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
        spreadsheet.SetContentsOfCell("b1", "5.2");
        spreadsheet.SetContentsOfCell("b1", "2.4");

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
        spreadsheet.SetContentsOfCell("b1", "= 5+4");
        spreadsheet.SetContentsOfCell("b1", "= 3+2");

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
        spreadsheet.SetContentsOfCell("c1", "= b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "= a1 * 2");
        IEnumerable<string> cells = spreadsheet.SetContentsOfCell("a1", "5.0");
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
    }

    /// <summary>
    /// Tests the list that setCellContents method returns when calling it for text
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsTextComplex()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("c1", "= b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "= a1 * 2");
        IEnumerable<string> cells = spreadsheet.SetContentsOfCell("a1", "5.0");
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
        spreadsheet.SetContentsOfCell("c1", "=b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "hello world");
        cells = spreadsheet.SetContentsOfCell("a1", "5.0");
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
        spreadsheet.SetContentsOfCell("c1", "= b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "= a1 * 2");
        IEnumerable<string> cells = spreadsheet.SetContentsOfCell("a1", "5.0");
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
        spreadsheet.SetContentsOfCell("c1", "= b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "4.875");
        cells = spreadsheet.SetContentsOfCell("a1", "5.0");
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
        spreadsheet.SetContentsOfCell("c1", "=b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "=a1 * 2");
        spreadsheet.SetContentsOfCell("a1", "5.0");

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
        spreadsheet.SetContentsOfCell("c1", "=b1 + a1");
        spreadsheet.SetContentsOfCell("b1", "=a1 * 2");
        spreadsheet.SetContentsOfCell("a1", "5.0");

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
        spreadsheet.SetContentsOfCell("c1", "");

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

        spreadsheet.SetContentsOfCell("b1", "");

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


    [TestMethod()]
    public void TestRemovestringWhiteSpace()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "     = 0 + 1");
        Assert.AreEqual(spreadsheet.GetCellContents("a1"), new Formula("0+1"));
    }

    [TestMethod()]
    public void testSetCellContents()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "     = 0 + 1");
        Assert.AreEqual(spreadsheet.GetCellValue("a1"), 1.0);
    }

    [TestMethod()]
    public void testGetValue()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "= b1");
        spreadsheet.SetContentsOfCell("b1", "= c1");
        spreadsheet.SetContentsOfCell("c1", "4");
        Assert.AreEqual(spreadsheet.GetCellValue("a1"), 4.0);
    }



    // STRESS TESTS
    [TestMethod(), Timeout(2000)]
    [TestCategory("31")]
    public void TestStress1()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=B1+B2");
        s.SetContentsOfCell("B1", "=C1-C2");
        s.SetContentsOfCell("B2", "=C3*C4");
        s.SetContentsOfCell("C1", "=D1*D2");
        s.SetContentsOfCell("C2", "=D3*D4");
        s.SetContentsOfCell("C3", "=D5*D6");
        s.SetContentsOfCell("C4", "=D7*D8");
        s.SetContentsOfCell("D1", "=E1");
        s.SetContentsOfCell("D2", "=E1");
        s.SetContentsOfCell("D3", "=E1");
        s.SetContentsOfCell("D4", "=E1");
        s.SetContentsOfCell("D5", "=E1");
        s.SetContentsOfCell("D6", "=E1");
        s.SetContentsOfCell("D7", "=E1");
        s.SetContentsOfCell("D8", "=E1");
        IList<String> cells = s.SetContentsOfCell("E1", "0");
        Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
    }

    // Repeated for extra weight
    [TestMethod(), Timeout(2000)]
    [TestCategory("32")]
    public void TestStress1a()
    {
        TestStress1();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("33")]
    public void TestStress1b()
    {
        TestStress1();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("34")]
    public void TestStress1c()
    {
        TestStress1();
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("35")]
    public void TestStress2()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<String> cells = new HashSet<string>();
        for (int i = 1; i < 200; i++)
        {
            cells.Add("A" + i);
            Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i,"= A" + (i + 1))));
        }
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("36")]
    public void TestStress2a()
    {
        TestStress2();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("37")]
    public void TestStress2b()
    {
        TestStress2();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("38")]
    public void TestStress2c()
    {
        TestStress2();
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("39")]
    public void TestStress3()
    {
        Spreadsheet s = new Spreadsheet();
        for (int i = 1; i < 200; i++)
        {
            s.SetContentsOfCell("A" + i, "=A" + (i + 1));
        }
        try
        {
            s.SetContentsOfCell("A150", "=A50");
            Assert.Fail();
        }
        catch (CircularException)
        {
        }
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("40")]
    public void TestStress3a()
    {
        TestStress3();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("41")]
    public void TestStress3b()
    {
        TestStress3();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("42")]
    public void TestStress3c()
    {
        TestStress3();
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("43")]
    public void TestStress4()
    {
        Spreadsheet s = new Spreadsheet();
        for (int i = 0; i < 500; i++)
        {
            s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
        }
        LinkedList<string> firstCells = new LinkedList<string>();
        LinkedList<string> lastCells = new LinkedList<string>();
        for (int i = 0; i < 250; i++)
        {
            firstCells.AddFirst("A1" + i);
            lastCells.AddFirst("A1" + (i + 250));
        }
        Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
        Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("44")]
    public void TestStress4a()
    {
        TestStress4();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("45")]
    public void TestStress4b()
    {
        TestStress4();
    }
    [TestMethod(), Timeout(2000)]
    [TestCategory("46")]
    public void TestStress4c()
    {
        TestStress4();
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("47")]
    public void TestStress5()
    {
        RunRandomizedTest(47, 2519);
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("48")]
    public void TestStress6()
    {
        RunRandomizedTest(48, 2521);
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("49")]
    public void TestStress7()
    {
        RunRandomizedTest(49, 2526);
    }

    [TestMethod(), Timeout(2000)]
    [TestCategory("50")]
    public void TestStress8()
    {
        RunRandomizedTest(50, 2521);
    }

    /// <summary>
    /// Sets random contents for a random cell 10000 times
    /// </summary>
    /// <param name="seed">Random seed</param>
    /// <param name="size">The known resulting spreadsheet size, given the seed</param>
    public void RunRandomizedTest(int seed, int size)
    {
        Spreadsheet s = new Spreadsheet();
        Random rand = new Random(seed);
        for (int i = 0; i < 10000; i++)
        {
            try
            {
                switch (rand.Next(3))
                {
                    case 0:
                        s.SetContentsOfCell(randomName(rand), "3.14");
                        break;
                    case 1:
                        s.SetContentsOfCell(randomName(rand), "hello");
                        break;
                    case 2:
                        s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                        break;
                }
            }
            catch (CircularException)
            {
            }
        }
        ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
        Assert.AreEqual(size, set.Count);
    }

    /// <summary>
    /// Generates a random cell name with a capital letter and number between 1 - 99
    /// </summary>
    /// <param name="rand"></param>
    /// <returns></returns>
    private String randomName(Random rand)
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
    }

    /// <summary>
    /// Generates a random Formula
    /// </summary>
    /// <param name="rand"></param>
    /// <returns></returns>
    private String randomFormula(Random rand)
    {
        String f = randomName(rand);
        for (int i = 0; i < 10; i++)
        {
            switch (rand.Next(4))
            {
                case 0:
                    f += "+";
                    break;
                case 1:
                    f += "-";
                    break;
                case 2:
                    f += "*";
                    break;
                case 3:
                    f += "/";
                    break;
            }
            switch (rand.Next(2))
            {
                case 0:
                    f += 7.2;
                    break;
                case 1:
                    f += randomName(rand);
                    break;
            }
        }
        return f;
    }

}
