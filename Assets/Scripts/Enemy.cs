using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] float health = 100;
    [SerializeField] int pointsPerHit = 100;
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [Header("Related VFX")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject explosionFX;
    [SerializeField] float durationOfExplosion = 1f;
    [Header("Audio")]
    [SerializeField] AudioClip explosionSound;
    [Range(0f, 1f)] [SerializeField] float explosionSoundVolume = 0.5f;
    [SerializeField] AudioClip laserSound;
    [Range(0f, 1f)] [SerializeField] float laserSoundVolume = 0.5f;
    //cached components
    GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
                projectile,
                transform.position,
                Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserSoundVolume);
        //projectile speed has a - before it to shoot downwards
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
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
        gameSession.AddToScore(pointsPerHit);
        if (health <= 0)
        {
            DestroyEnemy();

        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        GameObject explosionParticles = Instantiate(
            explosionFX,
            transform.position,
            Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionSoundVolume);
        Destroy(explosionParticles, durationOfExplosion);
    }
}
