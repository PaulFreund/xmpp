// client.cs
//
//Copyright © 2006 - 2012 Dieter Lunn
//Modified 2012 Paul Freund ( freund.paul@lvl3.org )
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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XMPP.registries;

namespace XMPP.tags.jabber.client
{
    public class Namespace
    {
        public static string Name = "jabber:client";
        public static XName message = XName.Get("message", Name);
        public static XName iq = XName.Get("iq", Name);
        public static XName presence = XName.Get("presence", Name);
        public static XName priority = XName.Get("priority", Name);
        public static XName show = XName.Get("show", Name);
        public static XName status = XName.Get("status", Name);
        public static XName body = XName.Get("body", Name);
        public static XName subject = XName.Get("subject", Name);
        public static XName thread = XName.Get("thread", Name);
        public static XName error = XName.Get("error", Name);
	public static XName received = XName.Get("received", Name);
    }

    [XMPPTag(typeof(Namespace), typeof(message))]
    public class message : Tag
    {
        public message() : base(Namespace.message) { id = NextId(); }
        public message(XElement other) : base(other) { id = NextId(); }

        public enum typeEnum
        {
            none,
            chat,
            error,
            groupchat,
            headline,
            normal
        }

		public string from { get { return (string)GetAttributeValue("from"); } set { SetAttributeValue("from", value); } }
		public string to { get { return (string)GetAttributeValue("to"); } set { SetAttributeValue("to", value); } }
        public typeEnum type { get { return GetAttributeEnum<typeEnum>("type"); } set { SetAttributeEnum<typeEnum>("type", value); } }
		public string id { get { return (string)GetAttributeValue("id"); } set { SetAttributeValue("id", value); } }
        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }

        public IEnumerable<body> bodyElements { get { return Elements<body>(Namespace.body); } }
        public IEnumerable<subject> subjectElements { get { return Elements<subject>(Namespace.subject); } }
        public IEnumerable<thread> threadElements { get { return Elements<thread>(Namespace.thread); } }
        public IEnumerable<error> errorElements { get { return Elements<error>(Namespace.error); } }

        public string body { get { return string.Join(" ", (from body in bodyElements select body.Value)); } }
    }


    [XMPPTag(typeof(Namespace), typeof(body))]
    public class body : Tag
    {
        public body() : base(Namespace.body) {} 
        public body(XElement other) : base(other) {}

        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }
    }


    [XMPPTag(typeof(Namespace), typeof(subject))]
    public class subject : Tag
    {
        public subject() : base(Namespace.subject) {} 
        public subject(XElement other) : base(other) {}

        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }
    }


    [XMPPTag(typeof(Namespace), typeof(thread))]
    public class thread : Tag
    {
        public thread() : base(Namespace.thread) {} 
        public thread(XElement other) : base(other) {}

		public string parent { get { return (string)GetAttributeValue("parent"); } set { SetAttributeValue("parent", value); } }
    }

    [XMPPTag(typeof(Namespace), typeof(presence))]
    public class presence : Tag
    {
        public presence() : base(Namespace.presence) { id = NextId(); }
        public presence(XElement other) : base(other) { id = NextId(); }

        public enum typeEnum
        {
            none,
            error,
            probe,
            subscribe,
            subscribed,
            unavailable,
            unsubscribe,
            unsubscribed
        }

		public string from { get { return (string)GetAttributeValue("from"); } set { SetAttributeValue("from", value); } }
		public string to { get { return (string)GetAttributeValue("to"); } set { SetAttributeValue("to", value); } }
        public typeEnum type { get { return GetAttributeEnum<typeEnum>("type"); } set { SetAttributeEnum<typeEnum>("type", value); } }
		public string id { get { return (string)GetAttributeValue("id"); } set { SetAttributeValue("id", value); } }
        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }

        public IEnumerable<show> showElements { get { return Elements<show>(Namespace.show); } }
        public IEnumerable<status> statusElements { get { return Elements<status>(Namespace.status); } }
        public IEnumerable<priority> priorityElements { get { return Elements<priority>(Namespace.priority); } }
        public IEnumerable<error> errorElements { get { return Elements<error>(Namespace.error); } }
    }


    [XMPPTag(typeof(Namespace), typeof(show))]
    public class show : Tag
    {
        public show() : base(Namespace.show) {} 
        public show(XElement other) : base(other) {}

        // Enums
        public enum valueEnum
        {
            none,
            away,
            chat,
            dnd,
            xa
        }

        public new valueEnum Value { get { return (valueEnum)Enum.Parse(typeof(valueEnum), base.Value, true); } set { base.Value = value.ToString(); } }
    }


    [XMPPTag(typeof(Namespace), typeof(status))]
    public class status : Tag
    {
        public status() : base(Namespace.status) {} 
        public status(XElement other) : base(other) {}

        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }
        public new string Value { get { return base.Value; } set { if (value.Length < 1 || value.Length > 1024) { throw new Exception("Text out of range"); } base.Value = value; } } // string1024
    }


    [XMPPTag(typeof(Namespace), typeof(priority))]
    public class priority : Tag
    {
        public priority() : base(Namespace.priority) {} 
        public priority(XElement other) : base(other) {}

        public new int Value { get { return Convert.ToInt32(base.Value); } set { base.Value = value.ToString(); } }
    }

    [XMPPTag(typeof(Namespace), typeof(iq))]
    public class iq : Tag
    {
        public iq() : base(Namespace.iq) { id = NextId(); }
        public iq(XElement other) : base(other) { id = NextId(); }

        public enum typeEnum
        {
            none,
            error,
            get,
            result,
            set
        }

		public string from { get { return (string)GetAttributeValue("from"); } set { SetAttributeValue("from", value); } }
		public string to { get { return (string)GetAttributeValue("to"); } set { SetAttributeValue("to", value); } }
        public typeEnum type { get { return GetAttributeEnum<typeEnum>("type"); } set { SetAttributeEnum<typeEnum>("type", value); } }
		public string id { get { return (string)GetAttributeValue("id"); } set { SetAttributeValue("id", value); } }
        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }

        public IEnumerable<error> errorElements { get { return Elements<error>(Namespace.error); } }
        public Tag Payload { get { return Tag.Get(Elements().FirstOrDefault()); } }
    }


    [XMPPTag(typeof(Namespace), typeof(error))]
    public class error : Tag
    {
        public error() : base(Namespace.error) {} 
        public error(XElement other) : base(other) {}

        // Enums
        public enum typeEnum
        {
            none,
            auth,     
            cancel,   
            @continue, 
            modify,   
            wait,     
        }

		public string by { get { return (string)GetAttributeValue("by"); } set { SetAttributeValue("by", value); } }

        public typeEnum type { get { return GetAttributeEnum<typeEnum>("type"); } set { SetAttributeEnum<typeEnum>("type", value); } }

        // Has children
    }
    
	
	
	
    [XMPPTag(typeof(Namespace), typeof(received))]
    public class received : Tag
    {
        public received() : base(Namespace.received) { }
        public received(XElement other) : base(other) { id = NextId(); }

        public enum typeEnum
        {
            none,
            
        }

        public string from { get { return (string)GetAttributeValue("from"); } set { SetAttributeValue("from", value); } }
        public string to { get { return (string)GetAttributeValue("to"); } set { SetAttributeValue("to", value); } }
        public typeEnum type { get { return GetAttributeEnum<typeEnum>("type"); } set { SetAttributeEnum<typeEnum>("type", value); } }
        public string id { get { return (string)GetAttributeValue("id"); } set { SetAttributeValue("id", value); } }
        public string lang { get { return (string)GetAttributeValue(XName.Get("lang", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("lang", xml.Namespace.Name), value); } }
        public string xmlns { get { return (string)GetAttributeValue(XName.Get("xmlns", xml.Namespace.Name)); } set { SetAttributeValue(XName.Get("xmlns", xml.Namespace.Name), value); } }

        public IEnumerable<body> bodyElements { get { return Elements<body>(Namespace.body); } }
        public IEnumerable<subject> subjectElements { get { return Elements<subject>(Namespace.subject); } }
        public IEnumerable<thread> threadElements { get { return Elements<thread>(Namespace.thread); } }
        public IEnumerable<error> errorElements { get { return Elements<error>(Namespace.error); } }

        public string body { get { return string.Join(" ", (from body in bodyElements select body.Value)); } }
    }



}

/*
<?xml version='1.0' encoding='UTF-8'?>

<xs:schema
    xmlns:xs='http://www.w3.org/2001/XMLSchema'
    targetNamespace='jabber:client'
    xmlns='jabber:client'
    elementFormDefault='qualified'>

  <xs:import namespace='urn:ietf:params:xml:ns:xmpp-stanzas'
             schemaLocation='http://xmpp.org/schemas/stanzaerror.xsd'/>
  <xs:import namespace='http://www.w3.org/XML/1998/namespace'
             schemaLocation='http://www.w3.org/2001/03/xml.xsd'/>

  <xs:element name='message'>
     <xs:complexType>
        <xs:sequence>
          <xs:choice minOccurs='0' maxOccurs='unbounded'>
            <xs:element ref='subject'/>
            <xs:element ref='body'/>
            <xs:element ref='thread'/>
          </xs:choice>
          <xs:any     namespace='##other'
                      minOccurs='0'
                      maxOccurs='unbounded'
                      processContents='lax'/>
          <xs:element ref='error'
                      minOccurs='0'/>
        </xs:sequence>
        <xs:attribute name='from'
                      type='xs:string'
                      use='optional'/>
        <xs:attribute name='id'
                      type='xs:NMTOKEN'
                      use='optional'/>
        <xs:attribute name='to'
                      type='xs:string'
                      use='optional'/>
        <xs:attribute name='type' 
                      use='optional' 
                      default='normal'>
          <xs:simpleType>
            <xs:restriction base='xs:NMTOKEN'>
              <xs:enumeration value='chat'/>
              <xs:enumeration value='error'/>
              <xs:enumeration value='groupchat'/>
              <xs:enumeration value='headline'/>
              <xs:enumeration value='normal'/>
            </xs:restriction>
          </xs:simpleType>
        </xs:attribute>
        <xs:attribute ref='xml:lang' use='optional'/>
     </xs:complexType>
  </xs:element>

  <xs:element name='body'>
    <xs:complexType>
      <xs:simpleContent>
        <xs:extension base='xs:string'>
          <xs:attribute ref='xml:lang' use='optional'/>
        </xs:extension>
      </xs:simpleContent>
    </xs:complexType>
  </xs:element>

  <xs:element name='subject'>
    <xs:complexType>
      <xs:simpleContent>
        <xs:extension base='xs:string'>
          <xs:attribute ref='xml:lang' use='optional'/>
        </xs:extension>
      </xs:simpleContent>
    </xs:complexType>
  </xs:element>

  <xs:element name='thread'>
    <xs:complexType>
      <xs:simpleContent>
        <xs:extension base='xs:NMTOKEN'>
          <xs:attribute name='parent'
                        type='xs:NMTOKEN'
                        use='optional'/>
        </xs:extension>
      </xs:simpleContent>
    </xs:complexType>
  </xs:element>

  <xs:element name='presence'>
    <xs:complexType>
      <xs:sequence>
        <xs:choice minOccurs='0' maxOccurs='unbounded'>
          <xs:element ref='show'/>
          <xs:element ref='status'/>
          <xs:element ref='priority'/>
        </xs:choice>
        <xs:any     namespace='##other'
                    minOccurs='0'
                    maxOccurs='unbounded'
                    processContents='lax'/>
        <xs:element ref='error'
                    minOccurs='0'/>
      </xs:sequence>
      <xs:attribute name='from'
                    type='xs:string'
                    use='optional'/>
      <xs:attribute name='id'
                    type='xs:NMTOKEN'
                    use='optional'/>
      <xs:attribute name='to'
                    type='xs:string'
                    use='optional'/>
      <xs:attribute name='type' use='optional'>
        <xs:simpleType>
          <xs:restriction base='xs:NMTOKEN'>
            <xs:enumeration value='error'/>
            <xs:enumeration value='probe'/>
            <xs:enumeration value='subscribe'/>
            <xs:enumeration value='subscribed'/>
            <xs:enumeration value='unavailable'/>
            <xs:enumeration value='unsubscribe'/>
            <xs:enumeration value='unsubscribed'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
      <xs:attribute ref='xml:lang' use='optional'/>
    </xs:complexType>
  </xs:element>

  <xs:element name='show'>
    <xs:simpleType>
      <xs:restriction base='xs:NMTOKEN'>
        <xs:enumeration value='away'/>
        <xs:enumeration value='chat'/>
        <xs:enumeration value='dnd'/>
        <xs:enumeration value='xa'/>
      </xs:restriction>
    </xs:simpleType>
  </xs:element>

  <xs:element name='status'>
    <xs:complexType>
      <xs:simpleContent>
        <xs:extension base='string1024'>
          <xs:attribute ref='xml:lang' use='optional'/>
        </xs:extension>
      </xs:simpleContent>
    </xs:complexType>
  </xs:element>

  <xs:simpleType name='string1024'>
    <xs:restriction base='xs:string'>
      <xs:minLength value='1'/>
      <xs:maxLength value='1024'/>
    </xs:restriction>
  </xs:simpleType>

  <xs:element name='priority' type='xs:byte'/>

  <xs:element name='iq'>
    <xs:complexType>
      <xs:sequence>
        <xs:any     namespace='##other'
                    minOccurs='0'
                    maxOccurs='1'
                    processContents='lax'/>
        <xs:element ref='error'
                    minOccurs='0'/>
      </xs:sequence>
      <xs:attribute name='from'
                    type='xs:string'
                    use='optional'/>
      <xs:attribute name='id'
                    type='xs:NMTOKEN'
                    use='required'/>
      <xs:attribute name='to'
                    type='xs:string'
                    use='optional'/>
      <xs:attribute name='type' use='required'>
        <xs:simpleType>
          <xs:restriction base='xs:NMTOKEN'>
            <xs:enumeration value='error'/>
            <xs:enumeration value='get'/>
            <xs:enumeration value='result'/>
            <xs:enumeration value='set'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
      <xs:attribute ref='xml:lang' use='optional'/>
    </xs:complexType>
  </xs:element>

  <xs:element name='error'>
    <xs:complexType>
      <xs:sequence xmlns:err='urn:ietf:params:xml:ns:xmpp-stanzas'>
        <xs:group ref='err:stanzaErrorGroup'/>
        <xs:element ref='err:text'
                    minOccurs='0'/>
      </xs:sequence>
      <xs:attribute name='by' 
                    type='xs:string' 
                    use='optional'/>
      <xs:attribute name='type' use='required'>
        <xs:simpleType>
          <xs:restriction base='xs:NMTOKEN'>
            <xs:enumeration value='auth'/>
            <xs:enumeration value='cancel'/>
            <xs:enumeration value='continue'/>
            <xs:enumeration value='modify'/>
            <xs:enumeration value='wait'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
    </xs:complexType>
  </xs:element>
</xs:schema>

<xs:schema
    xmlns:xs='http://www.w3.org/2001/XMLSchema'
    targetNamespace='urn:xmpp:receipts'
    xmlns='urn:xmpp:receipts'
    elementFormDefault='qualified'>

  <xs:annotation>
    <xs:documentation>
      The protocol documented by this schema is defined in
      XEP-0184: http://xmpp.org/extensions/xep-0184.html
    </xs:documentation>
  </xs:annotation>

  <xs:element name='received'>
    <xs:complexType>
      <xs:simpleContent>
        <xs:extension base='empty'>
          <xs:attribute name='id' type='xs:string' use='optional'/>
        </xs:extension>
      </xs:simpleContent>
    </xs:complexType>
  </xs:element>

  <xs:element name='request' type='empty'/>

  <xs:simpleType name='empty'>
    <xs:restriction base='xs:string'>
      <xs:enumeration value=''/>
    </xs:restriction>
  </xs:simpleType>

</xs:schema>

*/
