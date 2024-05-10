using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class MoveToGoalAgent : Agent
{
    [SerializeField]
    float speed = 1;
    [SerializeField]
    Transform target;
    [SerializeField]
    Renderer floor;


    public override void OnEpisodeBegin()
    {
       // transform.localPosition = Vector3.zero;
       transform.localPosition = Vector3.zero;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        //int act = actions.DiscreteActions[0];

           transform.position += new Vector3(moveX, 0, moveZ)* Time.deltaTime* speed;
       // transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * speed;
        Debug.Log(moveX);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Reward")
        {
            floor.material.color = Color.green;
            SetReward(1f);
            
            EndEpisode();
        }
        if (other.tag == "Obstacle")
        {
            SetReward(-1f);
            floor.material.color = Color.red;
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> act = actionsOut.ContinuousActions;
        act[0] = Input.GetAxisRaw("Horizontal") * speed;
        act[1] = Input.GetAxisRaw("Vertical") *speed;
    }
}
