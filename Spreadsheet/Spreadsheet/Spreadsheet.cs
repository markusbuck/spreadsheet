using System;
using System.Net.Http;
using SpreadsheetUtilities;

namespace SS;

public class Spreadsheet : AbstractSpreadsheet
{

    private DependencyGraph relationships;
    private Dictionary<string, Cell> cells;

	public Spreadsheet()
	{
        cells = new Dictionary<string, Cell>();
        relationships = new DependencyGraph();
	}

    public override object GetCellContents(string name)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        throw new NotImplementedException();
    }

    public override IList<string> SetCellContents(string name, double number)
    {
        throw new NotImplementedException();
    }

    public override IList<string> SetCellContents(string name, string text)
    {
        throw new NotImplementedException();
    }

    public override IList<string> SetCellContents(string name, Formula formula)
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        throw new NotImplementedException();
    }

}

public class Cell
{
    private object contents { get; set; }

    public Cell(string contents)
    {
        this.contents = contents;
    }
    public Cell(double contents)
    {
        this.contents = contents;
    }
    public Cell(Formula contents)
    {
        this.contents = contents;
    }
}

