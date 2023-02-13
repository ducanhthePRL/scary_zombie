using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadReaction : MonoBehaviour
{
    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
    }
    [SerializeField] private List<RectTransform> textList;
    public void PlayDeadRect(EnemyDeadType deadtype)
    {
        int value = 0;
        if (deadtype == EnemyDeadType.None)
        {
            value = Random.Range(0, 3);
        }
        else
        {
            if (deadtype == EnemyDeadType.FALL)
            {
                value = 0;
            }
            else if (deadtype == EnemyDeadType.FIRE)
            {
                value = 1;
            }
        }
        if (value == 0)
        {
            TPRLSoundManager.Instance.PlaySoundFx("ah");
            animator.Play("right-move");
        }
        else if (value == 2)
        {
            TPRLSoundManager.Instance.PlaySoundFx("ouchs");
            animator.Play("middle-zoom");
        }
        else if(value == 1)
        {
            TPRLSoundManager.Instance.PlaySoundFx("no");
            animator.Play("left-move");
        }
        textList[value].gameObject.SetActive(true);
        StartCoroutine(IERotationFollowCam(textList[value]));
    }
    private IEnumerator IERotationFollowCam(RectTransform textTransform)
    {
        while (textTransform.gameObject.activeSelf)
        {
            textTransform.rotation = Camera.main.transform.rotation;
            yield return new WaitForEndOfFrame();
        }
    }
}
