using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum State
{
    Balanced = 0, 
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
    // Maximum degree of rotation per frame, in absolute value. 
    // In current version, directly stands for angles to rotate per frame. 
    [SerializeField] [Range(0, 90)] private float rotationAgility = 10f;
    public float RotationAgility => rotationAgility;
    
    // 
    [SerializeField] [Min(0)] private float accelerationTolerance = 0.5f;

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

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < actionInterval)
        {
            return;
        }
        
        // Debug.Log(ToString());
        
        // Evaluate actions performed, calculate reward and update Q matrix. 
        foreach (var agent in _agents)
        {
            agent.UpdateAcceleration();
            
            float accelerationX = agent.Acceleration.x, 
                lastAccelerationX = agent.LastAcceleration.x;

            float reward = GetRewardRaw(agent.LastState, lastAccelerationX, accelerationX)
                * rewardMultiplier * _timer;
            Q[(int) agent.LastState][(int) agent.LastAction] += reward;
            
            agent.LastState = GetState(accelerationX);
            agent.LastAction = SelectAction(agent.LastState);
            agent.RunAction(agent.LastAction);
            
            Debug.Log(agent.LastState + ", " + agent.LastAction);
            Debug.Log((float) agent.Acceleration.x);

            // float pos = agent.GetBallPositionX();
            // float reward = GetRewardRaw(pos) * rewardMultiplier * Time.deltaTime;
            // Q [(int) GetState(pos)] [(int) agent.LastAction] += reward;

            // float axialVelocity = agent.XZflip 
            //     ? agent.GetVelocity().x 
            //     : agent.GetVelocity().z;
            // float lastAxialVelocity = agent.LastAxialVelocity;
            /*float reward = agent.LastAxialVelocity > axialVelocity 
                ? agent.LastAxialVelocity - axialVelocity 
                : ;*/

            /*float reward;
            if (Math.Abs(Mathf.Sign(lastAxialVelocity) - Mathf.Sign(axialVelocity)) < 0.01)
            {
                reward = Mathf.Abs(lastAxialVelocity - axialVelocity);
            }
            else
            {
                reward = Mathf.Abs(lastAxialVelocity + axialVelocity);
            }
            reward *= rewardMultiplier;*/

            // Q[(int) agent.LastAction][(int) Mathf.Sign(lastAxialVelocity) + 1] += reward;

            /*float axialVelocity = agent.XZflip 
                ? agent.GetVelocity().x 
                : agent.GetVelocity().z;
            float lastAxialVelocity = agent.LastAxialVelocity;
            float reward = GetRewardRaw(lastAxialVelocity, axialVelocity) 
                           * rewardMultiplier * Time.deltaTime;
            Q[(int) agent.LastAction][GetState(axialVelocity)] += reward;

            Debug.Log("" + lastAxialVelocity + ", " + axialVelocity);*/

            // Q[agent.GetState(agent.LastAxialVelocity)][(int) agent.LastAction] += reward;
        }

        _timer = 0f;
    }

    /*public State GetState(float pos)
    {
        if (pos < -AccelerationTolerance)
        {
            return (State) 0;
        }
        else if (pos > AccelerationTolerance)
        {
            return (State) 2;
        }
        else
        {
            return (State) 1;
        }
    }*/

    /*float GetRewardRaw(float pos)
    {
        if (pos < -_accelerationTolerance)
        {
            return pos - _accelerationTolerance;
        }
        else if (pos > AccelerationTolerance)
        {
            return _accelerationTolerance - pos;
        }
        else
        {
            // return _accelerationTolerance - Mathf.Abs(_accelerationTolerance - pos);
            return 0;
        }
    }*/

    private State GetState(float acceleration)
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
    }
    
    public Action SelectAction(State state)
    {
        float bestValue = Q[(int) state][0];
        for (int i = 0; i < Q.Count; i++)
        {
            if (Q[(int) state][i] - bestValue > 0.01)
            {
                bestValue = Q[(int) state][i];
            }
            else 
            if (Random.Range(0f, 100f) < 90)
            {
                return (Action)i;
            }
        }
        return Action.None;
    }
    
    string ToString()
    {
        string s = "";
        for (int i = 0; i < Q.Count; i++)
        {
            for (int j = 0; j < Q[i].Count; j++)
            {
                // s += (State) i + ", " + (Action) j + ", " + Q[i][j] + "  ";
                s += Q[i][j] + "  ";
            }
            // s += '\n';
        }
        return s;
    }
}
