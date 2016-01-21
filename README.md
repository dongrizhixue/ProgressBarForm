# 窗体上的多线程进度条
这是一个在使用多线程在控制台窗体上弹出进度条窗体的项目，并计划完善
界面上2个控件,一个按钮和一个进度条
using System;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication2
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

        private void button1_Click(object sender, EventArgs e)
        {
            ThreadMethod method = new ThreadMethod();
            //先订阅一下事件
            method.threadStartEvent += new EventHandler(method_threadStartEvent);
            method.threadEvent += new EventHandler(method_threadEvent);
            method.threadEndEvent += new EventHandler(method_threadEndEvent);

            Thread thread = new Thread(new ThreadStart(method.runMethod));
            thread.Start();
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
            this.Invoke(now, nowValue);
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
            this.Invoke(max, maxValue);
        }

        /// <summary>
        /// 我被委托调用,专门设置进度条最大值的
        /// </summary>
        /// <param name="maxValue"></param>
        private void setMax(int maxValue)
        {
            this.progressBar1.Maximum = maxValue;
        }

        /// <summary>
        /// 我被委托调用,专门设置进度条当前值的
        /// </summary>
        /// <param name="nowValue"></param>
        private void setNow(int nowValue)
        {
            this.progressBar1.Value = nowValue;
        }
    }
}

另外一个文件

using System;
using System.Threading;

namespace WindowsFormsApplication2
{
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

        public void runMethod()
        {
            int count = 100;      //执行多少次
            threadStartEvent.Invoke(count, new EventArgs());//通知主界面,我开始了,count用来设置进度条的最大值
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(100);//休息100毫秒,模拟执行大数据量操作
                threadEvent.Invoke(i, new EventArgs());//通知主界面我正在执行,i表示进度条当前进度
            }
            threadEndEvent.Invoke(new object(), new EventArgs());//通知主界面我已经完成了
        }
    }
}
