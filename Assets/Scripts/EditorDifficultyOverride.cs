using UnityEngine;

/// <summary>
/// Позволяет тестировать конкретную сложность при прямом запуске уровня
/// из эдитора (минуя MainMenu). В билде ничего не делает —
/// логика обёрнута в #if UNITY_EDITOR.
///
/// Если игрок пришёл через MainMenu, GameSettings.WasSetByMenu = true,
/// и override молчит. Иначе записывает свою сложность в GameSettings.
///
/// [DefaultExecutionOrder(-1000)] — раньше, чем DifficultyContainerManager
/// (-500), чтобы тот успел увидеть актуальную сложность.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class EditorDifficultyOverride : MonoBehaviour
{
    [Tooltip("Сложность, с которой запустится уровень при прямом старте из эдитора. " +
             "При запуске через MainMenu это значение игнорируется.")]
    [SerializeField] private Difficulty difficulty = Difficulty.Easy;

    [Tooltip("Если выключено — override отключён даже в эдиторе")]
    [SerializeField] private bool active = true;

    private void Awake()
    {
#if UNITY_EDITOR
        if (!active) return;
        GameSettings.SetByEditor(difficulty);
#endif
    }
}
