using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAgent : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    
    [SerializeField] private GameObject platform;

    private float _lastDistanceX;
    
    public Action LastAction { get; set; }

    public State LastState { get; set; }

    private float _initialPositionY;
    
    public QManager Manager { get; set; }

    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = ball.GetComponent<Rigidbody>();
        
        if (ball == null)
        {
            ball = transform.GetChild(0).gameObject;
        }

        if (platform == null)
        {
            platform = transform.GetChild(1).gameObject;
        }

        _initialPositionY = ball.transform.position.y;

        BouncingBall b = ball.GetComponent<BouncingBall>();
        b.MaxHorizontalVelocity = Manager.MaxHorizontalVelocity;
    }

    void Update()
    { 
        if (ball.transform.position.y < transform.position.y - 1)
        {
            ResetBall();
        }
        
    }

    public float GetDeltaDistanceX()
    {
        return GetDistanceX() - _lastDistanceX;
    }

    public float GetDistanceX()
    {
        return ball.transform.position.x - platform.transform.position.x;
    }

    public void UpdateAgent()
    {
        _lastDistanceX = GetDistanceX();
    }

    public void RunAction(Action action)
    {
        Vector3 axis = new Vector3(0, 0, -1);
        switch (action)
        {
            case Action.None:
                break;
            case Action.MoveLeft:
                platform.transform.position += Vector3.left * (Manager.MovingAgility * Time.deltaTime);
                break;
            case Action.MoveRight:
                platform.transform.position += Vector3.right * (Manager.MovingAgility * Time.deltaTime);
                break;
            default:
                Debug.Log(gameObject + " enum default");
                break;
        }
    }
    
    public void ResetBall()
    {
        ball.transform.position = new Vector3(
            platform.transform.position.x, 
            transform.position.y + _initialPositionY, 
            transform.position.z);
        _rigidbody.velocity = new Vector3();
    }
}
