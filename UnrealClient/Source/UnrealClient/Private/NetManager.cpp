// Fill out your copyright notice in the Description page of Project Settings.


#include "NetManager.h"
#include <bit>
#include <array>

#define PRINT(...) GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT(__VA_ARGS__))
#define PRINTDATA(p) GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Yellow, p)


// Sets default values
ANetManager::ANetManager()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

void ANetManager::EndPlay(EEndPlayReason::Type t)
{
	if (socket)
	{
		socket->Close();
		socket = nullptr;
	}
}

// Called when the game starts or when spawned
void ANetManager::BeginPlay()
{
	//FInternetAddr local = FInternetAddr::SetAnyAddress();
	FString ipAddr = "127.0.0.1";
	FIPv4Address::Parse(ipAddr, serverEndPoint);
	uint32_t port = 9050;

	localEndPoint = FIPv4Address::Any;

	//serverAddress = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->
	//	CreateInternetAddr(serverEndPoint.Value, port);
	
	socket = FUdpSocketBuilder("Socket").BoundToAddress(localEndPoint);

	if (!socket) 
	{
		GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT("Socket failed"));
	}

	TSharedPtr<FInternetAddr> sendAddr = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->CreateInternetAddr();
	
	sendAddr->SetIp(serverEndPoint.Value);
	sendAddr->SetPort(port);
	serverAddress = sendAddr;

	int32_t dataSent;
	//int32_t dataRecived;
	UE_LOG(LogTemp, Log, TEXT("Socket complete"));
	FString dataToSend = "Server Connect";
	std::vector<uint8_t> blobData;
	blobData.resize(dataToSend.Len() + 1);
	dataToSend.ToBlob(dataToSend, &blobData[0], dataToSend.Len() * sizeof(dataToSend[0]));

	TArray<uint8_t> sendData;
	FMemoryWriter writer(sendData);
	writer.Serialize(&dataToSend[0], dataToSend.Len() );

	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT("Socket Complete"));
	socket->SendTo(&sendData[0], sendData.Num(), dataSent, *sendAddr);
	PRINT("Sent to server");
	//socket->Recv()

	Super::BeginPlay();
	
}

// Called every frame
void ANetManager::Tick(float DeltaTime)
{
	//PRINT("OnTick");
	uint32_t pendingData = 0;
	int32_t readBytes = 0;
	//uint8_t* inData;
	if (socket->HasPendingData(pendingData))
	{
		TArray<uint8_t> packetStream;
		packetStream.SetNumZeroed(pendingData);
		socket->Recv(packetStream.GetData(), pendingData, readBytes);
		Packet packet;
		packet.deserilaise(packetStream);
		FString dataOut = FString(UTF8_TO_TCHAR(reinterpret_cast<const char*>(packet.data.GetData())));
		//packet.data.ToString()

		UE_LOG(LogTemp, Warning, TEXT("Return Data: %s"), *dataOut);
		
		
	}

	Super::Tick(DeltaTime);
}

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



