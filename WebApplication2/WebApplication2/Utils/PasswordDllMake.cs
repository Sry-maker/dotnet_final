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
        [DllImport("C:\\Users\\Derek\\Desktop\\.net期末项目\\WebApplication2\\WebApplication2\\DllPassword.dll")]
        public static extern IntPtr Encryption(string strtest);

        [DllImport("C:\\Users\\Derek\\Desktop\\.net期末项目\\WebApplication2\\WebApplication2\\DllPassword.dll")]
        public static extern IntPtr Decryption(string strtest);
    }
}
