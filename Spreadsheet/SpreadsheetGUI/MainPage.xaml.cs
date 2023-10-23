using System.Text.RegularExpressions;
using SS;
using SpreadsheetUtilities;
using CommunityToolkit.Maui.Storage;
using System.Text;
using Microsoft.Maui.Storage;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
    public delegate void SaveEventhHandler();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    public Spreadsheet spreadsheet;

    /// <summary>
    /// Constructor for the demo
    /// </summary>
	public MainPage()
    {
        InitializeComponent();

        // This an example of registering a method so that it is notified when
        // an event happens.  The SelectionChanged event is declared with a
        // delegate that specifies that all methods that register with it must
        // take a SpreadsheetGrid as its parameter and return nothing.  So we
        // register the displaySelection method below.
        spreadsheetGrid.SelectionChanged += displaySelection;

        spreadsheet = new Spreadsheet(x =>
        {
            if (Regex.IsMatch(x, @"^[A-Z][0-9]{1,2}$"))
            {
                return true;
            }
            else
                return false;
        }, x => x.ToUpper(), "ps6");


        spreadsheetGrid.SetSelection(0, 0);
        CellLocation.Text = ConvertToCellName(0, 0);
    }


    private void displaySelection(ISpreadsheetGrid grid)
    {
        spreadsheetGrid.GetSelection(out int col, out int row);
        string cellName = this.ConvertToCellName(col, row);
        string contents = this.spreadsheet.GetCellContents(cellName).ToString();
        CellLocation.Text = ConvertToCellName(col, row);

        if (contents != "")
        {
            CellContents.Text = contents;
        }

        else 
        {
            CellContents.Text = "";

        }

        if (this.spreadsheet.GetCellContents(ConvertToCellName(col, row))
                is Formula)
        {
            entryBoxText.Text =
            "=" + contents;

        }
        else
        {
            entryBoxText.Text = contents;
        }
    }

    private string ConvertToCellName(int col, int row)
    {
        row++;
        string colLetter = (char)(65 + col) + "";
        string cellName = "" + colLetter + row;

        Console.WriteLine(cellName);
        return cellName;
    }

    private async void SaveClicked(Object sender, EventArgs e)
    {

        using var stream = new MemoryStream(Encoding.Default.GetBytes(""));
        var path = await FileSaver.SaveAsync("test.sprd", stream, cancellationTokenSource.Token);
        this.spreadsheet.Save(path.FilePath);
    }

    private void NewClicked(Object sender, EventArgs e)
    {
        if (this.spreadsheet.Changed)
        {
            DisplayAlert("WARNING", "Must save changes before opening a new spreadsheet", "OK");
        }
        else
        {
            spreadsheetGrid.Clear();
            this.spreadsheet = new Spreadsheet(x =>
            {
                if (Regex.IsMatch(x, @"^[A-Z][0-9]{1,2}$"))
                {
                    return true;
                }
                else
                    return false;
            }, x => x.ToUpper(), "ps6");

            this.spreadsheetGrid.ClearHighlightedCells();

        }
    }

    /// <summary>
    /// Opens any file as text and prints its contents.
    /// Note the use of async and await, concepts we will learn more about
    /// later this semester.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {
        if (this.spreadsheet.Changed)
        {
            await DisplayAlert("WARNING", "Must save changes before opening a new spreadsheet", "OK");
        }

        else
        {

            try
            {

                FileResult fileResult = await FilePicker.Default.PickAsync();
                
                if (fileResult != null)
                {
                    this.spreadsheetGrid.ClearHighlightedCells();
                    string fileContents = File.ReadAllText(fileResult.FullPath);
                    Console.WriteLine(fileContents);

                    this.spreadsheet =  new Spreadsheet(fileResult.FullPath, x =>
                    {
                        if (Regex.IsMatch(x, @"^[A-Z][0-9]{1,2}$"))
                        {
                            return true;
                        }
                        else
                            return false;
                    }, x => x.ToUpper(), "ps6");


                    this.spreadsheetGrid.Clear();

                    foreach (string cell in this.spreadsheet.GetNamesOfAllNonemptyCells())
                    {

                        string cellContents = this.spreadsheet.GetCellValue(cell).ToString();
                        this.ConvertToCellNameToRowCol(cell, out int col, out int row);
                        this.spreadsheetGrid.SetValue(col, row, cellContents);
                    }

                    spreadsheetGrid.SetSelection(0, 0);
                    CellLocation.Text = ConvertToCellName(0, 0);
                    if (this.spreadsheet.GetCellContents(ConvertToCellName(0, 0))
                        is Formula)
                    {
                        entryBoxText.Text =
                        "=" + this.spreadsheet.GetCellContents(ConvertToCellName(0, 0)).ToString();

                    }
                    else
                    {
                        entryBoxText.Text =
                            this.spreadsheet.GetCellContents(ConvertToCellName(0, 0)).ToString();
                    }
                    CellContents.Text =
                            this.spreadsheet.GetCellContents(ConvertToCellName(0, 0)).ToString();


                }
                else
                {
                    Console.WriteLine("No file selected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening file:");
                Console.WriteLine(ex);
            }
        }
    }

    public void ConvertToCellNameToRowCol(string cellName, out int col, out int row)
    {
        int colLetter = (int)cellName[0];
        col = colLetter - 65;
        row = int.Parse(cellName.Substring(1)) - 1;
    }

    private void HelpClicked(Object sender, EventArgs e)
    {
        Navigation.PushAsync(new HelpPage());
    }

    private void YellowHighlightButtonClick(Object sender, EventArgs e)
    {
        this.spreadsheetGrid.addHighlightedAddress("#FDFD96");
    }

    private void RedHighlightButtonClick(Object sender, EventArgs e)
    {
        this.spreadsheetGrid.addHighlightedAddress("#FAA0A0");
    }

    private void BlueHighlightButtonClick(Object sender, EventArgs e)
    {
        this.spreadsheetGrid.addHighlightedAddress("#AEC6CF");
    }

    private void ClearButtonClick(Object sender, EventArgs e)
    {
        this.spreadsheetGrid.clearHighlightedAddress();
    }

    private void OnEntryCompleted(object sender, EventArgs e)
    {
        try
        {
            spreadsheetGrid.GetSelection(out int col, out int row);
            spreadsheetGrid.GetValue(col, row, out string value);

            IList<string> cellsToRecalculate;


            if (entryBoxText.Text != null)
            {
                this.spreadsheetGrid.SetValue(col, row, entryBoxText.Text);
                cellsToRecalculate = this.spreadsheet.SetContentsOfCell(ConvertToCellName(col, row), entryBoxText.Text);
            }
            else
            {
                this.spreadsheetGrid.SetValue(col, row, "");
                cellsToRecalculate = this.spreadsheet.SetContentsOfCell(ConvertToCellName(col, row), "");

            }

            if (entryBoxText.Text == null)
            {
                CellContents.Text = "";
            }

            else if (entryBoxText.Text == "")
            {
                CellContents.Text = "";
            }

            else
            {
                CellContents.Text = entryBoxText.Text;
            }

            foreach (string cell in cellsToRecalculate)
            {
                ConvertToCellNameToRowCol(cell, out int colNum, out int rowNum);
                this.spreadsheetGrid.SetValue(colNum, rowNum, this.spreadsheet.GetCellValue(cell).ToString());
            }

            if (this.spreadsheet.GetCellContents(ConvertToCellName(col, row))
                is Formula)
            {
                entryBoxText.Text =
                "=" + this.spreadsheet.GetCellContents(ConvertToCellName(col, row)).ToString();

            }
            else
            {
                entryBoxText.Text =
                    this.spreadsheet.GetCellContents(ConvertToCellName(col, row)).ToString();
            }

            CellLocation.Text = ConvertToCellName(col, row);
        }

        catch (FormulaFormatException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
        catch (CircularException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
