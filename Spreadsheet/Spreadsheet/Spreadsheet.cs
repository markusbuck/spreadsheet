// This file contains the code for the Spreadsheet API
// Author: Markus Buckwalter
// Date: September 22, 2023

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Serialization;


namespace SS;

/// <summary>
/// This class contains the contents of the functionality of the spreadsheet
/// MVC. This class implements AbstractSpreadsheet. 
/// </summary>
public class Spreadsheet : AbstractSpreadsheet
{
    private Func<string, bool> Validate;
    private Func<string, string> Normalizer;

    private DependencyGraph Relationships;

    [JsonInclude]
    public Dictionary<string, Cell> Cells;
    

    /// <summary>
    /// Default Constructor for the Spreadsheet
    /// </summary>
    public Spreadsheet() : this(s => true, s => s, "default")
    {
        Cells = new Dictionary<string, Cell>();
        Relationships = new DependencyGraph();
        Changed = false;
    }

    /// <summary>
    /// Constructor that takes in a version, a normalize function, and a validator
    /// function.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="normalize"></param>
    /// <param name="isValid"></param>
    public Spreadsheet(Func<string, bool> isValid,
        Func<string, string> normalize, string version) : base(version)
    {
        Cells = new Dictionary<string, Cell>();
        Relationships = new DependencyGraph();

        Validate = isValid;
        Normalizer = normalize;

        Changed = false;
    }

    /// <summary>
    /// Constructor that builds a spreadsheet when taking in a filename containing Json text.
    /// Also takes in a version, normalizer, and validator 
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="version"></param>
    /// <param name="normalize"></param>
    /// <param name="isValid"></param>
    /// <exception cref="SpreadsheetReadWriteException"></exception>
    public Spreadsheet(string filename, Func<string, bool> isValid,
        Func<string, string> normalize, string version) : base(version)
    {
        Cells = new Dictionary<string, Cell>();
        Relationships = new DependencyGraph();
        Validate = isValid;
        Normalizer = normalize;
        Changed = false;
        if (!File.Exists(filename))
        {
            throw new SpreadsheetReadWriteException("File provided does not exist");
        }
        try
        {
            string jsonString = File.ReadAllText(filename);
            Spreadsheet? sheet = JsonSerializer.Deserialize<Spreadsheet>(jsonString);
            if (sheet != null)
            {
                if (sheet.Version != version)
                {
                    throw new SpreadsheetReadWriteException("File Version does not match" +
                        " the provided Version");
                }
                sheet.Validate = isValid;
                sheet.Normalizer = normalize;
                foreach (string cell in sheet.Cells.Keys)
                {
                    SetContentsOfCell(cell, sheet.Cells[cell].StringForm);
                }
            }
        }
        catch
        {
            throw new SpreadsheetReadWriteException("File was not a valid spreedsheet");
        }

    }

    [JsonConstructor]
    public Spreadsheet(string version) : this(s => true, s => s, version) { }


    public override object GetCellContents(string name)
    {
        name = Normalizer(name);
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        if (!Validate(name))
        {
            throw new InvalidNameException();
        }
        if (Cells.ContainsKey(name))
        {
            return Cells[name].Contents;
        }
        else
            return "";

    }

    
    public override object GetCellValue(string name)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }
        if (!Validate(Normalizer(name)))
        {
            throw new InvalidNameException();
        }
        if (!Cells.ContainsKey(name))
        {
            return "";
        }
        else
            return Cells[name].Value;
    }

    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        return Cells.Keys;
    }

    public override void Save(string filename)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(this, options);
        try
        {


            File.WriteAllText(filename, jsonString);
        }
        catch
        {
            throw new SpreadsheetReadWriteException("Cannot Save to invalid path");
        }
        Changed = false;
    }

    protected override IList<string> SetCellContents(string name, double number)
    {
        Relationships.ReplaceDependees(name, new HashSet<string>());
        if (Cells.ContainsKey(name))
        {
            Cells[name].Contents = number;
            Cells[name].Value = number;
        }
        else
            Cells[name] = new Cell(number, number);
        return GetCellsToRecalculate(name).ToList();
    }

    protected override IList<string> SetCellContents(string name, string text)
    {
        Relationships.ReplaceDependees(name, new HashSet<string>());
        if (string.IsNullOrWhiteSpace(text))
        {
            Cells.Remove(name);
        }
        else if (Cells.ContainsKey(name))
        {
            Cells[name].Contents = text;
            Cells[name].Value = text;
        }
        else
            Cells[name] = new Cell(text, text);
        return GetCellsToRecalculate(name).ToList();
    }

    protected override IList<string> SetCellContents(string name, Formula formula)
    {
        Relationships.ReplaceDependees(name, formula.GetVariables());
        IEnumerable<string> cellCollection = GetCellsToRecalculate(name);
        if (Cells.ContainsKey(name))
        {
            Cells[name].Contents = formula;
            Cells[name].Value = formula.Evaluate(Lookup);
        }
        else
        {
            Cells[name] = new Cell(formula, formula.Evaluate(Lookup));
        }
        return cellCollection.ToList();
    }

    public override IList<string> SetContentsOfCell(string name, string content)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            throw new InvalidNameException();
        }

        if (!Validate(Normalizer(name)))
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
            Formula formula = new Formula(formulaString, Normalizer, Validate);
            cellCollection = SetCellContents(name, formula);
        }
        else
        {
            cellCollection = SetCellContents(name, content);
        }
        for(int i = 1; i < cellCollection.Count(); i++)
        {
            string cell = cellCollection.ElementAt(i);
            Formula form = (Formula)Cells[cell].Contents;
            Cells[cell].Value = form.Evaluate(Lookup);
        }
        if (Cells.ContainsKey(name))
        {
            Cells[name].StringForm = content;
        }
        return cellCollection;
    }

    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        return Relationships.GetDependents(name);
    }

    private double Lookup(string cell)
    {
        if (Cells.ContainsKey(cell))
        {
            if (Cells[cell].Value is double)
            {
                return (double)Cells[cell].Value;
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
    [JsonIgnore]
    public object Contents { get; set; }

    [JsonIgnore]
    public object Value { get; set; }

    [JsonInclude]
    public string StringForm { get; set; }

    public Cell(object contents, object value)
    {
        this.Contents = contents;
        this.Value = value;
        StringForm = "";
    }

    [JsonConstructor]
    public Cell(string stringForm)
    {
        this.StringForm = stringForm;
        this.Contents = stringForm;
        this.Value = stringForm;
    }
}