using UnityEngine;

public class RandomMovement : MonoBehaviour, IMovable
{
    [SerializeField] private float waypointRadius = 2.0f; 

    private Vector2 currentWaypoint;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0f;
        _rigidbody.isKinematic = true;

        GenerateRandomWaypoint();
    }

    private void GenerateRandomWaypoint()
    {
        float randomAngle = Random.Range(0f, 360f); 
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)); 
        currentWaypoint = (Vector2)transform.position + direction * Random.Range(waypointRadius * 0.5f, waypointRadius); 
    }

    public void Move(float movementSpeed)
    {
        Vector2 direction = (currentWaypoint - (Vector2)transform.position).normalized;
        _rigidbody.MovePosition((Vector2)transform.position + direction * movementSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentWaypoint) < 0.1f)
        {
            GenerateRandomWaypoint();
        }
    }
}