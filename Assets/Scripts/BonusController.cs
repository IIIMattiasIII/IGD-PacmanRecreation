using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AKA: CherryController
/// </summary>
public class BonusController : MonoBehaviour
{
    [SerializeField] private GameObject bonusChest;
    [SerializeField] private Vector2 levelSize = new(15, 14);

    public async void BonusLoop() {
        while (true) { // Update later to !isGamePaused
            await Task.Delay(5000);
            float yPos = Random.Range(-levelSize.y, levelSize.y);
            float xPos = levelSize.x * (Random.Range(0,2)*2-1);
            GameObject chest = Instantiate(bonusChest, new Vector3(xPos, yPos, -7), Quaternion.identity);
            BonusChest chestScript = chest.GetComponent<BonusChest>();
            await chestScript.life.Task;
        }
    }
}
