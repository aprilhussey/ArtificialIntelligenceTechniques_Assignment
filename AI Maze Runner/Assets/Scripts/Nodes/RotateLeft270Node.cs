using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLeft270Node : Node
{
    private Transform transform;

    public RotateLeft270Node(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluate RotateLeft270");
        transform.Rotate(0, -270, 0);
        return NodeState.SUCCESS;
    }
}
