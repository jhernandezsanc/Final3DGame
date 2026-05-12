using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    // This script is attached to the player's attack range colliders (short and long range).
    public enum AttackRangeType
    {
        ShortRange,
        LongRange
    }

    //set up in inspector to be either short or long range for each collider and be able to interact with other scripts
    public AttackRangeType rangeType;
}