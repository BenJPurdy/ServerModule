// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "Interfaces/IPv4/IPv4Address.h"
#include "Interfaces/IPv4/IPv4Endpoint.h"
#include "IPAddress.h"
#include "SocketSubsystem.h"
#include "Common/UdpSocketBuilder.h"
#include "Packet.h"
#include "NetworkGameObject.h"


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

	static TArray<UNetworkGameObject*> networkObjects;
	
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

private:
	int send(TArray<uint8_t>& data)
	{
		int ret = 0;
		socket->SendTo(&data[0], data.Num(), ret, *serverAddress);
		return ret;
	}

};








//unity		x y z
//unreal	z x y

//unreal	x y z
//unity		y z x


/*
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
*/