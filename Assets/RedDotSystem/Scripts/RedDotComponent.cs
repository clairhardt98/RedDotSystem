using UnityEngine;
using RedDotSystem;


/// <summary>
/// RedDotComponent that UI object can have
/// This component can be expanded for common use
/// </summary>
public class RedDotComponent : MonoBehaviour
{
    [SerializeField] private ERedDot _redDotType;
    [SerializeField] private GameObject _redDotImageObj;

    private RedDotNode _redDotNode;

    // Init should be called after the User's data load is done
    public void Init()
    {
        _redDotNode = RedDotManager.Instance.GetRedDotNode(_redDotType);
        _redDotNode.BindValueChangedEvent(OnRedDotValueChanged);
    }

    private void OnRedDotValueChanged(bool isOn)
    {
        _redDotImageObj.SetActive(isOn);
    }
}
