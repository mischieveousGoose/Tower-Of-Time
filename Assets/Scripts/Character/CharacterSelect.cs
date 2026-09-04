using UnityEngine;
using System.Collections.Generic;
using TMPro;

using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public List<Character> characters;
    Character selectedCharacter;

    [Header("UI")]
    public GameObject buttonPrefab;
    public GameObject buttonContainer;
    List<Image> characterBGs = new List<Image>();

    public TMP_Text characterNameText;
    public TMP_Text characterDescriptionText;
    public Image characterImage;

    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;

    void Start()
    {
        selectedCharacter = characters[0];
        InitializeUI();
    }
    void InitializeUI()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            Character character = characters[i];
            int characterIndex = i;
            GameObject button = Instantiate(buttonPrefab, buttonContainer.transform);
            button.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(characterIndex));
            button.transform.GetChild(0).GetComponent<Image>().sprite = character.sprite;
            characterBGs.Add(button.GetComponent<Image>());
        }

        UpdateUI();
    }

    public void SelectCharacter(int index)
    {
        if (index >= 0 && index < characters.Count)
        {
            selectedCharacter = characters[index];
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        characterNameText.text = selectedCharacter.characterName;
        characterDescriptionText.text = selectedCharacter.description;
        characterImage.sprite = selectedCharacter.sprite;

        for (int i = 0; i < characterBGs.Count; i++)
        {
            if (i == characters.IndexOf(selectedCharacter))
            {
                characterBGs[i].color = selectedColor;
            }
            else
            {
                characterBGs[i].color = unselectedColor;
            }
        }
    }

    public void Continue()
    {
	GameManager.Instance.ChooseCharacter(selectedCharacter);


    }

}
