using UnityEngine;
using TMPro;
using DG.Tweening;
using static cbValue;

public class DamageTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    Sequence sequence;
    Camera cam;

    public void Setup(Vector3 worldPosition, float amount, bool isCrit, DamageType damageType)
    {
        if (cam == null)
            cam = Camera.main;

        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);

        // Nếu sau camera → bỏ luôn
        if (screenPos.z < 0)
        {
            DamageTextPool.Instance.Return(this);
            return;
        }

        transform.position = screenPos;

        text.text = Mathf.RoundToInt(amount).ToString();

        if (isCrit)
        {
            text.color = Color.yellow;
            text.fontSize *= 1.3f;
        }
        else
        {
            switch (damageType)
            {
                case DamageType.Physical:
                    text.color = Color.red;
                    break;
                case DamageType.Magical:
                    text.color = Color.blue;
                    break;
                case DamageType.True:
                    text.color = Color.white;
                    break;
            }
        }

        PlayAnimation(isCrit);
    }

    void PlayAnimation(bool isCrit)
    {
        sequence?.Kill();

        RectTransform rect = transform as RectTransform;

        text.alpha = 1f;
        rect.localScale = Vector3.one * 0.8f;

        sequence = DOTween.Sequence();

        // 🔥 POP SCALE
        sequence.Append(rect.DOScale(1.25f, 0.15f).SetEase(Ease.OutBack));
        sequence.Append(rect.DOScale(1f, 0.1f));

        // 🎯 Bay lên theo hướng camera UP
        Vector2 upward = Vector2.up * Random.Range(80f, 120f);
        Vector2 sideOffset = Vector2.right * Random.Range(-40f, 40f);

        Vector2 finalOffset = upward + sideOffset;

        sequence.Join(
            rect.DOAnchorPos(rect.anchoredPosition + finalOffset, 0.9f)
                .SetEase(Ease.OutCubic)
        );

        // 🔥 Fade out
        sequence.Join(text.DOFade(0f, 0.9f));

        if (isCrit)
        {
            sequence.Join(
                rect.DOShakeAnchorPos(0.25f, 25f, 15)
            );
        }

        sequence.OnComplete(() =>
        {
            DamageTextPool.Instance.Return(this);
        });
    }

    public void ResetState()
    {
        sequence?.Kill();
        text.alpha = 1f;
        transform.localScale = Vector3.one;
    }
}