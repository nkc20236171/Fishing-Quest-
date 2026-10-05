using UnityEngine;
using UnityEngine.InputSystem;

public class ShopNPC : MonoBehaviour
{
    public enum ShopType
    {
        ItemShop,
        FishingShop,
        WeaponShop
    }

    // このNPCの店の種類
    [SerializeField] private ShopType shopType;

    // CanvasについているShopController
    [SerializeField] private ShopController shopController;

    private bool canOpenShop;

    // Fを押せる状態か
    private bool interactionReady = true;

    private void Update()
    {
        if (!canOpenShop) return;
        if (Keyboard.current == null) return;

        // ショップが開いている間は
        // Fで再び開けない状態にしておく
        if (ShopController.IsOpen)
        {
            interactionReady = false;
            return;
        }

        // ショップを閉じた後、
        // Fを一度離すまで再オープン禁止
        if (!interactionReady)
        {
            if (!Keyboard.current.fKey.isPressed)
            {
                interactionReady = true;
            }

            return;
        }

        // Fでショップを開く
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (shopController == null)
            {
                Debug.LogError("ShopControllerが設定されていません");
                return;
            }

            interactionReady = false;

            shopController.OpenShop(shopType);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        canOpenShop = true;

        // NPCの範囲に入った時、
        // Fを押しっぱなしなら開かない
        interactionReady =
            Keyboard.current == null ||
            !Keyboard.current.fKey.isPressed;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        canOpenShop = false;
        interactionReady = true;
    }
}