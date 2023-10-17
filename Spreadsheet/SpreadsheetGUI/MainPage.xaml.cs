using System.Text.RegularExpressions;
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
    public delegate void SaveEventhHandler();



    AbstractSpreadsheet spreadsheet = new Spreadsheet(x =>
    {
        if (Regex.IsMatch(x, @"^[A-Z][0-9]{1,2}$"))
        {
            return true;
        }
        else
            return false;
    }, x => x.ToUpper(), "ps6");

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

            string name = ConvertToCellName(col, row);
            if (!(entryBoxText.Text == null))
            {
                spreadsheet.SetContentsOfCell(name, entryBoxText.Text);
            }
            else
            {
                spreadsheet.SetContentsOfCell(name, "");
            }
            if (!(entryBoxText.Text == null))
            {
                spreadsheet.SetContentsOfCell(name, entryBoxText.Text);
            }
            else
            {
                spreadsheet.SetContentsOfCell(name, "");
            }

            if (value == "")
            {
                spreadsheetGrid.SetValue(col, row, entryBoxText.Text);
                spreadsheetGrid.GetValue(col, row, out value);
            }

            else
            {
                spreadsheetGrid.SetValue(col, row, spreadsheet.GetCellValue(name).ToString());
                spreadsheetGrid.GetValue(col, row, out value);
            }
            CellContents.Text = entryBoxText.Text;
            CellLocation.Text = ConvertToCellName(col, row);
        }
        catch (FormulaFormatException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
        catch(CircularException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }

    }

    private string ConvertToCellName(int col, int row)
    {
        //if(65 + col >= 65 && 65 + col <= 90 || 65 + col >= 97 && 65 + col <= 122)
        // {
        //     row++;
        //     string colLetter = (char)(65 + col) + "";
        //     string cellName = " " + colLetter + row;

        //     return cellName;
        // }
        row++;
        string colLetter = (char)(65 + col) + "";
        string cellName = "" + colLetter + row;

        Console.WriteLine(cellName);
        return cellName;
    }

    private void SaveClicked(Object sender, EventArgs e)
    {
        spreadsheet.Save("someFileName");
    }

    private void NewClicked(Object sender, EventArgs e)
    {
        if (spreadsheet.Changed == true)
        {
            DisplayAlert("WARNING", "Must save changes before opening a new spreadsheet", "OK");
        }
        else
        {
            spreadsheetGrid.Clear();
            spreadsheet = new Spreadsheet();
        }
    }

    /// <summary>
    /// Opens any file as text and prints its contents.
    /// Note the use of async and await, concepts we will learn more about
    /// later this semester.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {
        try
        {
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                Console.WriteLine("Successfully chose file: " + fileResult.FileName);
                // for windows, replace Console.WriteLine statements with:
                //System.Diagnostics.Debug.WriteLine( ... );

                string fileContents = File.ReadAllText(fileResult.FullPath);
                Console.WriteLine("First 100 file chars:\n" + fileContents.Substring(0, 100));
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

    void MenuFlyoutItem_Clicked(System.Object sender, System.EventArgs e)
    {

    }
}
