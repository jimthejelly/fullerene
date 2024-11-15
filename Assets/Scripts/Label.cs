using UnityEngine;
using UnityEngine.EventSystems;

public class Label : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{ 

    public GameObject label;
    void Start() { }

    void Update() { } 

    public void OnPointerEnter(PointerEventData eventData) {
        label.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        label.SetActive(false);
    }


}