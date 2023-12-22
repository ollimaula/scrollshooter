using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Transform _transform;
    private GameObject _main_ship;
    private GameObject _game_ender;
    private EnemyProjectilePool _pool;
    private ExplosionPool _explosion_pool;

    private void Start()
    {
        _transform = transform;
        _main_ship = GameObject.Find("main_ship");
        // Workaround to get an inactive gameobject
        _game_ender = GameObject.Find("GameEnderHolder").transform.Find("GameEnder").gameObject;
        _pool = FindObjectOfType<EnemyProjectilePool>();
        _explosion_pool = FindObjectOfType<ExplosionPool>();
    }
    private void Update()
    {
        _transform.Translate(0f, 5f * Time.deltaTime, 0f);
        if (_transform.position.y < -7f)
            ReturnToPool();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReturnToPool();
        GameObject explosion = _explosion_pool.GetExplosion("Small");
        explosion.transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        explosion.SetActive(true);
        if (collision.gameObject == _main_ship)
        {
            _main_ship.gameObject.SetActive(false);
            GameObject player_explosion = _explosion_pool.GetExplosion("Medium");
            player_explosion.transform.SetPositionAndRotation(_main_ship.transform.position, _main_ship.transform.rotation);
            player_explosion.SetActive(true);
            _game_ender.SetActive(true);
        }
    }
    private void ReturnToPool() => _pool.ReturnProjectile(gameObject);
}
