using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LidlExcel2
{
    class Writer
    {
        public void Write_Output_Table(OutputTable output_table, string output_file)
        {
            using (var writer = new StreamWriter(output_file))
            {
                for (int i = 0; i < output_table.Table.Count; i++)
                {
                    for (int j = 0; j < output_table.Table[i].Length; j++)
                    {
                        writer.Write(output_table.Table[i][j]);
                        if(j != output_table.Table[i].Length -1)
                        {
                            writer.Write(" ");
                        }
                    }

                    if(i != output_table.Table.Count -1)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}
