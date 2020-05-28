using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * 50, 0));
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Player")
        {
            ScoreManager.scoreValue += 10;
            Destroy(this.gameObject);
        }
    }
}
