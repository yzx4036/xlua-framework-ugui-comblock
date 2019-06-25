namespace MatchKitEx
{
    /// <summary>
    /// Csharp端 配置表数据接口 没有用到sqlite可忽略，也可根据需要扩展为自己的配置表数据类
    /// </summary>
    public interface ICfgData
    {
        void Initialize();
    }
}