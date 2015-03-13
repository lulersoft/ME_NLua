using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;
using NLua;
/// <summary>
/// 后面将制作菜单用来搜索所有用得到的delegate Type，自动生成 N lua delegate.
/// </summary>
[AttributeUsage(AttributeTargets.Delegate)]
public class NLuaDelegate : System.Attribute 
{
	public NLuaDelegate() 
	{
		//
	}
} 
class CallbackLuaFunction : NLua.Method.LuaDelegate
{
	void CallFunction ()
	{
		object [] args = new object [] { };
		object [] inArgs = new object [] {};
		int [] outArgs = new int [] { };
		base.CallFunction (args, inArgs, outArgs);
	}
}
class CallbackLuaFunction<T> : NLua.Method.LuaDelegate
{
	void CallFunction (T arg1)
	{
		object [] args = new object [] { arg1 };
		object [] inArgs = new object [] {arg1 };
		int [] outArgs = new int [] { };
		base.CallFunction (args, inArgs, outArgs);
	}
}
class CallbackLuaFunction<T,U> : NLua.Method.LuaDelegate
{
	void CallFunction ( T arg1,U arg2)
	{
		object [] args = new object [] { arg1,arg2 };
		object [] inArgs = new object [] {arg1,arg2 };
		int [] outArgs = new int [] { };
		base.CallFunction (args, inArgs, outArgs);
	}
}
class CallbackLuaFunction<T,U,V> : NLua.Method.LuaDelegate
{
	void CallFunction (T arg1,U arg2,V arg3)
	{
		object [] args = new object [] { arg1,arg2 ,arg3};
		object [] inArgs = new object [] {arg1,arg2 ,arg3};
		int [] outArgs = new int [] { };
		base.CallFunction (args, inArgs, outArgs);
	}
}

public class LuaBinder{
	public static void RegisterNLuaDelegate(Lua context){
		//主要针对ios，aot下不能有动态模板映射,当kera模式在ios发布平台的时候
		context.RegisterLuaDelegateType (typeof (EventListener.VoidDelegate), typeof (CallbackLuaFunction<GameObject>));
		context.RegisterLuaDelegateType (typeof (Action<object>), typeof (CallbackLuaFunction<object>));
		context.RegisterLuaDelegateType (typeof (Action), typeof (CallbackLuaFunction));
		context.RegisterLuaDelegateType (typeof (Callback), typeof (CallbackLuaFunction));
		context.RegisterLuaDelegateType (typeof (Callback<object>), typeof (CallbackLuaFunction<object>));
		context.RegisterLuaDelegateType (typeof (Callback<string,AssetBundle>), typeof (CallbackLuaFunction<string,AssetBundle>));
		context.RegisterLuaDelegateType (typeof (DownloadProgressChangedEventHandler), typeof (CallbackLuaFunction<object, DownloadProgressChangedEventArgs>));
		context.RegisterLuaDelegateType (typeof (AsyncCompletedEventHandler), typeof (CallbackLuaFunction<object, AsyncCompletedEventArgs>));
		context.RegisterLuaDelegateType (typeof (UploadProgressChangedEventHandler), typeof (CallbackLuaFunction<object, UploadProgressChangedEventArgs>));
		context.RegisterLuaDelegateType (typeof (UploadStringCompletedEventHandler), typeof (CallbackLuaFunction<object, UploadStringCompletedEventArgs>));
	}

}
public class API  {
        
    private static Lua lua;

    //资源加密解密常量定义
    public static int Encrypt_Len = 256;
    public static string Encrypt_Key = "this is source encryption key for me game frame,please custom this key string";

    //是否启用调试
    public static bool usingDebug = true;    

    public static Lua env
    {
        get
        {
            if (lua == null)
            {
                lua = new Lua();
                lua.LoadCLRPackage();
                //设置lua脚本文件查找路径
                lua["package.path"] = lua["package.path"] + ";" + API.AssetRoot + "lua/?.lua;";

				LuaBinder.RegisterNLuaDelegate (lua);
            }
            return lua;
        }
    }

    static string _assetRoot = "";

    public static string AssetRoot
    {
        get
        {
            if (_assetRoot != "")
            {
                return _assetRoot;
            }
            else
            {
                return Application.persistentDataPath + "/";
            }
        }
        set
        {
            _assetRoot = value;
        }
    }

    static string _assetPath="";
    public static string AssetPath
    {
        get
        {
            if (_assetPath != "")
            {
                return _assetPath;
            }else{
                 return API.AssetRoot + "asset/" + API.GetTargetPlatform + "/";
            }
        }
        set{
            _assetPath = value;
        }
    }

    static private string _GetTargetPlatform;
    //资源目标平台
    static public string GetTargetPlatform
    {
        get
        {
            if (_GetTargetPlatform != null)
                return _GetTargetPlatform;
            string target = "webplayer";
#if UNITY_STANDALONE
            target = "standalonewindows";
#elif UNITY_IPHONE
            target = "ios";
#elif UNITY_ANDROID
            target = "android";
#endif
            return target;
        }
    }

    #region DebugTools
    public static void Log(object msg)
    {
        if (usingDebug)
        {
            DebugTools.log += msg.ToString()+"\n\r";
            if(DebugTools.obj==null)
            {
                DebugTools.obj = new GameObject("~DebugTools");
                DebugTools.obj.AddComponent<DebugTools>();
            }
        }
    }

    public static void LogError(object msg)
    {
        Log(msg);
    }

    public static void LogWarning(object msg)
    {
        Log(msg);
    }
    #endregion

    //	--lua中添加个照相机例子：
	// local carm=GameObject("carm")	
	// API.AddComponent(carm,Camera)

    public static UnityEngine.Component AddComponent(GameObject obj, ProxyType t)
    {        
        return obj.AddComponent(t.UnderlyingSystemType);
    }

    public static UnityEngine.Component AddMissComponent(GameObject obj, ProxyType t)
    {
        Type _t = t.UnderlyingSystemType;
        string classname = _t.ToString();
        int idx = classname.LastIndexOf(".");
        classname = classname.Substring(idx + 1, classname.Length - idx - 1);      
        UnityEngine.Component _out = obj.GetComponent(classname);
         if (null == _out)
        {          
            _out = obj.AddComponent(_t);          
        }               
        return _out;
    }

   public static object AddComponent(GameObject obj,string classname)
    {
        Type t = Type.GetType(classname);
        return obj.AddComponent(t);
    }
    public static object AddMissComponent(GameObject obj, string classname)
    {
        Type t = Type.GetType(classname);
        object _out = obj.GetComponent(t);
        if(_out==null)
        {
            _out = obj.AddComponent(t);
        }
        return _out;
    }
    //zip压缩
    public static void PackFiles(string filename, string directory)
    {
        try
        {
            FastZip fz = new FastZip();
            fz.CreateEmptyDirectories = true;
            fz.CreateZip(filename, directory, true, "");
            fz = null;
        }
        catch (Exception)
        {
            throw;
        }
    }

    //zip解压
    public static bool UnpackFiles(string file, string dir)
    {
        try
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            ZipInputStream s = new ZipInputStream(File.OpenRead(file));

            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {

                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);

                if (directoryName != String.Empty)
                    Directory.CreateDirectory(dir + directoryName);

                if (fileName != String.Empty)
                {
                    FileStream streamWriter = File.Create(dir + theEntry.Name);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    streamWriter.Close();
                }
            }
            s.Close();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

	//异步HTTP
	public static WebClientEx SendRequest(string url, string data, UploadProgressChangedEventHandler progressHander, UploadStringCompletedEventHandler completeHandler)
    {
        WebClientEx webClient = new WebClientEx();
        webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";  //采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
        webClient.Encoding = System.Text.UTF8Encoding.UTF8;
        System.Uri uri = new System.Uri(url);

		webClient.UploadProgressChanged += progressHander;
		webClient.UploadStringCompleted += completeHandler;

        try
        {
            webClient.UploadStringAsync(uri, "POST", data);//得到返回字符流      
        }
        catch (NLua.Exceptions.LuaException e)
        {
            API.Log("Post err " + e.Message);
        }
         //返回client ，可用 client.CancelAsync(); 中断下载
        return webClient;
    }
 
    //异步下载
	public static WebClient DownLoad(string src, string SavePath, DownloadProgressChangedEventHandler progressHander, AsyncCompletedEventHandler completeHander)
    {
        WebClient client = new WebClient();
		client.DownloadProgressChanged += progressHander;
		client.DownloadFileCompleted += completeHander;
        try
        {
            client.DownloadFileAsync(new System.Uri(src), SavePath);
        }
        catch (NLua.Exceptions.LuaException e)
        {
            API.Log("AsyncDownLoad err:" + e.Message);
        }

        //返回client ，可用 client.CancelAsync(); 中断下载
        return client;
    }

     //时钟
    public static MeTimer AddTimer(float interval, Callback<MeTimer> onTimerHander)
    {
        return AddTimer(interval, 0, onTimerHander);
    }
    public static MeTimer AddTimer(float interval, int loop, Callback<MeTimer> onTimerHander)
    {
        if(LuaMeTimer.obj==null)
        {
            LuaMeTimer.obj = new GameObject("~LuaMeTimer");
            LuaMeTimer.obj.AddComponent<LuaMeTimer>();
        }
        MeTimer timer = new MeTimer();
        timer.onTimer = onTimerHander;
        timer.interval = interval;
        timer.loop = loop;

        LuaMeTimer.TimerList.Add(timer);

        return timer;
    }
    
    public static void KillTimer(MeTimer timer)
    {
        if (timer != null)
        {
            timer.close=true;
        }
    }


    /// <summary>
    /// HashToMD5Hex
    /// </summary>
    public static string HashToMD5Hex(string sourceStr)
    {
        byte[] Bytes = Encoding.UTF8.GetBytes(sourceStr);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] result = md5.ComputeHash(Bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
                builder.Append(result[i].ToString("x2"));
            return builder.ToString();
        }
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string MD5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
    //RC4 字符串
    public static string RC4(string input, string key)
    {
        StringBuilder result = new StringBuilder();
        int x, y, j = 0;
        int[] box = new int[256];

        for (int i = 0; i < 256; i++)
        {
            box[i] = i;
        }

        for (int i = 0; i < 256; i++)
        {
            j = (key[i % key.Length] + box[i] + j) % 256;
            x = box[i];
            box[i] = box[j];
            box[j] = x;
        }

        for (int i = 0; i < input.Length; i++)
        {
            y = i % 256;
            j = (box[y] + j) % 256;
            x = box[y];
            box[y] = box[j];
            box[j] = x;

            result.Append((char)(input[i] ^ box[(box[y] + box[j]) % 256]));
        }
        return result.ToString();
    }

    //RC4 byte 数组
    public static byte[] RC4(byte[] input, string key)
    {
        byte[] result = new byte[input.Length];
        int x, y, j = 0;
        int[] box = new int[256];

        for (int i = 0; i < 256; i++)
        {
            box[i] = i;
        }

        for (int i = 0; i < 256; i++)
        {
            j = (key[i % key.Length] + box[i] + j) % 256;
            x = box[i];
            box[i] = box[j];
            box[j] = x;
        }

        for (int i = 0; i < input.Length; i++)
        {
            y = i % 256;
            j = (box[y] + j) % 256;
            x = box[y];
            box[y] = box[j];
            box[j] = x;

            result[i] = (byte)(input[i] ^ box[(box[y] + box[j]) % 256]);
        }
        return result;
    }

    //局部加密解密
    public static void Encrypt(ref byte[] input)
    {
        if (input.Length > Encrypt_Len)
        {
            byte[] tmp = new byte[Encrypt_Len];
            System.Array.Copy(input, 0, tmp, 0, Encrypt_Len);
            byte[] de = API.RC4(tmp, Encrypt_Key);
            for (int i = 0; i < Encrypt_Len; i++)
            {
                input[i] = de[i];
            }
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - -  
 * Stream 和 byte[] 之间的转换 
 * - - - - - - - - - - - - - - - - - - - - - - - */
    /// <summary> 
    /// 将 Stream 转成 byte[] 
    /// </summary> 
    public static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);

        // 设置当前流的位置为流的开始 
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    /// <summary> 
    /// 将 byte[] 转成 Stream 
    /// </summary> 
    public static Stream BytesToStream(byte[] bytes)
    {
        Stream stream = new MemoryStream(bytes);
        return stream;
    } 

    //发射线
    public static object Raycast(Ray ray, out RaycastHit hit)
    {
        return Physics.Raycast(ray, out hit);
    }
    public static object Raycast(Ray ray, out RaycastHit hit, float distance, int layerMask)
    {
        return Physics.Raycast(ray, out hit, distance, layerMask);
    }
    //加载 AssetBundle
    public static void LoadBundle(string fname, Callback<string, AssetBundle> handler)
    {
        if (MeLoadBundle.obj == null)
        {
            MeLoadBundle.obj = new GameObject("~MeLoadBundle");
            MeLoadBundle.obj.AddComponent<MeLoadBundle>();
        }
        MeLoadBundle.self.LoadBundle(fname, handler);
    }
    //释放所有AssetBundle
    public static void UnLoadAllBundle()
    {
        if (MeLoadBundle.self != null)
        {
            MeLoadBundle.self.UnLoadAllBundle();
        }
    }
    //释放某AssetBundle
    public static void UnLoadBundle(AssetBundle bundle)
    {
        if (MeLoadBundle.self != null)
        {
            MeLoadBundle.self.UnLoadBundle(bundle);
        }
    }
    //释放某AssetBundle
    public static void UnLoadBundle(string key)
    {
        if (MeLoadBundle.self != null)
        {
            MeLoadBundle.self.UnLoadBundle(key);
        }
    }
    //回归主线程
    public static void AddMission(LuaFunction func, params object[] args)
    {
        if (MeMission.obj == null)
        {
            MeMission.obj = new GameObject("~MeMission");
            MeMission.self = MeMission.obj.AddComponent<MeMission>();
        }

        MeMission.self.AddMission(new MissionPack(func, args));
    }

    #region 消息中心
    //添加消息侦听
    public static void AddListener(string eventType, Callback handler)
    {
        Messenger.AddListener(eventType, handler);
    }

    public static void AddListener2(string eventType, Callback<object> handler)
    {
        Messenger.AddListener<object>(eventType, handler);
    }

    //移除一事件侦听
    public static void RemoveListener(string eventType, Callback handler)
    {
        Messenger.RemoveListener(eventType, handler);
    }
    public static void RemoveListener2(string eventType, Callback<object> handler)
    {
        Messenger.RemoveListener<object>(eventType, handler);
    }

    //触发消息广播
    public static void Broadcast(string eventType)
    {
        Messenger.Broadcast(eventType);
    }

    public static void Broadcast(string eventType, object args)
    {
        Messenger.Broadcast<object>(eventType, args);
    }
    #endregion   

}
