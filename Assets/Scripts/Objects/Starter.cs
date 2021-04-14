using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Starter : Item_highligh
{
    enum Starter_state { stop, start, starting };

    private Starter_state state;
    private AudioSource sound;
    private float val;
    private float space_available;
    private List<float> positions;
    private UnityAction action_prestarted;
    private UnityAction action_started;
    private UnityAction action_stoped;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        positions = new List<float>(4){-120f, -40f, 40f, 120f};
        val = -120f;
        space_available = 120f; // максимальный угол поворота
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
                dy *= 50f; // если мышь находится слева от объекта
            else
                dy *= -50f; // если мышь находится справа от объекта
        }

        dx *= 50f; // 50 - значение чувствительности

        val += dx + dy;
        val = Mathf.Clamp(val, -120f, space_available); // ограничение угла вращения от -120 градусов до 120

        transform.localEulerAngles = new Vector3(-90f, 0, val); // поворот на заданный угол

        if (val > 80 && state != Starter_state.starting)
        {
            action_started();
            state = Starter_state.starting;
        }
    }

    private void OnMouseUp()
    {
        int index = positions.BinarySearch(val); // поиск ближайшего положения
        if (index < 0) // если ключ в промежтке между положениями
        {
            sound.Play();
            index = ~index;
            if (positions[index] - 40 < val) // к какому положению ближе, на то и будет установлен
                val = positions[index];
            else
                val = positions[index - 1];
        }
        else
            val = positions[index]; // если ключ прямо на положении

        if (val > 40)
            val = positions[2];
        if (val < 40 && state != Starter_state.stop)
        {
            action_stoped();
            state = Starter_state.stop;
        }
        if (val == 40 && state != Starter_state.start)
        {
            action_prestarted();
            state = Starter_state.start;
        }

        transform.localEulerAngles = new Vector3(-90f, 0, val);
    }

    public void Block(bool value) // если двигатель работает, нельзя запускать стартер
    {
        if (value)
            space_available = 40f;
        else
            space_available = 120f;
    }

    public void Add_listener_prestarted(UnityAction action)
    {
        action_prestarted += action;
    }

    public void Add_listener_started(UnityAction action)
    {
        action_started += action;
    }

    public void Add_listener_stoped(UnityAction action)
    {
        action_stoped += action;
    }
}
