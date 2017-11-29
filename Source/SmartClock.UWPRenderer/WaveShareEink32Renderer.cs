﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SmartClock.Core;
namespace SmartClock.UWPRenderer
{
    public class WaveShareEink32Renderer : IClockRenderer
    {
        public RenderInfo Info => new RenderInfo() { Name = "WaveShareEInk32", Version = "1.0.0" };
        Eink32Device device;
        public void Render(Image<Rgba32> image)
        {
            if (image==null)
            {
                return;
            }
            if (device == null)
            {
                device = new Eink32Device();
                device.InitAsync().Wait();
                device.Reset().Wait();
            }
            var buffer = createFromImage(image);
            device.DisplayFrameAsync(buffer).Wait();
        }
        private byte[] createFromImage(Image<Rgba32> img)
        {
            var source = img.SavePixelData();
            byte[] result = new byte[source.Length / 4 / 8];
            int pos = 0;
            byte tmp = 0;
            byte[] masks =
            {
                0x80, //1000 0000
                0x40, //0100 0000
                0x20, //0010 0000
                0x10, //0001 0000
                0x08, //0000 1000
                0x04, //0000 0100
                0x02, //0000 0010
                0x01, //0000 0001
            };
            for (int i = 0; i < result.Length; i++)
            {
                tmp = 0;
                for (int j = 0; j < 8; j++)
                {
                    byte r = source[pos++];
                    byte g = source[pos++];
                    byte b = source[pos++];
                    byte a = source[pos++];//not used
                    if (!(r == 0 && b == 0 && g == 0))
                    {
                        tmp += masks[j];
                    }
                }
                result[i] = tmp;
            }
            return result;
        }
    }
}
