using System;
using PureMVC.Patterns;

public class AppProxy : Proxy {
    public static string NAME = "AppProxy";
    public static AppProxy instances;

    // Use this for initialization
    public AppProxy() : base(NAME) {
        instances = this;
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public override void OnRegister() {
        m_data = true;
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public override void OnRemove() {
        
    }

    
}
