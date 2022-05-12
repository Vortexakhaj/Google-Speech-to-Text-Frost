using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoat : MonoBehaviour
{
  bool piwiAlive = true;
  bool isCoolingDown = false;
  private bool angry = false;
  private float COOLDOWN = 0.5f;
  int sprintHash = Animator.StringToHash("angry");
  private Vector3 target;
  AudioSource src;

  public GameObject piwi;
  AnimatorStateInfo state;
  public Animator anim;
  public SpriteRenderer mySpriteRenderer;

  private void Awake()
  {
    mySpriteRenderer = GetComponent<SpriteRenderer>();
    piwi = GameObject.FindGameObjectWithTag("Player");
    src = GetComponent<AudioSource>();
    target = checkMove();
  }

  void Start()
  {
    anim = GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (isCoolingDown)
    {
      return;
    }
    if (!angry)
    {
      CheckForPiwi();
    }

    state = anim.GetCurrentAnimatorStateInfo(0);
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "egg" || (other.tag == "hole" && other.name == "pit1"))
    {
      Destroy(gameObject);
    }
    else if (other.tag == "Player")
    {
      piwiAlive = false;
      anim.SetBool("angry", false);
      Debug.Log("should be not angry in anim");
      angry = false;
    }
  }

  private void CheckForPiwi()
  {
    float piwiX = piwi.transform.position.x;
    float piwiY = piwi.transform.position.y;
    float x = transform.position.x;
    float y = transform.position.y;

    // check horizontal dist
    if (x - piwiX < 18f && x - piwiX > 0 && piwiX > target.x)
    {
      // check kiwi above stoat
      if (piwiY > y)
      {
        float res = Mathf.Abs(piwiY) - Mathf.Abs(y);
        if (Mathf.Abs(Mathf.Abs(piwiY - y)) < 2.6f)
        {
          StartCoroutine(Charge());
        }
      }
      // check kiwi below stoat
      else if (y > piwiY)
      {
        if (Mathf.Abs(y - piwiY) < 1.9f)
        {
          StartCoroutine(Charge());
        }
      }
    }
    //piwiY - transform.position.y < 2.71 | y - (-piwiY) < 2.49
  }

  private IEnumerator Charge()
  {
    anim.SetBool("angry", true);
    angry = true;
    src.Play();

    var start = transform.position;
    var end = target;
    var time = 0f;
    float mul = 7f; // changes speed
    float animMul = 7f;
    anim.SetFloat("speedMul", animMul);
    anim.speed = 1f;

    //Debug.Log("start pos is " + start);
    //Debug.Log("end pos is " + end);

    while (time < 1f)
    {
      transform.position = Vector3.Lerp(start, end, time);
      if (time < 0.02f)
      {
        time += Time.deltaTime / mul;
        mul -= 0.07f;
        animMul = 2f;
      }
      else if (time >= 0.02f && time < 0.1f)
      {
        animMul = 0.5f;
        time += Time.deltaTime / mul;
        mul -= 0.035f;
      }
      else if (time >= 0.1f && time < 0.3f)
      {
        animMul = 0.6f;
        time += Time.deltaTime / mul;
        mul -= 0.035f;
      }
      else if (time >= 0.3f && time < 0.33f)
      {
        animMul = 0.65f;
        time += Time.deltaTime / mul;
        mul -= 0.055f;
      }
      else if (time >= 0.33f && time < 0.7f)
      {
        animMul = 0.75f;
        time += Time.deltaTime / mul;
        mul -= 0.02f;
      }
      else if (time >= 0.7f && time < 0.72f)
      {
        animMul = 0.9f;
        time += Time.deltaTime / mul;
        mul -= 0.005f;
      }
      else if (time >= 0.72f)
      {
        animMul = 1.2f;
        time += Time.deltaTime / mul;
      }
      anim.SetFloat("speedMul", animMul);
      if (!angry)
      {
        yield break;
      }
      yield return null;
    }
    transform.position = end;
    anim.SetBool("angry", false);
    anim.SetBool("coolingOff", true);
    mySpriteRenderer.flipX = true;
    // walk back slowly
    time = 0f;
    mul = 3f;
    while (time < 1f)
    {
      transform.position = Vector3.Lerp(end, start, time);

      if (time > 0.8f)
      {
        time += Time.deltaTime / mul;
      }
      else
      {
        time += Time.deltaTime / (mul / 2f);
      }
      yield return null;
    }
    anim.SetBool("coolingOff", false);
    mySpriteRenderer.flipX = false;
    angry = false;
    isCoolingDown = false;
  }

  // We only need to check for obs once since the stoat is static

  private Vector3 checkMove()
  {

    Vector3 kiwiPos = new Vector3(piwi.transform.position.x, transform.position.y, transform.position.z);
    // need to shoot in front of stoat instead of through it
    Vector3 stoatPos = new Vector3(transform.position.x - 3f, transform.position.y, 0);


    float maxDist = Mathf.Abs(kiwiPos.x - transform.position.x) - 6f;

    RaycastHit2D hit = Physics2D.Raycast(stoatPos, Vector3.left, maxDist);

    if (hit.collider != null)
    {
      Debug.DrawLine(stoatPos, kiwiPos, Color.blue, 5f);
      Debug.Log("We hit " + hit.collider.name);
      if (hit.collider.tag == "obs")
      {
        float x = hit.point.x + 3f;
        //Debug.Log("hitpoint is " + x + ", new target location is " + new Vector3 (kiwiPos.x + x, stoatPos.y, stoatPos.z));
        return new Vector3(x, stoatPos.y, stoatPos.z);
        // returns earlier than the full distance
      }
    }
    return kiwiPos;
  }

}
