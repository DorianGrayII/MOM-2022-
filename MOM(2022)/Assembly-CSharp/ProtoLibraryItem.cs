using ProtoBuf;
using System;
using WorldCode;

[ProtoInclude(200, typeof(Vertex)), ProtoInclude(0xc9, typeof(MeshCell)), ProtoInclude(0xca, typeof(Chunk)), ProtoInclude(0xcb, typeof(PlaneMeshData)), ProtoContract]
public abstract class ProtoLibraryItem
{
    protected ProtoLibraryItem()
    {
    }
}

