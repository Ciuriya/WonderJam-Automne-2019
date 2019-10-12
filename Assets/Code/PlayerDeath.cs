﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{

    public bool m_isPraisingTheSun = false;
    public double m_expositionRate = 0d;
    public GameObject m_spawnPoint;
    //public GameObject m_player;
    public Slider m_expositionRateLevel;
    [HideInInspector]
    public Transform m_spawnPosition;
    //[HideInInspector]
    //public PlayerControllerWannabe m_playerController;
    

    // Start is called before the first frame update
    void Start()
    {  
        m_spawnPosition = m_spawnPoint.transform;
        gameObject.transform.position = m_spawnPosition.position;
        //m_playerController = m_player.GetComponent<PlayerControllerWannabe>();
        //Instantiate(m_player, m_spawnPosition.position, Quaternion.identity);     
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPraisingTheSun)
        {
            m_expositionRate += 0.3 * Time.deltaTime;
            m_expositionRateLevel.value = (float)m_expositionRate;
            if (m_expositionRate >= 1)
                Death();
        }
        else if(!m_isPraisingTheSun && m_expositionRate > 0)
        {
            m_expositionRate -= 0.3 * Time.deltaTime;
            m_expositionRateLevel.value = (float)m_expositionRate;
            if (m_expositionRate < 0)
                m_expositionRate = 0;
        }
    }

    void Death()
    {
        transform.position = m_spawnPosition.position;
        m_expositionRate = 0d;
        //Instantiate(m_player, m_spawnPosition.position, Quaternion.identity);
    }
}