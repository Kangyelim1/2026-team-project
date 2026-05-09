using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName;

    // 길드 여부 확인용
    public bool isGuild;

    public void Interact()
    {
        Debug.Log(objectName + " 와 상호작용!");

        // 길드면 UI 열기
        if (isGuild)
        {
            GuildUIManager.Instance.OpenGuildUI(this);
        }
    }
}
