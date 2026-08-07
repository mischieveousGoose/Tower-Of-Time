
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeRenderer : MonoBehaviour
{
    Roadmap roadmap;
    public List<Sprite> icons;
    LineRenderer lr;
    SpriteRenderer sr;

    RoadmapNode node;

    public void Setup(RoadmapNode thisNode)
    {
        node = thisNode;
        roadmap = FindFirstObjectByType<Roadmap>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = icons[(int)node.nodeType];
        transform.position = new Vector3(node.x - (roadmap.width / 2), node.y, 0f);
        lr = GetComponent<LineRenderer>();
        lr.positionCount = node.nextNodes.Count * 2;

        for (int i = 0; i < node.nextNodes.Count; i++)
        {
            RoadmapNode nextNode = node.nextNodes[i];
            // Node Point
            lr.SetPosition(i * 2, transform.position);
            lr.SetPosition((i * 2) + 1, new Vector3(nextNode.x - (roadmap.width / 2), nextNode.y, 0f));

        }
    }

    void Update()
    {
        if (roadmap.currentNode == node)
        {
            sr.color = Color.yellow;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    void OnMouseUpAsButton()
    {
        roadmap.SelectNode(node);
    }

}
