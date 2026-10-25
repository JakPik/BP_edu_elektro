using System.Collections;
using UnityEngine;

public struct BranchConectionState
{
    public NodeConectionStates[] nodeConectionStates;
    public NodeConectionStates[] animationStates;
    public MaterialPropertyBlock materialPropertyBlock;
    public Renderer renderer;
    private Coroutine coroutine;
    private MonoBehaviour coroutineRunner;
    private float animDuration;

    public void InitBranchConectionState(GameObject obj, float animDuration, MonoBehaviour coroutineRunner)
    {
        this.coroutineRunner = coroutineRunner;
        this.animDuration = animDuration;
        renderer = obj.GetComponent<Renderer>();
        coroutine = null;
        materialPropertyBlock = new MaterialPropertyBlock();

        nodeConectionStates = new NodeConectionStates[4];
        animationStates = new NodeConectionStates[4];
        for(int i = 0; i < 4; i++)
        {
            nodeConectionStates[i] = NodeConectionStates.EMPTY;
            animationStates[i] = NodeConectionStates.EMPTY;
            materialPropertyBlock.SetColor("_Color_" + i, InteractiveNodeStatesExt.ToColor(nodeConectionStates[i]));
        }
        renderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void UpdateConectionState(bool[] branchConnected, bool? nodeConnected, NodeType type)
    {
        nodeConectionStates[1] = branchConnected[0]? NodeConectionStates.PLACED_COMPONENT : NodeConectionStates.EMPTY;
        nodeConectionStates[3] = branchConnected[1]? NodeConectionStates.PLACED_COMPONENT : NodeConectionStates.EMPTY;
        switch(type)
        {
            case NodeType.BRANCH_OUT:
                if(nodeConnected != null ) nodeConectionStates[0] = nodeConnected.Value? NodeConectionStates.PLACED_COMPONENT : NodeConectionStates.EMPTY;
                nodeConectionStates[2] = branchConnected[2]? NodeConectionStates.PLACED_COMPONENT : NodeConectionStates.EMPTY;
            break;
            case NodeType.BRANCH_IN:
                nodeConectionStates[0] = branchConnected[2]? NodeConectionStates.PLACED_COMPONENT : NodeConectionStates.EMPTY;
                if(nodeConnected != null ) nodeConectionStates[2] = nodeConnected.Value? NodeConectionStates.PLACED_COMPONENT : NodeConectionStates.EMPTY;
            break;
        }
    }

    public void CheckNodeStateChanged()
    {
        for(int i = 0; i < nodeConectionStates.Length; i++)
        {
            if(nodeConectionStates[i] != animationStates[i])
            {
                StartAnimation();
                break;
            }
        }
    }

    public void StartAnimation()
    {
        animationStates = (NodeConectionStates[])nodeConectionStates.Clone();

        if(coroutine != null) coroutineRunner.StopCoroutine(coroutine);
        coroutine = coroutineRunner.StartCoroutine(AnimateColorChange());
    }

    private IEnumerator AnimateColorChange()
    {
        Color[] start = new Color[4];
        Color[] end = new Color[4];

        for(int i = 0; i < 4; i++)
        {
            start[i] = materialPropertyBlock.GetColor("_Color_" + i);
            end[i] = InteractiveNodeStatesExt.ToColor(nodeConectionStates[i]);
        }
        Color blend;
        float elapsed = 0f;
        yield return null;
        while(elapsed != animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            for(int i = 0; i < 4; i++)
            {
                blend = Color.Lerp(start[i], end[i], t);
                materialPropertyBlock.SetColor("_Color_" + i, blend);
            }

            renderer.SetPropertyBlock(materialPropertyBlock);
            yield return null;
        }
        coroutine = null;
    }
}