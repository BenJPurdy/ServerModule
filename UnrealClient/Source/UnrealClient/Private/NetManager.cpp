// Fill out your copyright notice in the Description page of Project Settings.


#include "NetManager.h"

#define PRINT(...) GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT(__VA_ARGS__))

// Sets default values
ANetManager::ANetManager()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

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

	int32_t dataSent;
	//int32_t dataRecived;
	UE_LOG(LogTemp, Log, TEXT("Socket complete"));
	FString dataToSend = "Server Connect";
	std::vector<uint8_t> blobData;
	blobData.resize(dataToSend.Len() + 1);
	dataToSend.ToBlob(dataToSend, &blobData[0], dataToSend.Len());

	TArray<uint8_t> sendData;
	FMemoryWriter writer(sendData);
	writer.Serialize(&dataToSend[0], dataToSend.Len() * 2);

	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT("Socket Complete"));
	socket->SendTo(&sendData[0], sendData.Num(), dataSent, *sendAddr);
	PRINT("Sent to server");
	//socket->Recv()

	Super::BeginPlay();
	
}

// Called every frame
void ANetManager::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void Packet::Deserilaise(TArray<uint8_t> in)
{
	FMemoryReader reader(in);
	reader.Serialize(&client, sizeof(client));
	reader.Serialize(&type, sizeof(type));
	reader.Serialize(&length, sizeof(length));
	reader.Serialize(&data, length);
}

