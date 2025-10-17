using UnityEngine;

public class SaveManager
{
    public static void Save(string levelId, int score, float time) {
        int bestScore;
        float bestTime;
        (bestScore, bestTime) = Load(levelId);
        if (score > bestScore || score == bestScore && time < bestTime) {
            PlayerPrefs.SetInt($"{levelId}_score", score);
            PlayerPrefs.SetFloat($"{levelId}_time", time);
            PlayerPrefs.Save();
        }
    }

    // Instructions specify this being public, but it has no need to be (see above). Likewise, this seemed most appropriate as a static function
    public static (int, float) Load(string levelId)
    {
        int score = PlayerPrefs.GetInt($"{levelId}_score");
        float time = PlayerPrefs.GetFloat($"{levelId}_time");
        return (score, time);
    }
}
