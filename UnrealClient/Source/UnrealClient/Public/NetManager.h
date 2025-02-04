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
	enum PacketType
	{
		Connect,
		Disconnect,
		Data,
	};

	Packet();

	void Serialise(TArray<uint8_t>* out)
	{
		FMemoryWriter writer(*out);
	}

	void Deserilaise(TArray<uint8_t>);
	uint32_t serialise(TArray<uint8_t>&);

	void addFloat(float);
	void addVec3(FVector);

	uint32_t client;
	uint8_t type;
	uint32_t length;
	TArray<uint8_t> data;
};

static void sendData(ANetManager& network, AActor& entity)
{
	
	Packet packet;
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
