using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 10f;

    protected override void Eat() {
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().PowerPelletEaten(this);
    }
}
