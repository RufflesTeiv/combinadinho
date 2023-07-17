using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public static UiController Instance {get; private set;}

    [Header("References")]
    [SerializeField] CharacterAreaController leftCharacterArea;
    [SerializeField] CharacterAreaController rightCharacterArea;

    //Unity event functions
    private void Awake() {
        Instance = this;
    }

    // Public methods and properties
    public void InitDiscussion(Discussion discussion) {
        leftCharacterArea.SetProgressColor(discussion.colors[0]);
        rightCharacterArea.SetProgressColor(discussion.colors[1]);
        InitCharacters(discussion.characters[0], discussion.characters[1]);
    }
    public void InitCharacters(Character leftCharacter, Character rightCharacter) {
        leftCharacterArea.SetCharacter(leftCharacter);
        rightCharacterArea.SetCharacter(rightCharacter);
    }
    public void UpdateScore(int score1, int score2) {
        leftCharacterArea.UpdateScore(score1);
        rightCharacterArea.UpdateScore(score2);
    }
}
