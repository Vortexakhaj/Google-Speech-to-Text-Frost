using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickHandler : MonoBehaviour
{
    public UnityEvent upEvent;
    public UnityEvent downEvent;

    public void OnPointerDown(){
        Debug.Log("OH MY GAOUD BAEB");
        downEvent?.Invoke();
    }

    public void OnPointerUp(){
        Debug.Log("reezsuis cjroist");
        upEvent?.Invoke();
    }

}
