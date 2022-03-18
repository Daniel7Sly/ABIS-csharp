SAL
===

Este é o meu projeto onde estou a criar uma linguagem Simples baseada em Ações. Simple Action Language SAL.
Ideia de Usar isto na PAP.

# Instruções

## PRINT
Print: Value(string)\id;

## PRINTL
igual ao PRINT mas faz quebra de linha no fim.

## READ 
Define o valor que receber á variavel dada (converte string recebida automaticamente para o type da variavel dada.)
Read: id;

## SET 
Define variavel, Variaveis são chamadas de id e são indentificadas no codigo pelo 1º caracter => '$'
Set: type | nome | value;
*types: num(float), bool, text.

## OPR
Faz Operaçoes
Opr: id | id\value | operador | id\value; (c = b + a)
*operadores: add\+ sub\- mul\* div\/

## EQL
Atribui valore a variavel
Eql: id | id\value;

## FLAG
Define flag
Flag: flag_name;

## Goto
Vai até á flag especificada
Goto: flag_name;

## CMP
Atribui a variavel o resultado da comparaçao (em bool)
Cmp: id(bool) | id\value | comparator | id\value;
*comparators: < > >= <= == !=

## IF
É um IF se true -> goto flag_name
IF: id(bool) | flag_name;