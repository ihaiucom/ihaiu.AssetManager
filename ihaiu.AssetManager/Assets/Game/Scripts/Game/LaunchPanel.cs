using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.ihaiu;


namespace Games 
{
    public class LaunchPanel : MonoBehaviour 
    {
        public Slider   barSlider;
        public Text     barRateText;
        public Text     barStateText;

        public GameObject   downloadAppPanel;
        public Text         downloadAppUrlText;

        private VersionManager versionManager;

        public void Show(VersionManager versionManager)
        {
            this.versionManager = versionManager;
            versionManager.stateCallback            += OnState;
            versionManager.updateFailedCallback     += OnUpdateFailed;
            versionManager.updateProgressCallback   += OnUpdateProgress;
            versionManager.needDownAppCallback      += OnNeedDownApp;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            if (versionManager != null)
            {
                versionManager.stateCallback            -= OnState;
                versionManager.updateFailedCallback     -= OnUpdateFailed;
                versionManager.updateProgressCallback   -= OnUpdateProgress;
                versionManager.needDownAppCallback      -= OnNeedDownApp;
            }

            versionManager = null;
        }

        void OnState(string txt)
        {
            barStateText.text = txt;
        }


        void OnUpdateFailed(string url)
        {
        }


        void OnUpdateProgress(float progress)
        {
            barSlider.value = progress;
            barRateText.text = Mathf.CeilToInt(progress * 100) + "%";
        }

        void OnNeedDownApp(string url)
        {
            downloadAppPanel.SetActive(true);
            downloadAppUrlText.text = url;
        }

        void OnFinal()
        {
        }
    	

        public void OnClickDownloadApp()
        {
            Application.OpenURL(versionManager.serverVersionInfo.downLoadUrl);
        }
    }
}