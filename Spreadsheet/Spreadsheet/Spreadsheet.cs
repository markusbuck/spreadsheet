// This file contains the code for the Spreadsheet API
// Author: Markus Buckwalter
// Date: September 22, 2023

using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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
    private Func<string, bool> validate;
    private Func<string, string> normalizer;

    private DependencyGraph relationships;
    private Dictionary<string, Cell> cells;
    

    /// <summary>
    /// Default Constructor for the Spreadsheet
    /// </summary>
    public Spreadsheet() : this("default", s => s, s => true)
    {
        cells = new Dictionary<string, Cell>();
        relationships = new DependencyGraph();
        Changed = false;
    }

    public Spreadsheet(string version,
        Func<string, string> normalize, Func<string, bool> isValid) : base(version)
    {
        cells = new Dictionary<string, Cell>();
        relationships = new DependencyGraph();

        validate = isValid;
        normalizer = normalize;

        Changed = false;
    }

    //FINISH IMPLEMENTATION
    public Spreadsheet(string filename, string version,
        Func<string, string> normalize, Func<string, bool> isValid) : base(version)
    {
        cells = new Dictionary<string, Cell>();
        relationships = new DependencyGraph();

        validate = isValid;
        normalizer = normalize;

        Changed = false;
    }


    public override object GetCellContents(string name)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        if (!validate(normalizer(name)))
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

    //TODO:Implement
    public override object GetCellValue(string name)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        if (!validate(normalizer(name)))
        {
            throw new InvalidNameException();
        }
        return cells[name].value;
    }

    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        return cells.Keys;
    }

    //TODO:Implement
    public override void Save(string filename)
    {
        throw new NotImplementedException();
    }

    protected override IList<string> SetCellContents(string name, double number)
    {
        relationships.ReplaceDependees(name, new HashSet<string>());
        if (cells.ContainsKey(name))
        {
            cells[name].contents = number;
            cells[name].value = number;
        }
        else
            cells[name] = new Cell(number, number);
        return GetCellsToRecalculate(name).ToList();
    }

    protected override IList<string> SetCellContents(string name, string text)
    {
        relationships.ReplaceDependees(name, new HashSet<string>());
        if (string.IsNullOrWhiteSpace(text))
        {
            cells.Remove(name);
        }
        else if (cells.ContainsKey(name))
        {
            cells[name].contents = text;
            cells[name].contents = text;
        }
        else
            cells[name] = new Cell(text, text);
        return GetCellsToRecalculate(name).ToList();
    }

    protected override IList<string> SetCellContents(string name, Formula formula)
    {
        relationships.ReplaceDependees(name, formula.GetVariables());
        IEnumerable<string> cellCollection = GetCellsToRecalculate(name);
        if (cells.ContainsKey(name))
        {
            cells[name].contents = formula;
            cells[name].value = formula.Evaluate(Lookup);
        }
        else
        {
            cells[name] = new Cell(formula, formula.Evaluate(Lookup));
        }
        return cellCollection.ToList();
    }

    public override IList<string> SetContentsOfCell(string name, string content)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }

        if (!validate(normalizer(name)))
        {
            throw new InvalidNameException();
        }

        IList<string> cellCollection;
        Changed = true;
        if (Double.TryParse(content, out double result))
        {
            cellCollection = SetCellContents(name, result);
        }

        else if (content.TrimStart().StartsWith("="))
        {
            string formulaString = content.TrimStart().Remove(0, 1);
            Formula formula = new Formula(formulaString, normalizer, validate);
            cellCollection = SetCellContents(name, formula);
        }
        else
        {
            cellCollection = SetCellContents(name, content);
        }
        for(int i = 1; i < cellCollection.Count(); i++)
        {
            string cell = cellCollection.ElementAt(i);
            Formula form = (Formula)cells[cell].contents;
            cells[cell].value = form.Evaluate(Lookup);
        }
        return cellCollection;
    }

    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        return relationships.GetDependents(name);
    }

    private double Lookup(string cell)
    {
        if (cells.ContainsKey(cell))
        {
            if (cells[cell].value is double)
            {
                return (double)cells[cell].value;
            }
            else
                throw new ArgumentException("The cells value is not a number");
        }
        else
            throw new ArgumentException("The cells value is empty");
    }
}

/// <summary>
/// Cell Class that is to be used in the spreedsheet. Has property to get and set
/// the contents of the cell. 
/// </summary>
public class Cell
{
    public object contents { get; set; }

    public object value { get; set; }

    public Cell(object contents, object value)
    {
        this.contents = contents;
        this.value = value;
    }

}