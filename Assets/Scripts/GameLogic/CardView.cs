using System.Collections;
using GameData;
using TMPro;
using UnityEngine;

namespace GameLogic
{
    public class CardView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer shiny;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text descriptionText;
        [SerializeField]
        private TMP_Text manaText;
        [SerializeField]
        private TMP_Text attackText;
        [SerializeField]
        private TMP_Text healthText;
        [SerializeField]
        private SpriteRenderer image;

        private Coroutine _statAnimRoutine;

        public void UpdateView(CardModel model)
        {
            nameText.text = model.Name;
            descriptionText.text = model.Description;
            image.sprite = model.Sprite;

            manaText.text = model.Mana.ToString();
            attackText.text = model.Attack.ToString();
            healthText.text = model.Health.ToString();
        }

        public void AnimateValueUpdate(CardStatName statName, int from, int to)
        {
            if (_statAnimRoutine != null)
                StopCoroutine(_statAnimRoutine);

            TMP_Text textField;

            switch (statName)
            {
                case CardStatName.Mana:
                    textField = manaText;
                    break;
                
                case CardStatName.Attack:
                    textField = attackText;
                    break;
                
                case CardStatName.Health:
                    textField = healthText;
                    break;
                
                default:
                    return;
            }

            _statAnimRoutine = StartCoroutine(AnimateValue(textField, from, to));
        }

        private IEnumerator AnimateValue(TMP_Text textField, int from, int to)
        {
            var delta = to - from;
            
            if (delta == 0)
                yield break;

            delta = delta < 0 ? -1 : 1;
            
            var dt = .15f;
            var value = from;
            while (value != to)
            {
                textField.text = value.ToString();
                value += delta;
                
                yield return new WaitForSeconds(dt);
            }

            textField.text = to.ToString();
        }

        public void EnableShine(Color color)
        {
            shiny.color = color;
            shiny.gameObject.SetActive(true);
        }

        public void DisableShine()
        {
            shiny.gameObject.SetActive(false);
        }
    }
}