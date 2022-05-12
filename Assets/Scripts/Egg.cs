using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    private float fwd;
    private float x;
    private float time = 0f;

    public void Setup(bool dir){
        this.fwd = dir ? 1f : -1f;
        if( dir ){
            x = 75;
        }
        else {
            x = 170;
        }
        Destroy(gameObject, 0.5f);
    }

    private void Update(){

        // rotate faster at the start, it looks more natural
        if(time < 0.1f){
            x-= fwd * Time.deltaTime * 600;
            transform.position += (new Vector3( fwd * 15f, 2.8f, 0) * Time.deltaTime);
        }
        else if(time < 0.3){
            x-= fwd * Time.deltaTime * 400;
            transform.position += (new Vector3( fwd * 15f, 2f, 0) * Time.deltaTime);
        }
        else {
            x-= fwd * Time.deltaTime * 300;
            transform.position += (new Vector3( fwd * 15f, -1f, 0) * Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(0, 0, x);
        time += Time.deltaTime;
    }

}
