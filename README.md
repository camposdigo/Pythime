# Pythime

Pythime é um jogo 2D top-down em Unity sobre exploração, viagem no tempo e programação como ferramenta de gameplay.

## Conceito

O jogador explora a mesma cidade em épocas diferentes. Mudanças feitas no passado alteram objetos, caminhos e situações no presente e no futuro. A máquina temporal também se adapta visualmente à época.

A direção visual é pixel-art cartunesca, legível e charmosa, inspirada na clareza de jogos top-down 2D clássicos. O objetivo é ter personagens compactos, cabeça grande, poucos tons por asset, falsa profundidade e leitura rápida, sem copiar personagens, mapas ou sprites proprietários de outras obras.

## Capítulo 1 — O Eco Impossível

Pythime foi rasgada em três versões temporais — 1956, 2026 e 2096 — e Tock, o companheiro temporal do jogador, guia a investigação sobre o que destruiu o Chrono Core.

O primeiro capítulo inclui:

- introdução com diálogos;
- Tock acompanhando o jogador pelo mapa;
- objetivo principal e localização atual na HUD;
- cidade grande com distritos, parque, estação, praça, oficina e bairros residenciais;
- máquina temporal retrofuturista com visual diferente por época;
- primeira cadeia de missão temporal;
- consequência de uma alteração feita em 1956;
- investigação de uma anomalia em 2096;
- gancho narrativo para um segundo viajante do tempo.

## Personagem e roupas

O personagem agora tem editor durante o jogo. Pressione `TAB` para abrir.

É possível editar:

- tom de pele;
- formato do cabelo;
- cor do cabelo;
- camisa;
- jaqueta/colete;
- calça;
- calçado;
- acessório;
- presets completos.

O preset **Time Traveler 85** usa colete vermelho, camisa clara, jeans e tênis branco como homenagem ao visual de aventura temporal dos anos 1980, sem reproduzir literalmente um personagem protegido.

A animação passou a usar deslocamento real do personagem. Se ele estiver encostado em uma colisão e não sair do lugar, o sprite não deve continuar correndo parado.

## Controles

| Tecla | Ação |
| --- | --- |
| WASD / Setas | mover |
| Q / E | época anterior / próxima |
| TAB | abrir/fechar editor do personagem |
| T | ação temporal no canteiro em 1956 |
| F | interagir / inspecionar |
| P | abrir/fechar PyTerminal |
| Espaço / Enter / X | avançar diálogo |
| Esc | pular conversa inteira |

## Arte e Tilemap

O projeto usa o sistema de **Grid + Tilemap** da Unity para adicionar detalhes urbanos reais sobre a cidade. Quando o Kenney RPG Urban Pack está instalado, `KenneyTilemapOverlay` adiciona tiles CC0 de árvores, carros e mobiliário urbano às três épocas. Se o pack ainda não estiver disponível, o jogo continua usando o fallback procedural.

A Unity baixa automaticamente os packs de desenvolvimento no editor. Também é possível executar:

`Pythime > Install or Update CC0 Art Packs`

Packs usados como base/referência:

- Kenney RPG Urban Pack;
- Kenney Roguelike Modern City;
- Kenney Roguelike Indoors.

Os downloads ficam fora do Git e são CC0. Consulte `docs/THIRD_PARTY_ART.md`.

## PyTerminal

Comandos disponíveis no protótipo:

```python
timeline.travel(1956)
timeline.travel(2026)
timeline.travel(2096)
player.speed(7)
world.plant_seed()
print(timeline.year)
help()
```

O terminal atual é um parser seguro de comandos Python-like para validar a mecânica. Ele ainda não executa Python arbitrário.

## Como testar

```bash
git pull
```

Abra o projeto no Unity 6000.5.7f1, espere a importação dos packs terminar, abra `Assets/Scenes/SampleScene.unity` e aperte Play.

O protótipo é criado em runtime. Não é necessário montar Player, câmera ou eras manualmente.

## Próximos passos

1. substituir mais fachadas procedurais por Tilemaps e prefabs editáveis;
2. criar interiores exploráveis com o pack Indoors;
3. adicionar NPCs com rotinas diferentes em cada época;
4. adicionar inventário e objetos persistentes entre eras;
5. construir puzzles temporais que modifiquem partes inteiras da cidade;
6. evoluir PyTerminal para desafios de Python reais em ambiente controlado;
7. adicionar save/load de timeline, escolhas e customização;
8. produzir capítulos seguintes da história.
