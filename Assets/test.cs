using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class test : MonoBehaviour {
    public GameObject r;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool still_animated;
    private SpriteRenderer sprite_final_explosion;
    private SpriteRenderer sprite_aura;
    public float score;
    public float best_score;
    public int team_number;
    public Animator rocket1;
    public Animator rocket2;
    public Animator final_explosion;
    public Animator aura;
    public Canvas c;
    public Camera ca;
    // Use this for initialization
    void Start ()
    {
        rb = r.GetComponent<Rigidbody2D>();
        best_score = DataModel.BestScore();
        score = DataModel.Scores[team_number - 1];
        if (team_number < DataModel.NumberOfTeams)
        {
            GetComponent<Image>().GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(team_number - 1);
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 0;
        }
        velocity = new Vector2(0f, 30f);
        rocket1.SetBool("isMoving", true);
        rocket2.SetBool("isMoving", true);
        still_animated = true;
        sprite_final_explosion = final_explosion.gameObject.GetComponent<SpriteRenderer>();
        sprite_final_explosion.enabled = false;
        if (score != best_score)
        {
            Destroy(aura.gameObject);
        }
        else
        {
            sprite_aura = aura.gameObject.GetComponent<SpriteRenderer>();
            sprite_aura.enabled = false;
        }
    }

    private IEnumerator KillOnAnimationEnd()
    {
        still_animated = false;
        yield return new WaitForSeconds(0.417f);

        Destroy(rocket1.gameObject);
        Destroy(rocket2.gameObject);
    }

    private IEnumerator LaunchFinalExplosion()
    {
     
        yield return new WaitForSeconds(1f);
        sprite_final_explosion.enabled = true;
        final_explosion.SetBool("isBestTeam", true);
        yield return new WaitForSeconds(0.5f);
        Destroy(final_explosion.gameObject);
        sprite_aura.enabled = true;
    }


    void FixedUpdate()
    {
        Vector3 screenPos = ca.WorldToScreenPoint(rb.position);
        if (rb.position.y < (score/best_score)*220)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        else if(still_animated)
        {
            rocket1.SetBool("isMoving", false);
            rocket2.SetBool("isMoving", false);
            StartCoroutine (KillOnAnimationEnd());

            if (score.Equals(best_score))
            {
                StartCoroutine(LaunchFinalExplosion());
            }
        }


    }
}
