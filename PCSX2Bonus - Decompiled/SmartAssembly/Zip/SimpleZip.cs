namespace SmartAssembly.Zip
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    public static class SimpleZip
    {
        public static string ExceptionMessage;

        private static ICryptoTransform GetAesTransform(byte[] key, byte[] iv, bool decrypt)
        {
            using (SymmetricAlgorithm algorithm = new RijndaelManaged())
            {
                return (decrypt ? algorithm.CreateDecryptor(key, iv) : algorithm.CreateEncryptor(key, iv));
            }
        }

        private static ICryptoTransform GetDesTransform(byte[] key, byte[] iv, bool decrypt)
        {
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                return (decrypt ? provider.CreateDecryptor(key, iv) : provider.CreateEncryptor(key, iv));
            }
        }

        private static bool PublicKeysMatch(Assembly executingAssembly, Assembly callingAssembly)
        {
            byte[] publicKey = executingAssembly.GetName().GetPublicKey();
            byte[] buffer2 = callingAssembly.GetName().GetPublicKey();
            if ((buffer2 == null) != (publicKey == null))
            {
                return false;
            }
            if (buffer2 != null)
            {
                for (int i = 0; i < buffer2.Length; i++)
                {
                    if (buffer2[i] != publicKey[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static byte[] Unzip(byte[] buffer)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            if ((callingAssembly != executingAssembly) && !PublicKeysMatch(executingAssembly, callingAssembly))
            {
                return null;
            }
            ZipStream stream = new ZipStream(buffer);
            byte[] buf = new byte[0];
            int num = stream.ReadInt();
            if (num == 0x4034b50)
            {
                short num2 = (short) stream.ReadShort();
                int num3 = stream.ReadShort();
                int num4 = stream.ReadShort();
                if (((num != 0x4034b50) || (num2 != 20)) || ((num3 != 0) || (num4 != 8)))
                {
                    throw new FormatException("Wrong Header Signature");
                }
                stream.ReadInt();
                stream.ReadInt();
                stream.ReadInt();
                int num5 = stream.ReadInt();
                int count = stream.ReadShort();
                int num7 = stream.ReadShort();
                if (count > 0)
                {
                    byte[] buffer3 = new byte[count];
                    stream.Read(buffer3, 0, count);
                }
                if (num7 > 0)
                {
                    byte[] buffer4 = new byte[num7];
                    stream.Read(buffer4, 0, num7);
                }
                byte[] buffer5 = new byte[stream.Length - stream.Position];
                stream.Read(buffer5, 0, buffer5.Length);
                Inflater inflater = new Inflater(buffer5);
                buf = new byte[num5];
                inflater.Inflate(buf, 0, buf.Length);
                buffer5 = null;
            }
            else
            {
                int num8 = num >> 0x18;
                num -= num8 << 0x18;
                if (num == 0x7d7a7b)
                {
                    switch (num8)
                    {
                        case 1:
                        {
                            int num12;
                            int num9 = stream.ReadInt();
                            buf = new byte[num9];
                            for (int i = 0; i < num9; i += num12)
                            {
                                int num11 = stream.ReadInt();
                                num12 = stream.ReadInt();
                                byte[] buffer6 = new byte[num11];
                                stream.Read(buffer6, 0, buffer6.Length);
                                new Inflater(buffer6).Inflate(buf, i, num12);
                            }
                            break;
                        }
                        case 2:
                        {
                            byte[] buffer7 = new byte[] { 0x94, 0xad, 0xc3, 0x85, 0xa5, 0x2a, 0xbd, 9 };
                            byte[] buffer8 = new byte[] { 0xbf, 0x45, 3, 0x1a, 0x41, 80, 14, 0xbf };
                            using (ICryptoTransform transform = GetDesTransform(buffer7, buffer8, true))
                            {
                                buf = Unzip(transform.TransformFinalBlock(buffer, 4, buffer.Length - 4));
                            }
                            break;
                        }
                    }
                    if (num8 != 3)
                    {
                        goto Label_026B;
                    }
                    byte[] key = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                    byte[] iv = new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                    using (ICryptoTransform transform2 = GetAesTransform(key, iv, true))
                    {
                        buf = Unzip(transform2.TransformFinalBlock(buffer, 4, buffer.Length - 4));
                        goto Label_026B;
                    }
                }
                throw new FormatException("Unknown Header");
            }
        Label_026B:
            stream.Close();
            stream = null;
            return buf;
        }

        public static byte[] Zip(byte[] buffer)
        {
            return Zip(buffer, 1, null, null);
        }

        private static byte[] Zip(byte[] buffer, int version, byte[] key, byte[] iv)
        {
            byte[] buffer12;
            try
            {
                ZipStream stream = new ZipStream();
                if (version == 0)
                {
                    Deflater deflater = new Deflater();
                    DateTime now = DateTime.Now;
                    long num = (long) ((ulong) ((((((((now.Year - 0x7bc) & 0x7f) << 0x19) | (now.Month << 0x15)) | (now.Day << 0x10)) | (now.Hour << 11)) | (now.Minute << 5)) | (now.Second >> 1)));
                    uint[] numArray = new uint[] { 
                        0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3, 0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 
                        0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5, 
                        0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59, 
                        0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 
                        0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01, 
                        0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65, 
                        0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 
                        0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f, 0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad, 
                        0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683, 0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1, 
                        0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 
                        0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79, 
                        0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d, 
                        0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21, 
                        0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45, 
                        0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9, 
                        0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
                     };
                    uint maxValue = uint.MaxValue;
                    uint num3 = maxValue;
                    int num4 = 0;
                    int length = buffer.Length;
                    while (--length >= 0)
                    {
                        num3 = numArray[(int) ((IntPtr) ((num3 ^ buffer[num4++]) & 0xff))] ^ (num3 >> 8);
                    }
                    num3 ^= maxValue;
                    stream.WriteInt(0x4034b50);
                    stream.WriteShort(20);
                    stream.WriteShort(0);
                    stream.WriteShort(8);
                    stream.WriteInt((int) num);
                    stream.WriteInt((int) num3);
                    long position = stream.Position;
                    stream.WriteInt(0);
                    stream.WriteInt(buffer.Length);
                    byte[] bytes = Encoding.UTF8.GetBytes("{data}");
                    stream.WriteShort(bytes.Length);
                    stream.WriteShort(0);
                    stream.Write(bytes, 0, bytes.Length);
                    deflater.SetInput(buffer);
                    while (!deflater.IsNeedingInput)
                    {
                        byte[] output = new byte[0x200];
                        int count = deflater.Deflate(output);
                        if (count <= 0)
                        {
                            break;
                        }
                        stream.Write(output, 0, count);
                    }
                    deflater.Finish();
                    while (!deflater.IsFinished)
                    {
                        byte[] buffer4 = new byte[0x200];
                        int num8 = deflater.Deflate(buffer4);
                        if (num8 <= 0)
                        {
                            break;
                        }
                        stream.Write(buffer4, 0, num8);
                    }
                    long totalOut = deflater.TotalOut;
                    stream.WriteInt(0x2014b50);
                    stream.WriteShort(20);
                    stream.WriteShort(20);
                    stream.WriteShort(0);
                    stream.WriteShort(8);
                    stream.WriteInt((int) num);
                    stream.WriteInt((int) num3);
                    stream.WriteInt((int) totalOut);
                    stream.WriteInt(buffer.Length);
                    stream.WriteShort(bytes.Length);
                    stream.WriteShort(0);
                    stream.WriteShort(0);
                    stream.WriteShort(0);
                    stream.WriteShort(0);
                    stream.WriteInt(0);
                    stream.WriteInt(0);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.WriteInt(0x6054b50);
                    stream.WriteShort(0);
                    stream.WriteShort(0);
                    stream.WriteShort(1);
                    stream.WriteShort(1);
                    stream.WriteInt(0x2e + bytes.Length);
                    stream.WriteInt((30 + bytes.Length) + ((int) totalOut));
                    stream.WriteShort(0);
                    stream.Seek(position, SeekOrigin.Begin);
                    stream.WriteInt((int) totalOut);
                }
                else if (version == 1)
                {
                    byte[] buffer5;
                    stream.WriteInt(0x17d7a7b);
                    stream.WriteInt(buffer.Length);
                    for (int i = 0; i < buffer.Length; i += buffer5.Length)
                    {
                        buffer5 = new byte[Math.Min(0x1fffff, buffer.Length - i)];
                        Buffer.BlockCopy(buffer, i, buffer5, 0, buffer5.Length);
                        long num11 = stream.Position;
                        stream.WriteInt(0);
                        stream.WriteInt(buffer5.Length);
                        Deflater deflater2 = new Deflater();
                        deflater2.SetInput(buffer5);
                        while (!deflater2.IsNeedingInput)
                        {
                            byte[] buffer6 = new byte[0x200];
                            int num12 = deflater2.Deflate(buffer6);
                            if (num12 <= 0)
                            {
                                break;
                            }
                            stream.Write(buffer6, 0, num12);
                        }
                        deflater2.Finish();
                        while (!deflater2.IsFinished)
                        {
                            byte[] buffer7 = new byte[0x200];
                            int num13 = deflater2.Deflate(buffer7);
                            if (num13 <= 0)
                            {
                                break;
                            }
                            stream.Write(buffer7, 0, num13);
                        }
                        long num14 = stream.Position;
                        stream.Position = num11;
                        stream.WriteInt((int) deflater2.TotalOut);
                        stream.Position = num14;
                    }
                }
                else
                {
                    if (version == 2)
                    {
                        stream.WriteInt(0x27d7a7b);
                        byte[] inputBuffer = Zip(buffer, 1, null, null);
                        using (ICryptoTransform transform = GetDesTransform(key, iv, false))
                        {
                            byte[] buffer9 = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                            stream.Write(buffer9, 0, buffer9.Length);
                            goto Label_044F;
                        }
                    }
                    if (version == 3)
                    {
                        stream.WriteInt(0x37d7a7b);
                        byte[] buffer10 = Zip(buffer, 1, null, null);
                        using (ICryptoTransform transform2 = GetAesTransform(key, iv, false))
                        {
                            byte[] buffer11 = transform2.TransformFinalBlock(buffer10, 0, buffer10.Length);
                            stream.Write(buffer11, 0, buffer11.Length);
                        }
                    }
                }
            Label_044F:
                stream.Flush();
                stream.Close();
                buffer12 = stream.ToArray();
            }
            catch (Exception exception)
            {
                ExceptionMessage = "ERR 2003: " + exception.Message;
                throw;
            }
            return buffer12;
        }

        public static byte[] ZipAndAES(byte[] buffer, byte[] key, byte[] iv)
        {
            return Zip(buffer, 3, key, iv);
        }

        public static byte[] ZipAndEncrypt(byte[] buffer, byte[] key, byte[] iv)
        {
            return Zip(buffer, 2, key, iv);
        }

        internal sealed class Deflater
        {
            private const int BUSY_STATE = 0x10;
            private SimpleZip.DeflaterEngine engine;
            private const int FINISHED_STATE = 30;
            private const int FINISHING_STATE = 0x1c;
            private const int FLUSHING_STATE = 20;
            private const int IS_FINISHING = 8;
            private const int IS_FLUSHING = 4;
            private SimpleZip.DeflaterPending pending = new SimpleZip.DeflaterPending();
            private int state = 0x10;
            private long totalOut;

            public Deflater()
            {
                this.engine = new SimpleZip.DeflaterEngine(this.pending);
            }

            public int Deflate(byte[] output)
            {
                int offset = 0;
                int length = output.Length;
                int num3 = length;
                while (true)
                {
                    int num4 = this.pending.Flush(output, offset, length);
                    offset += num4;
                    this.totalOut += num4;
                    length -= num4;
                    if ((length == 0) || (this.state == 30))
                    {
                        return (num3 - length);
                    }
                    if (!this.engine.Deflate((this.state & 4) != 0, (this.state & 8) != 0))
                    {
                        if (this.state == 0x10)
                        {
                            return (num3 - length);
                        }
                        if (this.state == 20)
                        {
                            for (int i = 8 + (-this.pending.BitCount & 7); i > 0; i -= 10)
                            {
                                this.pending.WriteBits(2, 10);
                            }
                            this.state = 0x10;
                        }
                        else if (this.state == 0x1c)
                        {
                            this.pending.AlignToByte();
                            this.state = 30;
                        }
                    }
                }
            }

            public void Finish()
            {
                this.state |= 12;
            }

            public void SetInput(byte[] buffer)
            {
                this.engine.SetInput(buffer);
            }

            public bool IsFinished
            {
                get
                {
                    return ((this.state == 30) && this.pending.IsFlushed);
                }
            }

            public bool IsNeedingInput
            {
                get
                {
                    return this.engine.NeedsInput();
                }
            }

            public long TotalOut
            {
                get
                {
                    return this.totalOut;
                }
            }
        }

        internal sealed class DeflaterEngine
        {
            private int blockStart;
            private const int HASH_MASK = 0x7fff;
            private const int HASH_SHIFT = 5;
            private const int HASH_SIZE = 0x8000;
            private short[] head;
            private SimpleZip.DeflaterHuffman huffman;
            private byte[] inputBuf;
            private int inputEnd;
            private int inputOff;
            private int ins_h;
            private int lookahead;
            private int matchLen;
            private int matchStart;
            private const int MAX_DIST = 0x7efa;
            private const int MAX_MATCH = 0x102;
            private const int MIN_LOOKAHEAD = 0x106;
            private const int MIN_MATCH = 3;
            private SimpleZip.DeflaterPending pending;
            private short[] prev;
            private bool prevAvailable;
            private int strstart;
            private const int TOO_FAR = 0x1000;
            private int totalIn;
            private byte[] window;
            private const int WMASK = 0x7fff;
            private const int WSIZE = 0x8000;

            public DeflaterEngine(SimpleZip.DeflaterPending pending)
            {
                this.pending = pending;
                this.huffman = new SimpleZip.DeflaterHuffman(pending);
                this.window = new byte[0x10000];
                this.head = new short[0x8000];
                this.prev = new short[0x8000];
                this.blockStart = this.strstart = 1;
            }

            public bool Deflate(bool flush, bool finish)
            {
                bool flag;
                do
                {
                    this.FillWindow();
                    bool flag2 = flush && (this.inputOff == this.inputEnd);
                    flag = this.DeflateSlow(flag2, finish);
                }
                while (this.pending.IsFlushed && flag);
                return flag;
            }

            private bool DeflateSlow(bool flush, bool finish)
            {
                if ((this.lookahead >= 0x106) || flush)
                {
                    while ((this.lookahead >= 0x106) || flush)
                    {
                        if (this.lookahead == 0)
                        {
                            if (this.prevAvailable)
                            {
                                this.huffman.TallyLit(this.window[this.strstart - 1] & 0xff);
                            }
                            this.prevAvailable = false;
                            this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, finish);
                            this.blockStart = this.strstart;
                            return false;
                        }
                        if (this.strstart >= 0xfefa)
                        {
                            this.SlideWindow();
                        }
                        int matchStart = this.matchStart;
                        int matchLen = this.matchLen;
                        if (this.lookahead >= 3)
                        {
                            int curMatch = this.InsertString();
                            if ((((curMatch != 0) && ((this.strstart - curMatch) <= 0x7efa)) && (this.FindLongestMatch(curMatch) && (this.matchLen <= 5))) && ((this.matchLen == 3) && ((this.strstart - this.matchStart) > 0x1000)))
                            {
                                this.matchLen = 2;
                            }
                        }
                        if ((matchLen >= 3) && (this.matchLen <= matchLen))
                        {
                            this.huffman.TallyDist((this.strstart - 1) - matchStart, matchLen);
                            matchLen -= 2;
                            do
                            {
                                this.strstart++;
                                this.lookahead--;
                                if (this.lookahead >= 3)
                                {
                                    this.InsertString();
                                }
                            }
                            while (--matchLen > 0);
                            this.strstart++;
                            this.lookahead--;
                            this.prevAvailable = false;
                            this.matchLen = 2;
                        }
                        else
                        {
                            if (this.prevAvailable)
                            {
                                this.huffman.TallyLit(this.window[this.strstart - 1] & 0xff);
                            }
                            this.prevAvailable = true;
                            this.strstart++;
                            this.lookahead--;
                        }
                        if (this.huffman.IsFull())
                        {
                            int storedLength = this.strstart - this.blockStart;
                            if (this.prevAvailable)
                            {
                                storedLength--;
                            }
                            bool lastBlock = (finish && (this.lookahead == 0)) && !this.prevAvailable;
                            this.huffman.FlushBlock(this.window, this.blockStart, storedLength, lastBlock);
                            this.blockStart += storedLength;
                            return !lastBlock;
                        }
                    }
                    return true;
                }
                return false;
            }

            public void FillWindow()
            {
                if (this.strstart >= 0xfefa)
                {
                    this.SlideWindow();
                }
                while ((this.lookahead < 0x106) && (this.inputOff < this.inputEnd))
                {
                    int length = (0x10000 - this.lookahead) - this.strstart;
                    if (length > (this.inputEnd - this.inputOff))
                    {
                        length = this.inputEnd - this.inputOff;
                    }
                    Array.Copy(this.inputBuf, this.inputOff, this.window, this.strstart + this.lookahead, length);
                    this.inputOff += length;
                    this.totalIn += length;
                    this.lookahead += length;
                }
                if (this.lookahead >= 3)
                {
                    this.UpdateHash();
                }
            }

            private bool FindLongestMatch(int curMatch)
            {
                int num = 0x80;
                int lookahead = 0x80;
                short[] prev = this.prev;
                int strstart = this.strstart;
                int index = this.strstart + this.matchLen;
                int num6 = Math.Max(this.matchLen, 2);
                int num7 = Math.Max(this.strstart - 0x7efa, 0);
                int num8 = (this.strstart + 0x102) - 1;
                byte num9 = this.window[index - 1];
                byte num10 = this.window[index];
                if (num6 >= 8)
                {
                    num = num >> 2;
                }
                if (lookahead > this.lookahead)
                {
                    lookahead = this.lookahead;
                }
                do
                {
                    if (((this.window[curMatch + num6] == num10) && (this.window[(curMatch + num6) - 1] == num9)) && ((this.window[curMatch] == this.window[strstart]) && (this.window[curMatch + 1] == this.window[strstart + 1])))
                    {
                        int num4 = curMatch + 2;
                        strstart += 2;
                        while ((((this.window[++strstart] == this.window[++num4]) && (this.window[++strstart] == this.window[++num4])) && ((this.window[++strstart] == this.window[++num4]) && (this.window[++strstart] == this.window[++num4]))) && (((this.window[++strstart] == this.window[++num4]) && (this.window[++strstart] == this.window[++num4])) && (((this.window[++strstart] == this.window[++num4]) && (this.window[++strstart] == this.window[++num4])) && (strstart < num8))))
                        {
                        }
                        if (strstart > index)
                        {
                            this.matchStart = curMatch;
                            index = strstart;
                            num6 = strstart - this.strstart;
                            if (num6 >= lookahead)
                            {
                                break;
                            }
                            num9 = this.window[index - 1];
                            num10 = this.window[index];
                        }
                        strstart = this.strstart;
                    }
                }
                while (((curMatch = prev[curMatch & 0x7fff] & 0xffff) > num7) && (--num != 0));
                this.matchLen = Math.Min(num6, this.lookahead);
                return (this.matchLen >= 3);
            }

            private int InsertString()
            {
                short num;
                int index = ((this.ins_h << 5) ^ this.window[this.strstart + 2]) & 0x7fff;
                this.prev[this.strstart & 0x7fff] = num = this.head[index];
                this.head[index] = (short) this.strstart;
                this.ins_h = index;
                return (num & 0xffff);
            }

            public bool NeedsInput()
            {
                return (this.inputEnd == this.inputOff);
            }

            public void SetInput(byte[] buffer)
            {
                this.inputBuf = buffer;
                this.inputOff = 0;
                this.inputEnd = buffer.Length;
            }

            private void SlideWindow()
            {
                Array.Copy(this.window, 0x8000, this.window, 0, 0x8000);
                this.matchStart -= 0x8000;
                this.strstart -= 0x8000;
                this.blockStart -= 0x8000;
                for (int i = 0; i < 0x8000; i++)
                {
                    int num2 = this.head[i] & 0xffff;
                    this.head[i] = (num2 >= 0x8000) ? ((short) (num2 - 0x8000)) : ((short) 0);
                }
                for (int j = 0; j < 0x8000; j++)
                {
                    int num4 = this.prev[j] & 0xffff;
                    this.prev[j] = (num4 >= 0x8000) ? ((short) (num4 - 0x8000)) : ((short) 0);
                }
            }

            private void UpdateHash()
            {
                this.ins_h = (this.window[this.strstart] << 5) ^ this.window[this.strstart + 1];
            }
        }

        internal sealed class DeflaterHuffman
        {
            private static readonly byte[] bit4Reverse = new byte[] { 0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15 };
            private const int BITLEN_NUM = 0x13;
            private static readonly int[] BL_ORDER = new int[] { 
                0x10, 0x11, 0x12, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 
                14, 1, 15
             };
            private Tree blTree;
            private const int BUFSIZE = 0x4000;
            private short[] d_buf;
            private const int DIST_NUM = 30;
            private Tree distTree;
            private const int EOF_SYMBOL = 0x100;
            private int extra_bits;
            private byte[] l_buf;
            private int last_lit;
            private const int LITERAL_NUM = 0x11e;
            private Tree literalTree;
            private SimpleZip.DeflaterPending pending;
            private const int REP_11_138 = 0x12;
            private const int REP_3_10 = 0x11;
            private const int REP_3_6 = 0x10;
            private static readonly short[] staticDCodes;
            private static readonly byte[] staticDLength;
            private static readonly short[] staticLCodes = new short[0x11e];
            private static readonly byte[] staticLLength = new byte[0x11e];

            static DeflaterHuffman()
            {
                int index = 0;
                while (index < 0x90)
                {
                    staticLCodes[index] = BitReverse((0x30 + index) << 8);
                    staticLLength[index++] = 8;
                }
                while (index < 0x100)
                {
                    staticLCodes[index] = BitReverse((0x100 + index) << 7);
                    staticLLength[index++] = 9;
                }
                while (index < 280)
                {
                    staticLCodes[index] = BitReverse((-256 + index) << 9);
                    staticLLength[index++] = 7;
                }
                while (index < 0x11e)
                {
                    staticLCodes[index] = BitReverse((-88 + index) << 8);
                    staticLLength[index++] = 8;
                }
                staticDCodes = new short[30];
                staticDLength = new byte[30];
                for (index = 0; index < 30; index++)
                {
                    staticDCodes[index] = BitReverse(index << 11);
                    staticDLength[index] = 5;
                }
            }

            public DeflaterHuffman(SimpleZip.DeflaterPending pending)
            {
                this.pending = pending;
                this.literalTree = new Tree(this, 0x11e, 0x101, 15);
                this.distTree = new Tree(this, 30, 1, 15);
                this.blTree = new Tree(this, 0x13, 4, 7);
                this.d_buf = new short[0x4000];
                this.l_buf = new byte[0x4000];
            }

            public static short BitReverse(int toReverse)
            {
                return (short) ((((bit4Reverse[toReverse & 15] << 12) | (bit4Reverse[(toReverse >> 4) & 15] << 8)) | (bit4Reverse[(toReverse >> 8) & 15] << 4)) | bit4Reverse[toReverse >> 12]);
            }

            public void CompressBlock()
            {
                for (int i = 0; i < this.last_lit; i++)
                {
                    int len = this.l_buf[i] & 0xff;
                    int distance = this.d_buf[i];
                    if (distance-- != 0)
                    {
                        int code = this.Lcode(len);
                        this.literalTree.WriteSymbol(code);
                        int count = (code - 0x105) / 4;
                        if ((count > 0) && (count <= 5))
                        {
                            this.pending.WriteBits(len & ((((int) 1) << count) - 1), count);
                        }
                        int num6 = this.Dcode(distance);
                        this.distTree.WriteSymbol(num6);
                        count = (num6 / 2) - 1;
                        if (count > 0)
                        {
                            this.pending.WriteBits(distance & ((((int) 1) << count) - 1), count);
                        }
                    }
                    else
                    {
                        this.literalTree.WriteSymbol(len);
                    }
                }
                this.literalTree.WriteSymbol(0x100);
            }

            private int Dcode(int distance)
            {
                int num = 0;
                while (distance >= 4)
                {
                    num += 2;
                    distance = distance >> 1;
                }
                return (num + distance);
            }

            public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
            {
                this.literalTree.freqs[0x100] = (short) (this.literalTree.freqs[0x100] + 1);
                this.literalTree.BuildTree();
                this.distTree.BuildTree();
                this.literalTree.CalcBLFreq(this.blTree);
                this.distTree.CalcBLFreq(this.blTree);
                this.blTree.BuildTree();
                int blTreeCodes = 4;
                for (int i = 0x12; i > blTreeCodes; i--)
                {
                    if (this.blTree.length[BL_ORDER[i]] > 0)
                    {
                        blTreeCodes = i + 1;
                    }
                }
                int num3 = ((((14 + (blTreeCodes * 3)) + this.blTree.GetEncodedLength()) + this.literalTree.GetEncodedLength()) + this.distTree.GetEncodedLength()) + this.extra_bits;
                int num4 = this.extra_bits;
                for (int j = 0; j < 0x11e; j++)
                {
                    num4 += this.literalTree.freqs[j] * staticLLength[j];
                }
                for (int k = 0; k < 30; k++)
                {
                    num4 += this.distTree.freqs[k] * staticDLength[k];
                }
                if (num3 >= num4)
                {
                    num3 = num4;
                }
                if ((storedOffset >= 0) && ((storedLength + 4) < (num3 >> 3)))
                {
                    this.FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
                }
                else if (num3 == num4)
                {
                    this.pending.WriteBits(2 + (lastBlock ? 1 : 0), 3);
                    this.literalTree.SetStaticCodes(staticLCodes, staticLLength);
                    this.distTree.SetStaticCodes(staticDCodes, staticDLength);
                    this.CompressBlock();
                    this.Init();
                }
                else
                {
                    this.pending.WriteBits(4 + (lastBlock ? 1 : 0), 3);
                    this.SendAllTrees(blTreeCodes);
                    this.CompressBlock();
                    this.Init();
                }
            }

            public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
            {
                this.pending.WriteBits(lastBlock ? 1 : 0, 3);
                this.pending.AlignToByte();
                this.pending.WriteShort(storedLength);
                this.pending.WriteShort(~storedLength);
                this.pending.WriteBlock(stored, storedOffset, storedLength);
                this.Init();
            }

            public void Init()
            {
                this.last_lit = 0;
                this.extra_bits = 0;
            }

            public bool IsFull()
            {
                return (this.last_lit >= 0x4000);
            }

            private int Lcode(int len)
            {
                if (len == 0xff)
                {
                    return 0x11d;
                }
                int num = 0x101;
                while (len >= 8)
                {
                    num += 4;
                    len = len >> 1;
                }
                return (num + len);
            }

            public void SendAllTrees(int blTreeCodes)
            {
                this.blTree.BuildCodes();
                this.literalTree.BuildCodes();
                this.distTree.BuildCodes();
                this.pending.WriteBits(this.literalTree.numCodes - 0x101, 5);
                this.pending.WriteBits(this.distTree.numCodes - 1, 5);
                this.pending.WriteBits(blTreeCodes - 4, 4);
                for (int i = 0; i < blTreeCodes; i++)
                {
                    this.pending.WriteBits(this.blTree.length[BL_ORDER[i]], 3);
                }
                this.literalTree.WriteTree(this.blTree);
                this.distTree.WriteTree(this.blTree);
            }

            public bool TallyDist(int dist, int len)
            {
                this.d_buf[this.last_lit] = (short) dist;
                this.l_buf[this.last_lit++] = (byte) (len - 3);
                int index = this.Lcode(len - 3);
                this.literalTree.freqs[index] = (short) (this.literalTree.freqs[index] + 1);
                if ((index >= 0x109) && (index < 0x11d))
                {
                    this.extra_bits += (index - 0x105) / 4;
                }
                int num2 = this.Dcode(dist - 1);
                this.distTree.freqs[num2] = (short) (this.distTree.freqs[num2] + 1);
                if (num2 >= 4)
                {
                    this.extra_bits += (num2 / 2) - 1;
                }
                return this.IsFull();
            }

            public bool TallyLit(int lit)
            {
                this.d_buf[this.last_lit] = 0;
                this.l_buf[this.last_lit++] = (byte) lit;
                this.literalTree.freqs[lit] = (short) (this.literalTree.freqs[lit] + 1);
                return this.IsFull();
            }

            public sealed class Tree
            {
                private int[] bl_counts;
                private short[] codes;
                private SimpleZip.DeflaterHuffman dh;
                public short[] freqs;
                public byte[] length;
                private int maxLength;
                public int minNumCodes;
                public int numCodes;

                public Tree(SimpleZip.DeflaterHuffman dh, int elems, int minCodes, int maxLength)
                {
                    this.dh = dh;
                    this.minNumCodes = minCodes;
                    this.maxLength = maxLength;
                    this.freqs = new short[elems];
                    this.bl_counts = new int[maxLength];
                }

                public void BuildCodes()
                {
                    int[] numArray = new int[this.maxLength];
                    int num = 0;
                    this.codes = new short[this.freqs.Length];
                    for (int i = 0; i < this.maxLength; i++)
                    {
                        numArray[i] = num;
                        num += this.bl_counts[i] << (15 - i);
                    }
                    for (int j = 0; j < this.numCodes; j++)
                    {
                        int num4 = this.length[j];
                        if (num4 > 0)
                        {
                            this.codes[j] = SimpleZip.DeflaterHuffman.BitReverse(numArray[num4 - 1]);
                            numArray[num4 - 1] += ((int) 1) << (0x10 - num4);
                        }
                    }
                }

                private void BuildLength(int[] childs)
                {
                    this.length = new byte[this.freqs.Length];
                    int num = childs.Length / 2;
                    int num2 = (num + 1) / 2;
                    int num3 = 0;
                    for (int i = 0; i < this.maxLength; i++)
                    {
                        this.bl_counts[i] = 0;
                    }
                    int[] numArray = new int[num];
                    numArray[num - 1] = 0;
                    for (int j = num - 1; j >= 0; j--)
                    {
                        if (childs[(2 * j) + 1] != -1)
                        {
                            int maxLength = numArray[j] + 1;
                            if (maxLength > this.maxLength)
                            {
                                maxLength = this.maxLength;
                                num3++;
                            }
                            numArray[childs[2 * j]] = numArray[childs[(2 * j) + 1]] = maxLength;
                        }
                        else
                        {
                            int num7 = numArray[j];
                            this.bl_counts[num7 - 1]++;
                            this.length[childs[2 * j]] = (byte) numArray[j];
                        }
                    }
                    if (num3 != 0)
                    {
                        int index = this.maxLength - 1;
                        do
                        {
                            while (this.bl_counts[--index] == 0)
                            {
                            }
                            do
                            {
                                this.bl_counts[index]--;
                                this.bl_counts[++index]++;
                                num3 -= ((int) 1) << ((this.maxLength - 1) - index);
                            }
                            while ((num3 > 0) && (index < (this.maxLength - 1)));
                        }
                        while (num3 > 0);
                        this.bl_counts[this.maxLength - 1] += num3;
                        this.bl_counts[this.maxLength - 2] -= num3;
                        int num9 = 2 * num2;
                        for (int k = this.maxLength; k != 0; k--)
                        {
                            int num11 = this.bl_counts[k - 1];
                            while (num11 > 0)
                            {
                                int num12 = 2 * childs[num9++];
                                if (childs[num12 + 1] == -1)
                                {
                                    this.length[childs[num12]] = (byte) k;
                                    num11--;
                                }
                            }
                        }
                    }
                }

                public void BuildTree()
                {
                    int length = this.freqs.Length;
                    int[] numArray = new int[length];
                    int num2 = 0;
                    int num3 = 0;
                    for (int i = 0; i < length; i++)
                    {
                        int num5 = this.freqs[i];
                        if (num5 != 0)
                        {
                            int num7;
                            int index = num2++;
                            while ((index > 0) && (this.freqs[numArray[num7 = (index - 1) / 2]] > num5))
                            {
                                numArray[index] = numArray[num7];
                                index = num7;
                            }
                            numArray[index] = i;
                            num3 = i;
                        }
                    }
                    while (num2 < 2)
                    {
                        int num8 = (num3 < 2) ? ++num3 : 0;
                        numArray[num2++] = num8;
                    }
                    this.numCodes = Math.Max(num3 + 1, this.minNumCodes);
                    int num9 = num2;
                    int[] childs = new int[(4 * num2) - 2];
                    int[] numArray3 = new int[(2 * num2) - 1];
                    int num10 = num9;
                    for (int j = 0; j < num2; j++)
                    {
                        int num12 = numArray[j];
                        childs[2 * j] = num12;
                        childs[(2 * j) + 1] = -1;
                        numArray3[j] = this.freqs[num12] << 8;
                        numArray[j] = j;
                    }
                    do
                    {
                        int num13 = numArray[0];
                        int num14 = numArray[--num2];
                        int num15 = 0;
                        int num16 = 1;
                        while (num16 < num2)
                        {
                            if (((num16 + 1) < num2) && (numArray3[numArray[num16]] > numArray3[numArray[num16 + 1]]))
                            {
                                num16++;
                            }
                            numArray[num15] = numArray[num16];
                            num15 = num16;
                            num16 = (num16 * 2) + 1;
                        }
                        int num17 = numArray3[num14];
                        while (((num16 = num15) > 0) && (numArray3[numArray[num15 = (num16 - 1) / 2]] > num17))
                        {
                            numArray[num16] = numArray[num15];
                        }
                        numArray[num16] = num14;
                        int num18 = numArray[0];
                        num14 = num10++;
                        childs[2 * num14] = num13;
                        childs[(2 * num14) + 1] = num18;
                        int num19 = Math.Min((int) (numArray3[num13] & 0xff), (int) (numArray3[num18] & 0xff));
                        numArray3[num14] = num17 = ((numArray3[num13] + numArray3[num18]) - num19) + 1;
                        num15 = 0;
                        num16 = 1;
                        while (num16 < num2)
                        {
                            if (((num16 + 1) < num2) && (numArray3[numArray[num16]] > numArray3[numArray[num16 + 1]]))
                            {
                                num16++;
                            }
                            numArray[num15] = numArray[num16];
                            num15 = num16;
                            num16 = (num15 * 2) + 1;
                        }
                        while (((num16 = num15) > 0) && (numArray3[numArray[num15 = (num16 - 1) / 2]] > num17))
                        {
                            numArray[num16] = numArray[num15];
                        }
                        numArray[num16] = num14;
                    }
                    while (num2 > 1);
                    this.BuildLength(childs);
                }

                public void CalcBLFreq(SimpleZip.DeflaterHuffman.Tree blTree)
                {
                    int index = -1;
                    int num5 = 0;
                    while (num5 < this.numCodes)
                    {
                        int num;
                        int num2;
                        int num3 = 1;
                        int num6 = this.length[num5];
                        if (num6 == 0)
                        {
                            num = 0x8a;
                            num2 = 3;
                        }
                        else
                        {
                            num = 6;
                            num2 = 3;
                            if (index != num6)
                            {
                                blTree.freqs[num6] = (short) (blTree.freqs[num6] + 1);
                                num3 = 0;
                            }
                        }
                        index = num6;
                        num5++;
                        while ((num5 < this.numCodes) && (index == this.length[num5]))
                        {
                            num5++;
                            if (++num3 >= num)
                            {
                                break;
                            }
                        }
                        if (num3 < num2)
                        {
                            blTree.freqs[index] = (short) (blTree.freqs[index] + ((short) num3));
                        }
                        else
                        {
                            if (index != 0)
                            {
                                blTree.freqs[0x10] = (short) (blTree.freqs[0x10] + 1);
                                continue;
                            }
                            if (num3 <= 10)
                            {
                                blTree.freqs[0x11] = (short) (blTree.freqs[0x11] + 1);
                                continue;
                            }
                            blTree.freqs[0x12] = (short) (blTree.freqs[0x12] + 1);
                        }
                    }
                }

                public int GetEncodedLength()
                {
                    int num = 0;
                    for (int i = 0; i < this.freqs.Length; i++)
                    {
                        num += this.freqs[i] * this.length[i];
                    }
                    return num;
                }

                public void SetStaticCodes(short[] stCodes, byte[] stLength)
                {
                    this.codes = stCodes;
                    this.length = stLength;
                }

                public void WriteSymbol(int code)
                {
                    this.dh.pending.WriteBits(this.codes[code] & 0xffff, this.length[code]);
                }

                public void WriteTree(SimpleZip.DeflaterHuffman.Tree blTree)
                {
                    int code = -1;
                    int index = 0;
                    while (index < this.numCodes)
                    {
                        int num;
                        int num2;
                        int num3 = 1;
                        int num6 = this.length[index];
                        if (num6 == 0)
                        {
                            num = 0x8a;
                            num2 = 3;
                        }
                        else
                        {
                            num = 6;
                            num2 = 3;
                            if (code != num6)
                            {
                                blTree.WriteSymbol(num6);
                                num3 = 0;
                            }
                        }
                        code = num6;
                        index++;
                        while ((index < this.numCodes) && (code == this.length[index]))
                        {
                            index++;
                            if (++num3 >= num)
                            {
                                break;
                            }
                        }
                        if (num3 < num2)
                        {
                            while (num3-- > 0)
                            {
                                blTree.WriteSymbol(code);
                            }
                        }
                        else if (code != 0)
                        {
                            blTree.WriteSymbol(0x10);
                            this.dh.pending.WriteBits(num3 - 3, 2);
                        }
                        else
                        {
                            if (num3 <= 10)
                            {
                                blTree.WriteSymbol(0x11);
                                this.dh.pending.WriteBits(num3 - 3, 3);
                                continue;
                            }
                            blTree.WriteSymbol(0x12);
                            this.dh.pending.WriteBits(num3 - 11, 7);
                        }
                    }
                }
            }
        }

        internal sealed class DeflaterPending
        {
            private int bitCount;
            private uint bits;
            protected byte[] buf = new byte[0x10000];
            private int end;
            private int start;

            public void AlignToByte()
            {
                if (this.bitCount > 0)
                {
                    this.buf[this.end++] = (byte) this.bits;
                    if (this.bitCount > 8)
                    {
                        this.buf[this.end++] = (byte) (this.bits >> 8);
                    }
                }
                this.bits = 0;
                this.bitCount = 0;
            }

            public int Flush(byte[] output, int offset, int length)
            {
                if (this.bitCount >= 8)
                {
                    this.buf[this.end++] = (byte) this.bits;
                    this.bits = this.bits >> 8;
                    this.bitCount -= 8;
                }
                if (length > (this.end - this.start))
                {
                    length = this.end - this.start;
                    Array.Copy(this.buf, this.start, output, offset, length);
                    this.start = 0;
                    this.end = 0;
                    return length;
                }
                Array.Copy(this.buf, this.start, output, offset, length);
                this.start += length;
                return length;
            }

            public void WriteBits(int b, int count)
            {
                this.bits |= (uint) (b << this.bitCount);
                this.bitCount += count;
                if (this.bitCount >= 0x10)
                {
                    this.buf[this.end++] = (byte) this.bits;
                    this.buf[this.end++] = (byte) (this.bits >> 8);
                    this.bits = this.bits >> 0x10;
                    this.bitCount -= 0x10;
                }
            }

            public void WriteBlock(byte[] block, int offset, int len)
            {
                Array.Copy(block, offset, this.buf, this.end, len);
                this.end += len;
            }

            public void WriteShort(int s)
            {
                this.buf[this.end++] = (byte) s;
                this.buf[this.end++] = (byte) (s >> 8);
            }

            public int BitCount
            {
                get
                {
                    return this.bitCount;
                }
            }

            public bool IsFlushed
            {
                get
                {
                    return (this.end == 0);
                }
            }
        }

        internal sealed class Inflater
        {
            private static readonly int[] CPDEXT = new int[] { 
                0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 
                7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
             };
            private static readonly int[] CPDIST = new int[] { 
                1, 2, 3, 4, 5, 7, 9, 13, 0x11, 0x19, 0x21, 0x31, 0x41, 0x61, 0x81, 0xc1, 
                0x101, 0x181, 0x201, 0x301, 0x401, 0x601, 0x801, 0xc01, 0x1001, 0x1801, 0x2001, 0x3001, 0x4001, 0x6001
             };
            private static readonly int[] CPLENS = new int[] { 
                3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 0x11, 0x13, 0x17, 0x1b, 0x1f, 
                0x23, 0x2b, 0x33, 0x3b, 0x43, 0x53, 0x63, 0x73, 0x83, 0xa3, 0xc3, 0xe3, 0x102
             };
            private static readonly int[] CPLEXT = new int[] { 
                0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 
                3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
             };
            private const int DECODE_BLOCKS = 2;
            private const int DECODE_CHKSUM = 11;
            private const int DECODE_DICT = 1;
            private const int DECODE_DYN_HEADER = 6;
            private const int DECODE_HEADER = 0;
            private const int DECODE_HUFFMAN = 7;
            private const int DECODE_HUFFMAN_DIST = 9;
            private const int DECODE_HUFFMAN_DISTBITS = 10;
            private const int DECODE_HUFFMAN_LENBITS = 8;
            private const int DECODE_STORED = 5;
            private const int DECODE_STORED_LEN1 = 3;
            private const int DECODE_STORED_LEN2 = 4;
            private SimpleZip.InflaterHuffmanTree distTree;
            private SimpleZip.InflaterDynHeader dynHeader;
            private const int FINISHED = 12;
            private SimpleZip.StreamManipulator input = new SimpleZip.StreamManipulator();
            private bool isLastBlock;
            private SimpleZip.InflaterHuffmanTree litlenTree;
            private int mode = 2;
            private int neededBits;
            private SimpleZip.OutputWindow outputWindow = new SimpleZip.OutputWindow();
            private int repDist;
            private int repLength;
            private int uncomprLen;

            public Inflater(byte[] bytes)
            {
                this.input.SetInput(bytes, 0, bytes.Length);
            }

            private bool Decode()
            {
                int num3;
                switch (this.mode)
                {
                    case 2:
                        if (!this.isLastBlock)
                        {
                            int num = this.input.PeekBits(3);
                            if (num < 0)
                            {
                                return false;
                            }
                            this.input.DropBits(3);
                            if ((num & 1) != 0)
                            {
                                this.isLastBlock = true;
                            }
                            switch ((num >> 1))
                            {
                                case 0:
                                    this.input.SkipToByteBoundary();
                                    this.mode = 3;
                                    goto Label_00DC;

                                case 1:
                                    this.litlenTree = SimpleZip.InflaterHuffmanTree.defLitLenTree;
                                    this.distTree = SimpleZip.InflaterHuffmanTree.defDistTree;
                                    this.mode = 7;
                                    goto Label_00DC;

                                case 2:
                                    this.dynHeader = new SimpleZip.InflaterDynHeader();
                                    this.mode = 6;
                                    goto Label_00DC;
                            }
                            break;
                        }
                        this.mode = 12;
                        return false;

                    case 3:
                        this.uncomprLen = this.input.PeekBits(0x10);
                        if (this.uncomprLen >= 0)
                        {
                            this.input.DropBits(0x10);
                            this.mode = 4;
                            goto Label_010F;
                        }
                        return false;

                    case 4:
                        goto Label_010F;

                    case 5:
                        goto Label_0137;

                    case 6:
                        if (this.dynHeader.Decode(this.input))
                        {
                            this.litlenTree = this.dynHeader.BuildLitLenTree();
                            this.distTree = this.dynHeader.BuildDistTree();
                            this.mode = 7;
                            goto Label_01BB;
                        }
                        return false;

                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        goto Label_01BB;

                    case 12:
                        return false;

                    default:
                        return false;
                }
            Label_00DC:
                return true;
            Label_010F:
                if (this.input.PeekBits(0x10) < 0)
                {
                    return false;
                }
                this.input.DropBits(0x10);
                this.mode = 5;
            Label_0137:
                num3 = this.outputWindow.CopyStored(this.input, this.uncomprLen);
                this.uncomprLen -= num3;
                if (this.uncomprLen == 0)
                {
                    this.mode = 2;
                    return true;
                }
                return !this.input.IsNeedingInput;
            Label_01BB:
                return this.DecodeHuffman();
            }

            private bool DecodeHuffman()
            {
                int freeSpace = this.outputWindow.GetFreeSpace();
                while (freeSpace >= 0x102)
                {
                    int num2;
                    switch (this.mode)
                    {
                        case 7:
                            goto Label_0051;

                        case 8:
                            goto Label_00B7;

                        case 9:
                            goto Label_0106;

                        case 10:
                            goto Label_0138;

                        default:
                        {
                            continue;
                        }
                    }
                Label_0037:
                    this.outputWindow.Write(num2);
                    if (--freeSpace < 0x102)
                    {
                        return true;
                    }
                Label_0051:
                    if (((num2 = this.litlenTree.GetSymbol(this.input)) & -256) == 0)
                    {
                        goto Label_0037;
                    }
                    if (num2 < 0x101)
                    {
                        if (num2 < 0)
                        {
                            return false;
                        }
                        this.distTree = null;
                        this.litlenTree = null;
                        this.mode = 2;
                        return true;
                    }
                    this.repLength = CPLENS[num2 - 0x101];
                    this.neededBits = CPLEXT[num2 - 0x101];
                Label_00B7:
                    if (this.neededBits > 0)
                    {
                        this.mode = 8;
                        int num3 = this.input.PeekBits(this.neededBits);
                        if (num3 < 0)
                        {
                            return false;
                        }
                        this.input.DropBits(this.neededBits);
                        this.repLength += num3;
                    }
                    this.mode = 9;
                Label_0106:
                    num2 = this.distTree.GetSymbol(this.input);
                    if (num2 < 0)
                    {
                        return false;
                    }
                    this.repDist = CPDIST[num2];
                    this.neededBits = CPDEXT[num2];
                Label_0138:
                    if (this.neededBits > 0)
                    {
                        this.mode = 10;
                        int num4 = this.input.PeekBits(this.neededBits);
                        if (num4 < 0)
                        {
                            return false;
                        }
                        this.input.DropBits(this.neededBits);
                        this.repDist += num4;
                    }
                    this.outputWindow.Repeat(this.repLength, this.repDist);
                    freeSpace -= this.repLength;
                    this.mode = 7;
                }
                return true;
            }

            public int Inflate(byte[] buf, int offset, int len)
            {
                int num = 0;
                do
                {
                    if (this.mode != 11)
                    {
                        int num2 = this.outputWindow.CopyOutput(buf, offset, len);
                        offset += num2;
                        num += num2;
                        len -= num2;
                        if (len == 0)
                        {
                            return num;
                        }
                    }
                }
                while (this.Decode() || ((this.outputWindow.GetAvailable() > 0) && (this.mode != 11)));
                return num;
            }
        }

        internal sealed class InflaterDynHeader
        {
            private static readonly int[] BL_ORDER = new int[] { 
                0x10, 0x11, 0x12, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 
                14, 1, 15
             };
            private byte[] blLens;
            private const int BLLENS = 3;
            private int blnum;
            private const int BLNUM = 2;
            private SimpleZip.InflaterHuffmanTree blTree;
            private int dnum;
            private const int DNUM = 1;
            private byte lastLen;
            private const int LENS = 4;
            private byte[] litdistLens;
            private int lnum;
            private const int LNUM = 0;
            private int mode;
            private int num;
            private int ptr;
            private static readonly int[] repBits = new int[] { 2, 3, 7 };
            private static readonly int[] repMin = new int[] { 3, 3, 11 };
            private const int REPS = 5;
            private int repSymbol;

            public SimpleZip.InflaterHuffmanTree BuildDistTree()
            {
                byte[] destinationArray = new byte[this.dnum];
                Array.Copy(this.litdistLens, this.lnum, destinationArray, 0, this.dnum);
                return new SimpleZip.InflaterHuffmanTree(destinationArray);
            }

            public SimpleZip.InflaterHuffmanTree BuildLitLenTree()
            {
                byte[] destinationArray = new byte[this.lnum];
                Array.Copy(this.litdistLens, 0, destinationArray, 0, this.lnum);
                return new SimpleZip.InflaterHuffmanTree(destinationArray);
            }

            public bool Decode(SimpleZip.StreamManipulator input)
            {
                int num2;
                int num3;
            Label_0000:
                switch (this.mode)
                {
                    case 0:
                        this.lnum = input.PeekBits(5);
                        if (this.lnum >= 0)
                        {
                            this.lnum += 0x101;
                            input.DropBits(5);
                            this.mode = 1;
                            break;
                        }
                        return false;

                    case 1:
                        break;

                    case 2:
                        goto Label_00B9;

                    case 3:
                        goto Label_013B;

                    case 4:
                        goto Label_01A8;

                    case 5:
                        goto Label_01DE;

                    default:
                        goto Label_0000;
                }
                this.dnum = input.PeekBits(5);
                if (this.dnum < 0)
                {
                    return false;
                }
                this.dnum++;
                input.DropBits(5);
                this.num = this.lnum + this.dnum;
                this.litdistLens = new byte[this.num];
                this.mode = 2;
            Label_00B9:
                this.blnum = input.PeekBits(4);
                if (this.blnum < 0)
                {
                    return false;
                }
                this.blnum += 4;
                input.DropBits(4);
                this.blLens = new byte[0x13];
                this.ptr = 0;
                this.mode = 3;
            Label_013B:
                while (this.ptr < this.blnum)
                {
                    int num = input.PeekBits(3);
                    if (num < 0)
                    {
                        return false;
                    }
                    input.DropBits(3);
                    this.blLens[BL_ORDER[this.ptr]] = (byte) num;
                    this.ptr++;
                }
                this.blTree = new SimpleZip.InflaterHuffmanTree(this.blLens);
                this.blLens = null;
                this.ptr = 0;
                this.mode = 4;
            Label_01A8:
                while (((num2 = this.blTree.GetSymbol(input)) & -16) == 0)
                {
                    this.litdistLens[this.ptr++] = this.lastLen = (byte) num2;
                    if (this.ptr == this.num)
                    {
                        return true;
                    }
                }
                if (num2 < 0)
                {
                    return false;
                }
                if (num2 >= 0x11)
                {
                    this.lastLen = 0;
                }
                this.repSymbol = num2 - 0x10;
                this.mode = 5;
            Label_01DE:
                num3 = repBits[this.repSymbol];
                int num4 = input.PeekBits(num3);
                if (num4 < 0)
                {
                    return false;
                }
                input.DropBits(num3);
                num4 += repMin[this.repSymbol];
                while (num4-- > 0)
                {
                    this.litdistLens[this.ptr++] = this.lastLen;
                }
                if (this.ptr == this.num)
                {
                    return true;
                }
                this.mode = 4;
                goto Label_0000;
            }
        }

        internal sealed class InflaterHuffmanTree
        {
            public static readonly SimpleZip.InflaterHuffmanTree defDistTree;
            public static readonly SimpleZip.InflaterHuffmanTree defLitLenTree;
            private const int MAX_BITLEN = 15;
            private short[] tree;

            static InflaterHuffmanTree()
            {
                byte[] codeLengths = new byte[0x120];
                int num = 0;
                while (num < 0x90)
                {
                    codeLengths[num++] = 8;
                }
                while (num < 0x100)
                {
                    codeLengths[num++] = 9;
                }
                while (num < 280)
                {
                    codeLengths[num++] = 7;
                }
                while (num < 0x120)
                {
                    codeLengths[num++] = 8;
                }
                defLitLenTree = new SimpleZip.InflaterHuffmanTree(codeLengths);
                codeLengths = new byte[0x20];
                num = 0;
                while (num < 0x20)
                {
                    codeLengths[num++] = 5;
                }
                defDistTree = new SimpleZip.InflaterHuffmanTree(codeLengths);
            }

            public InflaterHuffmanTree(byte[] codeLengths)
            {
                this.BuildTree(codeLengths);
            }

            private void BuildTree(byte[] codeLengths)
            {
                int[] numArray = new int[0x10];
                int[] numArray2 = new int[0x10];
                for (int i = 0; i < codeLengths.Length; i++)
                {
                    int index = codeLengths[i];
                    if (index > 0)
                    {
                        numArray[index]++;
                    }
                }
                int toReverse = 0;
                int num4 = 0x200;
                for (int j = 1; j <= 15; j++)
                {
                    numArray2[j] = toReverse;
                    toReverse += numArray[j] << (0x10 - j);
                    if (j >= 10)
                    {
                        int num6 = numArray2[j] & 0x1ff80;
                        int num7 = toReverse & 0x1ff80;
                        num4 += (num7 - num6) >> (0x10 - j);
                    }
                }
                this.tree = new short[num4];
                int num8 = 0x200;
                for (int k = 15; k >= 10; k--)
                {
                    int num10 = toReverse & 0x1ff80;
                    toReverse -= numArray[k] << (0x10 - k);
                    int num11 = toReverse & 0x1ff80;
                    for (int n = num11; n < num10; n += 0x80)
                    {
                        this.tree[SimpleZip.DeflaterHuffman.BitReverse(n)] = (short) ((-num8 << 4) | k);
                        num8 += ((int) 1) << (k - 9);
                    }
                }
                for (int m = 0; m < codeLengths.Length; m++)
                {
                    int num14 = codeLengths[m];
                    if (num14 != 0)
                    {
                        toReverse = numArray2[num14];
                        int num15 = SimpleZip.DeflaterHuffman.BitReverse(toReverse);
                        if (num14 <= 9)
                        {
                            do
                            {
                                this.tree[num15] = (short) ((m << 4) | num14);
                                num15 += ((int) 1) << num14;
                            }
                            while (num15 < 0x200);
                        }
                        else
                        {
                            int num16 = this.tree[num15 & 0x1ff];
                            int num17 = ((int) 1) << (num16 & 15);
                            num16 = -(num16 >> 4);
                            do
                            {
                                this.tree[num16 | (num15 >> 9)] = (short) ((m << 4) | num14);
                                num15 += ((int) 1) << num14;
                            }
                            while (num15 < num17);
                        }
                        numArray2[num14] = toReverse + (((int) 1) << (0x10 - num14));
                    }
                }
            }

            public int GetSymbol(SimpleZip.StreamManipulator input)
            {
                int num2;
                int index = input.PeekBits(9);
                if (index >= 0)
                {
                    num2 = this.tree[index];
                    if (num2 >= 0)
                    {
                        input.DropBits(num2 & 15);
                        return (num2 >> 4);
                    }
                    int num3 = -(num2 >> 4);
                    int n = num2 & 15;
                    index = input.PeekBits(n);
                    if (index >= 0)
                    {
                        num2 = this.tree[num3 | (index >> 9)];
                        input.DropBits(num2 & 15);
                        return (num2 >> 4);
                    }
                    int num5 = input.AvailableBits;
                    index = input.PeekBits(num5);
                    num2 = this.tree[num3 | (index >> 9)];
                    if ((num2 & 15) <= num5)
                    {
                        input.DropBits(num2 & 15);
                        return (num2 >> 4);
                    }
                    return -1;
                }
                int availableBits = input.AvailableBits;
                index = input.PeekBits(availableBits);
                num2 = this.tree[index];
                if ((num2 >= 0) && ((num2 & 15) <= availableBits))
                {
                    input.DropBits(num2 & 15);
                    return (num2 >> 4);
                }
                return -1;
            }
        }

        internal sealed class OutputWindow
        {
            private byte[] window = new byte[0x8000];
            private const int WINDOW_MASK = 0x7fff;
            private const int WINDOW_SIZE = 0x8000;
            private int windowEnd;
            private int windowFilled;

            public void CopyDict(byte[] dict, int offset, int len)
            {
                if (this.windowFilled > 0)
                {
                    throw new InvalidOperationException();
                }
                if (len > 0x8000)
                {
                    offset += len - 0x8000;
                    len = 0x8000;
                }
                Array.Copy(dict, offset, this.window, 0, len);
                this.windowEnd = len & 0x7fff;
            }

            public int CopyOutput(byte[] output, int offset, int len)
            {
                int windowEnd = this.windowEnd;
                if (len > this.windowFilled)
                {
                    len = this.windowFilled;
                }
                else
                {
                    windowEnd = ((this.windowEnd - this.windowFilled) + len) & 0x7fff;
                }
                int num2 = len;
                int length = len - windowEnd;
                if (length > 0)
                {
                    Array.Copy(this.window, 0x8000 - length, output, offset, length);
                    offset += length;
                    len = windowEnd;
                }
                Array.Copy(this.window, windowEnd - len, output, offset, len);
                this.windowFilled -= num2;
                if (this.windowFilled < 0)
                {
                    throw new InvalidOperationException();
                }
                return num2;
            }

            public int CopyStored(SimpleZip.StreamManipulator input, int len)
            {
                int num;
                len = Math.Min(Math.Min(len, 0x8000 - this.windowFilled), input.AvailableBytes);
                int length = 0x8000 - this.windowEnd;
                if (len > length)
                {
                    num = input.CopyBytes(this.window, this.windowEnd, length);
                    if (num == length)
                    {
                        num += input.CopyBytes(this.window, 0, len - length);
                    }
                }
                else
                {
                    num = input.CopyBytes(this.window, this.windowEnd, len);
                }
                this.windowEnd = (this.windowEnd + num) & 0x7fff;
                this.windowFilled += num;
                return num;
            }

            public int GetAvailable()
            {
                return this.windowFilled;
            }

            public int GetFreeSpace()
            {
                return (0x8000 - this.windowFilled);
            }

            public void Repeat(int len, int dist)
            {
                this.windowFilled += len;
                if (this.windowFilled > 0x8000)
                {
                    throw new InvalidOperationException();
                }
                int repStart = (this.windowEnd - dist) & 0x7fff;
                int num2 = 0x8000 - len;
                if ((repStart > num2) || (this.windowEnd >= num2))
                {
                    this.SlowRepeat(repStart, len, dist);
                }
                else if (len > dist)
                {
                    while (len-- > 0)
                    {
                        this.window[this.windowEnd++] = this.window[repStart++];
                    }
                }
                else
                {
                    Array.Copy(this.window, repStart, this.window, this.windowEnd, len);
                    this.windowEnd += len;
                }
            }

            public void Reset()
            {
                this.windowFilled = this.windowEnd = 0;
            }

            private void SlowRepeat(int repStart, int len, int dist)
            {
                while (len-- > 0)
                {
                    this.window[this.windowEnd++] = this.window[repStart++];
                    this.windowEnd &= 0x7fff;
                    repStart &= 0x7fff;
                }
            }

            public void Write(int abyte)
            {
                if (this.windowFilled++ == 0x8000)
                {
                    throw new InvalidOperationException();
                }
                this.window[this.windowEnd++] = (byte) abyte;
                this.windowEnd &= 0x7fff;
            }
        }

        internal sealed class StreamManipulator
        {
            private int bits_in_buffer;
            private uint buffer;
            private byte[] window;
            private int window_end;
            private int window_start;

            public int CopyBytes(byte[] output, int offset, int length)
            {
                int num = 0;
                while ((this.bits_in_buffer > 0) && (length > 0))
                {
                    output[offset++] = (byte) this.buffer;
                    this.buffer = this.buffer >> 8;
                    this.bits_in_buffer -= 8;
                    length--;
                    num++;
                }
                if (length == 0)
                {
                    return num;
                }
                int num2 = this.window_end - this.window_start;
                if (length > num2)
                {
                    length = num2;
                }
                Array.Copy(this.window, this.window_start, output, offset, length);
                this.window_start += length;
                if (((this.window_start - this.window_end) & 1) != 0)
                {
                    this.buffer = (uint) (this.window[this.window_start++] & 0xff);
                    this.bits_in_buffer = 8;
                }
                return (num + length);
            }

            public void DropBits(int n)
            {
                this.buffer = this.buffer >> n;
                this.bits_in_buffer -= n;
            }

            public int PeekBits(int n)
            {
                if (this.bits_in_buffer < n)
                {
                    if (this.window_start == this.window_end)
                    {
                        return -1;
                    }
                    this.buffer |= (uint) (((this.window[this.window_start++] & 0xff) | ((this.window[this.window_start++] & 0xff) << 8)) << this.bits_in_buffer);
                    this.bits_in_buffer += 0x10;
                }
                return (((int) this.buffer) & ((((int) 1) << n) - 1));
            }

            public void Reset()
            {
                this.buffer = (uint) (this.window_start = this.window_end = this.bits_in_buffer = 0);
            }

            public void SetInput(byte[] buf, int off, int len)
            {
                if (this.window_start < this.window_end)
                {
                    throw new InvalidOperationException();
                }
                int num = off + len;
                if (((0 > off) || (off > num)) || (num > buf.Length))
                {
                    throw new ArgumentOutOfRangeException();
                }
                if ((len & 1) != 0)
                {
                    this.buffer |= (uint) ((buf[off++] & 0xff) << this.bits_in_buffer);
                    this.bits_in_buffer += 8;
                }
                this.window = buf;
                this.window_start = off;
                this.window_end = num;
            }

            public void SkipToByteBoundary()
            {
                this.buffer = this.buffer >> (this.bits_in_buffer & 7);
                this.bits_in_buffer &= -8;
            }

            public int AvailableBits
            {
                get
                {
                    return this.bits_in_buffer;
                }
            }

            public int AvailableBytes
            {
                get
                {
                    return ((this.window_end - this.window_start) + (this.bits_in_buffer >> 3));
                }
            }

            public bool IsNeedingInput
            {
                get
                {
                    return (this.window_start == this.window_end);
                }
            }
        }

        internal sealed class ZipStream : MemoryStream
        {
            public ZipStream()
            {
            }

            public ZipStream(byte[] buffer) : base(buffer, false)
            {
            }

            public int ReadInt()
            {
                return (this.ReadShort() | (this.ReadShort() << 0x10));
            }

            public int ReadShort()
            {
                return (this.ReadByte() | (this.ReadByte() << 8));
            }

            public void WriteInt(int value)
            {
                this.WriteShort(value);
                this.WriteShort(value >> 0x10);
            }

            public void WriteShort(int value)
            {
                this.WriteByte((byte) (value & 0xff));
                this.WriteByte((byte) ((value >> 8) & 0xff));
            }
        }
    }
}

