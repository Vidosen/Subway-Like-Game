public interface IGameDataHandler
{
    GameData LoadGameData();
    void SaveGameData(GameData gameData);
}