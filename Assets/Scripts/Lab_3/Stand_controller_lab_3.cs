using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stand_controller_lab_3 : MonoBehaviour
{
    private Gauge gauge_rpm;
    private Gauge gauge_p;
    private Gauge gauge_load;
    private Gauge gauge_air;
    private Load_switch load_switch;
    private Rpm_slider rpm_slider;
    private Starter starter;
    private Info_system info_system;
    private Temperature temperature;
    private Engine_options_lab_3 options;
    private AudioSource sound_source;
    public Fuel_controller fuel_controller;
    public Animator anim;
    public AudioClip[] engine_sounds;

    private List<float> interpolated_rpms;
    private List<float> interpolated_consumtions;
    private List<float> interpolated_moments;
    private List<float> interpolated_air;
    private float max_moment;
    private bool engine_state;
    private bool load_state;
    private int rpm;
    private int rpm_old;
    private float fuel_weight;

    private void Start()
    {
        sound_source = GetComponent<AudioSource>();
        gauge_rpm = transform.Find("Gauge_rpm").GetComponent<Gauge>();
        gauge_p = transform.Find("Gauge_p").GetComponent<Gauge>();
        gauge_load = transform.Find("Gauge_load").GetComponent<Gauge>();
        gauge_air = transform.Find("Gauge_air").GetComponent<Gauge>();
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
            if (fuel_weight > 0)
            {
                float load_procent = rpm_slider.Get_procent() - load_switch.Get_procent();
                rpm = (int)Mathf.Lerp(600f, 7000f, load_procent);
                fuel_weight -= Interpolate(rpm_old, interpolated_consumtions) * temperature.Penalty();

                sound_source.pitch = rpm_old / 2000f;
                sound_source.volume = Mathf.InverseLerp(600f, 7000f, rpm) + 0.3f;

                fuel_controller.Fuel_spent(fuel_weight); // обновление количества топлива для весов
                anim.SetFloat("speed", rpm_old / 700); // установка скорости для анимации

                if (load_procent < 0 && !load_state)
                {
                    StartCoroutine("Engine_load_stop");
                    load_state = true;
                }
                if (load_procent > 0 && load_state)
                {
                    StopCoroutine("Engine_load_stop");
                    load_state = false;
                }
            }
            else
            {
                Engine_stop();
                info_system.Fuel(true);
            }
            rpm_old = (int)Mathf.Lerp(rpm_old, rpm, Time.deltaTime * 3f);
            gauge_rpm.Value(rpm_old);
            gauge_p.Value(Interpolate(rpm_old, interpolated_moments));
            gauge_air.Value(Interpolate(rpm_old, interpolated_air));
        }
        gauge_load.Value(load_switch.Get_procent() * max_moment / options.lever_length);
    }

    private void Setup_values()
    {
        interpolated_rpms = Calculation_formulas.Interpolated_x(
                options.Get_list_rpm(), options.interpolation);
        interpolated_moments = Calculation_formulas.Interpolated_y(
            options.Get_list_rpm(), options.Get_list_moment(), interpolated_rpms);
        for (int i = 0; i < interpolated_moments.Count; i++)
            interpolated_moments[i] /= options.lever_length;
        interpolated_consumtions = Calculation_formulas.Interpolated_y(
            options.Get_list_rpm(), options.Get_list_consumption(), interpolated_rpms);
        for (int i = 0; i < interpolated_consumtions.Count; i++)
            interpolated_consumtions[i] /= 3000f;
        interpolated_air = Calculation_formulas.Interpolated_y(
            options.Get_list_rpm(), options.Get_list_air(), interpolated_rpms);

        engine_state = false;
        rpm = 0;
        rpm_old = 0;
        load_state = false;
        max_moment = Mathf.Max(interpolated_moments.ToArray());

        gauge_rpm.Set_max_value(7000f);
        gauge_p.Set_max_value(max_moment / options.lever_length);
        gauge_load.Set_max_value(max_moment / options.lever_length);
        gauge_air.Set_max_value(Mathf.Max(interpolated_air.ToArray()));
        temperature.Heat_time_set(options.heat_time);
    }

    private float Interpolate(int rpm_val, List<float> info)
    {
        int index = interpolated_rpms.BinarySearch(rpm_val);
        if (index == -1)
            return info[0];
            
        if (index > 0)
            return info[index];

        if (index < 0)
            index = ~index;
        
        if (index == interpolated_rpms.Count)
            index--;
        
        float procent = Mathf.InverseLerp(
                interpolated_rpms[index - 1], interpolated_rpms[index], rpm_val);
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
        StartCoroutine("Gauge_zero");
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

    private IEnumerator Gauge_zero() // обнуление датчиков со временем
    {
        while(rpm_old > 0)
        {
            gauge_rpm.Value(rpm_old);
            gauge_p.Value(Interpolate(rpm_old, interpolated_moments) - Mathf.Min(interpolated_moments.ToArray())); 
            yield return new WaitForSeconds(0.05f);
            rpm_old -= 25;
        }
        rpm_old = 0;
        rpm = 0;
        gauge_rpm.Value(rpm_old);
    }

    public void Load_options(Engine_options_lab_3 loaded_options) // получить загруженные данные
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