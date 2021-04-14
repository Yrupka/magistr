using UnityEngine;

public class Rpm_slider : Item_highligh
{
    private float val;
    private float sensitivity;

    private void Awake()
    {
        val = 0.18f;
        sensitivity = 0.03f;
    }

    private void OnMouseDrag()
    {
        float dx = Input.GetAxis("Mouse X");

        dx *= sensitivity;

        val -= dx;
        val = Mathf.Clamp(val, -0.18f, 0.18f); // ограничение перемещения от -0.18 градусов до 0.18

        transform.localPosition = new Vector3(0, 0.078f, val);
    }

    public float Get_procent()
    {
        return Mathf.InverseLerp(0.18f, -0.18f, val);
    }
}
