﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterScript : MonoBehaviour {

    private Animator animator;
    private float MinimumHitPeriod = 1f;
    private float HitCounter = 0;
    public float CurrentHP = 100;

    public float MoveSpeed;
    public GameObject FollowTarget;
    private Rigidbody rigidBody;
    public ColiisionListScript PlayerSensor;
    public ColiisionListScript AttackSensor;
    //受傷的效果
    public GameObject PlayerTargetHittedEffect;

    private bool playerHittedState = false;
    private float playerHittedTime = 0;

    public void AttackPlayer() {
        if(AttackSensor.CollisionObjects.Count > 0) {
            AttackSensor.CollisionObjects[0].transform.GetChild(0).GetChild(0).SendMessage("Hit", 10);
            playerHittedState = true;
        }
    }

	// Use this for initialization
	void Start () {
        PlayerTargetHittedEffect = GameObject.Find("PlayerRoot").transform.GetChild(2).transform.gameObject;
        animator = this.GetComponent<Animator>();
        rigidBody = this.GetComponent<Rigidbody>();
    }
	
    public void Hit(float value) {
        if(HitCounter <= 0) {
            FollowTarget = GameObject.FindGameObjectWithTag("Player");
            HitCounter = MinimumHitPeriod;
            CurrentHP -= value;

            animator.SetFloat("HP", CurrentHP);
            animator.SetTrigger("Hit");

            if (CurrentHP <= 0)
                BuryTheBody();
        }
    }

    void BuryTheBody() {
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Collider>().enabled = false;
        this.transform.DOMoveY(-0.8f, 1f).SetRelative(true).SetDelay(1).OnComplete( () => {
            this.transform.DOMoveY(-0.8f, 1f).SetRelative(true).SetDelay(3).OnComplete( () => {
                GameObject.Destroy(this.gameObject);
            });
        });
    }

    // Update is called once per frame
    void Update () {
        //Player被遠程攻擊
        if (playerHittedState) {
            playerHittedTime += Time.deltaTime;
            PlayerTargetHittedEffect.SetActive(true);
            if (playerHittedTime > 1) {
                PlayerTargetHittedEffect.SetActive(false);
                playerHittedState = false;
                playerHittedTime = 0;
            }
        }


        if (PlayerSensor.CollisionObjects.Count > 0) {
            FollowTarget = PlayerSensor.CollisionObjects[0].gameObject;
        }

		if(CurrentHP > 0 && HitCounter > 0) {
            HitCounter -= Time.deltaTime;
        } else {
            if(CurrentHP > 0) {
                if(FollowTarget != null) {
                    Vector3 lookAt = FollowTarget.gameObject.transform.position;
                    lookAt.y = this.gameObject.transform.position.y;
                    this.transform.LookAt(lookAt);
                    animator.SetBool("Run", true);

                    if(AttackSensor.CollisionObjects.Count > 0) {
                        animator.SetBool("Attack", true);
                        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    } else {
                        animator.SetBool("Attack", false);
                        rigidBody.velocity = this.transform.forward * MoveSpeed;
                    }
                }
            }else {
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
	}
}
