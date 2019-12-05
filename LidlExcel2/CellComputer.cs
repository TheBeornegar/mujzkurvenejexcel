using System;
using System.Collections.Generic;
using System.Text;

namespace LidlExcel2
{
    public class CellComputer
    {/// <summary>
    /// List of cells that are being computed, first value is row, second is column.
    /// </summary>
        public List<Tuple<int,int>> Working_Cells = new List<Tuple<int,int>>(); //row, column

        public bool Cycle_Found;

        /// <summary>
        /// Translates the alphabetic representation of column into numeric 0 based representation.
        /// </summary>
        /// <param name="column_identifier">Alphabetic representation</param>
        /// <returns></returns>
        public int TranslateColumn(string column_identifier) //v podstate horner
        {
            int column_int = 0;
            for (int i = 0; i < column_identifier.Length; i++)
            {
                column_int *= 26;
                column_int += column_identifier[i] - '@';
            }
            return column_int - 1;
        }

        public string TypeCell(string cell)
        {
            int result;
            if (cell == "[]")
            {
                return "empty";
            }
            else if (cell.Substring(0, 1) == "=")
            {
                return "formula";
            }
            else if (int.TryParse(cell, out result))
            {
                if (result >= 0)
                {
                    return "value";
                }
                else
                {
                    return "inval";
                }
            }
            else
            {
                return "inval";
            }
        }


        public bool TryTranslation(string cell, out int resultColumn, out int resultRow)
        {
            string column = "";
            string row = "";
            int i = 0;
            
            for (; i < cell.Length && char.IsUpper(cell[i]); i++)
            {
                column += cell[i];
            }

            for (; i < cell.Length && char.IsDigit(cell[i]); i++)
            {
                row += cell[i];
            }

            if (row.Length + column.Length == cell.Length) //correct format
            {
                resultColumn = TranslateColumn(column);
                resultRow = int.Parse(row) - 1;
                return true;
            }
            else //error
            {
                resultColumn = 0;
                resultRow = 0;
                return false;
            }
        }

        public bool TryIndexOutOfRange(int column, int row, ref List<string[]> output_table)
        {
            if (row >= output_table.Count || column >= output_table[row].Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Compute_Operation(string a, string b,char operand)
        {
            bool first = int.TryParse(a, out int a_int);
            bool second = int.TryParse(b, out int b_int);
            if (first && second)
            {
                int result;
                switch (operand)
                {
                    case '+':
                        result = a_int + b_int;
                        break;
                    case '-':
                        result = a_int * b_int;
                        break;
                    case '*':
                        result = a_int * b_int;
                        break;
                    case '/':
                        if (b_int != 0)
                        {
                            result = a_int / b_int;
                            break;
                        }
                        else
                            return "#DIV0";
                    default:
                        return "#MISSOP";
                }
            return result.ToString();
            }
            else
            {
                return "#ERROR"; 
            }


        }
        public string Check_And_Compute(string[] formulae, char operand, ref List<string[]> output_table,InputTable input)
        {
                if(TryTranslation(formulae[0],out int result_column1,out int result_row1) && TryTranslation(formulae[1], out int result_column2, out int result_row2)) //je validni souradnice
                {
                    string result1 = "";
                    string result2 = "";
                    if(TryIndexOutOfRange(result_column1, result_row1, ref output_table))//je/neni v tabulce, muzu ukazovat na neco, co jeste neni spocitany
                    {
                        result1 = "0";
                    }
                    else
                    {
                        if (TypeCell(output_table[result_row1][result_column1]) == "empty")
                        {
                            result1 = "0";
                        }
                        else if (TypeCell(output_table[result_row1][result_column1]) == "inval")
                        {
                            result1= "#inval1";
                        }
                        else if (TypeCell(output_table[result_row1][result_column1]) == "formula")
                        {
                            var cell_in_formula_1 = new Tuple<int, int, string>(result_row1,result_column1, output_table[result_row1][result_column1]);
                            
                            Compute_Cell(cell_in_formula_1,input,ref output_table);

                            result1 = output_table[result_row1][result_column1];
                        }
                        else //value
                        {
                            result1 = output_table[result_row1][result_column1];
                        }
                    }
                    
                    if(TryIndexOutOfRange(result_column2, result_row2, ref output_table))//je/neni v tabulce
                    {
                        result2 = "0";
                    }
                    else
                    {
                        if (TypeCell(output_table[result_row2][result_column2]) == "empty")
                        {
                            result2= "0";
                        }
                        else if (TypeCell(output_table[result_row2][result_column2]) == "inval")
                        {
                            result2= "#inval2";
                        }
                        else if (TypeCell(output_table[result_row2][result_column2]) == "formula")
                        {
                            var cell_in_formula_2 = new Tuple<int, int, string>(result_row2,result_column2, output_table[result_row2][result_column2]);
                            Compute_Cell(cell_in_formula_2,input,ref output_table);

                            result2 = output_table[result_row2][result_column2];
                        } //else value
                        else
                        {
                            result2 = output_table[result_row2][result_column2];
                        }

                    }

                    if(Cycle_Found)
                    {
                        return "#CYCLE";
                    }
                    else
                    {
                        return Compute_Operation(result1, result2, operand);
                    }
                }
                else
                {
                    return "#FORMULA";
                }
        }

        public bool Try_Find_Operand(string formula, out char operand)
        {
           if(formula.Contains('+'))
           {
                operand = '+';
                return true;
           }
           else if(formula.Contains('-'))
           {
                operand = '-';
                return true;
           }
           else if(formula.Contains('*'))
           {
                operand = '*';
                return true;
           }
           else if(formula.Contains('/'))
           {
                operand = '/';
                return true;
           }
           else
           {
                operand = 'f';
                return false;
           }
        }

        public string Compute_Formula(Tuple<int, int, string> current_cell, ref List<string[]> output_table,InputTable input)
        {
            if (Try_Find_Operand(current_cell.Item3, out char operand))
            {
                string[] formulae = current_cell.Item3.Split(operand);
                formulae[0] = formulae[0].Substring(1); //getting rid of =
                return Check_And_Compute(formulae, operand, ref output_table,input);
            }
            else
            {
                return "#MISSOP";
            }
        }

        public void Compute_Cell(Tuple<int, int, string> computed_cell,InputTable input, ref List<string[]> output_table) //Tohle zavolam, pouze pokud bunka, ktera je ve formuli je formule, jinak volam primo value
        {
            var coordinates = new Tuple<int, int>(computed_cell.Item1, computed_cell.Item2);
            if (!Working_Cells.Contains(coordinates)) //neni v cyklu 
            {
                Working_Cells.Add(coordinates);
                output_table[computed_cell.Item1][computed_cell.Item2] = Compute_Formula(computed_cell,ref output_table,input);

                Working_Cells.Remove(coordinates); //kdyz neeni, tak to nepadne, pog, takze to tu muzu nechat
            }
            else //je v cyklu
            {
                Cycle_Found = true;
                //Working_Cells.Remove(coordinates);
            }
            input.To_Compute.Remove(computed_cell);
        }
    }
}
