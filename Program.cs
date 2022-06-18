using System;
using AbisInterpreter;

namespace InterpretadorAbis
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Any())
            {
                var path = args[0];
                if (File.Exists(path))
                {
                    do
                    {
                        //Recives and reads the file
                        StreamReader input = File.OpenText(path);
                        string comandos = input.ReadToEnd();
                        input.Close();
                        //Console.WriteLine(comandos);
                        // try
                        // {
                             Interpreter.Intepretar(comandos);
                        // }
                        // catch (Interpreter.InterpretationExeption ex)
                        // {
                        //     System.Console.WriteLine("Program finished with errors!");
                        //     System.Console.WriteLine(ex.Message);
                        // }
                        System.Console.WriteLine("-----------------------------------------------------------");
                        if(Console.ReadLine() == "e") break;
    
                    } while (true);
                    
                    //Outputs
                    Console.WriteLine("\n\nProgram finished.");
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
            else
            {
                Console.WriteLine("No File Expecified!");
            }
        }
    }
}