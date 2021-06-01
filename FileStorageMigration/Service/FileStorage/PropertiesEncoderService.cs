using FileStorageMigration.Model.FileStorage;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace FileStorageMigration.Service.FileStorage
{
    public class PropertiesEncoderService
    {
        private readonly byte[] _key;

        public PropertiesEncoderService(string key)
        {
            using (var shA256 = new SHA256Managed())
            {
                _key = shA256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        public string Encode(Properties props)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(props, stream);
                var data = Protect(stream.GetBuffer(), (int)stream.Length);
                return EncodeBase64Url(data);
            }
        }

        public Properties Decode(string text)
        {
            var data = DecodeBase64Url(text);
            data = Unprotect(data);
            using (var stream = new MemoryStream(data))
            {
                return Deserialize(stream);
            }
        }

        private byte[] Protect(byte[] data, int length)
        {
            byte[] hash;
            using (var shA256 = new SHA256Managed())
                hash = shA256.ComputeHash(data, 0, length);

            using (var aes = new AesManaged())
            {
                aes.Key = _key;
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        memoryStream.Write(aes.IV, 0, 16);
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var binaryWriter = new BinaryWriter(cryptoStream))
                            {
                                binaryWriter.Write(hash);
                                binaryWriter.Write(length);
                                binaryWriter.Write(data, 0, length);
                            }
                        }
                        return memoryStream.ToArray();
                    }
                }
            }
        }


        private byte[] Unprotect(byte[] data)
        {
            using (Aes aes = new AesManaged())
            {
                aes.Key = _key;
                {
                    byte[] buffer1 = new byte[16];
                    using (var source = new MemoryStream(data))
                    {
                        source.Read(buffer1, 0, 16);
                        aes.IV = buffer1;

                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        {
                            using (var cryptoStream = new CryptoStream(source, decryptor, CryptoStreamMode.Read))
                            {
                                using (var binaryReader = new BinaryReader(cryptoStream))
                                {
                                    byte[] numArray = binaryReader.ReadBytes(32);
                                    int count = binaryReader.ReadInt32();
                                    byte[] buffer2 = binaryReader.ReadBytes(count);
                                    byte[] hash;
                                    using (var shA256 = new SHA256Managed())
                                        hash = shA256.ComputeHash(buffer2);
                                    if (!hash.SequenceEqual(numArray))
                                        throw new SecurityException("Signature does not match the computed hash");
                                    return buffer2;
                                }
                            }
                        }
                    }
                }
            }
        }

        [Serializable]
        private sealed class PropertyItem
        {
            public byte[] Key { get; set; }
            public byte[] Value { get; set; }
        }

        private void Serialize(Properties data, Stream stream)
        {
            var f = new BinaryFormatter();

            var array = data._dict.Select(x => new PropertyItem()
            {
                Key = Encoding.UTF8.GetBytes(x.Key),
                Value = Encoding.UTF8.GetBytes(x.Value)
            }).ToArray();

            f.Serialize(stream, array);
        }

        private Properties Deserialize(MemoryStream stream)
        {
            var f = new BinaryFormatter();

            var array = (PropertyItem[])f.Deserialize(stream);

            return new Properties(array.ToDictionary(x => Encoding.UTF8.GetString(x.Key), x => Encoding.UTF8.GetString(x.Value)));

        }

        private string EncodeBase64Url(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private byte[] DecodeBase64Url(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));


            return Convert.FromBase64String(Pad(text.Replace('-', '+').Replace('_', '/')));
        }

        private static string Pad(string text)
        {
            int count = 3 - (text.Length + 3) % 4;
            if (count == 0)
                return text;
            return text + new string('=', count);
        }
    }
}
