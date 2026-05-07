using UnityEngine;

/// <summary>
/// Управляет вводом, движением и анимацией игрока.
/// Инвентарь и статусы — в PlayerStatuses (отдельный компонент).
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Настройки движения")]
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private Vector3 startPosition = new Vector3(2.92f, -2.52f, 0);

    [Header("Ссылки")]
    [SerializeField] private FlashlightController flashlight;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 playerInput;

    // -------------------------------------------------------------------------
    // Unity
    // -------------------------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnGameLost += HandleDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnGameLost -= HandleDeath;
    }

    private void Update()
    {
        if (!GameManager.Instance.GameIsActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
            flashlight.ToggleFlashlight();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.GameIsActive) return;

        playerInput = new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            0f
        );

        UpdateAnimationAndMove();
    }

    // -------------------------------------------------------------------------
    // Движение и анимация
    // -------------------------------------------------------------------------
    private void UpdateAnimationAndMove()
    {
        bool isMoving = playerInput != Vector3.zero;
        animator.SetBool("moving", isMoving);

        if (isMoving)
        {
            rb.MovePosition(transform.position + playerInput * (playerSpeed * Time.deltaTime));
            animator.SetFloat("moveX", playerInput.x);
            animator.SetFloat("moveY", playerInput.y);
        }
    }

    private void HandleGameStarted() => animator.SetBool("dead", false);
    private void HandleDeath() => animator.SetBool("dead", true);

    public void PlayerStartPoint()
    {
        transform.position = startPosition;
    }
}