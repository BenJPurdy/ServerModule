// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "Packet.h"

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "NetworkGameObject.generated.h"




UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALCLIENT_API UNetworkGameObject : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UNetworkGameObject();
	UPROPERTY(EditAnywhere);
	bool isLocal = false;
	UPROPERTY(EditAnywhere);
	uint32 networkID = 0;
	UPROPERTY(EditAnywhere);
	uint32 localID = 0;
	AActor* owner;
	static uint32 lastLocalID;

	TransformPacket transformPacket;

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

		
};
