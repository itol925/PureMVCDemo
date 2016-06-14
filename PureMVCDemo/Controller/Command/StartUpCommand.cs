using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class StartUpCommand : MacroCommand {

    protected override void InitializeMacroCommand() {
        base.InitializeMacroCommand();

        //if (!Util.CheckEnvironment()) return; // 资源、脚本等是否到位

        //BootstrapModels
        AddSubCommand(typeof(BootstrapModels));

        //BootstrapCommands
        AddSubCommand(typeof(BootstrapCommands));

        //BootstrapViewMediators
        AddSubCommand(typeof(BootstrapViewMediators));
    }

}