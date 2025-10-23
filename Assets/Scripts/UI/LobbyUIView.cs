using System.Collections;
using Core;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyUIView : UIView
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private TMP_Dropdown roleDropdown;
        
        private void Start()
        {
            startButton.interactable = false;
            startButton.onClick.AddListener(OnStartButtonClicked);
            nameInputField.onValueChanged.AddListener(text =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    errorText.text = "Name input field cannot be empty.";
                    startButton.interactable = false;
                }
                else
                {
                    errorText.text = string.Empty;
                    startButton.interactable = true;
                }
            });
            ipInputField.onValueChanged.AddListener(text =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    errorText.text = "IP input field cannot be empty.";
                }
                else
                {
                    errorText.text = string.Empty;
                    startButton.interactable = true;
                }
            });
            nameInputField.text = Utils.PLAYER_NAME.ToString();
            ipInputField.text = Utils.IP.ToString();
        }

        private void OnStartButtonClicked()
        {
            Debug.Log($"Starting game with player name: {nameInputField.text}");
            Utils.PLAYER_NAME = nameInputField.text;
            var role = (Game.Role)roleDropdown.value;
            Game.Instance.CreateWorlds(role);
            StartCoroutine(StartConnectionAndLoadScene());
        }

        private IEnumerator StartConnectionAndLoadScene()
        {
            yield return new WaitUntil(() => Game.Instance.ClientWorld.IsCreated);
            Game.GetService<NetworkService>().StartConnection(ipInputField.text, 7979);
            Game.GetService<SceneService>().LoadSceneAsync(Utils.PLAYGROUND_SCENE, false);
        }
    }
}