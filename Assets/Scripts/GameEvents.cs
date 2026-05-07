using System;

/// <summary>
/// Центральный хаб событий игры.
/// Все системы общаются через события — не через прямые ссылки друг на друга.
/// </summary>
public static class GameEvents
{
    /// <summary>Игра началась или перезапущена.</summary>
    public static event Action OnGameStarted;

    /// <summary>Игрок победил.</summary>
    public static event Action OnGameWon;

    /// <summary>Игрок проиграл.</summary>
    public static event Action OnGameLost;

    /// <summary>
    /// Все исходные части сокровища были собраны игроком хотя бы раз.
    /// Активирует выход. Срабатывает один раз за игру.
    /// </summary>
    public static event Action OnAllTreasuresCollected;

    /// <summary>
    /// Изменилось количество частей сокровища в слоте игрока.
    /// Параметр — текущее количество (0..N).
    /// </summary>
    public static event Action<int> OnPlayerBagChanged;

    // -------------------------------------------------------------------------
    public static void GameStarted() => OnGameStarted?.Invoke();
    public static void GameWon() => OnGameWon?.Invoke();
    public static void GameLost() => OnGameLost?.Invoke();
    public static void AllTreasuresCollected() => OnAllTreasuresCollected?.Invoke();
    public static void PlayerBagChanged(int newCount) => OnPlayerBagChanged?.Invoke(newCount);
}