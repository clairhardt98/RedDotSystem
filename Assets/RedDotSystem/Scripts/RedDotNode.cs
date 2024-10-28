using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace RedDotSystem
{
    public class RedDotNode
    {
        public ERedDot RedDotType { get; }

        private bool _isOn;
        private Func<bool> _evaluateFunc;
        private Action<bool> _onValueChanged;
        private Action<RedDotNode> _onChildChanged;
        private int _childStateBitFlags;
        private List<RedDotNode> _children = new List<RedDotNode>();


        private bool _isLeaf
        {
            get
            {
                return _children == null || _children.Count == 0;
            }
        }

        public RedDotNode(ERedDot type)
        {
            this.RedDotType = type;
        }

        public void AddChild(RedDotNode child)
        {
            child.SubscribeChildChangedEvent(OnChildChanged);
            _children.Add(child);
        }
        public void RemoveChild(RedDotNode child)
        {
            child.UnsubscribeChildChangedEvent(OnChildChanged);
            _children.Remove(child);
        }

        public void SubscribeChildChangedEvent(Action<RedDotNode> childChangedEvent)
        {
            _onChildChanged -= childChangedEvent;
            _onChildChanged += childChangedEvent;
        }

        public void UnsubscribeChildChangedEvent(Action<RedDotNode> childChangedEvent)
        {
            _onChildChanged -= childChangedEvent;
        }

        public void BindEvaluateFunc(Func<bool> func)
        {
            _evaluateFunc = func;
        }

        public void BindValueChangedEvent(Action<bool> action, bool fireOnBind = false)
        {
            _onValueChanged -= action;
            _onValueChanged += action;

            if (fireOnBind)
                _onValueChanged.Invoke(_isOn);
        }

        /// <summary>
        /// 레드닷의 상태를 평가합니다.
        /// [NOTE] : 레드닷 노드가 리프 노드가 아니라면, 재귀적 실행에 의해 성능 문제가 발생할 수 있습니다.
        /// </summary>
        /// <returns></returns>
        public bool Evaluate(bool topDown = false)
        {
            bool previousState = _isOn;

            if (_isLeaf)
            {
                _isOn = _evaluateFunc != null && _evaluateFunc.Invoke();
            }
            else
            {
                // 리프노드가 아닌 노드에 대해 호출되면, 자식 노드들을 모두 평가하고 자신의 상태를 업데이트
                UnityEngine.Debug.LogWarning($"{RedDotType} is not a leaf node. This may cause performance issue.");
                UpdateChildStateBitFlags();
                _isOn = _childStateBitFlags != 0;
            }

            if (previousState != _isOn)
            {
                _onValueChanged?.Invoke(_isOn);
                if (!topDown)
                    _onChildChanged?.Invoke(this);
            }

            UnityEngine.Debug.Log("Evaluate " + RedDotType.ToString() + $"{_isOn}");

            return _isOn;
        }

        // Update child state bitflags
        private void UpdateChildStateBitFlags()
        {
            _childStateBitFlags = 0;

            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i].Evaluate(true) == true)
                {
                    _childStateBitFlags |= 1 << i;
                }
            }
        }
        /// <summary>
        /// If child RedDot state changed, this method is called and check bitflags.
        /// This is called only when RedDot is not a leaf node.
        /// </summary>
        /// <param name="child"></param>
        private void OnChildChanged(RedDotNode child)
        {
            Assert.IsTrue(!_isLeaf);

            int childIdx = _children.IndexOf(child);
            if (childIdx < 0) return;

            if (child._isOn)
            {
                _childStateBitFlags |= 1 << childIdx;
            }
            else
            {
                _childStateBitFlags &= ~(1 << childIdx);
            }

            bool previousState = _isOn;
            _isOn = _childStateBitFlags != 0;

            if (_isOn != previousState)
            {
                _onValueChanged?.Invoke(_isOn);
                _onChildChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// EvaluateFunc를 실행하지 않고 상태를 변경합니다.
        /// </summary>
        /// <param name="on"></param>
        public void Toggle(bool on)
        {
            if (on == _isOn) return;

            _isOn = on;

            _onValueChanged?.Invoke(_isOn);
            _onChildChanged?.Invoke(this);
        }
    }

    public class RedDotTree
    {
        // 단일 인스턴스
        private static RedDotTree _instance;

        private Dictionary<ERedDot, RedDotNode> _nodeMap;
        private RedDotNode _root;

        public RedDotNode Root => _root;

        // 트리 인스턴스는 하나 이상 생성할 수 없습니다.
        public static RedDotTree CreateTree()
        {
            Assert.IsNull(_instance, $"RedDotTree is already created.");

            _instance = new RedDotTree();
            return _instance;
        }

        private RedDotTree()
        {
            BuildTree();
        }

        private void BuildTree()
        {
            _nodeMap = new();

            RedDotNode rootNode = new RedDotNode(ERedDot.Root);

            _root = _nodeMap[ERedDot.Root] = rootNode;

            var redDots = Enum.GetValues(typeof(ERedDot));
            foreach (ERedDot redDotType in redDots)
            {
                if (redDotType == ERedDot.None || redDotType == ERedDot.End || redDotType == ERedDot.Root)
                    continue;

                RedDotNode node = new RedDotNode(redDotType);
                _nodeMap[redDotType] = node;

                ERedDot parentType = GetParentType(redDotType);
                if (parentType != ERedDot.None && _nodeMap.ContainsKey(parentType))
                {
                    _nodeMap[parentType].AddChild(node);
                }
                else
                {
                    rootNode.AddChild(node);
                }

                BindEvaluateFunc(node);
            }

            InitTree();
        }

        private void InitTree()
        {
            Assert.IsNotNull(_root);
            _root.Evaluate();
        }

        private ERedDot GetParentType(ERedDot redDotType)
        {
            string enumName = redDotType.ToString();
            int lastUnderscoreIndex = enumName.LastIndexOf('_');

            if (lastUnderscoreIndex > 0)
            {
                string parentName = enumName.Substring(0, lastUnderscoreIndex);
                if (Enum.TryParse(parentName, out ERedDot parentType))
                {
                    return parentType;
                }
            }

            return ERedDot.None;
        }

        private void BindEvaluateFunc(RedDotNode node)
        {
            string enumName = node.RedDotType.ToString();
            int lastUnderscoreIndex = enumName.LastIndexOf('_');

            string methodName = (lastUnderscoreIndex >= 0)
                ? enumName.Substring(lastUnderscoreIndex + 1)
                : enumName;

            var method = typeof(RedDotFunction).GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            if (method != null)
            {
                var evaluateFunc = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method);
                node.BindEvaluateFunc(evaluateFunc);
                UnityEngine.Debug.LogWarning($"Method {methodName} Bound Success.");
            }
        }

        public RedDotNode GetRedDotNode(ERedDot redDot)
        {
            if (_root == null || !_nodeMap.ContainsKey(redDot)) return null;

            return _nodeMap[redDot];
        }
    }
}
