// Fill out your copyright notice in the Description page of Project Settings.


#include "NetManager.h"
#include <bit>
#include <array>

#define PRINT(...) GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT(__VA_ARGS__))
#define PRINTDATA(p) GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Yellow, p)

TArray<UNetworkGameObject*> ANetManager::networkObjects;

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
	networkObjects.Empty();
	UNetworkGameObject::lastLocalID = 0;
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
	{
	TArray<uint8_t> sendData;
	sendData.SetNumZeroed(sizeof(ConnectPacket));
	FMemoryWriter writer(sendData);
	Packet connect;
	auto out = connect.serialise(writer, Packet::PacketType::Connect);
	//writer.Serialize(&dataToSend[0], dataToSend.Len() );

	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Emerald, TEXT("Socket Complete"));
	socket->SendTo(&sendData[0], sendData.Num(), dataSent, *sendAddr);
	PRINT("Sent to server");
	}
	//socket->Recv()

	RequestJoinPacket rjp;
	for (auto& obj : networkObjects)
	{
		TArray<uint8_t> sendData, recvData;
		FMemoryWriter writer(sendData);
		FMemoryReader reader(recvData);
		reader.Serialize((void*)0, sizeof(int));
		rjp.serialise(writer, Packet::PacketType::RequestJoin);
		int sent = send(sendData);
		UE_LOG(LogTemp, Log, TEXT("%s sent %i bytes"), *obj->owner->GetName(), sent);
		uint32_t incomingSize = 0;
		int32_t readBytes = 0;
		socket->HasPendingData(incomingSize);
		recvData.SetNumZeroed(incomingSize);
		socket->Recv(recvData.GetData(), incomingSize, readBytes);
		Packet inPacket;
		uint32_t client;
		uint8_t type;
		reader.Serialize(&client, sizeof(client));
		reader.Serialize(&type, sizeof(type));

		inPacket.deserilaise(reader, (Packet::PacketType)type);
		obj->networkID = inPacket.UniqueID;
	}

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

	{
		for (auto& obj : networkObjects)
		{
			if (obj->isLocal && obj->networkID == 0)
			{
				RequestJoinPacket rjp;
				TArray<uint8_t> sendData, recvData;
				FMemoryWriter writer(sendData);
				FMemoryReader reader(recvData);
				reader.Serialize((void*)0, sizeof(int));
				rjp.serialise(writer, Packet::PacketType::RequestJoin);
				int sent = send(sendData);
				uint32_t incomingSize = 0;
				int32_t readBytes2 = 0;
				socket->HasPendingData(incomingSize);
				recvData.SetNumZeroed(incomingSize);
				socket->Recv(recvData.GetData(), incomingSize, readBytes2);
				Packet inPacket;
				uint32_t client;
				uint8_t type;
				reader.Serialize(&client, sizeof(client));
				reader.Serialize(&type, sizeof(type));
				auto deserialisedPacket = inPacket.deserilaise(reader, (Packet::PacketType)type);
				UniqueIDPacket* uidpacket = (UniqueIDPacket*)deserialisedPacket;
				obj->networkID = uidpacket->unique;

			}
			TArray<uint8_t> sendData;
			int32_t dataSent = 0;
			FMemoryWriter writer(sendData);
			writer.Serialize(&dataSent, sizeof(uint32_t));
			obj->transformPacket.serialise(writer, Packet::PacketType::Transform);
			socket->SendTo(&sendData[0], sendData.Num(), dataSent, *serverAddress);
			UE_LOG(LogTemp, Log, TEXT("%i"), dataSent);
		}
	}

	Super::Tick(DeltaTime);
}





