using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName; 

    public void Interact()
    {
        Debug.Log(objectName + " 와 상호작용!");
    }
}
