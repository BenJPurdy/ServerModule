// Fill out your copyright notice in the Description page of Project Settings.


#include "Packet.h"
#include <bit>
#include <array>

Packet::Packet()
{
	client = 0;
	type = 2;
	length = 0;
}

void Packet::deserilaise(TArray<uint8_t> in)
{
	FMemoryReader reader(in);
	reader.Serialize(&client, sizeof(client));
	reader.Serialize(&type, sizeof(type));
	reader.Serialize(&length, sizeof(length));
	data.SetNumZeroed(length);
	reader.Serialize(data.GetData(), length);
}

uint32_t Packet::serialise(TArray<uint8_t>& out)
{
	FMemoryWriter writer(out);
	writer.Serialize(&client, sizeof(client));
	writer.Serialize(&type, sizeof(type));
	writer.Serialize(&length, sizeof(length));
	writer.Serialize(data.GetData(), length);
	return (uint32_t)(sizeof(client) + sizeof(type) + sizeof(length) + length);
}

void Packet::addFloat(float v)
{
	std::array<uint8_t, 4> bytes = std::bit_cast<std::array<uint8_t, 4>>(v);
	for (auto i : bytes)
	{
		data.Add(i);
		length += sizeof(uint8_t);
	}
}

void Packet::addVec3(FVector v)
{
	addFloat(v.X);
	addFloat(v.Y);
	addFloat(v.Z);

}

uint32_t Packet::serialise(FMemoryWriter& writer, PacketType packetType)
{
	switch (packetType)
	{
	case PacketType::Transform:
	{
		PacketType t = PacketType::Transform;
		auto len = length + sizeof(TransformPacket);
		writer.Serialize(&t, sizeof(PacketType));
		writer.Serialize(&len, sizeof(length));
		TransformPacket* tmp = (TransformPacket*)this;
		swizzleToUnity(tmp->position);
		swizzleToUnity(tmp->rotation);
		writer.Serialize(&tmp->entity, sizeof(uint32_t));
		writer.Serialize(&tmp->position, sizeof(FVector3f));
		writer.Serialize(&tmp->rotation, sizeof(FVector3f));
		writer.Serialize(&tmp->padding, sizeof(uint32_t));
		return 1;
	}
	case PacketType::Connect:
	{
		PacketType t = PacketType::Connect;
		uint32_t length = sizeof(uint32_t) + sizeof(PacketType) + sizeof(uint32_t);
		writer.Serialize(&length, sizeof(length));
		writer.Serialize(&t, sizeof(PacketType));
		return 1;
	}
	case PacketType::Disconnect:
	{
		PacketType t = PacketType::Disconnect;
		writer.Serialize(&t, sizeof(PacketType));
		return 1;
	}
	case PacketType::RequestJoin:
	{
		PacketType t = PacketType::RequestJoin;
		writer.Serialize(&t, sizeof(PacketType));
		return 1;
	}
	}
	return 0;
}

Packet* Packet::deserilaise(FMemoryReader& reader, PacketType packetType)
{
	switch (packetType)
	{
	case PacketType::Transform:
	{
		FVector3f p, r;
		reader.Serialize(&p, sizeof(FVector3f));
		reader.Serialize(&r, sizeof(FVector3f));
		swizzleFromUnity(p);
		swizzleFromUnity(r);

		return new TransformPacket
		{
			.position = p,
			.rotation = r
		};
	}
	case PacketType::UniqueID:
	{
		uint32_t u;
		reader.Serialize(&u, sizeof(uint32_t));
		return new UniqueIDPacket
		{
			.unique = u
		};
	}


	}

	return nullptr;
}