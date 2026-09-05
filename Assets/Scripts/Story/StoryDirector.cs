using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class StoryDirector : MonoBehaviour
    {
        private Transform player;
        private Rigidbody2D playerBody;
        private Vector2 workshopPoint;
        private Vector2 soilPoint;
        private Vector2 monolithPoint;

        private int chapterStage;
        private bool dialogueOpen;
        private string[] dialogueLines = Array.Empty<string>();
        private int dialogueIndex;
        private Action dialogueComplete;

        private string objective = "Ouça Tock para entender o que aconteceu.";
        private string objectiveStep = "PRÓLOGO";
        private string objectiveTargetName = string.Empty;
        private string contextHint = "ESPAÇO / ENTER  continuar   •   ESC  pular conversa";
        private string locationName = "Bairro Sul";
        private Vector2 objectiveTarget;
        private bool hasObjectiveTarget;
        private int requiredYear;

        private GUIStyle dialogueNameStyle;
        private GUIStyle dialogueStyle;
        private GUIStyle dialogueHintStyle;
        private Texture2D dialogueTexture;

        public string MissionTitle => "O ECO IMPOSSÍVEL";
        public string Objective => objective;
        public string ObjectiveStep => objectiveStep;
        public string ObjectiveTargetName => objectiveTargetName;
        public string ContextHint => contextHint;
        public string LocationName => locationName;
        public bool ChapterComplete => chapterStage >= 5;
        public bool DialogueOpen => dialogueOpen;
        public bool HasObjectiveTarget => hasObjectiveTarget;
        public Vector2 ObjectiveTarget => objectiveTarget;
        public int RequiredYear => requiredYear;
        public int ChapterStage => chapterStage;

        public float DistanceToObjective
        {
            get
            {
                if (!hasObjectiveTarget || player == null) return 0f;
                return Vector2.Distance(player.position, objectiveTarget);
            }
        }

        public void Initialize(Transform playerTransform, Vector2 workshop, Vector2 soil, Vector2 monolith)
        {
            player = playerTransform;
            playerBody = player != null ? player.GetComponent<Rigidbody2D>() : null;
            workshopPoint = workshop;
            soilPoint = soil;
            monolithPoint = monolith;
            chapterStage = 0;

            SetObjective(
                "PRÓLOGO",
                "Ouça Tock para entender o que aconteceu.",
                string.Empty,
                Vector2.zero,
                false,
                0,
                "ESPAÇO / ENTER  continuar   •   ESC  pular conversa");

            BeginDialogue(new[]
            {
                "TOCK|Ei! Você tá me ouvindo? Ótimo. Isso já é melhor do que há trinta segundos.",
                "TOCK|O impacto rasgou Pythime em três versões da mesma cidade.",
                "TOCK|1956. 2026. 2096. Todas existem ao mesmo tempo e estão vazando umas nas outras.",
                "TOCK|Sua máquina sobreviveu, mais ou menos. Mas o Chrono Core não.",
                "TOCK|Primeiro passo: chegar à Oficina Temporal. Siga o marcador amarelo."
            }, () =>
            {
                chapterStage = 1;
                SetObjective(
                    "PASSO 1 DE 3",
                    "Vá até a Oficina Temporal.",
                    "OFICINA TEMPORAL",
                    workshopPoint,
                    true,
                    2026,
                    "SIGA O MARCADOR AMARELO");
            });
        }

        private void Update()
        {
            if (player == null || TimeTravelManager.Instance == null) return;

            UpdateLocationName();
            var keyboard = Keyboard.current;

            if (dialogueOpen)
            {
                contextHint = "ESPAÇO / ENTER / X  continuar   •   ESC  pular conversa";
                if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                    SkipDialogue();
                else if (keyboard != null && (keyboard.xKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame))
                    AdvanceDialogue();
                return;
            }

            var timeline = TimeTravelManager.Instance;

            if (chapterStage == 1)
            {
                requiredYear = 2026;
                objective = "Vá até a Oficina Temporal.";
                contextHint = "SIGA O MARCADOR AMARELO";

                if (timeline.CurrentYear == 2026 && Vector2.Distance(player.position, workshopPoint) < 2.6f)
                {
                    BeginDialogue(new[]
                    {
                        "TOCK|Achei o registro da Oficina. O Chrono Core não explodiu por acidente.",
                        "TOCK|Alguém ativou uma âncora temporal décadas antes de ela existir.",
                        "TOCK|O primeiro eco aparece em 1956, onde hoje fica a Praça do Relógio.",
                        "TOCK|Mude para 1956 e vá até o canteiro marcado no Parque Oeste."
                    }, () =>
                    {
                        chapterStage = 2;
                        SetObjective(
                            "PASSO 2 DE 3",
                            "Mude para 1956 e investigue o canteiro temporal.",
                            "CANTEIRO TEMPORAL",
                            soilPoint,
                            true,
                            1956,
                            "Q / E  MUDAR PARA 1956");
                    });
                }
            }
            else if (chapterStage == 2)
            {
                objectiveStep = "PASSO 2 DE 3";
                requiredYear = 1956;
                objectiveTargetName = "CANTEIRO TEMPORAL";
                objectiveTarget = soilPoint;
                hasObjectiveTarget = true;

                if (timeline.SeedPlanted)
                {
                    BeginDialogue(new[]
                    {
                        "TOCK|Você sentiu isso? A linha temporal acabou de se reorganizar.",
                        "TOCK|Uma mudança pequena em 1956 virou décadas de crescimento acumulado.",
                        "TOCK|Agora precisamos ver o resultado final. Mude para 2096 e siga o novo marcador."
                    }, () =>
                    {
                        chapterStage = 3;
                        SetObjective(
                            "PASSO 3 DE 3",
                            "Mude para 2096 e investigue o monólito.",
                            "ANOMALIA TEMPORAL",
                            monolithPoint,
                            true,
                            2096,
                            "Q / E  MUDAR PARA 2096");
                    });
                }
                else if (timeline.CurrentYear != 1956)
                {
                    objective = "Mude para 1956 e investigue o canteiro no Parque Oeste.";
                    contextHint = "Q / E  MUDAR PARA 1956";
                }
                else if (Vector2.Distance(player.position, soilPoint) < 2.1f)
                {
                    objective = "Interaja com o canteiro temporal.";
                    contextHint = "T  ALTERAR LINHA DO TEMPO";
                }
                else
                {
                    objective = "Vá até o canteiro temporal no Parque Oeste.";
                    contextHint = "SIGA O MARCADOR AMARELO";
                }
            }
            else if (chapterStage == 3)
            {
                objectiveStep = "PASSO 3 DE 3";
                requiredYear = 2096;
                objectiveTargetName = "ANOMALIA TEMPORAL";
                objectiveTarget = monolithPoint;
                hasObjectiveTarget = true;

                if (timeline.CurrentYear != 2096)
                {
                    objective = "Mude para 2096 e investigue o monólito.";
                    contextHint = "Q / E  MUDAR PARA 2096";
                }
                else if (Vector2.Distance(player.position, monolithPoint) < 2.4f)
                {
                    objective = "Inspecione o monólito temporal.";
                    contextHint = "F  INSPECIONAR ANOMALIA";

                    if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
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
                            SetObjective(
                                "CAPÍTULO CONCLUÍDO",
                                "O Eco Impossível concluído.",
                                string.Empty,
                                Vector2.zero,
                                false,
                                0,
                                "Explore Pythime ou use Q / E para comparar as épocas.");
                        });
                    }
                }
                else
                {
                    objective = "Vá até o monólito no nordeste da Praça do Relógio.";
                    contextHint = "SIGA O MARCADOR AMARELO";
                }
            }
        }

        private void SetObjective(
            string step,
            string text,
            string targetName,
            Vector2 target,
            bool showTarget,
            int targetYear,
            string hint)
        {
            objectiveStep = step;
            objective = text;
            objectiveTargetName = targetName;
            objectiveTarget = target;
            hasObjectiveTarget = showTarget;
            requiredYear = targetYear;
            contextHint = hint;
        }

        private void BeginDialogue(string[] lines, Action onComplete)
        {
            if (dialogueOpen) return;
            dialogueLines = lines;
            dialogueIndex = 0;
            dialogueComplete = onComplete;
            dialogueOpen = true;
            if (playerBody != null)
                playerBody.linearVelocity = Vector2.zero;
        }

        private void AdvanceDialogue()
        {
            dialogueIndex++;
            if (dialogueIndex < dialogueLines.Length) return;
            FinishDialogue();
        }

        public void SkipDialogue()
        {
            if (!dialogueOpen) return;
            FinishDialogue();
        }

        private void FinishDialogue()
        {
            dialogueOpen = false;
            dialogueLines = Array.Empty<string>();
            dialogueIndex = 0;
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

            dialogueTexture = MakeTexture(new Color32(17, 21, 27, 245));

            dialogueNameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 19,
                fontStyle = FontStyle.Bold
            };
            dialogueNameStyle.normal.textColor = new Color(0.35f, 0.95f, 1f);

            dialogueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                wordWrap = true
            };
            dialogueStyle.normal.textColor = Color.white;

            dialogueHintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight
            };
            dialogueHintStyle.normal.textColor = new Color(1f, 0.82f, 0.30f);
        }

        private void OnGUI()
        {
            if (!dialogueOpen || dialogueLines.Length == 0) return;
            BuildStyles();

            var raw = dialogueLines[Mathf.Clamp(dialogueIndex, 0, dialogueLines.Length - 1)];
            var split = raw.Split(new[] { '|' }, 2);
            var speaker = split.Length > 1 ? split[0] : string.Empty;
            var text = split.Length > 1 ? split[1] : raw;

            var scale = Mathf.Clamp(Screen.height / 1080f, .75f, 1.5f);
            var previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1f));
            var width = Mathf.Min(Screen.width / scale - 80f, 940f);
            var height = 158f;
            var x = (Screen.width / scale - width) * 0.5f;
            var y = Screen.height / scale - height - 24f;

            GUI.DrawTexture(new Rect(x, y, width, height), dialogueTexture);
            GUI.Label(new Rect(x + 24, y + 16, 180, 28), speaker, dialogueNameStyle);
            GUI.Label(new Rect(x + 24, y + 50, width - 48, 70), text, dialogueStyle);
            GUI.Label(new Rect(x + width - 430, y + 124, 406, 22),
                "ESPAÇO / ENTER / X continua   •   ESC pula", dialogueHintStyle);
            GUI.matrix = previousMatrix;
        }

        private static Texture2D MakeTexture(Color32 color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply(false, true);
            return texture;
        }
    }
}
