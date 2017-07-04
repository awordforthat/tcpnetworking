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
	public sealed class NetworkMessageCall
	{
		public int networkMessageType { get; private set; }

		public Func<byte[], bool> callbackFunction { get; private set; }

		public NetworkMessageCall(int networkMessageType, Func<byte[], bool> callbackFunction)
		{
			this.networkMessageType = networkMessageType;
			this.callbackFunction = callbackFunction;
		}
	}

	public static class NetworkController
	{
		private static Dictionary<int, Func<byte[], bool>> eventCallbacks = new Dictionary<int, Func<byte[], bool>>();

		/**
		 * Adds the NetworkMessageCall to the event callbacks. Note that if the NetworkMessageType index is already assigned, it will be overwritten.
		 */
		public static void AddNetworkCallback(NetworkMessageCall networkMessage)
		{
			eventCallbacks[networkMessage.networkMessageType] = networkMessage.callbackFunction;
		}

		/**
		 * Adds the NetworkMessageCalls to the event callbacks. Note that if the NetworkMessageType indexes are already assigned, they will be overwritten.
		 */
		public static void AddNetworkCallbacks(List<NetworkMessageCall> networkMessages)
		{
			for(int i = 0; i < networkMessages.Count; i++)
			{
				AddNetworkCallback(networkMessages[i]);
			}
		}

		/**
		 * Removes the NetworkMessageCall from the event callbacks.
		 */
		public static void RemoveNetworkCallback(NetworkMessageCall networkMessage)
		{
			eventCallbacks.Remove(networkMessage.networkMessageType);
		}

		/**
		 * Removes the NetworkMessageCalls from the event callbacks.
		 */
		public static void RemoveNetworkCallbacks(List<NetworkMessageCall> networkMessages)
		{
			for(int i = 0; i < networkMessages.Count; i++)
			{
				RemoveNetworkCallback(networkMessages[i]);
			}
		}

		/**
		 * Removes all of the NetworkMessageCalls from the event callbacks.
		 */
		public static void ClearNetworkCallbacks()
		{
			eventCallbacks.Clear();
		}

		/**
		 * Handles incoming data.
		 */
		public static void IncomingReadHandler(SocketController sender, byte[] data)
		{
			// get the event type and message length if possible
			if(data.Length == 4)
			{
				int eventType = NextInt32(ref data);
				Func<byte[], bool> eventCallback;
				if(eventCallbacks.TryGetValue(eventType, out eventCallback))
				{
					eventCallback(data);
				}
				else
				{
					// the event doesn't have a callback!
					RaiseIncomingErrorHandler(sender, new Exception("There was no callback associated with this event."));
				}
			}
			else
			{
				// something went wrong, dispatch an error event
				RaiseIncomingErrorHandler(sender, new Exception("The length of the data must be exactly 4 bytes! The actual number of bytes was: " + data.Length));
			}
		}

		/**
		 * Creates a byte array that contains the message string. This appends a 16-bit int to the beginning of the array indicating the length of the string byte array.
		 */
		public static byte[] CreateStringByteArray(string message)
		{
			byte[] messageBytes = Encoding.UTF8.GetBytes(message);

			short messageLength = (short) messageBytes.Length;
			byte[] msgLengthBytes = BitConverter.GetBytes(messageLength);

			if(BitConverter.IsLittleEndian)
			{
				Array.Reverse(msgLengthBytes);
			}

			byte[] messageByteArray = new byte[messageBytes.Length + msgLengthBytes.Length];
			Array.Copy(msgLengthBytes, messageByteArray, msgLengthBytes.Length);
			Array.Copy(messageBytes, 0, messageByteArray, 2, messageBytes.Length);

			return messageByteArray;
		}

		/**
		 * Combines the message byte array that contains the message string with the provided byte array and returns to the new byte array.
		 */
		public static byte[] WriteStringToByteArray(string message, byte[] data)
		{
			byte[] messageBytes = CreateStringByteArray(message);

			// add the messageBytes to the data byte array
			byte[] newData = new byte[data.Length + messageBytes.Length];
			data.CopyTo(newData, 0);
			messageBytes.CopyTo(newData, data.Length);

			return newData;
		}

		/**
		 * Parses a string out of the byte array data. Note that the first two bytes starting at startingIndex MUST be a 16-bit integer indicating the length of the string in bytes.
		 */
		public static string ParseString(byte[] data, int startingIndex = 0)
		{
			// make sure there's enough bytes left so that we can get the size of the next part of the message
			string result = "";
			int currentIndex = startingIndex;
			if(startingIndex + 1 < data.Length)
			{
				// get the size of the next part of the message
				int nextMsgLength = (data[currentIndex++] << 8) | data[currentIndex++];
				if(currentIndex + nextMsgLength <= data.Length)
				{
					// we can get the next string
					result = Encoding.UTF8.GetString(data, currentIndex, nextMsgLength);
				}
				else if(currentIndex < data.Length - 1)
				{
					// the next message string goes beyond the end of the data, so just get the rest of the data and turn it into a string
					result = Encoding.UTF8.GetString(data, currentIndex, data.Length - currentIndex);
				}
			}
			return result;
		}

		/**
		 * Parses a string out of the byte array data, and removes the string bytes from the byte array. This modifies the byte array!
		 */
		public static string NextString(ref byte[] data)
		{
			string result = "";
			int currentIndex = 0;
			if(currentIndex + 1 < data.Length)
			{
				// get the size of the next part of the message
				int nextMsgLength = (data[currentIndex++] << 8) | data[currentIndex++];
				if(currentIndex + nextMsgLength <= data.Length)
				{
					// we can get the next string
					result = Encoding.UTF8.GetString(data, currentIndex, nextMsgLength);
					currentIndex += nextMsgLength;
				}
				else if(currentIndex < data.Length - 1)
				{
					// the next message string goes beyond the end of the data, so just get the rest of the data and turn it into a string
					result = Encoding.UTF8.GetString(data, currentIndex, data.Length - currentIndex);
					currentIndex = data.Length;
				}
				byte[] newData = new byte[data.Length - currentIndex];
				Array.Copy(data, currentIndex, newData, 0, newData.Length);
				data = newData;
			}
			return result;
		}

		/**
		 * Combines the boolean with the provided byte array and returns to the new byte array.
		 */
		public static byte[] WriteBooleanToByteArray(bool boolean, byte[] data)
		{
			byte[] newBytes = BitConverter.GetBytes(boolean);

			// add the messageBytes to the data byte array
			byte[] newData = new byte[data.Length + newBytes.Length];
			data.CopyTo(newData, 0);
			newBytes.CopyTo(newData, data.Length);

			return newData;
		}

		/**
		 * Parses a boolean out of the byte array data.
		 */
		public static bool ParseBoolean(byte[] data, int startingIndex = 0)
		{
			// make sure there's enough bytes left so that we can get the size of the next part of the message
			bool result = false;
			if(startingIndex < data.Length)
			{
				// get the size of the next part of the message
				result = BitConverter.ToBoolean(data, startingIndex);
			}
			return result;
		}

		/**
		 * Parses a boolean out of the byte array data, and removes the boolean bytes from the byte array. This modifies the byte array!
		 */
		public static bool NextBoolean(ref byte[] data)
		{
			bool result = false;
			int currentIndex = 0;
			if(currentIndex < data.Length)
			{
				// get the size of the next part of the message
				result = BitConverter.ToBoolean(data, currentIndex);
				currentIndex++;

				byte[] newData = new byte[data.Length - currentIndex];
				Array.Copy(data, currentIndex, newData, 0, newData.Length);
				data = newData;
			}
			return result;
		}

		/**
		 * Combines the int with the provided byte array and returns to the new byte array.
		 */
		public static byte[] WriteInt32ToByteArray(int int32, byte[] data)
		{
			byte[] newBytes = BitConverter.GetBytes(int32);

			if(BitConverter.IsLittleEndian)
			{
				Array.Reverse(newBytes);
			}

			// add the messageBytes to the data byte array
			byte[] newData = new byte[data.Length + newBytes.Length];
			data.CopyTo(newData, 0);
			newBytes.CopyTo(newData, data.Length);

			return newData;
		}

		/**
		 * Parses an int out of the byte array data.
		 */
		public static int ParseInt32(byte[] data, int startingIndex = 0)
		{
			// make sure there's enough bytes left so that we can get the size of the next part of the message
			int result = 0;
			int currentIndex = startingIndex;
			if(currentIndex + 4 < data.Length)
			{
				// get the size of the next part of the message
				result = (data[currentIndex++] << 24) | (data[currentIndex++] << 16) | (data[currentIndex++] << 8) | data[currentIndex++];
			}
			return result;
		}

		/**
		 * Parses an int out of the byte array data, and removes the int bytes from the byte array. This modifies the byte array!
		 */
		public static int NextInt32(ref byte[] data)
		{
			int result = 0;
			int currentIndex = 0;
			if(currentIndex + 3 < data.Length)
			{
				// get the size of the next part of the message
				result = (data[currentIndex++] << 24) | (data[currentIndex++] << 16) | (data[currentIndex++] << 8) | data[currentIndex++];

				byte[] newData = new byte[data.Length - currentIndex];
				Array.Copy(data, currentIndex, newData, 0, newData.Length);
				data = newData;
			}
			return result;
		}

		/**
		 * Combines the short with the provided byte array and returns to the new byte array.
		 */
		public static byte[] WriteInt16ToByteArray(short int16, byte[] data)
		{
			byte[] newBytes = BitConverter.GetBytes(int16);

			if(BitConverter.IsLittleEndian)
			{
				Array.Reverse(newBytes);
			}

			// add the messageBytes to the data byte array
			byte[] newData = new byte[data.Length + newBytes.Length];
			data.CopyTo(newData, 0);
			newBytes.CopyTo(newData, data.Length);

			return newData;
		}

		/**
		 * Parses a short out of the byte array data.
		 */
		public static short ParseInt16(byte[] data, int startingIndex = 0)
		{
			// make sure there's enough bytes left so that we can get the size of the next part of the message
			short result = 0;
			int currentIndex = startingIndex;
			if(currentIndex + 2 < data.Length)
			{
				// get the size of the next part of the message
				result = (short) ((data[currentIndex++] << 8) | data[currentIndex++]);
			}
			return result;
		}

		/**
		 * Parses a short out of the byte array data, and removes the short bytes from the byte array. This modifies the byte array!
		 */
		public static short NextInt16(ref byte[] data)
		{
			short result = 0;
			int currentIndex = 0;
			if(currentIndex + 2 < data.Length)
			{
				// get the size of the next part of the message
				result = (short) ((data[currentIndex++] << 8) | data[currentIndex++]);

				byte[] newData = new byte[data.Length - currentIndex];
				Array.Copy(data, currentIndex, newData, 0, newData.Length);
				data = newData;
			}
			return result;
		}

		public static event Action<SocketController, Exception> OnIncomingErrorHandler;

		public static void RaiseIncomingErrorHandler(SocketController sender, Exception exception)
		{
			Debug.LogError("Error received: " + exception);
			if(OnIncomingErrorHandler != null)
			{
				OnIncomingErrorHandler(sender, exception);
			}
		}
	}
}