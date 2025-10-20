using UnityEngine;

namespace DefaultNamespace
{
    public class Utils
    {
        # region SCENES
        public static readonly string BOOTSTRAP_SCENE = "Bootstrap_Scene";
        public static readonly string SPLASH_SCENE = "Splash_Scene";
        public static readonly string MAIN_MENU_SCENE = "MainMenu_Scene";
        public static readonly string PLAYGROUND_SCENE = "Playground_Scene";
        # endregion
        
        # region EDITOR SPECIFIC
        public const string AUTO_LOAD_BOOTSTRAP_SCENE_KEY = "Game/AutoLoadBootStrapScene";
        #endregion

        # region PLAYER_PREFS CONSTS
        public static readonly string PLAYER_NAME_PREF = "PlayerName";
        #endregion

        public static string PLAYER_NAME
        {
            get => PlayerPrefs.GetString(PLAYER_NAME_PREF, "Player");
            set => PlayerPrefs.SetString(PLAYER_NAME_PREF, value);
        } 
    }
}