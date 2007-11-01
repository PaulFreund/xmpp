//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
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
using xmpp.net;

namespace xmpp.states
{
	public class ProtocolState
	{
		protected State _state;
		private AsyncSocket _socket;
		
		public ProtocolState(AsyncSocket socket)
		{
			_socket = socket;
		}
		
		public void Execute(object data)
		{
			_state.Execute(data);
		}
		
		public State State
		{
			get { return _state; }
			set { _state = value; }
		}
		
		public AsyncSocket Socket
		{
			get { return _socket; }
		}
	}	
}
