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
        }

        private void OnStartButtonClicked()
        {
            Debug.Log($"Starting game with player name: {nameInputField.text}");
            Utils.PLAYER_NAME = nameInputField.text;
            Game.GetService<NetworkService>().StartConnection(ipInputField.text, 7979);
            Game.GetService<SceneService>().LoadSceneAsync(Utils.PLAYGROUND_SCENE, false);
        }
    }
}