using UnityEngine;

public class EnemyMedium : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private AudioSource turret_sound;

    private GameObject _main_ship;
    private GameObject _right_cannon;
    private GameObject _left_cannon;
    private GameObject _game_ender;
    private EnemyPool _enemy_pool;
    private EnemyProjectilePool _projectile_pool;
    private Transform _transform;
    private Transform _left_cannon_transform;
    private Transform _right_cannon_transform;
    private ExplosionPool _explosion_pool;

    private int health = 4;
    private bool _score_incremented;
    private readonly float _move_speed = 1f;
    private float _cooldown = 5f;

    private void Start()
    {
        _right_cannon = transform.Find("right_cannon").gameObject;
        _left_cannon = transform.Find("left_cannon").gameObject;
        _main_ship = GameObject.Find("main_ship");
        // Workaround to get an inactive gameobject
        _game_ender = GameObject.Find("GameEnderHolder").transform.Find("GameEnder").gameObject;
        _enemy_pool = FindObjectOfType<EnemyPool>();
        _projectile_pool = FindObjectOfType<EnemyProjectilePool>();
        _explosion_pool = FindObjectOfType<ExplosionPool>();
        _transform = transform;
        _left_cannon_transform = _left_cannon.transform;
        _right_cannon_transform = _right_cannon.transform;
    }
    void Update()
    {
        if (_cooldown <= 0f && _transform.position.y < 5f)
        {
            GameObject left_projectile = _projectile_pool.GetProjectile();
            left_projectile.transform.SetPositionAndRotation(_left_cannon_transform.position, _left_cannon_transform.rotation);
            left_projectile.SetActive(true);
            GameObject right_projectile = _projectile_pool.GetProjectile();
            right_projectile.transform.SetPositionAndRotation(_right_cannon_transform.position, _right_cannon_transform.rotation);
            right_projectile.SetActive(true);
            turret_sound.Play();
            _cooldown = 2f;
        }
        _transform.Translate(0f, _move_speed * Time.deltaTime, 0f);
        _cooldown -= Time.deltaTime;
        if (_transform.position.y < -7f)
            ReturnToPool();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        health--;
        if (collision.gameObject == _main_ship)
        {
            Destroy(_main_ship);
            GameObject explosion = _explosion_pool.GetExplosion("Medium");
            explosion.transform.SetPositionAndRotation(_main_ship.transform.position, _main_ship.transform.rotation);
            explosion.SetActive(true);
            _game_ender.SetActive(true);
        }
        if (health <= 0 && !_score_incremented)
        {
            ReturnToPool();
            GameObject explosion = _explosion_pool.GetExplosion("Medium");
            explosion.transform.SetPositionAndRotation(_transform.position, _transform.rotation);
            explosion.SetActive(true);
            ScoreKeeper.score += 4;
        }
    }
    private void ReturnToPool()
    {
        health = 4;
        _score_incremented = false;
        _enemy_pool.ReturnEnemy(gameObject);
    }
}
