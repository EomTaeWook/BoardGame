using Dignus.Log;
using Dignus.Log.LogFormat;
using Dignus.Log.LogTarget.Interfaces;
using Dignus.Log.Model;
using UnityEngine;

namespace Assets.Scripts.Internals.Log
{
    public class UnityLogTarget : ILogTarget
    {
        private readonly LogFormatRenderer _renderer;
        public UnityLogTarget()
        {
            _renderer = new LogFormatRenderer();
            _renderer.SetLogFormat("${datetime} | ${message} | ${callerFileName} : ${callerLineNumber}");
        }
        public void Dispose()
        {

        }

        public void Write(LogMessageModel logMessage)
        {
            if (logMessage.LogLevel == LogLevel.Fatal)
            {
                return;
            }
#if UNITY_EDITOR

            if (logMessage.LogLevel == LogLevel.Debug)
            {
                Debug.Log(_renderer.GetRenderString(logMessage));
            }
            else if (logMessage.LogLevel == LogLevel.Info)
            {
                Debug.LogWarning(_renderer.GetRenderString(logMessage));
            }
#endif
            if (logMessage.LogLevel == LogLevel.Error)
            {
                Debug.LogError(_renderer.GetRenderString(logMessage));
            }
        }
    }
}
