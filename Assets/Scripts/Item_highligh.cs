using UnityEngine;

public class Item_highligh : MonoBehaviour
{
    private void OnMouseEnter()
    {
        transform.GetComponent<Renderer>().material.color += Color.white;
    }
    private void OnMouseExit()
    {
        transform.GetComponent<Renderer>().material.color -= Color.white;
    }
}
