using UnityEngine;

/// <summary>
/// Зона выхода. Победа засчитывается только если игрок несёт полное сокровище
/// (все N частей в слоте).
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        int held = PlayerStatuses.Instance.TreasurePartsHeld;
        int needed = GameManager.Instance.NumberOfTreasureParts;

        if (held >= needed)
        {
            GameManager.Instance.WinGame();
        }
        else
        {
            Debug.Log($"[ExitTrigger] Несёшь {held}/{needed} — не хватает частей.");
        }
    }
}