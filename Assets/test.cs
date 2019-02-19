using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    public GameObject r;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool still_animated;
    public float score;
    public float best_score;
    public int team_number;
    public Animator rocket1;
    public Animator rocket2;
    // Use this for initialization
    void Start () {
        rb = r.GetComponent<Rigidbody2D>();
        best_score = DataModel.BestScore();
        score = DataModel.Scores[team_number - 1];
        velocity = new Vector2(0f, 30f);
        rocket1.SetBool("isMoving", true);
        rocket2.SetBool("isMoving", true);
        still_animated = true;
    }

    private IEnumerator KillOnAnimationEnd()
    {
        still_animated = false;
        yield return new WaitForSeconds(0.417f);

        Destroy(rocket1.gameObject);
        Destroy(rocket2.gameObject);
    }


    void FixedUpdate()
    {
      
        if (rb.position.y < (score/best_score)*0.90f* Screen.height)
        {

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        else if(still_animated)
        {
            rocket1.SetBool("isMoving", false);
            rocket2.SetBool("isMoving", false);
            StartCoroutine (KillOnAnimationEnd());
        }


    }
}
