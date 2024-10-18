using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace RedDotSystem
{
    public class RedDotNode
    {
        public ERedDot Type { get; }
        public bool IsOn { get; set; }
        public Func<bool> EvaluateFunc { get; set; }
        public RedDotNode Parent { get; set; }

        public Action OnValueChanged;
        private List<RedDotNode> children = new List<RedDotNode>();
        private int childStateBitFlags;

        public bool IsLeaf
        {
            get
            {
                return children == null || children.Count == 0;
            }
        }

        public RedDotNode(ERedDot Type)
        {
            this.Type = Type;
        }

        public void AddChild(RedDotNode child)
        {
            child.Parent = this;
            children.Add(child);
        }

        public void RemoveChild(RedDotNode child)
        {
            children.Remove(child);
        }

        public void BindEvaluateFunc(Func<bool> func)
        {
            EvaluateFunc = func;
        }

        /// <summary>
        /// Evaluates RedDot's state
        /// [NOTE] : if RedDot is not a leaf node, it will evaluate every child nodes and update and this may cause performance issue
        /// </summary>
        /// <returns></returns>
        public bool Evaluate(bool topDown = false)
        {
            bool previousState = IsOn;

            if (IsLeaf)
            {
                IsOn = EvaluateFunc != null && EvaluateFunc.Invoke();
            }
            else
            {
                // 리프노드가 아닌 노드에 대해 호출되면, 자식 노드들을 모두 평가하고 자신의 상태를 업데이트

                UpdateChildStateBitFlags();
                IsOn = childStateBitFlags != 0;
            }

            if (previousState != IsOn)
            {
                OnValueChanged?.Invoke();
                if(!topDown)
                    Parent?.OnChildChanged(this);
            }

            UnityEngine.Debug.Log("Evaluate " + Type.ToString() + $"{IsOn}");

            return IsOn;
        }

        // Update child state bitflags
        private void UpdateChildStateBitFlags()
        {
            childStateBitFlags = 0;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].Evaluate(true) == true)
                {
                    childStateBitFlags |= 1 << i;
                }
            }
        }

        // If child RedDot state changed, this method is called and check bitflags
        public void OnChildChanged(RedDotNode child)
        {
            int childIdx = children.IndexOf(child);
            if (childIdx < 0) return;

            if (child.IsOn)
            {
                childStateBitFlags |= 1 << childIdx;
            }
            else
            {
                childStateBitFlags &= ~(1 << childIdx);
            }

            bool previousState = IsOn;
            IsOn = childStateBitFlags != 0;

            if (IsOn != previousState)
            {
                OnValueChanged?.Invoke();
                Parent?.OnChildChanged(this);
            }
        }
    }
}
