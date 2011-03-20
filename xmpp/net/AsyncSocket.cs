// AsyncSocket.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2011 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using ubiety.logging;
using ubiety;
using ubiety.states;
using ubiety.registries;
using ubiety.common;

namespace ubiety.net
{
	/// <remarks>
	/// AsyncSocket is the class that communicates with the server.
	/// </remarks>
	internal class AsyncSocket
	{
		private Socket _socket;
		private UTF8Encoding _utf = new UTF8Encoding();
		private Address _dest;
		private byte[] _buff = new byte[4096];
		private Stream _stream;
		private string _hostname;
		private bool _secure;
		private NetworkStream _netstream;
		private ProtocolState _states = ProtocolState.Instance;
		private bool _connected;

		// Used to determine if we are encrypting the socket to turn off returning the message to the parser
		private bool _encrypting = false;
		private SslStream _sslstream;
		private ManualResetEvent _resetEvent = new ManualResetEvent(false);
		
		// Timeout after 15 seconds by default
		private int _timeout = 15000;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncSocket"/> class.
		/// </summary>
		public AsyncSocket()
		{
		}

		/// <summary>
		/// Establishes a connection to the specified remote host.
		/// </summary>
		/// <returns>True if we connected, false if we didn't</returns>
		public void Connect()
		{
			if (!String.IsNullOrEmpty(Settings.Hostname))
			{
				_hostname = Settings.Hostname;
			}
			else
			{
				_hostname = Settings.ID.Server;
			}

			_dest = Address.Resolve(_hostname, Settings.Port);
			if (_dest == null)
				return;
			Logger.InfoFormat(this, "Connecting to: {0} on port {1}", _dest.IP.ToString(), _dest.Port.ToString());
			if (!_dest.IPV6)
			{
				Logger.Debug(this, "Connecting using IPv4");
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			}
			else
			{
				Logger.Debug(this, "Connecting using IPv6");
				_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
			}

			try
			{
				//_socket.Connect(_dest.EndPoint);
				_socket.BeginConnect(_dest.EndPoint, new AsyncCallback(FinishConnect), null);
				if(_resetEvent.WaitOne(_timeout, false))
				{
					if (_connected)
					{
						_netstream = new NetworkStream(_socket, true);
						_stream = _netstream;
						_stream.BeginRead(_buff, 0, _buff.Length, new AsyncCallback(Receive), null);
						_states.State = new ConnectedState();
						_states.Execute();
					}
				}
				else
				{
					Errors.Instance.SendError(this, ErrorType.ConnectionTimeout, "Timed out while connecting to server.");
				}
			}
			catch (SocketException)
			{
				//We Failed to connect
				//TODO: Return an error using the Errors class so that the hosting application can take action.
			}
		}
		
		private void FinishConnect(IAsyncResult ar)
		{
			try
			{
				_socket.EndConnect(ar);
				_connected = true;
			}
			catch (Exception)
			{
				//TODO: Return an error because we failed to connect
			}
			finally
			{
				_resetEvent.Set();
			}
		}

		/// <summary>
		/// Encrypts the connection using SSL/TLS
		/// </summary>
		public void StartSecure()
		{
			//_encrypting = true;
			Logger.Debug(this, "Starting .NET Secure Mode");
			_sslstream = new SslStream(_stream, true, new RemoteCertificateValidationCallback(RemoteValidation), null);
			Logger.Debug(this, "Authenticating as Client");
			try
			{
				_sslstream.AuthenticateAsClient(_dest.Hostname, null, SslProtocols.Tls, false);
				if (_sslstream.IsAuthenticated)
				{
					_stream = _sslstream;
					//_resetEvent.Set();
				}
				//_resetEvent.WaitOne();
			} catch (Exception e)
			{
				Logger.ErrorFormat(this, "SSL Error: {0}", e);
				Errors.Instance.SendError(this, ErrorType.XMLError, "SSL connection error", true);
			}
			//_encrypting = false;
		}
		
		/*
		private void EndAuthenticate(IAsyncResult result)
		{
			_sslstream.EndAuthenticateAsClient(result);
			if (_sslstream.IsAuthenticated)
			{
				_stream = _sslstream;
				_resetEvent.Set();
			}
		} */

		private static bool RemoteValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
		{
			if (errors == SslPolicyErrors.None)
			{
				return true;
			}

			Logger.DebugFormat(typeof(AsyncSocket), "Policy Errors: {0}", errors);
			return false;
		}

		/// <summary>
		/// Closes the current socket.
		/// </summary>
		public void Close()
		{
			Logger.Debug(this, "Closing socket (Graceful Shutdown)");
			_stream.Close();
			_socket.Close();
		}

		/// <summary>
		/// Writes data to the current connection.
		/// </summary>
		/// <param name="msg">Message to send</param>
		public void Write(string msg)
		{
			if (_connected)
			{
				Logger.DebugFormat(this, "Outgoing Message: {0}", msg);
				byte[] mesg = _utf.GetBytes(msg);
				_stream.Write(mesg, 0, mesg.Length);			
			}
		}

		private void Receive(IAsyncResult ar)
		{
			try
			{
				if (!_connected || _states.State is ClosedState)
				{
					return;
				}
				//Logger.Debug(this, ar.GetType().FullName);
				int rx = _stream.EndRead(ar);

				string m = _utf.GetString(TrimNull(_buff));

				Logger.DebugFormat(this, "Incoming Message: {0}", m);
				if (!_encrypting)
					ProtocolParser.Parse(m, rx);
				_stream.BeginRead(_buff, 0, _buff.Length, new AsyncCallback(Receive), null);
			}
			catch (SocketException e)
			{
				Logger.DebugFormat(this, "Socket Exception: {0}", e);
			}
			catch (InvalidOperationException e)
			{
				Logger.DebugFormat(this, "Invalid Operation: {0}", e);
			}
			catch (Exception e)
			{
				throw e;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="algorithm"></param>
		public void StartCompression(string algorithm)
		{
			Logger.DebugFormat(this, "Replacing stream with {0} compressed version.", algorithm);
			_stream = CompressionRegistry.Instance.GetCompression(algorithm, _stream);
		}

		private byte[] TrimNull(byte[] message)
		{
			if (message.Length > 1)
			{
				int c = message.Length - 1;
				while (message[c] == 0x00)
				{
					c--;
				}

				byte[] r = new byte[(c + 1)];
				for (int i = 0; i < (c + 1); i++)
				{
					r[i] = message[i];
				}

				return r;
			}

			return null;
		}

		/// <summary>
		/// Gets the current status of the socket.
		/// </summary>
		public bool Connected
		{
			get { return _connected; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Hostname
		{
			get { return _hostname; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool Secure
		{
			get { return _secure; }
			set { _secure = value; }
		}
	}
}
