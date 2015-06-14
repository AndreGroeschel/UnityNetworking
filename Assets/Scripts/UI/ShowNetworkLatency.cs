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
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace Kontraproduktiv
{
	/// <summary>
	/// Displays the latency in a UI
	/// </summary>
	public class ShowNetworkLatency : NetworkBehaviour 
	{
        #region MEMBER VARIABLES
        public Text m_Label;

        private NetworkClient m_NetworkClient;
		#endregion
		
		#region UNITY FUNCTIONS

		
		void Start ()
		{
            m_NetworkClient = GameObject.Find("Network Manager").GetComponent<NetworkManager>().client;
		}
		
		void Update ()
		{
            if(isLocalPlayer == true)
                m_Label.text = "Latency: " + m_NetworkClient.GetRTT().ToString();
        }
		
		#endregion
		
		#region METHODS
		#endregion
		
		#region EVENT HANDLER
		#endregion
		
		#region PROPERTIES
		#endregion
		
	}
}