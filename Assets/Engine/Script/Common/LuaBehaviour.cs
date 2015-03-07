using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NLua;
using System.ComponentModel;
using System.Text;

/// <summary>
/// LuaBehaviour
/// @author 小陆  QQ:2604904
/// </summary>
public class LuaBehaviour : MonoBehaviour
{
    [System.NonSerialized]
    public bool usingUpdate = false;
    [System.NonSerialized]
    public bool usingFixedUpdate = false;
    protected bool isLuaReady = false;
    private string script = "";

    protected LuaTable table;
    protected List<MissionPack> MissionList = new List<MissionPack>();


    protected Lua env
    {
        get
        {
            return API.env;
        }
    }
    protected void Update()
    {
        if (MissionList.Count > 0)
        {
            MissionPack pack = MissionList[0];
            MissionList.RemoveAt(0);
            pack.Call();
        }

        if (usingUpdate)
        {
            CallMethod("Update");
        }
    }

    protected void FixedUpdate()
    {
        if (usingFixedUpdate)
        {
            CallMethod("FixedUpdate");
        }
    }
    //切换回主线程用
    public void AddMission(LuaFunction func, params object[] args)
    {
        MissionList.Add(new MissionPack(func, args));
    }


    public string AssetPath
    {
        get
        {
            string target = string.Empty;
            if (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                target = "iphone";
            }
            else
            {
                target = "android";
            }
            return Application.persistentDataPath + "/asset/" + target + "/";
        }
    }    

    public void LoadBundle(string fname, Callback<string, AssetBundle> handler)
    {
        if (API.BundleTable.ContainsKey(fname))
        {
            AssetBundle bundle = API.BundleTable[fname] as AssetBundle;
            if (handler != null) handler(name, bundle);
        }
        else
        {
            StartCoroutine(onLoadBundle(fname, handler));
        }
    }

    public void UnLoadAllBundle()
    {
        foreach (AssetBundle bundle in API.BundleTable.Values)
        {
            bundle.Unload(true);
        }
        API.BundleTable.Clear();
    }


    IEnumerator onLoadBundle(string name, Callback<string, AssetBundle> handler)
    {
        string uri = "";
        if (name.LastIndexOf(".") != -1)
        {
            uri = "file:///" + AssetPath + name;
        }
        else
        {
            uri = "file:///" + AssetPath + name + ".assetbundle";
        }

        WWW www = new WWW(uri);
        yield return www;
        if (www.error != null)
        {
            API.Log("Warning erro: " + uri);
            API.Log("Warning erro: " + www.error);
            StopCoroutine("onLoadBundle");
            yield break;
        }
        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        byte[] data = www.bytes;

        //资源解密
        API.Encrypt(ref data);

        AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(data);

        yield return new WaitForEndOfFrame();

        try
        {
            API.BundleTable[name] = bundle;
            if (handler != null) handler(name, bundle);
        }
        catch (NLua.Exceptions.LuaException e)
        {
            API.LogError(FormatException(e));
        }
    }



    public void DestroyMe()
    {
        Destroy(gameObject);
    }
    protected void OnDestroy()
    {
        if (MissionList.Count>0)
        {
            MissionList.Clear();
        }

        CallMethod("OnDestroy");

        if (table != null)
        {
            table.Dispose();
        }
    }
    /*
        public IEnumerator RunCoroutine()
        {        
            object[] result = new object[0];
            if (table == null) return (IEnumerator) result[0];
            result = CallMethod("RunCoroutine");
            return (IEnumerator)result[0];   
        }
    */
    //加载脚本文件
    public void DoFile(string filename)
    {
        if (filename.EndsWith(".lua"))
        {
            script = API.AssetRoot + "lua/" + filename;
        }
        else
        {
            script = API.AssetRoot + "lua/" + filename + ".lua";
        }
        try
        {
            object[] chunk = env.DoFile(script);

            if (chunk != null && chunk[0] != null)
            {
                table = chunk[0] as LuaTable;
                table["this"] = this;
                table["transform"] = transform;
                table["gameObject"] = gameObject;

                CallMethod("Start");

                isLuaReady = true;
            }

        }
        catch (NLua.Exceptions.LuaException e)
        {
            isLuaReady = false;
            API.LogError(FormatException(e));
        }
    }
    //获取绑定的lua脚本
    public LuaTable GetChunk()
    {
        return table;
    }

    //设置lua脚本可直接使用变量
    public void SetEnv(string key, object val, bool isGlobal)
    {
        if (isGlobal)
        {
            env[key] = val;
        }
        else
        {
            if (table != null)
            {
                table[key] = val;
            }
        }
    }

    //延迟执行
    public void LuaInvoke(float delaytime, LuaFunction func, params object[] args)
    {
        StartCoroutine(doInvoke(delaytime, func, args));
    }
    private IEnumerator doInvoke(float delaytime, LuaFunction func, params object[] args)
    {
        yield return new WaitForSeconds(delaytime);
        if (args != null)
        {
            func.Call(args);
        }
        else
        {
            func.Call();
        }
    }
    //协程
    public void RunCoroutine(YieldInstruction ins, LuaFunction func, params object[] args)
    {
        StartCoroutine(doCoroutine(ins, func, args));
    }

    private IEnumerator doCoroutine(YieldInstruction ins, LuaFunction func, params object[] args)
    {
        yield return ins;
        if (args != null)
        {
            func.Call(args);
        }
        else
        {
            func.Call();
        }
    }


    public object[] CallMethod(string fn, params object[] args)
    {
        //API.Log ("call function  >> "+fn);
        object[] result = new object[0];

        if (table == null || table[fn] == null || !(table[fn] is LuaFunction)) return result;

        LuaFunction func = (LuaFunction)table[fn];//table.RawGet(fn) as LuaFunction;

        try
        {
            if (args != null)
            {
                result = func.Call(args);
            }
            else
            {
                result = func.Call();
            }

        }
        catch (NLua.Exceptions.LuaException e)
        {
            API.Log("Error whe call '" + fn + "',err=" + e.Message);
            API.LogWarning(FormatException(e));
        }

        return result;
    }

    public object[] CallMethod(string function)
    {
        return CallMethod(function, null);
    }

    private object[] Call(string fn, params object[] args)
    {

        object[] result = new object[0];

        if (env == null || !isLuaReady) return result;

        LuaFunction lf = env.GetFunction(fn);

        if (lf == null) return result;

        try
        {
            if (args != null)
            {
                result = lf.Call(args);
            }
            else
            {
                result = lf.Call();
            }
        }
        catch (NLua.Exceptions.LuaException e)
        {
            API.LogError(FormatException(e));
        }
        return result;
    }

    private object[] Call(string function)
    {
        return Call(function, null);
    }

    public static string FormatException(NLua.Exceptions.LuaException e)
    {
        string source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
        return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
    }

    #region 消息中心
    //添加消息侦听
    public void AddListener(string eventType, Callback handler)
    {
        Messenger.AddListener(eventType, handler);
    }

    public void AddListener2(string eventType, Callback<object> handler)
    {
        Messenger.AddListener<object>(eventType, handler);
    }

    //移除一事件侦听
    public void RemoveListener(string eventType, Callback handler)
    {
        Messenger.RemoveListener(eventType, handler);
    }
    public void RemoveListener2(string eventType, Callback<object> handler)
    {
        Messenger.RemoveListener<object>(eventType, handler);
    }

    //触发消息广播
    public void Broadcast(string eventType)
    {
        Messenger.Broadcast(eventType);
    }

    public void Broadcast(string eventType, object args)
    {
        Messenger.Broadcast<object>(eventType, args);
    }
    #endregion

}


public struct MissionPack
{
    public LuaFunction func;
    public object[] args;

    public MissionPack(LuaFunction _func, params object[] _args)
    {
        func = _func;
        args = _args;
    }

    public object[] Call()
    {
        if (func == null) return new object[0] { };
        if (args != null)
        {
            return func.Call(args);
        }

        return func.Call();
    }
}
