using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName;

    // 길드 여부 확인용
    public bool isGuild;
    public bool isShop;

    public void Interact()
    {
        Debug.Log(objectName + " 와 상호작용!");

        // 길드면 UI 열기
        if (isGuild)
        {
            GuildUIManager.Instance.OpenGuildUI(this);
        }
        if (isShop)
        {
            Debug.Log("상점 UI 열기 시도");

            if (ShopUIManager.Instance == null)
            {
                Debug.LogError("ShopUIManager Instance 없음!");
                return;
            }

            ShopUIManager.Instance.OpenShopUI(this);
        }
    }
}
