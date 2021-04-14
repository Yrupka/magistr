using System.Collections;
using UnityEngine;

public class Info_system : MonoBehaviour
{
    private Renderer check;
    private Renderer temp;
    private Renderer fuel;

    private void Awake()
    {
        check = transform.Find("Check").GetComponent<Renderer>();
        fuel = transform.Find("Fuel").GetComponent<Renderer>();
        temp = transform.Find("Temp").GetComponent<Renderer>();
        check.material.color = Color.clear;
        temp.material.color = Color.clear;
        fuel.material.color = Color.clear;
    }

    IEnumerator Check_light()
    {
        check.material.color = Color.white;
        yield return new WaitForSeconds(1.5f);
        check.material.color = Color.clear;
    }

    public void Check(bool value)
    {
        if (value)
            StartCoroutine(Check_light());
        else
            check.material.color = Color.clear;
    }

    public void Fuel(bool value)
    {
        if (value)
            fuel.material.color = Color.white;
        else
            fuel.material.color = Color.clear;
    }

    public void Temp(bool value)
    {
        if (value)
            temp.material.color = Color.white;
        else
            temp.material.color = Color.clear;
    }

    public void Pre_start()
    {
        check.material.color = Color.white;
        fuel.material.color = Color.white;
        temp.material.color = Color.white;
    }

    public void Off()
    {
        check.material.color = Color.clear;
        fuel.material.color = Color.clear;
        temp.material.color = Color.clear;
    }

    public bool Fuel_state()
    {
        if (fuel.material.color == Color.white)
            return true;
        return false;
    }

}
