using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    public GameObject r;
    private Rigidbody2D rb;
    private Vector2 velocity;
    // Use this for initialization
    void Start () {
        rb = r.GetComponent<Rigidbody2D>();
        velocity = new Vector2(0f, 30f);
    }
	
	// Update is called once per frame
	void Update () {
    }

    void FixedUpdate()
    {
        if (rb.position.y < 0.6 * Screen.height)
        {

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

    }
}
