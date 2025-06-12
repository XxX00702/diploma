using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Player : Entity
{
    [HideInInspector] public static PlayerInputs _playerInputsAction;
    private PlayerMovement _playerMovement;
    private PlayerInteract _playerInteract;
    private PlayerItemsInteract _playerItemsInteract;
    private PlayerActiveItems _playerActiveItems;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    public static bool _isPaused;
    [HideInInspector] public IActivatable CurrentActiveItem
    { 
        get => _currentActiveItem; 
        set
        {
            OnActiveItemChange?.Invoke(value);
            _currentActiveItem = value;
        }
    }
    [HideInInspector] public event System.Action<IActivatable> OnActiveItemChange;
    [Header("Sounds")] 
    [SerializeField] private AudioClip _takeDamage;
    [Header("Camera")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _cameraThreshhold = 5f;

    [Header("Hand movement")]
    [SerializeField] private GameObject _playerHand;
    [SerializeField] private float _smoothness = 10f;
    [SerializeField] private float _threshold = 2f;

    [Header("Interaction")]
    [SerializeField] private float _radius = 1f;
    [SerializeField] private LayerMask _interactableLayer;

    private MenuManager _menuManager;

    [SerializeField] private float invincibilityDuration = 5f;
    private bool isInvincible = false;
    private Coroutine invincibilityCoroutine;
    private IActivatable _currentActiveItem;
    private Collider2D _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _playerInputsAction = new PlayerInputs();
        _playerInputsAction.Player.Enable();

        _playerMovement = new PlayerMovement(_rigidbody, _playerCamera, _playerHand);
        _playerInteract = gameObject.AddComponent<PlayerInteract>();
        _playerInteract.Initialize(_interactableLayer, _radius);
        _playerItemsInteract = new PlayerItemsInteract(CurrentWeapon);
        _playerActiveItems = gameObject.AddComponent<PlayerActiveItems>();
        _playerActiveItems.Initialize();
        OnWeaponChanged += _playerItemsInteract.UpdateCurrentWeapon;

        _menuManager = FindObjectOfType<MenuManager>();
    }

    public void Update()
    {
        if (_isPaused) return;

        _playerMovement.ProcessPlayerMovement(WalkSpeed);
        _playerMovement.ProcessHandMovement(_smoothness, _threshold);
        _playerMovement.ProcessWeaponRotation(CurrentWeapon);
        _playerMovement.ProcessCameraMovement(_cameraThreshhold);
        _playerInteract.CheckForInteractable();
    }
    

    public void Heal(int healthCount)
    {
        if ((CurrentHealthPoints + healthCount) > MaxHealthPoints)
        {
            CurrentHealthPoints = MaxHealthPoints;
        }
        else
        {
            CurrentHealthPoints += healthCount;
        }
    }

    public void TakeDamage(int damage, Vector2 forceDirection)
    {
        if (isInvincible || _isPaused) return;

        Debug.Log("TakeDamage function invoked for player. Damage - " + damage);
        Debug.Log("Player health points before damage - " + CurrentHealthPoints);
        CurrentHealthPoints -= damage;
        Debug.Log("Player health points after damage - " + CurrentHealthPoints);
        SoundFXManager.Instance.PlaySoundFXClip(_takeDamage, transform, 1f);
        if (CurrentHealthPoints <= 0)
        {
            CurrentHealthPoints = 0;
            Debug.Log("ProcessDeath function invoked for player");
            ProcessDeath();
        }
        else
        {
            ApplyForce(-forceDirection);
        }
    }

    private void ApplyForce(Vector2 forceDirection)
    {
        _rigidbody.AddForce(forceDirection, ForceMode2D.Impulse);
    }

    private void ProcessDeath()
    {
        _menuManager.ShowDeathMenu();
    }

    public static void SetPaused(bool isPaused)
    {
        _isPaused = isPaused;
    }

    public void ActivateInvincibility()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }
        invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        Debug.Log("Player got invincibility.");
        isInvincible = true;
        Color originalColor = _spriteRenderer.color;
        Color invincibleColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // Find all enemies in the scene
        var enemyColliders = FindObjectsOfType<Enemy>();

        // Ignore collisions with enemies
        foreach (var enemy in enemyColliders)
        {
            if (enemy != null)
            {
                Physics2D.IgnoreCollision(_collider, enemy.GetComponent<Collider2D>(), true);
            }
        }

        float flashDuration = 0.1f; 
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            _spriteRenderer.color = invincibleColor;
            yield return new WaitForSeconds(flashDuration);
            _spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
            elapsedTime += flashDuration * 2;
        }

        foreach (var enemy in enemyColliders)
        {
            if (enemy != null)
            {
                Physics2D.IgnoreCollision(_collider, enemy.GetComponent<Collider2D>(), false);
            }
        }

        _spriteRenderer.color = originalColor; 
        isInvincible = false;
        Debug.Log("Player invincibility has ended.");
    }

}
