using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopNode : Node
{
    private Transform transform;
    private Rigidbody rigidBody;

    public StopNode(Transform transform, Rigidbody rigidbody)
    {
        this.transform = transform;
        this.rigidBody = rigidbody;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluate Stop");
        this.rigidBody.velocity = Vector3.zero;
        this.rigidBody.angularVelocity = Vector3.zero;
        //transform.rotation = Quaternion.identity;
        return NodeState.SUCCESS;
    }
}
