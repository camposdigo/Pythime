# Pythime

Pythime é um jogo 2D top-down em Unity sobre exploração, viagem no tempo e programação como ferramenta de gameplay.

## Conceito

O jogador explora a mesma cidade em épocas diferentes. Mudanças feitas no passado alteram objetos, caminhos e situações no presente e no futuro. A máquina temporal também se adapta visualmente à época.

A direção visual é pixel-art cartunesca, legível e charmosa, inspirada na clareza de jogos top-down 2D, sem copiar personagens, cenários ou assets proprietários de outras obras.

## Capítulo 1 — O Eco Impossível

O protótipo agora começa como uma pequena campanha jogável. Pythime foi rasgada em três versões temporais — 1956, 2026 e 2096 — e Tock, o companheiro temporal do jogador, guia a investigação sobre o que destruiu o Chrono Core.

O primeiro capítulo inclui:

- introdução com diálogos;
- Tock acompanhando o jogador pelo mapa;
- objetivo principal e localização atual na HUD;
- mapa aproximadamente quatro vezes maior que a primeira versão;
- distritos, parque, estação, praça central, oficina e bairros residenciais;
- máquina temporal retrofuturista com visual diferente por época;
- primeira cadeia de missão temporal;
- consequência de uma alteração feita em 1956;
- investigação de uma anomalia em 2096;
- gancho narrativo para um segundo viajante do tempo.

## Controles

| Tecla | Ação |
| --- | --- |
| WASD / Setas | mover |
| Q / E | época anterior / próxima |
| C | trocar cor da roupa |
| H | trocar cabelo |
| T | ação temporal no canteiro em 1956 |
| F | interagir / inspecionar |
| P | abrir/fechar PyTerminal |
| Espaço / Enter | avançar diálogos |

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

O terminal atual é um parser seguro de comandos Python-like para validar a mecânica. Ele ainda não executa Python arbitrário. Uma futura versão pode usar uma sandbox própria para desafios reais.

## Como testar

```bash
git pull
```

Abra o projeto no Unity 6000.5.7f1, abra `Assets/Scenes/SampleScene.unity` e aperte Play.

O protótipo é criado em runtime. Não é necessário montar Player, câmera, mapas ou eras manualmente.

## Arte externa

O projeto mantém suporte ao Kenney RPG Urban Pack (CC0) para referências e assets urbanos livres. Consulte `docs/THIRD_PARTY_ART.md`.

## Próximos passos

1. transformar os distritos em Tilemaps editáveis e interiores exploráveis;
2. criar Character Creator completo por camadas;
3. adicionar NPCs com rotinas diferentes em cada época;
4. adicionar inventário e objetos persistentes entre eras;
5. construir puzzles temporais que modifiquem partes inteiras da cidade;
6. evoluir PyTerminal para desafios de Python reais em ambiente controlado;
7. adicionar save/load de timeline, escolhas e customização;
8. produzir capítulos seguintes da história.
