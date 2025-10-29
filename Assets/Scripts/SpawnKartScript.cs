using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnKartScript : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public int m_playerCount;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.GetChild(0).position = SpawnPoints[m_playerCount].transform.position;
        if (m_playerCount == 0)
        {
            playerInput.GetComponentInChildren<KartController>();
        }
        m_playerCount++;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
