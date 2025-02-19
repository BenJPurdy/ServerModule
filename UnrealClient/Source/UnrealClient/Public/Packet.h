// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "SocketSubsystem.h"
#include "Common/UdpSocketBuilder.h"

#include "CoreMinimal.h"

/**
 * 
 */
class UNREALCLIENT_API Packet
{
public:
	enum PacketType : uint8_t
	{
		Connect,
		Disconnect,
		Data,
		Transform,
		RequestJoin,
		UniqueID,
	};

	Packet();


	//void Serialise(TArray<uint8_t>* out)
	//{
	//	FMemoryWriter writer(*out);
	//}

	void deserilaise(TArray<uint8_t>);
	Packet* deserilaise(FMemoryReader& reader, PacketType packetType);

	uint32_t serialise(FMemoryWriter& writer, PacketType packetType);
	//uint32_t serialise(FMemoryWriter&, Packet*);
	uint32_t serialise(TArray<uint8_t>&);



	void addFloat(float);
	void addVec3(FVector);

	uint32_t client;
	uint8_t type;
	uint32_t length;
	TArray<uint8_t> data; //remove later
};

class TransformPacket : public Packet
{
public:
	FVector3f position{ 0.0f, 0.0f, 0.0f };
	FVector3f rotation{ 0.0f, 0.0f, 0.0f };

	//void deserialise(TArray<uint8_t> in)
	//{
	//	type = PacketType::Transform;
	//	FMemoryReader reader(in);
	//	reader.Serialize(&client, sizeof(client));
	//	reader.Serialize(&type, sizeof(type));
	//	reader.Serialize(&position, sizeof(FVector3f));
	//}
	//
	//void serialise(TArray<uint8_t>& out)
	//{
	//	FMemoryWriter writer(out);
	//	writer.Serialize(&client, sizeof(client));
	//	writer.Serialize(&type, sizeof(type));
	//	swizzleToUnity(position);
	//	swizzleToUnity(rotation);
	//	rotation *= 100;
	//	writer.Serialize(&position, sizeof(FVector3f));
	//	writer.Serialize(&rotation, sizeof(FVector3f));
	//}

	void makeTransformPacket(AActor* actor)
	{
		position = (FVector3f)actor->GetActorLocation();
		rotation = (FVector3f)actor->GetActorTransform().GetRotation().Euler();
	}
};

class RequestJoinPacket : public Packet
{
	PacketType type = PacketType::RequestJoin;

	//void serialise()
	//{
	//	FMemoryWriter writer(data);
	//	writer.Serialize(&client, sizeof(client));
	//	writer.Serialize(&type, sizeof(type));
	//}
};

class UniqueIDPacket : public Packet
{
public:
	uint32_t unique;

};

class ConnectPacket : public Packet
{
public:
	ConnectPacket()
	{
		type = PacketType::Connect;
	}
};

static void swizzleFromUnity(FVector3f& unityVector)
{
	unityVector = { unityVector.Z, unityVector.X, unityVector.Y };
	unityVector *= 100;
}

static void swizzleToUnity(FVector3f& unrealVector)
{
	unrealVector = { unrealVector.Y, unrealVector.Z, unrealVector.X };
	unrealVector /= 100;
}
