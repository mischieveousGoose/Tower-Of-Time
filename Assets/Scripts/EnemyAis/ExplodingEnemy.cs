using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{

    SFXManager sfxManager;
    public GameObject explosion;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        sfxManager.PlaySFX(4);
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 1f);
        Destroy(gameObject);
    }


}
