using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BouncingBall : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public float MaxHorizontalVelocity { get; set; }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        float velocityX = Random.Range(-MaxHorizontalVelocity, MaxHorizontalVelocity);
        float velocityY = _rigidbody.velocity.y;
        _rigidbody.velocity = new Vector3(velocityX, velocityY, 0f);
        // _rigidbody.AddForce(velocityX, 0f, 0f);
    }

}
