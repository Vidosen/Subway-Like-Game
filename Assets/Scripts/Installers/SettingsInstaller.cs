using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
{
    public MobileInputSystem.Settings InputSettings;
    public Player.Settings PlayerSettings;
    public BlockManager.Settings RoadBlockManagerSettings;
    public override void InstallBindings()
    {
        Container.BindInstance(InputSettings);
        Container.BindInstance(PlayerSettings);
        Container.BindInstance(RoadBlockManagerSettings);
    }
    
    
}