﻿using System;
using System.Text;

namespace ProgettoInformaticaForense_Argentieri.Utils
{
    public class Header
    {
        public Header(byte[] rawBytes)
        {
            var index = 0;

            var ver = BitConverter.ToInt32(rawBytes, 0);

            switch (ver)
            {
                case (int)Version.WinXpOrWin2K3:
                    Version = Version.WinXpOrWin2K3;
                    break;
                case (int)Version.VistaOrWin7:
                    Version = Version.VistaOrWin7;
                    break;
                case (int)Version.Win8xOrWin2012x:
                    Version = Version.Win8xOrWin2012x;
                    break;
                case (int)Version.Win10:
                    Version = Version.Win10;
                    break;
            }

            index += 4; //version

            Signature = Encoding.ASCII.GetString(rawBytes, index, 4);

            index += 4; //signature
            index += 4; //unknown

            FileSize = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            var tempName = Encoding.Unicode.GetString(rawBytes, index, 60);
            ExecutableFilename = tempName.Substring(0, tempName.IndexOf('\0')).Trim();

            index += 60;

            Hash = BitConverter.ToInt32(rawBytes, index).ToString("X");
        }

        public Version Version { get; }
        public string Signature { get; }

        public int FileSize { get; }

        public string ExecutableFilename { get; }

        public string Hash { get; }

        public override string ToString()
        {
            return
                $"Version: {Version}, Sig: {Signature}, File size: {FileSize}, Executable name: {ExecutableFilename}, Hash: {Hash}";
        }
    }
}
