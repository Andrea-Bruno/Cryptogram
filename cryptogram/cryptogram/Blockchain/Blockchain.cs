using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static BlockchainManager.Network;
using static BlockchainManager.Utility;

namespace BlockchainManager
{
  public class Blockchain
  {
    public Blockchain()
    {
    }
    public Blockchain(string PublicKey, string Group, string Name, BlockchainType Type, bool AcceptBodySignature)
    {
      this.PublicKey = PublicKey;
      this.Group = Group;
      this.Name = Name;
      this.Type = Type;
      this.AcceptBodySignature = AcceptBodySignature;
    }
    public Blockchain(string Group, string Name, BlockchainType Type, bool AcceptBodySignature)
    {
      this.Group = Group;
      this.Name = Name;
      this.Type = Type;
      this.AcceptBodySignature = AcceptBodySignature;
    }
    public void Save()
    {
      if ((!System.IO.Directory.Exists(Directory())))
        System.IO.Directory.CreateDirectory(Directory());
      using (System.IO.FileStream Stream = new System.IO.FileStream(PathNameFile() + ".info", System.IO.FileMode.Create))
      {
        System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(this.GetType());
        System.Xml.Serialization.XmlSerializerNamespaces xmlns = new System.Xml.Serialization.XmlSerializerNamespaces();
        xmlns.Add(string.Empty, string.Empty);
        xml.Serialize(Stream, this, xmlns);
      }
    }
    public static Blockchain Load(string Group, string Name)
    {
      try
      {
        string File = PathNameFile(Group, Name) + ".info";
        Blockchain Value;
        if (System.IO.File.Exists(File))
        {
          using (System.IO.FileStream Stream = new System.IO.FileStream(File, System.IO.FileMode.Open, System.IO.FileAccess.Read))
          {
            System.Xml.Serialization.XmlSerializer XML = new System.Xml.Serialization.XmlSerializer(typeof(Blockchain));
            Value = (Blockchain)XML.Deserialize(Stream);
          }
          return Value;
        }
      }
      catch (Exception ex)
      {
      }
      return null;
    }
    public bool AcceptBodySignature;
    public string PublicKey; // If is not nothing then all block Checksum are signed with private key 
    public string Group;
    public string Name;
    public BlockchainType Type;
    public enum BlockchainType
    {
      LineOfText,
      Xml,
      Binary
    }
    public int MaxBlockLenght = 2048;
    public string BlockSeparator = "\r\n";
    public string FieldsSeparator = "\t";
    public long Length()
    {
      if (System.IO.File.Exists(this.PathNameFile()))
        return new System.IO.FileInfo(this.PathNameFile()).Length;
      else
        return 0;
    }
    public void Truncate(long Position)
    {
      using (System.IO.FileStream Stream = new System.IO.FileStream(this.PathNameFile(), System.IO.FileMode.Truncate))
      {
        Stream.SetLength(Position);
      }
    }
    public class VectorBlocks
    {
      public Blockchain Blockchain;
      public string Group;
      public string Name;
      public long Position = -1;
      public long RequestSendBlocksFromPosition = -1;
      public Block[] Blocks = new Block[0];
    }
    public void SyncNode()
    {
      foreach (var Node in Setup.Network.NodeList)
      {
        if (Node.MachineName != Setup.Network.MasterServerMachineName)
        {
          long CurrentLength = this.Length();
          VectorBlocks Vector = new VectorBlocks() { Blockchain = this, RequestSendBlocksFromPosition = CurrentLength };
          VectorToNode(Vector, Node.Server, Node.MachineName);
        }
      }
    }
    private void VectorToNode(VectorBlocks Vector, string Server, string MachineName)
    {
      string ReturnObjectName = null;
      string ReturnXmlObject = null;
      string ReturnFromUser = null;
      object Obj = null;
      var XmlObjectVector = SendObjectSync((object)Vector, Server, null, MachineName);
      if (!string.IsNullOrEmpty(XmlObjectVector))
      {
        object ReturmObj;
        Converter.XmlToObject(XmlObjectVector, typeof(ObjectVector), out ReturmObj);
        ObjectVector ObjVector = (ObjectVector)ReturmObj;
        ReturnObjectName = ObjVector.ObjectName;
        ReturnXmlObject = ObjVector.XmlObject;
        ReturnFromUser = ObjVector.FromUser;
        if (ReturnObjectName == "VectorBlocks")
        {
          Converter.XmlToObject(ReturnXmlObject, typeof(VectorBlocks), out Obj);
          VectorBlocks ReturnVector = (VectorBlocks)Obj;
          SyncBlocksFromNetwork(ReturnVector);
        }
        else
        {
          ReturnObjectName = "String";
          Converter.XmlToObject(ReturnXmlObject, typeof(string), out Obj);
          string ErrorMessage = System.Convert.ToString(Obj);
          Log("BlockchainError", 1000, ErrorMessage);
        }
      }
    }
    public void SyncBlockToNetwork(Block Block, long Position)
    {
      SyncBlocksToNetwork(new List<Block>() { Block }, Position);
    }
    public void SyncBlocksToNetwork(List<Block> Blocks, long Position)
    {
      VectorBlocks Vector = new VectorBlocks() { Blockchain = this, Blocks = Blocks.ToArray(), Position = Position };
      foreach (var Node in Setup.Network.NodeList)
      {
        if (Node.MachineName != Setup.Network.MachineName)
          VectorToNode(Vector, Node.Server, Node.MachineName);
      }
    }
    public static bool SyncBlocksFromNetwork(VectorBlocks Vector, VectorBlocks SetReturnVector = null)
    {
      Blockchain Blockchain = Vector.Blockchain;
      if (Blockchain == null)
        Blockchain = Blockchain.Load(Vector.Group, Vector.Name);
      long CurrentLength = Blockchain.Length();
      if (CurrentLength == 0)
        Blockchain.Save();

      if (Vector.RequestSendBlocksFromPosition != -1 && SetReturnVector != null)
      {
        if (CurrentLength > Vector.RequestSendBlocksFromPosition)
        {
          SetReturnVector.Blockchain = Blockchain;
          SetReturnVector.Blocks = Blockchain.GetBlocks(Vector.RequestSendBlocksFromPosition).ToArray();
          SetReturnVector.Position = Vector.RequestSendBlocksFromPosition;
        }
      }
      else
      {
        if (CurrentLength > Vector.Position)
        {
          if (SetReturnVector != null)
          {
            SetReturnVector.Blockchain = Blockchain;
            SetReturnVector.Blocks = Blockchain.GetBlocks(Vector.Position).ToArray();
            SetReturnVector.Position = Vector.Position;
          }
          else
          {
            Blockchain.Truncate(Vector.Position);
            CurrentLength = Vector.Position;
          }
        }

        if (CurrentLength == Vector.Position)
        {
          foreach (Block Block in Vector.Blocks)
          {
            if (!Block.AddToBlockchain(Blockchain))
              // Error in blockchain
              return false;
          }
        }
        else if (CurrentLength < Vector.Position)
        {
          // Send a request of th missed blocks 
          if (SetReturnVector != null)
          {
            SetReturnVector.Blockchain = Blockchain;
            SetReturnVector.RequestSendBlocksFromPosition = CurrentLength;
          }
        }
      }
      return true;
    }
    public class Block
    {
      private Block()
      {
      }
      public Block(Block PreviousBlock, Blockchain Blockchain, string Record)
      {
        this.PreviousBlock = PreviousBlock;
        this.Blockchain = Blockchain;
        this.Record = Record;
      }
      public Block(Blockchain Blockchain, byte[] Data)
      {
        _Block(Blockchain, Convert.ToBase64String(Data));
      }
      public Block(Blockchain Blockchain, string Data)
      {
        switch (Blockchain.Type)
        {
          case BlockchainType.Xml:
            Data.Replace("\n", "").Replace("\r", "");
            break;
          case BlockchainType.Binary:
            throw new System.InvalidOperationException("Invalid method with the blockchain in binary mode");
        }
        _Block(Blockchain, Data);
      }
      private void _Block(Blockchain Blockchain, string Data)
      {
        this.Blockchain = Blockchain;
        this._Data = Data;
        _Timestamp = DateTime.Now.ToUniversalTime();
        PreviousBlock = Blockchain.GetLastBlock();
        _Checksum = CalculateChecksum();
        if (!Blockchain.AcceptBodySignature)
        {
          if (string.IsNullOrEmpty(Blockchain.PublicKey))
            AddToBlockchain();
        }
      }
      private Block PreviousBlock;
      public bool AddBlockSignature(byte[] SignedChecksum)
      {
        _Checksum = Convert.ToBase64String(SignedChecksum);
        bool Result = CheckBlockSignature();
        if (Result)
          AddToBlockchain();
        return Result;
      }
      public bool CheckBlockSignature()
      {
        try
        {
          System.Security.Cryptography.RSACryptoServiceProvider RSAalg = new System.Security.Cryptography.RSACryptoServiceProvider();
          RSAalg.ImportCspBlob(Convert.FromBase64String(Blockchain.PublicKey));
          return RSAalg.VerifyHash(CalculateChecksumBytes(), System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"), ChecksumBytes);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
          return false;
        }
      }
      private byte[] BaseChecksum()
      {
        string PreviousChecksum = null;
        if (PreviousBlock != null)
          PreviousChecksum = PreviousBlock.Checksum;
        string BaseComputation = BodyRecord(true);
        return Encoding.Unicode.GetBytes(PreviousChecksum + BaseComputation);
      }
      public byte[] CalculateChecksumBytes()
      {
        System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
        byte[] hashBytes = hashType.ComputeHash(BaseChecksum());
        return hashBytes;
      }
      private string CalculateChecksum()
      {
        return Convert.ToBase64String(CalculateChecksumBytes());
      }
      public bool IsValid()
      {
        if (CheckBodySignatures())
        {
          if (!string.IsNullOrEmpty(Blockchain.PublicKey))
            return CheckBlockSignature();
          else
            return _Checksum == CalculateChecksum();
        }
        return false;
      }
      private Blockchain Blockchain;
      public bool AddToBlockchain(Blockchain Blockchain = null)
      {
        if (this.Blockchain == null)
          this.Blockchain = Blockchain;
        try
        {
          if ((!System.IO.Directory.Exists(this.Blockchain.Directory())))
            System.IO.Directory.CreateDirectory(this.Blockchain.Directory());
          using (System.IO.StreamWriter sw = System.IO.File.AppendText(this.Blockchain.PathNameFile()))
          {
            sw.WriteLine(Record, this.Blockchain.BlockSeparator);
          }
          return true;
        }
        catch (Exception ex)
        {
          return false;
        }
      }
      private string _Data;
      public string Data
      {
        get
        {
          if (Blockchain.Type == BlockchainType.Binary)
            throw new System.InvalidOperationException("Invalid method with the blockchain in binary mode");
          return _Data;
        }
      }
      public byte[] DataByteArray
      {
        get
        {
          if (Blockchain.Type != BlockchainType.Binary)
            throw new System.InvalidOperationException("Invalid method with the blockchain is not in binary mode");
          return Convert.FromBase64String(_Data);
        }
      }
      private DateTime _Timestamp;
      public DateTime Timestamp
      {
        get
        {
          return _Timestamp;
        }
      }
      private string _Checksum;
      public string Checksum
      {
        get
        {
          return _Checksum;
        }
      }
      public byte[] ChecksumBytes
      {
        get
        {
          return Convert.FromBase64String(_Checksum);
        }
      }
      private string _BodySignatures;
      /// <summary>
      /// Returns a dictionary indexed with public keys, and the values of the block signatures
      /// </summary>
      /// <returns></returns>
      public Dictionary<string, string> GetAllBodySignature()
      {
        Dictionary<string, string> Result = null;
        if (!string.IsNullOrEmpty(_BodySignatures))
        {
          Result = new Dictionary<string, string>();
          string[] Parts = _BodySignatures.Split(' ');
          string PublicKey = null;
          string Signature;
          bool Flag = false;
          foreach (string Part in Parts)
          {
            if (Flag)
            {
              Signature = Part;
              Result.Add(PublicKey, Signature);
            }
            else
              PublicKey = Part;
            Flag = !Flag;
          }
        }
        return Result;
      }
      public bool AddBodySignature(string PublicKey, byte[] Signature, bool AddNowToBlockchain)
      {
        if (Blockchain.AcceptBodySignature)
        {
          if (CheckBodySignature(PublicKey, Signature))
          {
            if (!string.IsNullOrEmpty(_BodySignatures))
              _BodySignatures += " ";
            _BodySignatures += PublicKey + " " + Convert.ToBase64String(Signature);
            _Checksum = CalculateChecksum();
            if (AddNowToBlockchain)
              return this.AddToBlockchain();
            return true;
          }
        }
        else
          throw new System.Exception("This blockchain does not allow to add signatures to the body");
        return false;
      }
      public bool CheckBodySignatures()
      {
        Dictionary<string, string> Signatures = GetAllBodySignature();
        if (Signatures != null)
        {
          foreach (string PubKey in Signatures.Keys)
          {
            if (!CheckBodySignature(PubKey, Convert.FromBase64String(Signatures[PubKey])))
              return false;
          }
        }
        return true;
      }
      private bool CheckBodySignature(string PublicKey, byte[] Signature)
      {
        try
        {
          System.Security.Cryptography.RSACryptoServiceProvider RSAalg = new System.Security.Cryptography.RSACryptoServiceProvider();
          RSAalg.ImportCspBlob(Convert.FromBase64String(PublicKey));
          return RSAalg.VerifyHash(HashBody(), System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"), Signature);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
          return false;
        }
      }
      public byte[] HashBody()
      {
        System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
        byte[] hashBytes = hashType.ComputeHash(Encoding.Unicode.GetBytes(BodyRecord(false)));
        return hashBytes;
      }
      private string BodyRecord(bool WithSigatures)
      {
        var HexTimestamp = _Timestamp.Ticks.ToString("X");
        string Signatures = null;
        if (WithSigatures)
        {
          if (!string.IsNullOrEmpty(_BodySignatures))
            Signatures = Blockchain.FieldsSeparator + _BodySignatures;
        }
        return _Data + Blockchain.FieldsSeparator + HexTimestamp + Signatures;
      }
      protected internal string Record
      {
        get
        {
          return BodyRecord(true) + Blockchain.FieldsSeparator + _Checksum;
        }
        set
        {
          if (!string.IsNullOrEmpty(value))
          {
            // ===========PARTS==========================
            // Data + Timestamp + (Signatures) + Checksum
            // ==========================================
            string[] Parts = value.Split(new string[] { Blockchain.FieldsSeparator }, StringSplitOptions.None);
            //if (Blockchain.Type != BlockchainType.LineOfText)
            //  _Data = Converter.Base64ToString(Parts[0]);
            //else
            _Data = Parts[0];
            string DateHex = Parts[1];
            _Timestamp = new DateTime(Convert.ToInt64(DateHex, 16));
            if (Parts.Count() == 4)
              _BodySignatures = Parts[2];
            _Checksum = Parts.Last();
          }
        }
      }
    }
    private static string MapPath(string PathNameFile)
    {
      //return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), PathNameFile);
      string Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      return System.IO.Path.Combine(Path, PathNameFile);

    }
    private static string Directory(string Group)
    {
      return MapPath(System.IO.Path.Combine(Setup.Ambient.Repository, Group));
    }
    private string Directory()
    {
      return Directory(Group);
    }
    private static string PathNameFile(string Group, string Name)
    {
      return System.IO.Path.Combine(Directory(Group), AbjustNameFile(Name) + ".bloks");
    }
    private string PathNameFile()
    {
      return System.IO.Path.Combine(Directory(), AbjustNameFile(Name) + ".bloks");
    }
    private static string AbjustNameFile(string FileName)
    {
      string Result = "";
      foreach (char c in FileName)
      {
        if (char.IsLetterOrDigit(c) || "+-=._".Contains(c))
          Result += c;
        else
        {
          Result += "(" + String.Format("{0:X}", Convert.ToInt32(c)) + ")";
        }
      }
      return Result;
    }
    public Block GetLastBlock()
    {
      return GetPreviousBlock(-1);
    }
    public Block GetPreviousBlock(long Position)
    {
      // Position -1 return last block
      Block Output = null;
      string File = PathNameFile();
      if (System.IO.File.Exists(File))
      {
        string Data = null;
        System.IO.StreamReader Stream = null;
        int NTryError = 0;
        try
        {
          Stream = new System.IO.StreamReader(File);
          if (Position == -1)
            Position = Stream.BaseStream.Length;
          long StartRead = Position - MaxBlockLenght;
          if (StartRead < 0)
            StartRead = 0;
          Stream.BaseStream.Position = StartRead;
          Data = Stream.ReadToEnd();
        }
        catch (Exception ex)
        {
          NTryError += 1;
          System.Threading.Thread.Sleep(500);
        }
        finally
        {
          if (Stream != null)
          {
            Stream.Close();
            Stream.Dispose();
          }
        }
        if (!string.IsNullOrEmpty(Data))
        {
          string[] Blocks = Data.Split(new string[] { BlockSeparator }, StringSplitOptions.None);
          string Block = Blocks[Blocks.Count() - 2];
          Output = new Block(null, this, Block);
        }
      }
      return Output;
    }

    public int Validate()
    {
      // Return 0 = No error, else return the block number with error
      Block LastBlock = null;
      int InvalidBlock = 0;
      if (System.IO.File.Exists(PathNameFile()))
      {
        using (System.IO.StreamReader Stream = System.IO.File.OpenText(PathNameFile()))
        {
          int N = 0;
          while (!Stream.EndOfStream)
          {
            N += 1;
            string Record = Stream.ReadLine();
            Block Block = new Block(LastBlock, this, Record);
            if (!Block.IsValid())
            {
              InvalidBlock = N;
              break;
            }
            LastBlock = Block;
          }
        }
      }
      return InvalidBlock;
    }
    public List<Block> GetBlocks(long FromPosition)
    {
      List<Block> List = new List<Block>();
      Block LastBlock = GetPreviousBlock(FromPosition);
      if (System.IO.File.Exists(PathNameFile()))
      {
        using (System.IO.StreamReader Stream = System.IO.File.OpenText(PathNameFile()))
        {
          Stream.BaseStream.Position = FromPosition;
          while (!Stream.EndOfStream)
          {
            string Record = Stream.ReadLine();
            Block Block = new Block(LastBlock, this, Record);
            if (!Block.IsValid())
              // Blockchain error!
              break;
            List.Add(Block);
            LastBlock = Block;
          }
        }
      }
      return List;
    }
  }
}

