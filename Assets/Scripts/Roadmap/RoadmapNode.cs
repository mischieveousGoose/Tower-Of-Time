using System.Collections.Generic;
using UnityEngine;

public class RoadmapNode
{
    public NodeType nodeType;
    public List<RoadmapNode> nextNodes;
    public int x, y;

    public bool hasChecked;

    public RoadmapNode(NodeType nodeType, int x, int y)
    {
        this.nodeType = nodeType;
        nextNodes = new List<RoadmapNode>();

        this.x = x;
        this.y = y;

        hasChecked = false;
    }


    public RoadmapNode()
    {
        nodeType = NodeType.Level;
        nextNodes = new List<RoadmapNode>();

        x = 0;
        y = 0;

        hasChecked = false;
    }

    public void AddNextNode(RoadmapNode node)
    {
        if (nextNodes.Contains(node)) return;
        nextNodes.Add(node);
    }
}

public enum NodeType
{
    Level,
    Shop,
    CombatEncounter,
    Boss,
}
