**ABIS**
============
## Intructions

Actions are the instructions of ABIS.
Every action have a name (ex: `Eql`) and parameters. Every parameter
is separated by `'|'` and the Action name is sperarated by the parameters by `':'`. The Action ends with `';'`. 

As showned bellow.
```
 |----instructrion:----|
 |                     |  
 Eql  :    $var | value;
  ^        |          |
actionName |Parameters|
```
 

## Variaveis

Variaveis são usadas para armazenar valores e fornecer valores a parametros de ações do tipo correspondente.

Existem 3 tipos de Variaveis:  
> `num` 	-> com um valor numerico(pode ter casas decimais).  
> `bool` 	-> valor 'true' ou 'false'.  
> `text` 	-> contem texto.  

Variaveis são indetificadas no codigo por `'$'` seguido pelo nome.
> Ex: `$variavel`

Em arrays é usado o `'#'` depois do nome para indicar o index que queremos o valor.
> Ex: `$listaNomes#23` --> dá o valor da posição 23

Também é possivel identificar o index do array com uma variavel.
> Ex: `$listaNumeros#$index` ($index é uma variavel num)

## Blocos

***TODO***

## Flags

Flags servem para marcar posições no codigo para ser redirecionado com `IF` & `Goto`. Flags são marcadas com `:` seguido de uma ação.

Ex:
```
FlagName:
	Print: someting;
	Goto: FlagName;
```

## Operations

***TODO***

# **Actions** 

## SET

Define uma variavel com o tipo da variavel o nome.  
Ex: 
> `Set: type|nome;`

> `Set: num|numero1;`

Isto cria a variavel `numero1` do tipo numerico com o valor 0;

Parametros:
> `type`: The type of the variable you wish to create.
> 
> `name`: The name that the variable will have.  

Default values:
> `num : 0`.  
> `text : ''`  
> `bool : false`

## SetArray

***DEPRECATE***  
> ~~Define um array do tipo de dado, com os valores/tamanho 
SetArr: type|nome|{1,2,3,4}; 		<-- array com tamanho 4 e valores predefinidos
SetArr: type|nome|$(num);			<-- array de tamanho do numero da variavel, sem valores predefinidos
SetArr: type|nome|10;				<-- array de tamanho 10 sem valores predefinidos
SetArr: type|nome|{amam, $b, $c#1};	<-- array de tamanho 3 com valores predefinidos~~

## Print

Escreve o valor na consola. `'_'` são substituidos por espaços
```
Print: text;
```

## Read

***OUTDATED***  
>Lê o que o usuario escrever no terminal.
Caso a variavel seja num ou bool READ tentará converter o texto recebido para o tipo dado.
Caso não consiga rebenta
```
Read: $;
```

## Equals

Atribui o valor (2º param) á variavel(1º param). O valor recebido tem que corresponder ao
tipo da variavel recebida.
```
Eql: $:type|$\value;
```

## JoinText

Junta os valores (param 2 & 3) como se fossem texto e atribui o resultado á variavel dada.

Pode ser usado para converter `bool` e `num` para text.
```
Jtxt: $(text)|value|value;
```

## Parse

Tenta converter o valor dado para num e atribui o valor á variavel dada.
Atribui true á variavel do 1º param se a conversão foi feita com sucesso senão atribui false.
Se conseguir converter o resultado é atribuido á 2ª variavel.
```
Prs: $(bool) | $(num) | $\value(text);
```

## GetArrayLegth

Obtem o tamanho do array dado (2º param) e atribui o valor á variavel 1º param.
```
GLength: $(num)|$#;
```

## Goto

Vai até á flag especificada.
```
Goto: flag_name;
```

## If

Caso o valor da variavel(1º param) dada for `true` é redirecionado para a flag indicada.
```
If: $(bool)|flag_name;
```

## IfN

Same as `IF` but the value of the variable has to be `false`.
```
IfN: $(bool)|flag_name;
```