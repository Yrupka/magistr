using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Fuel_pomp : Item_highligh
{
    private AudioSource sound;
    private Animator anim;
    private UnityAction click_action;

    private bool work_position;
    private bool interactable;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        work_position = false;
        interactable = true;
    }

    private void OnMouseUp()
    {
        if (interactable)
            click_action();
    }

    IEnumerator Animation(string anim_name) // для предотвращения нажатия на объект во время анимации
    {
        interactable = false;
        anim.Play(anim_name);
        yield return new WaitForSeconds(1f);
        interactable = true;
    }

        public void Set_state(bool state)
    {
        work_position = state;
    }

    public bool State_info()
    {
        return work_position;
    }

    public void Play_animation()
    {
        if (work_position)
        {
            StartCoroutine(Animation("Work_unset"));
            sound.time = 1f;
            sound.pitch = -1;
            sound.Play();
        }
        else
        {
            StartCoroutine(Animation("Work_set"));
            sound.time = 0f;
            sound.pitch = 1;
            sound.Play();
        }

    }

    public void Add_listener(UnityAction call)
    {
        click_action += call;
    }
}
