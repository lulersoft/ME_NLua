using UnityEngine;
using System.Collections;
using NLua;

public class MeLoadBundle : MonoBehaviour {

    public static Hashtable BundleTable = new Hashtable();

    public static GameObject obj = null;
    public static MeLoadBundle self = null;
	void Awake () {
        obj = this.gameObject;
        self = this;
        DontDestroyOnLoad(gameObject);  //防止销毁自己
	}

    public void LoadBundle(string fname, Callback<string, AssetBundle> handler)
    {
        if (BundleTable.ContainsKey(fname))
        {
            AssetBundle bundle = BundleTable[fname] as AssetBundle;
            if (handler != null) handler(name, bundle);
        }
        else
        {
            StartCoroutine(onLoadBundle(fname, handler));
        }
    }

    public void UnLoadAllBundle()
    {
        foreach (AssetBundle bundle in BundleTable.Values)
        {
            if (bundle != null)
                bundle.Unload(false);
        }
        BundleTable.Clear();
    }

    //释放某AssetBundle
    public  void UnLoadBundle(AssetBundle bundle)
    {
        string key = "";
        foreach (DictionaryEntry de in BundleTable)
        {
            key = de.Key.ToString();
            break;
        }

        if (BundleTable.ContainsKey(key))
        {
            BundleTable.Remove(key);
        }
        if (bundle != null)
            bundle.Unload(false);
    }
    //释放某AssetBundle
    public  void UnLoadBundle(string key)
    {
        if (BundleTable.ContainsKey(key))
        {
            AssetBundle bundle = BundleTable[key] as AssetBundle;
            if (bundle != null)
            {
                bundle.Unload(false);
            }
            BundleTable.Remove(key);
        }
    }

    protected IEnumerator onLoadBundle(string name, Callback<string, AssetBundle> handler)
    {
        string uri = "";
        if (name.LastIndexOf(".") != -1)
        {
            uri = "file:///" + API.AssetPath + name;
        }
        else
        {
            uri = "file:///" + API.AssetPath + name + ".ab";
        }

        WWW www = new WWW(uri);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Warning erro: " + uri);
            Debug.Log("Warning erro: " + www.error);
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
            BundleTable[name] = bundle;
            if (handler != null) handler(name, bundle);
        }
        catch (NLua.Exceptions.LuaException e)
        {
            Debug.LogError(FormatException(e), gameObject);
        }
    }

    public static string FormatException(NLua.Exceptions.LuaException e)
    {
        string source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
        return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
    }
    protected void OnDestroy()
    {
        UnLoadAllBundle(); 
    }
}
