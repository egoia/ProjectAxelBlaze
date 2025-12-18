using System;
using UnityEngine;

public class TouchCounter : MonoBehaviour
{
    public event Action OnTouch;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="AIAgent")OnTouch?.Invoke();
    }
}
