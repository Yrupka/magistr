using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;

public class Menu_options : MonoBehaviour
{
    private Slider mouse_slider;
    private InputField mouse_input;
    private Slider sound_slider;
    private Toggle sound_toggle;
    private InputField sound_input;
    private System.Globalization.CultureInfo culture;
    public AudioMixerGroup audio_mixer;

    private UnityAction action_mouse;

    private float mouse_sens_value;
    private float sound_level;
    public bool in_game;

    private void Awake()
    {
        Transform menu = transform.Find("Menu");
        // для разшения формата float с точкой вместо запятой
        culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

        mouse_slider = menu.Find("Mouse_sens").Find("Slider").GetComponent<Slider>();
        mouse_slider.onValueChanged.AddListener(
            (value) => mouse_input.text = value.ToString(culture));

        mouse_input = menu.Find("Mouse_sens").Find("Number").GetComponent<InputField>();
        mouse_input.onValueChanged.AddListener(Mouse_sensitivity_input);

        sound_slider = menu.Find("Sound").Find("Slider").GetComponent<Slider>();
        sound_slider.onValueChanged.AddListener(
            (value) => sound_input.text = value.ToString(culture));

        sound_input = menu.Find("Sound").Find("Number").GetComponent<InputField>();
        sound_input.onValueChanged.AddListener(Sound_input);

        sound_toggle = menu.Find("Sound").Find("Toggle").GetComponent<Toggle>();
        if (in_game)
            sound_toggle.onValueChanged.AddListener(Sound_toggle);

        menu.Find("Back").GetComponent<Button>().onClick.AddListener(Back);

        mouse_sens_value = 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    private void Sound_input(string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "1";
        sound_level = float.Parse(text, culture);
        sound_level = Mathf.Clamp(sound_level, 1f, 10f);
        sound_slider.value = sound_level;

        if (in_game)
            audio_mixer.audioMixer.SetFloat("Master_volume", Mathf.Lerp(-80, 20, sound_level / 10));
    }

    private void Mouse_sensitivity_input(string text)
    {
        if (string.IsNullOrEmpty(text))
            text = "1";
        mouse_sens_value = float.Parse(text, culture);
        mouse_sens_value = Mathf.Clamp(mouse_sens_value, 1f, 10f);
        mouse_input.text = mouse_sens_value.ToString(culture);
        mouse_slider.value = mouse_sens_value;

        if (in_game)
            action_mouse();
    }

    private void Sound_toggle(bool value)
    {
        if (value)
            audio_mixer.audioMixer.SetFloat("Master_volume", sound_level);
        else
            audio_mixer.audioMixer.SetFloat("Master_volume", -80f);
    }

    private void Back()
    {
        transform.gameObject.SetActive(false);
    }

    public void Add_mouse_sens_listener(UnityAction action)
    {
        action_mouse += action;
    }

    public float Get_sens()
    {
        return mouse_sens_value;
    }
}
