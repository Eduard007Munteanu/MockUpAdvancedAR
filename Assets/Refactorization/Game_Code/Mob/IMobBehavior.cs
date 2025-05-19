using UnityEngine;

public interface IMobBehavior
{
    void Init(DefaultMob mob, bool stationary = false); // Optional if different move patterns
    
    void ActionLoop(); //As Tick

    void InitMove(Vector3 destination, GameObject colliderObj);

    void OnClick();
}
