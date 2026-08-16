using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class LevelSpawner : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject endRoom;
    [HideInInspector] public int levelLength;
    public List<GameObject> rooms;

    public int startRooms;
    public int increaseAmount;



    void GenerateLevel()
    {
        Transform endTransform;
        endTransform = Instantiate(startRoom, Vector3.zero, Quaternion.identity).transform.GetChild(0);
        for (int i = 0; i < levelLength; i++)
        {
            endTransform = Instantiate(rooms[Random.Range(0, rooms.Count)], endTransform.position, Quaternion.identity).transform.GetChild(0);
        }

        Instantiate(endRoom, endTransform.position, Quaternion.identity);
    }

    void Start()
    {
        int level = GameManager.Instance.runData.level;
        levelLength = startRooms + ((level - 1) * increaseAmount);
        GenerateLevel();


    }

}
