using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMinimapManager : MonoBehaviour
{

    private Transform m_player;

    // Start is called before the first frame update
    void Start()
    {
         m_player = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 position = new Vector3(m_player.transform.position.x,transform.position.y, m_player.transform.position.z);
        transform.position = position;

            
        transform.rotation = Quaternion.Euler(90, m_player.transform.eulerAngles.y, 0);


    }
}
