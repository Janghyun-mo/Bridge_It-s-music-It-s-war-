﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    #region constants
    #endregion

    #region components
    [SerializeField]
    private PlayerController controller;    // 플레이어 컨트롤 관련 클래스
    #endregion

    #region variables
    private Transform objTransform;
    #endregion

    #region get / set
    #endregion

    #region UnityFunc
    void Awake()
    {
        objTransform = GetComponent<Transform>();
        scaleVector = new Vector3(1f, 1f, 1f);
        isRightDirection = true;
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region initialzation
    private void InitController()
    {
        ControllerUI.Instance.SetPlayer(this, ref controller);
    }

    public override void Init()
    {
        base.Init();
        CameraController.Instance.AttachObject(this.transform); // get Camera
        baseColor = Color.white;
        characterState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.PLAYER;
        damageImmune = CharacterInfo.DamageImmune.NONE;
        abnormalImmune = CharacterInfo.AbnormalImmune.NONE;

        //animationHandler.Init(this, PlayerManager.Instance.runtimeAnimator);

        directionVector = new Vector3(1, 0, 0);
        InitController();
        TimeController.Instance.PlayStart();
        //Debug.Log("hpMax : " + hpMax);
    }

    #endregion

    #region func

    //public override CustomObject Interact()
    //{
    //    float bestDistance = interactiveCollider2D.radius;
    //    Collider2D bestCollider = null;

    //    Collider2D[] collider2D = Physics2D.OverlapCircleAll(bodyTransform.position, interactiveCollider2D.radius, (1 << 1) | (1 << 9));

    //    for (int i = 0; i < collider2D.Length; i++)
    //    {
    //        if (!collider2D[i].GetComponent<CustomObject>().GetAvailable())
    //            continue;
    //        float distance = Vector2.Distance(bodyTransform.position, collider2D[i].transform.position);

    //        if (distance < bestDistance)
    //        {
    //            bestDistance = distance;
    //            bestCollider = collider2D[i];
    //        }
    //    }

    //    if (null == bestCollider)
    //        return null;

    //    return bestCollider.GetComponent<CustomObject>();
    //}


    private void Move()
    {
        if (rgbody)
        {
            rgbody.MovePosition(objTransform.position
            + controller.GetMovingInputVector() * (movingSpeed) * Time.fixedDeltaTime);
        }

        directionDegree = controller.GetMovingInputDegree();

        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = 1f;
            spriteTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -1f;
            spriteTransform.localScale = scaleVector;
        }
        //if (controller.GetMoveInputVector().sqrMagnitude > 0.1f)
        //{
        //    animationHandler.Walk();
        //}
        //else
        //{
        //    animationHandler.Idle();
        //}
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.W))
        {
            bodyTransform.Translate(Vector2.up * 5f * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bodyTransform.Translate(Vector2.down * 5f * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            bodyTransform.Translate(Vector2.right * 5f * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            bodyTransform.Translate(Vector2.left * 5f * Time.fixedDeltaTime);
        }
#endif
    }

    #endregion

 
    #region coroutine
    

    #endregion
}
