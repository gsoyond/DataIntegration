using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataIntegration.xml;

namespace DataIntegration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //设置应用程序如何响应未处理的异常
                //设置捕获未处理的异常   
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常   
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常   
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Global.logMutex.WaitOne();
                Global.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                Global.logMutex.ReleaseMutex();

                //系统异常出现时，记录各个数据库上一次同步结束的时间
                TXML.UpdateLastTime(Global.pptnLastTime.ToString(), Global.riverLastTime.ToString(), Global.rvsrLastTime.ToString(), Global.tideLastTime.ToString());

            }
        }


        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format("出现应用程序未处理的异常\n异常类型：{0}\n异常消息：{1}\n异常位置：{2}\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }
            Global.logMutex.WaitOne();
            Global.Log(str);
            Global.logMutex.ReleaseMutex();

            //MessageBox.Show(str, "应用程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            if (error != null)
            {
                str = string.Format("Application UnhandledException:{0};\n堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }
            Global.logMutex.WaitOne();
            Global.Log(str);
            Global.logMutex.ReleaseMutex();
            //MessageBox.Show(str, "应用程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
