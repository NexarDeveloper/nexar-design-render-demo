using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Terpsichore.AddIn.PnP.App
{
    public class AvatarPictureBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(0, 0, Width - 1, Height - 1);
                Region = new Region(gp);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(new Pen(new SolidBrush(BackColor), 1), 0, 0, Width - 1, Height - 1);
            }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // AvatarPictureBox
            // 
            this.ErrorImage = global::Nexar.Renderer.Properties.Resources.no_avatar;
            this.InitialImage = global::Nexar.Renderer.Properties.Resources.no_avatar;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
