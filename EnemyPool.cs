using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject small_enemy_prefab;
    [SerializeField] private GameObject medium_enemy_prefab;
    [SerializeField] private GameObject large_enemy_prefab;
    [SerializeField] private int pool_size = 10;

    private Dictionary<string, Queue<GameObject>> _enemy_pool;

    private void Start()
    {
        _enemy_pool = new Dictionary<string, Queue<GameObject>>();
        InitializeEnemyPool(small_enemy_prefab);
        InitializeEnemyPool(medium_enemy_prefab);
        InitializeEnemyPool(large_enemy_prefab);
    }
    private void InitializeEnemyPool(GameObject enemy_prefab)
    {
        string enemy_tag = enemy_prefab.tag;
        var enemy_queue = new Queue<GameObject>();
        for (int i = 0; i < pool_size; i++)
        {
            GameObject enemy = Instantiate(enemy_prefab);
            enemy.SetActive(false);
            enemy_queue.Enqueue(enemy);
        }
        _enemy_pool.Add(enemy_tag, enemy_queue);
    }
    public GameObject GetEnemy(string enemy_type)
    {
        if (_enemy_pool.TryGetValue(enemy_type, out var enemy_queue))
        {
            foreach (GameObject enemy in enemy_queue)
            {
                if (!enemy.activeSelf)
                {
                    enemy.SetActive(true);
                    return enemy;
                }
            }
            GameObject new_enemy = Instantiate(GetEnemyPrefab(enemy_type));
            new_enemy.SetActive(true);
            enemy_queue.Enqueue(new_enemy);
            return new_enemy;
        }
        Debug.LogWarning("Enemy type not found in the pool: " + enemy_type);
        return null;
    }
    private GameObject GetEnemyPrefab(string enemy_type)
    {
        return enemy_type switch
        {
            "Small" => small_enemy_prefab,
            "Medium" => medium_enemy_prefab,
            "Large" => large_enemy_prefab,
            _ => null
        };
    }
    public void ReturnEnemy(GameObject enemy) => enemy.SetActive(false);
}
