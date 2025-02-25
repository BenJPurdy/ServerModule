// Fill out your copyright notice in the Description page of Project Settings.

#include "Kismet/GameplayStatics.h"

#include "GameFramework/PlayerController.h"

#include "Weapon.h"

// Sets default values
AWeapon::AWeapon()
{
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	bullet = UGameplayStatics::GetActorOfClass(GetWorld(), ABullet::StaticClass());
}



// Called when the game starts or when spawned
void AWeapon::BeginPlay()
{
	pcm = UGameplayStatics::GetPlayerCameraManager(GetWorld(), 0);
	Super::BeginPlay();

	AActor* parent = GetAttachParentActor();
	if (parent)
	{
		cameraComponent = parent->FindComponentByClass<UCameraComponent>();
		inputComponent = parent->FindComponentByClass<UInputComponent>();
		if (!inputComponent)
		{
			UE_LOG(LogTemp, Warning, TEXT("failed to capture input controller"));
		}
	}
	
}

// Called every frame
void AWeapon::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	if (cameraComponent)
	{
		FRotator cameraRotation = cameraComponent->GetComponentRotation();
		FRotator thisRotator = GetActorRotation();
		thisRotator.Pitch = cameraRotation.Pitch;
		thisRotator.Roll = 0.0f;

		SetActorRotation(thisRotator);
	}
	
	
	//SetActorRotation()

}



// Called to bind functionality to input
void AWeapon::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	PlayerInputComponent->BindAction("Shoot", IE_Pressed, this, &AWeapon::shoot);
}

