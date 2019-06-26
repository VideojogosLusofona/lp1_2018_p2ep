<!--
Projeto de 2ª Época de Linguagens de Programação I 2018/2019 (c) by Nuno Fachada

Projeto de 2ª Época de Linguagens de Programação I 2018/2019 is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
-->

# Projeto de 2ª Época de Linguagens de Programação I 2018/2019

## Descrição do problema

Os alunos devem implementar, em grupos de 1 a 3 elementos, um jogo _roguelike_
em C# (.NET Core console app) com níveis em grelha
[gerados procedimentalmente](#procedural) e vários graus de dificuldade. O
jogador começa no lado esquerdo da grelha (1ª coluna), e o seu objetivo é
encontrar a saída do nível, que se encontra do lado direito dessa mesma
grelha (última coluna). Pelo meio o jogador pode encontrar inimigos, encontrar
itens (comida, armas, mapas), possivelmente apanhando-os, e cair em armadilhas.

Os níveis vão ficando progressivamente mais difíceis, com mais inimigos, mais
armadilhas e menos itens. O [_score_](#score) final do jogador depende do nível
atingido, do grau de dificuldade do jogo e do número de inimigos derrotados.
Deve existir, para cada dimensão da grelha, uma tabela dos _top_ 8 _high
scores_, que deve persistir quando o programa termina e o PC é desligado.

No início de cada nível o jogador só tem conhecimento da sua vizinhança de
[Moore]. À medida que o jogador se desloca, o mapa vai-se revelando. O
jogador pode deslocar-se na sua vizinhança de [Moore] usando as teclas do
_keypad_.

### Modo de funcionamento

### Invocação do programa

O programa deve aceitar três opções na linha de comando<sup>[1](#fn1)</sup>:

* `-r` - Número de linhas da grelha de jogo
* `-c` - Número de colunas da grelha de jogo.
* `-d` - Grau de dificuldade do jogo.

Um exemplo de execução:

```
dotnet run -- -r 10 -c 7 -d 3
```

A primeira opção, `--`, serve para separar entre as opções do comando `dotnet`
e as opções do programa a ser executado, neste caso o nosso jogo.

As opções indicadas são obrigatórias e podem ser dadas em qualquer ordem, desde
que o valor numérico suceda à opção propriamente dita. Se alguma das opções for
omitida o programa deve terminar com uma mensagem de erro indicando o modo de
uso.

Se os alunos implementarem a [Fase Extra](#faseextra), nomeadamente a opção de
_Save Game_, o programa pode também aceitar a opção `-l` para fazer _load_ de
um jogo guardado anteriormente. Esta opção tem de ser usada exclusivamente. Por
exemplo:

```
dotnet run -- -l mysavegame.sav
```

As opções de linha de comandos podem também ser definidas diretamente no Visual
Studio 2017 da seguinte forma: 1) clicar com o botão direito em cima do nome do
projeto; 2) selecionar "Properties"; 3) selecionar separador "Debug"; e, 4) na
caixa "Command line arguments" especificar os argumentos desejados.

### Menu principal

O jogo começa por apresentar o menu principal, que deve conter as seguintes
opções:

```
1. New game
2. High scores
3. Credits
4. Quit
```

Caso o utilizador selecione as opções 2 ou 3, é mostrada a informação
solicitada, após a qual o utilizador pressiona ENTER (ou qualquer tecla) para
voltar ao menu principal. A opção 4 termina o programa. Se for selecionada a
opção 1, começa um novo jogo.

#### Ações disponíveis no jogo

As ações disponíveis em cada _turn_ são as seguintes:

* Teclas do _keypad_ para movimento possível em oito direções
* `F` para atacar um inimigo no _tile_ atual.
* `E` para apanhar um item no _tile_ atual (incluindo mapas).
* `U` para usar um item (arma ou comida).
  * No caso de uma arma, a mesma é equipada (selecionada para combate). Caso
    exista uma arma equipada anteriormente, a mesma passa para o inventário.
  * No caso de comida, a mesma é consumida, aumentando o HP na quantidade
    especificada para a comida em questão, até um máximo de 100.
* `D` para deixar cair um item (arma ou comida) no _tile_ atual.
* `L` para mostrar descrição dos itens na vizinhança de [Moore] do jogador
  (incluindo local onde o jogador se encontra). Esta opção **não** consome uma
  _turn_.
* `H` para mostrar informação acerca dos itens (armas e comida) e armadilhas
  disponíveis no jogo. Esta opção **não** consome uma _turn_.
* `S` para guardar o jogo (funcionalidade opcional, tal como descrito na
  [Fase Extra](#faseextra)).
* `Q` para terminar o jogo.

As opções `F`, `E`, `U` e `D` devem ser seguidas de um número, indicando qual o
inimigo a atacar ou o item a apanhar/usar/deixar cair. Deve ser permitido
cancelar a opção antes da indicação do número, sem que o jogador gaste uma
_turn_.

Em cada _turn_ é consumido automaticamente 1 HP do jogador.

#### O jogador

O jogador tem várias características, algumas definidas diretamente, outras
calculáveis a partir das restantes:

* `HP` (_hit points_) - Vida do jogador, entre 0 e 100; quando chega a zero
  o jogador morre.
* `SelectedWeapon` - A arma que o jogador usa em combate.
* `Inventory` - Lista de itens que o jogador transporta, nomeadamente comida e
  armas.
* `MaxWeight` - Constante que define o peso máximo que o jogador pode carregar.
* `Weight` - Peso total de tudo o que o jogador transporta, nomeadamente
  itens no inventário (armas e comida) e a arma equipada. Não pode
  ultrapassar `MaxWeight`.

#### Inimigos

Os inimigos têm as seguintes características:

* `HP` (_hit points_) - Vida do inimigo, semelhante à do jogador. Inicialmente
  os inimigos devem ter HPs relativamente pequenos, aumentando à medida que o
  jogo progride para níveis mais difíceis. O HP inicial dos inimigos é
  [aleatório](#procedural), mas nunca ultrapassando o valor 100.
* `AttackPower` - O máximo de HP que o inimigo pode retirar ao jogador em cada
  ataque. Inicialmente os inimigos devem ter um `AttackPower` relativamente
  pequeno, aumentando à medida que o jogo progride para níveis mais difíceis. O
  `AttackPower` de cada inimigo é [aleatório](#procedural), mas nunca
  ultrapassando o valor 100.

#### Itens

Todos os itens têm a seguinte característica:

* `Weight` - Peso do item.

Existem os seguintes itens em concreto:

* Comida - Podem existir diferentes tipos de comida, à escolha dos alunos. Cada
  tipo diferente de comida fornece ao jogador um HP pré-definido
  (`HPIncrease`), que não pode ultrapassar o valor 100, quando usado.
* Armas - Podem existir diferentes tipos de armas, à escolha dos alunos. Cada
  tipo diferente de arma tem um `AttackPower` e `Durability` específicos. O
  primeiro, entre 1 e 100, representa o máximo de HP que o jogador pode retirar
  ao inimigo quando o ataca. A `Durability`, entre 0 e 1, representa a
  [probabilidade](#procedural) da arma não se estragar quando usada num
  ataque. As armas são retiradas do jogo no momento em que se estragam.

Os itens podem existir em qualquer _tile_ do nível (exceto `EXIT!`) bem como
no inventário do jogador. No caso das armas, podem ainda ser equipadas pelo
jogador. Os itens podem também ser deixados cair pelos inimigos no _tile_ onde
se encontram após perderem um combate com o jogador.

#### Mapas

Existe um mapa por nível, colocado [aleatoriamente](#procedural) num _tile_.
Caso o jogador apanhe o mapa, todas as partes inexploradas do nível são
reveladas. Tal como os itens, o mapa não pode existir no _tile_ `EXIT!`.

#### Combate

Um inimigo ataca o jogador quando este entra ou se mantém no _tile_ onde o
inimigo está presente. A quantidade de HP que o jogador perde é igual a um
valor [aleatório](#procedural) entre 0 e o `AttackPower` do inimigo.

O jogador pode atacar qualquer inimigo presente no mesmo _tile_ selecionando a
opção `F` e especificando qual o inimigo a atacar. A quantidade de HP que o
jogador retira ao inimigo é igual a um valor [aleatório](#procedural) entre 0 e
o `AttackPower` da arma equipada. O jogador gasta uma _turn_ se tentar atacar
inimigos sem uma arma equipada.

Quando é realizado um ataque pelo jogador, existe uma
[probabilidade](#procedural) igual a `1 - Durability` da arma equipada se
partir. Neste caso a arma é removida das "mãos" do jogador e do jogo.

Caso o jogador não tenha uma arma equipada, pode gastar uma _turn_ a equipar
uma arma que tenha no inventário. O jogador pode ainda gastar uma _turn_ a
consumir comida (caso a tenha) se considerar que o seu HP está muito baixo. Em
ambos os casos o jogador será atacado se estiver no mesmo _tile_ que um
inimigo.

Caso o jogador vença o inimigo (ou seja, caso o HP do inimigo diminua até
zero), o inimigo desaparece do jogo, deixando para trás zero ou mais itens
[aleatórios](#procedural), que o jogador pode ou não apanhar. O número de itens
deixados para trás pelo inimigo varia com a dificuldade concreta do nível. Ou
seja, em níveis mais fáceis o inimigo deixa para trás mais itens.

Se o inimigo vencer o jogador (ou seja, caso o HP do jogador chegue a zero), o
jogo termina.

#### Armadilhas

As armadilhas têm as seguintes características:

* `MaxDamage` - Valor máximo de HP que jogador pode perder se cair na armadilha.
  É um valor entre 0 e 100.
* `FallenInto` - Indica se o jogador já caiu na armadilha ou não.

Podem existir diferentes tipos de armadilha no jogo, cada uma com um valor
específico para `MaxDamage`. É possível inclusive existir mais do que uma
armadilha por _tile_.

Quando o jogador entra pela primeira vez num _tile_ com uma ou mais armadilhas,
cada armadilha provoca uma perda [aleatória](#procedural) de HP ao jogador
entre 0 e `MaxDamage`, e o respetivo estado `FallenInto` passa a `true`. Se o
jogador voltar a entrar ou se passar mais _turns_ nesse _tile_, as armadilhas
já não causam estragos.

#### Fim do jogo

O jogo pode terminar de duas formas:

1. Quando o HP do jogador chega a zero devido a cansaço (pois o jogador perde
   1 HP por _turn_), devido a combate ou devido a armadilhas.
2. Quando o jogador seleciona a opção `Q`.

No primeiro caso verifica-se se o _score_ alcançado está entre os 8 melhores, e
em caso afirmativo, solicita-se ao jogador o seu nome para o mesmo figurar na
tabela de _high scores_ para a dimensão da grelha em questão.

#### Níveis e dificuldade

À medida que o jogo avança, os níveis vão ficando mais difíceis. Mais
concretamente, à medida que o jogo avança:

* Devem existir tendencialmente mais inimigos, além de que o respetivo `HP` e
  `AttackPower` devem ser cada vez maiores (mas nunca ultrapassando o máximo,
  100).
* Devem existir tendencialmente mais armadilhas.
* Devem existir tendencialmente menos itens (comida e armas) disponíveis para o
  jogador apanhar.
* Os inimigos deixam para trás tendencialmente menos itens quando são
  derrotados.

Adicionalmente, o jogo em si pode ter diferentes graus de dificuldade, entre 1
(mais fácil) e 10 (mais difícil). A forma mais simples de relacionar este grau
de dificuldade com a dificuldade concreta de cada nível consiste em usar uma
fórmula do seguinte género:

```
concreteLevelDifficulty = level * gameDifficulty
```

Por exemplo, o nível 3 num jogo com grau de dificuldade 4 corresponderá a uma
dificuldade concreta de 12.

Na secção [Geração procedimental e aleatoriedade](#procedural) são apresentadas
algumas sugestões de como gerar níveis em função de um valor concreto para a
sua dificuldade.

<a name="score"></a>

#### Pontuação (_score_)

O _score_ final do jogador é obtido através da seguinte fórmula:

```
score = (1 + 0.4 * gameDifficulty) * (level + 0.1 * enemiesKilledInGame)
```

Deve existir, para cada dimensão da grelha, uma tabela dos _top_ 8 _high
scores_, que deve persistir quando o programa termina e o PC é desligado. Por
exemplo, para a dimensão 10x6, os respetivos _high scores_ podem ser guardados
num ficheiro chamado `scores_10x6.txt`.

<a name="procedural"></a>

### Geração procedimental e aleatoriedade

A [geração procedimental][GP] é uma peça fundamental na história dos Videojogos,
tanto antigos como atuais. A [geração procedimental][GP] consiste na criação
algorítmica e automática de dados, por oposição à criação manual dos mesmos. É
usada nos Videojogos para criar grandes quantidades de conteúdo, promovendo a
imprevisibilidade e a rejogabilidade dos jogos.

O C# oferece a classe [Random] para geração de números aleatórios, que por
sua vez tem vários métodos úteis. Para usarmos esta classe é primeiro
necessário criar uma instância da mesma:

```cs
// Criar uma instância de Random usando como semente a hora atual do sistema
// Para efeitos de debugging durante o desenvolvimento do jogo pode ser
// conveniente usar uma semente fixa
// Deve ser usada a mesma instância de Random para o jogo todo
Random rnd = new Random();
```

Um dos métodos úteis é o método [NextDouble()], que retorna um `double` entre
0 e 1. É possível "atirar a moeda ao ar" usando este método, por exemplo:

```cs
if (rnd.NextDouble() < 0.25)
{
    Console.WriteLine("Cara");
}
else
{
    Console.WriteLine("Coroa");
}
```

No exemplo anterior definimos a probabilidade de sair "Cara" em 25% (e
consequentemente, de sair "Coroa" em 75%). O mesmo tipo de código pode ser
usado para determinar se a arma do jogador se estragou durante um ataque:

```cs
if (rnd.NextDouble() < 1 - weapon.Durability)
{
    // Arma estragou-se, fazer qualquer coisa sobre isso
}
```

O HP a ser subtraído devido a ataque ou armadilhas também pode ser determinado
com o método [NextDouble()], multiplicando o valor retornado pelo possível
máximo em questão. Por exemplo:

```cs
// Valor de HP a subtrair num ataque, será no máximo igual a AttackPower
double damage = Random.NextDouble() * weapon.AttackPower;
```

```cs
// Valor de HP a subtrair devido a armadilha, será no máximo igual a MaxDamage
double damage = Random.NextDouble() * trap.MaxDamage;
```

O método [NextDouble()] pode ainda ser usado para determinar o `HP` inicial e
o `AttackPower` dos inimigos. Para o efeito basta multiplicar o valor de
retorno de [NextDouble()] pelo máximo desejado (nunca superior a 100).

A classe [Random] disponibiliza também três versões (_overloads_) do método
[Next()], úteis para obter números inteiros aleatórios:

* `Next()` - Retorna um inteiro aleatório não-negativo.
* `Next(int b)` - Retorna um inteiro aleatório no intervalo `[0, b[`.
* `Next(int a, int b)` - Retorna um inteiro aleatório no intervalo `[a, b[`.

Estes métodos são apropriados para determinar a quantidade inicial de inimigos,
itens e armadilhas, o número de itens deixados para trás por um inimigo quando
morre, bem como a posição inicial do jogador, da saída e do mapa.

O seguinte código exemplifica uma possível forma de criar os inimigos para um
novo nível (atenção que o código é meramente exemplificativo):

```cs
// Quantidade inicial de inimigos no nível
int numberOfEnemies = rnd.Next(maxEnemiesForThisLevel);

// Criar cada um dos inimigos
for (int i = 0; i < numberOfEnemies; i++)
{
    // Determinar HP inicial do inimigo
    double hp = rnd.NextDouble() * maxHPForThisLevel;
    // Determinar AttackPower do inimigo
    double attackPower = rnd.NextDouble() * maxAPForThisLevel;

    // Determinar posição do inimigo
    int row = rnd.Next(numRows); // numRows representa o número de linhas da grelha
    int col = rnd.Next(numCols); // numCols representa o número de colunas da grelha

    // Criar inimigo
    Enemy enemy = new Enemy(hp, attackPower);

    // Adicionar inimigo ao tile escolhido aleatoriamente
    level[row, col].Add(enemy);
}
```

O código anterior assume que as variáveis `maxEnemiesForThisLevel`,
`maxHPForThisLevel` e `maxAPForThisLevel` já existem. Estas variáveis devem ir
aumentando de valor à medida que o jogador vai passando os níveis, ou mais
especificamente, com a dificuldade concreta dos mesmos. A forma mais simples
consiste em usar uma [função][funções] para relacionar a variável desejada
(*y*) com a dificuldade concreta do nível atual (*x*). Algumas funções
apropriadas para o efeito são a [função linear], a
[função linear por troços], a [função logística] ou a
[função logarítmica]. No site disponibilizado através deste [link][funções],
é possível manipular os diferentes parâmetros das várias funções de modo a
visualizar como as mesmas podem relacionar a dificuldade concreta do nível
(*x*) com o valor de saída desejado (*y*). É também [disponibilizada uma classe
_static_](ProcGenFunctions.cs) com as várias funções sugeridas.

<a name="visualize"></a>

### Visualização do jogo

A visualização do jogo deve ser feita em modo de texto (consola).

#### Ecrã principal

O ecrã principal do jogo deve mostrar o seguinte:

* Mapa do jogo, distinguindo claramente a parte explorada da parte inexplorada.
* Estatísticas do jogador: nível atual, _hit points_ (HP), arma selecionada e
  percentagem de ocupação do inventário.
* Em cada _tile_ do mapa explorado devem ser diferenciáveis os vários elementos
  presentes (itens, inimigos, etc), até um máximo razoável. Podem
  inclusivamente existir mais elementos no _tile_ do que aqueles que é possível
  mostrar.
* Uma legenda, explicando o que é cada elemento no mapa.
* Uma ou mais mensagens descrevendo o resultado das ações em que o jogador se
  viu envolvido na última _turn_ no _tile_ atual, como por exemplo ataques de
  inimigos, ataques a inimigos, cair em armadilhas, consumo de comida, equipar
  arma, apanhar itens, etc.
* Indicação das opções disponíveis.

Uma vez que o C# suporta nativamente a representação [Unicode], os respetivos
caracteres podem e devem ser usados para melhorar a visualização do jogo. Para
o efeito deve ser incluída a instrução `Console.OutputEncoding = Encoding.UTF8;`
no método `Main()` (é necessário usar o _namespace_ `System.Text`).

A [Figura 1](#fig1) mostra uma possível implementação da visualização do jogo.
A representação do mundo de jogo em modo de texto impõe um limite prático no
que é possível mostrar na grelha. No entanto tal não significa que o número de
inimigos, armadilhas e itens num _tile_ deva ser limitado, pois todos os
conteúdos de um _tile_ podem ser conferidos com a opção `(L) Look around`.

<a name="fig1"></a>

```
+++++++++++ LP1 Rogue : Level 009 : Difficulty 04 : Size 08x08 ++++++++++++++++

~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~    Player stats
~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~    ------------
                                                   HP        - 34.4
☿.... ☢.... ..... ..... ~~~~~ ~~~~~ ~~~~~ ~~~~~    Weapon    - Rusty Sword
..... ..... ..... ..... ~~~~~ ~~~~~ ~~~~~ ~~~~~    Inventory - 91.7% full

..... ..... ..... ..... ~~~~~ ~~~~~ ~~~~~ ~~~~~
..... ..... ..... ..... ~~~~~ ~~~~~ ~~~~~ ~~~~~

..... ..... ..... ..... ..... †☿... ☿☿☿☢. ~~~~~    Legend
..... ..... ..... ..... ..... ..... ..... ~~~~~    ------
                                                      ⨀ - Player
~~~~~ ☿.... ..... ..... ✚☿... ..... ..... .....   EXIT! - Exit
~~~~~ ..... ..... ..... ..... ..... ..... .....       . - Empty
                                                      ~ - Unexplored
~~~~~ ..... ☢☿✚.. ..... ..... ..... ⨀☿☿†. EXIT!       ⍠ - Map
~~~~~ ..... ..... ..... ..... ..... ..... EXIT!       ☿ - Enemy
                                                      ✚ - Food
~~~~~ ~~~~~ ~~~~~ ~~~~~ ✚☢... ..... ☢⍠... ✚....       † - Weapon
~~~~~ ~~~~~ ~~~~~ ~~~~~ ..... ..... ..... .....       ☢ - Trap

~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~
~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~ ~~~~~

Messages
--------
* You moved WEST
* You were attacked by an enemy and lost 1.3 HP
* You were attacked by an enemy and lost 17.8 HP

Options
-------
↖↑↗        (F) Attack Enemy (E) Pick up item   (U) Use item   (D) Drop item
← → Move   (L) Look around  (H) Help
↙↓↘        (S) Save game    (Q) Quit game

>
```

**Figura 1** - Possível implementação da visualização do jogo para um grelha de
8x8 (ecrã principal).

#### Ecrã de ataque (opção F)

A opção `F` pode ser utilizada quando existem inimigos no mesmo _tile_ do
jogador. Deve ser mostrada uma mensagem de erro quando a opção `F` é
selecionada e não existem inimigos no _tile_ onde o jogador se encontra. Caso
existam inimigos no _tile_, deve ser apresentado um menu semelhante ao indicado
na [Figura 2](#fig2).

<a name="fig2"></a>

```
Select enemy to attack
----------------------

0. Go back
1. Enemy (HP=19.7|AP= 3.0)
2. Enemy (HP=62.1|AP=22.8)

>
```

**Figura 2** - Possível menu para seleção de inimigo a atacar.

#### Ecrã de apanhar/usar/deixar cair item (opções E, U e D)

A opção `E` pode ser utilizada quando existem itens no mesmo _tile_ do jogador.
Deve ser mostrada uma mensagem de erro quando a opção `E` é selecionada e não
existem itens no _tile_ onde o jogador se encontra.

As opções `U` e `D` podem ser utilizadas quando existem itens no inventário do
jogador. Deve ser mostrada uma mensagem de erro quando uma destas opções é
selecionada e não existem itens no inventário do jogador.

Caso existam itens no inventário deve ser apresentado um menu semelhante ao
indicado na [Figura 3](#fig3).

<a name="fig3"></a>

```
Select item to XXXX
-------------------

0. Go back
1. Weapon (Cursed Dagger)
2. Food (Apple)

>
```

**Figura 3** - Possível menu para seleção de item. `XXXX` deve ser substituído
por `pick up`, `use` ou `drop`, dependendo da opção escolhida.

#### Ecrã de olhar em redor (opção L)

Neste ecrã deve ser apresentada uma descrição do que está no _tile_ atual, bem
como nos _tiles_ na respetiva vizinhança de [Moore]. O jogador deve
pressionar ENTER ou qualquer tecla para voltar ao ecrã principal, que deve ser
redesenhado. O uso desta opção **não** gasta uma _turn_. A [Figura 4](#fig4)
mostra um possível ecrã de _look around_.

<a name="fig4"></a>

```
* HERE : Enemy (HP=19.7|AP= 3.0), Enemy(HP=62.1|AP=22.8), Weapon (Shiny Sword)
* ← W  : Empty
* ↖ NW : Empty
* ↑ N  : Empty
* ↗ NE : Empty
* → E  : Exit
* ↘ SE : Food (Water)
* ↓ S  : Trap (Hell Pit), Map
* ↙ SW : Empty
```

**Figura 4** - Possível ecrã de _look around_, mostrando a descrição dos itens
que estão no _tile_ atual e na vizinhança de [Moore] do jogador.

#### Ecrã de ajuda (opção H)

Este ecrã aparece quando é selecionada a opção `H`, mostrando informação sobre
os diferentes itens e armadilhas existentes no jogo. O jogador deve pressionar
ENTER ou qualquer tecla para voltar ao ecrã principal, que deve ser redesenhado.
O uso desta opção **não** gasta uma _turn_. A [Figura 5](#fig5) mostra um
possível ecrã de ajuda (os itens e armadilhas apresentadas são meramente
exemplificativos).

<a name="fig5"></a>

```
Food             HPIncrease      Weight
---------------------------------------
Apple                    +4         0.5
Eggs                     +5         0.6
Fish                    +10         1.0
Meat                    +10         1.2
Water                    +2         0.8

Weapon          AttackPower      Weight     Durability
------------------------------------------------------
Shiny Sword            10.0         3.0           0.90
Rusty Sword            10.0         3.0           0.60
Shiny Dagger            5.0         1.0           0.95
Cursed Dagger          12.0         1.0           0.20
Power Axe              18.0         8.0           0.92
Heavy Mace             16.0         7.0           0.96
Chainsaw               40.0        20.0           0.50

Trap              MaxDamage
---------------------------
Hell Pit                -10
Sharp Spikes            -15
Banana Peel              -3
Bear Trap                -8
Bottomless Chasm        -30
```

**Figura 5** - Possível ecrã de ajuda (os itens e armadilhas apresentadas
são meramente exemplificativos).

#### Ecrã de guardar o jogo (opção S)

Quando a opção `S` é solicitado ao utilizador o nome de ficheiro onde guardar
o jogo. Caso o jogador insira uma _string_ vazia, o jogo não é guardado. Após
ser guardado (ou não), o jogo continua no estado em que estava antes.

Esta funcionalidade faz parte da [Fase Extra](#faseextra), sendo por isso
opcional.

#### Ecrã de terminação do jogo (opção Q)

Quando a opção `Q` é selecionada deve ser apresentada uma pergunta de
confirmação do género `Do you really want to quit? (y/n)`.

## Implementação

<a name="orgclasses"></a>

### Organização do projeto e estrutura de classes

O projeto deve estar devidamente organizado, fazendo uso de classes, _structs_
e enumerações. Cada classe, _struct_ ou enumeração deve ser colocada num
ficheiro com o mesmo nome. Por exemplo, uma classe chamada `Agent` deve ser
colocada no ficheiro `Agent.cs`. A estrutura de classes deve ser bem pensada e
organizada de uma forma lógica, e [cada classe deve ter uma responsabilidade
específica e bem definida][SRP].

<a name="fases"></a>

### Fases da implementação

O jogo deve ser implementado incrementalmente em várias fases. Os projetos
precisam de implementar pelo menos a Fase 1 para serem avaliados. Atenção que a
[geração procedimental/aleatória](#procedural) dos elementos do jogo, bem como
a [visualização](#visualize), são **obrigatórias** em todas as fases de
implementação.

Para fins de avaliação, a fase tida em conta é a anterior à fase mais baixa que
ficou por implementar. Por exemplo, se o grupo implementar tudo até à fase 5
(inclusive), bem como as fases 7 e 9, a fase tida em conta para avaliação é a
fase 5. Ou seja, é vantajoso seguir a ordem sugerida para as fases de
implementação e não "saltar" fases.

#### Fase 1

Na fase 1 devem ser implementados os seguintes pontos:

* Menu principal, com todas as opções a funcionar excepto _High Scores_.
* Opções de linha de comando a funcionar: `-r` e `-c`.
* Jogo:
  * Grelha do jogo, com dimensão especificada nas opções de linha de comandos,
    contém jogador e _Exit_, colocados [aleatoriamente](#procedural) na 1ª e
    última colunas da grelha, respetivamente.
  * Jogador inicia jogo com HP igual a 100.
  * Jogador controlável com o _keypad_, quando chega à _Exit_ termina o nível
    atual, começando um novo nível.
  * Jogador perde 1 HP por cada _turn_.
  * Jogador morre quando HP chega a zero.
  * Implementação da opção `(L) Look around`, que neste caso mostra apenas
    informação sobre a saída quando a mesma está na vizinhança de [Moore] do
    jogador.

A implementação completa desta fase equivale a 50% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 5).

#### Fase 2

Na fase 2 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação das partes exploradas e inexploradas do mapa. As partes
  inexploradas devem ser claramente distinguíveis das partes exploradas.

A implementação completa desta fase equivale a 60% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 6).

#### Fase 3

Na fase 3 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação do mapa e da funcionalidade `(E) Pick up item` apenas para o
  mapa. Quando apanhado, o mapa revela o nível na sua totalidade. O mapa não é
  guardado no inventário, desaparecendo do nível quando apanhado.
* Atualização da opção `(L) Look around` para indicar a presença do mapa.

A implementação completa desta fase equivale a 65% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 6.5).

#### Fase 4

Na fase 4 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação de armadilhas: quando o jogador se move pela primeira vez para
  um _tile_ que contém uma armadilha, perde HP entre 0 e o valor de `MaxDamage`
  da armadilha em questão.
* Implementação da opção `(H) Help`, que apresenta informação acerca dos
  diferentes tipos de armadilha no jogo.
* Atualização da opção `(L) Look around` para descrição de armadilhas.

A implementação completa desta fase equivale a 70% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 7).

#### Fase 5

Na fase 5 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação dos _high scores_ usando ficheiros:
  * Opção _High Scores_ do menu principal permite visualizar os 8 melhores
    _scores_ para a dimensão da grelha especificada na linha de comandos.
  * Quando jogador morre o _score_ é guardado caso esteja entre os 8 melhores
    para a dimensão da grelha especificada na linha de comandos.

A implementação completa desta fase equivale a 75% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 7.5).

#### Fase 6

Na fase 6 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Jogador tem inventário que permite guardar itens até um peso máximo
  pré-determinado.
* Implementação das funcionalidades `(E) Pick up item` e `(D) Drop item` para
  comida e armas. Quando este tipo de itens (comida e armas) são apanhados, são
  guardados no inventário do jogador, caso o mesmo ainda suporte o peso.
* Atualização da opção `(H) Help` de modo a mostrar informação acerca dos
  diferentes tipos de comida e armas existentes no jogo.
* Atualização da opção `(L) Look around` para descrição de comida e armas.

A implementação completa desta fase equivale a 80% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 8).

#### Fase 7

Na fase 7 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação da funcionalidade `(U) Use item`, nomeadamente:
  * O jogador pode consumir itens de comida presentes no seu inventário, e o
    seu `HP` deve aumentar de acordo com a comida consumida, até ao máximo de
    100.
  * O jogador pode equipar uma das armas que tem no seu inventário. A arma
    equipada continua a contar para o peso total do inventário. A arma
    anteriormente equipada é movida para o inventário.

A implementação completa desta fase equivale a 85% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 8.5).

#### Fase 8

Na fase 8 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação de inimigos com as características pedidas. Os inimigos existem
  no jogo e aparecem na visualização, mas não interferem, não atacam o jogador
  e não podem ser atacados.
* Atualização da opção `(L) Look around` para descrição dos inimigos.

A implementação completa desta fase equivale a 90% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 9).

#### Fase 9

Na fase 9 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Combate passivo: o jogador é atacado por inimigos quando se move para _tile_
  onde os mesmos se encontrem.

A implementação completa desta fase equivale a 95% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 9.5).

#### Fase 10

Na fase 10 devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Combate ativo: implementação da opção `(F) Attack Enemy`.

A implementação completa desta fase equivale a 100% de cumprimento do
[objetivo **O1**](#objetivos) (nota máxima 10).

<a name="faseextra"></a>

#### Fase extra

Na fase extra devem ser implementados os seguintes pontos (além dos pontos
indicados nas fases anteriores):

* Implementação de _save games_, com opção extra na linha de comandos para
  _load game_.

A implementação completa desta fase permite compensar eventuais problemas
noutras partes do código e/ou do projeto, facilitando a obtenção da nota
máxima de 10 valores.

<a name="objetivos"></a>

## Objetivos e critério de avaliação

Este projeto tem os seguintes objetivos:

* **O1** - Jogo deve funcionar como especificado (ver [fases](#fases) de
  implementação, obrigatório implementar pelo menos a Fase 1).
* **O2** - Projeto e código bem organizados, nomeadamente:
  * Estrutura de classes bem pensada (ver secção [Organização do projeto e
    estrutura de classes](#orgclasses)).
  * Código devidamente comentado e indentado.
  * Inexistência de código "morto", que não faz nada, como por exemplo
    variáveis, propriedades ou métodos nunca usados.
  * Soluções [simples][KISS] e eficientes.
  * Projeto compila e executa sem erros e/ou *warnings*.
* **O3** - Projeto adequadamente documentado com
  [comentários de documentação XML][XML]. A documentação gerada em formato HTML
  ou CHM com [Doxygen], [Sandcastle] ou ferramenta similar \[[4][ref4]\], deve
  estar incluída no ZIP do projeto, mas **não** integrada no repositório Git.
* **O4** - Repositório Git deve refletir boa utilização do mesmo, com *commits*
  de todos os elementos do grupo e mensagens de *commit* que sigam as melhores
  práticas para o efeito (como indicado
  [aqui](https://chris.beams.io/posts/git-commit/),
  [aqui](https://gist.github.com/robertpainsi/b632364184e70900af4ab688decf6f53),
  [aqui](https://github.com/erlang/otp/wiki/writing-good-commit-messages) e
  [aqui](https://stackoverflow.com/questions/2290016/git-commit-messages-50-72-formatting)).
  Quaisquer *assets* binários, tais como imagens para uso no relatório,
  por exemplo, devem ser integrados no repositório em modo Git LFS.
* **O5** - Relatório em formato [Markdown] (ficheiro `README.md`), organizado
  da seguinte forma:
  * Título do projeto.
  * Nome dos autores (primeiro e último) e respetivos números de aluno.
  * Indicação do repositório público Git utilizado. Esta indicação é opcional,
    pois podem preferir desenvolver o projeto num repositório privado.
  * Informação de quem fez o quê no projeto. Esta informação é **obrigatória**
    e deve refletir os *commits* feitos no Git.
  * Descrição da solução:
    * Fase implementada (1 a 10, ou extra).
    * Arquitetura da solução, com breve explicação de como o programa foi
      organizado e indicação das estruturas de dados usadas (para o inventário
      e para a grelha de jogo, por exemplo), bem como os algoritmos
      implementados (para desenhar o mapa e para geração procedimental, por
      exemplo).
    * Um diagrama UML de classes simples (i.e., sem indicação dos membros da
      classe) descrevendo a estrutura de classes.
    * Um fluxograma mostrando o _game loop_.
    * Conclusões e matéria aprendida.
    * Referências, incluindo trocas de ideias com colegas, código aberto
      reutilizado ou no qual se basearam (e.g., do StackOverflow ou do GitHub)
      e bibliotecas de terceiros utilizadas. Devem ser o mais detalhados
      possível.
  * Nota adicionais sobre o relatório:
    * O relatório deve ser simples e breve, com informação mínima e suficiente
      para que seja possível ter uma boa ideia do que foi feito.
    * Atenção aos erros ortográficos, pois serão tidos em conta na nota final.
    * Atenção à formatação [Markdown], pois será tida em conta na nota final.
    * Se usarem o [Visual Studio Code] para fazer o relatório, façam uso da
      [extensão de correção ortográfica][VSCodeSpellCheck]
      (e o seu dicionário de [Português][VSCodeSpellCheckPT]) e [extensões para
      edição de Markdown][VSCodeMarkdown].

O projeto tem um peso de 10 valores na nota final da disciplina e será avaliado
de forma qualitativa. Isto significa que todos os objetivos têm de ser
parcialmente ou totalmente cumpridos. A cada objetivo, **O1** a **O5**, será
atribuída uma nota entre 0 e 1. A nota do projeto será dada pela seguinte
fórmula:

`N = 10 x O1 x O2 x O3 x O4 x O5 x D`

Em que *D* corresponde à nota da discussão e percentagem equitativa de
realização do projeto, também entre 0 e 1. Isto significa que se os alunos
ignorarem completamente um dos objetivos, não tenham feito nada no projeto ou
não comparecerem na discussão, a nota final será zero.

A nota mínima neste projeto para aprovação na componente prática de LP1 é de
4,5 valores.

## Entrega

O projeto deve ser entregue por **grupos de 1 a 3 alunos** via Moodle até às
**23h de 7 de julho de 2019**. Deve ser submetido um ficheiro `zip` com os
seguintes conteúdos:

* Solução ou projeto .NET Core (console app) com implementação do jogo.
* Pasta escondida `.git` com o repositório Git local do projeto.
* Documentação HTML ou CHM gerada com [Doxygen], [Sandcastle] ou ferramenta
  similar \[[4][ref4]\].
* Ficheiro `README.md` contendo o relatório do projeto em formato [Markdown].
* Ficheiros de imagem contendo o fluxograma e o diagrama UML de classes.
  Estes ficheiros podem ser incluídos no repositório em modo Git LFS.

## Honestidade académica

Nesta disciplina, espera-se que cada aluno siga os mais altos padrões de
honestidade académica. Isto significa que cada ideia que não seja do
aluno deve ser claramente indicada, com devida referência ao respetivo
autor. O não cumprimento desta regra constitui plágio.

O plágio inclui a utilização de ideias, código ou conjuntos de soluções
de outros alunos ou indivíduos, ou quaisquer outras fontes para além
dos textos de apoio à disciplina, sem dar o respetivo crédito a essas
fontes. Os alunos são encorajados a discutir os problemas com outros
alunos e devem mencionar essa discussão quando submetem os projetos.
Essa menção **não** influenciará a nota. Os alunos não deverão, no
entanto, copiar códigos, documentação e relatórios de outros alunos, ou dar os
seus próprios códigos, documentação e relatórios a outros em qualquer
circunstância. De facto, não devem sequer deixar códigos, documentação e
relatórios em computadores de uso partilhado.

Nesta disciplina, a desonestidade académica é considerada fraude, com
todas as consequências legais que daí advêm. Qualquer fraude terá como
consequência imediata a anulação dos projetos de todos os alunos envolvidos
(incluindo os que possibilitaram a ocorrência). Qualquer suspeita de
desonestidade académica será relatada aos órgãos superiores da escola
para possível instauração de um processo disciplinar. Este poderá
resultar em reprovação à disciplina, reprovação de ano ou mesmo suspensão
temporária ou definitiva da ULHT.

*Texto adaptado da disciplina de [Algoritmos e
Estruturas de Dados][aed] do [Instituto Superior Técnico][ist]*

## Notas

<sup><a name="fn1">1</a></sup> Os argumentos da linha de comandos estão
disponíveis num _array_ de _strings_ chamado `args` no método `Main()`, como
explicado nas páginas 285 e 286 do livro da disciplina.

## Referências

* <a name="ref1">\[1\]</a> Whitaker, R. B. (2016). **The C# Player's Guide**
  (3rd Edition). Starbound Software.
* <a name="ref2">\[2\]</a> Albahari, J. (2017). **C# 7.0 in a Nutshell**.
  O’Reilly Media.
* <a name="ref3">\[3\]</a> Dorsey, T. (2017). **Doing Visual Studio and .NET
  Code Documentation Right**. Visual Studio Magazine. Retrieved from
  <https://visualstudiomagazine.com/articles/2017/02/21/vs-dotnet-code-documentation-tools-roundup.aspx>.
* <a name="ref4">\[4\]</a> Procedural generation. (2018). Retrived May 25, 2018
from https://en.wikipedia.org/wiki/Procedural_generation.


## Licenças

Este enunciado é disponibilizado através da licença [CC BY-NC-SA 4.0].

## Metadados

* Autor: [Nuno Fachada]
* Curso:  [Licenciatura em Videojogos][lamv]
* Instituição: [Universidade Lusófona de Humanidades e Tecnologias][ULHT]

[ref1]:#ref1
[ref2]:#ref2
[ref3]:#ref3
[ref4]:#ref4
[CC BY-NC-SA 4.0]:https://creativecommons.org/licenses/by-nc-sa/4.0/
[GPLv3]:https://www.gnu.org/licenses/gpl-3.0.en.html
[lamv]:https://www.ulusofona.pt/licenciatura/videojogos
[Nuno Fachada]:https://github.com/fakenmc
[ULHT]:https://www.ulusofona.pt/
[aed]:https://fenix.tecnico.ulisboa.pt/disciplinas/AED-2/2009-2010/2-semestre/honestidade-academica
[ist]:https://tecnico.ulisboa.pt/pt/
[Markdown]:https://guides.github.com/features/mastering-markdown/
[Doxygen]:https://www.stack.nl/~dimitri/doxygen/
[Sandcastle]:https://github.com/EWSoftware/SHFB
[KISS]:https://en.wikipedia.org/wiki/KISS_principle
[XML]:https://docs.microsoft.com/dotnet/csharp/codedoc
[SRP]:https://en.wikipedia.org/wiki/Single_responsibility_principle
[KISS]:https://en.wikipedia.org/wiki/KISS_principle
[GP]:https://en.wikipedia.org/wiki/Procedural_generation
[`Console`]:https://docs.microsoft.com/dotnet/api/system.console
[Unicode]:https://unicode-table.com/
[Visual Studio Code]:https://code.visualstudio.com/
[VSCodeMarkdown]:https://code.visualstudio.com/docs/languages/markdown
[VSCodeSpellCheck]:https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker
[VSCodeSpellCheckPT]:https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker-portuguese
[Moore]:https://en.wikipedia.org/wiki/Moore_neighborhood
[`Thread.Sleep()`]:https://docs.microsoft.com/dotnet/api/system.threading.thread.sleep
[UTF-8]:https://en.wikipedia.org/wiki/UTF-8
[Unicode]:https://en.wikipedia.org/wiki/Unicode
[Random]:https://docs.microsoft.com/dotnet/api/system.random
[NextDouble()]:https://docs.microsoft.com/dotnet/api/system.random.nextdouble
[Next()]:https://docs.microsoft.com/dotnet/api/system.random.next
[função logística]:https://en.wikipedia.org/wiki/Logistic_function
[função linear por troços]:https://en.wikipedia.org/wiki/Piecewise_linear_function
[função logarítmica]:https://en.wikipedia.org/wiki/Logarithm#Logarithmic_function
[função linear]:https://en.wikipedia.org/wiki/Linear_function_(calculus)
[funções]:https://www.desmos.com/calculator/aavgzb9e5q
