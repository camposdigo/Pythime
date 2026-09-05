# Pythime

Pythime é um jogo 2D top-down em Unity sobre exploração, viagem no tempo e programação como ferramenta de gameplay.

## Conceito

O jogador explora o mesmo lugar em épocas diferentes. Mudanças feitas no passado alteram objetos, caminhos e situações no presente e no futuro. A máquina temporal também se adapta visualmente à época.

A direção visual pretendida é cartunesca, legível e charmosa, inspirada na clareza de jogos top-down 2D, sem copiar personagens, cenários ou assets de outras obras.

## Protótipo atual

O protótipo é criado em runtime ao apertar Play. Não é necessário montar Player, câmera ou eras manualmente na SampleScene.

Inclui:

- movimento top-down com colisão;
- câmera suave seguindo o jogador;
- três épocas: 1956, 2026 e 2096;
- troca instantânea de timeline;
- cidade simples com diferenças visuais entre épocas;
- veículo temporal adaptado a cada era;
- consequência temporal demonstrável com uma semente plantada em 1956;
- customização provisória do personagem;
- PyTerminal com comandos de sintaxe Python para testar a ideia de programação integrada ao mundo.

## Controles

| Tecla | Ação |
| --- | --- |
| WASD / Setas | mover |
| Q / E | época anterior / próxima |
| C | trocar cor da roupa |
| H | trocar cabelo |
| T | plantar/remover a semente no canteiro em 1956 |
| P | abrir/fechar PyTerminal |

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

## Como testar esta branch

```bash
git checkout feature/playable-prototype
git pull
```

Abra o projeto no Unity 6000.5.7f1, abra `Assets/Scenes/SampleScene.unity` e aperte Play.

## Próximos passos

1. substituir formas provisórias por sprites originais e animações;
2. criar Character Creator completo por camadas;
3. transformar as três épocas em mapas Tilemap reais;
4. adicionar NPCs e interiores exploráveis;
5. adicionar inventário e objetos persistentes entre eras;
6. construir puzzles temporais maiores;
7. evoluir PyTerminal para desafios de Python reais em ambiente controlado;
8. adicionar save/load de timeline e customização.
