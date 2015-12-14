using System;
using System.Drawing;
using System.IO;

namespace Alfred.Utils
{
    #region Structures
    public struct WaveHeader
    {
        public string RIIFTAG;
        public int SIZE1;
        public string WAVETAG;
        public string FMTTAG;
        public int LGDEF;
        public short FORMAT;
        public short NBCANAUX;
        public int FREQ;
        public int BYTEPERSEC;
        public short NBRBYTE;
        public short NBRBITS;
    }

    public struct Sample8Bit
    {
        public byte Left;
        public byte Right;
    }

    public struct Sample16Bit
    {
        public short Left;
        public short Right;
    }

    public struct WaveData
    {
        public string DATATAG;
        public int DataSize;

        public Sample8Bit[] Samples8Bit;
        public Sample16Bit[] Samples16Bit;
    }
    #endregion

    public class CWave
    {
        public WaveHeader waveHeader;
        public WaveData waveData;

        private byte[] _RawData;
        private int SeekPtr;

        public CWave(string Filename)
        {
            GetRawData(Filename);

            waveHeader = ReadWaveHeader();
            waveData = ReadWaveData();
        }

        public void Dispose()
        {

        }

        #region Wave Reading
        #region Read Tools
        private void GetRawData(string Filename)
        {
            var FS = new FileStream(Filename, FileMode.Open);
            var BR = new BinaryReader(FS);

            _RawData = BR.ReadBytes((int)FS.Length);

            BR.Close();
        }

        private string ReadString(int Lenght)
        {
            var ReadedString = String.Empty;

            for (var i = 0; i < Lenght; i++)
            {
                ReadedString += (char)_RawData[SeekPtr];
                SeekPtr++;
            }

            return ReadedString;
        }

        private byte ReadByte()
        {
            SeekPtr += 1;
            return _RawData[SeekPtr - 1];
        }

        private short ReadShort()
        {
            SeekPtr += 2;
            return BitConverter.ToInt16(_RawData, SeekPtr - 2);
        }

        private int ReadInt()
        {
            SeekPtr += 4;
            return BitConverter.ToInt32(_RawData, SeekPtr - 4);
        }

        private short ReadLittleEndienShort()
        {
            var BigEndienInt = new[] { _RawData[SeekPtr + 1], _RawData[SeekPtr] };
            SeekPtr += 2;

            return BitConverter.ToInt16(BigEndienInt, 0);
        }
        #endregion

        #region Header
        private WaveHeader ReadWaveHeader()
        {
            var WVHeader = new WaveHeader();

            //TAG1       (4 octets) : Constante « RIFF »         (0x52,0x49,0x46,0x46)
            WVHeader.RIIFTAG = ReadString(4);

            //SIZE1      (4 octets) : Taille du fichier moins 8 octets
            WVHeader.SIZE1 = ReadInt();

            //FORMAT     (4 octets) : Format = « WAVE »      (0x57,0x41,0x56,0x45)
            WVHeader.WAVETAG = ReadString(4);

            //TAG2       (4 octets) : Identifiant « fmt »   (0x66,0x6D,0x74,0x20)
            WVHeader.FMTTAG = ReadString(4);

            //LGDEF      (4 octets) : Nombre de bit d'un chunk (Left + Right)
            WVHeader.LGDEF = ReadInt();

            //FORMAT     (2 octets) : Format de fichier (1: PCM,  ...)
            WVHeader.FORMAT = ReadShort();

            //NBCANAUX   (2 octets) : Nombre de canaux (1 pour mono ou 2 pour stéréo)
            WVHeader.NBCANAUX = ReadShort();

            //FREQ       (4 octets) : Fréquence d'échantillonnage (en Hertz)
            WVHeader.FREQ = ReadInt();

            //BYTEPERSEC (4 octets) : Nombre d'octets par seconde de musique
            WVHeader.BYTEPERSEC = ReadInt();

            //NBRBYTE    (2 octets) : Nombre d'octets par échantillon
            WVHeader.NBRBYTE = ReadShort();

            //NBBITS     (2 octets) : Nombre de bits par donnée
            WVHeader.NBRBITS = ReadShort();

            return WVHeader;
        }
        #endregion

        #region Data
        private WaveData ReadWaveData()
        {
            var WVData = new WaveData();

            WVData.DATATAG = ReadString(4);
            WVData.DataSize = waveHeader.SIZE1 - waveHeader.NBRBITS*waveHeader.NBRBYTE;

            var ChunkCount = WVData.DataSize / waveHeader.NBRBYTE;

            if (waveHeader.NBRBITS == 8)
            {
                WVData.Samples8Bit = Read8BitWav(ChunkCount);
                WVData.Samples16Bit = Complete16BitWav(WVData.Samples8Bit);
            }

            if (waveHeader.NBRBITS == 16)
            {
                WVData.Samples16Bit = Read16BitWav(ChunkCount);
                WVData.Samples8Bit = Complete8BitWav(WVData.Samples16Bit);
            }

            return WVData;
        }

        private Sample8Bit[] Read8BitWav(int ChunkCount)
        {
            var Chunks = new Sample8Bit[ChunkCount];

            for (var i = 0; i < ChunkCount; i++)
            {
                if (waveHeader.NBCANAUX == 1)
                {
                    Chunks[i].Left = ReadByte();
                    Chunks[i].Right = Chunks[i].Left;
                }
                else
                {
                    Chunks[i].Left = ReadByte();
                    Chunks[i].Right = ReadByte();
                }
            }

            return Chunks;
        }
        private Sample16Bit[] Read16BitWav(int ChunkCount)
        {
            var Chunks = new Sample16Bit[ChunkCount];

            for (var i = 0; i < ChunkCount; i++)
            {
                if (waveHeader.NBCANAUX == 1)
                {
					try
					{
						Chunks[i].Left = ReadShort();
						Chunks[i].Right = Chunks[i].Left;
					}
					catch (Exception e)
					{
						var s = e.Message;
					}
                }
                else
                {
                    Chunks[i].Left = ReadShort();
                    Chunks[i].Right = ReadShort();
                }
            }

            return Chunks;
        }

        public static Sample16Bit[] Complete16BitWav(Sample8Bit[] Samples)
        {
            var Chunks = new Sample16Bit[Samples.Length];

            for (var i = 0; i < Chunks.Length; i++)
            {
                    Chunks[i].Left = (short)((Samples[i].Left - 128) * (int)(32768.0f / 255.0f));
                    Chunks[i].Right = (short)((Samples[i].Right - 128) * (int)(32768.0f / 255.0f));
            }

            return Chunks;
        }
        public static Sample8Bit[] Complete8BitWav(Sample16Bit[] Samples)
        {
            var Chunks = new Sample8Bit[Samples.Length];

            for (var i = 0; i < Chunks.Length; i++)
            {
                Chunks[i].Left = (byte)((Samples[i].Left + 32768.0f) * (255.0f / 65536.0f));
                Chunks[i].Right = (byte)((Samples[i].Right + 32768.0f) * (255.0f / 65536.0f));
            }

            return Chunks;
        }
        #endregion
        #endregion

        public Bitmap DrawWaveForm(int Width, int Height, Point[] data)
        {
            var tmpBitmap = new Bitmap(Width, Height);
            var PointList = new Point[Width];

            for (var i = 0; i < Width; i++)
                PointList[i] = new Point(data[i].X, (Height / 2) + data[i].Y);

            var Gr = Graphics.FromImage(tmpBitmap);
			try
			{
				Gr.DrawCurve(new Pen(Color.Black), PointList);
			}
			catch (Exception e)
			{
				var s = e.Message;
			}

            return tmpBitmap;
        }
    }
}
