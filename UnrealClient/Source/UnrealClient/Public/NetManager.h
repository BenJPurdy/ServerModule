// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "Interfaces/IPv4/IPv4Address.h"
#include "Interfaces/IPv4/IPv4Endpoint.h"
#include "IPAddress.h"
#include "SocketSubsystem.h"
#include "Common/UdpSocketBuilder.h"


#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "NetManager.generated.h"



UCLASS()
class UNREALCLIENT_API ANetManager : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ANetManager();

	FIPv4Address serverEndPoint;
	FIPv4Address localEndPoint;

	TSharedPtr<FInternetAddr> serverAddress;

	FSocket* socket;
	
	//network manager has (Packet*)deserialise which will make a memoryreader
	//this then determins the type of incoming packet
	//and casts the set packet(with data attached) to 
	//the correct subclass
	//
	
	//	TArray<char> inStream;
	//	FMemoryReader (instream)
	//	read client
	//	read type
	//	Packet* p;
	//	switch(type) call specialised deserialise
	//	p = deserialise(FMemoryReader&, type)
	// 
	//	p = (TransformPacket)p.deserialise();
	// 
	// 
	//

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	virtual void EndPlay(EEndPlayReason::Type) override;


public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

};

class Packet
{
public:
	enum PacketType : uint8_t
	{
		Connect,
		Disconnect,
		Data,
		Transform,
		ReqestJoin,
		UniqueID,
	};

	Packet();

	void Serialise(TArray<uint8_t>* out)
	{
		FMemoryWriter writer(*out);
	}

	void deserilaise(TArray<uint8_t>);
	Packet* deserilaise(FMemoryReader&, PacketType);
	uint32_t serialise(TArray<uint8_t>&);

	void addFloat(float);
	void addVec3(FVector);

	uint32_t client;
	uint8_t type; 
	uint32_t length;
	TArray<uint8_t> data; //remove later
};

Packet* Packet::deserilaise(FMemoryReader& reader, PacketType type)
{
	switch (type)
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
		
	}
		
		
	}
}

class TransformPacket : public Packet
{
public:
	FVector3f position;
	FVector3f rotation;

	void deserialise(TArray<uint8_t> in)
	{
		type = PacketType::Transform;
		FMemoryReader reader(in);
		reader.Serialize(&client, sizeof(client));
		reader.Serialize(&type, sizeof(type));
		reader.Serialize(&position, sizeof(FVector3f));
	}

	void serialise(TArray<uint8_t>& out)
	{
		FMemoryWriter writer(out);
		writer.Serialize(&client, sizeof(client));
		writer.Serialize(&type, sizeof(type));
		swizzleToUnity(position);
		swizzleToUnity(rotation);
		writer.Serialize(&position, sizeof(FVector3f));
		writer.Serialize(&rotation, sizeof(FVector3f));

	}
};

class RequestJoin : public Packet
{
	PacketType type = PacketType::ReqestJoin;

	void serialise()
	{
		FMemoryWriter writer(data);
		writer.Serialize(&client, sizeof(client));
		writer.Serialize(&type, sizeof(type));
	}
};

class UniqueID : public Packet
{
	PacketType type = PacketType::UniqueID;

	void deserialise()
	{
		FMemoryReader reader(data);

	}
};

//unity		x y z
//unreal	z x y

//unreal	x y z
//unity		y z x

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

static void transform(ANetManager& network, AActor& entity)
{
	TransformPacket packet;
	packet.type = Packet::PacketType::Transform;
	auto position = entity.GetActorTransform().GetLocation();
	auto rotation = entity.GetActorTransform().GetRotation();
	auto rotAsVec3 = rotation.Vector();
	packet.addVec3(position);
	packet.addVec3(rotAsVec3);
	TArray<uint8_t> toSend;
	uint32_t size = packet.serialise(toSend);
	int32_t sent = 0;
	network.socket->SendTo(toSend.GetData(), size, sent, *network.serverAddress);
	//UE_LOG(LogTemp, Warning, TEXT("Sent Data"));
}

static void requestJoin()
{

}
