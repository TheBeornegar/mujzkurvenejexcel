using System;

namespace LidlExcel2
{
    class Program
    {
        public static void Compute_Table(InputTable input, OutputTable output)
        {
            CellComputer computer = new CellComputer();
            output.Compute_Cells(input,computer);
        }

        public static void Run(string[] arguments)
        {
            Reader reader = new Reader();
            InputTable input_table = new InputTable();
            OutputTable output_table = new OutputTable();
            if(reader.ReadFromFile(arguments,input_table,output_table))
            {
                Compute_Table(input_table, output_table);
                Writer writer = new Writer();
                writer.Write_Output_Table(output_table, arguments[1]);
            }

        }

        static void Main(string[] args)
        {
            Run(args);
        }
    }
}
