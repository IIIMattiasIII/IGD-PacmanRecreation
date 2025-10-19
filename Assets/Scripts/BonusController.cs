using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AKA: CherryController
/// </summary>
public class BonusController : MonoBehaviour
{
    [SerializeField] private GameObject bonusChest;
    private Vector2 border;
    private Tweener tweener;
    private LevelManager levelManager;

    public void Start() {
        GameObject g = GameObject.FindWithTag("LevelManager");
        tweener = g.GetComponent<Tweener>();
        levelManager = g.GetComponent<LevelManager>();
        border = new((levelManager.levelSize.x+2)/2, (levelManager.levelSize.y+2)/2);
    }

    public async void BonusLoop() {
        while (true) {
            await Task.Delay(5000);
            if (this == null) { return; }
            (float xPos, float yPos) = RandPos();
            GameObject chestGO = Instantiate(bonusChest, new Vector3(xPos, yPos, -7), Quaternion.identity);
            BonusChest chest = chestGO.GetComponent<BonusChest>();
            chest.levelManager = levelManager;
            chest.tweener = tweener;
            await chest.life.Task;
        }
    }

    private (float, float) RandPos() {
        bool xy = Random.Range(0,2) == 0;
        float xPos, yPos;
        if (xy) {
            yPos = Random.Range(-border.y, border.y) - 0.5f;;
            xPos = border.x * (Random.Range(0,2)*2-1);
        } else {
            xPos = Random.Range(-border.x, border.x);
            yPos = (border.y-0.5f) * (Random.Range(0,2)*2-1);
        }
        return (xPos, yPos);
    }
}
