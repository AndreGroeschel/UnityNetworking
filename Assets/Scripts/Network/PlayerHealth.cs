using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerHealth : NetworkBehaviour {

    [SyncVar (hook ="OnHealthChanged") ]
    public int m_Health = 100;
    [SerializeField]
    private Text m_HealthLabel;

    public void ApplyDamage(int in_Damage)
    {
        m_Health -= in_Damage;

        if (isLocalPlayer)
        {
            m_HealthLabel.text = m_Health.ToString();
        }
    }

    private void OnHealthChanged(int in_Health)
    {
        m_Health = in_Health;
        m_HealthLabel.text = m_Health.ToString();
    }
}