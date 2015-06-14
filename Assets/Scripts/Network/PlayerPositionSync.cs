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
	/// Syncronizes the player's postion over the network
	/// </summary>
	public class PlayerPositionSync : NetworkBehaviour 
	{
		#region MEMBER VARIABLES
		[SyncVar]
		private Vector3 m_SyncPlayerPostion;

		[SerializeField]
		private Transform m_PlayerTransform;

		[SerializeField]
		private float m_SmoothingFactor;
        #endregion

        #region UNITY FUNCTIONS

        void Start ()
		{
			if(m_PlayerTransform == null)
				m_PlayerTransform = transform;
		}
		
		void FixedUpdate ()
		{		
			if(isLocalPlayer == true)
			{
				// transmit position to server
				TransmitPosition();

                // transmit rotation to server
                //TransmitRotation();
            }
			
			else
			{
                // interpolate positions of other players
                InterpolatePostion();

            }
		}
        #endregion


        #region METHODS
        private void InterpolatePostion()
        {

            m_PlayerTransform.position = Vector3.Lerp(m_PlayerTransform.position, m_SyncPlayerPostion, Time.deltaTime * m_SmoothingFactor);
        }
        #endregion

        #region NETWORKING METHODS
        [Command]
		void CmdProvidePositionToServer(Vector3 in_Position)
		{
			m_SyncPlayerPostion = in_Position;
		}


        [ClientCallback]
		private void TransmitPosition()
		{
			CmdProvidePositionToServer(m_PlayerTransform.position);
		}
        #endregion

        #region EVENT HANDLER
        #endregion

        #region PROPERTIES
        #endregion

    }
}