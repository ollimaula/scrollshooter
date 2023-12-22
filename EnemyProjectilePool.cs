using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectile_prefab;
    [SerializeField] private int pool_size = 10;
    private Queue<GameObject> _projectiles;
    private void Start()
    {
        _projectiles = new Queue<GameObject>();
        for (int i = 0; i < pool_size; i++)
        {
            GameObject projectile = Instantiate(projectile_prefab);
            projectile.SetActive(false);
            _projectiles.Enqueue(projectile);
        }
    }
    public GameObject GetProjectile()
    {
        foreach (GameObject projectile in _projectiles)
        {
            if (!projectile.activeSelf)
            {
                projectile.SetActive(true);
                return projectile;
            }
        }
        GameObject new_projectile = Instantiate(projectile_prefab);
        _projectiles.Enqueue(new_projectile);
        return new_projectile;
    }
    public void ReturnProjectile(GameObject projectile) => projectile.SetActive(false);
}
