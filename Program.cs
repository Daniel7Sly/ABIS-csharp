using System;
using AbisInterpreter;

namespace Minha_Linguagem_Intepretador
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
                    //Recives and reads the file
                    StreamReader input = File.OpenText(path);
                    string comandos = input.ReadToEnd();
                    input.Close();
                    //Console.WriteLine(comandos);

                    Interpreter.Intepretar(comandos);

                    //Outputs
                    Console.WriteLine("\n\nProgram finished without errors");
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