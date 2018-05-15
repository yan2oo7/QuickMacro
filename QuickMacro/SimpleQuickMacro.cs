﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickMacro
{
    public partial class SimpleQuickMacro : Form
    {
        public SimpleQuickMacro()
        {
            InitializeComponent();
        }

        #region 成员变量
        /// <summary>
        /// 是否重新编辑
        /// </summary>
        bool reEdit = false;
        /// <summary>
        /// 本地Script
        /// </summary>
        LocalScript localScript = new LocalScript();
        /// <summary>
        /// 动态执行类
        /// </summary>
        DynamicInvoke dicInvoke = new DynamicInvoke();
        /// <summary>
        /// 键盘钩子类
        /// </summary>
        KeyboardHook keyHook;
        /// <summary>
        /// 录制状态
        /// </summary>
        bool recordState = false;
        #endregion
        #region 总的
        #region 窗体加载
        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleQuickMacro_Load(object sender, EventArgs e)
        {
            ConfigInfo.Get_ConfigInfo();
            localScript.ReadScript();
            initPage_Choose();
            initPage_Set();
            cmb_Choose_c.SelectedItem = "Default";
            txt_Details_c.Text = localScript.scriptList.Find(i => i.ScriptName == cmb_Choose_c.SelectedItem.ToString()).Details;
            RegHotKeys();
        }
        #endregion 
        #region 注册热键
        /// <summary>
        /// 注册热键
        /// </summary>
        private void RegHotKeys()
        {
            SystemHotKey.UnRegHotKey(this.Handle, 7001);
            SystemHotKey.RegHotKey(this.Handle, 7001, (EnumClass.KeyModifiers)Enum.Parse(typeof(EnumClass.KeyModifiers), cmb_Activate_Shift.Text), (Keys)Enum.Parse(typeof(Keys), cmb_Activate_Main.Text));
            SystemHotKey.UnRegHotKey(this.Handle, 7002);
            SystemHotKey.RegHotKey(this.Handle, 7002, (EnumClass.KeyModifiers)Enum.Parse(typeof(EnumClass.KeyModifiers), cmb_Stop_Shift.Text), (Keys)Enum.Parse(typeof(Keys), cmb_Stop_Main.Text));
            SystemHotKey.UnRegHotKey(this.Handle, 7003);
            SystemHotKey.RegHotKey(this.Handle, 7003, (EnumClass.KeyModifiers)Enum.Parse(typeof(EnumClass.KeyModifiers), cmb_Start_Shift.Text), (Keys)Enum.Parse(typeof(Keys), cmb_Start_Main.Text));
            SystemHotKey.UnRegHotKey(this.Handle, 7004);
            SystemHotKey.RegHotKey(this.Handle, 7004, (EnumClass.KeyModifiers)Enum.Parse(typeof(EnumClass.KeyModifiers), cmb_Resize_Shift.Text), (Keys)Enum.Parse(typeof(Keys), cmb_Resize_Main.Text));
        }
        #endregion
        #region 捕获键盘事件
        /// <summary>
        /// 捕获键盘事件
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312:    //这个是window消息定义的注册的热键消息
                    switch (m.WParam.ToString())
                    {
                        case "7001":
                            dicInvoke.ReCompiler(txt_Details_c.Text.Trim());
                            dicInvoke.StartThread();
                            break;
                        case "7002":
                            dicInvoke.EndThread();
                            break;
                        case "7003":
                            BeginRecord();
                            break;
                        case "7004":
                            ShowAndHideForm();
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion
        #region 选项卡选择变更
        /// <summary>
        /// 选项卡选择变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MainTab.SelectedTab.Name)
            {
                case "page_Choose":
                    initPage_Choose();
                    break;
                case "page_Record":
                    initPage_Record();
                    break;
                case "page_Set":
                    initPage_Set();
                    break;
            }
        }
        #endregion
        #region 显示或隐藏窗体
        /// <summary>
        /// 隐藏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleQuickMacro_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;//隐藏窗体
                this.notifyIcon.Visible = true;//显示托盘图标   
            }
        }
        /// <summary>
        /// 显示窗体
        /// </summary>
        private void ShowForm()
        {
            this.Visible = true;//隐藏窗体
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon.Visible = false;//显示托盘图标
        }
        /// <summary>
        /// 隐藏窗体
        /// </summary>
        private void HideForm()
        {
            this.WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// 显示或隐藏窗体
        /// </summary>
        private void ShowAndHideForm()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                ShowForm();
            }
            else
            {
                HideForm();
            }
        }
        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MaxSize_Click(object sender, EventArgs e)
        {
            ShowForm();
        }
        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }
        #endregion
        #region 关闭窗体
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Quit_Click(object sender, EventArgs e)
        {
            SystemHotKey.UnRegHotKey(this.Handle, 7001);
            SystemHotKey.UnRegHotKey(this.Handle, 7002);
            SystemHotKey.UnRegHotKey(this.Handle, 7003);
            SystemHotKey.UnRegHotKey(this.Handle, 7004);
            this.Close();
        }
        #endregion
        #endregion
        #region 选择选项卡
        #region 选项卡初始化
        /// <summary>
        /// 选择选项卡初始化
        /// </summary>
        private void initPage_Choose()
        {
            cmb_Choose_c.DataSource = localScript.scriptList.Select(i => i.ScriptName).ToArray();
        }
        #endregion
        #region 重新编辑
        /// <summary>
        /// 重新编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Edit_c_Click(object sender, EventArgs e)
        {
            reEdit = true;
            MainTab.SelectedTab = page_Record;
            reEdit = false;
        }
        #endregion
        #region 选择不同的脚本
        /// <summary>
        /// 选择不同的脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_Choose_c_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_Details_c.Text = localScript.scriptList.Find(i => i.ScriptName == cmb_Choose_c.SelectedItem.ToString()).Details;
        }
        #endregion
        #region 删除脚本
        /// <summary>
        /// 删除脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delete_c_Click(object sender, EventArgs e)
        {
            if (cmb_Choose_c.Text != "Default")
            {
                localScript.scriptList.RemoveAll(i => i.ScriptName == cmb_Choose_c.Text);
                cmb_Choose_c.DataSource = localScript.scriptList.Select(i => i.ScriptName).ToArray();
                cmb_Choose_c.SelectedItem = "Default";
            }
            else
            {
                MessageBox.Show("默认脚本不能删除");
            }
        }
        #endregion
        #endregion
        #region 录制选项卡
        #region 录制选项卡初始化
        /// <summary>
        /// 录制选项卡初始化
        /// </summary>
        private void initPage_Record()
        {
            keyHook = new KeyboardHook(txt_Details_r);
            if (reEdit)
            {
                txt_FileName_r.Text = cmb_Choose_c.Text;
                txt_Details_r.Text = txt_Details_c.Text;
            }
            else
            {
                txt_FileName_r.Text = "新建脚本";
                txt_Details_r.Clear();
            }
        }
        #endregion
        #region 保存脚本
        /// <summary>
        /// 保存脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_r_Click(object sender, EventArgs e)
        {
            if (!new DynamicInvoke().ReCompiler(txt_Details_r.Text.Trim("\r\n").Trim()))
                return;
            ScriptClass sc;
            if ((sc = localScript.scriptList.Find(i => i.ScriptName == txt_FileName_r.Text.Trim("\r\n").Trim())) != null)
            {
                sc.Details = txt_Details_r.Text;
                localScript.WriteScript();
            }
            else
            {
                sc = new ScriptClass();
                sc.ScriptName = txt_FileName_r.Text.Trim("\r\n").Trim();
                sc.Details = txt_Details_r.Text.Trim("\r\n").Trim();
                localScript.scriptList.Add(sc);
                localScript.WriteScript();
                cmb_Choose_c.DataSource = localScript.scriptList.Select(i => i.ScriptName).ToArray();
            }
        }
        #endregion
        #region 录制
        /// <summary>
        /// 录制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Record_r_Click(object sender, EventArgs e)
        {
            BeginRecord();
        }
        /// <summary>
        /// 录制
        /// </summary>
        private void BeginRecord()
        {
            if (!recordState)
            {
                keyHook.Start();
                btn_Record_r.Text = "停止";
                recordState = true;
            }
            else
            {
                keyHook.Stop();
                btn_Record_r.Text = "录制";
                recordState = false;
            }
        }
        #endregion
        #region 编译
        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Compiler_Click(object sender, EventArgs e)
        {
            new DynamicInvoke().ReCompiler(txt_Details_r.Text.Trim("\r\n").Trim());
        }
        #endregion 
        #endregion
        #region 设置选项卡
        #region 设置选项卡初始化
        /// <summary>
        /// 设置选项卡初始化
        /// </summary>
        private void initPage_Set()
        {
            foreach (Control c in page_Set.Controls)
            {
                if (c.GetType() == typeof(ComboBox))
                {
                    if (c.Tag.ToString() == "shift")
                    {
                        ((ComboBox)c).DataSource = System.Enum.GetNames(typeof(EnumClass.KeyModifiers));
                    }
                    else if (c.Tag.ToString() == "main")
                    {
                        ((ComboBox)c).DataSource = System.Enum.GetNames(typeof(EnumClass.KeyMain));
                    }
                }
            }
            string[] strs = ConfigInfo.ActivateHotKey.Split('+');
            cmb_Activate_Shift.SelectedItem = strs[0];
            cmb_Activate_Main.SelectedItem = strs[1];
            strs = ConfigInfo.StopHotKey.Split('+');
            cmb_Stop_Shift.SelectedItem = strs[0];
            cmb_Stop_Main.SelectedItem = strs[1];
            strs = ConfigInfo.RecordHotKey.Split('+');
            cmb_Start_Shift.SelectedItem = strs[0];
            cmb_Start_Main.SelectedItem = strs[1];
            strs = ConfigInfo.ShowHideHotKey.Split('+');
            cmb_Resize_Shift.SelectedItem = strs[0];
            cmb_Resize_Main.SelectedItem = strs[1];
        }
        #endregion
        #region 重置参数
        /// <summary>
        /// 重置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Reset_s_Click(object sender, EventArgs e)
        {
            ConfigInfo.UpdateAppConfig("ActivateHotKey", "None+F10");
            ConfigInfo.UpdateAppConfig("StopHotKey", "None+F11");
            ConfigInfo.UpdateAppConfig("RecordHotKey", "None+F2");
            ConfigInfo.UpdateAppConfig("ShowHideHotKey", "None+F1");
            cmb_Activate_Shift.SelectedItem = "None";
            cmb_Activate_Main.SelectedItem = "F10";
            cmb_Stop_Shift.SelectedItem = "None";
            cmb_Stop_Main.SelectedItem = "F11";
            cmb_Start_Shift.SelectedItem = "None";
            cmb_Start_Main.SelectedItem = "F2";
            cmb_Resize_Shift.SelectedItem = "None";
            cmb_Resize_Main.SelectedItem = "F1";
        }
        #endregion
        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_s_Click(object sender, EventArgs e)
        {
            ConfigInfo.UpdateAppConfig("ActivateHotKey", cmb_Activate_Shift.Text + '+' + cmb_Activate_Main.Text);
            ConfigInfo.UpdateAppConfig("StopHotKey", cmb_Stop_Shift.Text + '+' + cmb_Stop_Main.Text);
            ConfigInfo.UpdateAppConfig("RecordHotKey", cmb_Start_Shift.Text + '+' + cmb_Start_Main.Text);
            ConfigInfo.UpdateAppConfig("ShowHideHotKey", cmb_Resize_Shift.Text + '+' + cmb_Resize_Main.Text);
            RegHotKeys();
        }
        #endregion
        #endregion
    }
}