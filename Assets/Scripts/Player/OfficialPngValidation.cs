using System;
using System.IO;
using System.Text;

namespace Pythime
{
    public static class OfficialPngValidation
    {
        private static readonly byte[] Signature = { 137, 80, 78, 71, 13, 10, 26, 10 };

        public static string DescribeFile(string path, string reason)
        {
            try
            {
                if (!File.Exists(path)) return $"Pythime: {path} | arquivo encontrado: não | motivo: {reason}";
                var bytes = File.ReadAllBytes(path);
                return $"Pythime: {path} | arquivo encontrado: sim | tamanho: {bytes.Length} bytes | assinatura: {BitConverter.ToString(bytes, 0, Math.Min(8, bytes.Length)).Replace('-', ' ')} | motivo: {reason}";
            }
            catch (Exception ex) { return $"Pythime: {path} | motivo: {reason} | leitura: {ex.Message}"; }
        }

        public static void ValidateFile(string path) => Validate(File.ReadAllBytes(path));

        public static void Validate(byte[] bytes)
        {
            if (bytes.Length < 8) throw new InvalidDataException("Arquivo curto demais para um PNG.");
            for (int i = 0; i < 8; i++)
                if (bytes[i] != Signature[i]) throw new InvalidDataException("Assinatura PNG inválida; extensão incorreta ou arquivo corrompido.");
            bool header = false, data = false;
            int offset = 8;
            while (offset < bytes.Length)
            {
                if (bytes.Length - offset < 12) throw new InvalidDataException("Cabeçalho de bloco PNG truncado.");
                uint length = ReadUInt(bytes, offset);
                string type = Encoding.ASCII.GetString(bytes, offset + 4, 4);
                if (length > bytes.Length - offset - 12) throw new InvalidDataException($"Bloco {type} truncado: declara {length} bytes; restam {bytes.Length - offset - 12}.");
                int count = (int)length;
                uint crc = 0xffffffff;
                for (int i = offset + 4; i < offset + 8 + count; i++)
                {
                    crc ^= bytes[i];
                    for (int bit = 0; bit < 8; bit++) crc = (crc >> 1) ^ ((crc & 1) != 0 ? 0xedb88320u : 0u);
                }
                if ((crc ^ 0xffffffff) != ReadUInt(bytes, offset + 8 + count)) throw new InvalidDataException($"CRC inválido no bloco {type}.");
                if (!header && type != "IHDR") throw new InvalidDataException("IHDR ausente no início do PNG.");
                if (type == "IHDR")
                {
                    if (header || count != 13 || ReadUInt(bytes, offset + 8) == 0 || ReadUInt(bytes, offset + 12) == 0)
                        throw new InvalidDataException("IHDR inválido.");
                    header = true;
                }
                if (type == "IDAT") data = true;
                offset += count + 12;
                if (type == "IEND")
                {
                    if (count != 0 || !data || offset != bytes.Length) throw new InvalidDataException("IEND inválido ou dados extras após o PNG.");
                    return;
                }
            }
            throw new InvalidDataException("PNG sem bloco IEND.");
        }

        private static uint ReadUInt(byte[] b, int i) => ((uint)b[i] << 24) | ((uint)b[i + 1] << 16) | ((uint)b[i + 2] << 8) | b[i + 3];
    }
}
