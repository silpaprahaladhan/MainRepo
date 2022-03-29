using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nirast.Pcms.Api.Sdk.Infrastructure
{
   public interface ISecurity
    {
         string EncryptPlainTextToCipherText(string PlainText = "");
        string DecryptCipherTextToPlainText(string CipherText = "");
        string Encrypt(string plainText, string passPhrase);
        string Decrypt(string cipherText, string passPhrase);
        byte[] Generate256BitsOfRandomEntropy();
    }
}
