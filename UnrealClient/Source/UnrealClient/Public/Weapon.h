// Fill out your copyright notice in the Description page of Project Settings.

#pragma once
#include "Camera/CameraComponent.h"
#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "Bullet.h"

#include "Weapon.generated.h"



UCLASS(ClassGroup = (Custom), meta = (BlueprintSpawnableComponent))
class UNREALCLIENT_API AWeapon : public APawn
{
	GENERATED_BODY()
	//AActor* owner;
	APlayerCameraManager* pcm;
	UCameraComponent* cameraComponent;
	UInputComponent* inputComponent;
	AActor* bullet;

public:
	// Sets default values for this pawn's properties
	AWeapon();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	UFUNCTION(BlueprintCallable, Category="Weapon")
	virtual void shoot()
	{
		UE_LOG(LogTemp, Warning, TEXT("Shot Fired"));
	}

	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

};
