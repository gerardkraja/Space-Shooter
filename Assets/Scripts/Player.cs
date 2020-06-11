using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //params
    [Header("Player Movement")]
    [Range(1, 20)] [SerializeField] int speedUpFactor = 12;
    [SerializeField] float padding = 0.5f;
    [SerializeField] int health = 200;
    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.2f;
    Coroutine firingCoroutine;
    float xMin, xMax, yMin, yMax;
    [SerializeField] AudioClip explosionSound;
    [Range(0f, 1f)] [SerializeField] float explosionSoundVolume = 0.7f;
    GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        gameSession = FindObjectOfType<GameSession>();
        gameSession.SetHealth(health);
    }



    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());

        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }
    private IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
                laserPrefab,
                transform.position,
                Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        float frameDelay = Time.deltaTime;
        float deltaX = Input.GetAxis("Horizontal") * frameDelay * speedUpFactor;
        float deltaY = Input.GetAxis("Vertical") * frameDelay * speedUpFactor;

        float newXPos = Mathf.Clamp(deltaX + transform.position.x, xMin, xMax);
        float newYPos = Mathf.Clamp(deltaY + transform.position.y, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        gameSession.SetHealth(health);
        if (health <= 0)
        {
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
            FindObjectOfType<Level>().LoadGameOver();
        }
    }
}
