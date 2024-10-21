using UnityEngine;
using RedDotSystem;
using UnityEngine.Assertions;


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
        _redDotNode?.BindValueChangedEvent(OnRedDotValueChanged);
    }

    private void OnRedDotValueChanged(bool isOn)
    {
        if(_redDotImageObj != null)
            _redDotImageObj.SetActive(isOn);
    }

    // Only used for None RedDotType.
    public void Toggle(bool on)
    {
        Assert.IsTrue(_redDotType == ERedDot.None, "Only None RedDotType can use Toggle");
        if(_redDotImageObj != null)
            _redDotImageObj.SetActive(on);
    }

    // Register RedDot to Timer that will be call Evaluate after time
    public void RegisterTimer(float time)
    {
        RedDotManager.Instance.RegisterTimer(_redDotType, time);
    }
}
