using UnityEngine;

// This is an Interface. It acts as a contract.
// Any script we put on a door or object MUST have an Interact() function if it uses this.
public interface IInteractable
{
    void Interact();
}