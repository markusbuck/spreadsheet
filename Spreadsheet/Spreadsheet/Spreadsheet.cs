// This file contains the code for the Spreadsheet API
// Author: Markus Buckwalter
// Date: September 22, 2023

using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS;

/// <summary>
/// This class contains the contents of the functionality of the spreadsheet
/// MVC. This class implements AbstractSpreadsheet. 
/// </summary>
public class Spreadsheet : AbstractSpreadsheet
{

    private DependencyGraph relationships;
    private Dictionary<string, Cell> cells;

    /// <summary>
    /// Default Constructor for the Spreadsheet
    /// </summary>
    public Spreadsheet() : base("default")
    {
        cells = new Dictionary<string, Cell>();
        relationships = new DependencyGraph();
    }

    public override object GetCellContents(string name)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
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

    public override object GetCellValue(string name)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {

        return cells.Keys;
    }

    public override void Save(string filename)
    {
        throw new NotImplementedException();
    }

    protected override IList<string> SetCellContents(string name, double number)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }

        relationships.ReplaceDependees(name, new HashSet<string>());
        if (cells.ContainsKey(name))
        {
            cells[name].contents = number;
        }
        else
            cells[name] = new Cell(number);
        return GetCellsToRecalculate(name).ToList();
    }

    protected override IList<string> SetCellContents(string name, string text)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }

        relationships.ReplaceDependees(name, new HashSet<string>());
        if (string.IsNullOrWhiteSpace(text))
        {
            cells.Remove(name);
        }
        else if (cells.ContainsKey(name))
        {
            cells[name].contents = text;
        }
        else
            cells[name] = new Cell(text);
        return GetCellsToRecalculate(name).ToList();
    }

    protected override IList<string> SetCellContents(string name, Formula formula)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        relationships.ReplaceDependees(name, formula.GetVariables());
        IEnumerable<string> cellCollection = GetCellsToRecalculate(name);
        if (cells.ContainsKey(name))
        {
            cells[name].contents = formula;
        }
        else
            cells[name] = new Cell(formula);
        return cellCollection.ToList();
    }

    public override IList<string> SetContentsOfCell(string name, string content)
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        return relationships.GetDependents(name);
    }

}

/// <summary>
/// Cell Class that is to be used in the spreedsheet. Has property to get and set
/// the contents of the cell. 
/// </summary>
public class Cell
{
    public object contents { get; set; }

    public Cell(object contents)
    {
        this.contents = contents;
    }
}