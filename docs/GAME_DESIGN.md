# Pythime — Game Design Base

## Fantasia principal

Explorar um mundo 2D cartunesco que existe em várias épocas e usar pequenas ações no passado para transformar fisicamente o presente e o futuro.

## Pilares

### 1. Exploração

O jogador deve ter motivos para andar pelo mapa mesmo fora da missão principal: interiores, NPCs, colecionáveis, objetos esquecidos, caminhos que só existem em certas épocas e pequenas histórias que mudam com o tempo.

### 2. Viagem no tempo

Cada região importante existe em versões históricas diferentes. O jogador alterna entre elas rapidamente, preservando posição quando possível, para que a própria troca de era vire ferramenta de puzzle e exploração.

### 3. Consequência temporal

Ações do passado deixam marcas no futuro. Exemplos: plantar algo, esconder um item, impedir uma construção, alterar uma máquina, entregar tecnologia cedo demais ou mudar a trajetória de um NPC.

### 4. Python como ferramenta

Programação não deve parecer aula separada. O personagem usa um dispositivo temporal capaz de conversar com sistemas, robôs, portas e equipamentos. Os desafios começam com comandos simples e podem evoluir para lógica, listas, loops, funções e análise de dados.

### 5. Customização

O personagem é editável por camadas. A identidade visual escolhida pelo jogador deve continuar reconhecível quando as roupas forem reinterpretadas para outras épocas.

## Direção visual

- 2D top-down;
- personagens pequenos, cartunescos e muito legíveis;
- proporções simpáticas e animações expressivas;
- mapas coloridos com leitura rápida de portas, ruas, objetos e NPCs;
- cada era possui paleta própria;
- efeitos de viagem no tempo mais modernos que o restante do cenário;
- nada de pixel art excessivamente genérica: a meta é ter silhuetas e objetos próprios do Pythime.

## Épocas iniciais

### 1956

Arquitetura baixa, materiais quentes, placas pintadas, mecânica visível e tecnologia limitada.

### 2026

Ponto de referência do jogador. Cidade contemporânea, mais familiar e equilibrada visualmente.

### 2096

Arquitetura reconstruída, elementos flutuantes, luzes frias e objetos que mostram consequências acumuladas das outras eras.

## Máquina temporal adaptativa

A mesma tecnologia assume uma aparência apropriada à era. A referência cultural serve apenas como linguagem visual ou piada para o jogador; o veículo e os objetos precisam ter design original.

Exemplos de arquétipos:

- carro temporal retrofuturista em uma era automobilística;
- veículo mecânico em uma época menos tecnológica;
- cápsula flutuante em um futuro distante.

## Referências culturais

Pythime pode brincar com cinema, ficção científica, jogos e cultura pop por meio de homenagens e situações reconhecíveis, sem reproduzir personagens, logos, modelos, falas, trilhas ou objetos protegidos de forma idêntica.

Um objeto tecnológico deixado cedo demais no passado, por exemplo, pode virar um artefato escuro, minimalista e quase sagrado para aquela sociedade, transmitindo a sensação de tecnologia incompreensível sem copiar um objeto específico de outra obra.

## Primeiro vertical slice

O primeiro recorte jogável deve provar quatro coisas:

1. explorar uma pequena cidade é agradável;
2. viajar entre 1956, 2026 e 2096 muda o mapa de forma clara;
3. uma ação em 1956 produz consequência visível nas épocas seguintes;
4. um comando no PyTerminal altera algo real no mundo.

O protótipo atual usa a semente temporal para validar exatamente esse fluxo.
