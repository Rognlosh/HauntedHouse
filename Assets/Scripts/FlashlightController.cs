using UnityEngine;

/// <summary>
/// Управляет фонариком игрока: включение/выключение, таймер заряда.
/// </summary>
public class FlashlightController : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float maxFlashlightDuration = 5f;

    private float flashlightTimer;
    private SpriteMask maskComponent;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        maskComponent = GetComponent<SpriteMask>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Таймер тикает только пока фонарик включён
        if (!GameManager.Instance.FlashlightOn) return;

        flashlightTimer += Time.deltaTime;
        if (flashlightTimer >= maxFlashlightDuration)
            TurnOffFlashlight();
    }

    public void ToggleFlashlight()
    {
        if (GameManager.Instance.FlashlightOn)
            TurnOffFlashlight();
        else
            TurnOnFlashlight();
    }

    private void TurnOnFlashlight()
    {
        gameObject.SetActive(true);
        GameManager.Instance.FlashlightOn = true;
        maskComponent.enabled = true;
        spriteRenderer.enabled = true;
        flashlightTimer = 0f;
    }

    private void TurnOffFlashlight()
    {
        GameManager.Instance.FlashlightOn = false;
        maskComponent.enabled = false;
        spriteRenderer.enabled = false;
        gameObject.SetActive(false);
    }
}