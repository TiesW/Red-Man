// Gives bonus points
public class DiamondPowerUp : PowerUpBase
{
    private const int Points = 10;

    public override void Activate(RedMan redMan)
    {
        Destroy(gameObject);
        redMan.AddScore(Points);
        redMan.scorePopup.Show("+" + Points + " score");
        redMan.diamondSound.Play();
    }
}
