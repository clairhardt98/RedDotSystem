using RedDotSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestRedDot : MonoBehaviour
{
    [SerializeField] Image _reddotImage;

    [SerializeField] ERedDot _reddotType;
    [SerializeField] TextMeshProUGUI _titleTmp;

    Color _onColor = new Color(0, 255, 0, 255);
    Color _offColor = new Color(255, 0, 0, 255);

    RedDotNode _redDotNode;
    
    private void OnEnable()
    {
        _redDotNode = RedDotManager.Instance.GetRedDotNode(_reddotType);
        _titleTmp.text = _reddotType.ToString();

        _redDotNode.BindValueChangedEvent(Refresh, true);
    }

    public void OnClick()
    {
        _redDotNode.Evaluate();
    }

    void Refresh(bool isOn)
    {
        _reddotImage.color = isOn ? _onColor : _offColor;
    }
}
