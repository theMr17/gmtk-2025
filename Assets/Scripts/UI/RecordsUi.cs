using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordsUi : MonoBehaviour
{
  [SerializeField] private List<Sprite> recordsSprites;
  [SerializeField] private Button nextButton;
  [SerializeField] private Image displayImage;

  private int currentIndex = 0;

  private void Start()
  {
    UpdateImage();

    nextButton.onClick.AddListener(() =>
    {
      if (currentIndex == recordsSprites.Count - 1)
      {
        currentIndex = 0;
        gameObject.SetActive(false);
      }
      currentIndex++;
      UpdateImage();
    });
  }

  private void UpdateImage()
  {
    if (recordsSprites.Count == 0) return;
    SoundManager.PlaySound(SoundType.PageTurn);
    displayImage.sprite = recordsSprites[currentIndex];
  }
}
