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
        //NodeList = new Network.Node[1] { new Network.Node() { Server = "http://www.bitboxlab.com", MachineName = "ANDREA", PublicKey = "" } };
        NodeList = new Network.Node[1] { new Network.Node() { Server = "http://localhost:8080", MachineName = "ANDREA", PublicKey = "" } };
#else
        NodeList = new Network.Node[1] { new Network.Node() { Server = "http://www.bitboxlab.com", MachineName = "ANDREA", PublicKey = "" } };
#endif
      }
      public string MachineName = Environment.MachineName;
      public string MasterServer;
      public string MasterServerMachineName;
      public Network.Node[] NodeList;
    }
    public static class Ambient
    {
      public static string Repository = "blockchains";
    }
  }

}
