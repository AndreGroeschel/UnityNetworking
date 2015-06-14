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
    /// Syncronizes the player's head orientation over the network
    /// </summary>
    public class PlayerHeadRotation : NetworkBehaviour 
	{
        #region MEMBER VARIABLES
        [SyncVar]
        private Quaternion m_SyncPlayerRotation;

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
            m_HeadTransform.rotation = Quaternion.Lerp(m_HeadTransform.rotation, m_SyncPlayerRotation, Time.deltaTime * m_SmoothingFactor);
        }
        #endregion

        #region NETWORKING METHODS
        [Command]
        void CmdProvideRotationToServer(Quaternion in_Rotation)
        {
            m_SyncPlayerRotation = in_Rotation;
        }

        [ClientCallback]
        private void TransmitRotation()
        {
            CmdProvideRotationToServer(m_PlayerCamera.rotation);
        }
        #endregion

        #region EVENT HANDLER
        #endregion

        #region PROPERTIES
        #endregion

    }
}