using UnityEngine;

public class EnemySmall : MonoBehaviour
{
    private GameObject _main_ship;
    private GameObject _game_ender;
    private EnemyPool _enemy_pool;
    private Transform _transform;
    private ExplosionPool _explosion_pool;

    private bool _score_incremented;
    private readonly float _move_speed = 4f;

    private void Start()
    {
        _main_ship = GameObject.Find("main_ship");
        // Workaround to get an inactive gameobject
        _game_ender = GameObject.Find("GameEnderHolder").transform.Find("GameEnder").gameObject;
        _enemy_pool = FindObjectOfType<EnemyPool>();
        _explosion_pool = FindObjectOfType<ExplosionPool>();
        _transform = transform;
    }
    private void Update()
    {
        _transform.Translate(0f, _move_speed * Time.deltaTime, 0f);
        if (_transform.position.y < -7f)
            ReturnToPool();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == _main_ship)
        {
            _main_ship.SetActive(false);
            GameObject explosion = _explosion_pool.GetExplosion("Medium");
            explosion.transform.SetPositionAndRotation(_main_ship.transform.position, _main_ship.transform.rotation);
            explosion.SetActive(true);
            _game_ender.SetActive(true);
        }
        if (!_score_incremented)
        {
            GameObject explosion = _explosion_pool.GetExplosion("Medium");
            explosion.transform.SetPositionAndRotation(_transform.position, _transform.rotation);
            explosion.SetActive(true);
            ScoreKeeper.score++;
            _score_incremented = true;
        }
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        _score_incremented = false;
        _enemy_pool.ReturnEnemy(gameObject);
    }
}
