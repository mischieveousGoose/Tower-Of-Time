using Unity.VisualScripting;
using UnityEngine;

public class RunData
{
    public int level;
    public float time;

    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    public RoadmapNode rootNode;
    public RoadmapNode currentNode;

    public float levelTimer;

    public Character selectedCharacter;

    public RunData(float startTime, GameObject wp)
    {
        levelTimer = 0;
        level = 1;
        time = startTime;

        primaryWeapon = wp;
        secondaryWeapon = null;

        rootNode = null;
        currentNode = null;

        selectedCharacter = null;
    }
}
