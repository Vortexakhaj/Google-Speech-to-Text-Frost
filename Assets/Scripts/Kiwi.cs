using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiwi : MonoBehaviour
{
  [SerializeField] private Transform Egg;
  [SerializeField] private Transform Gust;

  //SceneManager mgr;

  private Rigidbody2D rb;
  public SpriteRenderer mySpriteRenderer;
  public GameObject kiwiSprite;
  public Animator anim;
  AnimatorStateInfo state;

  AudioSource src;


  int walkHash = Animator.StringToHash("Walk");
  int eggHash = Animator.StringToHash("Egg");
  int jumpHash = Animator.StringToHash("Jump");
  int deathHash = Animator.StringToHash("Dead");
  int blowHash = Animator.StringToHash("Blow");
  int stillStateHash = Animator.StringToHash("Base Layer.Still");


  private float slowFactor = 2.5f;
  private float yDist = 2.7f;
  private float xDist = 3.55f;
  private static float GLOBAL_TIME;
  private static readonly float COOLDOWN = 0.5f;
  private static readonly float ACTION_COOLDOWN = 1f;
  private float invisFade = 0f;
  private bool isCoolingDown = false;
  private bool facingForward = true;
  private bool slowed = false;
  private bool grounded;
  public bool canMove;
  private bool invis;
  private bool canHide = false;

  private void Awake()
  {
    mySpriteRenderer = GetComponent<SpriteRenderer>();
    rb = gameObject.GetComponent<Rigidbody2D>();
    src = GetComponent<AudioSource>();
  }

  private void Start()
  {
    //mgr = new SceneManager();
    grounded = true;
    canMove = true;
    anim = GetComponent<Animator>();
    GLOBAL_TIME = 0f;
  }

  // Update is called once per frame
  void Update()
  {
    state = anim.GetCurrentAnimatorStateInfo(0);

    if (!canMove)
    {
      return;
    }


    //GLOBAL_TIME += Time.deltaTime;
    // if ( GLOBAL_TIME >= ACTION_COOLDOWN ){
    //     //return;
    // }

    if (isCoolingDown || state.nameHash != stillStateHash)
    {
      //Debug.Log(state.nameHash);
      return;
    }

    if (Input.GetKeyUp("x"))
    {
      CallEgg();
      return;
    }


    if (Input.GetKeyUp("b"))
    {
      CallBlow();
      return;
    }

    if (Input.GetKeyUp("j"))
    {
      CallJump();
      return;
    }

    var horiz = Input.GetAxis("Horizontal");
    var vert = Input.GetAxis("Vertical");

    if (Mathf.Abs(vert) > 0)
    {
      bool up = (Mathf.Sign(vert) == 1) ? true : false;
      Vector3 target = new Vector3(0, Mathf.Sign(vert) * yDist, 0);
      if (checkMove(target, true, up))
      {
        StartCoroutine(Move(target));
      }
    }
    else if (Mathf.Abs(horiz) > 0)
    {
      if (mySpriteRenderer != null)
      {
        Vector3 target = new Vector3(Mathf.Sign(horiz) * xDist, 0, 0);

        if (horiz < 0)
        {
          if (facingForward)
          {
            changeDirection();
          }
          else
          {
            if (checkMove(target, false, false))
            {
              StartCoroutine(Move(target));
            }
          }
        }
        else
        {
          if (!facingForward)
          {
            changeDirection();
          }
          if (checkMove(target, false, true))
          {
            StartCoroutine(Move(target));
          }
        }
      }
    }
  }

  public void SendCommand(string com){
    com = com.ToLower();
    if( canMove ){
      switch( com ){
        case "egg":
          CallEgg();
          break;
        case "jump":
          CallJump();
          break;
        case "hide":
          CallHide();
          break;
        case "blow":
          CallBlow();
          break;
        default:
          break;
      }
    }
  }

  public void CallHide(){
    canHide = true;
  }

  public void CallEgg()
  {
    anim.SetTrigger(eggHash);
    return;
  }

  public void CallBlow()
  {
    anim.SetTrigger(blowHash);
    return;
  }

  public void CallJump()
  {
    // if (!grounded)
    // {
    //   return;
    // }
    Vector3 jumpTarget = new Vector3(xDist, 0, 0);
    Vector3 dir = facingForward ? Vector3.right : Vector3.left;
    Vector3 sum = jumpTarget + kiwiSprite.transform.position;


    if (checkMove(dir) && (!facingForward && transform.position.x > 14f))
    {
      StartCoroutine(Jump());
    }
  }

  public void VerticalMove(bool up)
  {
    if( canMove ){
      float pos = up ? 1f : -1f;
      Vector3 target = new Vector3(0, pos * yDist, 0);
      if (checkMove(target, true, up))
      {
        StartCoroutine(Move(target));
      }
    }
  }

  public void HorizontalMove(bool right)
  {
    if( canMove ){
      float pos = right ? 1f : -1f;
      Vector3 target = new Vector3(pos * xDist, 0, 0);
      if (!right)
      {
        if (facingForward)
        {
          changeDirection();
        }
        else
        {
          if (checkMove(target, false, right))
          {
            StartCoroutine(Move(target));
          }
        }
      }
      else
      {
        if (!facingForward)
        {
          changeDirection();
        }
        if (checkMove(target, false, right))
        {
          StartCoroutine(Move(target));
        }
      }
    }
  }

  private void EggToss()
  {
    canMove = false;
    // calculate kiwi versus egg position
    Vector3 kiwiPos = kiwiSprite.transform.position;
    Transform eggTransform;
    float dir = facingForward ? 1f : -1f;
    Vector3 beakPos = kiwiPos + new Vector3(dir * 2.53f, 0.5f, 0);
    if (facingForward)
    {
      eggTransform = Instantiate(Egg, beakPos, Quaternion.Euler(0, 0, 75));
    }
    else
    {
      eggTransform = Instantiate(Egg, beakPos, Quaternion.Euler(0, 0, 190));
    }
    eggTransform.GetComponent<Egg>().Setup(transform.localScale.x > 0);
    canMove = true;
  }

  private void GustBlow()
  {
    canMove = false;
    Vector3 kiwiPos = kiwiSprite.transform.position;
    Transform gustTransform;
    float dir = facingForward ? 1f : -1f;
    Vector3 beakPos = kiwiPos + new Vector3(dir * 2.75f, -0.2f, 0);
    if (facingForward)
    {
      gustTransform = Instantiate(Gust, beakPos, Quaternion.Euler(0, 0, 0));
    }
    else
    {
      gustTransform = Instantiate(Gust, beakPos, Quaternion.Euler(0, 0, 0));
    }
    gustTransform.GetComponent<BlowFx>().Setup(facingForward);
    canMove = true;
  }


  public IEnumerator Jump()
  {

    canMove = false;
    isCoolingDown = true;
    grounded = false;
    float jH = 0;
    anim.SetTrigger(jumpHash);

    Vector3 v = new Vector3(xDist * 2, 0, 0);
    Vector3 h = new Vector3(xDist, jH, 0);
    Vector3 down = new Vector3(xDist, -jH, 0);
    if (!facingForward)
    {
      v = new Vector3(-xDist, 0, 0);
      h = new Vector3(-xDist, jH, 0);
      down = new Vector3(-xDist, -jH, 0);
    }


    // First determine the halfway point
    // Note: We're always jumping forward
    // lerp to halfway, lerp to end

    var current = kiwiSprite.transform.position;
    var start = current;
    var halfpoint = current + h;
    var time = 0f;

    // Adding a slight delay before movement, since the kiwi crouches before "flying"
    // The jump animation is 13 frames, the first/last 4 are spent standing still crouching

    // We want time to pass faster while crouching, slowing while jumping
    //anim.speed = 25f; // Crouch fast
    // We want to play
    anim.speed = 10f;
    while (time < 0.05f)
    {
      time = time + Time.deltaTime;
      yield return null;
    }

    time = 0f;

    anim.speed = 0.15f; // Fly slower


    while (time < 1f)
    {
      transform.position = Vector3.Lerp(current, halfpoint, time);
      time = time + Time.deltaTime / COOLDOWN;

      // change anim speed to be slower at the top and ending half
      if (time >= 0.3f && time < 0.33f)
      {
        anim.speed = 0.1f;
      }
      if (time >= 0.7f && time < 0.72f)
      {
        anim.speed = 0.1f;
      }


      yield return null;
    }

    //current = kiwiSprite.transform.position;
    current = halfpoint;
    time = 0f;
    Vector3 end = halfpoint + down;

    anim.speed = 0.3f; // Land faster

    while (time < 1f)
    {
      transform.position = Vector3.Lerp(current, end, time);
      time = time + Time.deltaTime / COOLDOWN;
      if (time >= 0.5f && time <= 0.55f)
      {
        anim.speed = 1.5f;
      }
      yield return null;
    }
    transform.position = end; // Ensures consistent movement
    anim.speed = 1f; // Reset to normal
    grounded = true;
    canMove = true;
    isCoolingDown = false;
  }

  private IEnumerator Move(Vector3 v)
  {
    canMove = false;
    isCoolingDown = true;
    anim.SetTrigger(walkHash);
    var start = kiwiSprite.transform.position;
    var end = start + v;
    var time = 0f;
    anim.speed = 2.5f;

    while (time < 1f)
    {
      transform.position = Vector3.Lerp(start, end, time);
      if (time >= 0.05f && time < 0.12f && !slowed)
      {
        anim.speed = 1f;
      }
      if (slowed && time < 0.05f)
      {
        anim.speed = 2.5f;
        time = time + Time.deltaTime / (COOLDOWN * slowFactor);
      }
      else if (slowed && time >= 0.05f)
      {
        time = time + Time.deltaTime / (COOLDOWN * slowFactor);
        anim.speed = 0.333333333f;
      }
      else
      {
        time = time + Time.deltaTime / (COOLDOWN);
      }
      yield return null;
    }

    anim.speed = 1f;
    transform.position = end;
    canMove = true;
    isCoolingDown = false;
  }


  void OnTriggerEnter2D(Collider2D other)
  {
    if ((other.tag == "enemy" && !invis) | (grounded && other.tag == "hole"))
    {
      canMove = false;
      anim.SetTrigger(deathHash);
    }
  }

  void OnTriggerStay2D(Collider2D other)
  {
    if ((other.tag == "enemy" && !invis) | other.tag == "stoat" | (grounded && other.tag == "hole"))
    {
      canMove = false;
      anim.SetTrigger(deathHash);
    }
    else if (other.tag == "camo" && canHide)
    {
      float time = Time.deltaTime / 2f;
      invisFade += time;
      if (invisFade <= 0.5f)
      {
        lowerAlpha(time, mySpriteRenderer);
      }
      if (invisFade >= 0.5f)
      {
        invis = true;
      }
    }
    else if (other.tag == "sand")
    {
      slowed = true;
    }

  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (other.tag == "camo")
    {
      restoreAlpha();
      invis = false;
      canHide = false;
    }
    else if (other.tag == "sand")
    {
      slowed = !slowed;
    }
  }


  // This function doesn't work
  // For some reason, you can't store an object hit by raycast in a variable
  // To get around this problem, need to add a script to sand and trigger some
  // variable to fade it away
  private IEnumerator RemoveSand(GameObject obj)
  {
    float time = 0f;
    SpriteRenderer sandRenderer = obj.GetComponent<SpriteRenderer>();
    Color tmp = sandRenderer.color;
    while (time < 0.5f)
    {
      lowerAlpha(time, sandRenderer);
      time += Time.deltaTime;
      yield return null;
    }
    Destroy(obj);
  }

  private IEnumerator StartDeath()
  {
    float time = 0f;
    anim.speed = 8f;
    Color tmp = mySpriteRenderer.color;
    while (time < 0.12f)
    {
      if (time < 0.03f)
      {
        mySpriteRenderer.color = Color.clear;
      }
      else if (time >= 0.03f && time < 0.06f)
      {
        anim.speed = 4f;
        mySpriteRenderer.color = tmp;
      }
      else if (time >= 0.06f && time < 0.09f)
      {
        anim.speed = 1f;
        mySpriteRenderer.color = Color.clear;
      }
      else if (time >= 0.09f && time < 0.12f)
      {
        mySpriteRenderer.color = tmp;
      }

      time += Time.deltaTime / 2;
      yield return null;
    }
  }

  private IEnumerator RestInPieces()
  {
    Destroy(gameObject);
    Application.LoadLevel(Application.loadedLevel);
    yield return null;
  }








  // CHECKING FUNCTIONS

  /*
      Vector A is relative to current position, not some absolute location
   */
  private bool checkMove(Vector3 A, bool vert, bool pos)
  {
    // (A, B) fire raycast
    float x = 0;
    float y = 0;
    if (vert)
    {
      y = pos ? 5f : -5f;
    }
    else
    {
      x = pos ? 5f : -5f;
    }

    Vector3 C = new Vector3(transform.position.x + A.x, transform.position.y + A.y, transform.position.z + A.z);
    Vector3 B = new Vector3(C.x + x, C.y + A.y + y, C.z);
    RaycastHit2D hit = Physics2D.Raycast(C, C - B, 5f, 999, -3f, 1f);
    if (hit != null && hit.collider != null)
    {
      if (hit.collider.tag == "obs"){
        src.Play();
        return false;
      }
    }
    return true;
  }

  private bool checkMove(Vector3 dir)
  {
    float d = xDist;
    float offset = facingForward ? 4f : -3.5f;
    if (!facingForward) d *= 1.2f;

    Vector3 C = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
    Vector3 ligma = new Vector3(C.x + d, C.y, C.z);
    RaycastHit2D hit = Physics2D.Raycast(C, dir, d, 999, -2.5f, -1.5f);
    if (hit != null && hit.collider != null)
    {
      if (hit.collider.tag == "obs"){
        src.Play();
        return false;
      }
    }
    return true;
  }

  private void CheckSand()
  {
    Vector3 A = transform.position;
    float x = 0;
    float y = 0;
    Vector3 dir;
    if (facingForward)
    {
      dir = Vector3.right;
      dir += new Vector3(2f, 0, 0);
    }
    else
    {
      dir = Vector3.left;
      dir -= new Vector3(2f, 0, 0);
    }

    x = facingForward ? 2f : -2f;

    Vector3 C = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
    //Vector3 B = new Vector3(C.x + x, C.y + A.y + y, C.z);
    RaycastHit2D hit = Physics2D.Raycast(C, dir, 1f, 999, -2f);
    Debug.DrawLine(C, dir, Color.blue, 1f);
    if (hit != null)
    {
      Debug.Log(hit.collider.tag);
      if (hit.collider.tag == "sand")
      {
        Destroy(hit.transform.gameObject);
      }
    }
    return;
  }












  // UTILITY FUNCTIONS

  private void lowerAlpha(float time, SpriteRenderer s)
  {
    Color tmp = s.color;
    tmp.a -= time;
    s.color = tmp;
  }

  private void restoreAlpha()
  {
    Color tmp = mySpriteRenderer.color;
    tmp.a = 1f;
    mySpriteRenderer.color = tmp;
    invisFade = 0f;
  }

  private void changeDirection()
  {
    float oldX = transform.localScale.x;
    float oldY = transform.localScale.y;
    transform.localScale = new Vector3(-1f * oldX, oldY, 0);
    facingForward = !facingForward;
  }



  // Animation Helpers

  private void StartBlow()
  {

    float t = 0f;
    anim.speed = 0.7f;
    while (t < 0.3f)
    {
      anim.speed -= 0.005f;
      t += Time.deltaTime * 2f;
    }

  }

  private void BreatheIn()
  {
    float t = 0f;
    while (t < 0.3f)
    {
      anim.speed -= 0.012f;
      t += Time.deltaTime * 1.5f;
    }
  }

  private void BreatheOut()
  {
    CheckSand();
    float t = 0f;
    anim.speed = 0.4f;
    while (t < 0.4f)
    {
      anim.speed += 0.008f;
      t += Time.deltaTime * 2f;
    }
  }

  private void EndBlow()
  {
    anim.speed = 1f;
  }
}