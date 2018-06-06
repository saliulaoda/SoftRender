using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace SoftRender_Windows
{
    public partial class FormMain : Form
    {
        Device mDevice;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            mDevice = new Device();
            DrawCall();
        }

        public void DrawCall()
        {
            Bitmap _tmp = mDevice.GetFrameBuffer();
            pictureBox1.Image = (Image)_tmp;
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            vector_t _newRotate = new vector_t(mDevice.mRotateVector.x, mDevice.mRotateVector.y, Tools.CMID_F(mDevice.mRotateVector.z + 0.1f, -1f, 1f), mDevice.mRotateVector.w);
            mDevice.mRotateVector = _newRotate;
            mDevice.ResetFrameBuffer();
            mDevice.DrawBox();
            DrawCall();
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            vector_t _newRotate = new vector_t(mDevice.mRotateVector.x, mDevice.mRotateVector.y, Tools.CMID_F(mDevice.mRotateVector.z - 0.1f, -1f, 1f), mDevice.mRotateVector.w);
            mDevice.mRotateVector = _newRotate;
            mDevice.ResetFrameBuffer();
            mDevice.DrawBox();
            DrawCall();
        }
    }
}
