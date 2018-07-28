using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace myApp
{
    public class FexpayUtils
    {
        #region 公用函数
        public string Bytes2Hex(byte[] bytes)
        {
            var buffer = new StringBuilder();
            foreach (var t in bytes)
            {
                buffer.AppendFormat("{0:x2}", t);
            }

            return buffer.ToString();
        }

        public byte[] Hex2Bytes(string mHex)
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return null;
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return vBytes;
        }
        #endregion

        #region DES 加解密
        public string DesEncrypt(string pubKey, string input)
        {
            var des = new DESCryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            des.Key = Encoding.ASCII.GetBytes(pubKey);
            des.IV = Encoding.ASCII.GetBytes(pubKey);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                var data = ms.ToArray();
                return BitConverter.ToString(data).Replace("-", "");
            }
        }

        public string DesDecrypt(string pubKey, string input)
        {
            var des = new DESCryptoServiceProvider();
            var bytes = Hex2Bytes(input);
            des.Key = Encoding.ASCII.GetBytes(pubKey);
            des.IV = Encoding.ASCII.GetBytes(pubKey);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                var data = ms.ToArray();
                return Encoding.UTF8.GetString(data);
            }
        }

        public string md5(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] byteValue, byteHash;
                byteValue = System.Text.Encoding.UTF8.GetBytes(str);
                byteHash = md5.ComputeHash(byteValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < byteHash.Length; i ++)
                {
                    sTemp += byteHash[i].ToString("X").PadLeft(2, '0');
                }

                str = sTemp.ToUpper();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return str;
        }

        #endregion
    }

    //入金数据结构定义
    [DataContract]
    public class DepositDataStructure
    {
        [DataMember(Order = 0)]
        public string realName {get; set;}

        [DataMember(Order = 1)]
        public string amount {get; set;}

        [DataMember(Order = 2)]
        public string orderId {get; set;}

        [DataMember(Order = 3)]
        public string idCard {get; set;}

        [DataMember(Order = 4)]
        public string notifyUrl {get; set;}

        [DataMember(Order = 5)]
        public string returnUrl {get; set;}

        [DataMember(Order = 6)]
        public string sendTime {get; set;}
    }

    [DataContract]
    public class DepositCipherTextStructure
    {
        [DataMember(Order = 0)]
        public DepositDataStructure data {get; set;}

        [DataMember(Order = 1)]
        public string sign;
    }

    //出金检查数据结构定义
    [DataContract]
    public class CheckWithdrawStructure
    {
        [DataMember(Order = 0)]
        public string realName;

        [DataMember(Order = 1)]
        public string coinId;

        [DataMember(Order = 2)]
        public string amount;

        [DataMember(Order = 3)]
        public string idCard;
    }

    [DataContract]
    public class CheckWithdrawCipherTextStructure
    {
        [DataMember(Order = 0)]
        public CheckWithdrawStructure data {get; set;}

        [DataMember(Order = 1)]
        public string sign;
    }

    //出金数据结构定义
    [DataContract]
    public class WithdrawDataStructure
    {
        [DataMember(Order = 0)]
        public string realName {get; set;}

        [DataMember(Order = 1)]
        public string coinId;

        [DataMember(Order = 2)]
        public string amount {get; set;}

        [DataMember(Order = 3)]
        public string orderId {get; set;}

        [DataMember(Order = 4)]
        public string idCard {get; set;}

        [DataMember(Order = 5)]
        public string notifyUrl {get; set;}

        [DataMember(Order = 6)]
        public string returnUrl {get; set;}

        [DataMember(Order = 7)]
        public string sendTime {get; set;}
    }

    [DataContract]
    public class WithdrawCipherTextStructure
    {
        [DataMember(Order = 0)]
        public WithdrawDataStructure data {get; set;}

        [DataMember(Order = 1)]
        public string sign;
    }

    public class myApp
    {
        static void Main(string[] args)
        {
            string Key = "83A6AEDE-3B5A-4D3E-B789-DC780421C1A1";
            string Smac = "C628C57D3F0FCB7652D9C5D64898DFFF";
            FexpayUtils util = new FexpayUtils();
            Key = util.md5(Key).Substring(0, 8).ToUpper();
            deposit(util, Key, Smac);
            checkWithdraw(util, Key, Smac);
            withdraw(util, Key, Smac);
        }

        //入金数据加密示例
        static void deposit(FexpayUtils util, string key, string smac)
        {
            //编辑入金数据
            DepositDataStructure data = new DepositDataStructure();
            data.realName = Uri.EscapeDataString("实名02");
            data.amount = "1000";
            data.orderId = "2018071318275278924";
            data.idCard = "2222222222";
            data.notifyUrl = "https://www.domain.com/notify/callback";
            data.returnUrl = "https://www.domain.com/return/callback";
            data.sendTime = "2018-07-13 18:27:52";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(DepositDataStructure));
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, data);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            string json = sr.ReadToEnd().Replace("\\", "");
            sr.Close();
            msObj.Close();

            Console.WriteLine("=========入金===========");
            string sign = util.md5(json + smac).ToUpper();
            Console.WriteLine("json + smac:" + json + smac);
            Console.WriteLine("sign:" + sign);

            //编辑入金密文
            DepositCipherTextStructure cipherText = new DepositCipherTextStructure();
            cipherText.data = data;
            cipherText.sign = sign;

            DataContractJsonSerializer cipherjs = new DataContractJsonSerializer(typeof(DepositCipherTextStructure));
            MemoryStream cipherObj = new MemoryStream();
            cipherjs.WriteObject(cipherObj, cipherText);
            cipherObj.Position = 0;
            StreamReader ciphersr = new StreamReader(cipherObj, Encoding.UTF8);
            string cipherjson = ciphersr.ReadToEnd().Replace("\\", "");
            ciphersr.Close();
            cipherObj.Close();

            Console.WriteLine("cipherjson:" + cipherjson);

            //加密后入金数据密文
            string encrypted = util.DesEncrypt(key, cipherjson);
            Console.WriteLine("Encrypted:" + encrypted);

            //解密密文
            Console.WriteLine("Descripted:"+ util.DesDecrypt(key, encrypted));
        }

        //入金数据加密示例
        static void checkWithdraw(FexpayUtils util, string key, string smac)
        {
            //编辑入金数据
            CheckWithdrawStructure data = new CheckWithdrawStructure();
            data.realName = Uri.EscapeDataString("实名02");
            data.amount = "0.48";
            data.coinId = "1";          // 1=BTC, 2=ETH, 3=UND
            data.idCard = "2222222222";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(CheckWithdrawStructure));
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, data);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            string json = sr.ReadToEnd().Replace("\\", "");
            sr.Close();
            msObj.Close();

            Console.WriteLine("\r\n=========出金验证===========");
            string sign = util.md5(json + smac).ToUpper();
            Console.WriteLine("json + smac:" + json + smac);
            Console.WriteLine("sign:" + sign);

            //编辑出金验证密文
            CheckWithdrawCipherTextStructure cipherText = new CheckWithdrawCipherTextStructure();
            cipherText.data = data;
            cipherText.sign = sign;

            DataContractJsonSerializer cipherjs = new DataContractJsonSerializer(typeof(CheckWithdrawCipherTextStructure));
            MemoryStream cipherObj = new MemoryStream();
            cipherjs.WriteObject(cipherObj, cipherText);
            cipherObj.Position = 0;
            StreamReader ciphersr = new StreamReader(cipherObj, Encoding.UTF8);
            string cipherjson = ciphersr.ReadToEnd().Replace("\\", "");
            ciphersr.Close();
            cipherObj.Close();

            Console.WriteLine("cipherjson:" + cipherjson);

            //加密后出金验证数据密文
            string encrypted = util.DesEncrypt(key, cipherjson);
            Console.WriteLine("Encrypted:" + encrypted);

            //解密密文
            Console.WriteLine("Descripted:"+ util.DesDecrypt(key, encrypted));
        }

        //出金数据加密示例
        static void withdraw(FexpayUtils util, string key, string smac)
        {
            //编辑出金数据
            WithdrawDataStructure data = new WithdrawDataStructure();
            data.realName = Uri.EscapeDataString("实名02");
            data.amount = "0.48";
            data.coinId = "1"; 
            data.orderId = "2018071318275278925";
            data.idCard = "2222222222";
            data.notifyUrl = "https://www.domain.com/notify/withdraw/callback";
            data.returnUrl = "https://www.domain.com/return/withdraw/callback";
            data.sendTime = "2018-07-13 18:27:52";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(WithdrawDataStructure));
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, data);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            string json = sr.ReadToEnd().Replace("\\", "");
            sr.Close();
            msObj.Close();

            Console.WriteLine("\r\n=========出金===========");
            string sign = util.md5(json + smac).ToUpper();
            Console.WriteLine("json + smac:" + json + smac);
            Console.WriteLine("sign:" + sign);

            //编辑出金密文
            WithdrawCipherTextStructure cipherText = new WithdrawCipherTextStructure();
            cipherText.data = data;
            cipherText.sign = sign;

            DataContractJsonSerializer cipherjs = new DataContractJsonSerializer(typeof(WithdrawCipherTextStructure));
            MemoryStream cipherObj = new MemoryStream();
            cipherjs.WriteObject(cipherObj, cipherText);
            cipherObj.Position = 0;
            StreamReader ciphersr = new StreamReader(cipherObj, Encoding.UTF8);
            string cipherjson = ciphersr.ReadToEnd().Replace("\\", "");
            ciphersr.Close();
            cipherObj.Close();

            Console.WriteLine("cipherjson:" + cipherjson);

            //加密后出金数据密文
            string encrypted = util.DesEncrypt(key, cipherjson);
            Console.WriteLine("Encrypted:" + encrypted);

            //解密密文
            Console.WriteLine("Descripted:"+ util.DesDecrypt(key, encrypted));
        }
    }
}