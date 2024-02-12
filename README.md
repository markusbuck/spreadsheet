Authors: Kevin Soto-Mirand and Markus Buckwalter

Our spreadsheet is a basic spreadsheet in which we added the option to highlight cells in the
spreadsheet.

10/13/23 - Started implementing GUI. Set up project with Skeleton.

10/16/23 - Set up basic functionality where a user would be able to enter text into a text
box to enter data into a selected cell, the user could see what cell is selected, and
what the contents of the given cell were. Added the basic spreadsheet backend functionality.
Problems: To set the cells contents you need to click on the cell. Can get confusing,
pressing enter would be better functionality.

10/18/23 - Changed where the spreadsheet functionality would live in the code. Changed it
to be intialized in the SpreadsheetGrid file instead of the MainPage.xaml.cs file. Added code
open a file into the spreadsheet.
Problems: Had issues where cells would not recalulate and data would show up in wrong cells,
but we were able to fix.

10/20/23 - Changed where the spreadsheet functionality would live again, moved it back to the
MainPage.xaml.cs Added our extra feature where a user can highlight selected cells.
Problems: Highlight was not working properly and not highlighting the right color.

10/21/23 - Reformatted the GUI and changed the look of the GUI. Changed the look of the
highlight button.

10/23/23 - Added a help page. Fixed bugs when opening the a saved spreadsheet file. Needed
to reset everything to make it like opening a new file.
