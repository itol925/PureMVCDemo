using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using System;
using System.Windows.Forms;

/**
 * 注册Command ，建立 Command 与Notification 之间的映射
 */
public class BootstrapViewMediators : SimpleCommand {

    /// <summary>
    /// 执行启动命令
    /// </summary>
    /// <param name="notification"></param>
    public override void Execute(INotification noti) {
        //------------ 将view与相应的mediator绑定 -------------
        
        AppView appView = new AppView();
        Facade.RegisterMediator(new AppMediator(appView));         
        Application.Run(appView);
    }
}
