using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLeft90Node : Node
{
    private Transform transform;

    public RotateLeft90Node(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluate RotateLeft90");
        transform.Rotate(0, -90, 0);
        return NodeState.SUCCESS;
    }
}
