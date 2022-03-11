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
            Açao[] Açoes = new Açao[instrunçoes_do_Ficheiro.Length-2];
            for (int i = 0; i < instrunçoes_do_Ficheiro.Length-2; i++)
            {
                Açoes[i] = new Açao(instrunçoes_do_Ficheiro[i]);
            }

            //Cria lista de Variaveis
            List<Variavel> lista_Variaveis = new List<Variavel>();

            //Lista de FLAGS
            List<Flag> lista_Flags = new List<Flag>();

            //TODO:
            //Lista de Processos

            //intepreta as instruçoes
            for (int i = 0; i < Açoes.Length-1 ; i++)
            {
                switch(Açoes[i].tipoaçao){
                    case "Set":
                        Set(Açoes, lista_Variaveis, i);
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

        private static void Set(Açao[] Açoes, List<Variavel> lista_Variaveis, int i)
        {
            try
            {
                string type = Açoes[i].parametros[0];
                string id = Açoes[i].parametros[1];
                string value = Açoes[i].parametros[2];
                Variavel var = new Variavel(type, id, value);
                if (lista_Variaveis.Find(x => x.id == id) == null)
                {
                    lista_Variaveis.Add(var);
                }
            }
            catch (System.Exception ex)
            {
                Exception exception = new Exception("Falha ao Criar Variavel. Parametros em falta?",ex);
                throw exception;
            }
        }
    }

    class Açao{
        public string tipoaçao;
        public string[]? parametros;

        public Açao(string instruçao){
            if(instruçao != ""){
                string[] palavras = instruçao.Split(':');   //  Set  :    type | id | value;
                this.tipoaçao = palavras[0];                //   ^               ^
                this.parametros = palavras[1].Split('|');   //tipoaçao       Parametros
            }
            else{
                this.tipoaçao = "FIM";
            }
        }
    }

    // public class VariavelT<T>{
    //     public string nome;
    //     public T value;

    //     public Variavell(T Valor){
    //         this.value = Valor;
    //     }
    // }

    class Variavel{
        public string type, id, value;

    public Variavel(string type, string id, string value){
        this.type = type;
        this.id = id;
        this.value = value;
    }
}

    class Flag{
        public string nome;
        public int posiçao;

        public Flag(string nome, int posiçao){
            this.nome = nome;
            this.posiçao = posiçao;
        }
    }
}
