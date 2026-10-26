using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Creates a small on-screen USE button at runtime. The CF2 rig has an
/// Interaction button but no separate UseItem button; this bridges that gap
/// by simulating the game's Mouse1 binding through InputManager.SimulateKey.
/// </summary>
public class MobileUseItemButton : MonoBehaviour
{
    private bool held;

    private void Start()
    {
        GameObject canvasGo = new GameObject("MobileExtrasCanvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(640f, 480f);
        UnityEngine.Object.DontDestroyOnLoad(canvasGo);

        GameObject btnGo = new GameObject("UseItemButton",
            typeof(RectTransform), typeof(Image), typeof(Button));
        btnGo.transform.SetParent(canvasGo.transform, false);
        RectTransform rt = btnGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.86f, 0.14f);
        rt.anchorMax = new Vector2(0.86f, 0.14f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(110f, 110f);
        rt.anchoredPosition = Vector2.zero;
        Image img = btnGo.GetComponent<Image>();
        img.color = new Color(1f, 0.55f, 0.12f, 0.78f);

        GameObject textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(btnGo.transform, false);
        RectTransform trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        Text label = textGo.GetComponent<Text>();
        label.text = "USE";
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.fontSize = 26;
        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null) label.font = font;

        EventTrigger trigger = btnGo.AddComponent<EventTrigger>();
        EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(_ => held = true);
        trigger.triggers.Add(down);
        EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(_ => held = false);
        trigger.triggers.Add(up);
        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => held = false);
        trigger.triggers.Add(exit);
    }

    private void Update()
    {
        if (!held) return;
        InputManager inputManager = Singleton<InputManager>.Instance;
        if (inputManager != null) inputManager.SimulateKey(KeyCode.Mouse1);
    }
}
