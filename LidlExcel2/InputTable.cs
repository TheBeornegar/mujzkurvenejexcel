using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LidlExcel2
{
    public class InputTable
    {/// <summary>
    /// First row, second column, third is formula.
    /// </summary>
        public List<Tuple<int,int,string>> To_Compute = new List<Tuple<int, int, string>> ();

        public string Remove_Multiple_Spaces(string line)
        {
            line = line.Trim();
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            line = regex.Replace(line, " ");
            return line;
        }
        public void Pass_Row(string line, int row_number, OutputTable output)
        {
            string clean_row = Remove_Multiple_Spaces(line);
            string[] cells = clean_row.Split(' ');
            
            output.Add_Row_To_Table(cells, row_number, ref To_Compute);
        }

        public void Add_To_To_Compute(int row, int column, string formula)
        {
            Tuple<int, int, string> new_tuple = new Tuple<int, int, string>(row, column, formula);
            To_Compute.Add(new_tuple);
        }
    }
}
