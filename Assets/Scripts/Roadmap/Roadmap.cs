using System;
using System.Collections.Generic;
using UnityEngine;


public class Roadmap : MonoBehaviour
{
    public int width, height;
    public GameObject nodeObject;
    public Transform nodeParent;

    RoadmapNode rootNode;
    public RoadmapNode currentNode;
    List<RoadmapNode> nodes;

    void Start()
    {
        rootNode = GenerateRoadmap();
        currentNode = rootNode;
    }

    RoadmapNode GenerateRoadmap()
    {
        rootNode = new RoadmapNode(NodeType.Level, width / 2, 0);
        CreateNodeGrid();
        nodes.Add(rootNode);
        rootNode.hasChecked = true;

        for (int x = 0; x < width; x++)
        {
            rootNode.AddNextNode(GetNode(x, 1));
            MakeGridPath(GetNode(x, 1));
        }

        rootNode.AddNextNode(GetNode(width / 2, 1));

        List<RoadmapNode> toRemove = nodes.FindAll(n => !n.hasChecked);
        foreach (var n in toRemove) nodes.Remove(n);

        foreach (RoadmapNode node in nodes)
        {
            Instantiate(nodeObject, nodeParent).GetComponent<NodeRenderer>().Setup(node);
        }

        return rootNode;
    }

    public void SelectNode(RoadmapNode node)
    {
        if (currentNode.nextNodes.Contains(node))
        {
            currentNode = node;
        }
    }

    RoadmapNode GetNode(int x, int y)
    {
        foreach (RoadmapNode node in nodes)
        {
            if (node.x == x && node.y == y) return node;
        }
        return null;
    }

    void CreateNodeGrid()
    {
        // Creates a grid of nodes to prepare for generation
        nodes = new List<RoadmapNode>();
        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                RoadmapNode node = new RoadmapNode(RandomNodeType(), x, y);
                nodes.Add(node);
            }
        }
    }

    void MakeGridPath(RoadmapNode startNode)
    {
        int x = startNode.x;
        int y = startNode.y;

        while (y < height)
        {
            RoadmapNode thisNode = GetNode(x, y);
            thisNode.hasChecked = true;

            if (y == height - 1) return;

            int nextDirection;
            if (x == 0) nextDirection = UnityEngine.Random.Range(0, 2);
            else if (x + 1 == width) nextDirection = UnityEngine.Random.Range(-1, 1);
            else nextDirection = UnityEngine.Random.Range(-1, 2);

            RoadmapNode nextNode = GetNode(x + nextDirection, y + 1);
            if (nextNode == null) return;

            thisNode.AddNextNode(nextNode);
            bool mergedIntoExistingPath = nextNode.hasChecked;

            x += nextDirection;
            y++;

            if (mergedIntoExistingPath) return;
        }
    }

    NodeType RandomNodeType()
    {
        int length = Enum.GetValues(typeof(NodeType)).Length;
        return (NodeType)UnityEngine.Random.Range(0, length);
    }
}
