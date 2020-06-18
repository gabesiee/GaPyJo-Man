using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttonmanager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button b in buttons) {

        }
    }


}
