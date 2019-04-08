using TerminalCommunication;

namespace TerminalCommunication
{
    partial class Operator
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrBkg = new System.Windows.Forms.Timer(this.components);
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.picResize = new System.Windows.Forms.PictureBox();
            this.pnlControl = new DoubleBufferedPanel();
            this.lblFlow = new System.Windows.Forms.Label();
            this.lblFps = new System.Windows.Forms.Label();
            this.lblDelay = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnEnd = new ImageToggleButton();
            this.btnMode = new ImageToggleButton();
            this.btnFullScreen = new ImageToggleButton();
            this.btnSound = new ImageToggleButton();
            this.btnLock = new ImageToggleButton();
            this.picFlowState = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picResize)).BeginInit();
            this.pnlControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFlowState)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrBkg
            // 
            this.tmrBkg.Interval = 150;
            this.tmrBkg.Tick += new System.EventHandler(this.tmrBkg_Tick);
            // 
            // hScrollBar
            // 
            this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar.Location = new System.Drawing.Point(0, 363);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(523, 17);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.Visible = false;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar.Location = new System.Drawing.Point(523, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 363);
            this.vScrollBar.TabIndex = 2;
            this.vScrollBar.Visible = false;
            this.vScrollBar.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
            // 
            // picResize
            // 
            this.picResize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picResize.Image = global::TerminalCommunication.Properties.Resources.resize;
            this.picResize.Location = new System.Drawing.Point(523, 363);
            this.picResize.Name = "picResize";
            this.picResize.Size = new System.Drawing.Size(17, 17);
            this.picResize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picResize.TabIndex = 3;
            this.picResize.TabStop = false;
            this.picResize.Visible = false;
            // 
            // pnlControl
            // 
            this.pnlControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlControl.BackColor = System.Drawing.Color.Transparent;
            this.pnlControl.BackgroundImage = global::TerminalCommunication.Properties.Resources.bg;
            this.pnlControl.Controls.Add(this.picFlowState);
            this.pnlControl.Controls.Add(this.lblFlow);
            this.pnlControl.Controls.Add(this.lblFps);
            this.pnlControl.Controls.Add(this.lblDelay);
            this.pnlControl.Controls.Add(this.lblMsg);
            this.pnlControl.Controls.Add(this.btnEnd);
            this.pnlControl.Controls.Add(this.btnMode);
            this.pnlControl.Controls.Add(this.btnFullScreen);
            this.pnlControl.Controls.Add(this.btnSound);
            this.pnlControl.Controls.Add(this.btnLock);
            this.pnlControl.Location = new System.Drawing.Point(80, 0);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(380, 32);
            this.pnlControl.TabIndex = 0;
            this.pnlControl.MouseEnter += new System.EventHandler(this.pnlControl_MouseEnter);
            // 
            // lblFlow
            // 
            this.lblFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFlow.Font = new System.Drawing.Font("宋体", 7F);
            this.lblFlow.ForeColor = System.Drawing.Color.LightGray;
            this.lblFlow.Location = new System.Drawing.Point(120, 20);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(60, 10);
            this.lblFlow.TabIndex = 8;
            this.lblFlow.Text = "------ KB/s";
            this.lblFlow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFps
            // 
            this.lblFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFps.Font = new System.Drawing.Font("宋体", 7F);
            this.lblFps.ForeColor = System.Drawing.Color.LightGray;
            this.lblFps.Location = new System.Drawing.Point(78, 20);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(36, 10);
            this.lblFps.TabIndex = 7;
            this.lblFps.Text = "-- fps";
            this.lblFps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDelay
            // 
            this.lblDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDelay.Font = new System.Drawing.Font("宋体", 7F);
            this.lblDelay.ForeColor = System.Drawing.Color.LightGray;
            this.lblDelay.Location = new System.Drawing.Point(36, 20);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(36, 10);
            this.lblDelay.TabIndex = 6;
            this.lblDelay.Text = "--- ms";
            this.lblDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMsg
            // 
            this.lblMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMsg.AutoEllipsis = true;
            this.lblMsg.ForeColor = System.Drawing.Color.Transparent;
            this.lblMsg.Location = new System.Drawing.Point(32, 4);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(209, 16);
            this.lblMsg.TabIndex = 5;
            this.lblMsg.Text = "正在控制 芦苇微微 的电脑";
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEnd
            // 
            this.btnEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnd.BackColor = System.Drawing.Color.Transparent;
            this.btnEnd.BackgroundImage = global::TerminalCommunication.Properties.Resources.button_normal;
            this.btnEnd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnEnd.CheckedHover = global::TerminalCommunication.Properties.Resources.button_hover;
            this.btnEnd.CheckedNormal = global::TerminalCommunication.Properties.Resources.button_normal;
            this.btnEnd.CheckedPressed = global::TerminalCommunication.Properties.Resources.button_down;
            this.btnEnd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnd.ForeColor = System.Drawing.Color.White;
            this.btnEnd.Hover = global::TerminalCommunication.Properties.Resources.button_hover;
            this.btnEnd.IsChecked = false;
            this.btnEnd.Location = new System.Drawing.Point(325, 4);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Normal = global::TerminalCommunication.Properties.Resources.button_normal;
            this.btnEnd.Pressed = global::TerminalCommunication.Properties.Resources.button_down;
            this.btnEnd.Size = new System.Drawing.Size(48, 24);
            this.btnEnd.TabIndex = 4;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnMode
            // 
            this.btnMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMode.BackColor = System.Drawing.Color.Transparent;
            this.btnMode.BackgroundImage = global::TerminalCommunication.Properties.Resources.icon_highdef_normal;
            this.btnMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMode.CheckedHover = global::TerminalCommunication.Properties.Resources.icon_highdef_hover;
            this.btnMode.CheckedNormal = global::TerminalCommunication.Properties.Resources.icon_highdef_normal;
            this.btnMode.CheckedPressed = global::TerminalCommunication.Properties.Resources.icon_highdef_normal;
            this.btnMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMode.Hover = global::TerminalCommunication.Properties.Resources.icon_fluent_hover;
            this.btnMode.IsChecked = true;
            this.btnMode.Location = new System.Drawing.Point(299, 6);
            this.btnMode.Name = "btnMode";
            this.btnMode.Normal = global::TerminalCommunication.Properties.Resources.icon_fluent_normal;
            this.btnMode.Pressed = global::TerminalCommunication.Properties.Resources.icon_fluent_normal;
            this.btnMode.Size = new System.Drawing.Size(20, 20);
            this.btnMode.TabIndex = 3;
            this.btnMode.Click += new System.EventHandler(this.btnMode_Click);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFullScreen.BackColor = System.Drawing.Color.Transparent;
            this.btnFullScreen.BackgroundImage = global::TerminalCommunication.Properties.Resources.icon_fullscreen_normal;
            this.btnFullScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnFullScreen.CheckedHover = global::TerminalCommunication.Properties.Resources.icon_window_hover;
            this.btnFullScreen.CheckedNormal = global::TerminalCommunication.Properties.Resources.icon_window_normal;
            this.btnFullScreen.CheckedPressed = global::TerminalCommunication.Properties.Resources.icon_window_normal;
            this.btnFullScreen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFullScreen.Hover = global::TerminalCommunication.Properties.Resources.icon_fullscreen_hover;
            this.btnFullScreen.IsChecked = false;
            this.btnFullScreen.Location = new System.Drawing.Point(273, 6);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Normal = global::TerminalCommunication.Properties.Resources.icon_fullscreen_normal;
            this.btnFullScreen.Pressed = global::TerminalCommunication.Properties.Resources.icon_fullscreen_normal;
            this.btnFullScreen.Size = new System.Drawing.Size(20, 20);
            this.btnFullScreen.TabIndex = 2;
            this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
            // 
            // btnSound
            // 
            this.btnSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSound.BackColor = System.Drawing.Color.Transparent;
            this.btnSound.BackgroundImage = global::TerminalCommunication.Properties.Resources.icon_close_sound_normal;
            this.btnSound.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSound.CheckedHover = global::TerminalCommunication.Properties.Resources.icon_sound_hover;
            this.btnSound.CheckedNormal = global::TerminalCommunication.Properties.Resources.icon_sound_normal;
            this.btnSound.CheckedPressed = global::TerminalCommunication.Properties.Resources.icon_sound_normal;
            this.btnSound.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSound.Hover = global::TerminalCommunication.Properties.Resources.icon_close_sound_hover;
            this.btnSound.IsChecked = false;
            this.btnSound.Location = new System.Drawing.Point(247, 6);
            this.btnSound.Name = "btnSound";
            this.btnSound.Normal = global::TerminalCommunication.Properties.Resources.icon_close_sound_normal;
            this.btnSound.Pressed = global::TerminalCommunication.Properties.Resources.icon_close_sound_normal;
            this.btnSound.Size = new System.Drawing.Size(20, 20);
            this.btnSound.TabIndex = 1;
            // 
            // btnLock
            // 
            this.btnLock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLock.BackColor = System.Drawing.Color.Transparent;
            this.btnLock.BackgroundImage = global::TerminalCommunication.Properties.Resources.icon_fixed_hover;
            this.btnLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLock.CheckedHover = global::TerminalCommunication.Properties.Resources.icon_fixed_hover;
            this.btnLock.CheckedNormal = global::TerminalCommunication.Properties.Resources.icon_fixed_hover;
            this.btnLock.CheckedPressed = global::TerminalCommunication.Properties.Resources.icon_fixed_normal;
            this.btnLock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLock.Hover = global::TerminalCommunication.Properties.Resources.icon_notfixed_hover;
            this.btnLock.IsChecked = true;
            this.btnLock.Location = new System.Drawing.Point(6, 6);
            this.btnLock.Name = "btnLock";
            this.btnLock.Normal = global::TerminalCommunication.Properties.Resources.icon_notfixed_normal;
            this.btnLock.Pressed = global::TerminalCommunication.Properties.Resources.icon_notfixed_normal;
            this.btnLock.Size = new System.Drawing.Size(20, 20);
            this.btnLock.TabIndex = 0;
            this.btnLock.TabStop = false;
            this.btnLock.Click += new System.EventHandler(this.btnLock_Click);
            // 
            // picFlowState
            // 
            this.picFlowState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picFlowState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picFlowState.Image = global::TerminalCommunication.Properties.Resources.flownone;
            this.picFlowState.ImageLocation = "";
            this.picFlowState.Location = new System.Drawing.Point(180, 20);
            this.picFlowState.Margin = new System.Windows.Forms.Padding(0);
            this.picFlowState.Name = "picFlowState";
            this.picFlowState.Size = new System.Drawing.Size(12, 12);
            this.picFlowState.TabIndex = 9;
            this.picFlowState.TabStop = false;
            // 
            // Operator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.picResize);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.pnlControl);
            this.DoubleBuffered = true;
            this.Name = "Operator";
            this.Size = new System.Drawing.Size(540, 380);
            ((System.ComponentModel.ISupportInitialize)(this.picResize)).EndInit();
            this.pnlControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picFlowState)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedPanel pnlControl;
        private ImageToggleButton btnLock;
        private ImageToggleButton btnEnd;
        private ImageToggleButton btnMode;
        private ImageToggleButton btnFullScreen;
        private ImageToggleButton btnSound;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Timer tmrBkg;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.Label lblFlow;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.PictureBox picResize;
        private System.Windows.Forms.PictureBox picFlowState;
    }
}
