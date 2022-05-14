using System;
using IntepretadorSAL;

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

                    Intepretador.Intepretar(comandos);

                    //Outputs
                    Console.Read();
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