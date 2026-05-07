using UnityEngine;

/// <summary>
/// “елепортирует игрока между этажами и обновл€ет текущий этаж в GameManager.
/// </summary>
public class Stairs : MonoBehaviour
{
    [Tooltip("true Ч лестница ведЄт вверх, false Ч вниз")]
    [SerializeField] private bool isStairsUp;

    private bool isPlayerTeleported = true;

    private const float TeleportOffsetX = 17f;
    private const float TeleportCooldown = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (GameManager.Instance.IsPlayerInside) return;

        GameManager.Instance.IsPlayerInside = true;
        isPlayerTeleported = false;

        if (isStairsUp)
            TeleportPlayerUp(collision.gameObject);
        else
            TeleportPlayerDown(collision.gameObject);

        Invoke(nameof(UncheckPlayerTeleported), TeleportCooldown);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerTeleported)
            GameManager.Instance.IsPlayerInside = false;
    }

    private void TeleportPlayerUp(GameObject player)
    {
        player.transform.position += new Vector3(TeleportOffsetX, 0, 0);
        GameManager.Instance.CurrentFloor++;
    }

    private void TeleportPlayerDown(GameObject player)
    {
        player.transform.position -= new Vector3(TeleportOffsetX, 0, 0);
        GameManager.Instance.CurrentFloor--;
    }

    private void UncheckPlayerTeleported()
    {
        isPlayerTeleported = true;
    }
}