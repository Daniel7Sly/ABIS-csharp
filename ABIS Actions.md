**ABIS**
============
## Actions

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
Every ABIS Script starts executing at the `@main` block.

Hello world in ABIS:
```
@main[]{
	PrintL: Hello_World!;
}
```
> Output: `Hello World!`
## Variaveis

Variaveis são usadas para armazenar valores e fornecer valores a parametros de ações do tipo correspondente.

Existem 3 tipos de Variaveis:  
> `num` 	-> com um valor numerico(pode ter casas decimais).  
> `bool` 	-> valor 'true' ou 'false'.  
> `text` 	-> contem texto.  

When a variable is created it starts with a default value:
> `num` 	-> 0  
> `bool` 	-> false  
> `text` 	-> ''. (empty)

Variaveis são indetificadas no codigo por `'$'` seguido pelo nome.
> Ex: `$variavel`

Em arrays é usado o `'#'` depois do nome para indicar o index que queremos o valor.
> Ex: `$listaNomes#23` --> dá o valor da posição 23

Também é possivel identificar o index do array com uma variavel.
> Ex: `$listaNumeros#$index` ($index é uma variavel num)

## Blocks

Blocks are pieces of code that preform intructions and return a value.

A Block must be defined out side of other blocks. To define a
Block start with `'@'` and then the name of the block. After
open `'[]'` and inside goes the input variables/parameters of
the block, first the type and than the name separated by `':'`, 
separate each parameter with `';'`. After the `'[]'` comes the
`->` and then the output type. Then open `'{ }'` and inside goes
all the actions of the block. A Block cannot finnish without
`RETURN` someting.

The example bellow shows a block which recives two numbers `a` and `b` , and returns the sum of the squares of that numbers.
```
@SumOfSquare[num:a;num:b] -> num {
	Set: num|rslt;
	Eql: $rslt|(($a * $a) + ($b * $b));
	Return:$rslt;
}
```

The example below show a program that prints the Sum of the
squares of the numbers `$i` and `$j` using the `@SumOfSquares`
block created above:
```
Set: num|i; Eql: $i | 5;
Set: num|j; Eql: $j | 10;

PrintL: @SumOfSquares[$i, $j];
```

>Output: `125`
## Flags

Flags servem para marcar posições no codigo para ser redirecionado com `IF` & `Goto`. Flags são marcadas com `:` seguido de uma ação.

Não é possivel redirecionar para flags de blocos diferentes.

Ex:
```
FlagName:
	Print: someting;
	Goto: FlagName;
```

## Operations

Operations are used to make comparations and Arithmetics.
To do an operation you have open and close `( )` and inside the
operation you want to do. You can use variables to, like showned
below where we are summing 5 to the value of `$a` (a is num):

Ex: `($a + 5)`

Operations return a `num` value or a `bool` value, it depends on what operator you are using.

List of operators
> operators that return `num`.  
> `+ - * / %`  
> operators that return `bool`  
> `< > = ! & "`  
> All the operators that return `num` can only work with `num`
> values.  
> The operators that return `bool` can only work with `num`
> with the exception of `&` and `"` that only work with `bool`,
> and the exception of `=` and `!` which can work with 
> `num`, `bool`, and `text`.

You can also do operations inside operations and call blocks. Ex:
```
If: (($a + $b) > (@SumOfSquares[$i, $j] - 100))| flag;
```
### Tenary Operator

The tenary operator is a special kind of operator. The tenary operator
allows you to do quick IF-ELSE atributions. The tenary operator differently
from the other operators this one has to be placed twice in a statement
because it takes 3 arguments instead of 2.

Ex:  
`((value1) ? value2 ? value3)`

How it works.  
> If the `value1` is `true` the `value2` is returned.  
> Otherwise it returns `value3`.  
> `value1` must be a bool value.

Example:  
In this example we are checking if `a` is bigger than `b`,if so the
value `Bigger` is assign to `x` other wise the value `Smaller` is assign.
```
Eql: $x | (($a > $b) ? Bigger ? Smaller);
PrintL: $x;
```
> a = 10 , b = 20  
> Output `Smaller`

> a = 10 , b = 5  
> Output `Bigger`  

# **Actions** 

## SET

Define uma variavel com o tipo da variavel o nome.  
Ex: 
> `Set: type|name;`

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

Tenta converter o valor dado para num.
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
