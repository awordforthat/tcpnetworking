using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Citrus
{
	public class AsyncClient : SocketController
	{
		private Boolean connected;

		private void Start()
		{
			connected = false;
			this.Connect();
		}

		private void OnApplicationQuit()
		{
			Debug.Log("Close...");
			this.asyncSocket.Close();
		}

		private void Connect()
		{
			Debug.Log("Connecting to server...");
			this.asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				this.asyncSocket.BeginConnect(new IPEndPoint(IP, this.portNumber), this.ConnectCallback, this.asyncSocket);
			}
			catch(System.Exception e)
			{
				Debug.LogError("Exception when attempting to connect (" + this.portNumber + ") " + e);
				this.asyncSocket = null;
			}
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;

				// Complete the connection.
				client.EndConnect(ar);
				connected = true;
				Debug.Log("Connected to server, start receiving...");
				// start receiving from the server
				this.BeginReceive(client);
			}
			catch(Exception e)
			{
				connected = false;
				Debug.LogError("Exception when connecting to server: " + e);
				Debug.Log ("Attempting to reconnect...");
				this.Connect ();
			}
		}

		protected override void ReceiveCallback(IAsyncResult ar)
		{
			EventManager.TriggerEvent ("ClientReceivedData");
			try
			{
				// Retrieve the state object and the client socket 
				// from the asynchronous state object.
				StateObject state = (StateObject) ar.AsyncState;
				Socket client = state.workSocket;

				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);

				if(bytesRead > 0)
				{
					// get the data
					byte[] read = new byte[bytesRead];
					Array.Copy(state.buffer, 0, read, 0, bytesRead);

					// raise the event
					//NetworkController.IncomingReadHandler(this, read);

					// start reading again
					client.BeginReceive(state.buffer, 0, StateObject.BUFFER_SIZE, 0,
					                    new AsyncCallback(this.ReceiveCallback), state);
				}
				else
				{
					// this socket disconnected, do something? maybe attempt to reconnect?
					Debug.Log("Client disconnected?");
					connected = false;
					Debug.Log ("Attempting to reconnect...");
					this.Connect ();
				}
			}
			catch(Exception e)
			{
				NetworkController.RaiseIncomingErrorHandler(this, e);
			}
		}


		public Boolean IsConnected()
		{
			return connected;
		}
	}
}