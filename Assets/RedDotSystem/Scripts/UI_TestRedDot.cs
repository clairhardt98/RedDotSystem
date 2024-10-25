using RedDotSystem;
using System.Collections;
using TMPro;
using UnityEngine;


// UI 테스트용 스크립트
public class UI_TestRedDot : MonoBehaviour
{
    [SerializeField] private RedDotComponent _redDotComp;
    [SerializeField] private TextMeshProUGUI _timerTxt;
    [SerializeField] private TextMeshProUGUI _nameTxt;

    Coroutine _drawTimeCoroutine;
    private bool _isOn = false;

    private void Awake()
    {
        if(_redDotComp == null)
            _redDotComp = GetComponent<RedDotComponent>();

        _nameTxt.text = _redDotComp.RedDotType.ToString();
    }

    public void OnToggleClick()
    {
        _redDotComp.Toggle(_isOn);
        _isOn = !_isOn;
    }

    public void RegisterTimer(float value)
    {
        _redDotComp.RegisterTimer(value, () =>
        {
            if(gameObject.activeSelf)
            {
                _timerTxt.text = "Timer End";
            }
        });

        if (_drawTimeCoroutine != null)
            StopCoroutine(_drawTimeCoroutine);

        _drawTimeCoroutine = StartCoroutine(DrawRemainTimeTxt(value));
    }

    IEnumerator DrawRemainTimeTxt(float startTime)
    {
        float endTime = Time.time + startTime;
        while(Time.time < endTime)
        {
            float remainingTime = endTime - Time.time;
            _timerTxt.text = remainingTime.ToString("F2");
            yield return null;
        }
    }

}
