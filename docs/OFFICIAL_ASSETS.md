# Integração dos assets oficiais

O PNG antigo tinha 13.130 bytes e assinatura PNG correta, mas seu bloco IDAT
estava truncado e a descompressão falhava. A cópia enviada pelo usuário tem
1.049.475 bytes, 1254 × 1254 pixels RGB e passa na validação de assinatura,
estrutura, CRCs e decodificação completa. O arquivo original enviado foi
preservado; o tratamento do fundo ocorre apenas na textura em memória.

## Personagem

`OfficialPlayerAnimator` é criado diretamente pelo bootstrap no Player. Não
há `PlayerVisual`, customizador ou correção de olhos alterando seu renderer.
Uma falha no PNG produz um erro com caminho, existência, tamanho, assinatura
e motivo; não troca silenciosamente para um personagem procedural.

O arquivo enviado tem fundo branco, margens largas e espaçamento irregular.
O animador remove somente o branco conectado às bordas, preservando roupas
brancas delimitadas pelo contorno, e detecta as cinco faixas e quatro frames
por faixa. Os sprites são criados em runtime, com pivot junto aos pés e PPU
calculado para altura máxima de 2,1 unidades, independente da resolução.

Linhas, de cima para baixo: idle, baixo, cima, esquerda, direita. No idle,
as colunas representam baixo, cima, esquerda e direita. A arte recebida
repete a pose de esquerda na quarta coluna: o idle direito espelha a terceira
coluna. A caminhada direita usa sua própria quinta linha sem espelhamento.
`PlayerController.MoveInput` determina a direção e a animação de caminhada.

## Mapas e colisões

As três épocas são construídas antes de desativar as épocas fora de uso,
sem procurar objetos inativos via `GameObject.Find`. Quando os três mapas
estão disponíveis, os geradores de praça, decoração, NPCs e interiores
procedurais não iniciam. A rotina antiga de limpeza e reposicionamento foi
removida. Os objetos de estado da semente temporal permanecem, sem desenhar
árvores procedurais por cima da imagem oficial.

Os mapas ocupam 64 × 64 unidades. `OfficialEraMapRuntime.MapPoint` converte
coordenadas normalizadas da imagem, com origem no canto superior esquerdo,
para o mundo. As colisões são filhos de
`Era_<ano>/PythimeOfficialMapColliders`, com prefixo `Collider_` e nomes
descritivos. 1956 tem uma disposição própria; 2026 e 2096 compartilham a
estrutura das ruas, prédios e jardins. Em 1956, o memorial ocupa o lugar
equivalente ao da fonte.

As colisões são aproximações jogáveis dos prédios, fonte/memorial, canteiros,
troncos e bases dos postes. A câmera segue suavemente e limita toda a área
visível aos limites do mapa, considerando a proporção da tela. Se uma troca
de época colocar o jogador dentro de um obstáculo, ele é movido para a
posição livre mais próxima encontrada. Se a posição estiver livre, ela é
mantida. O minimapa mostra a imagem da época atual com os mesmos limites.

## Importação

`OfficialAssetPostprocessor` configura os quatro assets automaticamente:
Sprite/Single, Point, sem compressão, sem mipmaps, sem redimensionamento NPOT.
Player: Read/Write ativo e Max Size 2048. Mapas: Read/Write desativado e
Max Size 4096. Overrides comuns de plataforma são removidos para manter
essas configurações. O menu **Pythime > Reimport Official Assets** permite
forçar a reimportação e executa a validação dos arquivos.

## Testar no Unity 6000.5.7f1

1. Saia do Play, aguarde a recompilação e abra `Assets/Scenes/SampleScene.unity`.
2. Execute **Pythime > Reimport Official Assets**. Limpe mensagens antigas do
   Console, inclusive as do arquivo corrompido anterior.
3. Entre em Play. Confira o personagem oficial sem fundo branco, o mapa de
   2026 e a ausência da praça/interior procedural. O spawn deve estar livre.
4. Use WASD ou setas nas quatro direções. Ao soltar, a pose idle deve manter
   a direção. Espaço/Enter avançam as conversas; Esc pula a conversa atual.
5. Use Q/E para percorrer 1956, 2026 e 2096. Confira as três imagens e o
   minimapa. Tente trocar de época em uma área cujo prédio muda de posição.
6. Caminhe pela praça e ruas laterais. Tente atravessar o prédio central,
   fonte/memorial, árvores, canteiros e bases de postes; devem bloquear.
7. Aproxime-se das bordas: a câmera não deve mostrar o exterior do mapa.
8. Na Game View, confira 1920 × 1080 e 2560 × 1440: épocas no topo, missão à
   direita, minimapa abaixo da missão e diálogo na parte inferior.

## Validação automatizada

`Pythime.EditorTools.OfficialIntegrationValidation.RunBatch` executa uma
verificação em Play em uma **cópia descartável** do projeto. Não execute o
batch contra o projeto aberto. Exemplo, após copiar Assets, Packages e
ProjectSettings para uma pasta temporária:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.5.7f1\Editor\Unity.exe' `
  -batchmode -projectPath 'C:\caminho\copia-temporaria' `
  -executeMethod Pythime.EditorTools.OfficialIntegrationValidation.RunBatch `
  -screen-width 1920 -screen-height 1080 -logFile validation.log
```

O processo encerra com código 0 ao passar e 1 ao falhar, escrevendo
`Logs/official-validation.txt`. Verifica importação, rejeição de PNG
corrompido, frames/direções, WASD, Q/E, spawn, obstáculos, conectividade de
rotas e câmera em 16:9, 21:9 e 4:3. A inspeção visual complementa essas
asserções, especialmente nos contornos aproximados das colisões.
