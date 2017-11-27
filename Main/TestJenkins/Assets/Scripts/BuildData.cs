

[System.Serializable]
public class BuildData{
	public enum ABcompressType
    {
		LZ4压缩=256,
		默认LZMA=0,
		不压缩=1,
	}
	public enum RuntimeConfigs{
		测试,
		读取资源包,
	}
	public string 备注="测试";
	public bool 是否生成资源包;
	public ABcompressType 资源包压缩格式;
	public bool 是否保留Resources;
	public bool 是否保留所有的场景;
	/// <summary>
	/// 如果不保留所有的场景,保留哪些场景,分号间隔
	/// </summary>
	public string 保留的场景;
	public bool 是否拷贝资源包到StreamingAssets;
	public string 编译参数;
	public string 插件目录名;
	public string 运行时配置方案;
	public string 文件名="g01";
	public bool 文件名加日期 =true;
    public string adk;
    public string jdk;
    public string ndk;
	public string 输出路径="out1";
    public string BundleIdentifier = "com.huanyz.g01";
    public string productName = "TestJenkins";
    public int bundleVersionCode = 1;
    public string BundleVersion = "1.0";
}
