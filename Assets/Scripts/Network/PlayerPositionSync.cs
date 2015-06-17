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
	/// Syncronizes the player's postion over the network
	/// </summary>
    [NetworkSettings (channel = 0,sendInterval = 0.1f)]
	public class PlayerPositionSync : NetworkBehaviour 
	{
		#region MEMBER VARIABLES
		[SyncVar (hook = "SyncPositions")]
		private Vector3 m_SyncPlayerPostion;

		[SerializeField]
		private Transform m_PlayerTransform;

        private float m_SmoothingFactor;

        [SerializeField]
        private float m_SmoothingFactorNormal = 15f;
        [SerializeField]
        private float m_SmoothingFactorFast = 25f;

        // minimum distance in meters the player has to move before transmitting the new position to the server
        [SerializeField]
        private float m_MoveThreshold = 0.3f;

        private Vector3 m_PreviousPosition;

        // syncs positions of other players, used for historical interpolation
        private List<Vector3> m_SyncPositions = new List<Vector3>();
        [SerializeField]
        private bool m_UseHistoricalInterpolation = true;

        [SerializeField]
        private float m_PositionRemoveTreshold = 0.1f;
        #endregion

        #region UNITY FUNCTIONS

        void Start ()
		{
			if(m_PlayerTransform == null)
				m_PlayerTransform = transform;

            m_SmoothingFactor = m_SmoothingFactorNormal;
        }

        void Update()
        {
            if (isLocalPlayer == false)
            {
                if (m_UseHistoricalInterpolation == true)
                    InterpolateHistorical();
                //else
                    // interpolate positions of other players
                    //InterpolatePostion();
            }
        }

       

		void FixedUpdate ()
		{		
			if(isLocalPlayer == true)
			{
				// transmit position to server
				TransmitPosition();
            }
		}

        void OnDrawGizmos()
        {

            Gizmos.color = Color.green;
            foreach(Vector3 position in m_SyncPositions)
            {

                Gizmos.DrawWireSphere(position, 0.2f);
            }
        }
        #endregion


        #region METHODS
        private void InterpolatePostion()
        {

            m_PlayerTransform.position = Vector3.Lerp(m_PlayerTransform.position, m_SyncPlayerPostion, Time.deltaTime * m_SmoothingFactor);
        }

        void InterpolateHistorical()
        {
            if(m_SyncPositions.Count > 0)
            {
                m_PlayerTransform.position = Vector3.Lerp(m_PlayerTransform.position, m_SyncPositions[0], Time.deltaTime * m_SmoothingFactor);


                // remove first position in queue when moved closed enough to it
                if (Vector3.Distance(m_PlayerTransform.position, m_SyncPositions[0]) < m_PositionRemoveTreshold)
                {
                    m_SyncPositions.RemoveAt(0);
                }

                // lerp faster when queue becomes too long
                if (m_SyncPositions.Count > 10)
                    m_SmoothingFactor = m_SmoothingFactorFast;
                else
                    m_SmoothingFactor = m_SmoothingFactorNormal;

                //Debug.Log(m_SyncPositions.Count.ToString());
            }
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
            bool hasPlayerMoved = Vector3.Distance(m_PlayerTransform.position, m_PreviousPosition) >= m_MoveThreshold;

            if(hasPlayerMoved == true)
            {
                CmdProvidePositionToServer(m_PlayerTransform.position);

                // store last position
                m_PreviousPosition = m_PlayerTransform.position;

                //Debug.Log("Player moved");
            }
			
		}

        /// <summary>
        /// Hook for position sync. Stores latest position in a queue
        /// </summary>
        [ClientCallback]
        private void SyncPositions(Vector3 in_Position)
        {
            m_SyncPlayerPostion = in_Position;

            if(!isLocalPlayer)
                m_SyncPositions.Add(in_Position);
        }
        #endregion

        #region EVENT HANDLER

        #endregion

        #region PROPERTIES
        #endregion

    }
}