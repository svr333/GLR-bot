﻿using System;
using System.Threading.Tasks;

namespace GLR.Core
{
    public class Program
    {
        public static Task Main(string[] args)
            => new GLRClient().InitializeAsync();
    }
}
