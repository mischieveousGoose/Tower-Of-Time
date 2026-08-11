using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Roadmap : MonoBehaviour
{
    public int maxConnectionGap;
    public int width, height;
    public GameObject nodeObject;
    public Transform nodeParent;

    public string[] sceneNames;
    public float[] nodeTypeChances;

    public RoadmapNode rootNode;
    public RoadmapNode currentNode;
    public bool resplitPaths;
    List<RoadmapNode> nodes;

    public int minRepaths, maxRepaths;
    RunData runData => GameManager.Instance.runData;

    void Start()
    {
        if (runData.rootNode == null)
        {
            rootNode = GenerateRoadmap();
            currentNode = rootNode;
        }
        else
        {
            rootNode = runData.rootNode;
            currentNode = runData.currentNode;
            nodes = GetNodesList(rootNode);
            CreateNodeGameObjects();
        }
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
            MakeGridPath(GetNode(x, 1), resplitPaths);
        }

        int repaths = UnityEngine.Random.Range(minRepaths, maxRepaths + 1);

        for (int i = 0; i < repaths; i++)
        {
            List<RoadmapNode> available = nodes.FindAll(n => n.hasChecked);
            RoadmapNode recheckNode = available[UnityEngine.Random.Range(0, available.Count)];
            MakeGridPath(recheckNode, true);
        }

        List<RoadmapNode> toRemove = nodes.FindAll(n => !n.hasChecked);
        foreach (var n in toRemove) nodes.Remove(n);

        List<RoadmapNode> lastRow = nodes.FindAll(n => n.y == height - 1);
        RoadmapNode bossNode = new RoadmapNode(NodeType.Boss, width / 2, height);
        foreach (RoadmapNode n in lastRow) n.AddNextNode(bossNode);
        nodes.Add(bossNode);

        CreateNodeGameObjects();
        return rootNode;
    }

    void CreateNodeGameObjects()
    {
        foreach (RoadmapNode node in nodes)
        {
            Instantiate(nodeObject, nodeParent).GetComponent<NodeRenderer>().Setup(node);
        }
    }

    List<RoadmapNode> GetNodesList(RoadmapNode root)
    {
        HashSet<RoadmapNode> output = new HashSet<RoadmapNode>() { root };
        if (root.nextNodes.Count < 1) return output.ToList();

        foreach (RoadmapNode node in root.nextNodes)
        {
            output.UnionWith(GetNodesList(node));
        }
        return output.ToList();

    }

    public void SelectNode(RoadmapNode node)
    {
        if (currentNode.nextNodes.Contains(node))
        {
            currentNode = node;
            GameManager.Instance.ContinueFromRoadmap(sceneNames[(int)node.nodeType]);
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

    void MakeGridPath(RoadmapNode startNode, bool ignoreRechecks = false)
    {
        int x = startNode.x;
        int y = startNode.y;

        while (y < height)
        {
            RoadmapNode thisNode = GetNode(x, y);
            thisNode.hasChecked = true;

            if (y == height - 1) return;

            int minStep = Mathf.Max(-maxConnectionGap, -x);
            int maxStep = Mathf.Min(maxConnectionGap, width - 1 - x);


            int nextDirection = UnityEngine.Random.Range(minStep, maxStep + 1);

            RoadmapNode nextNode = GetNode(x + nextDirection, y + 1);
            if (nextNode == null) return;

            thisNode.AddNextNode(nextNode);
            bool mergedIntoExistingPath = nextNode.hasChecked;

            x += nextDirection;
            y++;

            if (mergedIntoExistingPath && !ignoreRechecks) return;
        }
    }

    NodeType RandomNodeType()
    {
        float counter = 0;
        float rand = UnityEngine.Random.Range(0f, 100f);

        for (int i = 0; i < nodeTypeChances.Length; i++)
        {
            counter += nodeTypeChances[i];
            if (rand <= counter)
            {
                return (NodeType)i;
            }
        }

        Debug.Log("Random node Type Generating is broken");
        return NodeType.Level;
    }
}
