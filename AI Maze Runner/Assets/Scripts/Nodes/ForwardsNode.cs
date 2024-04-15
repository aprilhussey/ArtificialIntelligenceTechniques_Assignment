using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardsNode : Node
{
    private float moveSpeed;
    private Rigidbody rigidBody;
    private Transform transform;

    public ForwardsNode (Rigidbody rigidBody, Transform transform, float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.rigidBody = rigidBody;
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluate Forwards");
        // Set everything nil to begin with
        this.rigidBody.velocity = Vector3.zero;
        this.rigidBody.angularVelocity = Vector3.zero;
        // Make the NPC move forward at the defined speed
        this.rigidBody.velocity = this.transform.forward * this.moveSpeed;
        return NodeState.SUCCESS;
    }
}
