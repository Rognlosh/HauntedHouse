#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor-only удобство: меню «HauntedHouse → Always Start From Bootstrap».
/// Когда включено, нажатие Play в редакторе всегда запускает Bootstrap.unity
/// независимо от того, какая сцена сейчас открыта.
///
/// Снимите галочку, чтобы тестировать сцену уровня напрямую без меню.
/// В билд этот файл не попадает — лежит в папке Editor.
/// </summary>
[InitializeOnLoad]
internal static class EditorPlayFromBootstrap
{
    private const string MenuPath = "HauntedHouse/Always Start From Bootstrap";
    private const string PrefKey = "HH_AlwaysStartFromBootstrap";
    private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";

    static EditorPlayFromBootstrap()
    {
        // При перезагрузке домена статический конструктор дёргается рано —
        // откладываем Apply, чтобы AssetDatabase успел подняться.
        EditorApplication.delayCall += Apply;
    }

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        bool now = !EditorPrefs.GetBool(PrefKey, true);
        EditorPrefs.SetBool(PrefKey, now);
        Apply();
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(PrefKey, true));
        return true;
    }

    private static void Apply()
    {
        bool enabled = EditorPrefs.GetBool(PrefKey, true);
        if (!enabled)
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        var bootstrap = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);
        // Bootstrap.unity ещё не создан — это нормально на свежем клоне
        // до того как разработчик создаст сцену. Молча ничего не делаем.
        EditorSceneManager.playModeStartScene = bootstrap;
    }
}
#endif
