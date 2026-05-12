using UnityEngine;

public class HurtBoxToggles : MonoBehaviour
{
    //helper class to assist weapon calls to turn on and off collission zones
    private CapsuleCollider hurtBox;
    
    void Start()
    {
        hurtBox = GetComponent<CapsuleCollider>();
    }

    public void ToggleHurtBoxOn()
    {
        hurtBox.enabled = true;
    }
    public void ToggleHurtBoxOff()
    {
        hurtBox.enabled = false;
    }
}
