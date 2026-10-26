using ControlFreak2;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Instantiates the Control Freak 2 touch rig in every scene.
/// This is what makes the mobile controls appear without editing all 10 scenes.
/// </summary>
public static class MobileControlsBootstrap
{
    private const string RigResourcePath = "CF2-Rig";
    private static bool hooked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (!hooked)
        {
            hooked = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        EnsureEventSystem();
        SpawnRig();
        EnsureExtraButtons();
    }

    private static void EnsureExtraButtons()
    {
        if (UnityEngine.Object.FindObjectOfType<MobileUseItemButton>() != null)
            return;
        GameObject go = new GameObject("MobileExtras");
        go.AddComponent<MobileUseItemButton>();
        UnityEngine.Object.DontDestroyOnLoad(go);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureEventSystem();
        SpawnRig();
        EnsureExtraButtons();
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;
        GameObject go = new GameObject("EventSystem (Mobile)");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
        UnityEngine.Object.DontDestroyOnLoad(go);
    }

    private static void SpawnRig()
    {
        if (CF2Input.activeRig != null)
            return;
        GameObject prefab = Resources.Load<GameObject>(RigResourcePath);
        if (prefab == null)
        {
            Debug.LogWarning("[MobileControls] Assets/Resources/CF2-Rig.prefab bulunamadi.");
            return;
        }
        GameObject rig = UnityEngine.Object.Instantiate(prefab);
        rig.name = "CF2-Rig (Mobile Controls)";
        UnityEngine.Object.DontDestroyOnLoad(rig);
        CF2Input.SetMobileMode(CF2Input.MobileMode.Auto);
    }
}
