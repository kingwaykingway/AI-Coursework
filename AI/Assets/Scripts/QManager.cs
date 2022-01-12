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
    RotateCounterclockwise = 1,
    RotateClockwise = 2
}

public class QManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float stateTolerance = 0f;
    
    [SerializeField] [Min(0)] private float speedTolerance = 0;
    
    // Maximum degree of rotation per frame, in absolute value. 
    // In current version, directly stands for angles to rotate per frame. 
    [SerializeField] [Range(0, 90)] private float rotationAgility = 10f;
    public float RotationAgility => rotationAgility;
    
    // 
    // [SerializeField] [Min(0)] private float accelerationTolerance = 0.5f;

    [SerializeField] [Range(0, 1)] private float actionInterval = 0.05f;
    
    // Number of subdivision into Q states. Higher value means better precision and more CPU demanding. 
    // [SerializeField] [Min(1)] private int _subdivisionSegments = 2;
    // public int SubdivisionSegments => _subdivisionSegments;

    [SerializeField] [Min(0)] private float rewardMultiplier = 1;
    
    private List<List<float>> Q;
    public List<List<float>> QMatrix => Q;

    private float _timer;

    private List<QAgent> _agents;


    void Start()
    {
        // populate the Q matrix 
        Q = new List<List<float>>(3);
        for (int i = 0; i < 3; i++)
        {
            // Q.Add(new List<float>(SubdivisionSegments + 1));
            Q.Add(new List<float>(3));
        }
        // for (int i = 0; i < SubdivisionSegments + 1; i++)
        for (int i = 0; i < 3; i++)
        {
            // Hard coded version for when there are only 3 actions. 
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

    void FixedUpdate()
    {
        /*_timer += Time.deltaTime;
        if (_timer < actionInterval)
        {
            return;
        }*/
        
        
        // Evaluate actions performed, calculate reward and update Q matrix. 
        foreach (var agent in _agents)
        {

            float reward = GetReward(agent);
                // GetReward(agent.LastState, lastAccelerationX, accelerationX) * rewardMultiplier * Time.deltaTime;
                
            Q[(int) agent.LastState][(int) agent.LastAction] += reward;

            Debug.Log(agent.GetDeltaVelocityX());
            // Debug.Log(agent.GetRelativePositionX() + ", " + agent.GetDeltaVelocityX());
            /*Debug.Log(agent.GetDeltaVelocityX() + " - " + agent._lastDeltaVelocityX 
                      + " = " + (agent.GetDeltaVelocityX() - agent._lastDeltaVelocityX));   */         
            // Debug.Log(agent._lastDeltaVelocityX);
            
            agent.UpdatePhysicalStates();
            
            agent.LastState =  GetState(agent);
            agent.LastAction = SelectAction(agent.LastState);
            // agent.RunAction(agent.LastAction);

            // Debug.Log(agent.LastState + ", " + agent.LastAction);

            
            // _timer = 0f;
        }
        Debug.Log(ToString());
    }

    private State GetState(QAgent agent)
    {
        if (agent.GetRelativePositionX() < -stateTolerance)
        {
            return State.Negative;
        }
        else if (agent.GetRelativePositionX() > stateTolerance)
        {
            return State.Positive;
        }
        else return State.Centered;
    }

    private float GetReward(QAgent agent)
    {
        // float v = agent.GetDeltaVelocityX();
        if (agent.LastState == State.Negative)
        {
            return agent.GetDeltaVelocityX() > -speedTolerance
                // ? Mathf.Abs(agent.GetDeltaVelocityX())
                // : -Mathf.Abs(agent.GetDeltaVelocityX());
                ? 1 : -1;
        }
        if (agent.LastState == State.Positive)
        {
            return agent.GetDeltaVelocityX() < speedTolerance
                // ? Mathf.Abs(agent.GetDeltaVelocityX())
                // : -Mathf.Abs(agent.GetDeltaVelocityX());
                ? 1 : -1;
        }

        return 0;    // ?
    }

    /*private State GetState(float acceleration)
    {
        if (acceleration < -accelerationTolerance)
        {
            return (State) 1;
        }
        else if (acceleration > accelerationTolerance)
        {
            return (State) 2;
        }
        else
        {
            return (State) 0;
        }
    }

    private float GetRewardRaw(State state, float lastAcceleration, float acceleration)
    {
        switch (state)
        {
            case State.Balanced:
                return 0f;
            case State.Negative:
                return lastAcceleration - acceleration;
            case  State.Positive:
                return acceleration - lastAcceleration;
        }
        return 0f;
    }*/
    
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
            // if (Random.Range(0f, 100f) < 90)
            {
                // return (Action)i;
            }
        }
        return (Action) a;
    }
    
    string ToString()
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
}
