using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAgent : MonoBehaviour
{
    // The ball that the platform balances. 
    [SerializeField] private GameObject ball;
    
    //
    [SerializeField] private GameObject platform;

    // private Rigidbody _rigidbody;
    public QManager Manager { get; set; }

    private Rigidbody _rigidbody;

    // private bool _XZflip;
    // public bool XZflip => _XZflip;
    
    private float _lastAxialVelocity;
    public float LastAxialVelocity => _lastAxialVelocity;

    public Action LastAction { get; set; }

    public State LastState { get; set; }

    private Vector3 acceleration, lastAcceleration;
    private Vector3 distanceMoved;
    private Vector3 lastDistanceMoved;
    private Vector3 last;
    public Vector3 Acceleration => acceleration;
    public Vector3 LastAcceleration
    {
        get => lastAcceleration;
        set => lastAcceleration = value;
    }



    void Start()
    {
        // Manager = GameObject.FindWithTag("GameController").GetComponent<QManager>();
        _rigidbody = ball.GetComponent<Rigidbody>();
        
        if (ball == null
            || platform == null
            || _rigidbody == null)
        {
            Debug.Log(gameObject + " not configured properly. ");
            gameObject.SetActive(false);
        }
        
        lastDistanceMoved = Vector3.zero;
        distanceMoved = Vector3.zero;
        last = ball.transform.position;
        
        // Start with platform in random posture, to give the ball an initial horizontal pos. 
        // ..
    }

    void Update()
    {
                
        if (ball.transform.position.y < -5)
        {
            ResetBall();
        }
    }

    public void UpdateAcceleration()
    {
        distanceMoved = (ball.transform.position - last) * Time.deltaTime ;
        acceleration = distanceMoved - lastDistanceMoved;
        lastDistanceMoved = distanceMoved;
        last = ball.transform.position;
    }
    
    void FixedUpdate()
    {
        // var position = ball.transform.position;
        // var scale = platform.transform.localScale;
        /*float posX = position.x / scale.x, 
            posZ = position.z / scale.z;*/
        /*_XZflip = !_XZflip;
        float pos = _XZflip
            ? position.x / scale.x
            : position.z / scale.z;*/

        // float pos = GetBallPositionX();

        // _lastAction = Manager.SelectAction(pos);
        // RunAction(_lastAction);

        // Debug.Log("" + Manager.GetState(pos) + ", " + _lastAction);

        
        /*
        // Select and perform action
        _XZflip = !_XZflip;
        _lastAxialVelocity = _XZflip ? GetVelocity().x : GetVelocity().z;
        Action action = SelectAction(_lastAxialVelocity);
        // Debug.Log(_lastAction);
        RunAction(_lastAction);
        
        _XZflip = !_XZflip;
        _lastAxialVelocity = _XZflip ? GetVelocity().x : GetVelocity().z;
        _lastAction = action;
        */

        /*
        _XZflip = !_XZflip;
        _lastAxialVelocity = _XZflip ? GetVelocity().x : GetVelocity().z;
        _lastAction = SelectAction(_lastAxialVelocity);
        */

    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.velocity;
    }

    public float GetBallPositionX()
    {        
        var position = ball.transform.position;
        var scale = platform.transform.localScale;
        return position.x / scale.x;
    }

    public void RunAction(Action action)
    {
        // Vector3 axis = _XZflip ? new Vector3(0, 0, 1) : new Vector3(-1, 0, 0);
        Vector3 axis = new Vector3(0, 0, -1);
        switch (action)
        {
            case Action.None:
                break;
            case Action.RotateCounterclockwise:
                platform.transform.Rotate(axis, -Manager.RotationAgility);
                break;
            case Action.RotateClockwise:
                platform.transform.Rotate(axis, Manager.RotationAgility);
                break;
            default:
                Debug.Log(gameObject + " enum default");
                break;
        }
    }

    void ResetBall()
    {
        ball.transform.position = new Vector3(0, 3, 0);
        _rigidbody.velocity = new Vector3();
    }
}
