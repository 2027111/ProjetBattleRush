using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{ 
        
    [SerializeField] private float timeElapsed = 0f;
    [SerializeField] private float timeToReachTarget = 0.05f;    
    [SerializeField] private float movementThreshold= 0.05f;


    private readonly List<TransformUpdates> futureTransformUpdates = new List<TransformUpdates>();

    private float squareMovementThreshold;
    private TransformUpdates to;
    private TransformUpdates from;
    private TransformUpdates previous;



    private void Start()
    {
        squareMovementThreshold = movementThreshold * movementThreshold;
        to = new TransformUpdates(NetworkManager.Singleton.ServerTick, transform.position);
        from = new TransformUpdates(NetworkManager.Singleton.ServerTick, transform.position);
        previous = new TransformUpdates(NetworkManager.Singleton.ServerTick, transform.position);


    }

/*
    private void Update()
    {
        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            Debug.Log("Pre Check : " + to.Position + "   " + from.Position);
            if (NetworkManager.Singleton.ServerTick >= futureTransformUpdates[i].Tick)
            {
                previous = to;
                to = futureTransformUpdates[i];
                from = new TransformUpdates(NetworkManager.Singleton.ServerTick, transform.position);
                
                futureTransformUpdates.RemoveAt(i);
                i--;
                timeElapsed = 0f;
                timeToReachTarget = (to.Tick - from.Tick) * Time.fixedDeltaTime;
            }

            Debug.Log("Post Check : " + to.Position + "   " + from.Position);

        }

        timeElapsed += Time.deltaTime;
        InterpolatePosition(timeElapsed / timeToReachTarget);
        
    }
*/

    private void InterpolatePosition(float lerpAmount)
    {
        if ((to.Position - previous.Position).sqrMagnitude < squareMovementThreshold)
        {
            if (to.Position != from.Position)
            {
                transform.position = Vector3.Lerp(from.Position, to.Position, lerpAmount);
                return;
            }
        }
        transform.position = Vector3.LerpUnclamped(from.Position, to.Position, lerpAmount);
    }

    public void NewUpdate(uint tick, Vector3 position)
    {
        if (tick <= NetworkManager.Singleton.InterpolationTick)
        {
            Debug.Log("Tick is under or equal to InterpolationTick");
            return;
        }

        for(int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (tick < futureTransformUpdates[i].Tick)
            {
                futureTransformUpdates.Insert(i, new TransformUpdates(tick, position));
                return;
            }
        }
        futureTransformUpdates.Add(new TransformUpdates(tick, position));
    }

}
