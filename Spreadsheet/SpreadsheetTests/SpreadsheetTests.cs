// This file contains the tests for the spreadsheet program
// Author: Markus Buckwalter
// Date: September 22, 2023
using SS;
using SpreadsheetUtilities;
using System.Text.Json;

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

    /// <summary>
    /// Tests the white space doesn't effect the string from being a formula
    /// </summary>
    [TestMethod()]
    public void TestRemovestringWhiteSpace()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "     = 0 + 1");
        Assert.AreEqual(spreadsheet.GetCellContents("a1"), new Formula("0+1"));
    }

    /// <summary>
    /// Tests the same as above and makes sure the value is correct
    /// </summary>
    [TestMethod()]
    public void testSetCellContents()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "     = 0 + 1");
        Assert.AreEqual(spreadsheet.GetCellValue("a1"), 1.0);
    }

    /// <summary>
    /// Tests the getValue method when the value must be recalculated
    /// </summary>
    [TestMethod()]
    public void testGetValue()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "= b1");
        spreadsheet.SetContentsOfCell("b1", "= c1");
        spreadsheet.SetContentsOfCell("c1", "4");

    }

    /// <summary>
    /// Tests the save method
    /// </summary>
    [TestMethod()]
    public void testSave()
    {
        string sheet = "";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("a1", "=b1");
        spreadsheet.SetContentsOfCell("b1", "=c1");
        spreadsheet.SetContentsOfCell("c1", "4");
        spreadsheet.Save("save.txt");
        Console.Write(File.ReadAllText("save.txt"));
        Assert.AreEqual(spreadsheet.GetCellValue("a1"), 4.0);
    }

    /// <summary>
    /// Tests what the save method does when the cells dictionary is empty
    /// </summary>
    [TestMethod()]
    public void testSaveEmpty()
    {
        string sheet = "";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.Save("save.txt");
        Console.Write(File.ReadAllText("save.txt"));
    }

    /// <summary>
    /// Makes sure the method throws when name is not valid
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(InvalidNameException))]
    public void testValueException()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet(s => false, s => s, "default");
        spreadsheet.GetCellValue("a1");
    }


    /// <summary>
    /// Makes sure the method throws when name is not valid
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(InvalidNameException))]
    public void testContentsException()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet(s => false, s => s, "default");
        spreadsheet.GetCellContents("a1");
    }

    /// <summary>
    /// Makes sure the method throws when name is not valid
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(InvalidNameException))]
    public void testSetContentsException()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet(s => false, s => s, "default");
        spreadsheet.SetContentsOfCell("a1", "= 4 / 5");
    }


    /// <summary>
    /// Makes sure the method throws when name is not valid
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(InvalidNameException))]
    public void testInvalidNameException()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.GetCellValue("$");
    }

    /// <summary>
    /// Makes sure the method throws when name is not valid
    /// </summary>
    [TestMethod()]
    public void testValueEmpty()
    {
        AbstractSpreadsheet spreadsheet = new Spreadsheet();
        Assert.AreEqual(spreadsheet.GetCellValue("a1"), "");
    }

    /// <summary>
    /// Tests the constructor that opens a file
    /// </summary>
    [TestMethod()]
    public void testReloadConstruct()
    {
        string sheet = @"
    {
    ""Cells"": {
    ""A1"": {
    ""StringForm"": ""5""
    },
    ""B3"": {
    ""StringForm"": ""=A1+2""
    }
    },
    ""Version"": ""default""
    }";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet("save.txt", s => true,
            s => s, "default");
        spreadsheet.Save("save.txt");
        Assert.AreEqual(spreadsheet.GetCellContents("A1"), 5.0);
    }

    /// <summary>
    /// Tests the reload contructor throws when it can't deserialize
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void testReloadConstructExceptionInvalidSheet()
    {
        string sheet = @"
    {
    ""Cells"": {
    ""A1"": {
    ""StringForm"": 5
    },
    ""B3"": {
    ""StringForm"": ""=A1+2""
    }
    }
    },
    ""Version"""": ""default""
    }";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet("save.txt", s => true,
            s => s, "default");
    }

    /// <summary>
    /// Tests the reload contructor throws when it can't deserialize
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void testReloadConstructExceptionNullSheet()
    {
        string sheet = @"
    {
    ""Cells"": {
    ""A1"": {
    ""StringForm"": ""5""
    },
    ""B3"": {
    ""StringForm"": ""=A1+2""
    }
    }
    },
    ""Relationships"" : nothing
    ""Version"""": ""default""
    }";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet("save.txt", s => true,
            s => s, "default");
    }


    /// <summary>
    /// Tests the constructor that opens a file when the file doesn't exist
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void testReloadConstructFileNotExist()
    {
        string sheet = @"
    {
    ""Cells"": {
    ""A1"": {
    ""StringForm"": ""5""
    },
    ""B3"": {
    ""StringForm"": ""=A1+2""
    }
    },
    ""Version"": ""default""
    }";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet("8768.txt", s => true,
            s => s, "default");
    }


    /// <summary>
    /// Tests the constructor that opens a file with a wrong version 
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void testReloadConstructInvalidVersion()
    {
        string sheet = @"
    {
    ""Cells"": {
    ""A1"": {
    ""StringForm"": ""5""
    },
    ""B3"": {
    ""StringForm"": ""=A1+2""
    }
    },
    ""Version"": ""default""
    }";
        File.WriteAllText("save.txt", sheet);
        AbstractSpreadsheet spreadsheet = new Spreadsheet("save.txt", s => true,
            s => s, "1.1");
        spreadsheet.Save("save.txt");
        Assert.AreEqual(spreadsheet.GetCellContents("A1"), 5.0);
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
            Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "= A" + (i + 1))));
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


    //Tests from autograder

    // Verifies cells and their values, which must alternate.
    public void VV(AbstractSpreadsheet sheet, params object[] constraints)
    {
        for (int i = 0; i < constraints.Length; i += 2)
        {
            if (constraints[i + 1] is double)
            {
                Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
            }
            else
            {
                Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
            }
        }
    }


    // For setting a spreadsheet cell.
    public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
    {
        List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
        return result;
    }

    // Tests IsValid
    [TestMethod, Timeout(2000)]
    [TestCategory("1")]
    public void IsValidTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "x");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("2")]
    [ExpectedException(typeof(InvalidNameException))]
    public void IsValidTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
        ss.SetContentsOfCell("A1", "x");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("3")]
    public void IsValidTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "= A1 + C1");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("4")]
    [ExpectedException(typeof(FormulaFormatException))]
    public void IsValidTest4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
        ss.SetContentsOfCell("B1", "= A1 + C1");
    }

    // Tests Normalize
    [TestMethod, Timeout(2000)]
    [TestCategory("5")]
    public void NormalizeTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("", s.GetCellContents("b1"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("6")]
    public void NormalizeTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("hello", ss.GetCellContents("b1"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("7")]
    public void NormalizeTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "5");
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("8")]
    public void NormalizeTest4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("A1", "6");
        ss.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
    }

    // Simple tests
    [TestMethod, Timeout(2000)]
    [TestCategory("9")]
    public void EmptySheet()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        VV(ss, "A1", "");
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("10")]
    public void OneString()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneString(ss);
    }

    public void OneString(AbstractSpreadsheet ss)
    {
        Set(ss, "B1", "hello");
        VV(ss, "B1", "hello");
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("11")]
    public void OneNumber()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneNumber(ss);
    }

    public void OneNumber(AbstractSpreadsheet ss)
    {
        Set(ss, "C1", "17.5");
        VV(ss, "C1", 17.5);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("12")]
    public void OneFormula()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneFormula(ss);
    }

    public void OneFormula(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "5.2");
        Set(ss, "C1", "= A1+B1");
        VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("13")]
    public void ChangedAfterModify()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Assert.IsFalse(ss.Changed);
        Set(ss, "C1", "17.5");
        Assert.IsTrue(ss.Changed);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("13b")]
    public void UnChangedAfterSave()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "C1", "17.5");
        ss.Save("changed.txt");
        Assert.IsFalse(ss.Changed);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("14")]
    public void DivisionByZero1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        DivisionByZero1(ss);
    }

    public void DivisionByZero1(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "0.0");
        Set(ss, "C1", "= A1 / B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("15")]
    public void DivisionByZero2()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        DivisionByZero2(ss);
    }

    public void DivisionByZero2(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "5.0");
        Set(ss, "A3", "= A1 / 0.0");
        Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
    }



    [TestMethod, Timeout(2000)]
    [TestCategory("16")]
    public void EmptyArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        EmptyArgument(ss);
    }

    public void EmptyArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "C1", "= A1 + B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("17")]
    public void StringArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        StringArgument(ss);
    }

    public void StringArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "hello");
        Set(ss, "C1", "= A1 + B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("18")]
    public void ErrorArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ErrorArgument(ss);
    }

    public void ErrorArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "");
        Set(ss, "C1", "= A1 + B1");
        Set(ss, "D1", "= C1");
        Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("19")]
    public void NumberFormula1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        NumberFormula1(ss);
    }

    public void NumberFormula1(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "C1", "= A1 + 4.2");
        VV(ss, "C1", 8.3);
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("20")]
    public void NumberFormula2()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        NumberFormula2(ss);
    }

    public void NumberFormula2(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "= 4.6");
        VV(ss, "A1", 4.6);
    }


    // Repeats the simple tests all together
    [TestMethod]
    //, Timeout(2000)]
    [TestCategory("21")]
    public void RepeatSimpleTests()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "A1", "17.32");
        Set(ss, "B1", "This is a test");
        Set(ss, "C1", "= A1+B1");
        OneString(ss);
        OneNumber(ss);
        OneFormula(ss);
        DivisionByZero1(ss);
        DivisionByZero2(ss);
        StringArgument(ss);
        ErrorArgument(ss);
        NumberFormula1(ss);
        NumberFormula2(ss);
    }

    // Four kinds of formulas
    [TestMethod, Timeout(2000)]
    [TestCategory("22")]
    public void Formulas()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Formulas(ss);
    }

    public void Formulas(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.4");
        Set(ss, "B1", "2.2");
        Set(ss, "C1", "= A1 + B1");
        Set(ss, "D1", "= A1 - B1");
        Set(ss, "E1", "= A1 * B1");
        Set(ss, "F1", "= A1 / B1");
        VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("23")]
    public void Formulasa()
    {
        Formulas();
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("24")]
    public void Formulasb()
    {
        Formulas();
    }


    // Are multiple spreadsheets supported?
    [TestMethod, Timeout(2000)]
    [TestCategory("25")]
    public void Multiple()
    {
        AbstractSpreadsheet s1 = new Spreadsheet();
        AbstractSpreadsheet s2 = new Spreadsheet();
        Set(s1, "X1", "hello");
        Set(s2, "X1", "goodbye");
        VV(s1, "X1", "hello");
        VV(s2, "X1", "goodbye");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("26")]
    public void Multiplea()
    {
        Multiple();
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("27")]
    public void Multipleb()
    {
        Multiple();
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("28")]
    public void Multiplec()
    {
        Multiple();
    }

    // Reading/writing spreadsheets
    [TestMethod, Timeout(2000)]
    [TestCategory("29")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save(Path.GetFullPath("/missing/save.txt"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("30")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("31")]
    public void SaveTest3()
    {
        AbstractSpreadsheet s1 = new Spreadsheet();
        Set(s1, "A1", "hello");
        s1.Save("save1.txt");
        s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
        Assert.AreEqual("hello", s1.GetCellContents("A1"));
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("32")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest4()
    {
        using (StreamWriter writer = new StreamWriter("save2.txt"))
        {
            writer.WriteLine("This");
            writer.WriteLine("is");
            writer.WriteLine("a");
            writer.WriteLine("test!");
        }
        AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("33")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest5()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save("save3.txt");
        ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("35")]
    public void SaveTest7()
    {
        var sheet = new
        {
            Cells = new
            {
                A1 = new { StringForm = "hello" },
                A2 = new { StringForm = "5.0" },
                A3 = new { StringForm = "4.0" },
                A4 = new { StringForm = "= A2 + A3" }
            },
            Version = ""
        };

        File.WriteAllText("save5.txt", JsonSerializer.Serialize(sheet));


        AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
        VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("36")]
    public void SaveTest8()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "A1", "hello");
        Set(ss, "A2", "5.0");
        Set(ss, "A3", "4.0");
        Set(ss, "A4", "= A2 + A3");
        ss.Save("save6.txt");

        string fileContents = File.ReadAllText("save6.txt");
        JsonDocument o = JsonDocument.Parse(fileContents);
        Assert.AreEqual("default", o.RootElement.GetProperty("Version").ToString());
        Assert.AreEqual("hello", o.RootElement.GetProperty("Cells").GetProperty("A1").GetProperty("StringForm").ToString());
        Assert.AreEqual(5.0, double.Parse(o.RootElement.GetProperty("Cells").GetProperty("A2").GetProperty("StringForm").ToString()), 1e-9);
        Assert.AreEqual(4.0, double.Parse(o.RootElement.GetProperty("Cells").GetProperty("A3").GetProperty("StringForm").ToString()), 1e-9);
        Assert.AreEqual("=A2+A3", o.RootElement.GetProperty("Cells").GetProperty("A4").GetProperty("StringForm").ToString().Replace(" ", ""));
    }


    // Fun with formulas
    [TestMethod, Timeout(2000)]
    [TestCategory("37")]
    public void Formula1()
    {
        Formula1(new Spreadsheet());
    }
    public void Formula1(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a2 + a3");
        Set(ss, "a2", "= b1 + b2");
        Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
        Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
        Set(ss, "a3", "5.0");
        Set(ss, "b1", "2.0");
        Set(ss, "b2", "3.0");
        VV(ss, "a1", 10.0, "a2", 5.0);
        Set(ss, "b2", "4.0");
        VV(ss, "a1", 11.0, "a2", 6.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("38")]
    public void Formula2()
    {
        Formula2(new Spreadsheet());
    }
    public void Formula2(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a2 + a3");
        Set(ss, "a2", "= a3");
        Set(ss, "a3", "6.0");
        VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
        Set(ss, "a3", "5.0");
        VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("39")]
    public void Formula3()
    {
        Formula3(new Spreadsheet());
    }
    public void Formula3(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a3 + a5");
        Set(ss, "a2", "= a5 + a4");
        Set(ss, "a3", "= a5");
        Set(ss, "a4", "= a5");
        Set(ss, "a5", "9.0");
        VV(ss, "a1", 18.0);
        VV(ss, "a2", 18.0);
        Set(ss, "a5", "8.0");
        VV(ss, "a1", 16.0);
        VV(ss, "a2", 16.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("40")]
    public void Formula4()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Formula1(ss);
        Formula2(ss);
        Formula3(ss);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("41")]
    public void Formula4a()
    {
        Formula4();
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("42")]
    public void MediumSheet()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        MediumSheet(ss);
    }

    public void MediumSheet(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "1.0");
        Set(ss, "A2", "2.0");
        Set(ss, "A3", "3.0");
        Set(ss, "A4", "4.0");
        Set(ss, "B1", "= A1 + A2");
        Set(ss, "B2", "= A3 * A4");
        Set(ss, "C1", "= B1 + B2");
        VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
        Set(ss, "A1", "2.0");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
        Set(ss, "B1", "= A1 / A2");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("43")]
    public void MediumSheeta()
    {
        MediumSheet();
    }


    [TestMethod, Timeout(2000)]
    [TestCategory("44")]
    public void MediumSave()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        MediumSheet(ss);
        ss.Save("save7.txt");
        ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
    }

    [TestMethod, Timeout(2000)]
    [TestCategory("45")]
    public void MediumSavea()
    {
        MediumSave();
    }


    // A long chained formula. Solutions that re-evaluate 
    // cells on every request, rather than after a cell changes,
    // will timeout on this test.
    // This test is repeated to increase its scoring weight
    [TestMethod, Timeout(6000)]
    [TestCategory("46")]
    public void LongFormulaTest()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("47")]
    public void LongFormulaTest2()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("48")]
    public void LongFormulaTest3()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("49")]
    public void LongFormulaTest4()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    [TestMethod, Timeout(6000)]
    [TestCategory("50")]
    public void LongFormulaTest5()
    {
        object result = "";
        LongFormulaHelper(out result);
        Assert.AreEqual("ok", result);
    }

    public void LongFormulaHelper(out object result)
    {
        try
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("sum1", "= a1 + a2");
            int i;
            int depth = 100;
            for (i = 1; i <= depth * 2; i += 2)
            {
                s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
            }
            s.SetContentsOfCell("a" + i, "1");
            s.SetContentsOfCell("a" + (i + 1), "1");
            Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
            s.SetContentsOfCell("a" + i, "0");
            Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
            s.SetContentsOfCell("a" + (i + 1), "0");
            Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
            result = "ok";
        }
        catch (Exception e)
        {
            result = e;
        }
    }
}
