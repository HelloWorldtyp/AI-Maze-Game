using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

public class MoveToTarget : Agent
{

    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private TextMeshProUGUI scoreboardText;

    private int count = 0;

    private void Awake() {
        scoreboardText.SetText(count.ToString());
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(Random.Range(-3.5f, -1.5f), Random.Range(-3.5f, 3.5f));
        target.position = new Vector3(Random.Range(1.5f, 3.5f), Random.Range(-3.5f, 3.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)target.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float movementSpeed = 5f;

        transform.position += new Vector3(moveX, moveY) * Time.deltaTime * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Target")
        {
            AddReward(10f);
            count++;
            scoreboardText.SetText(count.ToString());
            backgroundSpriteRenderer.color = Color.green;
            EndEpisode();
        }
        else if (collision.name == "Walls")
        {
            AddReward(-2f);
            count--;
            scoreboardText.SetText(count.ToString());
            backgroundSpriteRenderer.color = Color.red;
            EndEpisode();

        }
    }
}
