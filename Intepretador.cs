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
            file_content = file_content.Replace("\t","");

            // System.Console.WriteLine(file_content);
            // System.Console.WriteLine("----------");

            //array com todas as instruçoes cada index contem uma instruçao
            instrunçoes_do_Ficheiro = file_content.Split(';'); //Ultimo index pode ser vazio

            //Mostra todas as instruçoes recebidas
            // foreach(string instruçao in instrunçoes_do_Ficheiro)
            // {
            //     Console.WriteLine("-"+instruçao);
            // }

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
                    case "SETARR":
                        SetArr(lista_Variaveis,Açoes[i].parametros);
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
                    case "JTXT":
                        JoinText(lista_Variaveis,Açoes[i].parametros);
                        break; 
                    case "GLENGTH":
                        GetLength(lista_Variaveis,Açoes[i].parametros);
                        break;
                    default:
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
                    throw new InterpretationExeption("Variavel não encontrada/definida.");
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
            Variavel? var_result = GetVariavel(lista_Variaveis, parametros[0],1);
            if(var_result == null){
                throw new InterpretationExeption(1,"Variavel não encontrada/definida.");
            }
            if(var_result.type != "bool"){
                throw new InterpretationExeption(1,"Valor a ser atribuido a variavel não booleana.");
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
                    throw new InterpretationExeption(3,"Comparador invalido");
            }

            //Obtem e valida os valores a comparar
            string valor1 = GetValue(lista_Variaveis, parametros[1], 2);
            string valor2 = GetValue(lista_Variaveis, parametros[3], 4);
            

            //Faz a comparação
            if(tipo == "num"){
                if(!float.TryParse(valor1, out float a)){
                    throw new InterpretationExeption(2,"Tipo de dado invalido para comparação pedida.");
                }
                if(!float.TryParse(valor2, out float b)){
                    throw new InterpretationExeption(4,"Tipo de dado invalido para comparação pedida.");
                }
                float numval1 = a;
                float numval2 = b;
                //Faz a comparação de acordo com o comparador dado
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

        private static void JoinText(List<Variavel> lista_Variaveis, string[] parametros){
            //Valida a quantidade de parametros
            if(parametros.Length != 3){
                 throw new InterpretationExeption("Quantidade de parametros invalida.");
            }
            
            Variavel var_result = GetVariavel(lista_Variaveis,parametros[0],1);
            string valor1 = GetValue(lista_Variaveis,parametros[1],2);
            string valor2 = GetValue(lista_Variaveis,parametros[2],3);

            if(var_result.type != "text"){
                throw new InterpretationExeption(1,"Variavel não é do tipo 'text'");
            }

            var_result.value = valor1 + valor2;
        }

        private static void GetLength(List<Variavel> lista_Variaveis, string[] parametros){
            if(parametros.Length != 2){
                throw new InterpretationExeption("Quantidade de parametros invalida.");
            }

            Variavel varr = GetVariavel(lista_Variaveis,parametros[0],1);
            Variavel var1 = GetVariavel(lista_Variaveis,parametros[1],2);

            if(varr.type != "num"){
                throw new InterpretationExeption(1,"Variavel de atribuição não é do tipo numerica.");
            }
            if(var1 !is Array){
                throw new InterpretationExeption(2,"Parametro não é do tipo Array.");
            }

            varr.value = ((Array)var1).vars.Length.ToString();
        }

        private static void Operaçao(List<Variavel> lista_Variaveis, string[] parametros){
            //Valida a quantidade de parametros
            if(parametros.Length != 4){
                throw new InterpretationExeption("Quantidade de parametros invalida.");
            }
            
            //Valida o 1º parametro
            Variavel? var = GetVariavel(lista_Variaveis,parametros[0],1);
            if(var == null){
                throw new InterpretationExeption(1,"Variavel não encontrada/definida.");
            }
            if(var.type != "num"){
                throw new InterpretationExeption(1,"Valor a ser atribuido a variavel não numerica.");
            }

            //Valida 2º parametro e atribui o valor á variavel varlor1
            string valor1 = GetValue(lista_Variaveis, parametros[1],2);
            
            //Valida 4º parametro e atribui o valor á variavel varlor2
            string valor2 = GetValue(lista_Variaveis, parametros[3],4);
            
            //Verifica se os valores são numeros
            if(!float.TryParse(valor1, out float a)){
                throw new InterpretationExeption(2,"Valor do parametro não é do tipo num.");
            }
            if(!float.TryParse(valor2, out float b)){
                throw new InterpretationExeption(2,"Valor do parametro não é do tipo num.");
            }

            //Operaçao
            switch(parametros[2]){
                case "+":
                    var.value = (a + b).ToString();
                    break;
                case "-":
                    var.value = (a - b).ToString();
                    break;
                case "*":
                    var.value = (a * b).ToString();
                    break;
                case "/":
                    var.value = (a / b).ToString();
                    break;
                default:
                    throw new InterpretationExeption(3,"Operador invalido.");
            }
        }

        private static int IF(List<Flag> lista_Flags, List<Variavel> lista_Variaveis, string[] parametros){
            if(parametros.Length < 2){
                throw new InterpretationExeption("Parametros em faltas.");
            }
            //Valida a variavel 1º parametro
            if(parametros[0][0] != '$'){
                throw new InterpretationExeption(1,"Parametro não é variavel");
            }
            Variavel? var = lista_Variaveis.Find(x => x.id == parametros[0]);
            if(var == null){
                throw new InterpretationExeption(1,"Variavel não encontrada.");
            }
            if(var.type != "bool"){
                throw new InterpretationExeption(1,"Variavel de tipo invalido");
            }

            //Valida a flag 2º parametro
            Flag? flag = lista_Flags.Find(x => x.nome == parametros[1]);
            if(flag == null){
                throw new InterpretationExeption(2,"Flag não encontrada/definida");
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
            if(parametros.Length != 2){
                throw new InterpretationExeption("Quantidade de Parametros Invalida.");
            }

            //verifica se o primeiro parametro é uma variavel valida
            Variavel? var1;
            int index = 0;
            if(parametros[0][0] != '$'){
                throw new InterpretationExeption(1,"Parametro não é variavel");
            }
            if(parametros[0].Contains('#')){
                string[] l = parametros[0].Split('#');
                var1 = lista_Variaveis.Find(x => x.id == l[0]);
                if(int.TryParse(GetValue(lista_Variaveis,l[1],1),out int r)){
                    index = r;
                }
                else{
                    throw new InterpretationExeption(1,"Index especificado invalido.");
                }
            }
            else{
                var1 = lista_Variaveis.Find(x => x.id == parametros[0]);
            }

            if(var1 == null){
                throw new InterpretationExeption(1,"Variavel não encontrada.");
            }

            //obtem o o valor do 2º parametro
            string value2 = GetValue(lista_Variaveis,parametros[1],2);

            string valor = "";
            //Aplica a operaçao correspondente ao tipo de dado
            switch(var1.type){
                case "num":
                    if(float.TryParse(value2, out float result_num)){
                        valor = result_num.ToString();
                    }
                    else{
                        throw new InterpretationExeption(2,"Não foi possivel converter parametro para num.");
                    }
                    break;
                case "bool":
                    if(bool.TryParse(value2, out bool result_bool)){
                        valor = result_bool.ToString();
                    }
                    else{
                        throw new InterpretationExeption(2,"Não foi possivel converter parametro para bool.");
                    }
                    break;
                case "text":
                    valor = value2;
                    break;
                default:
                    break;
            }

            if(var1 is Array){
                if(index >= ((Array)var1).vars.Length){
                    throw new InterpretationExeption(1, "Index indicado ultrapassa os limites do Array.");
                }
                ((Array)var1).vars[index].value = valor;
            }
            else{
                var1.value = valor;
            }
        }

        private static int Goto(string[] parametros, List<Flag> lista_Flags, int index){
            //valida o parametro
            if(parametros[0] == ""){
                throw new InterpretationExeption("Parametro em falta.");
            }
            Flag? flag = lista_Flags.Find(x => x.nome == parametros[0]);
            if (flag == null)
            {
                throw new InterpretationExeption(1,"Flag não encontrada/definida.");
            }
            
            //Define i para a posiçao da flag
            index = flag.posiçao;
            return index;
        }

        private static void SetArr(List<Variavel> lista_Variaveis, string[] parametros){
            //Verifica se chegam três parametros
            if(parametros.Length != 3){
                throw new InterpretationExeption("Quantidade de Parametros Invalida.");
            }

            //Valida o 1º parametro se é um tipo valido (num, bool, text)
            string type = GetType(parametros[0], 1);

            //Valida o 2º parametro se é um nome valido
            string name = GetVarName(parametros[1], lista_Variaveis, 2);

            if(int.TryParse(parametros[2], out int length)){
                lista_Variaveis.Add(new Array(type, name, length));
                return;
            }
            Variavel? var = lista_Variaveis.Find(x => x.id == parametros[2]);
            if(var != null){
                if(var is Array){
                    if(((Array)var).type != type){
                        throw new InterpretationExeption(3,"Tipo de dados dos arrays não condizem");
                    }
                    //Cria um array com os mesmos valores que o array dado
                    lista_Variaveis.Add(new Array(type, name, ((Array)var).vars));
                    return;
                }
                else{
                    //Isto kinda podia ser aceite mas não faz sentido criar um array so com um valor
                    throw new InterpretationExeption(3,"Não é possivel atribuir unico valor de variavel a valores de array.");
                }
            }
            else{//Por fim verifica se é dada uma lista de valores.
                if(parametros[2][0] == '{' && parametros[2][parametros.Length-1] == '}'){
                    string param = parametros[2];
                    param = param.Remove('{').Remove('}');
                    
                    string[] valores = param.Split(',');

                    //Verifica se todos os dados dados são validos correspondente ao type
                    switch(type){
                        case "num":
                            foreach (string valor in valores){
                                if(int.TryParse(GetValue(lista_Variaveis, valor,3), out int a) == false){
                                    throw new InterpretationExeption(3,"Não foi possivel converter um dos valores para num");
                                }
                            }
                        break;
                        case "bool":
                            foreach (string valor in valores){
                                if(bool.TryParse(GetValue(lista_Variaveis, valor,3), out bool a) == false){
                                    throw new InterpretationExeption(3,"Não foi possivel converter um dos valores para num");
                                }
                            }
                        break;
                        case "text":
                            //Para text não é preciso validaçao
                        break;
                    }

                    //Apos a verificação define um novo array com os valores dados
                    lista_Variaveis.Add(new Array(type, name, valores));
                }
                else{
                    throw new InterpretationExeption(3,"O parametro não contem um tamanho nem uma lista de valores para o array.");
                }
            }
        }

        private static void Set(string[] parametros, List<Variavel> lista_Variaveis)
        {
            //Verifica se chegam três parametros
            if (parametros.Length != 3)
            {
                throw new InterpretationExeption("Quantidade de Parametros Invalida.");
            }

            //Valida o 1º parametro se é um tipo valido
            string type = GetType(parametros[0], 1);

            //Valida e obtem o nome do 2º parametro
            string name = GetVarName(parametros[1], lista_Variaveis, 2);

            //Aplica o valor de acordo com o tipo expecificado
            string param = GetValue(lista_Variaveis, parametros[2], 3);
            string value = "";
            switch (type)
            {
                case "num":
                    if (float.TryParse(param, out float result_num)){
                        value = result_num.ToString();
                    }
                    else{
                        throw new InterpretationExeption(3, "Não foi possivel converter parametro para num.");
                    }
                    break;
                case "bool":
                    if (bool.TryParse(param, out bool result_bool)){
                        value = result_bool.ToString();
                    }
                    else{
                        throw new InterpretationExeption(3, "Não foi possivel converter parametro para bool.");
                    }
                    break;
                case "text":
                    value = param;
                    break;
            }

            //Cria a variavel e acrecenta-a á lista de variaveis
            lista_Variaveis.Add(new Variavel(type, name, value));
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

//############################################

        private static Variavel GetVariavel(List<Variavel> lista_Variaveis, string parametro, int paramIndex){
            //Verifica se o parametro é variavel
            if(parametro[0] != '$'){
                throw new InterpretationExeption(paramIndex,"Parametro não é variavel.");
            }

            Variavel? var;

            //Verifica se é uma variavel de um array
            if(parametro.Contains('#')){
                string[] l = parametro.Split('#');
                var = lista_Variaveis.Find(x => x.id == l[0]);
                if(var == null){
                    throw new InterpretationExeption(paramIndex,"Variavel não encontrada/definida.");
                }
                if(int.TryParse(GetValue(lista_Variaveis,l[1],1),out int index)){
                    if(index >= ((Array)var).vars.Length || index < 0){
                        throw new InterpretationExeption(paramIndex, "Index indicado ultrapassa os limites do Array.");
                    }
                    
                    //Retorna a variavel do Array do index especificado
                    return ((Array)var).vars[index];
                }
                else{
                    throw new InterpretationExeption(1,"Index especificado invalido.");
                }
            }
            else{//Busca a Variavel correspondente ao parametro
                var = lista_Variaveis.Find(x => x.id == parametro);
                if(var == null){
                    throw new InterpretationExeption(paramIndex,"Variavel não encontrada/definida.");
                }
            }

            return var;
        }

        private static string GetVarName(string parametro, List<Variavel> lista_Variaveis, int paramIndex)
        {
            if (parametro == ""){
                throw new InterpretationExeption(paramIndex, "Nome de variavel vazio / não definido.");
            }
            if (parametro.Contains('#') || parametro.Contains('$')){
                throw new InterpretationExeption(paramIndex, "Nomes de variaveis não podem conter caracters especiais.('#', '$')");
            }
            if (lista_Variaveis.Find(x => x.id == "$" + parametro) != null){
                throw new InterpretationExeption(paramIndex, "Variavel ja existente.");
            }
            return parametro;
        }

        private static string GetType(string parametro, int paramIndex)
        {
            if (parametro != "num" && parametro != "bool" && parametro != "text")
            {
                throw new InterpretationExeption(paramIndex, "Parametro tipo invalido. Type must be num, bool, or text.");
            }

            return parametro;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lista_Variaveis"></param>
        /// <param name="parametro"></param>
        /// <param name="paramIndex"></param>
        /// <returns>Returns the value in 'parametro'</returns>
        private static string GetValue(List<Variavel> lista_Variaveis, string parametro, int paramIndex){
            //Verifica se o parametro é variavel
            if(parametro[0] == '$'){
                //Verifica se o parametro é referente a um valor dum array
                if(parametro.Contains('#')){
                    //separa o nome do array do index
                    string[] l = parametro.Split('#');

                    Variavel? var = lista_Variaveis.Find(x => x.id == l[0]);
                    if(var == null){
                        throw new InterpretationExeption(paramIndex, "Variavel não encontrada/definida.");
                    }
                    if(var !is Array){
                        throw new InterpretationExeption(paramIndex, "Variavel indicada não é do tipo array.");
                    }
                    if(int.TryParse(GetValue(lista_Variaveis,l[1],paramIndex), out int index)){
                        if(index >= ((Array)var).vars.Length){
                            throw new InterpretationExeption(paramIndex, "Index indicado ultrapassa os limites do Array.");
                        }
                        
                        //Retorna o valor no index especificado do Array
                        return ((Array)var).vars[index].value;
                    }
                    else{
                        throw new InterpretationExeption(paramIndex, "Index especificado invalido.");
                    }
                }
                else{//Caso  seja referente a um valor duma variavel
                    Variavel? var = lista_Variaveis.Find(x => x.id == parametro);
                    if(var == null){
                        throw new InterpretationExeption(paramIndex, "Variavel não encontrada/definida.");
                    }
                    if(var is Array){
                        throw new InterpretationExeption(paramIndex, "Não é possivel obter valor de Variavel do tipo Array sem index especificado.");
                    }
                    
                    return var.value;
                }
            }
            else{//Caso não seja variavel
                return parametro;
            }
        }

        private class Variavel{
            public string type;
            public string? id;
            public string value;
            
            public Variavel(string type, string name, string value){
                this.type = type;
                this.id = '$'+name;
                this.value = value;
            }

            public Variavel(string value, string type){
                this.value = value;
                this.type = type;
            }
        }

        private class Array : Variavel{
            public Variavel[] vars;

            public Array(string type, string name, string[] values) : base(type, name, "Array"){
                Variavel[] arr = new Variavel[values.Length];
                for(int i = 0; i < values.Length-1; i++){
                    arr[i] = new Variavel(values[i], type);
                }
                
                this.vars = arr;
            }

            public Array(string type, string name, Variavel[] vars) : base(type, name, "Array"){
                this.vars = vars;
            }
            
            public Array(string type, string name, int length) : base(type, name, "Array"){
                this.vars = new Variavel[length];
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
                    string[] a = {};
                    if(palavras.Length > 1){
                        a = palavras[1].Split('|');
                    }
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
            public InterpretationExeption(int parametro, string message) : base(MontarMensagemErro(parametro, message)) {}

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

            // public InterpretationExeption(string message, System.Exception inner) : base(MontarMensagemErro(message), inner) { }
            // protected InterpretationExeption(
            //     System.Runtime.Serialization.SerializationInfo info,
            //     System.Runtime.Serialization.StreamingContext context) : base(info, context) {

            //     }
        }
    }
    
}
