using System;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        if (Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        if (cells.ContainsKey(name))
        {
            return cells[name].contents;
        }
        else
            return "";
       
    }

    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        return cells.Keys;
    }

    public override IList<string> SetCellContents(string name, double number)
    {
        if (Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        if (cells.ContainsKey(name))
        {
            cells[name].contents = number;
        }
        else
            cells[name] = new Cell(number);
        return relationships.GetDependents(name).Prepend(name).ToList();
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
    public object contents { get; set; }

    public Cell(object contents)
    {
        this.contents = contents;
    }
}

