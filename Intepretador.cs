using System;

namespace IntepretadorSAL
{
    internal static class Intepretador
    {
        //São definidas globalmente para serem usadas nas exeçoes
        static string[] instrunçoes_do_Ficheiro = {};
        static int indexAtual;

        public static void Intepretar(string file_content)
        {
            //Remove quebras de linha e espaços
            file_content = file_content.Replace(" ","");
            file_content = file_content.Replace(System.Environment.NewLine,"");

            System.Console.WriteLine(file_content);
            System.Console.WriteLine("----------");

            //array com todas as instruçoes cada index contem uma instruçao
            instrunçoes_do_Ficheiro = file_content.Split(';'); //Ultimo index pode ser vazio

            //Mostra todas as instruçoes recebidas
            foreach(string instruçao in instrunçoes_do_Ficheiro)
            {
                Console.WriteLine("-"+instruçao);
            }

            Console.WriteLine("###################################");

            //Passa as instruçoes para um lista de Açoes
            // List<Açao> lista = new List<Açao>();
            // foreach(string instruçao in instrunçoes)
            // {
            //     lista.Add(new Açao(instruçao));
            // }

            //Passa as instruçoes para um array de Açoes
            Açao[] Açoes = new Açao[instrunçoes_do_Ficheiro.Length];
            for (int i = 0; i < instrunçoes_do_Ficheiro.Length-1; i++)
            {
                Açoes[i] = new Açao(instrunçoes_do_Ficheiro[i]);
            }

            //Cria lista de Variaveis
            List<Variavel> lista_Variaveis = new List<Variavel>();

            //Lista de FLAGS - Flags sao definidas antes de intrepertar o codigo
            List<Flag> lista_Flags = new List<Flag>();
            for (int i = 0; i < Açoes.Length-1; i++)
            {
                if(Açoes[i].tipoaçao == "FLAG"){
                    Flags(Açoes[i].parametros, lista_Flags, i);
                }
            }

            //TODO: *talvez*
            //Lista de Processos

            

            //intepreta as instruçoes
            for (int i = 0; i < Açoes.Length-1 ; i++)
            {
                indexAtual = i;
                switch(Açoes[i].tipoaçao){
                    case "SET":
                        Set(Açoes[i].parametros, lista_Variaveis);
                        break;
                    case "PRINT":
                        Print(lista_Variaveis,Açoes[i].parametros);
                        break;
                    case "PRINTL":
                        PrintL(lista_Variaveis,Açoes[i].parametros);
                        break;
                    case "READ":
                        Read(lista_Variaveis, Açoes[i].parametros);
                        break;
                    case "OPR":
                        Operaçao(lista_Variaveis,Açoes[i].parametros);
                        break;
                    case "EQL":
                        Equals(lista_Variaveis, Açoes[i].parametros);
                        break;
                    case "CMP":
                        Comparaçao(lista_Variaveis,Açoes[i].parametros);
                        break;
                    // case "Flag":
                    //     break;
                    case "GOTO":
                        i = Goto(Açoes[i].parametros, lista_Flags, i);
                        break;
                    case "IF":
                        int p = IF(lista_Flags,lista_Variaveis,Açoes[i].parametros);
                        if(p != -1){
                            i = p;
                        }
                        break;
                }
            }
        }

        //! Isto é apenas uma solução temporaria!
        private static void Read(List<Variavel> lista_Variaveis, string[] parametros){
            // if(parametros.Length < 1){
            //     throw new InterpretationExeption("Parametro em falta");
            // }

            if(parametros[0][0] == '$'){
                Variavel? var = lista_Variaveis.Find(x => x.id == parametros[0]);
                if(var == null){
                    throw new InterpretationExeption("'1ºP' Variavel não encontrada/definida.");
                }
                else{
                    string? value;
                    do{
                        value = Console.ReadLine();
                    }while(value == null);

                    //! Isto é apenas uma solução temporaria!
                    switch(var.type){
                        case "string":
                            var.value = value;
                            break;
                        case "bool":
                            if(bool.TryParse(value, out bool r)){
                                var.value = r.ToString();
                            }
                            else{
                                throw new InterpretationExeption("Não foi possivel definir valor para variavel de tipo bool.");
                            }
                            break;
                        case "num":
                            if(float.TryParse(value, out float r1)){
                                var.value = r1.ToString();
                            }
                            else{
                                throw new InterpretationExeption("Não foi possivel definir valor para variavel de tipo num.");
                            }
                            break;
                    }
                }
            }
            else{
                throw new InterpretationExeption("1º Parametro não é variavel.");
            }
        }

        //! Isto é apenas uma solução temporaria!
        private static void Print(List<Variavel> lista_Variaveis, string[] parametros){

            // if(parametros.Length < 1){
            //     throw new InterpretationExeption("Parametro em falta");
            // }

            if(parametros[0][0] == '$'){
                Variavel? var_result = lista_Variaveis.Find(x => x.id == parametros[0]);
                if(var_result == null){
                    throw new InterpretationExeption("'1ºP' Variavel não encontrada/definida.");
                }
                else{
                    //! Isto é apenas uma solução temporaria!
                    Console.Write(var_result.value);
                }
            }
            else{
                //! Isto é apenas uma solução temporaria!
                Console.Write(parametros[0].Replace('_',' '));
            }
        }
        
        //! Isto é apenas uma solução temporaria!
        private static void PrintL(List<Variavel> lista_Variaveis, string[] parametros){
            Print(lista_Variaveis,parametros);
            Console.Write('\n');
        }

        private static void Comparaçao(List<Variavel> lista_Variaveis, string[] parametros){
            //Valida a quantidade de parametros
            if(parametros.Length != 4){
                throw new InterpretationExeption("Quantidade de parametros invalida.");
            }

            //Valida o 1º parametro, tem de ser variavel.
            if(parametros[0][0] != '$'){
                throw new InterpretationExeption("'1ºP' Primeiro parametro não é variavel.");
            }
            Variavel? var_result = lista_Variaveis.Find(x => x.id == parametros[0]);
            if(var_result == null){
                throw new InterpretationExeption("Variavel não encontrada/definida.");
            }
            if(var_result.type != "bool"){
                throw new InterpretationExeption("Valor a ser atribuido a variavel não numerica.");
            }
            
            string tipo;
            //Valida 3º parametro comparador
            switch(parametros[2]){
                case "<": case ">": case "<=": case ">=":
                    tipo = "num";
                    break;
                case "==": case "!=":
                    tipo = "text";
                    break;
                default:
                    throw new InterpretationExeption("Comparador invalido");
            }

            string valor1;
            //Valida 2º parametro
            if(parametros[1][0] == '$'){
                Variavel? var2 = lista_Variaveis.Find(x => x.id == parametros[1]);
                if(var2 == null){
                    throw new InterpretationExeption("Variavel não encontrada/definida.");
                }
                if(var2.type != tipo){
                    throw new InterpretationExeption("Tipo de variavel invalida para a comparaçao pedida.");
                }

                valor1 = var2.value;
            }
            else{//caso não seja variavel
                if(tipo == "num"){
                    if(float.TryParse(parametros[1],out float r)){
                        valor1 = r.ToString();
                    }
                    else{
                        throw new InterpretationExeption("Não foi possivel converter parametro para int.");
                    }
                }
                else{
                    valor1 = parametros[1];
                }
            }

            string valor2;
            //Valida o 4º parametro
            if(parametros[3][0] == '$'){
                Variavel? var2 = lista_Variaveis.Find(x => x.id == parametros[3]);
                if(var2 == null){
                    throw new InterpretationExeption("Variavel não encontrada/definida.");
                }
                if(var2.type != tipo){
                    throw new InterpretationExeption("Tipo de variavel invalida para a comparaçao pedida.");
                }

                valor2 = var2.value;
            }
            else{//caso não seja variavel
                if(tipo == "num"){
                    if(float.TryParse(parametros[3],out float r)){
                        valor2 = r.ToString();
                    }
                    else{
                        throw new InterpretationExeption("Não foi possivel converter parametro para num.");
                    }
                }
                else{
                    valor2 = parametros[3];
                }
            }

            //Faz a comparação
            if(tipo == "num"){
                float numval1 = float.Parse(valor1);
                float numval2 = float.Parse(valor2);

                switch(parametros[2]){
                    case "<":
                        if(numval1 < numval2){
                            var_result.value = "true";
                        }
                        else{
                            var_result.value = "false";
                        }
                        break;
                    case ">":
                        if(numval1 > numval2){
                            var_result.value = "true";
                        }
                        else{
                            var_result.value = "false";
                        }
                        break;
                    case "<=":
                        if(numval1 <= numval2){
                            var_result.value = "true";
                        }
                        else{
                            var_result.value = "false";
                        }
                        break;
                    case ">=":
                        if(numval1 >= numval2){
                            var_result.value = "true";
                        }
                        else{
                            var_result.value = "false";
                        }
                        break;
                    default:
                        //não é suposto vir pra qui
                        break;
                }
            }
            else{
                switch(parametros[2]){
                    case "==":
                        if(valor1 == valor2){
                            var_result.value = "true";
                        }
                        else{
                            var_result.value = "false";
                        }
                        break;
                    case "!=":
                        if(valor1 != valor2){
                            var_result.value = "true";
                        }
                        else{
                            var_result.value = "false";
                        }
                        break;
                }
            }
        }

        private static void Operaçao(List<Variavel> lista_Variaveis, string[] parametros){
            //Valida a quantidade de parametros
            if(parametros.Length != 4){
                throw new InterpretationExeption("Quantidade de parametros invalida.");
            }
            
            //Valida o 1º parametro
            if(parametros[0][0] != '$'){
                throw new InterpretationExeption("Primeiro parametro não é variavel.");
            }
            Variavel? var = lista_Variaveis.Find(x => x.id == parametros[0]);
            if(var == null){
                throw new InterpretationExeption("Variavel não encontrada/definida.");
            }
            if(var.type != "num"){
                throw new InterpretationExeption("Valor a ser atribuido a variavel não numerica.");
            }

            float valor1;
            //Valida 2º parametro e atribui o valor á variavel varlor1
            if(parametros[1][0] == '$'){
                Variavel? var2 = lista_Variaveis.Find(x => x.id == parametros[1]);
                if(var2 == null){
                    throw new InterpretationExeption("Variavel não encontrada/definida.");
                }
                if(var2.type != "num"){
                    throw new InterpretationExeption("Valor a ser operado a variavel não numerica.");
                }

                valor1 = float.Parse(var2.value);
            }
            else{//caso não seja variavel
                if(float.TryParse(parametros[1], out float r1)){
                    valor1 = r1;
                }
                else{
                    throw new InterpretationExeption("Valor numerico não valido.");
                }
            }

            float valor2;
            //Valida 4º parametro e atribui o valor á variavel varlor2
            if(parametros[3][0] == '$'){
                Variavel? var3 = lista_Variaveis.Find(x => x.id == parametros[3]);
                if(var3 == null){
                    throw new InterpretationExeption("Variavel não encontrada/definida.");
                }
                if(var3.type != "num"){
                    throw new InterpretationExeption("Valor a ser operado a variavel não numerica.");
                }

                valor2 = float.Parse(var3.value);
            }
            else{//caso não seja variavel
                if(float.TryParse(parametros[3], out float r1)){
                    valor2 = r1;
                }
                else{
                    throw new InterpretationExeption("Valor numerico não valido.");
                }
            }

            //Operaçao
            switch(parametros[2]){
                case "+":
                    var.value = (valor1 + valor2).ToString();
                    break;
                case "-":
                    var.value = (valor1 - valor2).ToString();
                    break;
                case "*":
                    var.value = (valor1 * valor2).ToString();
                    break;
                case "/":
                    var.value = (valor1 / valor2).ToString();
                    break;
                default:
                    throw new InterpretationExeption("Operador invalido.");
            }
        }

        private static int IF(List<Flag> lista_Flags, List<Variavel> lista_Variaveis, string[] parametros){
            //Valida a variavel 1º parametro
            if(parametros.Length < 2){
                throw new InterpretationExeption("Parametros em faltas.");
            }
            if(parametros[0][0] != '$'){
                throw new InterpretationExeption("1º parametro não é variavel");
            }
            Variavel? var = lista_Variaveis.Find(x => x.id == parametros[0]);
            if(var == null){
                throw new InterpretationExeption("Variavel não encontrada");
            }
            if(var.type != "bool"){
                throw new InterpretationExeption("Variavel de tipo invalido");
            }

            //Valida a flag 2º parametro
            Flag? flag = lista_Flags.Find(x => x.nome == parametros[1]);
            if(flag == null){
                throw new InterpretationExeption("Flag não encontrada/definida");
            }

            //Retorna a posiçao da flag
            if(var.value == "true"){
                return flag.posiçao;
            }
            //Retorna -1 caso seja false
            return -1;
        }   

        private static void Equals(List<Variavel> lista_Variaveis, string[] parametros){
            //verifica se chegou dois parametros
            if(parametros.Length < 2){
                throw new InterpretationExeption("Parametros em falta.");
            }

            //verifica se o primeiro parametro é uma variavel valida
            if(parametros[0][0] != '$'){
                throw new InterpretationExeption("1º parametro não é variavel");
            }
            Variavel? var1 = lista_Variaveis.Find(x => x.id == parametros[0]);
            if(var1 == null){
                throw new InterpretationExeption("1ª variavel não encontrada.");
            }

            Variavel? var2 = null;
            if(parametros[1][0] == '$'){
                //Valida a 2ª variavel
                var2 = lista_Variaveis.Find(x => x.id == parametros[1]);
                if(var2 == null){
                    throw new InterpretationExeption("2º parametro variavel não encontrada.");
                }
                
                //Define o varlor da var2 á var1
                if(var1.type == var2.type){
                    var1.value = var2.value;
                }
                else{
                    throw new InterpretationExeption("Variaveis não são do mesmo tipo.");
                }
            }
            else{//caso não seja variavel
                //valida 2º parametro
                if(parametros[1] == "" && var1.type != "text"){
                    throw new InterpretationExeption("2º parametro vazio.");
                }
                
                //Aplica a operaçao correspondente ao tipo de dado
                switch(var1.type){
                    case "num":
                        if(float.TryParse(parametros[1], out float result_num)){
                            var1.value = result_num.ToString();
                        }
                        else{
                            throw new InterpretationExeption("Não foi possivel converter parametro para num.");
                        }
                        break;
                    case "bool":
                        if(bool.TryParse(parametros[1], out bool result_bool)){
                            var1.value = result_bool.ToString();
                        }
                        else{
                            throw new InterpretationExeption("Não foi possivel converter parametro para bool.");
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

        private static int Goto(string[] parametros, List<Flag> lista_Flags, int i){
            //valida o parametro
            if(parametros[0] == ""){
                throw new InterpretationExeption("Parametro em falta.");
            }
            Flag? flag = lista_Flags.Find(x => x.nome == parametros[0]);
            if (flag == null)
            {
                throw new InterpretationExeption("Flag não encontrada.");
            }
            
            //Define i para a posiçao da flag
            i = flag.posiçao;
            return i;
        }

        private static void Set(string[] parametros, List<Variavel> lista_Variaveis){
            //Verifica se chegam três parametros
            if(parametros == null || parametros.Length != 3){
                throw new InterpretationExeption("parametros em falta.");
            }

            //Valida o 1º parametro se é um tipo valido
            if(parametros[0] != "num" && parametros[0] != "bool" && parametros[0] != "text"){
                throw new InterpretationExeption("1º parametro tipo invalido. Type must be num, bool, or text.");
            }
            string type = parametros[0];            

            //Valida o 2º parametro
            if(parametros[1] == ""){
                throw new InterpretationExeption("Nome de variavel vazio / não definida.");
            }
            if(lista_Variaveis.Find(x => x.id == "$"+parametros[1]) != null){
                throw new InterpretationExeption("Variavel ja existente.");
            }
            string name = parametros[1];

            //Aplica o valor de acordo com o tipo expecificado
            string value = "";
            switch(parametros[0]){
                case "num":
                    if(float.TryParse(parametros[2], out float result_num)){
                        value = result_num.ToString();
                    }
                    else{
                        throw new InterpretationExeption("Não foi possivel converter parametro para num.");
                    }
                    break;
                case "bool":
                    if(bool.TryParse(parametros[2], out bool result_bool)){
                        value = result_bool.ToString();
                    }
                    else{
                        throw new InterpretationExeption("Não foi possivel converter parametro para bool.");
                    }
                    break;
                case "text":
                    value = parametros[2];
                    break;
            }

            lista_Variaveis.Add(new Variavel(type,name,value));
        }

        private static void Flags(string[] parametros, List<Flag> lista_Flags, int i){
            //Valida o parametro nome
            if(parametros == null || parametros.Length != 1 || parametros[0] == ""){
                throw new InterpretationExeption("Parametro 'nome' invalido.");
            }
            if(lista_Flags.Find(x => x.nome == parametros[0]) != null){
                throw new InterpretationExeption("Nome de Flag repetido/já existente");
            }

            //Acrescenta a flag á lista de flags
            lista_Flags.Add(new Flag(parametros[0], i));
        }

        private class Variavel{
            public string type, id, value;

            public Variavel(string type, string name, string value){
                this.type = type;
                this.id = '$'+name;
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
            public string[] parametros = {}; //<-- Amamm

            public Açao(string instruçao){                      //  |--------instruçao:-------|
                if(instruçao != ""){                            //  |                         |       
                    string[] palavras =  instruçao.Split(':');  //  Set  :    type | id | value ;
                    this.tipoaçao = palavras[0].ToUpper();      //   ^        |       ^       |
                    //this.parametros = palavras[1].Split('|'); //tipoaçao    |--Parametros---|
                    string[] a = palavras[1].Split('|');
                    this.parametros = a;
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

        [System.Serializable]
        public class InterpretationExeption : System.Exception{
            //public InterpretationExeption() {}
            public InterpretationExeption(string message) : base(MontarMensagemErro(message)) {}
            public InterpretationExeption(int parametro, string message) : base(MontarMensagemErro(message)) {}

            //Monta uma mesagem que mostra a linha onde ocorreu o erro com as 3 linhas anteriores e a menssagem de erro;
            private static string MontarMensagemErro(int parametro, string mensagem){
                string ultimas3Instruçoes = "";
                if(indexAtual >= 3)
                    ultimas3Instruçoes = instrunçoes_do_Ficheiro[indexAtual-3]+'\n'+instrunçoes_do_Ficheiro[indexAtual-2]+'\n'+instrunçoes_do_Ficheiro[indexAtual-1]+'\n';
                string instruçaoComErro = instrunçoes_do_Ficheiro[indexAtual];
                return ("\n \n"+ultimas3Instruçoes + instruçaoComErro + " <-- \n Falha no: "+parametro+"º parametro.\n" + mensagem);
            }

            private static string MontarMensagemErro(string mensagem){
                string ultimas3Instruçoes = "";
                if(indexAtual >= 3)
                    ultimas3Instruçoes = instrunçoes_do_Ficheiro[indexAtual-3]+'\n'+instrunçoes_do_Ficheiro[indexAtual-2]+'\n'+instrunçoes_do_Ficheiro[indexAtual-1]+'\n';
                string instruçaoComErro = instrunçoes_do_Ficheiro[indexAtual];
                return ("\n \n"+ultimas3Instruçoes + instruçaoComErro + " <-- \n \n" + mensagem);
            }

            public InterpretationExeption(string message, System.Exception inner) : base(MontarMensagemErro(message), inner) { }
            // protected InterpretationExeption(
            //     System.Runtime.Serialization.SerializationInfo info,
            //     System.Runtime.Serialization.StreamingContext context) : base(info, context) {

            //     }
        }
    }
    
}
