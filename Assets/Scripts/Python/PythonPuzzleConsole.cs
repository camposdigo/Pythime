using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class PythonPuzzleConsole : MonoBehaviour
    {
        private bool visible;
        private string command = "timeline.travel(1956)";
        private string output = "Digite help() para ver os comandos do protótipo.";
        private Rect windowRect = new(24, 170, 520, 250);

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.pKey.wasPressedThisFrame)
                visible = !visible;
        }

        private void OnGUI()
        {
            if (!visible) return;
            windowRect = GUI.Window(9137, windowRect, DrawWindow, "PyTerminal — protótipo");
        }

        private void DrawWindow(int id)
        {
            GUI.Label(new Rect(18, 34, 480, 38), "Python como ferramenta do mundo, não como tela de aula.");
            command = GUI.TextField(new Rect(18, 76, 484, 34), command);

            if (GUI.Button(new Rect(18, 120, 112, 34), "Executar"))
                ExecuteCommand();

            if (GUI.Button(new Rect(140, 120, 112, 34), "help()"))
            {
                command = "help()";
                ExecuteCommand();
            }

            GUI.Box(new Rect(18, 166, 484, 60), output);
            GUI.DragWindow(new Rect(0, 0, 520, 28));
        }

        private void ExecuteCommand()
        {
            var text = command.Trim();
            var timeline = TimeTravelManager.Instance;

            if (text == "help()")
            {
                output = "timeline.travel(1956) | player.speed(7) | world.plant_seed() | print(timeline.year)";
                return;
            }

            if (text == "print(timeline.year)")
            {
                output = timeline == null ? "timeline indisponível" : timeline.CurrentYear.ToString();
                return;
            }

            if (TryReadArgument(text, "timeline.travel", out var yearText) && int.TryParse(yearText, out var year))
            {
                output = timeline != null && timeline.TravelToYear(year)
                    ? $"Linha temporal alterada para {year}."
                    : "Ano indisponível. Use 1956, 2026 ou 2096.";
                return;
            }

            if (TryReadArgument(text, "player.speed", out var speedText) &&
                float.TryParse(speedText, NumberStyles.Float, CultureInfo.InvariantCulture, out var speed))
            {
                var player = FindFirstObjectByType<PlayerController>();
                if (player == null)
                {
                    output = "player não encontrado";
                    return;
                }

                player.MoveSpeed = speed;
                output = $"Velocidade do jogador: {player.MoveSpeed:0.0}";
                return;
            }

            if (text == "world.plant_seed()")
            {
                output = timeline != null && timeline.PlantSeedFromConsole()
                    ? "Semente inserida em 1956. Observe 2026 e 2096."
                    : "Não foi possível alterar a timeline.";
                return;
            }

            output = "Comando não reconhecido. Execute help().";
        }

        private static bool TryReadArgument(string text, string functionName, out string argument)
        {
            argument = string.Empty;
            var prefix = functionName + "(";
            if (!text.StartsWith(prefix) || !text.EndsWith(")")) return false;
            argument = text.Substring(prefix.Length, text.Length - prefix.Length - 1).Trim();
            return true;
        }
    }
}
