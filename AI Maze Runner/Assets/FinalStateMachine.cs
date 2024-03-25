using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalStateMachine : MonoBehaviour
{
    // State definitions
    // PatrolIdle is the initial state, PatrolForward means moving forwards, PatrolLeft90 means moving left 90 degrees, PatrolLeft180 means turning around, PatrolLeft90TurnAround is for turning the last 90 to turn around.
    // CoinIdle is the initial state when a coin is visible, CoinRight means turning right to face a coin, CoinLeft means turning left to face a coin, CoinForward means moving forwards to a coin.
    enum State { PatrolIdle, PatrolForward, PatrolLeft90, PatrolLeft180, PatrolLeft90TurnAround, CoinIdle, CoinRight, CoinLeft, CoinForward };

    // Direction a coin is in
    enum CoinDirection { Left, Right, Ahead };

    // The Main NPC object
    private GameObject npc;

    private State currentState;

    // The coin that is presently targetted for collection
    private GameObject targetCoin;

    // Start is called before the first frame update
    void Start()
    {
        npc = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called once per Physics frame by default is every 0.02 seconds or rather 50 calls per second.
    void FixedUpdate()
    {
        this.ProcessStateMachine()
    }

    void ProcessStateMachine()
    {
        switch (currentState)
        {
            case State.PatrolIdle:
                if (this.CoinVisible())
                {
                    // Coin spotted change state to CoinIdle
                    currentState = State.CoinIdle;
                } 
                else
                {
                    // Coin not seen start patrol!
                    currentState = State.PatrolForward;
                }
                break;
            case State.PatrolForward:
                if (this.CoinVisible())
                {
                    currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    currentState = State.PatrolLeft90;
                }
                break;
            case State.PatrolLeft90:
                if (this.CoinVisible())
                {
                    currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    currentState = State.PatrolLeft180;
                }
                break;
            case State.PatrolLeft180:
                if (this.CoinVisible())
                {
                    currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    currentState = State.PatrolLeft90TurnAround;
                }
                break;
            case State.PatrolLeft90TurnAround:
                if (this.CoinVisible())
                {
                    currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    currentState = State.PatrolLeft90TurnAround;
                }
                break;
            case State.CoinIdle:
            case State.CoinRight:
            case State.CoinLeft:
                this.ProcessCoinCollectState();
                break;
            case State.CoinForward:
                if (this.CoinCollected())
                {

                } 
                else if (this.EnoughCoinsCollected())
                {

                }
            default:
                Debug.Log("State is invalid.");
        }
    }

    void ProcessCoinCollectState()
    {
        CoinDirection direction = GetCoinDirection();
        switch (direction)
        {
            case CoinDirection.Left:
                currentState = State.CoinLeft;
                break;
            case CoinDirection.Right:
                currentState = State.CoinRight;
                break;
            case CoinDirection.Ahead:
                currentState = State.CoinForward;
                break;
            default:
                Debug.Log("Coin Direction was not valid");
        }
    }

    bool CoinVisible()
    {
        // Determine if a coin is visible and if so, set it as the target and return true, else return false.
        // A coin is visible if it is visible without walls in the way for half the coin, use the position of the coin to determine if half is visible from the position of the capsule, both are assumed to be in the middle.
    }

    CoinDirection GetCoinDirection()
    {
        // Return what direction the coin is in, if it is within where the collider of the capsule will be when it gets to that position return ahead, if it's to the left return left, and if it's to the right return right.
    }

    bool ObstacleVisible()
    {
        // If a none coin obstacle is directly infront of the NPC less than half the distance of a maze cell's length
    }

    bool CoinCollected()
    {
        // Is a coin needing to be collected?
    }

    bool CoinAhead()
    {
        // Is there a coin ahead?
    }

    bool EnoughCoinsCollected()
    {
        // Has the NPC found enough coins to be successful yet?
    }
}
