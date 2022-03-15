using UnityEngine;
using System.Collections;

public class Degree : MonoBehaviour
{
    private TextMesh info;
    private int degree;
    private void Awake()
    {
        info = transform.Find("Info").GetComponent<TextMesh>();
        transform.Find("Button_1").GetComponent<Item_button>().Add_listener(Button_1);
        transform.Find("Button_2").GetComponent<Item_button>().Add_listener(Button_2);
        degree = 0;
        Info_set(degree);
    }

    private void Button_1()
    {
        if (degree > -40)
            degree--;
        Info_set(degree);
    }

    private void Button_2()
    {
        if (degree < 40)
            degree++;
        Info_set(degree);
    }

    private void Info_set(int value)
    {
        info.text = value.ToString();
    }

    public int Get_degree()
    {
        return degree;
    }
}
