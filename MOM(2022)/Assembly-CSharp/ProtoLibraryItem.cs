using ProtoBuf;
using WorldCode;

[ProtoInclude(200, typeof(Vertex))]
[ProtoInclude(201, typeof(MeshCell))]
[ProtoInclude(202, typeof(Chunk))]
[ProtoInclude(203, typeof(PlaneMeshData))]
[ProtoContract]
public abstract class ProtoLibraryItem
{
}
