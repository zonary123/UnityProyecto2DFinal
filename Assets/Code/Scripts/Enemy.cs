using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{
    [SerializeField]private bool isTouchingWall;
    [SerializeField]private bool isFloor;
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private Animator anim;
    [SerializeField]private SpriteRenderer sr;
    [SerializeField]private Collider2D coll;
    [SerializeField]private Vector2 direction = Vector2.right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>(); }

    // Update is called once per frame
    void Update()
    {
        if (CanWalk()){
            gameObject.transform.Translate(direction * Time.deltaTime);
        }
    }

    private bool CanWalk(){
        return false;
    }
}
