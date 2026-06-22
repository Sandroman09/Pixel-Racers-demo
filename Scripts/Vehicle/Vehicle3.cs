using Godot;
using System;
using System.Diagnostics;


public partial class Vehicle3 : CharacterBody2D
{
	public float ForwardVelocity;
	public float RightVelocity;
	public float SpeedPercentage = 0f;		//Used in calculating the wheels wear


	/// Nodes
	protected int WheelsCount;
	protected Node2D WheelsNode;
	protected Wheel[] Wheels;
	protected Wheel[] SteeringWheels;

	protected Label label;		//


	//Gears
	protected int Gear = 0;	//0 = neutral, 1 = drive, -1 = reverse
	private const int maxGear = 1;
	private const int minGear = -1;



	//Constants
	private const float maximumReverseSpeed = 5f;
	private const float wheelsGripAccelerationInpactDiminisher = 2f;
	private const float maxSteeringSpeed = 75f;
	private const float handbrakePowerMultiplier = 1.75f;
	private const float handbrakeFrictionMultiplierValue = 0.5f;
	private const float handbrakeFrictionMultiplierDiminisher = 0.025f;
	private const float handbrakeSteeringMultiplierValue = 2.25f;
	private const float handbrakeSteeringMultiplierDimisher = 0.0625f;
	private const float steeringSensivity = 0.15f;	//
	private const float maxWheelAngle = 25;
	


    public override void _Ready()
    {
        /// Getting nodes
		label = GetNode<Label>("UI/Label");		//
		//Getting wheels
		WheelsNode = GetNode<Node2D>("Wheels");
		Godot.Collections.Array<Node> WheelsGodotArray = WheelsNode.GetChildren();
		WheelsCount = WheelsGodotArray.Count;
		Wheels = new Wheel[WheelsCount];
		for(int i = 0; i < WheelsCount; i++)
		{
			Wheels[i] = WheelsGodotArray[i] as Wheel;
			Wheels[i].VehicleMaxSpeed = MaxSpeed;	//
		}
		SteeringWheels = new Wheel[2];
		for(int i = 0; i < 2; i++)
		{
			SteeringWheels[i] = Wheels[i];
		}
		//
    }



	public Vector2 Direction;
	protected Vector2 InputDirection;
	private float handbrakeFrictionMultiplier = 1f;
	private float handbrakeSteeringMultiplier = 1f;
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;




											  ///		  \\\
											 ///		   \\\
											///				\\\
										   ///				 \\\
										  ///	   			  \\\
										 ///				   \\\
										///		CAR PHYSICS		\\\

		//Calculating wheels grip
		float wheelsGrip = GetWheelsGrip();
		//

		//Gear change
		if (Input.IsActionJustPressed("gearUp") && Gear < maxGear)
			Gear++;
		if (Input.IsActionJustPressed("gearDown") && Gear > minGear)
			Gear--;
		//
		
		//Input
		//Steering sensivity
		//InputDirection.X = Input.GetAxis("left", "right");
		float steeringInputAxis = Input.GetAxis("left", "right");
		InputDirection.X = Mathf.MoveToward(InputDirection.X, steeringInputAxis, steeringSensivity);	//TODO: cambiare la costante con una variabile delle impostazioni


		InputDirection.Y = Input.GetActionStrength("accelerator");
		float brake = Input.GetActionStrength("brake");
		bool handbrake = Input.IsActionPressed("handbrake");


		CalculateVelocities();		//


		if (InputDirection.Y > 0 && Gear != 0)
		{
			//Accelerating or reverse
			if (Gear == 1)
			{
				//Accelerating or braking
				if (ForwardVelocity > -maximumReverseSpeed)
				{
					//velocity += InputDirection.Y * Acceleration * Direction * SpeedAccelerationFactor(MaxSpeed, forwardVelocity) * wheelsGrip * (float)delta;
					CalculateAcceleration(1, MaxSpeed);
				}
				else
					brake = 1f;
			}
			else
			{
				//Reverse or braking
				if (ForwardVelocity < maximumReverseSpeed)
				{
					//velocity += InputDirection.Y * Acceleration * -Direction * SpeedAccelerationFactor(MaxReverseSpeed, -forwardVelocity) * wheelsGrip * (float)delta;
					CalculateAcceleration(-1, MaxReverseSpeed);
				}
				else
					brake = 1f;
			}
			CalculateVelocities();		//
		}

		//braking
		if (handbrake)
		{
			ApplyBrakes(BrakePower * handbrakePowerMultiplier);
			handbrakeFrictionMultiplier = handbrakeFrictionMultiplierValue;
			handbrakeSteeringMultiplier = handbrakeSteeringMultiplierValue;
			if (Mathf.Abs(InputDirection.X) < 0.5f)
				InputDirection.X += (float)GD.RandRange(-0.5f, 0.5f);
			CalculateVelocities();
		}
		else
		{
			float handbrakeMultiplier = GD.Randf();
			if (handbrakeFrictionMultiplier != 1f)
				handbrakeFrictionMultiplier = Mathf.MoveToward(handbrakeFrictionMultiplier, 1f, handbrakeFrictionMultiplierDiminisher * handbrakeMultiplier);
			if (handbrakeSteeringMultiplier != 1f)
				handbrakeSteeringMultiplier = Mathf.MoveToward(handbrakeSteeringMultiplier, 1f, handbrakeSteeringMultiplierDimisher * handbrakeMultiplier);

			if (brake > 0f)
			{
				/*Vector2 brakeDirection = Direction;
				if (ForwardVelocity < 0)
					brakeDirection = -Direction;
				velocity = velocity.MoveToward(velocity - (Mathf.Abs(ForwardVelocity) * brakeDirection), BrakePower * (float)delta);*/
				ApplyBrakes(BrakePower);
				CalculateVelocities();		//
			}
		}
		//

		

		//Steering
		if (InputDirection.X != 0)
		{
			float rotation = 0f;
			if (ForwardVelocity <= maxSteeringSpeed)
				rotation = InputDirection.X * Steering * ForwardVelocity * (float)delta;
			else
				//rotation = InputDirection.X * Steering * Mathf.Lerp(maxSteeringSpeed, 0f, SpeedPercentage) * (float)delta;
				rotation = InputDirection.X * Steering * (maxSteeringSpeed / (ForwardVelocity / maxSteeringSpeed)) * handbrakeSteeringMultiplier * (float)delta;
			Rotate(rotation);
			velocity.Rotated(rotation);

			CalculateVelocities();		//

		}
		else
		{
		}
		//

		//Steering visual input
		foreach(Wheel wheel in SteeringWheels)
			wheel.Rotation = InputDirection.X * Mathf.DegToRad(maxWheelAngle);
		//
		
		//Friction
		if (RightVelocity > 0)
		{
			//velocity -= rightVelocity * Direction.Rotated(Mathf.DegToRad(90)) * (float)delta;
			CalculateFriction(1);
		}
		else
		{
			//velocity -= -rightVelocity * Direction.Rotated(Mathf.DegToRad(-90)) * (float)delta;
			CalculateFriction(-1);
		}
		CalculateVelocities();		//
		velocity = velocity.MoveToward(Vector2.Zero, Weight * (float)delta);

		


		Velocity = velocity;
		MoveAndSlide();



		//Local functions
		void CalculateFriction(int multiplier) => velocity -= (RightVelocity * multiplier) * Direction.Rotated(Mathf.DegToRad(90 * multiplier)) * wheelsGrip * handbrakeFrictionMultiplier * (float)delta * CarGrip;		//Per rimettere la formula in forma normale togliere la moltiplicazione con i multiplier
		void CalculateAcceleration(int multiplier, float maxSpeed) => velocity += InputDirection.Y * Acceleration * (Direction * multiplier) * SpeedAccelerationFactor(maxSpeed, ForwardVelocity * multiplier) * (1f -((1f - wheelsGrip) / wheelsGripAccelerationInpactDiminisher)) * (float)delta;	//Per rimettere la formula in modo normale togliere (Direction * directionMultiplier) e mettere solo Direction
		float SpeedAccelerationFactor(float maxSpeed, float speed) => (maxSpeed - speed) / maxSpeed;
		float GetWheelsGrip()
		{
			float wheelsGrip = 0f;
			for(int i = 0; i < WheelsCount; i++)
			{
				wheelsGrip += Wheels[i].CurrentGrip;
			}
			return wheelsGrip / WheelsCount;
		}
		void CalculateVelocities()
		{
			Direction = Vector2.FromAngle(Rotation - Mathf.DegToRad(90));

			ForwardVelocity = velocity.Dot(Direction);
			RightVelocity = velocity.Dot(Vector2.FromAngle(Rotation));

			SpeedPercentage = Mathf.InverseLerp(0f, MaxSpeed, ForwardVelocity);
		}
		void ApplyBrakes(float brakePower)
		{
			Vector2 brakeDirection = Direction;
			if (ForwardVelocity < 0)
				brakeDirection = -Direction;
			velocity = velocity.MoveToward(velocity - (Mathf.Abs(ForwardVelocity) * brakeDirection), brakePower * (float)delta);
		}

										///					\\\
									   ///					 \\\
									  ///					  \\\


		//Label (to remove)
		label.Text =
		"Gear: " + Gear
		+ "\nSpeed: " + ForwardVelocity
		+ "\nSpeed percentage: " + SpeedPercentage
		+ "\nInput direction: " + InputDirection
		;
		for(int i = 0; i < WheelsCount; i++)
		{
			label.Text += "\nWheel " + (i + 1) + " grip: " + (float)(((int)(Wheels[i].CurrentGrip * 1000f)) / 1000f)
			+ "   Durability: " + (float)(((int)(Wheels[i].Durability * 1000f)) / 1000f)
			+ "   Rotation: " + (float)(((int)(Wheels[i].Rotation * 1000f)) / 1000f)
			;
		}
	}
}
