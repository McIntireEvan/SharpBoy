using System.IO;

namespace SharpBoyR
{
    class ROM
    {
        public byte[,] pages;

        public ROM(string file)
        {
            Stream reader = File.OpenRead(file);
            long pageNum = reader.Length / (0x2000);

            pages = new byte[pageNum, 0x2000];

            for (int i = 0; i < pageNum; i++)
            {
                for (int j = 0; j < 0x2000; j++)
                {
                    pages[i, j] = (byte)(reader.ReadByte() & 0xFF);
                }
            }
        }

        public string Name
        {
            get
            {
                string name = "";
                for (int i = 0x134; i < 0x142; i++)
                {
                    name += (char)pages[0, i];
                }
                return name;
            }
        }
    }
}
