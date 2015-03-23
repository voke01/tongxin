using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class NetTcpException : Exception
    {
        internal const string a = "<Modulus>{0}<Modulus/>";
        internal const string b = "The message is too long";
        internal const string c = "Cross-border access to data";
        internal const string d = " IPAddress Error!";
        internal const string e = "{0} Type not found!";
        internal const string f = "{0} load data error!";
        internal const string g = "Connection is not available";
        internal const string h = "channel is disposed!";
        internal const string i = "channel receive data timeout!";
        internal const string j = "channel data process error!";
        internal const string k = "channel must be connected";
        internal const string l = "{0} config section not found!";
        internal const string m = "{0} client pool config section not found!";
        internal const int n = 10001;
        internal const int o = 10004;
        internal const int p = 10006;
        [CompilerGenerated]
        private int q;
        public int ErrorNumber
        {
            internal get;
            set;
        }
        public NetTcpException()
        {
        }
        public NetTcpException(string msg)
            : base(msg)
        {
        }
        public NetTcpException(Exception innererr, string msg)
            : base(msg, innererr)
        {
        }
        public static NetTcpException ConfigSelectionNotFound(string name)
        {
            return new NetTcpException(string.Format("{0} config section not found!", name));
        }
        public static NetTcpException ConnectionIsNotAvailable()
        {
            return new NetTcpException("Connection is not available");
        }
        public static NetTcpException ObjectLoadError(string type, Exception innerexception)
        {
            return new NetTcpException(innerexception, string.Format("{0} load data error!", type));
        }
        public static NetTcpException ReadToByteArraySegment()
        {
            return new NetTcpException("read to ByteArraySegment error!");
        }
        public static NetTcpException ClientDataProcessError(Exception innerexception)
        {
            return new NetTcpException(innerexception, "channel data process error!");
        }
        public static NetTcpException ClientPoolSectionNotFound(string name)
        {
            return new NetTcpException(string.Format("{0} client pool config section not found!", name));
        }
        public static NetTcpException ClientReceiveTimeout()
        {
            return new NetTcpException("channel receive data timeout!");
        }
        public static NetTcpException ClientMustBeConnected()
        {
            return new NetTcpException("channel must be connected");
        }
        public static NetTcpException ClientIsDisposed()
        {
            return new NetTcpException("channel is disposed!");
        }
        public static NetTcpException StringEncodingError(Exception innerexception)
        {
            return new NetTcpException(innerexception, "string encoding error!");
        }
        public static NetTcpException StringDecodingError(Exception innerexception)
        {
            return new NetTcpException(innerexception, "string decoding error!");
        }
        public static NetTcpException NotInitialize()
        {
            return new NetTcpException("Beetle component did not initialize!\r\nCall TcpUtil.Setup!");
        }
        public static NetTcpException PacketAnyalysisNotInitialize()
        {
            return new NetTcpException("Beetle Anyalysis  did not initialize!\r\nCall PacketAnalysis.Setup!");
        }
        public static NetTcpException TypeNotFound(string name)
        {
            return new NetTcpException(string.Format("{0} Type not found!", name));
        }
        public static NetTcpException DataOverflow()
        {
            return new NetTcpException("The message is too long")
            {
                ErrorNumber = 10001
            };
        }
        public static NetTcpException ReadDataError(Exception innererror)
        {
            return new NetTcpException(innererror, "Cross-border access to data")
            {
                ErrorNumber = 10004
            };
        }
        public static NetTcpException IPAddressError()
        {
            return new NetTcpException(" IPAddress Error!")
            {
                ErrorNumber = 10006
            };
        }
    }
}
