using Unity.Collections;
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
        public static readonly string LOADING_SCENE = "Loading_Scene";

        # endregion

        # region EDITOR SPECIFIC

        public const string AUTO_LOAD_BOOTSTRAP_SCENE_KEY = "Game/AutoLoadBootStrapScene";

        #endregion

        # region PLAYER_PREFS CONSTS

        public static readonly string PLAYER_NAME_PREF = "PlayerName";
        public static readonly string IP_PREF = "IP";

        #endregion

        public static FixedString64Bytes PLAYER_NAME
        {
            get
            {
                string playerName = PlayerPrefs.GetString(PLAYER_NAME_PREF, "Player");
                FixedString64Bytes name = new FixedString64Bytes(playerName);
                return name;
            }
            set => PlayerPrefs.SetString(PLAYER_NAME_PREF, value.ToString());
        }

        public static FixedString64Bytes IP
        {
            get
            {
                string ip = PlayerPrefs.GetString(IP_PREF, "127.0.0.1");
                FixedString64Bytes ipFixed = new FixedString64Bytes(ip);
                return ipFixed;
            }
            set => PlayerPrefs.SetString(IP_PREF, value.ToString());
        }
    }
}