using System;
using Bro.Network.TransmitProtocol;

namespace Bro
{
    public static class DataTableConverter
    {
        private static readonly DataTableParam Param = new DataTableParam();
        private static readonly IWriter Writer = DataWriter.GetBinaryWriter(4096);

        private static DataTable FromByteArray(byte[] array)
        {
            var reader = DataReader.GetBinaryReader(array);
            Param.Read(reader);
            return Param.Value;
        }

        private static byte[] ToByteArray(DataTable table)
        {
            Writer.Reset();
            Param.Value = table;
            Param.Write(Writer);
            return Writer.Data;
        }

        public static string ToBase64(DataTable table)
        {
            var data = ToByteArray(table);
            return Convert.ToBase64String(data);
        }

        public static DataTable FromBase64(string base64)
        {
            var data = Convert.FromBase64String(base64);
            return FromByteArray(data);
        }
    }
}