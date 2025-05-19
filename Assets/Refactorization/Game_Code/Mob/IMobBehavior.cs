using UnityEngine;

public interface IMobBehavior
{
    void Init(DefaultMob mob); // Optional if different move patterns
    
    void ActionLoop(); //As Tick

    void InitMove(Vector3 destination, GameObject colliderObj);

    void OnClick();
}
