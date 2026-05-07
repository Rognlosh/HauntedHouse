using UnityEngine;
using HountedHouse.Utils;

/// <summary>
/// Огонёк — хаотично блуждает, никогда не преследует напрямую.
/// Вблизи игрока: ускоряется и слегка корректирует маршрут в его сторону.
/// </summary>
public class Flame : Enemy
{
    [Header("Настройки огонька")]
    [Tooltip("Радиус, в котором огонёк чувствует игрока и ускоряется")]
    [SerializeField] private float playerDetectionRadius = 4f;

    [Tooltip("Множитель скорости когда игрок близко")]
    [SerializeField] private float speedBoostMultiplier = 2f;

    [Tooltip("Насколько маршрут смещается к игроку (0 = полностью случайно, 1 = прямо к игроку)")]
    [Range(0f, 1f)]
    [SerializeField] private float playerBiasWeight = 0.25f;

    // -------------------------------------------------------------------------
    // Переопределения
    // -------------------------------------------------------------------------

    /// <summary>
    /// Огонёк никогда не переходит в Chasing —
    /// только регулирует скорость в зависимости от близости игрока.
    /// </summary>
    protected override void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        navMeshAgent.speed = distanceToPlayer <= playerDetectionRadius
            ? roamingSpeed * speedBoostMultiplier
            : roamingSpeed;

        // Состояние всегда Roaming — переключения не происходит
    }

    /// <summary>
    /// Случайная точка блуждания с лёгким смещением в сторону игрока.
    /// </summary>
    protected override Vector3 GetRoamingPosition()
    {
        Vector3 randomDir = Utils.GetRandomDir();
        Vector3 toPlayer = (Player.Instance.transform.position - transform.position).normalized;

        // Смешиваем случайное направление с направлением к игроку
        Vector3 biasedDir = Vector3.Lerp(randomDir, toPlayer, playerBiasWeight).normalized;
        float distance = Random.Range(roamingDistanceMin, roamingDistanceMax);

        // Двигаемся от текущей позиции, а не от стартовой —
        // так огонёк свободно бродит по всему этажу
        return transform.position + biasedDir * distance;
    }
}