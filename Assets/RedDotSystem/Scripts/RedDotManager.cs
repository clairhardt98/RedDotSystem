using System;

namespace RedDotSystem
{
    public class RedDotManager
    {
        private readonly RedDotTree _redDotTree;
        private readonly RedDotTimer _redDotTimer;

        private static RedDotManager _instance;
        public static RedDotManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RedDotManager();
                }
                return _instance;
            }
        }
        private RedDotManager()
        {
            // Initiate RedDotTree
            _redDotTree = RedDotTree.CreateTree();

            // Initiate RedDotTimer
            _redDotTimer = RedDotTimer.CreateTimer();
            UnityEngine.Object.DontDestroyOnLoad(_redDotTimer);
        }

        public RedDotNode GetRedDotNode(ERedDot redDot)
        {
            if(_redDotTree != null)
                return _redDotTree.GetRedDotNode(redDot);

            return null;
        }

        public bool Evaluate(ERedDot redDot)
        {
            return GetRedDotNode(redDot).Evaluate();
        }

        //  ���� �ð� �� RedDot�� Evaluate �մϴ�. Timer�� ������ �� ȣ��Ǵ� Callback�� �Ҵ��� �� �ֽ��ϴ�.
        public void RegisterTimer(ERedDot redDot, float duration, Action onTimerEndCallback = null)
        {
            if(_redDotTimer != null)
                _redDotTimer.RegisterTimer(redDot, duration, onTimerEndCallback);
        }
    }
}

