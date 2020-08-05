using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DIO_Library;

namespace D7250_DIO
{
    public partial class Form1 : Form
    {
        public short m_dev;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_dev = DASK.Register_Card(DASK.PCI_7432, 0);/* เปิดระบบ I/O */
            if (m_dev < 0)
            {
                MessageBox.Show("Register_Card error!");

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            short ret;
            if (m_dev >= 0)
            {
                ret = DASK.Release_Card((ushort)m_dev); /* ปิดระบบ I/O */
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            short ret;
            int out_value;            
            if (Int32.TryParse(textBox1.Text, out out_value))            
            {
                //ret = DASK.DO_WritePort((ushort)m_dev, 0, (uint)out_value); /* OUTPUT */
                ret = DASK.DO_WriteLine((ushort)m_dev, 0, 4, (ushort)out_value); /* OUTPUT */
                if (ret < 0)
                {
                    MessageBox.Show("DO_WritePort error!");
                }
            }
            else
            {
                MessageBox.Show("Input error!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            short ret;
            uint int_value;
            ushort s_value;
            //ret = DASK.DI_ReadPort((ushort)m_dev, 0, out int_value); /* INPUT */
            ret = DASK.DI_ReadLine((ushort)m_dev, 0, 0, out s_value); /* INPUT */
            if (ret < 0)
            {
                MessageBox.Show("D2K_DI_ReadPort error!");
                return;
            }
            //textBox2.Text = String.Format("{0}", int_value);
            textBox2.Text = String.Format("{0}", s_value);
        }
        
        /* ทดสอบ Rotary Library 
         */

        private void btn90_Click(object sender, EventArgs e)
        {
        }

        private void btn180_Click(object sender, EventArgs e)
        {
        }

        private void btn270_Click(object sender, EventArgs e)
        {
        }

        private void btnZero_Click(object sender, EventArgs e)
        { }

    }
}