using PureMVC.Patterns;
using PureMVC.Interfaces;
using System.Collections.Generic;

public class AppMediator : Mediator {

    public const string NAME = "AppMediator";

    public AppMediator(object viewComponent)
        : base(NAME, viewComponent) {
    }

    public AppView view {
        get {
            return ViewComponent as AppView;
        }
    }

    /// <summary>
    ///  AppMediator需要监听的消息
    /// </summary>
    /// <returns></returns>
    override public IList<string> ListNotificationInterests() {
        return new List<string>()
        { 
            NotiConst.LOGIN,
            NotiConst.OTHER_NOTIFICATION
        };
    }

    override public void HandleNotification(INotification notification) {
        string name = notification.Name;
        object body = notification.Body;
        switch (name) {
            case NotiConst.LOGIN:      //登录
                view.OnLogin(notification.ToString());
            break;
            case NotiConst.OTHER_NOTIFICATION:

            break;
        }
    }
}
