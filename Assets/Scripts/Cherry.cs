using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : MonoBehaviour
{
  private float time = 0f;
  AudioSource src;
  // Start is called before the first frame update
  void Start()
  {
    src = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    if (time % 1 >= 0.5)
    {
      // increase
      transform.localScale += new Vector3(0.00035f, 0.00035f, 0);
    }
    else
    {
      // decrease
      transform.localScale -= new Vector3(0.00035f, 0.00035f, 0);
    }


    time += Time.deltaTime;
  }

  IEnumerator Wait()
  {
    src.Play();
    yield return new WaitForSecondsRealtime(0.3f);
    Application.LoadLevel(Application.loadedLevel + 1);
    yield return null;
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
    {
      StartCoroutine(Wait());
    }
  }
}
