// Fill out your copyright notice in the Description page of Project Settings.


#include "NetworkGameObject.h"
#include "NetManager.h"

#include <array>
#include "Kismet/GameplayStatics.h"

uint32_t UNetworkGameObject::lastLocalID = 0;

// Sets default values for this component's properties
UNetworkGameObject::UNetworkGameObject()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UNetworkGameObject::BeginPlay()
{
	Super::BeginPlay();
	if (isLocal)
	{
		localID = ++lastLocalID;
		ANetManager::networkObjects.Add(this);

	}
	owner = GetOwner();
	UE_LOG(LogTemp, Warning, TEXT("%s assigned to %i"), *owner->GetName(), localID);

	

	// ...
	
}


// Called every frame
void UNetworkGameObject::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	
	transformPacket.makeTransformPacket(owner);

	// ...
}

