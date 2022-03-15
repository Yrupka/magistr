using UnityEngine;

public class Rpm_slider : Item_highligh
{
    private float val;
    private float sensitivity;

    private void Start()
    {
        val = 0.18f;
        sensitivity = 0.03f;
        transform.parent.Find("Button_1").GetComponent<Item_button>().Add_listener(Button_1);
        transform.parent.Find("Button_2").GetComponent<Item_button>().Add_listener(Button_2);
    }

    private void OnMouseDrag()
    {
        float dx = Input.GetAxis("Mouse X");

        dx *= sensitivity;

        val -= dx;
        val = Mathf.Clamp(val, -0.18f, 0.18f); // ограничение перемещения от -0.18 градусов до 0.18

        transform.localPosition = new Vector3(0, 0.078f, val);
    }

    private void Button_1()
    {
        val += 0.000045f;
        val = Mathf.Clamp(val, -0.18f, 0.18f);
        transform.localPosition = new Vector3(0, 0.078f, val);
    }

    private void Button_2()
    {
        val -= 0.000045f;
        val = Mathf.Clamp(val, -0.18f, 0.18f);
        transform.localPosition = new Vector3(0, 0.078f, val);
    }

    public float Get_procent()
    {
        return Mathf.InverseLerp(0.18f, -0.18f, val);
    }
}
