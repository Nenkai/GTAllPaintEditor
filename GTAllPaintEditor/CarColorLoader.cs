using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using Syroot.BinaryData;

namespace GTAllPaintEditor
{
    public class CarColorLoader
    {
        public const int PaintSizeGT6 = 0x20E0;

        public List<PaintEntry> PaintEntries { get; set; } = new List<PaintEntry>();
        public string _fileName;

        public CarColorLoader(string file)
        {
            _fileName = file;
        }

        public void Load()
        {
            using (var fs = new FileStream(_fileName, FileMode.Open))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                long paintCount = bs.Length / PaintSizeGT6;

                for (int i = 0; i < paintCount; i++)
                {
                    bs.Position = i * PaintSizeGT6;

                    PaintEntry paint = new PaintEntry(bs.ReadInt32());
                    paint.UnkID = bs.ReadInt32();

                    bs.Position += 8;

                    paint.Chrome = bs.ReadSingle();

                    paint.Unk1 = bs.Read1Byte();
                    paint.Unk2 = bs.Read1Byte();
                    paint.Unk3 = bs.Read1Byte();
                    paint.Unk4 = bs.Read1Byte();

                    paint.Reflectiveness = bs.ReadSingle();
                    bs.Position += 12;

                    paint.ReflectPowerR = bs.ReadSingle();
                    paint.ReflectPowerG = bs.ReadSingle();
                    paint.ReflectPowerB = bs.ReadSingle();
                    bs.ReadSingle();

                    paint.ColorBuffer = bs.ReadInt32s(64 * 32, ByteConverter.Little);
                    paint.ColorBuffer2 = bs.ReadInt32s(32, ByteConverter.Little);

                    PaintEntries.Add(paint);
                }
            }
        }

        public void Save(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create))
            using (var bs = new BinaryStream(fs, ByteConverter.Big))
            {
                for (int i = 0; i < PaintEntries.Count; i++)
                {
                    var paint = PaintEntries[i];
                    bs.WriteInt32(paint.ID);
                    bs.WriteInt32(paint.UnkID);
                    bs.Position += 8;
                    bs.WriteSingle(paint.Chrome);
                    bs.WriteByte(paint.Unk1);
                    bs.WriteByte(paint.Unk2);
                    bs.WriteByte(paint.Unk3);
                    bs.WriteByte(paint.Unk4);
                    bs.WriteSingle(paint.Reflectiveness);
                    bs.Position += 12;

                    bs.WriteSingle(paint.ReflectPowerR);
                    bs.WriteSingle(paint.ReflectPowerG);
                    bs.WriteSingle(paint.ReflectPowerB);
                    bs.WriteSingle(0);

                    bs.WriteInt32s(paint.ColorBuffer, ByteConverter.Little);
                    bs.WriteInt32s(paint.ColorBuffer2, ByteConverter.Little);

                    for (int j = 0; j < 40; j++)
                        bs.WriteByte(0); // Pad
                }
            }
        }

        public Bitmap GetPaintImageBuffer(int index)
        {
            var paintEntry =  PaintEntries[index];
            return GetBitmapFromColorBuffer(paintEntry.ColorBuffer, 64, 32);
        }

        public Bitmap GetPaintImageBuffer2(int index)
        {
            var paintEntry = PaintEntries[index];
            return GetBitmapFromColorBuffer(paintEntry.ColorBuffer2, 32, 1);
        }

        private Bitmap GetBitmapFromColorBuffer(int[] argb, int width, int height)
        {
            int x = 0;
            int y = 0;

            Bitmap img = new Bitmap(width, height);

            for (int i = 0; i < argb.Length; i++)
            {
                img.SetPixel(x, y, Color.FromArgb(argb[i]));

                x++;
                if (x == width)
                {
                    x = 0;
                    y++;

                }

            }

            return img;
        }
    }
}
