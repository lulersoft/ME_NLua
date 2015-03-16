using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NLua;
public class MeMission : MonoBehaviour
{

    public static List<MissionPack> MissionList = new List<MissionPack>();
    public static GameObject obj = null;
    public static MeMission self = null;

    void Awake()
    {
        obj = this.gameObject;
        self = this;
        DontDestroyOnLoad(gameObject);  //防止销毁自己
    }

    void Update()
    {
        if (MeMission.MissionList.Count > 0)
        {
            MissionPack pack = MeMission.MissionList[0];
            MeMission.MissionList.RemoveAt(0);
            pack.Call();
        }
    }

    public void AddMission(MissionPack pack)
    {
        MeMission.MissionList.Add(pack);
    }
}

public struct MissionPack
{
    public LuaFunction func;
    public object args;

    public MissionPack(LuaFunction _func, object _args)
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
