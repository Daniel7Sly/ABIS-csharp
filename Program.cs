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
                        
                        try
                        {
                            Interpreter.Intepretar(comandos);
                        }
                        catch (Interpreter.InterpretationException ex)
                        {
                            //System.Console.WriteLine("Program finished with errors!");
                            System.Console.WriteLine(ex.Message);
                        }
                        catch(System.Exception ex){
                            System.Console.WriteLine(
                                "\n-----------------<ERROR>----------------\n"+
                                "Caught Unhandled Error:"+ ex.Message+
                                "\n-----------------<ERROR>----------------"
                            );
                            //System.Console.WriteLine(ex.StackTrace);
                        }
                        System.Console.WriteLine("############################################################################");
                        if(Console.ReadLine() == "e") break;
    
                    } while (true);
                    
                    //Outputs
                    Console.WriteLine("\n\nProgram Closed.");
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

        static void Tabuda_Csharp(){

            int numero;
            do {
                Console.WriteLine("Digite o numero para ver a tabuada");
                if(int.TryParse(Console.ReadLine(), out int result)){
                    numero = result;
                    break;
                }
            } while(true);

            for(int i = 1; i <= 10; i++)
            {
                Console.WriteLine("{0} * {1} = {2}", numero, i, (numero * i));
            }

        }
    }
}