using UnityEngine;

public class Explosion : MonoBehaviour
{
    ExplosionPool _explosion_pool;
    private void Start() => _explosion_pool = FindObjectOfType<ExplosionPool>();
    public void OnAnimationEnd() => _explosion_pool.ReturnExplosion(gameObject);
}
