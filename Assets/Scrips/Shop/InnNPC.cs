using UnityEngine;
using UnityEngine.InputSystem;

public class InnNPC : MonoBehaviour
{
    // Canvas‚É‚Â‚¢‚Ä‚¢‚éInnController
    [SerializeField] private InnController innController;

    private bool canOpenInn;

    // F‚ğ‰Ÿ‚¹‚éó‘Ô‚©
    private bool interactionReady = true;

    private void Update()
    {
        if (!canOpenInn) return;
        if (Keyboard.current == null) return;

        // h‰®‚ªŠJ‚¢‚Ä‚¢‚éŠÔ‚Í
        // Ä“ü—Í‹Ö~‚É‚µ‚Ä‚¨‚­
        if (InnController.IsOpen)
        {
            interactionReady = false;
            return;
        }

        // h‰®‚ğ•Â‚¶‚½ŒãA
        // F‚ğˆê“x—£‚·‚Ü‚ÅÄƒI[ƒvƒ“‹Ö~
        if (!interactionReady)
        {
            if (!Keyboard.current.fKey.isPressed)
            {
                interactionReady = true;
            }

            return;
        }

        // F‚Åh‰®‚ğŠJ‚­
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (innController == null)
            {
                Debug.LogError("InnController‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
                return;
            }

            interactionReady = false;

            innController.OpenInn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        canOpenInn = true;

        interactionReady =
            Keyboard.current == null ||
            !Keyboard.current.fKey.isPressed;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        canOpenInn = false;
        interactionReady = true;
    }
}