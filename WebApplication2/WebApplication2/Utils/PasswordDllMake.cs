using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace WebApplication2.Utils
{
    public class PasswordDllMake
    {
        [DllImport("Dllpassword.dll")]
        public static extern IntPtr Encryption(string strtest);

        [DllImport("Dllpassword.dll")]
        public static extern IntPtr Decryption(string strtest);
    }
}
