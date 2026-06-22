using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

public partial class Wheel : Area2D
{
	public float CurrentGrip = 1f;
	
	public float Durability = 1f;
	public float CurrentGripMultiplier;

	protected TyreWear CurrentTyreWear;

	//Stats
	protected float TarmacGripMultiplier = 0.9f;
	protected float DirtGripMultiplier = 0.7f;
	protected TyreWear TarmacTyreWear = new TyreWear(0.001f, 0.0025f);
	protected TyreWear DirtTyreWear = new TyreWear(0.005f, 0.01f);



	//Nodes
	protected Vehicle3 Vehicle;
	public float VehicleMaxSpeed;
	
	


	public override void _Ready()
	{
		//Getting nodes
		Vehicle = GetParent<Node2D>().GetParent<Vehicle3>();
		//Load stats
		
		//Reset
		CurrentGripMultiplier = TarmacGripMultiplier;
		CurrentTyreWear = TarmacTyreWear;
	}


	private const float TyreWearForwardVelocityMultiplier = 0.0025f;
	private const float TyreWearRightVelocityMultiplier = 0.005f;
    public override void _PhysicsProcess(double delta)
	{
		Durability -= CurrentTyreWear.GetTyreWear() * Mathf.Abs(Vehicle.ForwardVelocity) * TyreWearForwardVelocityMultiplier * (1f + Mathf.Abs(Rotation)) * (float)delta;//Aggiungere altro nella formula
		Durability -= CurrentTyreWear.GetTyreWear() * Mathf.Abs(Vehicle.RightVelocity) * TyreWearRightVelocityMultiplier * (float)delta;
		//Durability -= (float)GD.RandRange(CurrentTyreWear.Min, CurrentTyreWear.Max) * Mathf.Abs(Vehicle.Velocity.Dot(Vector2.FromAngle(Rotation))) * (float)delta;
		CurrentGrip = Durability * CurrentGripMultiplier;
	}


	public void OnDirtEntered(Node2D body)
	{
		CurrentGripMultiplier = DirtGripMultiplier;
		CurrentTyreWear = DirtTyreWear;
	}
	public void OnDirtExited(Node2D body)
	{
		CurrentGripMultiplier = TarmacGripMultiplier;
		CurrentTyreWear = TarmacTyreWear;
	}

}




public struct TyreWear
{
	public float Min;
	public float Max;

	public TyreWear(float min, float max)
	{
		Min = min;
		Max = max;
	}

	public float GetTyreWear() => (float)GD.RandRange(Min, Max);
}


