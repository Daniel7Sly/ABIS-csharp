using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntepretadorSAL
{
    internal static class Intepretador
    {
        public static void Intepretar(string file_content)
        {
            //Remove quebras de linha e espaços
            file_content = file_content.Replace(" ","");
            file_content = file_content.Replace(System.Environment.NewLine,"");

            System.Console.WriteLine(file_content);
            System.Console.WriteLine("----------");

            //array com todas as instruçoes cada index contem uma instruçao
            string[] instrunçoes_do_Ficheiro = file_content.Split(';'); //Ultimo index é vazio

            //Mostra todas as instruçoes recebidas
            foreach(string instruçao in instrunçoes_do_Ficheiro)
            {
                Console.WriteLine("-"+instruçao);
            }

            //Passa as instruçoes para um lista de Açoes
            // List<Açao> lista = new List<Açao>();
            // foreach(string instruçao in instrunçoes)
            // {
            //     lista.Add(new Açao(instruçao));
            // }

            //Passa as instruçoes para um array de Açoes
            Açao[] instrunçoes = new Açao[instrunçoes_do_Ficheiro.Length-2];
            for (int i = 0; i < instrunçoes_do_Ficheiro.Length-2; i++)
            {
                instrunçoes[i] = new Açao(instrunçoes_do_Ficheiro[i]);
            }

            //Cria lista de Variaveis
            List<VariavelBase> lista_Variaveis = new List<VariavelBase>();
            //lista_Variaveis.Add(new Variavel<string>("AMA"));
            //AMAMAMAMAMAMAM
            


            //intepreta as instruçoes
            for (int i = 0; i < instrunçoes.Length-1 ; i++)
            {
                switch(instrunçoes[i].tipoaçao){
                    case "Set":

                        break;
                    case "Print":

                        break;
                    case "Read":

                        break;
                    case "Opr":

                        break;
                    case "Eql":

                        break;
                    case "Cmp":

                        break;
                    case "Flag":

                        break;
                    case "Goto":

                        break;
                    case "If":

                        break;
                }
            }
        }
    }

    class Açao{
        public string tipoaçao;
        public string[]? parametros;

        public Açao(string instruçao){
            if(instruçao != ""){
                string[] palavras = instruçao.Split(':');
                this.tipoaçao = palavras[0];
                this.parametros = palavras[1].Split('|');
            }
            else{
                this.tipoaçao = "FIM";
            }
        }
    }
    class VariavelBase{
        // public string nome;
    }

    class Variavel<T>{
        public string nome;
        public T value;

        public Variavel(T Valor){
            this.value = Valor;
        }
    }
}
