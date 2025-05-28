using Assets.Scripts.Extensions;
using Assets.Scripts.Internals.Log;
using Dignus.Log;
using Dignus.Log.LogFormat;
using Dignus.Log.LogTarget;
using Dignus.Log.Model;
using Dignus.Log.Rule;
using Dignus.Unity;
using Dignus.Unity.Coroutine;
using Dignus.Unity.DependencyInjection;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Internals
{
    public class ApplicationManager : SingletonMonoBehaviour<ApplicationManager>
    {
        public int TargetWidth { get; private set; } = 768;
        public int TargetHeight { get; private set; } = 1280;

        public int DeviceWidth { get; set; }
        public int DeviceHeight { get; set; }

        private string _logPath;
        private string _archiveLogPath;
        private BuildTaretType _buildTargetType;
        private readonly StringBuilder _sb = new();
        private bool _isInit = false;

        protected override void OnAwake()
        {
            _logPath = Path.Combine(Application.persistentDataPath, "UnityLogFile.txt");
            _archiveLogPath = Path.Combine(Application.persistentDataPath, "archive", "UnityLogFile.{#}.txt");
        }
        public void Init(BuildTaretType buildTargetType)
        {
            if (_isInit == false)
            {
                _isInit = true;
                _buildTargetType = buildTargetType;
                var conatiner = DignusUnityServiceContainer.RegisterDependencies(GetType().Assembly);

                Application.targetFrameRate = 60;
                Input.multiTouchEnabled = false;
                Application.runInBackground = true;
                InitResolution();
                InitLog();
                conatiner.Build();

                DignusUnitySceneManager.Instance.OnSceneLoadCompleted += OnSceneLoadCompleted;

                var _ = UIManager.Instance;

                DignusUnityCoroutineManager.Start(UnityMainThread.ExecutePending());
            }
        }
        private void InitResolution()
        {
            DeviceWidth = Screen.width;
            DeviceHeight = Screen.height;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log)
            {
                return;
            }
            if (type == LogType.Warning)
            {
                return;
            }
            _sb.AppendLine(condition);
            _sb.AppendLine(stackTrace);
            _sb.AppendLine();
            LogHelper.Fatal(_sb.ToString());
            _sb.Clear();
        }
        public Vector2 GetTargetResolution()
        {
            return new Vector2(TargetWidth, TargetHeight);
        }
        public Vector2 GetResolution()
        {
            return new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            LogHelper.Fatal(exception);
            UIManager.Instance.ShowAlert("Error", exception.Message, () =>
            {
                DignusUnitySceneManager.Instance.LoadScene(SceneType.TitleScene);
            });
        }
        private void InitLog()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Application.logMessageReceivedThreaded += Application_logMessageReceived;
            Application.logMessageReceived += Application_logMessageReceived;

#if UNITY_EDITOR
            _logPath = "./logs/UnityLogFile.txt";
            _archiveLogPath = "./logs/archive/UnityLogFile.{#}.txt";
#endif

            var logConfiguration = new LogConfiguration();

            var fileLogTarget = new FileLogTarget()
            {
                ArchiveFileName = _archiveLogPath,
                ArchiveRollingType = FileRollingType.Day,
                AutoFlush = true,
                KeepOpenFile = true,
                LogFileName = _logPath,
                MaxArchiveFile = 7,
                LogFormatRenderer = new LogFormatRenderer()
            };
            fileLogTarget.LogFormatRenderer.SetLogFormat("${datetime} | ${message} | ${callerFileName} : ${callerLineNumber}");

            var fileRule = new LoggerRule("unity logger", LogLevel.Fatal, fileLogTarget);
            logConfiguration.AddLogRule("file rule", fileRule);

            var unityTarget = new UnityLogTarget();
            var unityConsoleRule = new LoggerRule("unity logger", LogLevel.Debug, unityTarget);
            logConfiguration.AddLogRule("unity console rule", unityConsoleRule);
            LogBuilder.Configuration(logConfiguration);
            LogBuilder.Build();
            LogHelper.SetLogger(LogManager.GetLogger("unity logger"));
        }
        public void OnSceneLoadCompleted(string sceneName)
        {
            AdjustCameraViewportToAspectRatio(Camera.main);
            var mainCamData = Camera.main.GetUniversalAdditionalCameraData();
            mainCamData.cameraStack.Add(UIManager.Instance.UICamera);
        }

        public void AdjustCameraViewportToAspectRatio(Camera camera)
        {
            var rect = camera.rect;
            var scaleheight = (float)DeviceWidth / DeviceHeight / ((float)TargetWidth / TargetHeight);
            var scalewidth = 1F / scaleheight;
            if (scaleheight < 1F)
            {
                rect.height = scaleheight;
                rect.y = (1f - scaleheight) / 2F;
            }
            else
            {
                rect.width = scalewidth;
                rect.x = (1f - scalewidth) / 2F;
            }
            camera.rect = rect;
        }
    }
}
