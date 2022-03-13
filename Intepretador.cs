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

            //Lista de FLAGS - Flags sao definidas antes de intrepertar o codigo
            List<Flag> lista_Flags = new List<Flag>();
            for (int i = 0; i < Açoes.Length-1; i++)
            {
                if(Açoes[i].tipoaçao == "Flag"){
                    Flags(Açoes, lista_Flags, i);
                }
            }

            //TODO: *talvez*
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
                        Equals(lista_Variaveis, Açoes[i].parametros);
                        break;
                    case "Cmp":

                        break;
                    // case "Flag":
                    //     break;
                    case "Goto":
                        i = Goto(Açoes, lista_Flags, i);
                        break;
                    case "If":

                        break;
                }
            }
        }

        private static void Equals(List<Variavel> lista_Variaveis,string[] parametros){
            //verifica se chegou dois parametros
            if(parametros.Length != 2){
                throw new Exception("Parametros em falta.");
            }

            //verifica se o primeiro parametro é uma variavel valida
            if(parametros[0][0] != '$'){
                throw new Exception("1º parametro não é variavel");
            }
            Variavel? var1 = lista_Variaveis.Find(x => x.id == parametros[0]);
            if(var1 == null){
                throw new Exception("1ª variavel não encontrada.");
            }

            Variavel? var2 = null;
            if(parametros[1][0] == '$'){
                //Valida a 2ª variavel
                var2 = lista_Variaveis.Find(x => x.id == parametros[1]);
                if(var2 == null){
                    throw new Exception("2º parametro variavel não encontrada.");
                }
                
                //Define o varlor da var2 á var1
                if(var1.type == var2.type){
                    var1.value = var2.value;
                }
                else{
                    throw new Exception("Variaveis não são do mesmo tipo.");
                }
            }
            else{//caso não seja variavel
                //valida 2º parametro
                if(parametros[1] == "" && var1.type != "text"){
                    throw new Exception("2º parametro vazio.");
                }
                
                //Aplica a operaçao correspondente ao tipo de dado
                switch(var1.type){
                    case "num":
                        if(float.TryParse(parametros[1], out float result_num)){
                            var1.value = result_num.ToString();
                        }
                        else{
                            throw new Exception("Não foi possivel converter parametro para num.");
                        }
                        break;
                    case "bool":
                        if(bool.TryParse(parametros[1], out bool result_bool)){
                            var1.value = result_bool.ToString();
                        }
                        else{
                            throw new Exception("Não foi possivel converter parametro para bool.");
                        }
                        break;
                    case "text":
                        var1.value = parametros[1];
                        break;
                    default:
                        break;
                }
            }
        }

        private static int Goto(Açao[] Açoes, List<Flag> lista_Flags, int i)
        {
            Flag flag = lista_Flags.Find(x => x.nome == Açoes[i].parametros[0]);
            if (flag != null)
            {
                i = flag.posiçao;
            }

            return i;
        }

        private static void Set(Açao[] Açoes, List<Variavel> lista_Variaveis, int i)
        {
            try
            {
                //TODO: NUll and type cheking
                string type = Açoes[i].parametros[0];
                string id = "$"+Açoes[i].parametros[1];//todo o nome de variavel começa por $
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

        private static void Flags(Açao[] Açoes, List<Flag> lista_Flags, int i)
        {
            try
            {
                string nome = Açoes[i].parametros[0];
                int posiçao = int.Parse(Açoes[i].parametros[1]);

                Flag flag = new Flag(nome, posiçao);
                if (lista_Flags.Find(x => x.nome == nome) == null)
                {
                    lista_Flags.Add(flag);
                }
            }
            catch (System.Exception ex)
            {
                Exception exception = new Exception("Falha ao Definir Flag. Parametros em falta?",ex);
                throw exception;
            }
        }

        private class Variavel{
            public string type, id, value;

            public Variavel(string type, string id, string value){
                this.type = type;
                this.id = id;
                this.value = value;
            }
        }

        // public class VariavelT<T>{
        //     public string nome;
        //     public T value;

        //     public Variavell(T Valor){
        //         this.value = Valor;
        //     }
        // }

        private class Açao{
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

        private class Flag{
            public string nome;
            public int posiçao;

            public Flag(string nome, int posiçao){
                this.nome = nome;
                this.posiçao = posiçao;
            }
        }
    }
}
