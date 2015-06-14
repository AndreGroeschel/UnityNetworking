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

        [SerializeField]
        private float m_SmoothingFactor = 15f;

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

        void FixedUpdate()
        {
            if (isLocalPlayer == true)
            {
                // transmit rotation to server
                TransmitRotation();
            }
            // interpolate head rotations of other players
            else
            {
                InterpolateRotation();
            }
        }

        #endregion

        #region METHODS
        private void InterpolateRotation()
        {
            m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_SyncRotation, Time.deltaTime * m_SmoothingFactor);
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
            CmdProvideRotationToServer(m_Transform.rotation, m_PlayerCamera.rotation);
        }
        #endregion

        #region EVENT HANDLER
        #endregion

        #region PROPERTIES
        #endregion

    }
}