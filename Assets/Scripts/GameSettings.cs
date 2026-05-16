using UnityEngine;

/// <summary>
/// Статический канал передачи настроек от меню к уровню.
/// Переживает SceneManager.LoadScene, потому что статика C# не очищается
/// между сценами.
///
/// WasSetByMenu — флаг для EditorDifficultyOverride: если игрок пришёл
/// через MainMenu, флаг = true, и редакторский override молчит.
/// При прямом запуске уровня в редакторе флаг остаётся false,
/// override может назначить свою сложность.
/// </summary>
public static class GameSettings
{
    public static Difficulty SelectedDifficulty { get; private set; } = Difficulty.Easy;
    public static bool WasSetByMenu { get; private set; } = false;

    /// <summary>
    /// Сбрасывает состояние при каждом старте Play Mode.
    /// Это страховка на случай, если в Project Settings отключён
    /// Domain Reload (Enter Play Mode → Reload Domain off) —
    /// тогда статика не сбрасывается автоматически между запусками.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetState()
    {
        SelectedDifficulty = Difficulty.Easy;
        WasSetByMenu = false;
    }

    /// <summary>Вызывается из MainMenu перед загрузкой уровня.</summary>
    public static void SetByMenu(Difficulty difficulty)
    {
        SelectedDifficulty = difficulty;
        WasSetByMenu = true;
    }

    /// <summary>
    /// Вызывается из EditorDifficultyOverride при прямом запуске уровня.
    /// Если WasSetByMenu уже true — ничего не делает.
    /// </summary>
    public static void SetByEditor(Difficulty difficulty)
    {
        if (WasSetByMenu) return;
        SelectedDifficulty = difficulty;
    }
}
