using UnityEngine;

public interface IMobBehavior
{
    void Init(DefaultMob mob, bool stay = false); // Optional if different move patterns
    
    void ActionLoop(); //As Tick

    void InitMove(Vector3 destination, GameObject colliderObj);    // Optional if different move patterns

    void OnClick();
}
