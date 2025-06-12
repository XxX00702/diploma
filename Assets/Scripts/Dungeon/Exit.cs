using UnityEngine;

public class Exit : MonoBehaviour
{
    private DungeonGenerator _dungeonGenerator;

    private void Start()
    {
        _dungeonGenerator = FindObjectOfType<DungeonGenerator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enemy[] _enemies = FindObjectsOfType<Enemy>();
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].ProcessDeath();
            }
            _dungeonGenerator.RegenerateDungeon();
        }
    }
}
