using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace RedDotSystem
{
    public class RedDotNode
    {
        public ERedDot RedDotType { get; }
        public RedDotNode Parent { get; set; }

        private bool _isOn;
        private Func<bool> _evaluateFunc;
        private Action<bool> _onValueChanged;
        private int _childStateBitFlags;
        private List<RedDotNode> _children = new List<RedDotNode>();

        public bool IsLeaf
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
            child.Parent = this;
            _children.Add(child);
        }

        public void RemoveChild(RedDotNode child)
        {
            _children.Remove(child);
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
        /// Evaluates RedDot's state
        /// [NOTE] : if RedDot is not a leaf node, it will evaluate every child nodes and update and this may cause performance issue
        /// </summary>
        /// <returns></returns>
        public bool Evaluate(bool topDown = false)
        {
            bool previousState = _isOn;

            if (IsLeaf)
            {
                _isOn = _evaluateFunc != null && _evaluateFunc.Invoke();
            }
            else
            {
                // 리프노드가 아닌 노드에 대해 호출되면, 자식 노드들을 모두 평가하고 자신의 상태를 업데이트

                UpdateChildStateBitFlags();
                _isOn = _childStateBitFlags != 0;
            }

            if (previousState != _isOn)
            {
                _onValueChanged?.Invoke(_isOn);
                if(!topDown)
                    Parent?.OnChildChanged(this);
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

        // If child RedDot state changed, this method is called and check bitflags
        public void OnChildChanged(RedDotNode child)
        {
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
                Parent?.OnChildChanged(this);
            }
        }
    }
}
