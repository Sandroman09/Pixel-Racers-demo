using Godot;
using System;

public partial class Vehicle3
{
			///		VEHICLE STATS FILE		\\\
	

	//Stats
	protected float Acceleration = 150f;	//150
	protected float MaxSpeed = 350f;	//350
	protected float MaxReverseSpeed = 75f;
	protected float BrakePower = 150f;
	protected float Steering = 0.045f;
	protected float Weight = 7.5f;
	protected float CarGrip = 11f;


	protected void SetStats(
							float acceleration, float maxSpeed, float maxReverseSpeed, float brakePower, float steering, float weight, float carGrip
							)
	{
		Acceleration = acceleration;
		MaxSpeed = maxSpeed;
		MaxReverseSpeed = maxReverseSpeed;
		BrakePower = brakePower;
		Steering = steering;
		Weight = weight;
		CarGrip = carGrip;
	}
	
}
