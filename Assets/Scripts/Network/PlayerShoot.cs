using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerShoot : NetworkBehaviour
{

    public int m_Damage = 5;
    public float m_Range = 100f;
    public LayerMask m_Shootable;
    [SerializeField]
    private ParticleSystem m_GunParticles;
    [SerializeField]
    private GameObject m_HitFX = null;
    [SerializeField]
    private AudioSource m_GunShot;
    [SerializeField]
    private Transform m_CameraTransform;
    private Ray m_shootRay;
    private RaycastHit m_RayHit;
    public float m_TimeBetweenBullets = 0.15f;
    private float m_Timer;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        m_Timer += Time.deltaTime;

        if (IsShooting() == true)
        {
            if (m_Timer >= m_TimeBetweenBullets && Time.timeScale != 0)
            {
                Fire();
            }
        }
	}

    private bool IsShooting()
    {
        if(isLocalPlayer && Input.GetKeyDown(KeyCode.Mouse0) == true)
        {
            return true;
        }

        return false;
    }

    private void Fire()
    {
        m_Timer = 0f;

        m_GunShot.Play();
        m_GunParticles.Stop();
        m_GunParticles.Play();


        m_shootRay.origin = m_CameraTransform.position;
        m_shootRay.direction = m_CameraTransform.forward;

        if (Physics.Raycast(m_shootRay, out m_RayHit, m_Range, m_Shootable))
        {

            CmdShoot(m_RayHit.point, m_RayHit.normal);

            if (m_RayHit.transform.tag == "Player")
            {
                Debug.Log("Hit Player " + m_RayHit.transform.name);
                string uniqueIdentity = m_RayHit.transform.name;
                CmdApplyDamageToPlayer(uniqueIdentity, m_Damage);
            }
        }
    }

    [Command]
    public void CmdShoot(Vector3 in_Position, Vector3 in_Normal)
    {

        Debug.Log("Spawn at " + in_Position);
        // create server-side instance
        GameObject explosion = (GameObject)Instantiate(m_HitFX, in_Position, Quaternion.LookRotation(in_Normal));
    
        // destroy after 2 secs
        Destroy(explosion, 2f);
        // spawn on the clients
        NetworkServer.Spawn(explosion);
    }


    [Command]
    private void CmdApplyDamageToPlayer(string in_PlayerID, int in_Damage)
    {
        Debug.Log("Applying damage to player " + in_PlayerID);
        GameObject playerGO = GameObject.Find(in_PlayerID);
        PlayerHealth playerHealth = playerGO.GetComponent<PlayerHealth>();
        playerHealth.ApplyDamage(in_Damage);
    }
}
