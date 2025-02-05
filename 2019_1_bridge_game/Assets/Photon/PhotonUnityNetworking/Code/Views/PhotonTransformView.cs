﻿// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;


    [AddComponentMenu("Photon Networking/Photon Transform View")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    [RequireComponent(typeof(PhotonView))]
    public class PhotonTransformView : MonoBehaviour, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        private PhotonView m_PhotonView;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;
        private Vector3 currentPos;

        private float lag = 0;
        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;


        public void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = transform.position;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;
        }

        public void Update()
        {
            if (!this.m_PhotonView.IsMine)
            {
                //끊어진 시간이 너무 길 경우(텔레포트)
                if ((transform.position - currentPos).sqrMagnitude >= 5.0f * 5.0f)
                {
                    transform.position = currentPos;
                }
                //끊어진 시간이 짧을 경우(자연스럽게 연결 - 데드레커닝)
                else
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + m_NetworkPosition, Time.deltaTime * 10f);
                }
                //transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_Direction = transform.position - this.m_StoredPosition;
                    this.m_StoredPosition = transform.position;

                    stream.SendNext(transform.position);
                    stream.SendNext(transform.position);
                    stream.SendNext(this.m_Direction);
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(transform.rotation);
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(transform.localScale);
                }
            }
            else
            {
                if (this.m_SynchronizePosition)
                {
                    currentPos = (Vector3)stream.ReceiveNext();
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    Debug.Log(lag);
                    this.m_NetworkPosition += this.m_Direction * lag;

                    this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
                }

                if (this.m_SynchronizeScale)
                {
                    transform.localScale = (Vector3)stream.ReceiveNext();
                }
            }
        }
    }
}