namespace com.ihaiu
{
    public enum VersionCheckState
    {
        [HelpAttribute("不需要更新")]
        Normal,

        [HelpAttribute("需要热更新")]
        HotUpdate,

        [HelpAttribute("需要下载APP")]
        DownApp,
    }
}