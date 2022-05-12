using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPAD : MonoBehaviour
{
    public GameObject kiwiObj;
    public Kiwi kiwiScript;
    public string name;
    // Start is called before the first frame update
    void Start()
    {
        kiwiObj = GameObject.Find("Kiwi");
        kiwiScript = (Kiwi)kiwiObj.GetComponent(typeof(Kiwi));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
