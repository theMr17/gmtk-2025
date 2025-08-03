using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxSelectionUi : MonoBehaviour
{
  [SerializeField] private Transform boxButtonContainer;
  [SerializeField] private GameObject boxButtonPrefab;

  private void Start()
  {
    boxButtonContainer.gameObject.SetActive(false); // Hide the selection UI
    StorageSceneManager.Instance.OnBoxSelectionStarted += StorageSceneManager_OnBoxSelectionStarted;
  }

  private void StorageSceneManager_OnBoxSelectionStarted(object sender, StorageSceneManager.OnBoxSelectionStartedEventArgs e)
  {
    // Show the box selection UI
    ShowBoxSelection(e.excludedBoxIds, e.isBoxMoveSelection);
  }

  private void ShowBoxSelection(List<int> excludedBoxIds, bool isBoxMoveSelection)
  {
    boxButtonContainer.gameObject.SetActive(true);
    // Clear existing box buttons
    foreach (Transform child in boxButtonContainer)
    {
      Destroy(child.gameObject);
    }

    // Create box buttons for boxes 1 to 12 (i from 1 to 12)
    for (int i = 1; i <= 12; i++)
    {
      if (excludedBoxIds != null && excludedBoxIds.Contains(i))
        continue; // Skip excluded boxes

      GameObject boxButton = Instantiate(boxButtonPrefab, boxButtonContainer);

      // Set button text to "Box i"
      TextMeshProUGUI buttonText = boxButton.GetComponentInChildren<TextMeshProUGUI>();
      if (buttonText != null)
        buttonText.text = $"Box {i}";

      // Capture the correct box ID in the lambda
      int capturedBoxId = i;
      Button buttonComponent = boxButton.GetComponent<Button>();
      if (buttonComponent != null)
      {
        buttonComponent.onClick.AddListener(() =>
        {
          boxButtonContainer.gameObject.SetActive(false); // Hide the selection UI
          if (isBoxMoveSelection)
            StorageSceneManager.Instance.OnBoxMoveTargetSelectedFromUI(capturedBoxId);
          else
            StorageSceneManager.Instance.OnBoxSelectedFromUI(capturedBoxId);
        });
      }
    }
  }
}