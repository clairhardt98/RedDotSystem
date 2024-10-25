using UnityEngine;
using System;

namespace RedDotSystem
{

    /// <summary>
    /// UI ������Ʈ�� ���� �� �ִ� ����� ������Ʈ �Դϴ�. ������ �ν����Ϳ��� ����� Ÿ���� �����ؾ� �մϴ�.
    /// </summary>
    public class RedDotComponent : MonoBehaviour
    {
        [SerializeField] private ERedDot _redDotType;
        [SerializeField] private GameObject _redDotImageObj;

        private RedDotNode _redDotNode;

        public ERedDot RedDotType => _redDotType;

        // !TODO : Init should be called after the User's data load is done
        private void Start()
        {
            Init();
        }
        public void Init()
        {
            _redDotNode = RedDotManager.Instance.GetRedDotNode(_redDotType);
            _redDotNode?.BindValueChangedEvent(OnRedDotValueChanged, true);
        }

        private void OnRedDotValueChanged(bool isOn)
        {
            if (_redDotImageObj != null)
                _redDotImageObj.SetActive(isOn);
        }

        // Register RedDot to Timer that will be call Evaluate after time
        public void RegisterTimer(float time, Action onEnd = null)
        {
            RedDotManager.Instance.RegisterTimer(_redDotType, time, onEnd);
        }

        // ������� Evaluate���� �ʰ� ����մϴ�.
        public void Toggle(bool on)
        {
            _redDotNode?.Toggle(on);
        }
    }
}
