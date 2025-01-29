using System;
using System.Collections;
using System.Collections.Generic;
using Cainos.PixelArtPlatformer_VillageProps;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Codigo para hacer diferentes tipos plataformas para los niveles del juego
 */
public class Platform : MonoBehaviour
{
    private enum PlatformType{
        Moving,
        Breakable,
        Falling
    }

    [SerializeField] private bool active;
    [SerializeField]private Animator Animator;
    [SerializeField]private PlatformType platformType;
    [SerializeField]private float timeToRespawn;
    [SerializeField]private bool isBroken;
    [SerializeField]private Vector3 respawnPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        respawnPosition = transform.position;   
        Animator = GetComponent<Animator>();
     
    }

    // Update is called once per frame
    void Update()
    {
        if (isBroken){
            timeToRespawn -= Time.deltaTime;
            if (timeToRespawn <= 0){
                isBroken = false;
                transform.position = respawnPosition;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        if (active) return;
        if (other.gameObject.CompareTag("Player")){
            active = true;
            Debug.Log("Player collision");
            if (Animator != null){
                Animator.SetBool(platformType.DisplayName(), true);
            }
            switch (platformType){
                case PlatformType.Breakable:
                    StartCoroutine(Breakable());
                    break;
                case PlatformType.Falling:
                    StartCoroutine(Falling());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    IEnumerator Breakable(){
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        isBroken = true;
        timeToRespawn = 2;
    }

    IEnumerator  Falling(){
        yield return new WaitForSeconds(1);
        gameObject.transform.position += new Vector3(111, -1, 0);
        isBroken = true;
        timeToRespawn = 2;
    }
}
