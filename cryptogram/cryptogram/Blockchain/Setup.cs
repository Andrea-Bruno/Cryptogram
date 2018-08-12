using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainManager
{

  public static class Setup
  {
    public static NetworkConfiguration Network = new NetworkConfiguration();
    public class NetworkConfiguration
    {
      public NetworkConfiguration()
      {
#if DEBUG
        NodeList = new Node[] { new Node() { Server = "http://www.bitboxlab.com", MachineName = "WEBSN3S167", PublicKey = "" } };
        // NodeList = new Node[1] { new Node() { Server = "http://localhost:64046", MachineName = "ANDREA", PublicKey = "" } };
#else
        NodeList = new Node[] { new Node() { Server = "http://www.bitboxlab.com", MachineName = "", PublicKey = "" } };
#endif
      }
      public string MachineName = Environment.MachineName;
      public string MasterServer;
      public string MasterServerMachineName;
      public Node[] NodeList;
      public class Node
      {
        public string Server;
        public string MachineName;
        public string PublicKey;
      }
    }
    public static class Ambient
    {
      public static string Repository = "blockchains";
    }
  }

}
