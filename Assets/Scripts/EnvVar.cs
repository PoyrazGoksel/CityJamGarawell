using UnityEngine;

public static class EnvVar
{
    public const string SettingsPath = "Settings/";
    public static readonly string PlayerSavePath = Application.persistentDataPath + "/Player.sav";
    public const string LoginSceneName = "Login";
    public const string MainSceneName = "Main";
}