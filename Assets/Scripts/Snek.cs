using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snek : MonoBehaviour
{
    bool isCoolingDown = false;
    private float COOLDOWN = 0.4f;

    AnimatorStateInfo state;
    public Animator anim;
    public SpriteRenderer mySpriteRenderer;
    public bool horiz;
    public bool pos;

    
    private void Awake(){
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isCoolingDown){
            return;
        }
        Move();
        state = anim.GetCurrentAnimatorStateInfo(0);
    }



    private void Move(){
        // move in current direction unless we hit a block
        Vector3 nextMove = getNextMove();
        if( checkMove(nextMove) ){
            StartCoroutine(SneakDiss(nextMove));
        }
        else { 
            // We couldn't move, so we're going to turn around
            pos = !pos;
            changeDirection();
        }
    }


    private IEnumerator SneakDiss(Vector3 travel){
        //Debug.Log("travel is " + travel);
        isCoolingDown = true;
        Vector3 start = transform.position;
        Vector3 end = start + travel;
        var time = 0f;
        while(time < 1f){
            transform.position = Vector3.Lerp(start, end, time);
            time = time + Time.deltaTime / (COOLDOWN+0.25f);
            yield return null;
        }

        transform.position = end;
        isCoolingDown = false;
    }

    // move in 1-tile increments until "obs" hit, then turn around

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "egg"){
            Destroy(gameObject);
        }
    }


    private Vector3 getNextMove(){
        // Determine travel based on direction
        Vector3 travel;

        // Determine direction based on bool
        float fwd = pos ? 1f : -1f;

        if( horiz ){ // MOVING ALONG X-AXIS 
            travel = new Vector3(  fwd * 3.6f, 0, 0);
        }
        else { // MOVING ALONG Y-AXIS
            travel = new Vector3( 0, fwd * 2.6f, 0);
        }
        return travel;
    }
    // -11.5


    // Helper FNs
    private bool checkMove(Vector3 A){
        // (A, B) fire raycast
        float x = 0;
        float y = 0;
        float rad = 2f;
        if(!horiz){
            y = pos ? rad : -rad;
        }
        else {
            x = pos ? rad : -rad;
        }
        Vector3 C = new Vector3(transform.position.x + A.x, transform.position.y + A.y, transform.position.z + A.z);
        Vector3 B = new Vector3(C.x + x, C.y + A.y + y, C.z);
        RaycastHit2D hit = Physics2D.Raycast(C, C-B, 2f);
        Debug.DrawLine(C, B, Color.red);
        //Debug.Log("We hit " + hit.collider.name + " and tag " + hit.collider.tag);
        if( hit.collider != null ){
            if( hit.collider.tag == "nest" | hit.collider.tag == "obs" | hit.collider.tag == "hole" | hit.collider.tag == "stoat"){
                //Debug.Log(hit.collider.tag);
                return false;
            }
        }
        return true;
    }


    private void changeDirection(){
        float oldX = transform.localScale.x;
        float oldY = transform.localScale.y;
        transform.localScale = new Vector3(oldX, -1f * oldY, 0);
        if(gameObject.name == "snek (1)"){
            //Debug.Log("position is " + transform.position);
        }
    }
}
