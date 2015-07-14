/*
The MIT License (MIT)

Copyright (c) 2015 Andre Groeschel

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Kontraproduktiv
{
    public class PlayerID : NetworkBehaviour
    {

        #region MEMBER VARIABLES
        [SyncVar (hook = "OnPlayerIDChanged")]
        public string m_PlayerIdentity;

        private NetworkInstanceId m_PlayerID;
        private Transform m_Transform;
        #endregion


        #region UNITY FUNCTIONS
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            GetNetworkIdentity();
            SetNetworkIdentity();
        }
        #endregion

        void Awake()
        {
            m_Transform = transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Transform.name == "" || m_Transform.name == "Player(Clone)")
            {
                SetNetworkIdentity();

            }
        }

        [Client]
        private void GetNetworkIdentity()
        {
            m_PlayerID = GetComponent<NetworkIdentity>().netId;
        }

        private void SetNetworkIdentity()
        {
            if(isLocalPlayer == true)
            {
                // create a unique identity for local player
                string uniqueID = m_PlayerIdentity + m_PlayerID.ToString();
                m_Transform.name = uniqueID;
                CmdTransmitIdentityToServer(uniqueID);
            }
            else
            {
                m_Transform.name = m_PlayerIdentity;

            }             
            // after identity is set, disable this component
            this.enabled = false;
        }

        [Command]
        private void CmdTransmitIdentityToServer(string in_ID)
        {
            m_PlayerIdentity = in_ID;
        }
    
        private void OnPlayerIDChanged(string in_PlayerID)
        {
            Debug.Log("Player ID Set to " + in_PlayerID);
            // use id provided by server
            m_PlayerIdentity = in_PlayerID;
            m_Transform.name = m_PlayerIdentity;

        }

    }
}
