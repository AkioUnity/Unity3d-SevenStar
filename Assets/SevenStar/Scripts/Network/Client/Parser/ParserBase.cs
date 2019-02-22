using System;
using System.Collections;
using System.Collections.Generic;
public class RecvPacketObject
{
    public Protocols protocol;
    public object obj = null;
}

public interface ParserBase
{
    RecvPacketObject Parser(Protocols protocol, byte[] data);
}

public class PacketParser
{
    delegate RecvPacketObject dParser(Protocols protocol, byte[] data);
    List<ParserBase> m_ArrParser = new List<ParserBase>();
    dParser OnParser = null;

    public void InsertParser(ParserBase parser)
    {
        if (parser == null)
            return;
        m_ArrParser.Add(parser);
        OnParser += parser.Parser;
    }

    public RecvPacketObject Parser(Protocols protocol, byte[] data)
    {
        int i, j;
        j = m_ArrParser.Count;
        for (i = 0; i < j; i++)
        {
            RecvPacketObject obj = m_ArrParser[i].Parser(protocol, data);
            if (obj != null) return obj;
        }
        //return OnParser(protocol, data);
        return null;
    }
}