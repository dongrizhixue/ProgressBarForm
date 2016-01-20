using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 窗体上的进度条
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        //线程开始的时候调用的委托
        private delegate void maxValueDelegate(int maxValue);
        //线程执行中调用的委托
        private delegate void nowValueDelegate(int nowValue);
        private delegate void 实时信息委托(实时信息 msg);
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            ThreadMethod method = new ThreadMethod();
            //先订阅一下事件
            method.threadStartEvent += new EventHandler(method_threadStartEvent);
            method.threadEvent += new EventHandler(method_threadEvent);
            method.threadEndEvent += new EventHandler(method_threadEndEvent);
            method.thread实时信息 += new EventHandler(method_thread实时信息);
            Thread thread = new Thread(new ThreadStart(method.runMethod));
            thread.Start();
        }

        private void method_thread实时信息(object sender, EventArgs e)
        {

            实时信息 msg = (实时信息)sender;
            实时信息委托 now = new 实时信息委托(实时信息显示方法);
            Invoke(now, msg);

        }

        private void 实时信息显示方法(实时信息 msg)
        {
            label3.Text = msg.sum.ToString();
            label4.Text = msg.current.ToString();
        }

        /// <summary>
        /// 线程完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void method_threadEndEvent(object sender, EventArgs e)
        {
            MessageBox.Show("我完成啦!");

        }
        /// <summary>
        /// 线程执行中的事件,设置进度条当前进度
        /// 但是我不能直接操作进度条,需要一个委托来替我完成
        /// </summary>
        /// <param name="sender">ThreadMethod函数中传过来的当前值</param>
        /// <param name="e"></param>
        void method_threadEvent(object sender, EventArgs e)
        {
            int nowValue = Convert.ToInt32(sender);
            nowValueDelegate now = new nowValueDelegate(setNow);
            Invoke(now, nowValue);
        }
        /// <summary>
        /// 线程开始事件,设置进度条最大值
        /// 但是我不能直接操作进度条,需要一个委托来替我完成
        /// </summary>
        /// <param name="sender">ThreadMethod函数中传过来的最大值</param>
        /// <param name="e"></param>
        void method_threadStartEvent(object sender, EventArgs e)
        {
            int maxValue = Convert.ToInt32(sender);
            maxValueDelegate max = new maxValueDelegate(setMax);
            Invoke(max, maxValue);
        }
        /// <summary>
        /// 我被委托调用,专门设置进度条最大值的
        /// </summary>
        /// <param name="maxValue"></param>
        private void setMax(int maxValue)
        {
            progressBar1.Maximum = maxValue;
            label1.Text = maxValue.ToString();
        }

        /// <summary>
        /// 我被委托调用,专门设置进度条当前值的
        /// </summary>
        /// <param name="nowValue"></param>
        private void setNow(int nowValue)
        {
            progressBar1.Value = nowValue;
            label2.Text = nowValue.ToString();
            if (progressBar1.Value == progressBar1.Maximum - 1)
            {
                progressBar1.Visible = false;
            }
        }
    }
    /// <summary>
    /// 线程处理方法
    /// </summary>
    public class ThreadMethod
    {

        /// <summary>
        /// 线程开始事件
        /// </summary>
        public event EventHandler threadStartEvent;
        /// <summary>
        /// 线程执行时事件
        /// </summary>
        public event EventHandler threadEvent;
        /// <summary>
        /// 线程结束事件
        /// </summary>
        public event EventHandler threadEndEvent;

        public event EventHandler thread实时信息;

        public void runMethod()
        {
            int cont = 100000;      //执行多少次
            threadStartEvent.Invoke(100, new EventArgs());//通知主界面,我开始了,count用来设置进度条的最大值
            double t1 = 0, t2 = 0;
            int jindu = 0, temp = 0;
            实时信息 msg = new 实时信息();
            for (int i = 0; i < cont; i++)
            {
                msg.current = i + 1;
                msg.sum = cont;
                thread实时信息.Invoke(msg, new EventArgs());
                t1 = i;
                t2 = cont;
                if (t1 > 0)
                {
                    jindu = (int)(t1 / t2 * 100);
                }
                if (jindu == temp + 1)
                {
                    threadEvent.Invoke(jindu, new EventArgs());//通知主界面我正在执行,i表示进度条当前进度
                    Thread.Sleep(100);//休息100毫秒,模拟执行大数据量操作
                    temp = jindu;
                }

            }
            threadEndEvent.Invoke(new object(), new EventArgs());//通知主界面我已经完成了
        }
    }

    public class 实时信息
    {
        public int current = 0;
        public int sum = 0;
    }
}
