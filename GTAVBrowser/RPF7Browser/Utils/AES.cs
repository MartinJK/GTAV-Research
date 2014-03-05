/*
 
    Copyright (C) 2013, GTA-Network Team <contact at gta-network dot net>

    This software is provided 'as-is', without any express or implied
    warranty.  In no event will the authors be held liable for any damages
    arising from the use of this software.

    Permission is granted to anyone to use this software for any purpose,
    including commercial applications, and to alter it and redistribute it
    freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.
    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.
    3. This notice may not be removed or altered from any source distribution.
 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace GTANETWORKV.Utils
{
    public static class AES
    {
        private static RijndaelManaged aes = null;
        private static ICryptoTransform decryptor;
        private static ICryptoTransform decryptor16;
        private static ICryptoTransform encryptor;
        private static ICryptoTransform encryptor16;

        public static byte[] Key = null;

        // Here is an ugly thing. Rockstar using uncommon padding mode - which is to not encrypt the last block 
        // So I need to change the TransformBlock, to not encrypt the last block, and I do it with a proxy
        // Is there a better way?
        private class CryptoTransformProxy : RealProxy
        {
            private readonly ICryptoTransform _instance;
            private readonly bool SixteenRounds;

            private CryptoTransformProxy(ICryptoTransform instance, bool sixteenRounds = false)
                : base(typeof(ICryptoTransform))
            {
                _instance = instance;
                this.SixteenRounds = sixteenRounds;
            }

            public static ICryptoTransform Create(ICryptoTransform instance, bool sixteenRounds = false)
            {
                return (ICryptoTransform)new CryptoTransformProxy(instance, sixteenRounds).GetTransparentProxy();
            }

            private static int TransformBlock16Rounds(ICryptoTransform instance, byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                int res = 0;
                for (int i = 0; i < 16; ++i)
                {
                    res = instance.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
                }
                return inputCount;
            }


            private static byte[] TransformFinalBlock(ICryptoTransform instance, byte[] inputBuffer, int inputOffset, int inputCount)
            {
                if (inputCount >= instance.InputBlockSize)
                {
                    throw new Exception("Unexpected value");
                }
                byte[] result = new byte[inputCount];
                Buffer.BlockCopy(inputBuffer, 0, result, 0, inputCount);
                return result;
            }

            public override IMessage Invoke(IMessage msg)
            {
                var methodCall = (IMethodCallMessage)msg;
                var method = (MethodInfo)methodCall.MethodBase;

                try
                {
                    if (SixteenRounds && method.Name == "TransformBlock")
                    {
                        MethodInfo newMethod = typeof(CryptoTransformProxy).GetMethod("TransformBlock16Rounds", BindingFlags.Static | BindingFlags.NonPublic);
                        object[] newParams = new object[methodCall.InArgs.Length + 1];
                        methodCall.InArgs.CopyTo(newParams, 1);
                        newParams[0] = _instance;
                        var result = newMethod.Invoke(null, newParams);
                        return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                    }
                    else if (method.Name == "TransformFinalBlock")
                    {
                        MethodInfo newMethod = typeof(CryptoTransformProxy).GetMethod("TransformFinalBlock", BindingFlags.Static | BindingFlags.NonPublic);
                        object[] newParams = new object[methodCall.InArgs.Length + 1];
                        methodCall.InArgs.CopyTo(newParams, 1);
                        newParams[0] = _instance;
                        var result = newMethod.Invoke(null, newParams);
                        return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                    }
                    else
                    {
                        var result = method.Invoke(_instance, methodCall.InArgs);
                        return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                    }
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException && e.InnerException != null)
                    {
                        return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
                    }

                    return new ReturnMessage(e, msg as IMethodCallMessage);
                }
            }
        }

        private static void InitalizeAES()
        {
            aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.Key = AES.Key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            decryptor = CryptoTransformProxy.Create(aes.CreateDecryptor());
            decryptor16 = CryptoTransformProxy.Create(aes.CreateDecryptor(), true);
            encryptor = CryptoTransformProxy.Create(aes.CreateEncryptor());
            encryptor16 = CryptoTransformProxy.Create(aes.CreateEncryptor(), true);
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (aes == null)
            {
                InitalizeAES();
            }
            decryptor.TransformBlock(data, 0, (data.Length / 0x10) * 0x10, data, 0);
            return data;
        }

        public static CryptoStream DecryptStream(Stream input, bool sixteenRounds = false)
        {
            if (aes == null)
            {
                InitalizeAES();
            }
            if (sixteenRounds)
            {
                return new CryptoStream(input, decryptor16, CryptoStreamMode.Read);
            }
            else
            {
                return new CryptoStream(input, decryptor, CryptoStreamMode.Read);
            }
        }

        public static CryptoStream EncryptStream(Stream input, bool sixteenRounds = false)
        {
            if (aes == null)
            {
                InitalizeAES();
            }
            if (sixteenRounds)
            {
                return new CryptoStream(input, encryptor16, CryptoStreamMode.Write);
            }
            else
            {
                return new CryptoStream(input, encryptor, CryptoStreamMode.Write);
            }
        }

    }
}
