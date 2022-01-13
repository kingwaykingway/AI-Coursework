using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum State
{
    Centered = 0, 
    Negative = 1, 
    Positive = 2
}

public enum Action
{
    None = 0,
    MoveLeft = 1,
    MoveRight = 2
}

public class QManager : MonoBehaviour
{
    [Header("Rules")]
    // Critical value among states. 
    // In current version, it is the critical horizontal distance between ball and platform. 
    [SerializeField] [Min(0)] private float stateTolerance = 0f;
    
    // Probability value (between 0 and 1) in which an unexpected action is selected. 
    [SerializeField] [Range(0, 1)] private float mutationRate = 0.1f;
    
    [Header("Platform")]
    // Moving speed of the platform. 
    [SerializeField] [Min(0)] private float movingAgility = 2f;
    public float MovingAgility => movingAgility;
    
    [Header("Ball")]
    // Bound of random horizontal speed each time the ball hits platform. 
    [SerializeField] [Min(0)] private float maxHorizontalVelocity = 100f;
    public float MaxHorizontalVelocity => maxHorizontalVelocity;
    
    private List<List<float>> Q;

    private List<QAgent> _agents;

    void Awake()
    {
        // populate the Q matrix 
        Q = new List<List<float>>(3);
        for (int i = 0; i < 3; i++)
        {
            Q.Add(new List<float>(3));
        }
        for (int i = 0; i < 3; i++)
        {
            Q[0].Add(0.0f);
            Q[1].Add(0.0f);
            Q[2].Add(0.0f);
        }
        
        // Scan for all Q agents
        _agents = new List<QAgent>();
        foreach (var o in GameObject.FindGameObjectsWithTag("Player"))
        {
            QAgent a = o.GetComponent<QAgent>();
            a.Manager = this;
            _agents.Add(a);
        }
    }

    void Update()
    {
        // Evaluate actions performed, calculate reward and update Q matrix. 
        foreach (var agent in _agents)
        {

            float reward = GetReward(agent);
                
            Q[(int) agent.LastState][(int) agent.LastAction] += reward;

            agent.UpdateAgent();
            
            agent.LastState =  GetState(agent);
            agent.LastAction = SelectAction(agent.LastState);
            agent.RunAction(agent.LastAction);
            
            Debug.Log(agent.LastState + ", " + agent.LastAction);
        }
        Debug.Log(GetQMatrixToString());
    }

    private State GetState(QAgent agent)
    {
        if (agent.GetDistanceX() < -stateTolerance)
        {
            return State.Negative;
        }
        else if (agent.GetDistanceX() > stateTolerance)
        {
            return State.Positive;
        }
        else return State.Centered;
    }

    private float GetReward(QAgent agent)
    {
        if (agent.LastState == State.Negative)
        {
            return agent.GetDeltaDistanceX() > 0
                ? 1 : -1;
        }
        if (agent.LastState == State.Positive)
        {
            return agent.GetDeltaDistanceX() < 0
                ? 1 : -1;
        }
        return 0;
    }
    
    public Action SelectAction(State state)
    {
        float bestValue = Q[(int) state][0];
        int a = 0;
        for (int i = 0; i < Q[(int) state].Count; i++)
        {
            if (Q[(int) state][i] > bestValue)
            {
                bestValue = Q[(int) state][i];
                a = i;
            }
            else 
            if (Random.Range(0f, 1f) < mutationRate)
            {
                return (Action)i;
            }
        }
        return (Action) a;
    }
    
    public string GetQMatrixToString()
    {
        string s = "Q matrix: ";
        for (int i = 0; i < Q.Count; i++)
        {
            s += "( ";
            for (int j = 0; j < Q[i].Count; j++)
            {
                // s += (State) i + ", " + (Action) j + ", " + Q[i][j] + "  ";
                s += Q[i][j] + "  ";
            }
            s += ")";
            // s += '\n';
        }
        return s;
    }

    private void OnDisable()
    {
        foreach (var agent in _agents)
        {
            agent.enabled = false;
        }
    }
}
