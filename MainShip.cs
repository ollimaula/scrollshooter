using UnityEngine;

public class MainShip : MonoBehaviour
{
    [SerializeField] private PlayerProjectilePool projectile_pool;
    [SerializeField] private Transform _left_cannon_transform;
    [SerializeField] private Transform _right_cannon_transform;
    [SerializeField] private Sprite _thrust;
    [SerializeField] private Sprite _no_thrust;
    [SerializeField] private Sprite[] heatbar;
    [SerializeField] private AudioSource turret_sound;

    private SpriteRenderer ship;
    private CanvasOverlay canvas_overlay;

    private readonly float _fall_speed = -4f;
    private readonly float _move_speed = 8f;
    private readonly float _spacebar_cd = 0.25f;
    private readonly float _gunheat_increment = 1f;
    private bool _gun_overheated = false;
    private float _gunheat = 0f;
    private float _spacebar_counter = 0f;

    private void Start()
    {
        canvas_overlay = GameObject.Find("Canvas").GetComponent<CanvasOverlay>();
        ship = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        Movement();
        Combat();
        Heatbar();
    }
    private void Movement()
    {
        float move_horizontal = Input.GetAxis("Horizontal");
        float move_vertical = Input.GetAxis("Vertical");
        transform.Translate(0f, move_vertical * _move_speed * Time.deltaTime, 0f);
        transform.Translate(move_horizontal * _move_speed * Time.deltaTime, 0f, 0f);
        if (move_vertical <= 0f && transform.position.y > -4f)
            transform.Translate(0f, _fall_speed * Time.deltaTime, 0f);

        if (Input.GetKeyDown(KeyCode.W) && !canvas_overlay.paused)
            ship.sprite = _thrust;
        else if (Input.GetKeyUp(KeyCode.W) && !canvas_overlay.paused)
            ship.sprite = _no_thrust;
    }
    private void Combat()
    {
        if (Input.GetKey(KeyCode.Space) && _spacebar_counter <= 0f && !_gun_overheated && !canvas_overlay.paused)
        {
            GameObject left_projectile = projectile_pool.GetProjectile();
            left_projectile.transform.SetPositionAndRotation(_left_cannon_transform.position, _left_cannon_transform.rotation);
            left_projectile.SetActive(true);

            GameObject right_projectile = projectile_pool.GetProjectile();
            right_projectile.transform.SetPositionAndRotation(_right_cannon_transform.position, _right_cannon_transform.rotation);
            right_projectile.SetActive(true);

            _spacebar_counter = _spacebar_cd;
            _gunheat += _gunheat_increment;
            turret_sound.Play();
        }
        else
        {
            _spacebar_counter -= Time.deltaTime;
            if ((_gunheat >= 0f && !Input.GetKey(KeyCode.Space)) || _gun_overheated && !canvas_overlay.paused)
                _gunheat -= 3 * Time.deltaTime;
        }

        if (_gunheat > 15f)
            _gun_overheated = true;
        else if (_gunheat <= 0f)
            _gun_overheated = false;
    }
    private void Heatbar()
    {
        if (_gunheat <= 0f)
            canvas_overlay.heatbar_sprite = heatbar[0];
        else
        {
            for (int i = 1; i <= 16; i++)
            {
                if (_gunheat >= (i - 1) && _gunheat < i)
                {
                    canvas_overlay.heatbar_sprite = heatbar[i];
                    break;
                }
            }
        }
    }
}
