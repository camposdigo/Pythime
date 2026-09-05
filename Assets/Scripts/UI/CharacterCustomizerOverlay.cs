using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class CharacterCustomizerOverlay : MonoBehaviour
    {
        private PlayerVisual visual;
        private PlayerController controller;
        private bool open;
        private Rect windowRect = new Rect(24f, 126f, 420f, 540f);
        private GUIStyle headerStyle;
        private GUIStyle hintStyle;

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
            windowRect = GUI.Window(GetInstanceID(), windowRect, DrawWindow, "PERSONAGEM");
        }

        private void DrawWindow(int id)
        {
            var y = 32f;
            GUI.Label(new Rect(18, y, 360, 28), "Customização", headerStyle);
            y += 36f;

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
            GUI.Box(new Rect(18, y, 384, 52), string.Empty);
            GUI.Label(new Rect(30, y + 8, 360, 38),
                "Preset Time Traveler 85: colete vermelho, camisa clara, jeans e tênis branco — uma homenagem retrô, não uma cópia literal.", hintStyle);

            y += 66f;
            if (GUI.Button(new Rect(18, y, 184, 34), "TIME TRAVELER 85")) visual.ApplyPreset(1);
            if (GUI.Button(new Rect(218, y, 184, 34), "FECHAR  [TAB]")) SetOpen(false);

            GUI.DragWindow(new Rect(0, 0, 420, 28));
        }

        private static void DrawRow(ref float y, string label, string value, System.Action previous, System.Action next)
        {
            GUI.Label(new Rect(18, y + 5, 118, 24), label);
            if (GUI.Button(new Rect(140, y, 34, 30), "<")) previous();
            GUI.Box(new Rect(180, y, 180, 30), value);
            if (GUI.Button(new Rect(368, y, 34, 30), ">")) next();
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

            hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                wordWrap = true
            };
            hintStyle.normal.textColor = new Color(0.84f, 0.88f, 0.92f);
        }
    }
}
