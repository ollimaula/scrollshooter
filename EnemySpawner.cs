using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemy_pool;

    private float _spawn_counter = 0f;
    private float _large_spawn_counter = 15f;

    private readonly string _small_enemy_type = "Small";
    private readonly string _medium_enemy_type = "Medium";
    private readonly string _large_enemy_type = "Large";

    private void Update()
    {
        if (_spawn_counter <= 0f)
        {
            _spawn_counter = Random.Range(0.5f, 3f);
            SpawnEnemy(_small_enemy_type);
            if (_spawn_counter > 1.5f)
                SpawnEnemy(_medium_enemy_type);
        }
        else _spawn_counter -= Time.deltaTime;
        if (_large_spawn_counter <= 0f)
        {
            _large_spawn_counter = Random.Range(10f, 20f);
            SpawnEnemy(_large_enemy_type);
        }
        else _large_spawn_counter -= Time.deltaTime;
    }
    private void SpawnEnemy(string enemy_type)
    {
        GameObject enemy = enemy_pool.GetEnemy(enemy_type);
        enemy.transform.SetPositionAndRotation(new Vector3(Random.Range(-8f, 8f), 9f, 0), Quaternion.Euler(180f, 0f, 0f));
    }
}
