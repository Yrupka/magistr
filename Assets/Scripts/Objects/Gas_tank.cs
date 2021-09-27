using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Gas_tank : Item_highligh
{
    private AudioSource sound;
    private Animator anim;
    private int fuel_add_amount;
    private bool interactable;
    private UnityAction click_action;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0.3f);
        interactable = true; 
    }

    private void OnMouseUp()
    {
        if (interactable)
            click_action();
    }

    IEnumerator Animation() // для предотвращения нажатия на объект во время анимации
    {
        interactable = false;
        anim.Play("Fuel_add");
        sound.Play();
        yield return new WaitForSeconds(3f);
        interactable = true;
    }

    public void Set_fuel(int fuel) // получить загруженные данные
    {
        fuel_add_amount = fuel;
    }

    public void Play_animation()
    {
        StartCoroutine(Animation());
    }

    public int Get_fuel()
    {
        return fuel_add_amount;
    }

    public void Add_listener(UnityAction action)
    {
        click_action += action;
    }
}
