using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEmotion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer emotionSprite;
    [SerializeField] private GameObject emotionBox;
    [SerializeField] private List<Sprite> emotionSpriteList;
    public void PlayEmotion(Emotion type)
    {
        StartCoroutine(IEPlayEmotion(type));
    }
    public void StopEmotion()
    {
        StopAllCoroutines();
        emotionBox.SetActive(false);
    }
    private IEnumerator IERotationFollowCam()
    {
        while (emotionBox.activeSelf)
        {
            emotionBox.transform.rotation = Camera.main.transform.rotation;
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator IEPlayEmotion(Emotion type)
    {
        if (type == Emotion.ANY)
        {
            int emoIndex;
            do
            {
                emoIndex = Random.Range(0, 14);
            }
            while (emoIndex == 10);
            emotionSprite.sprite = emotionSpriteList[emoIndex];
        }
        else if (type == Emotion.SCARY)
        {
            emotionSprite.sprite = emotionSpriteList[10];
        }
        emotionBox.SetActive(true);
        StartCoroutine(IERotationFollowCam());
        yield return new WaitForSeconds(3f);
        emotionBox.SetActive(false);
    }
}
public enum Emotion
{
    ANY,
    SCARY
}