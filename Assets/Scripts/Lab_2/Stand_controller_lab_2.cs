using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stand_controller_lab_2 : MonoBehaviour
{
    private struct rpm_items
    {
        public List<float> moment;
        public List<float> consumption;
        public List<float> degree;
        public List<float> load;
    }
    private Gauge gauge_rpm;
    private Gauge gauge_p;
    private Gauge gauge_load;
    private Load_switch load_switch;
    private Rpm_slider rpm_slider;
    private Starter starter;
    private Info_system info_system;
    private Temperature temperature;
    private Degree degree;
    private Engine_options_lab_2 options;
    private AudioSource sound_source;
    public Fuel_controller fuel_controller;
    public Animator anim;
    public AudioClip[] engine_sounds;

    private List<float> interpolated_rpms;
    private List<rpm_items> interpolated_items;
    private bool engine_state;
    private bool load_state;

    // показатели в текущий момент времени
    private int rpm;
    private float moment;
    private float load;
    private float fuel_weight;

    private void Start()
    {
        sound_source = GetComponent<AudioSource>();
        gauge_rpm = transform.Find("Gauge_rpm").GetComponent<Gauge>();
        gauge_p = transform.Find("Gauge_p").GetComponent<Gauge>();
        gauge_load = transform.Find("Gauge_load").GetComponent<Gauge>();
        load_switch = transform.Find("Load_switch").Find("Head").GetComponent<Load_switch>();
        rpm_slider = transform.Find("Rpm_slider").Find("Head").GetComponent<Rpm_slider>();
        starter = transform.Find("Starter").Find("Head").GetComponent<Starter>();
        starter.Add_listener_prestarted(Engine_prestart);
        starter.Add_listener_started(Engine_start);
        starter.Add_listener_stoped(Engine_stop);
        info_system = transform.Find("Info_system").GetComponent<Info_system>();
        temperature = transform.Find("Temperature").GetComponent<Temperature>();
        degree = transform.Find("Degree").GetComponent<Degree>();
        temperature.Add_listener_heated(Engine_heat_ready);
        enabled = false; // функция обновления не будет работать
    }

    private void FixedUpdate() // выполняется фиксированно раз в 0.02 секунды (50 раз за секунду)
    {
        if (engine_state)
        {
            rpm = (int)Mathf.Lerp(600f, 7000f, rpm_slider.Get_procent());
            load = load_switch.Get_procent() * options.max_load;

            Interpolate();

            sound_source.pitch = rpm / 2000f;
            sound_source.volume = Mathf.InverseLerp(600f, 7000f, rpm) + 0.3f;
            fuel_controller.Fuel_spent(fuel_weight); // обновление количества топлива для весов
            anim.SetFloat("speed", rpm / 700); // установка скорости для анимации

            if (moment < load && !load_state)
            {
                StartCoroutine(Engine_load_stop());
                load_state = true;
            }
            if (moment > load && load_state)
            {
                StopCoroutine(Engine_load_stop());
                load_state = false;
            }

            gauge_rpm.Value(rpm);
            gauge_p.Value(moment);
            gauge_load.Value(load);

            if (fuel_weight <= 0)
            {
                Engine_stop();
                info_system.Fuel(true);
            }
        }
    }

    private void Setup_values()
    {
        // interpolated_rpms = Calculation_formulas.Interpolated_x(
        //         options.Get_list_rpm(), options.interpolation);
        // interpolated_moments = Calculation_formulas.Interpolated_y(
        //     options.Get_list_rpm(), options.Get_list_moment(), interpolated_rpms);
        // for (int i = 0; i < interpolated_moments.Count; i++)
        //     interpolated_moments[i] /= options.lever_length;
        // interpolated_consumtions = Calculation_formulas.Interpolated_y(
        //     options.Get_list_rpm(), options.Get_list_consumption(), interpolated_rpms);
        // for (int i = 0; i < interpolated_consumtions.Count; i++)
        //     interpolated_consumtions[i] /= 3600f;


        interpolated_rpms = options.Get_list_rpm();
        interpolated_rpms.Sort(); // на всякий случай, если вносили изменения в файл сохранения
        var result = interpolated_rpms.GroupBy(
            x => x,
            (k, val) => new
            {
                Key = k,
                Count = val.Count()
            }
        );

        interpolated_items = new List<rpm_items>(result.Count());
        int count = 0;
        foreach (var item in result)
        {
            float[] moments = new float[item.Count];
            float[] consumptions = new float[item.Count];
            float[] degrees = new float[item.Count];
            float[] loads = new float[item.Count];
            for (int i = 0; i < item.Count; i++)
            {
                moments[i] = options.rpms[count].moment;
                consumptions[i] = options.rpms[i].consumption / (3600f * 50f);
                degrees[i] = options.rpms[i].deg;
                loads[i] = options.rpms[i].load;
                count++;
            }
            // необходимо отсортировать все массивы по порядку на основании градусов
            // только по массиву градусов происходит бинарный поиск
            System.Array.Sort((float[])degrees.Clone(), moments);
            System.Array.Sort((float[])degrees.Clone(), consumptions);
            System.Array.Sort(degrees, loads);

            rpm_items items = new rpm_items();
            items.moment = new List<float>(moments);
            items.consumption = new List<float>(consumptions);
            items.degree = new List<float>(degrees);
            items.load = new List<float>(loads);
            interpolated_items.Add(items);
        }
        interpolated_rpms = interpolated_rpms.Distinct().ToList();
        engine_state = false;
        rpm = 0;
        load_state = false;

        gauge_rpm.Set_max_value(7000f);
        gauge_p.Set_max_value(options.max_moment / options.lever_length);
        gauge_load.Set_max_value(options.max_load);
        temperature.Heat_time_set(options.heat_time);
    }

    // функция, которая считает все параметры для стенда
    private void Interpolate()
    {
        /* 1 шаг - найти нужные данные по (момент | топливо) в значениях для оборотов находящихся
        по краям от текущего значения оборотов
        2 шаг - с помощью найденных значений, найти текущее значение для (момент | топливо),
        используя value_1, value_2 как крайние границы, а процент это значение зависещее от оборотов
        */

        int index = Get_index(interpolated_rpms, rpm);
        // момент
        float moment_1 = Get_value(interpolated_items[index].moment, index);
        // топливо
        float fuel_1 = Get_value(interpolated_items[index].consumption, index);

        if (index != 0)
        {
            float procent = Mathf.InverseLerp(
                    interpolated_rpms[index - 1], interpolated_rpms[index], rpm);
            
            float moment_2 = Get_value(interpolated_items[index - 1].moment, index - 1);
            moment = Mathf.LerpUnclamped(moment_2, moment_1, procent);
            
            float fuel_2 = Get_value(interpolated_items[index - 1].consumption, index - 1);
            fuel_weight -= Mathf.Lerp(fuel_2, fuel_1, procent) * temperature.Penalty();
        }
        else
        {
            moment = moment_1;
            fuel_weight -= fuel_1 * temperature.Penalty();
        }    
    }

    // поиск и интерполяция значения в данном списке на основании значения УОЗ
    private float Get_value(List<float> list, int index)
    {
        float value = degree.Get_degree();
        int ind = Get_index(interpolated_items[index].degree, value);
        if (ind == 0)
            return list[0];

        float procent = Mathf.InverseLerp(
            interpolated_items[index].degree[ind - 1], interpolated_items[index].degree[ind], value);

        return Mathf.Lerp(list[ind - 1], list[ind], procent);
    }

    private int Get_index(List<float> list, float value)
    {
        int index = list.BinarySearch(value);
        if (index == -1)
            return 0;

        if (index > 0)
            return index;

        if (index < 0)
            index = ~index;

        if (index == list.Count)
            index--;

        return index;
    }

    private void Engine_heat_ready() // двигатель нагрет
    {
        info_system.Temp(false);
    }

    private void Engine_prestart() // ключ во 2 положении
    {
        if (!engine_state)
        {
            if (!info_system.Fuel_state())
                info_system.Pre_start();
            sound_source.Stop();
        }
        else
            starter.Block(true);
        StopCoroutine(Engine_starter_in_work());
    }

    private void Engine_start()
    {
        StartCoroutine(Engine_starter_in_work());

        gauge_rpm.In_work();
        gauge_p.In_work();
        gauge_load.In_work();
    }

    private void Engine_stop()
    {
        info_system.Off();
        temperature.Heat(false);
        starter.Block(false);
        if (engine_state)
            Play_sound(2);
        engine_state = false;
        load_state = false;
        fuel_controller.Set_engine_state(false);
        anim.SetFloat("speed", 0);
        rpm = 0;
        load = 0;

        gauge_rpm.Not_in_work();
        gauge_p.Not_in_work();
        gauge_load.Not_in_work();
    }

    private void Play_sound(int index)
    {
        sound_source.clip = engine_sounds[index];
        sound_source.pitch = 1;
        sound_source.volume = 0.3f;
        sound_source.Play();
        if (index == 1)
            sound_source.loop = true;
        else
            sound_source.loop = false;
    }

    private IEnumerator Engine_starter_in_work()
    {
        Play_sound(0);
        yield return new WaitForSeconds(1f);
        fuel_weight = fuel_controller.Fuel_get(); // получили топливо
        if (fuel_weight > 0)
        {
            info_system.Check(true);
            info_system.Fuel(false);
            info_system.Temp(true);
            temperature.Heat(true);
            engine_state = true;
            Play_sound(1);
        }
        else
            info_system.Fuel(true);
    }

    private IEnumerator Engine_load_stop()
    {
        yield return new WaitForSeconds(2f);
        Engine_stop();
    }

    public void Load_options(Engine_options_lab_2 loaded_options) // получить загруженные данные
    {
        options = loaded_options;
        Setup_values();
        enabled = true;
    }
}