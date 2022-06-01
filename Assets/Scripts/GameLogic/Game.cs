using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameLogic
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private PlayerHand playerHand;
        [SerializeField]
        private GameObject spritesIsLoading;
        [SerializeField]
        private Button changeCardsValuesButton;
        [SerializeField]
        private Button getNewCardsButton;

        private System.Random _rnd = new System.Random();
        private Coroutine _setupCardsRoutine;
        
        private void Start()
        {
            SetupCards();

            changeCardsValuesButton.onClick.RemoveAllListeners();
            changeCardsValuesButton.onClick.AddListener(playerHand.ChangeCardsValues);
            
            getNewCardsButton.onClick.RemoveAllListeners();
            getNewCardsButton.onClick.AddListener(SetupCards);
        }

        private void SetupCards()
        {
            if (_setupCardsRoutine != null)
                StopCoroutine(_setupCardsRoutine);

            _setupCardsRoutine = StartCoroutine(SetupNewCards());
        }

        private IEnumerator SetupNewCards()
        {
            spritesIsLoading.SetActive(true);

            var getCards = GetCardsForPlayer();
            
            while (getCards.MoveNext())
                yield return null;

            var cards = getCards.Current;

            spritesIsLoading.SetActive(false);
            
            playerHand.Setup(cards);
        }

        private IEnumerator<CardModel[]> GetCardsForPlayer()
        {
            var minCardsToGenerate = 4;
            var maxCardsToGenerate = 6;
            
            var amount = _rnd.Next(minCardsToGenerate, maxCardsToGenerate + 1);
            var cards = new CardModel[amount];

            for (int i = 0; i < amount; i++)
            {
                var loadRoutine = SpritesLoader.LoadSprite();

                while (loadRoutine.MoveNext())
                    yield return null;

                cards[i] = new CardModel(
                    $"Card_{i}",
                    $"Description_{i}\nSome extra text and abilities",
                    i + 1,
                    i + 2,
                    i + 2,
                    loadRoutine.Current.Item1,
                    loadRoutine.Current.Item2);
            }

            yield return cards;
        }
    }
}