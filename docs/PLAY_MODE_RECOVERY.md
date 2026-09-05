# Play Mode recovery

Se a Unity não entrar em Play após atualizar o projeto:

1. aguarde o status `Compiling...` / `Importing...` terminar;
2. abra `Window > General > Console`;
3. corrija primeiro qualquer erro vermelho de compilação;
4. os packs CC0 não são mais baixados automaticamente durante a abertura do Editor;
5. para instalar ou atualizar arte externa, use manualmente `Pythime > Install or Update CC0 Art Packs`;
6. mesmo sem os packs externos, o jogo deve abrir com o fallback visual procedural.

O instalador de arte é deliberadamente manual para nunca bloquear Play Mode por download, importação ou falha de rede.
