using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowFx : MonoBehaviour
{
    //int fxHash = Animator.StringToHash("")

    public void Setup(bool fwd){
        if( !fwd ){
            transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);
        }
        Debug.Log(transform.position);
    }

    private void Destroy(){
        Destroy(gameObject);
    }
}
