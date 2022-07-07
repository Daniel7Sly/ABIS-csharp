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

Variables are used to store and provide values to parameters of actions of 
the corresponding type.

There are 3 types of variables:  
> `num` 	-> stores a number(can have decimal places).  
> `bool` 	-> value 'true' ot 'false'.  
> `text` 	-> just stores text.  

When a variable is created it starts with a default value:
> `num` 	-> 0  
> `bool` 	-> false  
> `text` 	-> ''. (empty)

Variables are identified in code by the `'$'` followed by the name.

> Ex: `$variavel`

In arrays is used the `'#'` after the name to identify the index that we 
want the value.
> Ex: `$listaNomes#23` --> gives the value in position 23.

It is also possible to identify the index with a variable.
> Ex: `$listaNumeros#$index` ($index is of type num)

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

The example bellow shows a block which recives two numbers `a` and `b` , and returns the sum
of the squares of that numbers.
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

Flags are used to mark positions in code to be redirected with IF or GOTO
actions. Flags are marked with `:` before the next action name.

It´s not possible to jump to flags of diferent blocks.

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

Creates a variable of the given type and name.  
Ex: 
> `Set: type|name;`

> `Set: num|numero1;`

This creates the variable `number1` of type num.

Parameters:
> `type`: The type of the variable you wish to create.
> 
> `name`: The name that the variable will have.  

Default values:
> `num : 0`.  
> `text : ''`  
> `bool : false`

## SetArray

***! ARRAYS ARE NOT TOTALY IMPLEMENTED!***  
Define um array do tipo de dado, com os valores/tamanho.

>``SetArr: type|nome|{1,2,3,4};`` 		<-- array com tamanho 4 e valores 
>predefinidos
>
>``SetArr: type|nome|$(num);``			<-- array de tamanho do numero davariavel, >sem valores predefinidos
>
>``SetArr: type|nome|10;``				<-- array de tamanho 10 sem valores
>predefinidos
>
>``SetArr: type|nome|{amam, $b, $c#1};``	<-- array de tamanho 3 com valores
>predefinidos

## Print

Writes the given value in the console. `'_'` are subtituted 
```
Print: text;
```

## Read

Reads what the user writes in the console and atributed the values to the given variable.
```
Read: $(text);
```

## Equals

Gives to the variable the given value.
```
Eql: $:type|value:(type);
```

## JoinText

Merges the 2º and 3º param values as text and atributes the value to the given
variable(1º param).
```
Jtxt: $(text)|value|value;
```

## SplitText

Splits the given text(2º param) with the text spliter(3º param) and asigns each part to
the text array(1º param).
```
SplitText: #rsltArr(text) | textToDivide:(text) | Divisor:(text);
```
Example:
```
  SetArr:text|textArr|0;
  
  SplitText: $textArr | 2020/40/35 | /;
  
  PrintL: $textArr#0;
  PrintL: $textArr#1;
  PrintL: $textArr#2;
  --------------------
  Output: 	2020
			40
			35
```


## Parse

Trys to parse the given text into a number.
If the convertion fails `false` is atributed to the 1º param, otherwise `true`
is given and the convertion result is given to the variable(2º param).
```
Prs: $(bool) | $(num) | $\value(text);
```

## GetArrayLegth

Atributes to the 1º param the lenght of the given variable.
```
GLength: $(num)|$#;
```

## Goto

Goes to the specified flag.
```
Goto: flag_name;
```

## If

If the variable is true, jumps to the indicated flag.
```
If: $(bool)|flag_name;
```

## IfN

Same as `IF` but the value of the variable has to be `false`.
```
IfN: $(bool)|flag_name;
```
