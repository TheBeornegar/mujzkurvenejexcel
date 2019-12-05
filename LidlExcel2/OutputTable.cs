using System;
using System.Collections.Generic;
using System.Text;

namespace LidlExcel2
{
    public class OutputTable
    {
        public List<string[]> Table = new List<string[]>();


        public void Compute_Cells(InputTable input, CellComputer computer)
        {
            while(input.To_Compute.Count != 0)
            {
                computer.Cycle_Found = false;
                computer.Compute_Cell(input.To_Compute[0],input,ref Table);
            }
        }

        public void Add_Row_To_Table(string[] row,int row_number, ref List<Tuple<int, int, string>> cell_list)
        {
            CellComputer computer = new CellComputer();
            string[] output_row = new string[row.Length];
            for (int i = 0; i < row.Length; i++)
            {
                if(computer.TypeCell(row[i]) == "empty")
                {
                    output_row[i] = "[]";
                }
                else if(computer.TypeCell(row[i]) == "value")
                {
                    output_row[i] = row[i];
                }
                else if (computer.TypeCell(row[i]) == "formula")
                {
                    var new_cell = new Tuple<int, int, string>(row_number, i, row[i]);
                    cell_list.Add(new_cell);
                    output_row[i] = row[i];
                }
                else
                {
                    output_row[i] = "#INVAL";
                }
            }

            Table.Add(output_row);
        }

        public Cell Make_Cell(int row, int column, string formula)
        {
            Cell new_cell = new Cell();
            new_cell.Column = column;
            new_cell.Formula = formula;
            new_cell.Row = row;
            return new_cell;
        }
    }
}
