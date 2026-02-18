---
deck_name: "GCM"
source: ""
separator: "---"
templates:
  - name: "Conceito"
    fields: ["Pergunta", "Resposta", "Contexto"]
    usage: ""
    html_question_format: "<div class='question'>{{Pergunta}}</div>"
    html_answer_format: "<div class='answer'><strong>{{Resposta}}</strong></div><div class='context'>{{Contexto}}</div>"
    css_format: |
      .card {
        font-family: arial;
        font-size: 20px;
        text-align: center;
        color: #333;
        background-color: #fff;
      }
      .question {
        margin-bottom: 30px;
        font-weight: bold;
        color: #225599;
      }
      .answer {
        margin-bottom: 20px;
        color: #007700;
        font-size: 18px;
      }
      .context {
        margin-top: 15px;
        font-size: 14px;
        color: #666;
        font-style: italic;
      }
      
  - name: "Omissao"
    fields: ["Texto", "Comentário"]
    usage: ""
    html_question_format: "<div class='cloze-question'>{{Texto}}</div>"
    html_answer_format: "<div class='cloze-answer'>{{Comentário}}</div>"
    css_format: |
      .card {
        font-family: arial;
        font-size: 20px;
        text-align: center;
        color: #333;
        background-color: #fff;
      }
      .cloze-question {
        margin-bottom: 25px;
      }
      .cloze {
        background-color: #0066ff;
        color: #0066ff;
        padding: 0 2px;
        border-radius: 3px;
      }
      .cloze-answer {
        margin-top: 20px;
        font-size: 16px;
        color: #666;
        font-style: italic;
        padding: 15px;
        background-color: #f0f0f0;
        border-radius: 5px;
      }
  - name: "Questao"
    fields: ["Enunciado", "Gabarito", "Comentário", "Fonte"]
    usage: ""
    html_question_format: "<div class='question'>{{Enunciado}}</div>"
    html_answer_format: "<div class='answer'>Gabarito: <strong>{{Gabarito}}</strong></div><div class='comment'>{{Comentário}}</div><div class='source'>Fonte: {{Fonte}}</div>"
    css_format: |
      .card {
        font-family: arial;
        font-size: 18px;
        text-align: left;
        color: #333;
        background-color: #f9f9f9;
      }
      .question {
        margin-bottom: 25px;
        padding: 15px;
        background-color: #eef5ff;
        border-left: 4px solid #225599;
      }
      .answer {
        margin-bottom: 15px;
        font-weight: bold;
        color: #007700;
        font-size: 16px;
      }
      .comment {
        margin-bottom: 12px;
        font-size: 14px;
        color: #555;
        padding: 10px;
        background-color: #fffacd;
      }
      .source {
        font-size: 12px;
        color: #999;
        margin-top: 10px;
      }
      
---

# matematica_e_raciocinio_logico

## interpretacao_matematica_de_situacoes_problema_contextualizadas

### adicao_subtracao_multiplicacao_e_divisao

#### naturais_n_inteiros_z_racionais_q_irracionais_i_reais_r

```Conceito
em 30/5, quem é o dividendo, divisor, resto e quociente?
---
Dividendo = 30

Divisor = 5

Quociente = 6

Resto = 0
---
Quociente é o resultado da fração
```

```Conceito
Forma fracionária de 8,333...
---
25/3
---
A: 8,333 = 8,333

B:10*8,333... = 83,333...

B-A = 9*8,333 = 83,333 - 8,333 = 75 => 8,333 = 75/9 = 25/3
```

```Conceito
Numero irracional
---
Número que não possui representação fracionária devido a inexistência de um padrão na dízima periódica.
---
6,33225148....

pi

número de Euler
```

```Conceito
Quem contem quem em: R, I, Q, Z, N
---
R c I, R c Q c Z c N
---
I não contem nenhum outro
```

## potenciacao_e_radiciacao_com_numeros_racionais_nas_suas_representacoes_fracionaria_ou_decimal

```Conceito
Propriedades de potenciação
---
a^b => a * a * a (b vezes)

(a^b)^c => a^b*c => a * a * a (b*c vezes)

a^(1/2) => sqrt(a) (raiz)
---
(4^(1/2))^(1/2) = 4^(1/4) = (2^2)^(1/4) = 2^(1/2) = sqrt(2)
```

## resolucao_de_problemas_envolvendo_mais_de_uma_operacao

```Conceito
Ordem das operações
---
Parênteses > Expoentes > Multiplicação > Divisão > Adição > Subtração
---
PEMDAS
```

## minimo_multiplo_comum_e_maximo_divisor_comum

```Conceito
Enunciados relacionados ao MMC geralmente remetem a ideia de:
---
Periodicidade, repetição e ciclo de acontecimentos.



Atente aos termos “a cada”, “em” e “ou” nos enunciados, pois estes podem indicar uma ideia de repetição, ciclo e periodicidade.
---
Em uma linha de montagem de fábrica, duas luzes de sinalização piscam em intervalos diferentes: uma a cada 20 minutos e a outra a cada 35 minutos. Se ambas piscarem juntas às 8 horas da manhã, em que horário isso voltará a acontecer?



Observe as expressões “a cada 20 minutos” e “a cada 35 minutos”. Percebe-se, aqui, uma ideia de repetição. Por exemplo, se a luz que pisca a cada 20 minutos acender às 15h, ela irá piscar novamente após 20 minutos, ou seja, às 15h20, depois às 15h40, 16h, e assim por diante.

Portanto, esse é um tipo clássico de questão envolvendo MMC.

Para resolvermos a questão anterior, devemos encontrar o MMC entre 20 e 30 e converter para horas = 140min => 2h20 => 10h20min
```

## razao_e_proporcao

```Conceito
Definição de Razão e proporção
---
A razão entre duas grandezas é igual à divisão entre elas. Já a proporção é a igualdade entre razões.
---
2/3 = 4/6
```

### propriedades

```Conceito
Somas Externas
---
a/b = c/d = (a+c) / (b+d)
---
Suponha que uma fábrica vai distribuir um prêmio de R$ 10 mil para seus dois empregados (Carlos e Diego). Esse prêmio vai ser dividido de forma proporcional ao seu tempo de serviço na fábrica. Carlos está há três anos na fábrica e Diego está há dois anos na fábrica. Quanto cada um vai

receber?



É a constante de proporcionalidade! Temos 5 partes par 10mil.
```

```Conceito
Somas Internas
---
a/b = c/d => (a+b)/b = (c+d)/d ou (a+b)/a = (c+d)/c
---
Contexto/Exemplos
```

```Conceito
Soma com Produto por Escalar
---
a/b = c/d => (a+2b)/b = (c+2d)/d
---
Contexto/Exemplos
```

### raciocinio_proporcional_aplicado_a_contextos_praticos

#### diretamente_e_inversamente_proporcional

```Questao
A quantia de R$ 900 mil deve ser dividida em partes proporcionais aos números 4, 5 e 6. A menor dessas partes corresponde a...
---
240
---
900/(4+5+6) = 60
---
NovaConcursos
```

```Questao
Suponha que queiramos dividir 740 mil em partes inversamente proporcionais a 4, 5 e 6.
---
Gabarito
---
X/4 + X/5 + X/6 = 740000

37X/60 = 740000

X = 1200000 => 300000; 240000; 200000
---
NovaConcursos
```

```Questao
(VUNESP — 2020) Em um grupo com somente pessoas com idades de 20 e 21 anos, a razão entre o número de pessoas com 20 anos e o número de pessoas com 21 anos, atualmente, é 4 ÷ 5. No próximo mês, duas pessoas com 20 anos farão aniversário, assim como uma pessoa com 21 anos, e a razão em questão passará a ser de 5 ÷ 8. O número total de pessoas nesse grupo é
---
27
---
A/B = 4x/5x => 9x ao todo

4x-2 / 5x+1 = 5/8 => x=3

9*3 = 27
---
NovaConcursos
```

```Questao
(CEBRASPE-CESPE — 2018) A respeito de razões, proporções e inequações, julgue o item seguinte. Situação hipotética: Vanda, Sandra e Maura receberam R$ 7.900 do gerente do departamento onde trabalham, para ser divido entre elas, de forma inversamente proporcional a 1 ÷ 6, 2 ÷ 9 e 3

÷ 8, respectivamente. Assertiva: Nessa situação, Sandra deverá receber menos de R$ 2.500
---
Falso
---
(6/1 + 9/2 + 8/3)*X = 7900 => X = 600

9/2*600 =9*300 = 2700
---
NovaConcursos
```

#### regra_de_tres_simples

```Questao
Exemplo 1: um muro de 12 m foi construído utilizando 2.160 tijolos. Caso queira construir um muro de 30 m nas mesmas condições do anterior, quantos tijolos serão necessários?
---
5400
---
12/30 = 2160/x => 12*x = 30*2160 => x = 5400

Diretamente proporcionais multiplica cruzado
---
NovaConcursos
```

```Questao
Exemplo 2: uma equipe de cinco professores gastou 12 dias para corrigir as provas de um vestibular. Considerando a mesma proporção, quantos dias levarão 30 professores para corrigir as provas?
---
Gabarito
---
Veja que, de cinco (professores) para 30 (professores), tivemos um aumento (+), mas, como agora estamos com uma equipe maior, o trabalho será realizado mais rapidamente. Logo, a quantidade de dias deverá diminuir (–). Dessa forma, as grandezas são inversamente proporcionais;

resolveremos multiplicando na horizontal.



30*x = 5*12 => x = 2
---
NovaConcursos
```

```Questao
(IESES — 2019) Cinco pedreiros construíram uma casa em 28 dias. Se o número de pedreiros fosse aumentado para sete, em quantos dias essa mesma casa ficaria pronta?

a) 18 dias.

b) 16 dias.

c) 20 dias.

d) 22 dias.
---
20
---
5 / 7 = 28 / X => Inversamente proporcional

=> 7*X = 28*5 = 20
---
NovaConcursos
```

#### regra_de_tres_composta

```Conceito
Princípios da regra de três composta
---
as análises devem sempre partir da variável dependente em relação às outras variáveis;



as análises devem ser feitas individualmente. Ou seja, deve-se comparar as grandezas duas a duas, mantendo as demais constantes;



a variável dependente fica isolada em um dos lados da proporção.
---
Exemplo 1: se seis impressoras iguais produzem mil panfletos em 40 minutos, em quanto tempo três dessas impressoras produziriam dois mil desses panfletos?



1) Compara X com número de impressoras => Inversamente proporcional

2) Compara X com número de planfetos => Diretamente proporcional

3) 40/X fica do lado esquerdo da igualdade e 1) inverte e 2) mantem

=> 160min
```

```Questao
(VUNESP — 2016) Em uma fábrica, 5 máquinas, todas operando com a mesma capacidade de produção, fabricam um lote de peças em 8 dias, trabalhando 6 horas por dia. O número de dias necessários para que 4 dessas máquinas, trabalhando 8 horas por dia, fabriquem dois lotes dessas peças é
---
15
---
5maq - 1l - 8d - 6h

4maq - 2l - xd - 8h

=> 8/x = 4/5*1/2*8/6 => x=15



Quanto mais dias para a entrega do lote, menos horas trabalhadas por dia (inversa), menos máquinas para fazer o serviço (inversa) e mais lotes para serem entregues (direta).
---
NovaConcursos
```

#### porcentagem

```Questao
(CEBRASPE-CESPE — 2020) Em determinada loja, uma bicicleta é vendida por R$ 1.720 à vista ou em duas vezes, com uma entrada de R$ 920 e uma parcela de R$ 920 com vencimento para o mês seguinte. Caso queira antecipar o crédito correspondente ao valor da parcela, a lojista paga

para a financeira uma taxa de antecipação correspondente a 5% do valor da parcela. Com base nessas informações, julgue o item a seguir.

Na compra a prazo, o custo efetivo da operação de financiamento pago pelo cliente será inferior a 14% ao mês.
---
15%
---
Valor da bicicleta = R$ 1.720

Parcelado = R$ 920 (entrada) + R$ 920 (parcela)

Na compra a prazo, o agente vai pagar R$ 920 (entrada); logo, sobrarão (1.720 – 920 = 800).

No próximo mês, é preciso pagar R$ 920, ou seja, 800 + 120 de juros. Agora, peguemos 120 (juros) e dividamos por 800: 120,00 ÷ 800,00 = 0,15% ao mês. A questão afirma que seria inferior a 0,14%, ou seja, está errada.
---
NovaConcursos
```

```Questao
(FUNCAB — 2015) Adriana e Leonardo investiram R$ 20.000,00, sendo o 3/5 desse valor em uma aplicação que gerou lucro mensal de 4% ao mês durante dez meses. O restante foi investido em uma aplicação, que gerou um prejuízo mensal de 5% ao mês, durante o mesmo período. Ambas as aplicações foram feitas no sistema de juros simples. Pode-se concluir que, no final desses dez meses, eles tiveram
---
800 de lucro
---
3 ÷ 5 de 20.000 = 12.000

12.000 · 4% = 12.000 · 0,04 = 480

480 · 10 (meses) = 4.800 (lucro)

O que sobrou: 20.000 – 12.000 = 8.000. Aplicação que foi investida e gerou prejuízo de 5% ao

mês, durante 10 meses:

8.000 · 5% = 8.000 · 0,05 = 400

400 · 10 meses = 4.000 (prejuízo)

Logo, a aplicação que gerou lucro menos a aplicação que gerou prejuízo:

4.800 – 4.000 = 800 (lucro)
---
NovaConcursos
```

