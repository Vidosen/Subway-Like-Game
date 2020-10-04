using UnityEngine;

public class PlayerPrefsGameDataHandler : IGameDataHandler
{
    private const string BEST_SCORE_KEY = "bestScore";
    private const string TOTAL_PICKUP_COUNT = "totalCount";

    public GameData LoadGameData()
    {
        float bestScore = PlayerPrefs.GetFloat(BEST_SCORE_KEY, 0);
        int totalCount = PlayerPrefs.GetInt(TOTAL_PICKUP_COUNT, 0);
        return new GameData(totalCount, bestScore);
    }

    public void SaveGameData(GameData gameData)
    {
        PlayerPrefs.SetFloat(BEST_SCORE_KEY, gameData.BestScore.Value);
        PlayerPrefs.SetInt(TOTAL_PICKUP_COUNT, gameData.TotalPickUpCount.Value);
    }
}