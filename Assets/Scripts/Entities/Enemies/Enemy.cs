using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SimpleFlash))]
public class Enemy : Entity
{
    private SimpleFlash _flashEffect;
    public event Action<Enemy> OnDeath;
    private IMovable _enemyMovement;
    private Rigidbody2D _rigidbody;
    private string _playerTag = "Player";
    [SerializeField] private int hp = 2;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private AudioClip _takeDamage;
    [SerializeField] private GameObject _healItem;
    [SerializeField] private float _healSpawnChance = 0.3f; 
    private float _lastAttackTime;

    private void Awake()
    {
        MaxHealthPoints = hp;
        CurrentHealthPoints = MaxHealthPoints;
        _enemyMovement = GetComponent<IMovable>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _flashEffect = GetComponent<SimpleFlash>();
        _lastAttackTime = -attackCooldown; 
    }

    private void FixedUpdate()
    {
        _enemyMovement.Move(WalkSpeed);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag(_playerTag) && Time.time >= _lastAttackTime + attackCooldown)
        {
            Vector2 forceDirection = (other.transform.position - transform.position).normalized;
            other.collider.GetComponent<Player>().TakeDamage(damage, forceDirection);
            _lastAttackTime = Time.time;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(_playerTag) && Time.time >= _lastAttackTime + attackCooldown)
        {
            Vector2 forceDirection = (other.transform.position - transform.position).normalized;
            other.GetComponent<Player>().TakeDamage(damage, forceDirection);
            _lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage, Vector2 forceDirection)
    {
        CurrentHealthPoints -= damage;
        _flashEffect.Flash();
        SoundFXManager.Instance.PlaySoundFXClip(_takeDamage, transform, 1f);
        if (CurrentHealthPoints <= 0)
        {
            CurrentHealthPoints = 0;
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

    public void ProcessDeath()
    {
        if (UnityEngine.Random.value <= _healSpawnChance)
        {
            Instantiate(_healItem, transform.position, Quaternion.identity);
        }
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}