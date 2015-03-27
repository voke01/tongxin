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
        public static int ReceiveBufferLength = 1024;
        public static int maxConnect = 10000;
        static void Main(string[] args)
        {
            initConfig conf = new initConfig(maxConnect, ReceiveBufferLength);
            ServerBase server = new ServerBase();
            server.Open(8088);

            Console.ReadKey();
        }

    }
    //初始化配置
    public class initConfig
    {
        internal static initSocketAsyncQueue socketasync_queue;
        public static int Connections;
        public initConfig(int maxconnecttinos, int ReceiveBufferLength)
        {
            Connections = maxconnecttinos;
            socketasync_queue = new initSocketAsyncQueue(maxconnecttinos, ReceiveBufferLength);
        }

    }

    #region 初始化配置
    internal class initSocketAsyncQueue
    {
        private int _receviceBuffLength;
        protected Queue<saea> socketAsync_Queue;
        public initSocketAsyncQueue(int maxConnect, int receBuffLength)
        {
            this._receviceBuffLength = receBuffLength;
            this.socketAsync_Queue = new Queue<saea>(maxConnect);
            for (int i = 0; i < maxConnect; i++)
            {
                socketAsync_Queue.Enqueue(saeaEntry());
            }
        }

        protected saea saeaEntry()
        {
            saea _saea;
            if (this._receviceBuffLength > 0)
            {
                _saea = new saea(this._receviceBuffLength);
            }
            else
            {
                _saea = new saea();
            }
            return _saea;
        }

        public void saea_enq(saea e)
        {
            lock (this)
            {
                socketAsync_Queue.Enqueue(e);
                Console.WriteLine(socketAsync_Queue.Count);
            }
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <returns></returns>
        public virtual saea saea_dep()
        {
            saea result;
            lock (socketAsync_Queue)
            {
                saea pram;
                if (socketAsync_Queue.Count > 0)
                {
                    pram = socketAsync_Queue.Dequeue();
                }
                else
                {
                    pram = saeaEntry();
                }

                pram.resetBuff();
                result = pram;
            }
            Console.WriteLine(socketAsync_Queue.Count);
            return result;
        }
    }

    internal class saea : SocketAsyncEventArgs
    {
        public Socket _socket;
        //构造函数
        public saea(int receBuffLength)
        {
            byte[] array = new byte[receBuffLength];
            base.SetBuffer(array, 0, array.Length);
        }
        public saea() { }
        public void resetBuff()
        {
            if (base.Buffer != null)
            {
                base.SetBuffer(0, base.Buffer.Length);
            }
        }

        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);
            if (e.LastOperation == SocketAsyncOperation.Send)
            {

            }
            else if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                processReceive((saea)e);
            }
        }

        internal void processSend(saea e)
        {

        }

        internal void processReceive(saea e)
        {
            try
            {
                //aq aq = e._aq;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    Console.WriteLine(e.BytesTransferred);



                    if (!e._socket.ReceiveAsync(e))
                    {
                        Console.WriteLine("------------------------------------------------------------------");
                        //同步接收完成处理事件
                        processReceive(e);
                    }
                }
                else
                {
                    ReleaseSocketAsync(e);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        void ReleaseSocketAsync(saea e)
        {
            if (e._socket != null)
            {
                Console.WriteLine("A client  {0}  Release", e._socket.RemoteEndPoint);
                try
                {
                    if (e._socket != null)
                    {
                        e._socket.Shutdown(SocketShutdown.Both);
                    }
                }
                catch
                {
                }
                try
                {
                    if (e._socket != null)
                    {
                        e._socket.Close();
                    }
                }
                catch
                {
                }
                initConfig.socketasync_queue.saea_enq(e);


            }
        }

    }
    #endregion

    public class ServerBase
    {
        public int Listens
        {
            get;
            set;
        }
        private TcpServer tcpserver;
        public ServerBase()
        {
            this.Listens = 50;
        }
        
        public void Open(int port)
        {
            this.Open(new IPEndPoint(IPAddress.Any, port));
        }

        public void Open(IPEndPoint ep)
        {
            this.tcpserver = new TcpServer();

            //程序启动
            this.tcpserver.start(ep, this.Listens);
        }

    }

    public class TcpServer
    {
        private Socket listenSocket;
        private static int m_numConnectedSockets;

        private Queue<Socket> Socket_accpet_queue = new Queue<Socket>(1024);
        private Socket _socket;

        public static ObjectPool<SocketAsyncEventArgs> SocketAsyncEventArgsPools = new ObjectPool<SocketAsyncEventArgs>(1024);

        /// <summary>
        /// 程序开始
        /// </summary>
        /// <param name="ipendpoint"></param>
        /// <param name="listens"></param>
        public void start(IPEndPoint ipendpoint, int listens)
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(ipendpoint);
            listenSocket.Listen(listens);

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.handle_SocketQueue));
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.startAccept));

            Console.WriteLine("服务已启动...");
        }

        //--------------------------------------------------------------
        // 对SocketQueue队列里的Socket进行处理
        //
        //
        //-------------------------------------------------------------- 
        private void handle_SocketQueue(object obj)
        {
            while (true)
            {
                try
                {
                    this._socket = this.PopQueue();
                    if (_socket != null)
                    {
                        handle_Socket(_socket);
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }

        private void handle_Socket(Socket se)
        {
            //aq aq = new aq(se._socket);
            //this.OnConnected(aq);
            //取出队列
            saea _socketasync = initConfig.socketasync_queue.saea_dep();

            _socketasync._socket = se;
            try
            {
                if (!_socketasync._socket.ReceiveAsync(_socketasync))
                {
                    //同步接收完成处理事件
                    _socketasync.processReceive(_socketasync);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }




        //--------------------------------------------------------------------------------
        //开始接收连接，【SocketAsyncEventArgs有连接时，通过Complete事件回调】
        //
        //--------------------------------------------------------------------------------
        private void startAccept(object obj)
        {
            try
            {
                bool is_asyncEvent = true;
                while (is_asyncEvent)
                {
                    SocketAsyncEventArgs acceptEventArg = SocketAsyncEventArgsPools.Pop();
                    acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);

                    is_asyncEvent = !listenSocket.AcceptAsync(acceptEventArg);
                    //同步完成
                    if (is_asyncEvent)
                    {
                        ProcessAccept(acceptEventArg);
                        Console.WriteLine("同步---------------------------------------------------");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        //接受连接响应事件
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
            //投递下次请求
            startAccept(null);
        }

        //接收连接处理事件，成功时加入队列，
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    Interlocked.Increment(ref m_numConnectedSockets);
                    Console.WriteLine("{1} connect. {0} clients lastoptin {2}. socketError is {3}",
                        m_numConnectedSockets, e.AcceptSocket.RemoteEndPoint, e.LastOperation.ToString(), e.SocketError);

                    //加入队列
                    addQueue(e.AcceptSocket);
                }
                else
                {
                    //连接失败，释放socket
                    ReleaseSocket(e.AcceptSocket);
                }

                e.AcceptSocket = null;//释放
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                e.Completed -= new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
                SocketAsyncEventArgsPools.Push(e);
            }
        }

        //加入队列
        private void addQueue(Socket qe)
        {
            lock (Socket_accpet_queue)
            {
                Socket_accpet_queue.Enqueue(qe);
            }
        }

        //出队列
        private Socket PopQueue()
        {
            Socket se;
            lock (Socket_accpet_queue)
            {
                if (this.Socket_accpet_queue.Count > 0)
                {
                    se = Socket_accpet_queue.Dequeue();
                }
                else
                {
                    se = null;
                }
            }
            return se;
        }

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


    }
    //
    //internal class SocketEntry
    //{
    //    public ByteArraySegment a = new ByteArraySegment();
    //    public Socket _socket;
    //    //internal static string c = "V/r2I2CpRaI=";
    //}


}
