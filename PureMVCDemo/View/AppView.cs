using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class AppView : Form{
    #region designer
    private System.ComponentModel.IContainer components = null;
    private Button m_loginBtn;
    private Label m_messageLbl;

    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Text = "AppView";

        m_loginBtn = new Button();
        m_loginBtn.SetBounds(100, 100, 100, 100);
        this.Controls.Add(m_loginBtn);
        m_loginBtn.Text = "登录";
        m_loginBtn.Click += m_loginBtn_Click;

        m_messageLbl = new Label();
        m_messageLbl.Width = 500;
        m_messageLbl.Height = 300;
        m_messageLbl.Text = "message:................";
        this.Controls.Add(m_messageLbl);
    }

    #endregion

    public AppView() {
        InitializeComponent();
    }

    public void OnLogin(string notiStr) { 
        m_messageLbl.Text = notiStr;
    }

    void m_loginBtn_Click(object sender, EventArgs e) {
        AppFacade.Instance.SendNotification(NotiConst.LOGIN, "usrname:张三;pwd:123");
    }
}

