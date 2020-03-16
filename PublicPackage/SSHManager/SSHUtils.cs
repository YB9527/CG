using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SSHManager
{
    public class SSHUtils
    {
        private static SshClient sshClient;
        static void ReadLine(ShellStream stream)
        {
            //读取返回流
            string line;
            while ((line = stream.ReadLine(TimeSpan.FromSeconds(2))) != null)
            {
                Console.WriteLine(line);
            }

            //等待用户输入
            string input = Console.ReadLine();
            if (input == "exit")
            {
                return;//如果输入了exit结束程序
            }
            System.Console.WriteLine("用户输入:" + input);
            //执行命令
            stream.WriteLine(input);
            //再次等待用户输入
            ReadLine(stream);
        }

        /// <summary>
        /// 关闭ssh 流
        /// </summary>
        public static void CloseSSH()
        {
            if(sshClient != null  && sshClient.IsConnected)
            {
                sshClient.Disconnect();
            }
           
        }
        public static void CopyCommand()
        {            
            if(sshClient == null &&sshClient.IsConnected)
            {
                MessageBox.Show("没有打开链接！！！");
                return;
            }
            //普通执行，这个无法正常执行cd指令
            SshCommand cmd = sshClient.CreateCommand("ls ");
            string res =  cmd.Execute();
            Console.Write(res);
            cmd.Dispose();
        }

        public static void OpenSSH()
        {
            //创建一个SSH连接
            sshClient = new SshClient("139.9.4.108", 22, "root", "Mxy_19901012");
            try
            {
                sshClient.Connect();
            }
            catch (Exception err)
            {
                Console.Write("发生错误：" + err.Message);
                Console.ReadLine();
            }
            //连接成功后创建一个Shell输入流，用来执行命令
            if (sshClient.IsConnected)
            {
                Console.WriteLine(sshClient.ConnectionInfo.ServerVersion);
                //ShellStream stream = sshClient.CreateShellStream("dumb", 0, 0, 0, 0, 1000);
                //这里会在输入和执行之间无限切换
                // ReadLine(stream);
                //当用户执行了exit时关闭流
                //stream.Close();
            }
        }
    }

}