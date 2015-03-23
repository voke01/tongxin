using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            start();
            Console.ReadKey();
        }
        static Socket listenSocket;
        static int m_numConnectedSockets;
        static ObjectPool<SocketAsyncEventArgs> SocketAsyncEventArgsPools = new ObjectPool<SocketAsyncEventArgs>(1024);

        static void start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 9900);
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            listenSocket.Listen(10);
            startAccept(null);
            Console.WriteLine("服务已启动...");
        }

        //开始接收连接，SocketAsyncEventArgs有连接时，通过Complete事件回调
        static void startAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {
                if (acceptEventArg == null)
                {
                    acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
                }
                else
                {
                    acceptEventArg.AcceptSocket = null;//释放，投递下次请求
                }

                //如果IO操作被挂起，返回true, 操作完成时，将引发 e 参数的 SocketAsyncEventArgs::Completed 事件。
                //如果IO操作同步完成，返回false，在这种情况下，
                //将不会引发 e 参数的 SocketAsyncEventArgs::Completed 事件，并且可能在方法调用返回后立即检查作为参数传递的 e 对象以检索操作的结果
                bool is_asyncEvent = listenSocket.AcceptAsync(acceptEventArg);
                //同步完成
                if (!is_asyncEvent)
                {
                    ProcessAccept(acceptEventArg);

                    //把当前异步事件释放，等待下次连接  
                    startAccept(acceptEventArg);
                    Console.WriteLine("同步");
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        //接受连接响应事件
        static void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);

            //把当前异步事件释放，等待下次连接  
            startAccept(e);
        }

        //接收连接处理
        static void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    Interlocked.Increment(ref m_numConnectedSockets);
                    Console.WriteLine("{1} connect. {0} clients lastoptin {2}. socketError is {3}",
                        m_numConnectedSockets, e.AcceptSocket.RemoteEndPoint, e.LastOperation.ToString(), e.SocketError);

                    //加入队列
                    addQueue(new QueueEntry
                    {
                        _socket = e.AcceptSocket
                    });
                }
                else
                {
                    //连接失败，释放socket
                    ReleaseSocket(e.AcceptSocket);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        #region 加入队列
        private static Queue<QueueEntry> qentry = new Queue<QueueEntry>(1024);

        private static void addQueue(QueueEntry qe)
        {
            lock (qentry)
            {
                qentry.Enqueue(qe);
            }
        }

        #endregion

        //释放socket
        static void ReleaseSocket(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    if (socket != null)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                }
                catch
                {
                }
                try
                {
                    if (socket != null)
                    {
                        socket.Close();
                    }
                }
                catch
                {
                }
            }
        }

        /**end*/
    }

    internal class QueueEntry
    {
        //public ByteArraySegment a = new ByteArraySegment();
        public Socket _socket;
        //internal static string c = "V/r2I2CpRaI=";
    }
}
