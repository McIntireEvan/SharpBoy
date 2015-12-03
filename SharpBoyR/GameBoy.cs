using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpBoyR
{
    class GameBoy
    {
        public Debug debug;
        MainWindow parent;
        ROM rom;

        ushort PC;
        ushort SP;

        byte[] memory;

        public GameBoy(MainWindow parent, ROM rom)
        {
            this.parent = parent;
            this.rom = rom;
            memory = new byte[0xFFFF];

            SP = 0xFFFE;
            PC = 0;

            Registers.Init();

            LoadBios();
            for (int i = 0x100; i < 0x2000; i++)
            {
                memory[i] = rom.pages[0, i];
            }

            memory[0xff44] = 0x90; //Makes things work for some reason, remove later
            debug = new Debug();

            debug.LogLine("Test");
        }

        #region BIOS
        public void LoadBios()
        {
            byte[] bios = LoadFile("bios.gb");
            for (int i = 0; i < bios.Length; i++)
            {
                memory[i] = bios[i];
            }
        }

        public void OverwriteBios()
        {
            for (int i = 0; i < 0x100; i++)
            {
                memory[i] = rom.pages[0, i];
            }
        }
        #endregion

        public byte[] LoadFile(string file)
        {
            byte[] hex;
            using (Stream reader = File.OpenRead(file))
            {
                hex = new byte[reader.Length];
                int i = 0;
                while ((reader.Position != reader.Length) && (i < hex.Length))
                {
                    hex[i] = (byte)(reader.ReadByte() & 0xFF);
                    i++;
                }
            }
            return hex;
        }

        public void Update()
        {
            byte opcode = memory[PC];
            ProcessOpcode(opcode);
        }

        public void ProcessOpcode(byte opcode)
        {
            //Log(ToHex(PC) + " :: " + ToHex(memory[0x11]));
            switch(opcode)
            {
                case 0x00: //NOP
                    {
                        PC++;
                        break;
                    }
                case 0x01: //LD BC, d16
                    {
                        Registers.BC = ReadTwoBytes();
                        PC += 3;
                        Log("BC :: " + ToHex(Registers.BC));
                        break;
                    }
                case 0x03: //INC BC
                    {
                        Registers.BC++;
                        PC++;
                        Log("INC BC :: " + Registers.BC);
                        break;
                    }
                case 0x04: //INC B
                    {
                        Registers.B = INC(Registers.B);
                        PC++;
                        break;
                    }
                case 0x05: //DEC C
                    {
                        Registers.B = DEC(Registers.B);
                        PC++;
                        break;
                    }
                case 0x06: //LD B, d8
                    {
                        Registers.B = ReadByte();
                        PC++;
                        Log("B :: " + Registers.B);
                        break;
                    }
                case 0x0A: //LD A, (BC)
                    {
                        Registers.A = memory[Registers.BC];
                        //TODO: Log
                        PC++;
                        break;
                    }
                case 0x0C: //INC C
                    {
                        Registers.C = INC(Registers.C);
                        PC++;
                        break;
                    }
                case 0x0D: //DEC C
                    {
                        Registers.C = DEC(Registers.C);
                        PC++;
                        break;
                    }
                case 0x0E: //LD c, d8
                    {
                        Registers.C = ReadByte();
                        Log("C :: " + ToHex(Registers.C));
                        PC += 2;
                        break;
                    }
                case 0x10: //STOP 0
                    {
                        //Stop CPU and LCD until button press
                        //TODO: Implement
                        Log("STOP");
                        Thread.Sleep(1000);
                        break;
                    }
                case 0x11: //LD DE, d16
                    {
                        Registers.DE = ReadTwoBytes();
                        PC += 3;
                        Log("DE :: " + ToHex(Registers.DE));
                        break;
                    }
                case 0x13: //INC DE
                    {
                        Registers.DE++;
                        PC++;
                        Log("INC DE :: " + Registers.DE);
                        break;
                    }
                case 0x14: //INC D
                    {
                        Registers.D = INC(Registers.D);
                        PC++;
                        break;
                    }
                case 0x15: //DEC D
                    {
                        Registers.D = DEC(Registers.D);
                        PC++;
                        break;
                    }
                case 0x16: //LD D, d8
                    {
                        Registers.D = ReadByte();
                        PC++;
                        Log("D :: " + Registers.D);
                        break;
                    }
                case 0x17: //RLA
                    {
                        RLA();
                        PC++;
                        break;
                    }
                case 0x1A: //LD A, (DE)
                    {
                        Registers.A = memory[Registers.DE];
                        //TODO: Log
                        PC++;
                        break;
                    }
                case 0x1C: //INC E
                    {
                        Registers.E = INC(Registers.E);
                        PC++;
                        break;
                    }
                case 0x1D: //DEC E
                    {
                        Registers.E = DEC(Registers.E);
                        PC++;
                        break;
                    }
                case 0x1E: //LD E, d8
                    {
                        Registers.E = ReadByte();
                        Log("E :: " + ToHex(Registers.E));
                        PC += 2;
                        break;
                    }
                case 0x20: //JR NZ, r8
                    {
                        JR("NZ");
                        break;
                    }
                case 0x21: //LD HL, d16
                    {
                        Registers.HL = ReadTwoBytes();
                        PC += 3;
                        Log("HL :: " + ToHex(Registers.HL));
                        break;
                    }
                case 0x22: //LD (HL+), A
                    {
                        LOAD(Registers.HL, Registers.A);
                        Registers.HL++;
                        PC++;
                        break;
                    }
                case 0x23: //INC HL
                    {
                        Registers.HL++;
                        PC++;
                        Log("INC HL :: " + Registers.HL);
                        break;
                    }
                case 0x24: //INC H
                    {
                        Registers.H = INC(Registers.H);
                        PC++;
                        break;
                    }
                case 0x25: //DEC H
                    {
                        Registers.H = DEC(Registers.H);
                        PC++;
                        break;
                    }
                case 0x26: //LD H, d8
                    {
                        Registers.H = ReadByte();
                        PC++;
                        Log("H :: " + Registers.H);
                        break;
                    }
                case 0x2C: //INC L
                    {
                        Registers.L = INC(Registers.L);
                        PC++;
                        break;
                    }
                case 0x2D: //DEC L
                    {
                        Registers.L = DEC(Registers.L);
                        PC++;
                        break;
                    }
                case 0x2E: //LD L, d8
                    {
                        Registers.L = ReadByte();
                        Log("L :: " + ToHex(Registers.L));
                        PC += 2;
                        break;
                    }
                case 0x30: //JR NC, r8
                    {
                        JR("NC");
                        break;
                    }
                case 0x31: //LD SP, d16
                    {
                        SP = ReadTwoBytes();
                        PC += 3;
                        Log("SP :: " + ToHex(SP));
                        break;
                    }
                case 0x32: //LD (HL-), A
                    {
                        LOAD(Registers.HL, Registers.A);
                        Registers.HL--;
                        PC++;
                        break;
                    }
                case 0x33: //INC SP
                    {
                        SP++;
                        PC++;
                        Log("INC SP :: " + SP);
                        break;
                    }
                case 0x3C: //INC A
                    {
                        Registers.A = INC(Registers.A);
                        PC++;
                        break;
                    }
                case 0x3D: //DEC A
                    {
                        Registers.A = DEC(Registers.A);
                        PC++;
                        break;
                    }
                case 0x3E: //LD a, d8
                    {
                        Registers.A = ReadByte();
                        Log("A :: " + ToHex(Registers.A));
                        PC += 2;
                        break;
                    }
                case 0x4F: //LD C, (HL)
                    {
                        Registers.C = memory[Registers.HL];
                        PC++;
                        //TODO: Log;
                        break;
                    }
                case 0x70: //LD (HL), B
                    {
                        LOAD(Registers.HL, Registers.B);
                        PC++;
                        break;
                    }
                case 0x71: //LD (HL), C
                    {
                        LOAD(Registers.HL, Registers.C);
                        PC++;
                        break;
                    }
                case 0x72: //LD (HL), D
                    {
                        LOAD(Registers.HL, Registers.D);
                        PC++;
                        break;
                    }
                case 0x73: //LD (HL), E
                    {
                        LOAD(Registers.HL, Registers.E);
                        PC++;
                        break;
                    }
                case 0x74: //LD (HL), H
                    {
                        LOAD(Registers.HL, Registers.H);
                        PC++;
                        break;
                    }
                case 0x75: //LD (HL), L
                    {
                        LOAD(Registers.HL, Registers.L);
                        PC++;
                        break;
                    }
                case 0x77: //LD (HL), A
                    {
                        LOAD(Registers.HL, Registers.A);
                        PC++;
                        break;
                    }
                case 0x80: //ADD A,B
                    {
                        Registers.A = ADD(Registers.A, Registers.B);
                        PC++;
                        break;
                    }
                case 0x81: //ADD A,C
                    {
                        Registers.A = ADD(Registers.A, Registers.C);
                        PC++;
                        break;
                    }
                case 0x82: //ADD A,D
                    {
                        Registers.A = ADD(Registers.A, Registers.D);
                        PC++;
                        break;
                    }
                case 0x83: //ADD A,E
                    {
                        Registers.A = ADD(Registers.A, Registers.E);
                        PC++;
                        break;
                    }
                case 0x84: //ADD A,H
                    {
                        Registers.A = ADD(Registers.A, Registers.H);
                        PC++;
                        break;
                    }
                case 0x85: //ADD A,L
                    {
                        Registers.A = ADD(Registers.A, Registers.L);
                        PC++;
                        break;
                    }
                case 0x86: //ADD A,(HL)
                    {
                        Registers.A = ADD(Registers.A, memory[Registers.HL]);
                        PC++;
                        break;
                    }
                case 0x87: //ADD A,A
                    {
                        Registers.A = ADD(Registers.A, Registers.A);
                        PC++;
                        break;
                    }
                case 0xAF: //XOR A
                    {
                        XOR(Registers.A);
                        PC++;
                        Log("Z flag :: " + Registers.GetFlags()[0]);
                        break;
                    }
                case 0xC1: //POP BC
                    {
                        Registers.BC = POP();
                        PC++;
                        Log("POP :: " + Registers.BC);
                        break;
                    }
                case 0xC5: //PUSH BC
                    {
                        PUSH(Registers.BC);
                        PC++;
                        break;
                    }
                case 0xC9:
                    {
                        RET();
                        break;
                    }
                case 0xCB: //Instruction set 2
                    {
                        Log("Loading Instruction set 2");
                        byte code = ReadByte();
                        ProcessCBCode(code);
                        break;
                    }
                case 0xCD: //CALL a16
                    {
                        CALL(ReadTwoBytes());
                        break;
                    }
                case 0xD1: //POP DE
                    {
                        Registers.DE = POP();
                        PC++;
                        Log("POP :: " + Registers.DE);
                        break;
                    }
                case 0xD5: //PUSH DE
                    {
                        PUSH(Registers.DE);
                        PC++;
                        break;
                    }
                case 0xE0: //LDH (a8),A
                    {
                        LOAD((ushort)((ReadByte() & 0xFF) + 0xFF00), Registers.A);
                        Log("LDH");
                        PC += 2;
                        break;
                    }
                case 0xE2: //LD(C),A
                    {
                        LOAD((ushort)(0xFF00 & Registers.C), Registers.A);
                        PC++;
                        break;
                    }
                case 0xE1: //POP HL
                    {
                        Registers.HL = POP();
                        PC++;
                        Log("POP :: " + Registers.HL);
                        break;
                    }
                case 0xE5: //PUSH HL
                    {
                        PUSH(Registers.HL);
                        PC++;
                        break;
                    }
                case 0xF1: //POP AF
                    {
                        //TODO: FLAGSSSSSSS
                        Registers.AF = POP();
                        PC++;
                        Log("POP :: " + Registers.AF);
                        break;
                    }
                case 0xF3: //DI
                    {
                        //TODO: IMPLEMENT WITH INTERRUPTS
                        PC++;
                        break;
                    }
                case 0xF5: //PUSH AF
                    {
                        PUSH(Registers.AF);
                        PC++;
                        break;
                    }
                case 0xFB: //EI
                    {
                        //TODO: IMPLEMENT WITH INTERRUPTS
                        PC++;
                        break;
                    }
                case 0xFE: //CP d8
                    {
                        SUB(ReadByte());
                        PC+=2;
                        break;
                    }
                default:
                    {
                        Log("NYI :: " + ToHex(opcode));
                        Thread.Sleep(1000);
                        break;
                    }
            }
        }

        public void ProcessCBCode(byte code)
        {
            switch(code)
            {
                case 0x11:
                    {
                        Registers.C = RL(Registers.C);
                        PC += 2;
                        break;
                    }
                case 0x7C: //BIT 7,H
                    {
                        BIT(Registers.H, 7);
                        PC += 2;
                        break;
                    }
                default:
                    {
                        Log(ToHex(code));
                        Thread.Sleep(1000);
                        break;
                    }
            }
        }

        #region CPU Instructions
        private void LOAD(ushort location, byte value)
        {
            memory[location] = value;
            //TODO: Check for VRAM writing
            Log("LD " + ToHex(location) + ", " + ToHex(value));
        }

        private void XOR(byte value)
        {
            Registers.A ^= value;
            byte Z = Registers.A == 0 ? (byte)1 : (byte)0;
            Registers.SetFlags(new byte[4] { Z, 0, 0, 0 });
            //TODO: Log
        }

        private void BIT(byte value, byte pos)
        {
            bool isSet = (value & (1 << pos)) == 1;
            int Z = isSet == true ? 1 : 0;

            Registers.SetFlags(new byte[4] { (byte)Z, 0, 1, Registers.GetFlags()[3]});
            //TODO: Log
        }

        private void JR(string flag)
        {
            bool jump = false;
            switch (flag.ToLower())
            {
                case "nz":
                    {
                        if(Registers.GetFlags()[0] != 0)
                            jump = true;
                        break;
                    }
                case "z":
                    {
                        if (Registers.GetFlags()[0] == 0)
                            jump = true;
                        break;
                    }
                case "c":
                    {
                        if (Registers.GetFlags()[3] != 0)
                            jump = true;
                        break;
                    }
                case "nc":
                    {
                        if (Registers.GetFlags()[0] == 0)
                            jump = true;
                        break;
                    }
                case "":
                    {
                        jump = true;
                        break;
                    }
            }

            if(jump)
            {
                char jPos = (char)ReadByte();
                if (jPos > 0x7f)
                {
                    //This converts the hex to a signed char
                    //First the bits are flipped (1000 to 0111, for example)
                    //Then 1 is added to that number, and it is masked with 0xFF just in case
                    //Then it is made negative like it should be
                    jPos = (char)(-((~jPos + 1) & 0xFF));
                }

                //Ignore the JR NZ instruction and the address after it

                PC += jPos;
            }

            PC += 2;
            //TODO: Log
        }

        private void JP()
        {

        }

        private byte INC(byte val)
        {
            val++;
            int z = val == 0 ? 1 : 0;
            int h = (val & 0xF) < ((val - 1) & 0xF) ? 1 : 0;
            Registers.SetFlags(new byte[4] { (byte)z, 0, (byte)h, Registers.GetFlags()[3] });
            return val;
        }

        private byte DEC(byte val)
        {
            val--;
            int z = val == 0 ? 1 : 0;
            int h = (val & 0xF) < ((val - 1) & 0xF) ? 1 : 0;
            Registers.SetFlags(new byte[4] { (byte)z, 1, (byte)h, Registers.GetFlags()[3] });
            return val;
        }

        private void CALL(ushort location)
        {
            memory[SP - 1] = (byte)((PC + 2) & 0xFF);
            memory[SP] = (byte)((PC + 1) << 8 & 0xFF);
            SP -= 2;

            PC = location;
            Log("Call :: " + ToHex(PC));
        }

        private void RET()
        {
            SP += 2;
            byte l = memory[SP];
            byte h = memory[SP - 1];

            PC = (ushort)((h & 0xFF00) + l);
            PC++;
            //TODO: Log
        }

        private void PUSH(ushort value)
        {
            memory[SP] = (byte)(value & 0xFF);
            memory[SP - 1] = (byte)((value >> 8) & 0xFF);
            SP -= 2;
            Log("PUSH :: " + value);
        }

        private ushort POP()
        {
            SP += 2;
            byte l = memory[SP];
            byte h = memory[SP - 1];
            //TODO: Log
            return (ushort)((h & 0xFF) + l);
        }

        private byte RL(byte value)
        {
            byte shifted = CircularLeftShift(value);
            //Z 0 0 C

            byte C = Registers.GetFlags()[3];
            byte carry = (byte)(shifted & 1);

            if (C != carry)
            {
                shifted ^= 1;
            }
            Registers.SetFlags(new byte[4] { (byte)((shifted == 0) ? 1 : 0), 0, 0, carry });
            //TODO: Log and ensure correctness
            return shifted;
        }

        private void RLA()
        {
            Registers.A = CircularLeftShift(Registers.A);
            byte C = Registers.GetFlags()[3];
            byte carry = (byte)(Registers.A & 1);
            
            if (C != carry)
            {
                Registers.A ^= 1;
            }
            Registers.SetFlag(3, carry);
        }

        private byte ADD(byte value, byte add)
        {
            byte nValue = (byte)(value + add);
            int Z = nValue == 0 ? 1 : 0;
            int C = ((value) > nValue) ? 1 : 0;
            int H = (nValue & 0xF) > (value & 0xF) ? 1 : 0;
            Registers.SetFlags(new byte[4] { (byte)Z, 0, (byte)H, (byte)C });
            //TODO: Log
            return nValue;
        }

        private byte SUB(byte value)
        {
            byte nValue = (byte)(Registers.A - value);
            int Z = nValue == 0 ? 1 : 0;
            int C = ((Registers.A - value) > Registers.A) ? 1 : 0;
            int H = (Registers.A & 0xF) - value > (Registers.A & 0xF) ? 1 : 0;
            Registers.SetFlags(new byte[4] { (byte)Z, 1, (byte)H, (byte)C });
            //TODO: Log
            return nValue;
        }
        #endregion

        /// <summary>
        /// Shifts the bits over in a circular manner
        /// The most efficient and short way to do it is to shift it left/right the amount you need,
        /// and bitwise OR it with the same value shifted the opposite way 8-shiftAmount.
        /// This is demonstrated below
        /// 00111100 << 3 | 00111100 >> (8 - 3)
        /// 11100000 | 00000001
        /// 11100001
        /// </summary>
        public static byte CircularLeftShift(byte value)
        {
            return (byte)((value << 1) | (value >> (7)));
        }

        public void Log(string s)
        {
            debug.Dispatcher.Invoke(new Action(() => {
                debug.LogLine(s);
            }));
        }

        public byte ReadByte()
        {
            return memory[PC + 1];
        }

        public ushort ReadTwoBytes()
        {
            return (ushort)((memory[PC + 2] << 8) + (memory[PC + 1]));
        }

        #region utils
        public static string ToHex(byte val)
        {
            return "0x" + val.ToString("x");
        }

        public static string ToHex(ushort val)
        {
            return "0x" + val.ToString("x");
        }

        public static string ToHex(int val)
        {
            return "0x" + val.ToString("x");
        }
        #endregion
    }
}