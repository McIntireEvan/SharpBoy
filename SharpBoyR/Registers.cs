namespace SharpBoyR
{
    class Registers
    {
        public static byte[] values;

        public static void Init()
        {
            values = new byte[8];
        }

        public static byte A
        {
            get { return values[0]; }
            set { values[0] = (byte)value; }
        }

        public static byte F
        {
            get { return values[1]; }
            set { values[1] = (byte)value; }
        }

        public static byte B
        {
            get { return values[2]; }
            set { values[2] = (byte)value; }
        }

        public static byte C
        {
            get { return values[3]; }
            set { values[3] = (byte)value; }
        }

        public static byte D
        {
            get { return values[4]; }
            set { values[4] = (byte)value; }
        }

        public static byte E
        {
            get { return values[5]; }
            set { values[5] = (byte)value; }
        }

        public static byte H
        {
            get { return values[6]; }
            set { values[6] = (byte)value; }
        }

        public static byte L
        {
            get { return values[7]; }
            set { values[7] = (byte)value; }
        }

        public static ushort AF
        {
            get { return (ushort)((values[0] << 8) + (values[1])); }
            set
            {
                values[0] = (byte)((value & 0xFF00) >> 8);
                values[1] = (byte)(value & 0x00FF);
            }
        }

        public static ushort BC
        {
            get { return (ushort)((values[2] << 8) + (values[3])); }
            set
            {
                values[2] = (byte)((value & 0xFF00) >> 8);
                values[3] = (byte)(value & 0x00FF);
            }
        }

        public static ushort DE
        {
            get { return (ushort)((values[4] << 8) + (values[5])); }
            set
            {
                values[4] = (byte)((value & 0xFF00) >> 8);
                values[5] = (byte)(value & 0x00FF);
            }
        }

        public static ushort HL
        {
            get { return (ushort)((values[6] << 8) + (values[7])); }
            set
            {
                values[6] = (byte)((value & 0xFF00) >> 8);
                values[7] = (byte)(value & 0x00FF);
            }
        }

        public static void SetFlag(int flag, int value)
        {
            byte[] vals = { 0x80, 0x40, 0x20, 0x10 };
            values[1] -= (byte)(GetFlags()[flag] * vals[flag]);
            values[1] += (byte)(value * vals[flag]);
        }

        public static void SetFlags(byte[] values)
        {
            SetFlag(0, values[0]);
            SetFlag(1, values[1]);
            SetFlag(2, values[2]);
            SetFlag(3, values[3]);
        }

        public static byte[] GetFlags()
        {
            byte register = values[1];
            byte[] flags = new byte[4];

            if (register >= 0x80)
            {
                flags[0] = 1;
                register -= 0x80;
            }

            if (register >= 0x40)
            {
                flags[1] = 1;
                register -= 0x40;
            }

            if (register >= 0x20)
            {
                flags[2] = 1;
                register -= 0x20;
            }

            if (register >= 0x10)
            {
                flags[3] = 1;
            }
            return flags;
        }
    }
}
