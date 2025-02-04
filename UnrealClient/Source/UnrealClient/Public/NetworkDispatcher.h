// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "NetManager.h"


#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "NetworkDispatcher.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class UNREALCLIENT_API UNetworkDispatcher : public UActorComponent
{
	GENERATED_BODY()
	ANetManager* manager;
	AActor* owner;

public:	
	// Sets default values for this component's properties
	UNetworkDispatcher();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

		
};
