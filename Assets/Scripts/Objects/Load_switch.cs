using UnityEngine;

public class Load_switch : Item_highligh
{
    private float val;
    private float sensitivity;

    private void Awake()
    {
        val = -120f;
        sensitivity = 25;
    }

    private void OnMouseDrag()
    {
        // смещение мыши за кадр
        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");

        // создание луча от камеры до мыши
        Vector2 mouse = Input.mousePosition;
        Ray ray;
        ray = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;

        // получение объекта над которым находится мышь
        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.point.x < transform.position.x)
                dy *= sensitivity; // если мышь находится слева от объекта
            else
                dy *= -sensitivity; // если мышь находится справа от объекта
        }

        dx *= sensitivity; // 50 - значение чувствительности

        val += dx + dy;
        val = Mathf.Clamp(val, -120f, 120f); // ограничение угла вращения от -120 градусов до 120

        transform.localEulerAngles = new Vector3(-90f, 0f , val); // поворот на заданный угол
    }

    public float Get_procent()
    {
        return Mathf.InverseLerp(-120f, 120f, val);
    }
}
