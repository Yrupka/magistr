using System.Collections;
using UnityEngine;

public class Gauge : MonoBehaviour
{
    public string value_name;

    private const float value_max_angle = -126; // угол максимального значения
    private const float min_value_angle = 126; // угол минимального значения
    private const int label_amount = 7; // количество подписей

    private float value; // текущее значение
    private float value_old; // запаздывающее значение для плавного хода
    private float value_max; // максимальное значение

    private Transform needle;
    private Transform label;
    private Transform value_text;
    private Transform info;

    void Awake()
    {
        needle = transform.Find("Needle");
        label = transform.Find("Label");
        info = transform.Find("Info");
        value_text = transform.Find("Value");
        info.GetComponent<TextMesh>().text = value_name;
        label.gameObject.SetActive(false);
        value = 0f;
        value_max = 1f;
        enabled = false;
    }

    void Update()
    {
        value = Mathf.Clamp(value, 0f, value_max);
        value_old = Mathf.Lerp(value_old, value, Time.deltaTime * 3f);
        needle.localEulerAngles = new Vector3(90f, 0f, Get_rotation());
        value_text.GetComponent<TextMesh>().text = value_old.ToString("0.00");
    }

    private float Get_rotation()
    {
        float total_angle = min_value_angle - value_max_angle;
        float value_norm = value_old / value_max;
        return min_value_angle - value_norm * total_angle;
    }

    private void Create_labels()
    {
        float total_angle = min_value_angle - value_max_angle;
        float radius = Mathf.Sqrt(Mathf.Pow(label.localPosition.x, 2) + Mathf.Pow(label.localPosition.z, 2));

        for (int i = 0; i <= label_amount; i++) 
        {
            Transform value_label = Instantiate(label, transform);
            float label_norm = (float)i / label_amount;
            float label_angle = min_value_angle - label_norm * total_angle;
            value_label.localPosition = new Vector3(-1 * radius * Mathf.Cos(label_angle * Mathf.Deg2Rad), 0.11f, -1 * radius * Mathf.Sin(label_angle * Mathf.Deg2Rad));
            value_label.GetComponent<TextMesh>().text = Mathf.RoundToInt(label_norm * value_max).ToString();
            value_label.gameObject.SetActive(true);
        }
        needle.SetAsLastSibling(); // стрелка поверх циферблата
    }

    public IEnumerator Value_zero() // обнуляет значение со временем
    {
        while(value > 1)
        {
            value = Mathf.Lerp(value, 0, Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }
        value = 0f;
    }

    public void Not_in_work() // обнуляет значение со временем
    {
        StartCoroutine(Value_zero());
    }

    public void In_work() // прекращает обнуление
    {
        StopCoroutine(Value_zero());
    }

    public void Value(float val)
    {
        value = val;
    }

    public void Set_max_value(float max_val)
    {
        value_max = max_val;
        Create_labels();
        enabled = true;
    }
}
