using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ObjectState
    {
        public Socket wSocket = null;
        public const int bufferSize = 1024;
        public byte[] buffer = new byte[bufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
