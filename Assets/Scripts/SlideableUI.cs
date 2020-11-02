using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlideableUI : MonoBehaviour, ISetupable
{
    [SerializeField] float _disableX = 0f, _activeX = 0f;
    [SerializeField] float _slideDuration = 0.5f;
    RectTransform _rectTransform;
    Coroutine _coroutine;
    public virtual void Initalize() {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void Toggle() {
       if (!gameObject.activeSelf)
            SlideOut();
        else
            SlideIn();
    }

    public void SlideOut() {
        gameObject.SetActive(true);
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ExecuteSlide(_rectTransform, _disableX, _activeX, true));
    }

    public void SlideIn() {
        if (gameObject.activeSelf)
            _coroutine = StartCoroutine(ExecuteSlide(_rectTransform, _disableX, _activeX, false));
    }

    IEnumerator ExecuteSlide(RectTransform target, float disableX, float activeX, bool isSlideOut) {
        float timeAgo = 0f;
        while (timeAgo <= 1f) {
            timeAgo += Time.deltaTime / _slideDuration;

            float startX = isSlideOut ? disableX : activeX;
            float endX = isSlideOut ? activeX : disableX;
            float x = Mathf.Lerp(startX, endX, timeAgo);

            Vector2 nextPosition = target.anchoredPosition;
            nextPosition.x = x;
            target.anchoredPosition = nextPosition;

            yield return null;
        }

        if (!isSlideOut)
            gameObject.SetActive(false);
    }
}
