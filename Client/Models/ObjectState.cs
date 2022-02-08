using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ObjectState
    {
        public const int BufferSize = 256;
        public Socket wSocket = null;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
