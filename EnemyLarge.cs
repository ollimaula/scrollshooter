using System;
using UnityEngine;

public class EnemyLarge : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private AudioSource turret_sound;

    private Transform _main_ship_transform;
    private GameObject _game_ender;
    private Transform _turret;
    private Transform _transform;
    private Transform _turret_muzzle;
    private EnemyPool _enemy_pool;
    private EnemyProjectilePool _projectile_pool;
    private ExplosionPool _explosion_pool;

    private int health = 10;
    private bool _score_incremented;
    private readonly float _move_speed = 0.3f;
    private float _cooldown = 4f;

    private void Start()
    {
        _transform = transform;
        try { _main_ship_transform = GameObject.Find("main_ship").transform; }
        catch { _main_ship_transform = null; }
        _turret = _transform.Find("turret");
        _turret_muzzle = _turret.Find("turret_muzzle");
        // Workaround to get an inactive gameobject
        _game_ender = GameObject.Find("GameEnderHolder").transform.Find("GameEnder").gameObject;
        _enemy_pool = FindObjectOfType<EnemyPool>();
        _projectile_pool = FindObjectOfType<EnemyProjectilePool>();
        _explosion_pool = FindObjectOfType<ExplosionPool>();
    }
    private void Update()
    {
        if (_cooldown <= 0f && _transform.position.y < 5f && _transform.position.y > -5f)
        {
            try
            {
                GameObject projectile = _projectile_pool.GetProjectile();
                projectile.transform.SetPositionAndRotation(_turret_muzzle.position, _turret_muzzle.rotation);
                projectile.SetActive(true);
                turret_sound.Play();
            }
            catch (Exception e) { Debug.Log(e.Message); }
            _cooldown = 4f;
        }
        _transform.Translate(0f, _move_speed * Time.deltaTime, 0f);
        _cooldown -= Time.deltaTime;
        if (_transform.position.y < -7f)
            ReturnToPool();
        Turret();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        health--;
        if (collision.gameObject == _main_ship_transform.gameObject)
        {
            Destroy(_main_ship_transform.gameObject);
            GameObject explosion = _explosion_pool.GetExplosion("Medium");
            explosion.transform.SetPositionAndRotation(_main_ship_transform.position, _main_ship_transform.rotation);
            explosion.SetActive(true);
            _game_ender.SetActive(true);
        }
        if (health <= 0 && !_score_incremented)
        {
            ReturnToPool();
            GameObject explosion = _explosion_pool.GetExplosion("Large");
            explosion.transform.SetPositionAndRotation(_transform.position, _transform.rotation);
            explosion.SetActive(true);
            ScoreKeeper.score += 10;
        }
    }
    private void Turret()
    {
        if (_main_ship_transform != null)
        {
            Vector3 direction = _main_ship_transform.position - _turret.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            _turret.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }
    }
    private void ReturnToPool()
    {
        health = 10;
        _score_incremented = false;
        _enemy_pool.ReturnEnemy(gameObject);
    }
}
