using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ihaiu.Debugs
{
    public class DebugLogManager : MonoBehaviour
    {
		private static DebugLogManager _Instance;
		public static DebugLogManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					GameObject go = GameObject.Find("GameManagers");
					if(go == null) go = new GameObject("GameManagers");
					
					_Instance = go.GetComponent<DebugLogManager>();
					if(_Instance == null) _Instance = go.AddComponent<DebugLogManager>();
				}
				return _Instance;
			}
		}


        // public field
        // ------------
		
		public Queue allQueue;
        public Queue logQueue;
        public Queue warningQueue;
        public Queue errorQueue;
        public bool isShowLog = true;
        public bool isShowWarning = true;
        public bool isShowError = true;
		public bool isShowException = true;
        // private field
        // -------------
        private DebugLogVO vo;


        // public property
        // ---------------
        public bool IsShowLog
        {
            get
            {
                return isShowLog;
            }
            set
            {
                isShowLog = value;
            }
        }

        public bool IsShowWarning
        {
            get
            {
                return isShowWarning;
            }
            set
            {
                isShowWarning = value;
            }
        }

        public bool IsShowError
        {
            get
            {
                return isShowError;
            }
            set
            {
                isShowError = value;
            }
        }

        public bool IsShowException
        {
            get
            {
                return isShowException;
            }
            set
            {
                isShowException = value;
            }
        }

        // private method
        // --------------
        void Awake()
        {
			_Instance = this;


			allQueue = Queue.Synchronized(new Queue());
            logQueue = Queue.Synchronized(new Queue());
            warningQueue = Queue.Synchronized(new Queue());
            errorQueue = Queue.Synchronized(new Queue());

            vo = new DebugLogVO();
            vo.logString = "Register Log Callback !";
            vo.stackTrace = "";
            vo.logType = LogType.Log;
            logQueue.Enqueue(vo);
            Application.logMessageReceived          += CatchLogInfo;
            Application.logMessageReceivedThreaded  += CatchLogInfo;
        }

		private DebugLogVO preVO;
    
        private void CatchLogInfo(string logString, string stackTrace, LogType type)
        {
            if(preVO != null && preVO.logString == logString && preVO.stackTrace == stackTrace && preVO.logType == type)
			{
				return;
			}

            vo = new DebugLogVO();
            vo.logString = logString;
            vo.stackTrace = stackTrace;
            vo.logType = type;

			preVO = vo;

            switch (type)
            {
                case LogType.Log:
                    logQueue.Enqueue(vo);
                    break;
                case LogType.Warning:
                    warningQueue.Enqueue(vo);
                    break;
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    errorQueue.Enqueue(vo);
                    break;
                default:
                    break;
            }

			allQueue.Enqueue(vo);
        }

		
    }
}
