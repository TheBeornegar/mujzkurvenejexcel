using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LidlExcel2
{
    class Reader
    {
        public int row_counter = 0;
        public bool ReadFromFile(string[] files,InputTable input,OutputTable output)
        {
            if (files.Length != 2)
            {
                Console.WriteLine("Argument Error");
                return false;
            }
            else
            {
                try
                {
                    using (var reader = new StreamReader(files[0]))
                    {
                        while (!reader.EndOfStream)
                        {
                            string row = reader.ReadLine();
                            input.Pass_Row(row,row_counter,output);
                            row_counter++;
                        }

                        return true;
                    }

                }
                catch (IOException e)
                {
                    Console.WriteLine("File Error");
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
    }
}
