using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    // Relative chance of this powerup spawning, compared to the weights of the other powerups
    [SerializeField, Min(0f), Tooltip("The higher the value, the more likely this power-up is to spawn")]
    private float spawnWeight = 1f;

    public float SpawnWeight => spawnWeight;

    // Called when Red Man picks up the powerup. The powerup is responsible for destroying itself.
    public abstract void Activate(RedMan redMan);
}
