using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class ByteArraySegment
    {
        private int a;
        private Stream b;
        internal byte[] c;
        public int Offset;
        public int Count;
        public byte[] Array
        {
            get
            {
                return this.c;
            }
            private set
            {
                this.c = value;
            }
        }
        public int BufferLength
        {
            get
            {
                if (this.Array == null)
                {
                    return 0;
                }
                return this.Array.Length;
            }
        }
        public ByteArraySegment()
        {
        }
        public byte GetData(int index)
        {
            return this.c[this.Offset + index];
        }
        public ByteArraySegment(int length)
        {
            this.Array = new byte[length];
        }
        public void Encoding(string value, Encoding coding)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.SetInfo(0, 0);
                }
                else
                {
                    int bytes = coding.GetBytes(value, 0, value.Length, this.Array, 0);
                    this.SetInfo(0, bytes);
                }
            }
            catch (Exception innerexception)
            {
                throw NetTcpException.StringEncodingError(innerexception);
            }
        }
        public void Import(ByteArraySegment e)
        {
            this.Import(e.Array, e.Offset, e.Count);
        }
        public void Import(Stream steram)
        {
            int count = steram.Read(this.Array, 0, (int)steram.Length);
            this.SetInfo(0, count);
            this.SetPostion(0);
        }
        public void Import(byte[] data, int offet, int count)
        {
            Buffer.BlockCopy(data, offet, this.Array, 0, count);
            this.SetInfo(0, count);
            this.SetPostion(0);
        }
        public string Decoding(Encoding coding)
        {
            if (this.Count == 0)
            {
                return null;
            }
            return coding.GetString(this.c, this.Offset, this.Count);
        }
        public string Decoding(Encoding coding, byte[] data, int poffset, int pcount)
        {
            return coding.GetString(data, poffset, pcount);
        }
        public string ToBase64String(byte[] data, int poffset, int pcount)
        {
            return Convert.ToBase64String(data, poffset, pcount);
        }
        public string ToBase64String()
        {
            return Convert.ToBase64String(this.c, this.Offset, this.Count);
        }
        public void FromBase64String(string value)
        {
            try
            {
                byte[] array = Convert.FromBase64String(value);
                this.Import(array, 0, array.Length);
            }
            catch (Exception innerexception)
            {
                throw NetTcpException.StringEncodingError(innerexception);
            }
        }
        public Stream GetStream()
        {
            if (this.b == null)
            {
                this.b = new ArraySegmentStream(this.c);
            }
            return this.b;
        }
        public void SetInfo(int offset, int count)
        {
            this.Offset = offset;
            this.Count = count;
        }
        public void SetPostion(int value)
        {
            this.a = value;
        }
        public void SetInfo(byte[] data, int offset, int count)
        {
            this.Array = data;
            this.Offset = offset;
            this.Count = count;
        }
        public void Clear()
        {
            this.Array = null;
        }
        public void EncryptTo(ByteArraySegment segment, DESCryptoServiceProvider mDESProvider)
        {
            MemoryStream memoryStream = new MemoryStream(segment.c);
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, mDESProvider.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(this.c, this.Offset, this.Count);
                cryptoStream.FlushFinalBlock();
                segment.SetInfo(0, (int)memoryStream.Position);
                cryptoStream.Close();
            }
        }
        public void DecryptTo(ByteArraySegment segment, DESCryptoServiceProvider mDESProvider)
        {
            MemoryStream stream = new MemoryStream(this.c, this.Offset, this.Count);
            int num = 0;
            using (CryptoStream cryptoStream = new CryptoStream(stream, mDESProvider.CreateDecryptor(), CryptoStreamMode.Read))
            {
                for (int i = (int)((byte)cryptoStream.ReadByte()); i >= 0; i = cryptoStream.ReadByte())
                {
                    segment.c[num] = (byte)i;
                    num++;
                }
                segment.SetInfo(0, num);
            }
        }
    }
}
