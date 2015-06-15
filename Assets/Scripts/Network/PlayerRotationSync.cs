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
    /// <summary>
    /// Syncronizes the player's orientation as well as the head orientation over the network
    /// </summary>
    public class PlayerRotationSync : NetworkBehaviour 
	{
        #region MEMBER VARIABLES
        [SyncVar]
        private Quaternion m_SyncRotation;
        [SyncVar]
        private Quaternion m_SyncHeadRotation;

        [SerializeField]
        private Transform m_Transform;

        [SerializeField]
        private Transform m_HeadTransform;

        [SerializeField]
        private Transform m_PlayerCamera;

        private float m_SmoothingFactor = 15f;
        [SerializeField]
        private float m_SmoothingFactorNormal = 15f;
        [SerializeField]
        private float m_SmoothingFactorFast = 25f;

        // minimum rotation angle in degrees the player has to turn before transmitting the new orientation to the server
        [SerializeField]
        private float m_MinTurnTreshold = 5f;

        private Quaternion m_PreviousOrientation;
        private Quaternion m_PreviousHeadOrientation;


        #endregion

        #region UNITY FUNCTIONS

        void Start ()
		{
            if (m_HeadTransform == null)
            {
                m_HeadTransform = transform;
            }

            if (m_Transform == null)
                m_Transform = transform;
        }

        void Update()
        {
            // interpolate head rotations of other players at each frame
            if (isLocalPlayer == false)
            {
                InterpolateRotation();
            }
        }

        void FixedUpdate()
        {
            if (isLocalPlayer == true)
            {
                // transmit rotation to server at fixed interval
                TransmitRotation();
            }
           
        }

        #endregion

        #region METHODS
        private void InterpolateRotation()
        {
            if(m_SyncRotation != null)
                m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_SyncRotation, Time.deltaTime * m_SmoothingFactor);
            if (m_SyncHeadRotation != null)
                m_HeadTransform.rotation = Quaternion.Lerp(m_HeadTransform.rotation, m_SyncHeadRotation, Time.deltaTime * m_SmoothingFactor);
        }
        #endregion

        #region NETWORKING METHODS
        [Command]
        void CmdProvideRotationToServer(Quaternion in_Rotation, Quaternion in_HeadRotation)
        {
            m_SyncRotation = in_Rotation;
            m_SyncHeadRotation = in_HeadRotation;
        }

        [ClientCallback]
        private void TransmitRotation()
        {
            bool hasPlayerTurned = Quaternion.Angle(m_Transform.rotation, m_PreviousOrientation) > m_MinTurnTreshold
                                || Quaternion.Angle(m_PlayerCamera.rotation, m_PreviousHeadOrientation) > m_MinTurnTreshold;
            
            if(hasPlayerTurned == true)
            {
                CmdProvideRotationToServer(m_Transform.rotation, m_PlayerCamera.rotation);

                // store snapshot of orientations
                m_PreviousOrientation = m_Transform.rotation;
                m_PreviousHeadOrientation = m_PlayerCamera.rotation;

                Debug.Log("Player turned");
            }

        }
        #endregion

        #region EVENT HANDLER
        #endregion

        #region PROPERTIES
        #endregion

    }
}