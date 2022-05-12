using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
  private float rand = 0f;
  private float time = 0f;
  private float xRot = 0f;
  private float yRot = 0f;
  private float zRot = 0f;
  private Rigidbody2D rb;
  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    rb.AddForce(new Vector2(550f, 0));
    StartCoroutine(Move());
  }

  // Update is called once per frame
  void Update()
  {
    // if(transform.position.x < 1.4){
    //     Move();
    // }

    if (transform.position.x < 0)
    {
      Pulse();
    }


  }

  private IEnumerator Move()
  {
    int count = 0;
    while (transform.position.x < -1f)
    {
      if (count % 40 == 1)
      {
        rb.AddForce(new Vector2(375f, 0));
      }
      count++;
      yield return null;
    }
  }


  // creates wobbling effect in the water
  private void Pulse()
  {
    rand = 1f + (Random.Range(0, 0.02f) - 0.01f);


    if (time % 3 >= 1.4f)
    {
      xRot += 0.015f;
      yRot += 0.08f;
      zRot += 0.01f;
      transform.localScale += new Vector3(0.0008f, 0.0008f, 0);
    }
    else
    {
      xRot -= 0.015f;
      yRot -= 0.08f;
      zRot -= 0.01f;
      transform.localScale -= new Vector3(0.0008f, 0.0008f, 0);
    }

    if ((time / 3f) % 1 < 0.33333333f)
    {
      xRot *= rand;

    }
    else if (((time / 3f) % 1) >= 0.33333333f && ((time / 3f) % 1) < 0.66666666f)
    {
      yRot *= rand;
    }
    else
    {
      zRot *= rand;
    }
    transform.rotation = Quaternion.Euler(xRot, yRot, zRot);
    time += Time.deltaTime;
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
    {
      Application.LoadLevel(1);
    }
  }
}
