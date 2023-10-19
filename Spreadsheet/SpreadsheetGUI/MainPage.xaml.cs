using System.Text.RegularExpressions;
using SS;
using SpreadsheetUtilities;
using CommunityToolkit.Maui.Storage;
using System.Text;
namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
    public delegate void SaveEventhHandler();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
        
        spreadsheetGrid.SetSelection(0, 0);
        CellLocation.Text = ConvertToCellName(0, 0);
    }


    private void displaySelection(ISpreadsheetGrid grid)
    {
        try
        {
            spreadsheetGrid.GetSelection(out int col, out int row);
            spreadsheetGrid.GetValue(col, row, out string value);

            if (!(entryBoxText.Text == null))
            {
                spreadsheetGrid.SetValue(col, row, entryBoxText.Text);
            }
            else
            {
                spreadsheetGrid.SetValue(col, row, "");
            }

            if (entryBoxText.Text == null)
            {
                CellContents.Text = "Cell contents";
            }

            else if (entryBoxText.Text == "")
            {
                CellContents.Text = "Cell contents";
            }

            else
            {
                CellContents.Text = entryBoxText.Text;
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
        spreadsheetGrid.Save(path.FilePath);
    }

    private void NewClicked(Object sender, EventArgs e)
    {
        if (spreadsheetGrid.Changed)
        {
            DisplayAlert("WARNING", "Must save changes before opening a new spreadsheet", "OK");
        }
        else
        {
            spreadsheetGrid.Clear();
            spreadsheetGrid.CreateNewSpreadsheet();
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
        if (this.spreadsheetGrid.Changed)
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

                    this.spreadsheetGrid.CreateNewSpreadSheetWithFilePath(fileResult.FullPath);

                    foreach (string cell in this.spreadsheetGrid.GetNonEmptyCells())
                    {
                        this.spreadsheetGrid.ConvertToCellNameToRowCol(cell, out int col, out int row);
                        this.spreadsheetGrid.SetValue(col, row, this.spreadsheetGrid.getCellContents(cell));
                    }
                    

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

    private void HelpClicked(Object sender, EventArgs e)
    {
        DisplayAlert("Help", "Msdslkdfjskfslkjfksjdfsldfjskdjfslkjfsl", "OK");
    }

    private void HighlightButtonClick(Object sender, EventArgs e)
    {
        this.spreadsheetGrid.addHighlightedAddress();
    }
}
