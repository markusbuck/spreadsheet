using SS;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
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

        spreadsheetGrid.GetSelection(out int col, out int row);
        spreadsheetGrid.GetValue(col, row, out string value);
        if (value == "")
        {
            
            spreadsheetGrid.SetValue(col, row, entryBoxText.Text);
            spreadsheetGrid.GetValue(col, row, out value);
            //DisplayAlert("Selection:", "column " + col + " row " + row + " value " + value, "OK");
        }

        else
        {
            spreadsheetGrid.SetValue(col, row, entryBoxText.Text);
            spreadsheetGrid.GetValue(col, row, out value);
        }

        CellLocation.Text = ConvertToCellName(col, row);
    }

    private string ConvertToCellName(int col, int row)
    {
        row++;
        string colLetter = (char) (65+col) + "";
        string cellName = " " + colLetter + row;

        return cellName;
    }

    private void NewClicked(Object sender, EventArgs e)
    {
        spreadsheetGrid.Clear();
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
        Console.WriteLine( "Successfully chose file: " + fileResult.FileName );
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
}
