﻿using System;
using SpreadsheetUtilities;

namespace SS;

public class Spreadsheet : AbstractSpreadsheet
{
	public Spreadsheet()
	{
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

