using System;
using System.Threading.Tasks;

namespace GLR.Core
{
    class Program
    {
        static Task Main(string[] args)
            => new GLRClient().InitializeAsync();
    }
}
