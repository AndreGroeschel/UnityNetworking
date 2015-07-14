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
	/// Sets up the player for networking. Switches player components on / off if player is local or network client
	/// </summary>
	public class PlayerNetworkSetup : NetworkBehaviour
	{
        #region MEMBER VARIABLES
        [SerializeField]
        private GameObject m_Head;

        [SerializeField]
        private GameObject m_UI;

        [SerializeField]
        private GameObject m_Weapon;

        [SerializeField]
        private GameObject m_EgoPerspWeaponCam;

        [SerializeField]
        private string m_GunLayer;

        #endregion

        #region UNITY FUNCTIONS

        void Start()
		{
			// enable components that are required if player is local
			Camera cam = GetComponentInChildren<Camera>();
			AudioListener audioListener = GetComponentInChildren<AudioListener>();
			UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();

			if(isLocalPlayer == true)
			{	
				cam.enabled = true;
				audioListener.enabled = true;
				fpsController.enabled = true;
                m_Head.SetActive(false);
                m_UI.SetActive(true);
                m_EgoPerspWeaponCam.SetActive(true);
                m_Weapon.layer = LayerMask.NameToLayer(m_GunLayer);
            }
			else
			{
				cam.enabled = false;
				audioListener.enabled = false;
				fpsController.enabled = false;
                m_Head.SetActive(true);
                m_UI.SetActive(false);
                m_EgoPerspWeaponCam.SetActive(false);
                m_Weapon.layer = this.gameObject.layer;
            }
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