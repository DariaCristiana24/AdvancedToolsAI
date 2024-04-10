using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField]
    float speed = 5;
    [SerializeField]
    Transform target;
    [SerializeField]
    Renderer floor;
    [SerializeField]
    List<Transform> legJoints = new List<Transform>();

    public override void OnEpisodeBegin()
    {
       // transform.localPosition = Vector3.zero;
       transform.localPosition = Vector3.zero;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.position);
        foreach (var joint in legJoints)
        {
            sensor.AddObservation(joint.rotation);
        }


    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //float moveX = actions.ContinuousActions[0];
        //float moveZ = actions.ContinuousActions[1];

        //transform.position += new Vector3(moveX, 0, moveZ)* Time.deltaTime* speed;

        float rotX = actions.ContinuousActions[0];
        float rotZ = actions.ContinuousActions[1];
        int leg = actions.DiscreteActions[0];

        Quaternion rot = Quaternion.Euler(rotX*30 * speed, 0, rotZ*30*speed);

        //legJoints[leg].rotation =  rot;
        legJoints[leg].rotation = Quaternion.Lerp(legJoints[leg].rotation, rot, Time.time * speed);
        //transform.position += new Vector3(moveX, 0, moveZ)* Time.deltaTime* speed;
    }

    private void Update()
    {
        if(transform.localPosition.x > 0)
        {
            SetReward(0.001f * transform.localPosition.x);
        }
        if (transform.position.y < -10)
        {
            SetReward(-10); 
            floor.material.color = Color.red;
            EndEpisode();
        }

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
