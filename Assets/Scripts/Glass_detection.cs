using UnityEngine.Events;
using UnityEngine;

public class Glass_detection : MonoBehaviour
{
    private UnityAction enter_action;
    private UnityAction exit_action;

    private void OnTriggerEnter(Collider other)
    {
        enter_action();
    }

    private void OnTriggerExit(Collider other)
    {
        exit_action();
    }

    public void Add_listener_enter(UnityAction action)
    {
        enter_action += action;
    }

    public void Add_listener_exit(UnityAction action)
    {
        exit_action += action;
    }
}
