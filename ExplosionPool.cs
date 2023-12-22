using System;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    [SerializeField] private GameObject small_explosion_prefab;
    [SerializeField] private GameObject medium_explosion_prefab;
    [SerializeField] private GameObject large_explosion_prefab;
    [SerializeField] private int pool_size = 5;

    private Dictionary<string, Queue<GameObject>> _explosion_pool;

    private void Start()
    {
        _explosion_pool = new Dictionary<string, Queue<GameObject>>();
        InitializeExplosionPool(small_explosion_prefab);
        InitializeExplosionPool(medium_explosion_prefab);
        InitializeExplosionPool(large_explosion_prefab);
    }
    private void InitializeExplosionPool(GameObject explosion_prefab)
    {
        string explosion_tag = explosion_prefab.tag;
        var explosion_queue = new Queue<GameObject>();
        for (int i = 0; i < pool_size; i++)
        {
            GameObject explosion = Instantiate(explosion_prefab);
            explosion.SetActive(false);
            explosion_queue.Enqueue(explosion);
        }
        _explosion_pool.Add(explosion_tag, explosion_queue);
    }
    public GameObject GetExplosion(string explosion_type)
    {
        if (_explosion_pool.TryGetValue(explosion_type, out var explosion_queue))
        {
            foreach (GameObject explosion in explosion_queue)
            {
                if (!explosion.activeSelf)
                {
                    explosion.SetActive(true);
                    return explosion;
                }
            }
            GameObject new_explosion = Instantiate(GetExplosionPrefab(explosion_type));
            new_explosion.SetActive(true);
            explosion_queue.Enqueue(new_explosion);
            return new_explosion;
        }
        Debug.LogWarning("Explosion type not found in the pool: " + explosion_type);
        return null;
    }
    private GameObject GetExplosionPrefab(string explosion_type)
    {
        return explosion_type switch
        {
            "Small" => small_explosion_prefab,
            "Medium" => medium_explosion_prefab,
            "Large" => large_explosion_prefab,
            _ => null
        };
    }
    public void ReturnExplosion(GameObject explosion) => explosion.SetActive(false);
}
