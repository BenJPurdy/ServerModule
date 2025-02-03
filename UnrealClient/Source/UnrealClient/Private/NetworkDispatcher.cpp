// Fill out your copyright notice in the Description page of Project Settings.


#include <array>
#include "Kismet/GameplayStatics.h"
#include "NetManager.h"

#include "NetworkDispatcher.h"


// Sets default values for this component's properties
UNetworkDispatcher::UNetworkDispatcher()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;
	manager = UGameplayStatics::GetActorOfClass(GetWorld(), ANetManager::StaticClass());
	if (manager != nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("Found actor"));
	}
	
	// ...
}


// Called when the game starts
void UNetworkDispatcher::BeginPlay()
{
	Super::BeginPlay();

	// ...
	
}


// Called every frame
void UNetworkDispatcher::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	
	// ...
}

