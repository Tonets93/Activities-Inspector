﻿using System;
using System.Linq;

namespace ProgettoInformaticaForense_Argentieri.Utils
{
    public class FileMetric
    {
        public FileMetric(byte[] rawBytes, bool ver17)
        {
            if (ver17)
            {
                Unknown0 = BitConverter.ToInt32(rawBytes, 0);
                Unknown1 = BitConverter.ToInt32(rawBytes, 4);

                FilenameStringOffset = BitConverter.ToInt32(rawBytes, 8);
                FilenameStringSize = BitConverter.ToInt32(rawBytes, 12);

                Unknown2 = BitConverter.ToInt32(rawBytes, 16);
            }
            else
            {
                Unknown0 = BitConverter.ToInt32(rawBytes, 0);
                Unknown1 = BitConverter.ToInt32(rawBytes, 4);
                Unknown2 = BitConverter.ToInt32(rawBytes, 8);

                FilenameStringOffset = BitConverter.ToInt32(rawBytes, 12);
                FilenameStringSize = BitConverter.ToInt32(rawBytes, 16);

                Unknown3 = BitConverter.ToInt32(rawBytes, 20);

                MFTInfo = new MFTInformation(rawBytes.Skip(24).Take(8).ToArray());
            }
        }

        public int Unknown0 { get; }
        public int Unknown1 { get; }
        public int FilenameStringOffset { get; }
        public int FilenameStringSize { get; }
        public int Unknown2 { get; }
        public int Unknown3 { get; }
        public MFTInformation MFTInfo { get; }

        public override string ToString()
        {
            return $"Filename offset: {FilenameStringOffset}, Size: {FilenameStringSize}, MFT: {MFTInfo}";
        }
    }
}
