﻿using System;
using System.Linq;
using System.Text;

namespace ProgettoInformaticaForense_Argentieri.ExtraData.ExtraData
{
    public class EnvironmentVariableDataBlock : ExtraDataBase
    {
        public EnvironmentVariableDataBlock(byte[] rawBytes)
        {
            Signature = ExtraDataTypes.EnvironmentVariableDataBlock;

            Size = BitConverter.ToUInt32(rawBytes, 0);

            EnvironmentVariablesAscii = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, 8, 260).Split('\0').First();
            EnvironmentVariablesUnicode = Encoding.Unicode.GetString(rawBytes, 268, 520).Split('\0').First();
        }

        public string EnvironmentVariablesAscii { get; }
        public string EnvironmentVariablesUnicode { get; }


        public override string ToString()
        {
            return $"Environment variable data block" +
                   $"\r\nEnvironment variables Ascii: {EnvironmentVariablesAscii}" +
                   $"\r\nEnvironment variables unicode: {EnvironmentVariablesUnicode}";
        }
    }
}