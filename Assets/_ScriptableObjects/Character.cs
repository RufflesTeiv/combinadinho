using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterExpression {
    Neutral,
    Angry,
    AngrySpeaking,
    Happy
}

[CreateAssetMenu(fileName = "Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [Header("Text")]
    public new string name;
    [TextArea] public string description;
    [Header("Character sprites")]
    public Sprite neutralSprite;
    public Sprite angrySprite;
    public Sprite angrySpeakingSprite;
    public Sprite happySprite;
    public bool spriteFacingLeft = true;
    [Header("Progress bar sprites")]
    public Sprite emptyBarSprite;
    public Sprite filledBarSprite;

    private Dictionary<CharacterExpression, Sprite> expressionDictionary = new Dictionary<CharacterExpression, Sprite>();

    
    // Public methods and properties
    public void Initialize() {
        expressionDictionary.Add(CharacterExpression.Neutral, neutralSprite);
        expressionDictionary.Add(CharacterExpression.Angry, angrySprite);
        expressionDictionary.Add(CharacterExpression.AngrySpeaking, angrySpeakingSprite);
        expressionDictionary.Add(CharacterExpression.Happy, happySprite);
    }
    public Sprite GetExpressionSprite(CharacterExpression expression) => expressionDictionary[expression];
}
