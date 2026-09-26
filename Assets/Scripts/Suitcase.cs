using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Suitcase : MonoBehaviour
{
    public float openDelay;
    public Sprite openSprite;

    public List<GameObject> enemies;

    GameObject GetRandomEnemy()
    {
        return enemies[Random.Range(0, enemies.Count)];
    }

    void Start()
    {
        StartCoroutine(SuitcaseCoroutine());
    }

    IEnumerator SuitcaseCoroutine()
    {
        yield return new WaitForSeconds(openDelay);
        GetComponent<SpriteRenderer>().sprite = openSprite;
        Instantiate(GetRandomEnemy(), transform.position, Quaternion.identity);
        Destroy(gameObject, openDelay);
    }
}
