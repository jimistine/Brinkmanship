using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Yarn.Unity
{
    public class Br_CharacterStyleView : Yarn.Unity.DialogueViewBase
    {
        [Serializable]
        public class CharacterStyleData
        {
            public string characterName;
            public TMP_FontAsset characterFont;
            public Color characterColor = Color.white;
        }
        [SerializeField] Color defaultColor = Color.white;
        [SerializeField] CharacterStyleData[] styleData;
        [SerializeField] List<TMPro.TextMeshProUGUI> lineTexts = new List<TMPro.TextMeshProUGUI>();

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            var characterName = dialogueLine.CharacterName;

            Color colorToUse = defaultColor;

            if (string.IsNullOrEmpty(characterName) == false) {
                foreach (var color in styleData) {
                    if (color.characterName.Equals(characterName, StringComparison.InvariantCultureIgnoreCase)) {
                        colorToUse = color.characterColor;
                        break;
                    }
                }
            }

            foreach (var text in lineTexts) {
                text.color = colorToUse;
            }

            onDialogueLineFinished();
        }

        public CharacterStyleData GetCharacterStyle(string characterName){
            //CharacterStyleData styleToUse = defaultColor;
            foreach (var style in styleData) {
                if (style.characterName.Equals(characterName, StringComparison.InvariantCultureIgnoreCase)) {
                    
                    return style;
                }
            }
            return null;    
        }
    }
}