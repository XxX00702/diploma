using UnityEngine;
using UnityEngine.AI;

public class DefaultMovement : MonoBehaviour, IMovable
{
    private Transform _playerTransform;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Move(float speed)
    {
        Vector2 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        Vector2 velocity = directionToPlayer * speed;
        _rigidbody2D.MovePosition((Vector2)transform.position + velocity * Time.fixedDeltaTime);
    }
}