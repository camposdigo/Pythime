using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class StoryDirector : MonoBehaviour
    {
        private Transform player;
        private PlayerController controller;
        private Rigidbody2D playerBody;
        private Vector2 workshopPoint;
        private Vector2 soilPoint;
        private Vector2 monolithPoint;

        private int chapterStage;
        private bool dialogueOpen;
        private string[] dialogueLines = Array.Empty<string>();
        private int dialogueIndex;
        private Action dialogueComplete;
        private string objective = string.Empty;
        private string locationName = "Bairro Sul";

        private GUIStyle dialogueNameStyle;
        private GUIStyle dialogueStyle;
        private GUIStyle objectiveTitleStyle;
        private GUIStyle objectiveStyle;
        private GUIStyle locationStyle;

        public string Objective => objective;
        public bool ChapterComplete => chapterStage >= 5;
        public bool DialogueOpen => dialogueOpen;

        public void Initialize(Transform playerTransform, Vector2 workshop, Vector2 soil, Vector2 monolith)
        {
            player = playerTransform;
            controller = player != null ? player.GetComponent<PlayerController>() : null;
            playerBody = player != null ? player.GetComponent<Rigidbody2D>() : null;
            workshopPoint = workshop;
            soilPoint = soil;
            monolithPoint = monolith;
            chapterStage = 0;

            BeginDialogue(new[]
            {
                "TOCK|Ei! Você tá me ouvindo? Ótimo. Isso já é melhor do que há trinta segundos.",
                "TOCK|O impacto rasgou Pythime em três versões da mesma cidade.",
                "TOCK|1956. 2026. 2096. Todas existem ao mesmo tempo e estão vazando umas nas outras.",
                "TOCK|Sua máquina sobreviveu, mais ou menos. Mas o Chrono Core não.",
                "TOCK|A Oficina Temporal, no extremo nordeste, pode ter um registro do que aconteceu. Vamos até lá."
            }, () =>
            {
                chapterStage = 1;
                objective = "Chegue à Oficina Temporal no extremo nordeste de Pythime.";
            });
        }

        private void Update()
        {
            if (player == null || TimeTravelManager.Instance == null) return;

            UpdateLocationName();

            var keyboard = Keyboard.current;
            if (dialogueOpen)
            {
                if (keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame))
                    AdvanceDialogue();
                return;
            }

            var timeline = TimeTravelManager.Instance;

            if (chapterStage == 1)
            {
                if (timeline.CurrentYear == 2026 && Vector2.Distance(player.position, workshopPoint) < 2.6f)
                {
                    BeginDialogue(new[]
                    {
                        "TOCK|Achei o registro da Oficina. O Chrono Core não explodiu por acidente.",
                        "TOCK|Alguém ativou uma âncora temporal décadas antes de ela existir.",
                        "TOCK|O primeiro eco aparece em 1956, onde hoje fica a Praça do Relógio.",
                        "TOCK|Volte para 1956. Procure o canteiro no parque a oeste da praça."
                    }, () =>
                    {
                        chapterStage = 2;
                        objective = "Viaje para 1956 e investigue o canteiro temporal no parque.";
                    });
                }
            }
            else if (chapterStage == 2)
            {
                if (timeline.SeedPlanted)
                {
                    BeginDialogue(new[]
                    {
                        "TOCK|Você sentiu isso? A linha temporal acabou de se reorganizar.",
                        "TOCK|Uma mudança pequena em 1956 virou décadas de crescimento acumulado.",
                        "TOCK|Agora precisamos ver o resultado final. Vá para 2096."
                    }, () =>
                    {
                        chapterStage = 3;
                        objective = "Viaje para 2096 e investigue o futuro alterado.";
                    });
                }
                else if (timeline.CurrentYear == 1956)
                {
                    objective = Vector2.Distance(player.position, soilPoint) < 2.1f
                        ? "Pressione T perto do canteiro para criar uma alteração na timeline."
                        : "Encontre o canteiro temporal no parque, a oeste da Praça do Relógio.";
                }
                else
                {
                    objective = "Viaje para 1956 usando Q/E e investigue a Praça do Relógio.";
                }
            }
            else if (chapterStage == 3)
            {
                if (timeline.CurrentYear != 2096)
                {
                    objective = "Viaje para 2096 e procure a origem da anomalia.";
                }
                else
                {
                    objective = Vector2.Distance(player.position, monolithPoint) < 2.4f
                        ? "Pressione F para inspecionar o monólito temporal."
                        : "Siga para o monólito ao nordeste da Praça do Relógio.";

                    if (keyboard != null && keyboard.fKey.wasPressedThisFrame && Vector2.Distance(player.position, monolithPoint) < 2.4f)
                    {
                        BeginDialogue(new[]
                        {
                            "TOCK|Isso não estava aqui antes.",
                            "TOCK|O material não pertence a 2096. Nem a 1956. Nem a 2026.",
                            "TOCK|Tem uma assinatura do Chrono Core aqui dentro... mas existe uma segunda assinatura junto dela.",
                            "TOCK|Alguém mais está viajando no tempo.",
                            "TOCK|E pelo jeito essa pessoa sabia que você viria até Pythime."
                        }, () =>
                        {
                            chapterStage = 5;
                            objective = "CAPÍTULO 1 CONCLUÍDO — O Eco Impossível";
                        });
                    }
                }
            }
        }

        private void BeginDialogue(string[] lines, Action onComplete)
        {
            dialogueLines = lines;
            dialogueIndex = 0;
            dialogueComplete = onComplete;
            dialogueOpen = true;
            if (controller != null) controller.enabled = false;
            if (playerBody != null) playerBody.linearVelocity = Vector2.zero;
            if (TimeTravelManager.Instance != null) TimeTravelManager.Instance.enabled = false;
        }

        private void AdvanceDialogue()
        {
            dialogueIndex++;
            if (dialogueIndex < dialogueLines.Length) return;

            dialogueOpen = false;
            dialogueLines = Array.Empty<string>();
            dialogueIndex = 0;
            if (controller != null) controller.enabled = true;
            if (TimeTravelManager.Instance != null) TimeTravelManager.Instance.enabled = true;
            var completion = dialogueComplete;
            dialogueComplete = null;
            completion?.Invoke();
        }

        private void UpdateLocationName()
        {
            var p = (Vector2)player.position;
            if (p.y > 8f && p.x > 10f) locationName = "Distrito da Oficina";
            else if (p.y > 6f && p.x < -8f) locationName = "Parque Oeste";
            else if (p.y > 5f) locationName = "Praça do Relógio";
            else if (p.y < -8f && p.x < 0f) locationName = "Bairro Sul";
            else if (p.y < -8f) locationName = "Distrito Residencial";
            else if (p.x < -12f) locationName = "Estação Antiga";
            else if (p.x > 12f) locationName = "Avenida Leste";
            else locationName = "Avenida Central";
        }

        private void BuildStyles()
        {
            if (dialogueStyle != null) return;

            dialogueNameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            dialogueNameStyle.normal.textColor = new Color(0.35f, 0.95f, 1f);

            dialogueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                wordWrap = true
            };
            dialogueStyle.normal.textColor = Color.white;

            objectiveTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            objectiveTitleStyle.normal.textColor = new Color(1f, 0.80f, 0.24f);

            objectiveStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                wordWrap = true
            };
            objectiveStyle.normal.textColor = Color.white;

            locationStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            locationStyle.normal.textColor = new Color(0.92f, 0.95f, 0.98f);
        }

        private void OnGUI()
        {
            BuildStyles();

            GUI.Box(new Rect(Screen.width - 390, 20, 365, 86), string.Empty);
            GUI.Label(new Rect(Screen.width - 372, 30, 330, 22), "OBJETIVO", objectiveTitleStyle);
            GUI.Label(new Rect(Screen.width - 372, 52, 330, 48), objective, objectiveStyle);

            GUI.Box(new Rect(Screen.width / 2f - 105, 18, 210, 32), string.Empty);
            GUI.Label(new Rect(Screen.width / 2f - 95, 20, 190, 27), locationName, locationStyle);

            if (!dialogueOpen || dialogueLines.Length == 0) return;

            var raw = dialogueLines[Mathf.Clamp(dialogueIndex, 0, dialogueLines.Length - 1)];
            var split = raw.Split(new[] { '|' }, 2);
            var speaker = split.Length > 1 ? split[0] : "";
            var text = split.Length > 1 ? split[1] : raw;

            var boxHeight = 150f;
            var y = Screen.height - boxHeight - 24f;
            GUI.Box(new Rect(60, y, Screen.width - 120, boxHeight), string.Empty);
            GUI.Label(new Rect(86, y + 18, 180, 28), speaker, dialogueNameStyle);
            GUI.Label(new Rect(86, y + 50, Screen.width - 172, 64), text, dialogueStyle);
            GUI.Label(new Rect(Screen.width - 260, y + 116, 170, 24), "ESPAÇO / ENTER  continuar", objectiveTitleStyle);
        }
    }
}
