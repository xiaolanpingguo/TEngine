using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;


namespace GameLogic
{
    public class GameInfoUI : MonoBehaviour
    {
        public static GameInfoUI Instance;
        public TextMeshProUGUI TextPing;
        public TextMeshProUGUI AuthorityTick;
        public TextMeshProUGUI PredictionTick;

        private void Start()
        {
            Instance = this;
        }

        private void Update()
        {
        
        }

        public void SetPing(int ping)
        {
            TextPing.text = $"Ping:{ping}";
        }

        public void SetAuthorityTick(int tick)
        {
            AuthorityTick.text = $"AuthorityTick:{tick}";
        }

        public void SetPredictionTick(int tick)
        {
            PredictionTick.text = $"PredictionTick:{tick}";
        }
    }
}
