using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Citrus
{
	public class StateObject
	{
		public Socket workSocket = null;
		public const int BUFFER_SIZE = 1024;
		public byte[] buffer = new byte[BUFFER_SIZE];
	}

	public abstract class SocketController : MonoBehaviour
	{
		private static IPAddress ip;

		public static IPAddress IP
		{
			get
			{
				if(ip == null)
				{
					ip = (from entry in Dns.GetHostEntry(Dns.GetHostName()).AddressList
					      where entry.AddressFamily == AddressFamily.InterNetwork
					      select entry).FirstOrDefault();
				}
				return ip;
			}
		}

		[SerializeField]
		protected int portNumber = 42209;
		protected Socket asyncSocket;


		protected void BeginReceive(Socket handler)
		{
			try
			{
				// Create the state object.
				StateObject state = new StateObject();
				state.workSocket = handler;
				Debug.Log("Begin receiving...");
				// Begin receiving the data from the remote device.
				handler.BeginReceive(state.buffer, 0, StateObject.BUFFER_SIZE, 0,
				                     new AsyncCallback(this.ReceiveCallback), state);
			}
			catch(Exception e)
			{
				Debug.LogError("Exception when trying to begin receiving: " + e);
			}
		}

		public void Send(int eventType, byte[] message)
		{
			Debug.Log ("Trying to send, is socket connected? " + this.asyncSocket.Connected);
			this.Send(this.asyncSocket, eventType, message);
		}

		protected void Send(Socket handler, int eventType, byte[] message)
		{
			
			byte[] eventTypeBytes = BitConverter.GetBytes(eventType);

			byte[] msgSizeBytes = BitConverter.GetBytes(message.Length);

			if(BitConverter.IsLittleEndian)
			{
				Array.Reverse(eventTypeBytes);
				Array.Reverse(msgSizeBytes);
			}
	
			byte[] byteData = new byte[eventTypeBytes.Length + msgSizeBytes.Length + message.Length];
			eventTypeBytes.CopyTo(byteData, 0);
			msgSizeBytes.CopyTo(byteData, 4);
			message.CopyTo(byteData, 8);

			// Begin sending the data to the remote device.
			handler.BeginSend(byteData, 0, byteData.Length, 0,
			                  new AsyncCallback(this.SendCallback), handler);
		}

		protected abstract void ReceiveCallback(IAsyncResult ar);

		protected virtual void SendCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = client.EndSend(ar);
				Debug.Log("Bytes sent: " + bytesSent);
			}
			catch(Exception e)
			{
				Debug.LogError("Exception when sending to server: " + e);
			}
		}
	}
}