using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreTest.Helpes
{
    public class HashFactory
    {
        public static string GetHash(object entity)
        {
            string result = string.Empty;
            string json = JsonConvert.SerializeObject(entity);
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                result = BitConverter.ToString(hash);
                result = result.Replace("-", "");
            }
            return result;
        }

    }
}
