using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    public static GlobalManagers Instance { get; set; }
    [SerializeField] private GameObject parentObj;
    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(parentObj);
        }
    }
}
