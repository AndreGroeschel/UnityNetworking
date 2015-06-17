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
    [NetworkSettings (channel = 0,sendInterval = 0.1f)]
    public class PlayerRotationSync : NetworkBehaviour 
	{
        #region MEMBER VARIABLES
        /// root transform of player
        [SerializeField]
        private Transform m_Transform;
        /// head transform of player
        [SerializeField]
        private Transform m_HeadTransform;
        /// the player's camera
        [SerializeField]
        private Transform m_PlayerCamera;

        /// syncs head rotation over network
        [SyncVar(hook = "SyncRotations")]
        private float m_SyncRotation;
        /// syncs head rotation over network
        [SyncVar(hook = "SyncHeadRotations")]
        private Quaternion m_SyncHeadRotation;

        // sync head rotations of other players, used for historical interpolation
        private List<float> m_SyncRotations = new List<float>();

        // sync head rotations of other players, used for historical interpolation
        private List<Quaternion> m_SyncHeadRotations = new List<Quaternion>();

        private float m_SmoothingFactor = 15f;
        [SerializeField]
        private float m_SmoothingFactorNormal = 15f;
        [SerializeField]
        private float m_SmoothingFactorFast = 25f;

        // minimum rotation angle in degrees the player has to turn before transmitting the new orientation to the server
        [SerializeField]
        private float m_MinTurnTreshold = 5f;

        private Quaternion m_PreviousOrientation;
        private float m_PreviousRotationAngle;
        private Quaternion m_PreviousHeadOrientation;

        [SerializeField]
        private float m_RotationRemoveTreshold = 3f;

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

            m_SmoothingFactor = m_SmoothingFactorNormal;
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

            //LerpSimple();
            InterpolateHistorical();

        }
        #endregion

        private void InterpolateSimple()
        {
            Vector3 playerRotation = new Vector3(0f, m_SyncRotation, 0f);
            m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, Quaternion.Euler(playerRotation), Time.deltaTime * m_SmoothingFactor);
            m_HeadTransform.rotation = Quaternion.Lerp(m_HeadTransform.rotation, m_SyncHeadRotation, Time.deltaTime * m_SmoothingFactor);
        }
        /// <summary>
        /// Interpolates the root transform's and the head transform's rotation based on history of rotations that have been received via the network
        /// </summary>
        private void InterpolateHistorical()
        {
            if(m_SyncRotations.Count > 0)
            {
                Vector3 playerRotation = new Vector3(0f, m_SyncRotations[0], 0f);
                m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, Quaternion.Euler(playerRotation), Time.deltaTime * m_SmoothingFactor);

                // remove rotation if it is within threshold
                if (Mathf.Abs(m_Transform.eulerAngles.y - m_SyncRotations[0]) < m_RotationRemoveTreshold)
                    m_SyncRotations.RemoveAt(0);
            }

            //Debug.Log("Rotations root: " + m_SyncHeadRotations.Count + " ### Rotations head: " + m_SyncHeadRotations.Count);

            if (m_SyncHeadRotations.Count > 0)
            {
                if (m_HeadTransform.rotation != m_SyncHeadRotations[0])
                {
                    m_HeadTransform.rotation = Quaternion.Lerp(m_HeadTransform.rotation, m_SyncHeadRotations[0], Time.deltaTime * m_SmoothingFactor);
                }

                // remove head rotation if it is within threshold
                if (Quaternion.Angle(m_HeadTransform.rotation, m_SyncHeadRotations[0]) < m_RotationRemoveTreshold)
                {
                    m_SyncHeadRotations.RemoveAt(0);
                }
            }

            // lerp faster when queue becomes too long
            if (m_SyncRotations.Count > 5 || m_SyncHeadRotations.Count > 5)
                m_SmoothingFactor = m_SmoothingFactorFast;
            else
                m_SmoothingFactor = m_SmoothingFactorNormal;
        }

        #region NETWORKING METHODS
        [Command]
        void CmdProvideRotationToServer(float in_Rotation, Quaternion in_HeadRotation)
        {
            m_SyncRotation = in_Rotation;
            m_SyncHeadRotation = in_HeadRotation;
        }

        [ClientCallback]
        private void TransmitRotation()
        {

            bool hasPlayerTurned = (Mathf.Abs(m_Transform.eulerAngles.y- m_PreviousRotationAngle) > m_MinTurnTreshold)
                                 || Quaternion.Angle(m_PlayerCamera.rotation, m_PreviousHeadOrientation) > m_MinTurnTreshold; ;
            
            if(hasPlayerTurned == true)
            {
                CmdProvideRotationToServer(m_Transform.eulerAngles.y, m_PlayerCamera.rotation);

                // store snapshot of rotations
                m_PreviousRotationAngle = m_Transform.eulerAngles.y;
                m_PreviousHeadOrientation = m_PlayerCamera.rotation;

            }

        }

        /// <summary>
        /// Hook for root rotation syncing. Stores latest rotation in a queue
        /// </summary>
        [ClientCallback]
        private void SyncRotations(float in_Angle)
        {
            m_SyncRotation = in_Angle;

            if (!isLocalPlayer)
                m_SyncRotations.Add(in_Angle);
        }

        /// <summary>
        /// Hook for head rotation syncing
        /// </summary>
        [ClientCallback]
        private void SyncHeadRotations(Quaternion in_Rotation)
        {
            m_SyncHeadRotation = in_Rotation;

            if (!isLocalPlayer)
                m_SyncHeadRotations.Add(in_Rotation);
        }
        #endregion

        #region EVENT HANDLER
        #endregion

        #region PROPERTIES
        #endregion

    }
}