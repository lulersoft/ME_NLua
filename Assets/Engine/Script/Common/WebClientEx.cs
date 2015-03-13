using UnityEngine;
using System.Collections;
using System.Net;


public class WebClientEx : WebClient
{
    private int _timeout;
    /// <summary>
    /// 超时时间(毫秒)
    /// </summary>
    public int Timeout
    {
        get
        {
            return _timeout;
        }
        set
        {
            _timeout = value;
        }
    }

    public WebClientEx()
    {
        this._timeout = 6500;
    }

    public WebClientEx(int timeout)
    {
        this._timeout = timeout;
    }

    protected override WebRequest GetWebRequest(System.Uri address)
    {
        var result = base.GetWebRequest(address);
        result.Timeout = this._timeout;
        return result;
    }
}

