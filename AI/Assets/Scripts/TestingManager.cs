using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestingManager : MonoBehaviour
{
    [SerializeField] [Min(0)] private float experimentTime = 30;
    [SerializeField] private float samplingFrequency = 1;
    
    private float _timer;
    private int _index = 1;

    private QManager _QManager;

    private string log;

    private string _oPath = "Assets/";
    
    void Start()
    {
        _QManager = GetComponent<QManager>();
        
        /*if (Directory.Exists("Assets/"))
        {
            Debug.Log(Directory.GetFiles("Assets/")[0]);
        }*/
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= samplingFrequency * _index)
        {
            Sample();
            _index++;
        }
        
        if (_timer >= experimentTime)
        {
            Finalize();
        }
    }

    private void Sample()
    {
        // Debug.Log("sampled, time = " + _timer);
        log += "Time: " + _timer + '\n'
                      + _QManager.GetQMatrixToString() + '\n';
    }

    void Finalize()
    {
        Debug.Log(log);
        /*_oPath += "out/";
        if (!Directory.Exists(_oPath))
        {
            Directory.CreateDirectory(_oPath);
        }
        _oPath += System.DateTime.Now + ".txt";
        File.Create(_oPath);
        File.WriteAllText(_oPath, log);*/
        _QManager.enabled = false;
        enabled = false;
    }
}
