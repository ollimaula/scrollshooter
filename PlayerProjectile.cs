using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private PlayerProjectilePool _projectile_pool;
    private ExplosionPool _explosion_pool;
    private void Start()
    {
        _projectile_pool = FindObjectOfType<PlayerProjectilePool>();
        _explosion_pool = FindObjectOfType<ExplosionPool>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject explosion = _explosion_pool.GetExplosion("Small");
        explosion.transform.SetPositionAndRotation(transform.position, transform.rotation);
        explosion.SetActive(true);
        _projectile_pool.ReturnProjectile(gameObject);
    }
    private void Update()
    {
        transform.Translate(0f, 20f * Time.deltaTime, 0f);
        if (transform.position.y > 5f)
            _projectile_pool.ReturnProjectile(gameObject);
    }
}

