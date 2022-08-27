/*
MIT License

Copyright (c) 2022 Daniel Tomás <dani7sly12345@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace AbisInterpreter
{
    public class Interpreter
    {
        //They are defined globaly to be used in the exceptions
        //static string[] instrunçoes_do_Ficheiro = {};
        private static int currentActionIndex;
        private static Stack<Block> blockStack;
        private static List<Block> block_list;
        private static List<Struct> struct_list;

        public static void Intepretar(string file_content)
        {
            currentActionIndex = 0;
            block_list = new List<Block>();
            blockStack = new Stack<Block>();

            LexerParser(file_content);

            Block? mainBlock = block_list.Find((x) => x.name == "@main");
            if (mainBlock == null)
            {
                throw new InterpretationException(InterExecpType.NoMainBlockFound);
            }

            Intepretar(mainBlock);
        }

        private static void Intepretar(Block mainBlock)
        {
            Value[] noInputVars = { };
            mainBlock.RunBlock(noInputVars);
        }

        private static void LexerParser(string file_content)
        {

            //Removes Spaces tabs and newlines
            file_content = file_content.Replace(" ", "");
            //file_content = file_content.Replace(System.Environment.NewLine, "");
            file_content = file_content.Replace("\t", "");

            //Removes comments
            List<string> lines = file_content.Split(System.Environment.NewLine).ToList();
            file_content = "";
            foreach (string line in lines)
            {
                if (!line.StartsWith('#'))
                {
                    file_content += line;
                }
            }

            //Assumes that the blocks are build properly
            //@Abis[..:..;..:..]->..{-----}@Block[..:..;..:..]->..{-----}
            string[] blocks = file_content.Split("}");

            for (int i = 0; i < blocks.Length; i++)
            {
                if (i == blocks.Length - 1)
                {
                    break;
                }

                try
                {
                    string blockHead = blocks[i].Split("{")[0];         //@Block[..:..;..:..]->..
                    string blockInstructions = blocks[i].Split("{")[1]; //-----

                    string[] blockHeadSplit = blockHead.Split("[");
                    string blockName = "@" + blockHeadSplit[0].Split("@")[1];

                    string[] inputOutput = blockHeadSplit[1].Split("]"); //..:..;..:..   ]   ->..

                    string[] inputVars = inputOutput[0].Split(";");

                    string outputType = (inputOutput[1].StartsWith("->") ? inputOutput[1].Split("->")[1] : "");

                    //if(existsDuplicateVarName(inputVars)) throw new InterpretationException(InterExecpType.DuplicateInputBlockVariableName);

                    block_list.Add(new Block(blockName, outputType, inputVars, blockInstructions));
                }
                catch (InterpretationException e)
                {
                    throw new InterpretationException("Error Creating " + (i + 1) + "º Block. Error: " + e.Message);
                }
                catch (System.Exception e)
                {
                    throw new InterpretationException("Error Creating " + (i + 1) + "º Block. Error:" + e.Message + "\n" + e.StackTrace);
                    //throw new InterpretationException("Error Creating " + (i + 1) + "º Block.");
                }
            }

            bool existsDuplicateVarName(string[] inputVarsAndTypes)
            {
                for (int i = 0; i < inputVarsAndTypes.Length; i++)
                {
                    for (int j = 0; j < inputVarsAndTypes.Length; j++)
                    {
                        if (i == j) continue;

                        if (inputVarsAndTypes[i].Split(":")[1] == inputVarsAndTypes[j].Split(":")[1])
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        //---ACTIONS---
        //################################################################################################

        //! Temporary Solution!
        private static void Read(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            if (parameters.Length != 1)
            {
                throw new InterpretationException("Invalid param quantity.");
            }

            Variable var = GetVariavel(var_list, parameters[0], 1, false);
            // if(var is Array){
            //     throw new InterpretationExeption(1,"Impossivel ");
            // }

            if (var.type == TYPE_TEXT)
            {
                string? valor = Console.ReadLine();
                if (valor == null)
                {
                    var.value.basicValue = "";
                }
                else
                {
                    var.value.basicValue = valor;
                }
            }
            else
            {
                throw new InterpretationException(1, "Expected Variable of type Text. Got: " + var.type);
            }
        }

        //! Temporary Solution!
        private static void Print(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            // if(parametros.Length < 1){
            //     throw new InterpretationExeption("Parametro em falta");
            // }

            Value value = GetValue(var_list, parameters[0], 1);

            if (value.isStruct) throw new InterpretationException(1, "Cannot Print Composite Values.");

            Console.Write(value.basicValue.Replace('_', ' '));

        }

        //! Temporary Solution!
        private static void PrintL(Block currentBlock)
        {
            Print(currentBlock);
            Console.Write('\n');
        }

        /*//! DEPRECATE
        private static void Comparaçao(List<Variable> lista_Variaveis, string[] parametros)
        {
            //Valida a quantidade de parametros
            if (parametros.Length != 4)
            {
                throw new InterpretationException("Invalid param quantity.");
            }

            //Valida o 1º parametro, tem de ser variavel.
            Variable? var_result = GetVariavel(lista_Variaveis, parametros[0], 1, false);
            // if(var_result is Array){
            //     throw new InterpretationExeption(1,"Não pode ser atribuido um valor a um array sem index expecificado.");
            // }
            if (var_result.type != "bool")
            {
                throw new InterpretationException(1, "Variable is not of type bool.");
            }
            string comparator = GetValue(lista_Variaveis, parametros[2], 3);
            string tipo;
            //Valida 3º parametro comparador
            switch (comparator)
            {
                case "<":
                case ">":
                case "<=":
                case ">=":
                    tipo = "num";
                    break;
                case "==":
                case "!=":
                    tipo = "text";
                    break;
                default:
                    throw new InterpretationException(3, "Invalid Comparator.");
            }

            //Obtem e valida os valores a comparar
            string valor1 = GetValue(lista_Variaveis, parametros[1], 2);
            string valor2 = GetValue(lista_Variaveis, parametros[3], 4);


            //Faz a comparação
            if (tipo == "num")
            {
                if (!float.TryParse(valor1, out float a))
                {
                    throw new InterpretationException(2, "Invalid data type for requested comparison.");
                }
                if (!float.TryParse(valor2, out float b))
                {
                    throw new InterpretationException(4, "Invalid data type for requested comparison.");
                }
                float numval1 = a;
                float numval2 = b;
                //Faz a comparação de acordo com o comparador dado
                switch (comparator)
                {
                    case "<":
                        if (numval1 < numval2)
                        {
                            var_result.value = "True";
                        }
                        else
                        {
                            var_result.value = "False";
                        }
                        break;
                    case ">":
                        if (numval1 > numval2)
                        {
                            var_result.value = "True";
                        }
                        else
                        {
                            var_result.value = "False";
                        }
                        break;
                    case "<=":
                        if (numval1 <= numval2)
                        {
                            var_result.value = "True";
                        }
                        else
                        {
                            var_result.value = "False";
                        }
                        break;
                    case ">=":
                        if (numval1 >= numval2)
                        {
                            var_result.value = "True";
                        }
                        else
                        {
                            var_result.value = "False";
                        }
                        break;
                    default:
                        throw new InterpretationException("A place where u should not be.");
                }
            }
            else
            {
                switch (comparator)
                {
                    case "==":
                        if (valor1 == valor2)
                        {
                            var_result.value = "True";
                        }
                        else
                        {
                            var_result.value = "False";
                        }
                        break;
                    case "!=":
                        if (valor1 != valor2)
                        {
                            var_result.value = "True";
                        }
                        else
                        {
                            var_result.value = "False";
                        }
                        break;
                }
            }
        }
        */
        private static void JoinText(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            //Valida a quantidade de parametros
            if (parameters.Length != 3)
            {
                throw new InterpretationException(0, "Invalid param Quantity.");
            }

            Variable var_result = GetVariavel(var_list, parameters[0], 1, false);
            Value valor1 = GetValue(var_list, parameters[1], 2);
            Value valor2 = GetValue(var_list, parameters[2], 3);

            if (var_result.type != TYPE_TEXT) throw new InterpretationException(1, "Variable is not of type text.");

            var_result.value.basicValue = valor1.basicValue + valor2.basicValue;
        }
        /*
        private static void SplitText(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            if (parameters.Length != 3)
            {
                throw new InterpretationException("Invalid parameter count. Expected 3, got: " + parameters.Length);
            }

            Array array = (Array)GetVariavel(var_list, parameters[0], 1, true);
            string textToSplit = GetValue(var_list, parameters[1], 2);
            string splitText = GetValue(var_list, parameters[2], 3);

            string[] values = textToSplit.Split(splitText);

            array.vars = strArrToVarArr(values);

            Variable[] strArrToVarArr(string[] values)
            {
                Variable[] arr = new Variable[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    arr[i] = new Variable(values[i], "text");
                }
                return arr;
            }
        }
        */
        private static void Parse(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            if (parameters.Length != 3)
            {
                throw new InterpretationException("Invalid param Quantity.");
            }

            //Recebe os parametros
            Variable var1 = GetVariavel(var_list, parameters[0], 1, false);
            Variable var2 = GetVariavel(var_list, parameters[1], 2, false);
            Value val = GetValue(var_list, parameters[2], 3);

            //Valida as variaveis
            if (var1.type != TYPE_BOOL) throw new InterpretationException(1, "Variable is not of type bool.");
            if (var2.type != TYPE_NUM) throw new InterpretationException(2, "Variable is not of type num.");
            if (val.type != TYPE_TEXT) throw new InterpretationException(2, "Value is not of type text.");

            //Realiza o parse
            bool parsed = float.TryParse(val.basicValue, out float num);
            var1.value.basicValue = parsed.ToString();
            if (parsed)
            {
                var2.value.basicValue = num.ToString();
            }
        }
        /*
        private static void GetLength(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            if (parameters.Length != 2)
            {
                throw new InterpretationException("Invalid Param Quantity.");
            }

            //Busca a variavel e o array
            Variable varr = GetVariavel(var_list, parameters[0], 1, false);
            Variable var1 = GetVariavel(var_list, parameters[1], 2, true);

            if (varr.type != "num")
            {
                throw new InterpretationException(1, "Varible is not of type num.");
            }
            if (var1! is Array)
            {
                throw new InterpretationException(2, "Variable is not of type Array.");
            }

            varr.value = ((Array)var1).vars.Length.ToString();
        }
        */
        //! DEPRECATE
        /*private static void Operaçao(List<Variable> lista_Variaveis, string[] parametros)
        {
            //Valida a quantidade de parametros
            if (parametros.Length != 4)
            {
                throw new InterpretationException("Invalid Param Quantity.");
            }

            //Valida o 1º parametro
            Variable? var = GetVariavel(lista_Variaveis, parametros[0], 1, false);
            // if(var is Array){
            //     throw new InterpretationExeption(1,"Não pode ser atribuido um valor a um array sem index expecificado.");
            // }
            if (var.type != "num")
            {
                throw new InterpretationException(1, "Variable is not of type num.");
            }

            //Valida 2º parametro e atribui o valor á variavel varlor1
            string valor1 = GetValue(lista_Variaveis, parametros[1], 2);

            //Recebe o operador do 3º parametro
            string operador = GetValue(lista_Variaveis, parametros[2], 3);

            //Valida 4º parametro e atribui o valor á variavel varlor2
            string valor2 = GetValue(lista_Variaveis, parametros[3], 4);

            //Verifica se os valores são numeros
            if (!float.TryParse(valor1, out float a))
            {
                throw new InterpretationException(2, "Valor do parametro não é do tipo num.");
            }
            if (!float.TryParse(valor2, out float b))
            {
                throw new InterpretationException(2, "Valor do parametro não é do tipo num.");
            }

            //Operaçao
            switch (operador)
            {
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
                case "%":
                    var.value = (a % b).ToString();
                    break;
                default:
                    throw new InterpretationException(3, "Operador invalido.");
            }
        }
        */

        private static void IF(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));
            List<Flag> flag_list = (currentBlock.flag_list != null ? currentBlock.flag_list : throw new InterpretationException(InterExecpType.NullBlockFlagList));

            if (parameters.Length != 2)
            {
                throw new InterpretationException(0, "Expected 3 parameters.");
            }

            //Recebe e Valida a variavel do 1º parametro
            Value value = GetValue(var_list, parameters[0], 1);
            if (value.type != TYPE_BOOL)
            {
                throw new InterpretationException(1, "Expected Bool value. Got: " + value.type);
            }
            // if(var is Array){
            //     throw new InterpretationExeption(1,"Não é possivel ler valor de um array sem index especificado");
            // }

            //Valida a flag 2º parametro
            Flag? flag = flag_list.Find(x => x.nome == parameters[1]);
            if (flag == null)
            {
                throw new InterpretationException(2, "Flag not found.");
            }

            //changes the current index for the index of the flag, the -1 is because of the loop where the actions are executed.
            if (value.basicValue == "True")
            {
                currentBlock.nextAction = flag.posiçao - 1;
            }
        }

        private static void IFN(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));
            List<Flag> flag_list = (currentBlock.flag_list != null ? currentBlock.flag_list : throw new InterpretationException(InterExecpType.NullBlockFlagList));


            if (parameters.Length != 2)
            {
                throw new InterpretationException("Expected 3 parameters.");
            }

            //Recebe e Valida a variavel do 1º parametro
            Value value = GetValue(var_list, parameters[0], 1);
            if (value.type != TYPE_BOOL)
            {
                throw new InterpretationException(1, "Expected Bool value. Got: " + value.type);
            }

            //Valida a flag 2º parametro
            Flag? flag = flag_list.Find(x => x.nome == parameters[1]);
            if (flag == null)
            {
                throw new InterpretationException(2, "Flag not found.");
            }

            //changes the current index for the index of the flag, the -1 is because of the loop where the actions are executed.
            if (value.basicValue == "False")
            {
                currentBlock.nextAction = flag.posiçao - 1;
            }
        }

        private static void Equalss(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList, 0));

            //verifica se chegou dois parametros
            if (parameters.Length != 2)
            {
                throw new InterpretationException(InterExecpType.InvalidParameterQuantity, 0);
            }

            //verifica se o primeiro parametro é uma variavel valida
            Variable? var1 = GetVariavel(var_list, parameters[0], 1, false);

            //obtem o valor do 2º parametro
            Value value = GetValue(var_list, parameters[1], 2);

            //Aplica a atribuição correspondente ao tipo de dado da var1
            if (var1.type != value.type) throw new InterpretationException(InterExecpType.TypesDontMatch, 2);

            var1.value = value;
        }

        private static void Goto(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Flag> flag_list = (currentBlock.flag_list != null ? currentBlock.flag_list : throw new InterpretationException(InterExecpType.NullBlockFlagList));

            //valida o parametro
            if (parameters[0] == "")
            {
                throw new InterpretationException(InterExecpType.InvalidParameterQuantity);
            }
            Flag? flag = flag_list.Find(x => x.nome == parameters[0]);
            if (flag == null)
            {
                throw new InterpretationException(1, "Flag not found.");
            }

            currentActionIndex = flag.posiçao - 1;
        }
        /*
        private static void SetArr(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            //Verifica se chegam três parametros
            if (parameters.Length != 3)
            {
                throw new InterpretationException("Invalid Param Quantity.");
            }

            //Valida o 1º parametro se é um tipo valido (num, bool, text)
            string type = GetType(parameters[0], 1);

            //Valida o 2º parametro se é um nome valido
            string name = GetVarName(parameters[1], var_list, 2);

            //Caso seja dado um tamanho é criado um array com o tamanho especificado
            if (int.TryParse(parameters[2], out int length))
            {
                var_list.Add(new Array(type, name, length));
                return;
            }

            Variable? var = var_list.Find(x => x.id == parameters[2]);
            if (var != null)
            {
                if (var is Array)
                {
                    if (((Array)var).type != type)
                    {
                        throw new InterpretationException(3, "Arrays types do not match.");
                    }
                    //Cria um array com os mesmos valores que o array dado
                    var_list.Add(new Array(type, name, ((Array)var).vars));
                    return;
                }
                else
                {
                    if (var.type == "num")
                    {
                        var_list.Add(new Array(type, name, int.Parse(var.value)));
                    }
                    throw new InterpretationException(3, "Given variable is not of type num.");
                }
            }
            else
            {//Por fim verifica se é dada uma lista de valores.
                if (parameters[2][0] == '{' && parameters[2][parameters[2].Length - 1] == '}')
                {
                    string param = parameters[2];
                    param = param.Replace("{", String.Empty).Replace("}", String.Empty);

                    string[] valores = param.Split(',');

                    //Verifica se todos os dados dados são validos correspondente ao type
                    switch (type)
                    {
                        case "num":
                            foreach (string valor in valores)
                            {
                                if (int.TryParse(GetValue(var_list, valor, 3), out int a) == false)
                                {
                                    throw new InterpretationException(3, "Could not convert one of the values to a num");
                                }
                            }
                            break;
                        case "bool":
                            foreach (string valor in valores)
                            {
                                if (bool.TryParse(GetValue(var_list, valor, 3), out bool a) == false)
                                {
                                    throw new InterpretationException(3, "Could not convert one of the values to a bool");
                                }
                            }
                            break;
                        case "text":
                            //Para text não é preciso validaçao
                            break;
                    }

                    //Apos a verificação define um novo array com os valores dados
                    var_list.Add(new Array(type, name, valores));
                }
                else
                {
                    throw new InterpretationException(3, "The parameter does not contain a size or a list of values for the array.");
                }
            }
        }
        */
        private static void Set(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            //Verifica se chegam três parametros
            if (parameters.Length != 2)
            {
                throw new InterpretationException("Invalid param quantity.");
            }

            //Valida o 1º parametro se é um tipo valido
            string type = GetType(parameters[0], 1);

            //Valida e obtem o nome do 2º parametro
            string name = GetVarName(parameters[1], var_list, 2);

            //Creates the varable and adds it to the varable list.
            var_list.Add(new Variable(type, name));
        }

        //! DEPRECATE
        private static void Flags(string[] parametros, List<Flag> lista_Flags, int i)
        {
            //Valida o parametro nome
            if (parametros == null || parametros.Length != 1 || parametros[0] == "")
            {
                throw new InterpretationException("Invalid 'name' parameter.");
            }
            if (lista_Flags.Find(x => x.nome == parametros[0]) != null)
            {
                throw new InterpretationException("Nome de Flag repetido/já existente");
            }

            //Acrescenta a flag á lista de flags
            lista_Flags.Add(new Flag(parametros[0], i));
        }

        private static void Execute(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList,0));

            if (parameters.Length != 1) throw new InterpretationException(0, "Invalid param quantity.");
            string param = parameters[0];
            if (param[0] == '@')
            {
                if (!param.Contains('[') && param[param.Length - 1] != ']')
                    throw new InterpretationException(InterExecpType.InvalidBlockCallSyntax, 1);

                Block? blockToBeRun = block_list.Find((x) => x.name == param.Split("[")[0]);
                if (blockToBeRun == null)
                    throw new InterpretationException(InterExecpType.BlockNotFound, 1);

                if(blockToBeRun.name == "@main") throw new InterpretationException(1, "Block main cannot be called.");

                //if (blockToBeRun.outputType == null) throw new InterpretationException(InterExecpType.GettingValueFromBlockWithoutReturnValue, 1);

                //TODO: maybe implement BlockCall support in blockCalls
                string[] inputParams = param.Split('[')[1].Split(']')[0].Split(",");

                if(inputParams.Length == 1 && inputParams[0] == "")
                    inputParams = new string[0];

                Value[] inputValues = new Value[inputParams.Length];

                for (int i = 0; i < inputParams.Length; i++)
                {
                    Value value = GetValue(var_list, inputParams[i], 1);
                    inputValues[i] = (blockToBeRun.inputVarsAndTypes[i].Item1 == value.type ? value : throw new InterpretationException(InterExecpType.TypesDontMatch, 1));
                }

                Value? rtrn = blockToBeRun.RunBlock(inputValues);
            }
            else throw new InterpretationException(1, "parameter is not a block call.");
        }

        private static void Return(Block currentBlock)
        {
            string[] parameters = currentBlock.actions[currentActionIndex].parameters;
            List<Variable> var_list = (currentBlock.var_list != null ? currentBlock.var_list : throw new InterpretationException(InterExecpType.NullBlockVarList));

            if (parameters.Length != 1)
            {
                throw new InterpretationException("Invalid param quantity.");
            }

            currentBlock.outputValue = GetValue(var_list, parameters[0], 1);
        }

        //#########################################################################################################

        const string TYPE_TEXT = "text";
        const string TYPE_NUM = "num";
        const string TYPE_BOOL = "bool";

        /// <summary>
        ///     Verifica se existe. Verifica se é de um array e valida se o index é valido caso dado.
        /// </summary>
        /// <param name="lista_Variaveis"></param>
        /// <param name="parametro"></param>
        /// <param name="paramIndex"></param>
        /// <returns>Retorna a variavel pedida(normal ou de array). Não retorna NULL</returns>
        private static Variable GetVariavel(List<Variable> lista_Variaveis, string parametro, int paramIndex, bool isArray)
        {
            //Verifica se o parametro é variavel
            if (parametro[0] != '$')
            {
                throw new InterpretationException(InterExecpType.ExpectedVariable, paramIndex);
            }

            Variable? var;

            //Verifica se é uma variavel de um array
            if (parametro.Contains('#'))
            {
                //TODO FIX THIS
                throw new InterpretationException(InterExecpType.TODO);
                // string[] l = parametro.Split('#');
                // var = lista_Variaveis.Find(x => x.id == l[0]);
                // if (var == null)
                // {
                //     throw new InterpretationException(paramIndex, "Variable not found.");
                // }
                // if (!(var is Array))
                // {
                //     throw new InterpretationException(paramIndex, "Given variable is not array type");
                // }

                // if (int.TryParse(GetValue(lista_Variaveis, l[1], 1), out int index))
                // {
                //     if (index >= ((Array)var).vars.Length || index < 0)
                //     {
                //         throw new InterpretationException(paramIndex, "Indicated index exceeds the limits of the Array.");
                //     }

                //     //Verifica se a variavel do Array é um array(não sei se vou manter isto ja que não da para obter o valor de um dos index do array interno)
                //     if (((Array)var).vars[index]! is Array && isArray)
                //     {
                //         throw new InterpretationException(paramIndex, "The received variable is not an Array.");
                //     }
                //     if (((Array)var).vars[index] is Array && !isArray)
                //     {
                //         throw new InterpretationException(paramIndex, "Received Array expected normal variable");
                //     }

                //     //Retorna a variavel do Array no index especificado
                //     return ((Array)var).vars[index];
                // }
                // else
                // {
                //     throw new InterpretationException(paramIndex, "Invalid specified index.");
                // }
            }
            else
            {//Busca a Variavel correspondente ao parametro
                var = lista_Variaveis.Find(x => x.id == parametro);
                if (var == null)
                {
                    throw new InterpretationException(InterExecpType.VariableNotFound, paramIndex);
                }

                // if (!(var is Array) && isArray)
                // {
                //     throw new InterpretationException(paramIndex, "received variable is not an Array.");
                // }
                // if (var is Array && !isArray)
                // {
                //     throw new InterpretationException(paramIndex, "Received Array expected normal value.");
                // }

            }

            return var;
        }

        private static string GetVarName(string parametro, List<Variable> lista_Variaveis, int paramIndex)
        {
            if (parametro == "")
            {
                throw new InterpretationException(paramIndex, "Empty / undefined variable name.");
            }
            if (parametro.Contains('#') || parametro.Contains('$'))
            {
                throw new InterpretationException(paramIndex, "Variable names cannot contain special characters.('#', '$')");
            }
            if (lista_Variaveis.Find(x => x.id == "$" + parametro) != null)
            {
                throw new InterpretationException(paramIndex, "Existing variable.");
            }
            return parametro;
        }

        private static string GetType(string parametro, int paramIndex)
        {
            //TODO: Add Structs name check
            if (parametro != "num" && parametro != "bool" && parametro != "text")
            {
                throw new InterpretationException(paramIndex, "Invalid parameter type. Type must be num, bool, or text.");
            }

            return parametro;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="var_list"></param>
        /// <param name="param"></param>
        /// <param name="paramIndex"></param>
        /// <returns>Returns the value in 'parametro'</returns>
        private static Value GetValue(List<Variable> var_list, string param, int paramIndex)
        {
            if (param.Length == 0)
            {
                return new Value(TYPE_TEXT); //Represents a Value with empty text
            }

            //Checks if the param is a variable
            if (param[0] == '$')
            {
                //Checks if the value is of an idex of an array
                if (param.Contains('#'))
                {
                    //TODO: FIX THIS
                    throw new InterpretationException(InterExecpType.TODO);
                    /*
                    //separates the name of the array and the index
                    string[] l = param.Split('#');

                    Variable? var = var_list.Find(x => x.id == l[0]);
                    if (var == null)
                    {
                        throw new InterpretationException(paramIndex, "Variable not found.");
                    }
                    if (!(var is Array))
                    {
                        throw new InterpretationException(paramIndex, "Variable is not of an array.");
                    }

                    //Vai buscando as variaveis/arrays de acordo com os '#'
                    for (int i = 1; i < l.Length; i++)
                    {
                        if (int.TryParse(GetValue(var_list, l[i], paramIndex).normalvalue, out int index))
                        {

                            //simulates index overflow
                            while (index >= ((Array)var).vars.Length)
                            {
                                index -= ((Array)var).vars.Length;
                            }

                            //Variable contained in specified index($arr#i)
                            var = ((Array)var).vars[index];

                            //O ultimo é o valor a ser returnado
                            if (i == l.Length - 1)
                            {
                                if (!(var is Array))
                                {
                                    break;
                                }
                                else
                                {
                                    throw new InterpretationException(paramIndex, "Expecting a value, got an array.");
                                }
                            }
                        }
                        else
                        {
                            throw new InterpretationException(paramIndex, "Specified index is not a num.");
                        }
                    }

                    if (var is Array)
                    {
                        throw new InterpretationException(paramIndex, "Expecting a value, got an array.");
                    }

                    return var.value;
                    */
                }
                else
                {//Caso  seja referente a um valor duma variavel normal
                    Variable? var = var_list.Find(x => x.id == param);
                    if (var == null) throw new InterpretationException(paramIndex, "Variable not found.");

                    return var.value;
                }
            }
            else if (param[0] == '@')
            {
                if (!param.Contains('[') && param[param.Length - 1] != ']')
                {
                    throw new InterpretationException(InterExecpType.InvalidBlockCallSyntax, paramIndex);
                }

                Block? blockToBeRun = block_list.Find((x) => x.name == param.Split("[")[0]);
                if (blockToBeRun == null)
                {
                    throw new InterpretationException(InterExecpType.BlockNotFound, paramIndex);
                }

                if (blockToBeRun.outputType == null) throw new InterpretationException(InterExecpType.GettingValueFromBlockWithoutReturnValue, paramIndex);

                //TODO: maybe implement BlockCall support in blockCalls
                string[] inputParams = param.Split('[')[1].Split(']')[0].Split(",");

                if(inputParams.Length == 1 && inputParams[0] == "")
                    inputParams = new string[0];
                
                Value[] inputValues = new Value[inputParams.Length];

                for (int i = 0; i < inputParams.Length; i++)
                {
                    Value value = GetValue(var_list, inputParams[i], paramIndex);
                    inputValues[i] = (blockToBeRun.inputVarsAndTypes[i].Item1 == value.type ? value : throw new InterpretationException(InterExecpType.TypesDontMatch, paramIndex));
                }

                Value? rtrn = blockToBeRun.RunBlock(inputValues);

                if (rtrn != null) return rtrn;

                throw new InterpretationException(paramIndex, "This should not had happen. Block returned null in GetValue method.");
            }
            else if (param[0] == '(')
            {//caso seja operação
                //Removes the first '(' and the last ')'
                param = param.Remove(0, 1);
                param = param.Remove(param.Length - 1, 1);

                //Finds the middle operator
                const string opr = "+-*/%<>=!?&\"";
                int splitChar = FindMiddleOperator(param, opr);

                //Gets the value on each side
                string strValue1 = param.Substring(0, splitChar);
                string strValue2 = param.Substring(splitChar + 1);


                //Gets the operator
                string operatorr = param[splitChar].ToString();

                Value result = new Value(TYPE_TEXT);

                //Checks what kind of operation are we doing
                const string oprNum = "+-/*%";
                const string oprComp = "<=>=!";
                const string oprTenary = "?";
                const string oprLogic = "&\"";
                if (oprComp.Contains(operatorr))
                {
                    Value value1 = GetValue(var_list, strValue1, paramIndex);
                    Value value2 = GetValue(var_list, strValue2, paramIndex);
                    string type = "";

                    switch (operatorr)
                    {
                        case "<":
                        case ">":
                        case "<=":
                        case ">=":
                            type = TYPE_NUM;
                            break;
                        case "=":
                        case "!":
                            type = TYPE_TEXT;
                            break;
                        default:
                            throw new InterpretationException(paramIndex, "Invalid comparator.");
                    }

                    if (type == TYPE_NUM)
                    {
                        if (value1.type != type) throw new InterpretationException(paramIndex, "Invalid data type for request comparation '" + operatorr + "', got value of type: " + value1.type);
                        if (value2.type != type) throw new InterpretationException(paramIndex, "Invalid data type for request comparation '" + operatorr + "', got value of type: " + value2.type);

                        float numval1 = (float.TryParse(value1.basicValue, out float a) ? a : throw new InterpretationException(InterExecpType.GotNullBasicValue, paramIndex));
                        float numval2 = (float.TryParse(value2.basicValue, out float b) ? b : throw new InterpretationException(InterExecpType.GotNullBasicValue, paramIndex));

                        result.type = TYPE_BOOL;

                        switch (operatorr)
                        {
                            case "<":
                                if (numval1 < numval2)
                                {
                                    result.basicValue = "True";
                                }
                                else
                                {
                                    result.basicValue = "False";
                                }
                                break;
                            case ">":
                                if (numval1 > numval2)
                                {
                                    result.basicValue = "True";
                                }
                                else
                                {
                                    result.basicValue = "False";
                                }
                                break;

                            //TODO: Currently not Working maybe fix this. 
                            //TODO: The problem is that it only accepts 1 char operator
                            // case "<=":
                            //     if(numval1 <= numval2){
                            //         result = "True";
                            //     }
                            //     else{
                            //         result = "False";
                            //     }
                            //     break;
                            // case ">=":
                            //     if(numval1 >= numval2){
                            //         result = "True";
                            //     }
                            //     else{
                            //         result = "False";
                            //     }
                            //     break;
                            default:
                                throw new InterpretationException("A place where u should not be.");
                        }
                    }
                    else
                    {//"text"
                        result.type = TYPE_BOOL;
                        switch (operatorr)
                        {
                            case "=":
                                if (value1.basicValue == value2.basicValue)
                                {
                                    result.basicValue = "True";
                                }
                                else
                                {
                                    result.basicValue = "False";
                                }
                                break;
                            case "!":
                                if (value1.basicValue != value2.basicValue)
                                {
                                    result.basicValue = "True";
                                }
                                else
                                {
                                    result.basicValue = "False";
                                }
                                break;
                        }
                    }
                }
                else if (oprLogic.Contains(operatorr))
                {
                    Value value1 = GetValue(var_list, strValue1, paramIndex);
                    Value value2 = GetValue(var_list, strValue2, paramIndex);

                    if (value1.type != TYPE_BOOL) throw new InterpretationException(paramIndex, "Invalid data type for request comparation '" + operatorr + "', got value of type: " + value1.type);
                    if (value2.type != TYPE_BOOL) throw new InterpretationException(paramIndex, "Invalid data type for request comparation '" + operatorr + "', got value of type: " + value2.type);

                    bool boolval1 = (bool.TryParse(value1.basicValue, out bool a) ? a : throw new InterpretationException(InterExecpType.GotNullBasicValue, paramIndex));
                    bool boolval2 = (bool.TryParse(value2.basicValue, out bool b) ? b : throw new InterpretationException(InterExecpType.GotNullBasicValue, paramIndex));

                    result.type = TYPE_BOOL;
                    switch (operatorr)
                    {
                        case "&":
                            result.basicValue = (boolval1 && boolval2 ? true : false).ToString();
                            break;
                        case "\"":
                            result.basicValue = (boolval1 || boolval2 ? true : false).ToString();
                            break;
                    }
                }
                else if (oprNum.Contains(operatorr))
                {
                    Value value1 = GetValue(var_list, strValue1, paramIndex);
                    Value value2 = GetValue(var_list, strValue2, paramIndex);

                    if (value1.type != TYPE_NUM) throw new InterpretationException(paramIndex, "Invalid data type for request operation '" + operatorr + "', got value of type: " + value1.type);
                    if (value2.type != TYPE_NUM) throw new InterpretationException(paramIndex, "Invalid data type for request operation '" + operatorr + "', got value of type: " + value2.type);

                    float num1 = (float.TryParse(value1.basicValue, out float x) ? x : throw new InterpretationException(InterExecpType.GotNullBasicValue,paramIndex));
                    float num2 = (float.TryParse(value2.basicValue, out float y) ? y : throw new InterpretationException(InterExecpType.GotNullBasicValue, paramIndex));

                    result.type = TYPE_NUM;
                    //Operation
                    switch (operatorr)
                    {
                        case "+":
                            result.basicValue = (num1 + num2).ToString();
                            break;
                        case "-":
                            result.basicValue = (num1 - num2).ToString();
                            break;
                        case "*":
                            result.basicValue = (num1 * num2).ToString();
                            break;
                        case "/":
                            result.basicValue = (num1 / num2).ToString();
                            break;
                        case "%":
                            result.basicValue = (num1 % num2).ToString();
                            break;
                        default:
                            throw new InterpretationException(paramIndex, "Invalid Operator.");
                    }
                }
                else if (oprTenary.Contains(operatorr))
                {
                    //((1) ? (2) ? (3))
                    //value1 = (1)
                    //valeu2 = (2) ? (3)

                    splitChar = FindMiddleOperator(strValue2, "?");
                    string strValue3 = strValue2.Substring(splitChar + 1);
                    strValue2 = strValue2.Substring(0, splitChar);

                    Value value1 = GetValue(var_list, strValue1, paramIndex);
                    Value value2 = GetValue(var_list, strValue2, paramIndex);
                    Value value3 = GetValue(var_list, strValue3, paramIndex);

                    if (value1.type != TYPE_BOOL) throw new InterpretationException(paramIndex, "Invalid data type for request comparation '" + operatorr + "', got value of type: " + value1.type);
                    bool cmp = (bool.TryParse(value1.basicValue, out bool a) ? a : throw new InterpretationException(InterExecpType.GotNullBasicValue, paramIndex));

                    result = cmp ? value2 : value3;
                }
                else throw new InterpretationException(InterExecpType.InvalidOperator, paramIndex);

                return result;

                int FindMiddleOperator(string param, string opr)
                {
                    int splitChar = 0;
                    for (int i = 0, j = 0; i < param.Length; i++)
                    {
                        if (param[i] == '(')
                        {
                            j++;
                        }
                        else if (opr.Contains(param[i]) && j == 0)
                        {
                            splitChar = i;
                            break;
                        }
                        else if (param[i] == ')')
                        {
                            j--;
                        }
                    }

                    return splitChar;
                }
            }
            else
            {
                Value val;
                if (float.TryParse(param, out float f))
                {
                    val = new Value(TYPE_NUM);
                    val.basicValue = f.ToString();
                }
                else if (bool.TryParse(param, out bool b))
                {
                    val = new Value(TYPE_BOOL);
                    val.basicValue = b.ToString();
                }
                else
                {
                    val = new Value(TYPE_TEXT);
                    val.basicValue = param;
                }
                return val;
            }
        }

        //#########################################################################################################

        private class Value
        {
            public bool isStruct;
            public string type;
            public string? basicValue;
            public Dictionary<string, Value>? valuePairs;

            public Value(string type)
            {
                this.type = type;

                switch (type)
                {
                    case TYPE_TEXT: this.isStruct = false; this.basicValue = ""; break;
                    case TYPE_BOOL: this.isStruct = false; this.basicValue = "false"; break;
                    case TYPE_NUM: this.isStruct = false; this.basicValue = "0"; break;

                    default:
                        this.isStruct = true;

                        Struct? structt = struct_list.Find(x => x.name == type);
                        if (structt is null) throw new InterpretationException(InterExecpType.StructNotFound);

                        this.valuePairs = new Dictionary<string, Value>();

                        foreach (var field in structt.fields)
                        {
                            this.valuePairs.Add(field.Item2, new Value(field.Item1));
                        }
                        break;
                }
            }
        }

        private class Variable
        {
            public Value value;
            public string id;

            public string type { get => value.type; }

            public Variable(string validType, string validName)
            {
                this.value = new Value(validType);
                this.id = '$' + validName;
            }

            public Variable(Value value, string validName)
            {
                this.value = value;
                this.id = '$' + validName;
            }
        }

        private class Struct
        {
            public string name;
            public Tuple<string, string>[] fields;//(type, name)
        }


        //TODO: ARRAYS WILL BE BROKEN AFTER IMPLEMENTING STRUCTS
        // private class Array : Variable
        // {
        //     public Variable[] vars;

        //     public Array(string type, string name, string[] values) : base(type, name, "Array")
        //     {
        //         Variable[] arr = new Variable[values.Length];
        //         for (int i = 0; i < values.Length; i++)
        //         {
        //             arr[i] = new Variable(values[i], type);
        //         }

        //         this.vars = arr;
        //     }

        //     public Array(string type, string name, Variable[] vars) : base(type, name, "Array")
        //     {
        //         this.vars = vars;
        //     }

        //     public Array(string type, string name, int length) : base(type, name, "Array")
        //     {
        //         this.vars = new Variable[length];

        //         string value = "";

        //         if (type == "num") value = "0";
        //         else if (type == "bool") value = "False";

        //         for (int i = 0; i < this.vars.Length; i++)
        //         {
        //             vars[i] = new Variable(type, "", value);
        //         }
        //     }
        // }

        //Map of the actions
        private static Dictionary<string, Action<Block>> actionMap = new Dictionary<string, Action<Block>>{
            {"SET", Set},
            {"EQL", Equalss},
            {"EXE", Execute},
            //TODO: REACTIVATE THIS AFTER FIXING ARRAYS
            //{"SETARR", SetArr},
            //{"GLEN", GetLength},
            //{"SPLITTEXT", SplitText},
            {"PRS", Parse},
            {"JTXT", JoinText},
            {"PRINT", Print},
            {"PRINTL", PrintL},
            {"READ", Read},
            {"IF", IF},
            {"IFN", IFN},
            {"GOTO", Goto},
            {"RETURN", Return},
        };

        private class Block
        {
            public string name;
            public string? outputType;
            public Value? outputValue;
            public Tuple<string, string>[] inputVarsAndTypes; //[(type, name)]
            public int nextAction; //Used in IF, IFN & GOTO

            public List<Action> actions;
            public List<Flag> flag_list;

            public List<Variable>? var_list;

            public Block(string name, string outputType, string[] inputVarsAndTypes, string instructions)
            {
                this.name = name;
                this.outputType = (outputType == "" ? null : outputType);
                this.inputVarsAndTypes = ParseInputVarsAndTypes(inputVarsAndTypes);

                //this.var_list = new List<Variavel>();

                this.flag_list = new List<Flag>();
                this.actions = new List<Action>();

                ParseIntructionsToBlockActions(this.flag_list, this.actions, instructions);
            }

            private Tuple<string, string>[] ParseInputVarsAndTypes(string[] inputVarsAndTypes)
            {
                if (inputVarsAndTypes.Length == 1 && inputVarsAndTypes[0] == "") return new Tuple<string, string>[0];
                //Format ["type:name"] -> [(type, name)]
                Tuple<string, string>[] inputFields = new Tuple<string, string>[inputVarsAndTypes.Length];
                for (int i = 0; i < inputFields.Length; i++)
                {
                    string[] a = inputVarsAndTypes[i].Split(":");
                    inputFields[i] = new Tuple<string, string>(a[0], a[1]);
                }

                return inputFields;
            }

            /// <summary>
            ///     Runs the block instructions. A Block must always return a value.
            /// </summary>
            /// <returns>Returns value acording with output type.</returns>
            public Value? RunBlock(Value[] inputValues)
            {
                this.var_list = new List<Variable>();
                blockStack.Push(this);
                currentActionIndex = -1;

                //Create the input variables
                if (inputValues.Length > 0)
                {
                    for (int i = 0; i < inputVarsAndTypes.Length; i++)
                    {
                        string varName = inputVarsAndTypes[i].Item2;

                        //input Values Should be validated in GetValue Method.

                        this.var_list.Add(new Variable(inputValues[i], varName));
                    }
                }

                //Runs the intructions in the block
                for (int i = 0; i < actions.Count; i++)
                {
                    currentActionIndex = i;
                    nextAction = i;

                    string action = actions[i].actionType;
                    if (actionMap.ContainsKey(action))
                    {
                        actionMap[action](this);

                        if (action == "RETURN")
                            break;
                    }
                    else throw new InterpretationException(InterExecpType.ActionNotFound);

                    i = nextAction;
                }


                // else if (this.outputType != "")
                // {
                //     throw new InterpretationException("Block finished without return statement.");
                // }

                this.var_list.Clear();
                this.var_list = null;
                blockStack.Pop();

                return outputValue;
            }

            private static void ParseIntructionsToBlockActions(List<Flag> lista_Flags, List<Action> Açoes, string instructions)
            {
                string[] actions = instructions.Split(";");
                for (int i = 0, j = 0; i < actions.Length; i++)
                {
                    string[] a = actions[i].Split(':');
                    switch (a.Length)
                    {
                        case 3:
                            lista_Flags.Add(new Flag(a[0], j));

                            Açoes.Add(new Action(a[1].ToUpper(), a[2].Split('|')));
                            j++;
                            break;
                        case 2:
                            Açoes.Add(new Action(a[0].ToUpper(), a[1].Split('|')));
                            j++;
                            break;
                        case 1:
                            //não faz nada because seria um comentario
                            //Açoes.Add(new Açao());
                            break;
                        default:
                            //Action ignored
                            break;
                    }
                }
            }
        }

        private class Action
        {
            public string actionType;
            public string[] parameters = { }; //<-- Amamm

            public Action(string açao, string[] parameters)
            {
                this.actionType = açao;
                this.parameters = parameters;
            }

            // public Action()
            // {
            //     this.actionType = "Comentario";
            // }
        }

        private class Flag
        {
            public string nome;
            public int posiçao;

            public Flag(string nome, int posiçao)
            {
                this.nome = nome;
                this.posiçao = posiçao;
            }
        }

        //Used for making sure that everything is working proprely
        //If the given expression is not true an exception is throwned
        private static void Assert(bool expr, InterExecpType type)
        {
            if (!expr)
            {
                throw new InterpretationException("Assertion Catch. Type: " + type.ToString("G"));
            }
        }

        private static void Assert(bool expr, string msg)
        {
            if (!expr)
            {
                throw new InterpretationException("Assertion Catch. Msg: " + msg);
            }
        }

        public enum InterExecpType
        {
            NullBlockVarList,
            NullBlockFlagList,
            ActionNotFound,
            NoMainBlockFound,
            StructNotFound,
            DuplicateInputBlockVariableName,
            InvalidBlockCallSyntax,
            BlockNotFound,
            TypesDontMatch,
            GettingValueFromBlockWithoutReturnValue,
            GotNullBasicValue,
            InvalidOperator,
            TODO,
            ExpectedVariable,
            VariableNotFound,
            InvalidParameterQuantity,
        }

        [System.Serializable]
        public class InterpretationException : System.Exception
        {
            //public InterpretationExeption() {}
            public InterpretationException(string message) : base(MontarMensagemErro(message)) { }
            public InterpretationException(int parametro, string message) : base(MontarMensagemErro(parametro, message)) { }
            public InterpretationException(InterExecpType type) : base(MontarMensagemErro(type.ToString("G"))) { }
            public InterpretationException(InterExecpType type, int parameter) : base(MontarMensagemErro(parameter, type.ToString("G"))) { }

            //Monta uma mesagem que mostra a linha onde ocorreu o erro com as 3 linhas anteriores e a menssagem de erro;
            private static string MontarMensagemErro(int parametro, string mensagem)
            {
                string stackTrace = "Block Trace: ";
                Block currentBlock = blockStack.Peek();
                foreach (Block block in blockStack)
                {
                    stackTrace += block.name + " <- ";
                }

                string ultimas3Instruçoes = "";
                if (currentActionIndex >= 3)
                    ultimas3Instruçoes = GetBlockInstruction(currentBlock, currentActionIndex - 3) + "\n" + GetBlockInstruction(currentBlock, currentActionIndex - 2) + "\n" + GetBlockInstruction(currentBlock, currentActionIndex - 1) + "\n";

                string instruçaoComErro = GetBlockInstruction(currentBlock, currentActionIndex);
                return ("\n-----------------<ERROR>----------------\n" +
                        stackTrace + "\n\n" +
                        ultimas3Instruçoes +
                        instruçaoComErro + "\n" +
                        (currentActionIndex >= 0 ? BuildErrorArrow(instruçaoComErro, parametro, mensagem) : "# " + mensagem) + "\n" +
                        "-----------------<ERROR>----------------"
                        );
            }

            //Returns the Error Arrow
            private static string BuildErrorArrow(string wrongInstruction, int wrongParamIndex, string message)
            {
                //Set:num|amam;
                //        ^^^^#>Variavel ja Existente.

                string[] words = wrongInstruction.Split(':', '|', ';');
                int wrongParamLenght = words[wrongParamIndex].Length;


                int wordIndex = wrongInstruction.IndexOf(words[wrongParamIndex]);

                string arrow = BuildThings(wordIndex, ' ') + BuildThings(wrongParamLenght, '^') + "  # " + message + "(" + wrongParamIndex + "ª)";

                return arrow;

                string BuildThings(int length, char character)
                {
                    string arrow = "";
                    for (int i = 0; i < length; i++)
                    {
                        arrow += character;
                    }
                    return arrow;
                }
            }

            private static string MontarMensagemErro(string mensagem)
            {
                if (mensagem == "No 'main' block found")
                {
                    return mensagem;
                }

                return ("\n-----------------<ERROR>----------------\n" +
                        "# " + mensagem + "\n" +
                        "-----------------<ERROR>----------------"
                        );
                string stackTrace = "Block Trace: ";
                Block currentBlock = blockStack.Peek();
                foreach (Block block in blockStack)
                {
                    stackTrace += block.name + " <- ";
                }

                string ultimas3Instruçoes = "";
                if (currentActionIndex >= 3)
                    ultimas3Instruçoes = GetBlockInstruction(currentBlock, currentActionIndex - 3) + "\n" + GetBlockInstruction(currentBlock, currentActionIndex - 2) + "\n" + GetBlockInstruction(currentBlock, currentActionIndex - 1) + "\n";

                string instruçaoComErro = GetBlockInstruction(currentBlock, currentActionIndex);
                return ("\n-----------------<ERROR>----------------\n" +
                        stackTrace + "\n\n" +
                        ultimas3Instruçoes +
                        instruçaoComErro + "\n" +
                        "# " + mensagem + "\n" +
                        "-----------------<ERROR>----------------"
                        );
            }

            private static string GetBlockInstruction(Block block, int index)
            {
                string paramss = "";
                if (index == -1)
                {
                    return "";
                }
                foreach (string paramm in block.actions[index].parameters)
                {
                    paramss += paramm + "|";
                }
                paramss = paramss.Remove(paramss.Length - 1) + ";";
                return block.actions[index].actionType + ":" + paramss;
            }

            // public InterpretationExeption(string message, System.Exception inner) : base(MontarMensagemErro(message), inner) { }
            // protected InterpretationExeption(
            //     System.Runtime.Serialization.SerializationInfo info,
            //     System.Runtime.Serialization.StreamingContext context) : base(info, context) {

            //     }
        }
    }
}
