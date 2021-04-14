using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Degree : MonoBehaviour
{

    private void Awake()
    {
        transform.Find("Button_1").GetComponent<Item_button>().Add_listener(Button_1);
        transform.Find("Button_2").GetComponent<Item_button>().Add_listener(Button_2);
    }


    private void Update()
    {
        
    }

    private void Button_1()
    {

    }

    private void Button_2()
    {
        
    }
}
