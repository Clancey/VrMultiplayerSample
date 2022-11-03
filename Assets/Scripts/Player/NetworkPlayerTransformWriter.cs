public class NetworkPlayerTransformWriter
{
    ByteWriter writer = new ByteWriter();
    ByteReader reader = new ByteReader();

    public byte[] GetBytes (PlayerTransformData data)
    {
        writer.Clear();
        writer.Write(data.Position);
        writer.Write(data.HeadPosition);
        writer.Write(data.LeftHandPosition);
        writer.Write(data.RightHandPosition);
        writer.WriteCompressed(data.Rotation);
        writer.WriteCompressed(data.HeadRotation);
        writer.WriteCompressed(data.LeftHandRotation);
        writer.WriteCompressed(data.RightHandRotation);

        writer.WritePackedUInt32(data.IsLeftHandEquiped);
        if (data.IsLeftHandEquiped > 0) {
            writer.Write(data.LeftHandEquipedPosition);
            writer.WriteCompressed(data.LeftHandEquipedRotation);
        }
        writer.WritePackedUInt32(data.IsRightHandEquiped);
        if (data.IsRightHandEquiped > 0)
        {
            writer.Write(data.RightHandEquipedPosition);
            writer.WriteCompressed(data.RightHandEquipedRotation);
        }
        return writer.ToArray();
    }

    public PlayerTransformData FromBytes (byte[] bytes)
    {
        PlayerTransformData data = new PlayerTransformData();

        reader.Replace(bytes);

        data.Position = reader.ReadVector3();
        data.HeadPosition = reader.ReadVector3();
        data.LeftHandPosition = reader.ReadVector3();
        data.RightHandPosition = reader.ReadVector3();
        data.Rotation = reader.ReadCompressedRotation();
        data.HeadRotation = reader.ReadCompressedRotation();
        data.LeftHandRotation = reader.ReadCompressedRotation();
        data.RightHandRotation = reader.ReadCompressedRotation();

        data.IsLeftHandEquiped = reader.ReadPackedUInt32();
        if(data.IsLeftHandEquiped > 0)
        {
            data.LeftHandEquipedPosition = reader.ReadVector3();
            data.LeftHandEquipedRotation = reader.ReadCompressedRotation();
        }

        data.IsRightHandEquiped = reader.ReadPackedUInt32();
        if (data.IsRightHandEquiped > 0)
        {
            data.RightHandEquipedPosition = reader.ReadVector3();
            data.RightHandEquipedRotation = reader.ReadCompressedRotation();
        }

        return data;
    }
}