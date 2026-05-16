/// <summary>
/// Параметры, которые меняются от сложности. По мере роста проекта
/// сюда добавляются поля: счётчики врагов, тайминги, видимость HUD и т.п.
///
/// На данном этапе используется только видимость контейнеров
/// (через DifficultyContainerManager). Поля lightsOff и doorsEnabled
/// сериализованы здесь для будущих PR — пока никто их не читает.
/// </summary>
[System.Serializable]
public struct DifficultyConfig
{
    /// <summary>
    /// На Medium и Hard ambient-свет выключен, видно только в зоне фонарика.
    /// Реализация в следующем PR.
    /// </summary>
    public bool lightsOff;

    /// <summary>
    /// На Hard в локации появляются двери, открываемые ключом.
    /// Реализация в следующем PR.
    /// </summary>
    public bool doorsEnabled;

    /// <summary>
    /// Возвращает преднастроенный конфиг для указанной сложности.
    /// Если позже захочется редактировать значения в инспекторе без
    /// перекомпиляции — мигрируется в ScriptableObject одним PR.
    /// </summary>
    public static DifficultyConfig For(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return new DifficultyConfig
                {
                    lightsOff = false,
                    doorsEnabled = false
                };

            case Difficulty.Medium:
                return new DifficultyConfig
                {
                    lightsOff = true,
                    doorsEnabled = false
                };

            case Difficulty.Hard:
                return new DifficultyConfig
                {
                    lightsOff = true,
                    doorsEnabled = true
                };

            default:
                return new DifficultyConfig
                {
                    lightsOff = false,
                    doorsEnabled = false
                };
        }
    }
}
