using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLeft180Node : Node
{
    private Transform transform;

    public RotateLeft180Node(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluate RotateLeft180");
        transform.Rotate(0, -180, 0);
        return NodeState.SUCCESS;
    }
}
