using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Title("Layout References (Direct Siblings in Content)")]
    [SerializeField] private LayoutElement _staminaLayout;
    [SerializeField] private LayoutElement _ghostLayout;
    [SerializeField] private LayoutElement _hitLayout;
    [SerializeField] private LayoutElement _poisonLayout;
    [SerializeField] private LayoutElement _hungerLayout;

    [Title("Animation Settings")]
    [SerializeField] private float _animDuration = 0.35f;
    [SerializeField] private float _ghostDelay = 0.15f;
    [SerializeField] private Ease _animEase = Ease.OutCubic;

    [Title("Current State (Read Only)")]
    [ShowInInspector, ReadOnly] private float _stamina = 100f;
    [ShowInInspector, ReadOnly] private float _ghost = 0f;
    [ShowInInspector, ReadOnly] private float _hit = 0f;
    [ShowInInspector, ReadOnly] private float _poison = 0f;
    [ShowInInspector, ReadOnly] private float _hunger = 0f;

    private Sequence _animSequence;

    private void Start()
    {
        ResetBar();
    }

    [Button("Reset / Full Health", ButtonSizes.Medium)]
    public void ResetBar()
    {
        _stamina = 100f;
        _ghost = 0f;
        _hit = 0f;
        _poison = 0f;
        _hunger = 0f;
        AnimateAllToCurrentState(instant: true);
    }

    [Button("Take Damage", ButtonSizes.Medium)]
    public void TakeDamage(float damage = 15f)
    {
        float actualDamage = Mathf.Min(damage, _stamina);
        _stamina -= actualDamage;
        _hit += actualDamage;
        _ghost += actualDamage; // Ghost absorbs lost stamina, then fades

        AnimateAllToCurrentState();
    }

    [Button("Apply Poison", ButtonSizes.Medium)]
    public void ApplyPoison(float amount = 10f)
    {
        _poison = Mathf.Clamp(_poison + amount, 0f, 100f - _hunger);
        ClampStaminaToAvailableSpace();
        AnimateAllToCurrentState();
    }

    [Button("Apply Hunger", ButtonSizes.Medium)]
    public void ApplyHunger(float amount = 10f)
    {
        _hunger = Mathf.Clamp(_hunger + amount, 0f, 100f - _poison);
        ClampStaminaToAvailableSpace();
        AnimateAllToCurrentState();
    }

    [Button("Use Stamina (Sprint)", ButtonSizes.Medium)]
    public void UseStamina(float amount = 20f)
    {
        float drain = Mathf.Min(amount, _stamina);
        _stamina -= drain;
        _ghost += drain;
        AnimateAllToCurrentState();
    }

    [Button("Update All Values Custom", ButtonSizes.Medium)]
    public void SetValues(float stamina, float hit, float poison, float hunger)
    {
        _stamina = Mathf.Max(0, stamina);
        _hit = Mathf.Max(0, hit);
        _poison = Mathf.Max(0, poison);
        _hunger = Mathf.Max(0, hunger);
        AnimateAllToCurrentState();
    }

    private void ClampStaminaToAvailableSpace()
    {
        float maxAvailable = Mathf.Max(0f, 100f - (_poison + _hunger + _hit));
        if (_stamina > maxAvailable)
        {
            _stamina = maxAvailable;
        }
    }

    private void AnimateAllToCurrentState(bool instant = false)
    {
        _animSequence?.Kill();

        if (instant)
        {
            SetLayoutFlexibleWidth(_staminaLayout, _stamina);
            SetLayoutFlexibleWidth(_ghostLayout, _ghost);
            SetLayoutFlexibleWidth(_hitLayout, _hit);
            SetLayoutFlexibleWidth(_poisonLayout, _poison);
            SetLayoutFlexibleWidth(_hungerLayout, _hunger);
            return;
        }

        _animSequence = DOTween.Sequence();

        // Animate main bars smoothly
        TweenFlexibleWidth(_animSequence, _staminaLayout, _stamina);
        TweenFlexibleWidth(_animSequence, _hitLayout, _hit);
        TweenFlexibleWidth(_animSequence, _poisonLayout, _poison);
        TweenFlexibleWidth(_animSequence, _hungerLayout, _hunger);

        // Ghost bar animates with a slight lag/delay
        _animSequence.Insert(_ghostDelay, DOVirtual.Float(
            _ghostLayout != null ? _ghostLayout.flexibleWidth : 0f,
            0f, // Ghost fades down to 0 over time
            _animDuration * 1.5f,
            val =>
            {
                _ghost = val;
                SetLayoutFlexibleWidth(_ghostLayout, val);
            }
        ).SetEase(_animEase));
    }

    private void TweenFlexibleWidth(Sequence seq, LayoutElement layout, float targetValue)
    {
        if (layout == null) return;
        seq.Join(DOVirtual.Float(layout.flexibleWidth, targetValue, _animDuration, val =>
        {
            SetLayoutFlexibleWidth(layout, val);
        }).SetEase(_animEase));
    }

    private void SetLayoutFlexibleWidth(LayoutElement layout, float val)
    {
        if (layout == null) return;
        layout.flexibleWidth = Mathf.Max(0f, val);
    }
}

