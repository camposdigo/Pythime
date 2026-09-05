using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class CharacterCustomizerOverlay : MonoBehaviour
    {
        private const int WindowId = 203085;

        private PlayerVisual visual;
        private PlayerController controller;
        private bool open;
        private Rect windowRect = new Rect(24f, 126f, 440f, 590f);
        private GUIStyle headerStyle;
        private GUIStyle hintStyle;
        private GUIStyle subStyle;

        public void Initialize(GameObject player)
        {
            if (player == null) return;
            visual = player.GetComponent<PlayerVisual>();
            controller = player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.tabKey.wasPressedThisFrame)
                SetOpen(!open);

            if (open && keyboard.escapeKey.wasPressedThisFrame)
                SetOpen(false);
        }

        private void SetOpen(bool value)
        {
            open = value;
            if (controller != null)
            {
                controller.InputLocked = open;
                if (open) controller.StopImmediately();
            }
        }

        private void OnGUI()
        {
            if (!open || visual == null) return;
            BuildStyles();
            windowRect = GUI.Window(WindowId, windowRect, DrawWindow, "PERSONAGEM");
        }

        private void DrawWindow(int id)
        {
            var y = 32f;
            GUI.Label(new Rect(18, y, 380, 28), "Customização", headerStyle);
            y += 29f;
            GUI.Label(new Rect(18, y, 390, 22), "Monte o viajante do tempo do seu jeito.", subStyle);
            y += 32f;

            DrawRow(ref y, "Preset", visual.PresetName, () => visual.CyclePreset(-1), () => visual.CyclePreset(1));
            DrawRow(ref y, "Pele", visual.SkinName, () => visual.CycleSkin(-1), () => visual.CycleSkin(1));
            DrawRow(ref y, "Cabelo", visual.HairStyleName, () => visual.CycleHairStyle(-1), () => visual.CycleHairStyle(1));
            DrawRow(ref y, "Cor do cabelo", visual.HairColorName, () => visual.CycleHairColor(-1), () => visual.CycleHairColor(1));
            DrawRow(ref y, "Camisa", visual.ShirtName, () => visual.CycleShirt(-1), () => visual.CycleShirt(1));
            DrawRow(ref y, "Jaqueta / colete", visual.JacketName, () => visual.CycleJacket(-1), () => visual.CycleJacket(1));
            DrawRow(ref y, "Calça", visual.PantsName, () => visual.CyclePants(-1), () => visual.CyclePants(1));
            DrawRow(ref y, "Calçado", visual.ShoesName, () => visual.CycleShoes(-1), () => visual.CycleShoes(1));
            DrawRow(ref y, "Acessório", visual.AccessoryName, () => visual.CycleAccessory(-1), () => visual.CycleAccessory(1));

            y += 8f;
            GUI.Box(new Rect(18, y, 404, 56), string.Empty);
            GUI.Label(new Rect(30, y + 8, 380, 42),
                "Marty 1985: cabelo castanho bagunçado, camisa xadrez, jaqueta jeans, colete vermelho, jeans e tênis branco.", hintStyle);

            y += 70f;
            if (GUI.Button(new Rect(18, y, 194, 36), "MARTY 1985")) visual.ApplyPreset(1);
            if (GUI.Button(new Rect(228, y, 194, 36), "FECHAR  [TAB]")) SetOpen(false);

            GUI.DragWindow(new Rect(0, 0, 440, 28));
        }

        private static void DrawRow(ref float y, string label, string value, System.Action previous, System.Action next)
        {
            GUI.Label(new Rect(18, y + 5, 128, 24), label);
            if (GUI.Button(new Rect(148, y, 34, 30), "<")) previous();
            GUI.Box(new Rect(188, y, 188, 30), value);
            if (GUI.Button(new Rect(386, y, 34, 30), ">")) next();
            y += 38f;
        }

        private void BuildStyles()
        {
            if (headerStyle != null) return;

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            headerStyle.normal.textColor = Color.white;

            subStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12
            };
            subStyle.normal.textColor = new Color(0.58f, 0.76f, 0.86f);

            hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                wordWrap = true
            };
            hintStyle.normal.textColor = new Color(0.84f, 0.88f, 0.92f);
        }
    }
}
