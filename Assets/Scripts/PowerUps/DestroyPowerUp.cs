using UnityEngine;

// Explodes all moving objects on screen
public class DestroyPowerUp : PowerUpBase
{
    public override void Activate(RedMan redMan)
    {
        Destroy(gameObject);

        var objects = redMan.GetMovingObjects();
        if (objects.Count == 0)
        {
            return;
        }

        foreach (var obj in objects)
        {
            Instantiate(redMan.particles, obj.position, Quaternion.identity);
            Destroy(obj.gameObject);
        }
        objects.Clear();
        redMan.explodeSound.Play();
    }
}
