using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerminalCommunication
{
    internal sealed class ImageToggleButton : UserControl
    {
        public ImageToggleButton()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ImageToggleButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "ImageToggleButton";
            this.ResumeLayout(false);
        }

        private Image normal;
        private Image hover;
        private Image pressed;
        private Image checkedNormal;
        private Image checkedHover;
        private Image checkedPressed;
        private bool isChecked;
        private Status state = 0;

        private Image Image { get; set; }

        public Image Normal
        {
            get { return normal; }
            set
            {
                if(normal != value)
                {
                    normal = value;
                    ResetImage();
                }
            }
        }
        public Image Hover
        {
            get { return hover; }
            set
            {
                if (hover != value)
                {
                    hover = value;
                    ResetImage();
                }
            }
        }
        public Image Pressed
        {
            get { return pressed; }
            set
            {
                if (pressed != value)
                {
                    pressed = value;
                    ResetImage();
                }
            }
        }
        public Image CheckedNormal
        {
            get { return checkedNormal; }
            set
            {
                if (checkedNormal != value)
                {
                    checkedNormal = value;
                    ResetImage();
                }
            }
        }
        public Image CheckedHover
        {
            get { return checkedHover; }
            set
            {
                if (checkedHover != value)
                {
                    checkedHover = value;
                    ResetImage();
                }
            }
        }
        public Image CheckedPressed
        {
            get { return checkedPressed; }
            set
            {
                if (checkedPressed != value)
                {
                    checkedPressed = value;
                    ResetImage();
                }
            }
        }
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    if (isChecked)
                    {
                        State |= Status.Checked;
                    }
                    else
                    {
                        State &= ~Status.Checked;
                    }
                }
            }
        }
        private Status State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    ResetImage();
                }
            }
        }

        private void ResetImage()
        {
            switch(State)
            {
                case Status.Checked:
                    BackgroundImage = CheckedNormal;
                    break;
                case Status.Checked | Status.Hover:
                    BackgroundImage = CheckedHover;
                    break;
                case Status.Checked | Status.Pressed:
                case Status.Checked | Status.Hover | Status.Pressed:
                    BackgroundImage = CheckedPressed;
                    break;
                case Status.Pressed:
                case Status.Hover | Status.Pressed:
                    BackgroundImage = Pressed;
                    break;
                case Status.Hover:
                    BackgroundImage = Hover;
                    break;
                default:
                    BackgroundImage = Normal;
                    break;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            BackgroundImage = IsChecked ? CheckedNormal : Normal;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            State |= Status.Hover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            State &= ~(Status.Hover | Status.Pressed);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            State |= Status.Pressed;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            State &= ~Status.Pressed;
        }

        protected override void OnClick(EventArgs e)
        {
            IsChecked = !IsChecked;
            
            base.OnClick(e);
        }
        
        [Flags]
        private enum Status
        {
            Checked = 1,
            Hover = 2,
            Pressed = 4
        }
    }
}
