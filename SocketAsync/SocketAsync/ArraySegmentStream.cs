using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class ArraySegmentStream : Stream
    {
        private byte[] a;
        private int b;
        private int c;
        private int d;
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        public override long Length
        {
            get
            {
                return (long)this.d;
            }
        }
        public override long Position
        {
            get
            {
                return (long)this.c;
            }
            set
            {
                this.c = (int)value;
            }
        }
        public ArraySegmentStream(byte[] data)
        {
            this.a = data;
            this.d = 0;
            this.c = 0;
            this.b = 0;
        }
        public override void Flush()
        {
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = this.d - this.c;
            if (num > count)
            {
                num = count;
            }
            if (num <= 0)
            {
                return 0;
            }
            if (num <= 8)
            {
                int num2 = num;
                while (--num2 >= 0)
                {
                    buffer[offset + num2] = this.a[this.c + num2];
                }
            }
            else
            {
                Buffer.BlockCopy(this.a, this.c, buffer, offset, num);
            }
            this.c += num;
            return num;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        int num = this.b + (int)offset;
                        this.c = num;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        int num2 = this.c + (int)offset;
                        this.c = num2;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        int num3 = this.d + (int)offset;
                        this.c = num3;
                        break;
                    }
            }
            return (long)this.c;
        }
        public override void SetLength(long value)
        {
            int num = this.b + (int)value;
            this.d = num;
            if (this.c > num)
            {
                this.c = num;
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            int num = this.c + count;
            if (num > this.d)
            {
                this.d = num;
            }
            if (count <= 8 && buffer != this.a)
            {
                int num2 = count;
                while (--num2 >= 0)
                {
                    this.a[this.c + num2] = buffer[offset + num2];
                }
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, this.a, this.c, count);
            }
            this.c = num;
        }
    }
}
