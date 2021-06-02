using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace GTAllPaintEditor
{
    public class PaintEntry
    {
        public int ID { get; set; }
        public int UnkID { get; set; }

        public float Chrome { get; set; }
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
        public byte Unk4 { get; set; }

        public float Reflectiveness { get; set; }
        public float ReflectPowerR { get; set; }
        public float ReflectPowerG { get; set; }
        public float ReflectPowerB { get; set; }

        public int[] ColorBuffer { get; set; }
        public int[] ColorBuffer2 { get; set; }

        public PaintEntry(int id)
        {
            ID = id;
        }

        public override string ToString()
        {
            return $"ID: {ID}";
        }

        public void SetColorBuffer(Bitmap image)
        {

            int i = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    ColorBuffer[i++] = image.GetPixel(x, y).ToArgb();
                }
            }

        }

        public void SetColorBuffer2(Bitmap image)
        {

            int i = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    ColorBuffer2[i++] = image.GetPixel(x, y).ToArgb();
                }
            }

        }
    }
}
