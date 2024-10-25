using UnityEngine;
using System;

namespace RedDotSystem
{

    /// <summary>
    /// UI 오브젝트가 가질 수 있는 레드닷 컴포넌트 입니다. 프리팹 인스펙터에서 레드닷 타입을 설정해야 합니다.
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

        // 레드닷을 Evaluate하지 않고 토글합니다.
        public void Toggle(bool on)
        {
            _redDotNode?.Toggle(on);
        }
    }
}
