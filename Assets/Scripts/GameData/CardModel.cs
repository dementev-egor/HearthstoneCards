using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace GameData
{
    public class CardModel
    {
        public string Name { get; }
        public string Description { get; }
        public Sprite Sprite { get; }
        public int Mana => _statsByName[CardStatName.Mana];
        public int Attack => _statsByName[CardStatName.Attack];
        public int Health => _statsByName[CardStatName.Health];
        
        private Dictionary<CardStatName, int> _statsByName = new Dictionary<CardStatName, int>();
        private Action _onDestroy;
        
        public int this[CardStatName statName] => _statsByName[statName];
        
        public CardModel(string name, string description, int mana, int attack, int health, Sprite sprite, Action onDestroy)
        {
            Name = name;
            Description = description;
            Sprite = sprite;
            
            _onDestroy = onDestroy;
            _statsByName[CardStatName.Mana] = mana;
            _statsByName[CardStatName.Attack] = attack;
            _statsByName[CardStatName.Health] = health;
        }

        public void UpdateStatValue(CardStatName statName, int value)
        {
            var statValue = _statsByName[statName];
            statValue += value;

            if (statName.IsOneOf(CardStatName.Mana, CardStatName.Attack))
            {
                if (statValue < 0)
                    statValue = 0;
            }

            _statsByName[statName] = statValue;
        }

        public void OnDestroy()
        {
            _onDestroy?.Invoke();
        }
    }
}