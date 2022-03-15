using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stand_controller_lab_1 : MonoBehaviour
{
    private Gauge gauge_rpm;
    private Gauge gauge_p;
    private Gauge gauge_load;
    private Load_switch load_switch;
    private Rpm_slider rpm_slider;
    private Starter starter;
    private Info_system info_system;
    private Temperature temperature;
    private Engine_options_lab_1 options;
    private AudioSource sound_source;
    public Fuel_controller fuel_controller;
    public Animator anim;
    public AudioClip[] engine_sounds;

    private List<float> interpolated_rpms;
    private List<float> interpolated_consumtions;
    private List<float> interpolated_moments;
    private bool engine_state;
    private bool load_state;
    private float moment;
    private float load;
    private float rpm;
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
        temperature.Add_listener_heated(Engine_heat_ready);
        enabled = false; // функция обновления не будет работать
    }

    private void FixedUpdate()
    {
        if (engine_state)
        {
            load = load_switch.Get_procent() * options.max_moment;
            rpm = Mathf.Lerp(600f, 7000f, rpm_slider.Get_procent() - load_switch.Get_procent());
            moment = Interpolate(rpm, interpolated_moments) - load;
            fuel_weight -= Interpolate(rpm, interpolated_consumtions) * temperature.Penalty();

            sound_source.pitch = rpm / 2000f;
            sound_source.volume = Mathf.InverseLerp(600f, 7000f, rpm) + 0.3f;

            fuel_controller.Fuel_spent(fuel_weight); // обновление количества топлива для весов
            anim.SetFloat("speed", rpm / 700); // установка скорости для анимации

            if (moment <= 0 && !load_state)
            {
                StartCoroutine("Engine_load_stop");
                load_state = true;
            }
            if (moment > 0 && load_state)
            {
                StopCoroutine("Engine_load_stop");
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
        interpolated_rpms = Calculation_formulas.Interpolated_x(
                options.Get_list_rpm(), options.interpolation);
        interpolated_moments = Calculation_formulas.Interpolate_dublicate(
            options.Get_list_rpm(), options.Get_list_moment(), interpolated_rpms);

        for (int i = 0; i < interpolated_moments.Count; i++)
            interpolated_moments[i] /= options.lever_length;
        interpolated_consumtions = Calculation_formulas.Interpolate_dublicate(
            options.Get_list_rpm(), options.Get_list_consumption(), interpolated_rpms);
        for (int i = 0; i < interpolated_consumtions.Count; i++)
            interpolated_consumtions[i] /= 3000f;

        engine_state = false;
        rpm = 0f;
        rpm = 0f;
        load_state = false;

        options.max_moment = Mathf.Max(interpolated_moments.ToArray());

        gauge_rpm.Set_max_value(7000f);
        gauge_p.Set_max_value(options.max_moment);
        gauge_load.Set_max_value(options.max_moment);
        temperature.Heat_time_set(options.heat_time);
    }

    private float Interpolate(float rpm_val, List<float> info)
    {
        int index = interpolated_rpms.BinarySearch(rpm_val);

        if (index == -1)
            return info[0];

        if (index >= 0)
            return info[index];

        if (index < 0)
            index = ~index;

        if (index == interpolated_rpms.Count)
            index--;
        float procent = Mathf.InverseLerp(interpolated_rpms[index - 1], interpolated_rpms[index], rpm_val);

        return Mathf.Lerp(info[index - 1], info[index], procent);
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
        StopCoroutine("Engine_starter_in_work");
    }

    private void Engine_start()
    {
        StartCoroutine("Engine_starter_in_work");

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
        moment = 0;

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

    public void Load_options(Engine_options_lab_1 loaded_options) // получить загруженные данные
    {
        options = loaded_options;
        // всё что ниже было в Start
        transform.Find("Lever_length").Find("Info").GetComponent<TextMesh>().text
            = options.lever_length.ToString() + "м";
        Setup_values();
        // это вообще тогда надо удалить, небыло до костыля
        enabled = true;
    }
}