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
      public string MachineName = Environment.MachineName;
      public string MasterServer;
      public string MasterServerMachineName;
      public Node[] NodeList = new Node[0];
      public class Node
      {
        public string Server;
        public string MachineName;
        public string PublicKey;
      }
    }
    public static class Ambient
    {
      public static string Repository = "/blockchains";


    }
  }

}
