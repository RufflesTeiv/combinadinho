using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAreaController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool rightSide = false;
    [Header("General references")]
    [SerializeField] private RectTransform characterPortraitMaskRectTransform;
    [SerializeField] private RectTransform characterRectTransform;
    [SerializeField] private RectTransform panelMaskRectTransform;
    [Header("Progress bar")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image progressBarBackground;
    [SerializeField] private Image progressBarFill;

    private Discussion discussion;
    private Character character;
    private Image characterImage;


    // Unity event functions
    private void Awake() {
        // Get components
        characterImage = characterRectTransform.GetComponent<Image>();
    }
    private void Start() {
        AdjustBorders();
    }

    // Public functions and properties
    public void SetProgressColor(Color color) => progressBarFill.color = color;
    public void SetCharacter(Character newCharacter) {
        character = newCharacter;

        // Invert sprite
        bool invertCharacter = (character.spriteFacingLeft && !rightSide) || (!character.spriteFacingLeft && rightSide);
        characterRectTransform.localScale = new Vector3(invertCharacter ? -1 : 1, 1, 1);

        // Set sprites
        SetCharacterExpression(CharacterExpression.Neutral);
        progressBarBackground.sprite = character.emptyBarSprite;
        progressBarFill.sprite = character.filledBarSprite;
    }
    public void SetCharacterExpression(CharacterExpression expression) {
        characterImage.sprite = character.GetExpressionSprite(expression);
    }
    public void UpdateScore(int score) {
        progressBar.value = Mathf.Clamp01((float)score/(float)GameController.Instance.GetScoreGoal());
        Debug.Log(progressBar.value);
    }

    // Private functions and properties
    private void AdjustBorders() {
        float characterPortraitFarBorder = characterPortraitMaskRectTransform.offsetMin.x;
        float characterPortraitNearBorder = characterPortraitMaskRectTransform.offsetMax.x;
        float panelFarBorder = panelMaskRectTransform.offsetMin.x;
        float panelNearBorder = panelMaskRectTransform.offsetMax.x;

        if (rightSide) {
            characterPortraitMaskRectTransform.offsetMin = new Vector2(-characterPortraitNearBorder, characterPortraitMaskRectTransform.offsetMin.y);
            characterPortraitMaskRectTransform.offsetMax = new Vector2(-characterPortraitFarBorder, characterPortraitMaskRectTransform.offsetMax.y);
            panelMaskRectTransform.offsetMin = new Vector2(-panelNearBorder, panelMaskRectTransform.offsetMin.y);
            panelMaskRectTransform.offsetMax = new Vector2(-panelFarBorder, panelMaskRectTransform.offsetMax.y);
        }
    }
}
