using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Citrus
{
	public class AsyncServer : SocketController
	{
		private enum ConnectionType
		{
			TCP = 0,
			UDP,
			BOTH
		}

		[SerializeField]
		private int udpPortNumber = 9050, maxHostConnectionBacklog = 10;
		[SerializeField]
		private ConnectionType connectionType = ConnectionType.TCP;
		private Socket udpServer;
		private List<Socket> clients;

		private void Start()
		{
			this.clients = new List<Socket>();
			this.StartServer();
		}

		private void OnApplicationQuit()
		{
			Debug.Log("Quitting... number of clients: " + this.clients.Count);
			for(int i = 0; i < this.clients.Count; i++)
			{
				Debug.Log("Closing client... " + i);
				this.clients[i].Close();
			}

			if(this.asyncSocket != null)
			{
				Debug.Log("Close self...");
				this.asyncSocket.Close();
			}

			if(this.udpServer != null)
			{
				Debug.Log("Close UDP server...");
				this.udpServer.Close();
			}
		}

		private void StartServer()
		{
			if(this.connectionType == ConnectionType.TCP)
			{
				this.StartTcpServer();
			}
			else if(this.connectionType == ConnectionType.UDP)
			{
				this.StartUdpServer();
			}
			else
			{
				this.StartTcpServer();
				this.StartUdpServer();
			}
		}

		private void StartTcpServer()
		{
			this.asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Debug.Log("Starting TCP server on port: " + this.portNumber + ", with IP: " + IP);
			try
			{
				this.asyncSocket.Bind(new IPEndPoint(IP, this.portNumber));
				this.asyncSocket.Listen(maxHostConnectionBacklog);
				this.asyncSocket.BeginAccept(new System.AsyncCallback(this.AcceptCallback), this.asyncSocket);

			}
			catch(System.Exception e)
			{
				Debug.LogError("Exception when attempting to host (" + this.portNumber + ") " + e);
				this.asyncSocket = null;
			}
		}

		private void StartUdpServer()
		{
			// create a UDP server
			this.udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			try
			{
				IPEndPoint ipep = new IPEndPoint(IPAddress.Any, this.udpPortNumber);
				this.udpServer.Bind(ipep);
				Debug.Log("Starting UDP server on port: " + this.udpPortNumber + ", with IP: " + IPAddress.Any);

				SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
				eventArgs.Completed += this.OnUdpReceive;
				eventArgs.SetBuffer(new byte[StateObject.BUFFER_SIZE], 0, StateObject.BUFFER_SIZE);
				eventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
				eventArgs.AcceptSocket = this.udpServer;
				if(!this.udpServer.ReceiveFromAsync(eventArgs))
				{
					Debug.Log("Not waiting for clients...");
					this.OnUdpReceive(this.udpServer, eventArgs);
				}
			}
			catch(Exception e)
			{
				Debug.LogError("Exception when attempting to host UDP server on (" + this.udpPortNumber + ") " + e);
				this.udpServer = null;
			}
		}

		private void OnUdpReceive(object sender, SocketAsyncEventArgs args)
		{
			// received something from the UDP client, display it
			int bytesRead = args.BytesTransferred;
			#if UNITY_EDITOR
			Debug.Log("UDP received data... " + bytesRead);
			#endif
			if(bytesRead > 0)
			{
				// get the data
				byte[] read = new byte[bytesRead];
				Array.Copy(args.Buffer, 0, read, 0, bytesRead);

				// raise the event
				NetworkController.IncomingReadHandler(this, read);
			}
			// start reading again
			if(!this.udpServer.ReceiveFromAsync(args))
			{
				this.OnUdpReceive(this.udpServer, args);
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			// Get the socket that handles the client request.
			Socket listener = (Socket) ar.AsyncState;
			Socket handler = listener.EndAccept(ar);
			this.clients.Add(handler);
			Debug.Log("Client connected: " + this.clients.Count);
			Debug.Log ("Listener connected: " + listener.Connected);
			Debug.Log ("Handler connected: " + handler.Connected);
			// Create the state object.
			this.BeginReceive(handler);

			try
			{
				listener.BeginAccept(new System.AsyncCallback(this.AcceptCallback), listener);
			}
			catch(System.Exception e)
			{
				Debug.LogError("Exception when starting new accept process: " + e);
			}
		}

		protected override void ReceiveCallback(IAsyncResult ar)
		{
			Debug.Log("Receive callback...");
			try
			{
				// Retrieve the state object and the handler socket
				// from the asynchronous state object.
				StateObject state = (StateObject) ar.AsyncState;
				Socket handler = state.workSocket;

				// Read data from the client socket.
				int bytesRead = handler.EndReceive(ar);
				Debug.Log("How many bytes are we getting? " + bytesRead);
				if(bytesRead > 0)
				{
					// get the data
					byte[] read = new byte[bytesRead];
					Array.Copy(state.buffer, 0, read, 0, bytesRead);

					// raise the event
					NetworkController.IncomingReadHandler(this, read);

					// start reading again
					handler.BeginReceive(state.buffer, 0, StateObject.BUFFER_SIZE, 0,
					                     new AsyncCallback(this.ReceiveCallback), state);
				}
				else
				{
					Debug.Log("Client disconnected.");
					// this socket disconnected
					this.clients.Remove(handler);
				}
			}
			catch(Exception e)
			{
				NetworkController.RaiseIncomingErrorHandler(this, e);
			}
		}

		public int GetNumClients()
		{
			return this.clients.Count;
		}

		public void SendToClient(int eventType, byte[] message, int clientID)
		{
			try
			{
				this.Send(this.clients[clientID - 1], eventType, message);
			}
			catch(System.Exception e) {
				Debug.Log (e.Message);
			}
		}

		public void SendToAllClients(int eventType, byte[] message)
		{
			for(int i = 0; i < this.clients.Count; i++)
			{
				this.Send(this.clients[i], eventType, message);
			}
		}
	}
}