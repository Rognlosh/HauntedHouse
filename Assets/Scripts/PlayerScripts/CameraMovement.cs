using UnityEngine;

/// <summary>
/// Камера мгновенно следует за игроком.
/// X-позиция зависит от текущего этажа, Y зажата между minPositionY и maxPositionY.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform player;

    [Header("Ограничения по Y")]
    [SerializeField] private float maxPositionY = 2.19f;
    [SerializeField] private float minPositionY = -9.18f;

    private const float FloorOffsetX = 17f;

    private void LateUpdate()
    {
        float targetX = FloorOffsetX * (GameManager.Instance.CurrentFloor - 1);
        float targetY = Mathf.Clamp(player.position.y, minPositionY, maxPositionY);

        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}