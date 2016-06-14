using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;

/**
 * 注册Command ，建立 Command 与Notification 之间的映射
 */
public class BootstrapCommands : SimpleCommand {

    /// <summary>
    /// 执行启动命令
    /// </summary>
    /// <param name="notification"></param>
    public override void Execute(INotification notification) {
        //------------ 注册登录命令，即绑定 消息名称与命令类型 ---------
        Facade.RegisterCommand(NotiConst.LOGIN, typeof(LoginCommand));        
    }
}
