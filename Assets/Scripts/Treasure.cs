using UnityEngine;

/// <summary>
/// Часть сокровища на полу.
/// Подбирается игроком если включён фонарик и в слоте нет предмета.
/// </summary>
public class Treasure : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!GameManager.Instance.FlashlightOn) return;
        if (!PlayerStatuses.Instance.CanPickUpTreasurePart()) return;

        PlayerStatuses.Instance.AddTreasurePart();
        GameManager.Instance.NotifyTreasurePartTaken();
        gameObject.SetActive(false);
    }
}