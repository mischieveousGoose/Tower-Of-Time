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

    public RunData(float startTime, GameObject wp)
    {
        level = 1;
        time = startTime;

        primaryWeapon = wp;
        secondaryWeapon = null;

        rootNode = null;
        currentNode = null;
    }
}
