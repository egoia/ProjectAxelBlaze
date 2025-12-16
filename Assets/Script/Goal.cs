using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action OnScore;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Ball")OnScore?.Invoke();
    }
}
