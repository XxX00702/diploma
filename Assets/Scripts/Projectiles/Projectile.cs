using System.ComponentModel.Design.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 1f;
    [SerializeField] private bool _isImpactable = true;
    
    public int _damage = 1;
    private Rigidbody2D _bulletRigidbody2D;
    private Vector2 _direction;

    private void Awake()
    {
        
        _bulletRigidbody2D = GetComponent<Rigidbody2D>();
        _direction = transform.right; 
    }

    private void Update()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        _bulletRigidbody2D.velocity = _direction * _projectileSpeed;
    }
    public void SetDirection(Vector2 direction)
    {
        _direction = direction.normalized; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>()) return;
        var isEnemy = other.TryGetComponent(out Enemy enemy);
        if (isEnemy)
        {
            enemy.TakeDamage(_damage, -_direction); 
        }
        if(_isImpactable) ProcessImpact();
    }

    public void ProcessImpact()
    {
        Destroy(gameObject);
    }
}