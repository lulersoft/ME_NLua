﻿using UnityEngine;
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
    public int UID;
    public int Draggable = -1;
    protected bool isLuaReady = false;
    private string script = "";
  
    protected LuaTable table;
 
    //保存的lua 数据存取
    public LuaTable data{get;set;} 

    protected Lua env
    {
        get
        {
            return API.env;
        }
    }
    protected void Update()
    {
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

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
    protected void OnDestroy()
    {    

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
            script = Application.persistentDataPath + "/lua/" + filename;
        }
        else
        {
            script = Application.persistentDataPath + "/lua/" + filename + ".lua"; 
        }
        try
        {
            //Debug.Log("DoFile:" + script);

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
            Debug.LogError(FormatException(e), gameObject);
        }
    }
    //获取绑定的lua脚本
    public LuaTable GetChunk()
    {
        return table;
    }

    //设置lua脚本可直接使用变量
    public void SetEnv(string key,object val,bool isGlobal)
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
    public void LuaInvoke(float delaytime,LuaFunction func,params object[] args)
    {
        StartCoroutine(doInvoke(delaytime, func, args));
    }
    private IEnumerator doInvoke(float delaytime, LuaFunction func, params object[] args)
    {
        yield return new  WaitForSeconds(delaytime);
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
		//Debug.Log ("call function  >> "+fn);
		object[] result=new object[0];

		if (table == null || table[fn] == null || !(table[fn] is LuaFunction)) return result;

		LuaFunction func = (LuaFunction)table[fn];// table.RawGet(fn) as LuaFunction;

		try
        {
            if (args != null)
            {
				result= func.Call(args);
            }
            else
            {
				result= func.Call();
            }

        }
        catch (NLua.Exceptions.LuaException e)
        {
			Debug.Log("Error whe call '"+fn+"',err="+e.Message);
            Debug.LogWarning(FormatException(e));            
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
            Debug.LogError(FormatException(e));
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

    //挂接回调调用函数：一般用于jni或者invoke等操作
    public void MeMessage(object arg)
    {
        Messenger.Broadcast<object>(this.name + "MeMessage", arg);
    }

    //挂接回调调用函数：一般用于jin或者invoke等操作
    public void MeMessageAll(object arg)
    {
        Messenger.Broadcast<object>("MeMessageAll", arg);
    }
}


