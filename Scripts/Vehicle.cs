using Godot;
using System;
//using System.Numerics;
//using System.Xml.Schema;

public partial class Vehicle : CharacterBody2D
{

	//Stats
	private float acceleration = 10f;
	private float maxSpeed = 100f;
	private float maxReverseSpeed = 25f;
	private float brakingPower = 15f;
	private float steeringPower = 0.05f;
	private float weight = 10f;


	//Constants
	private const float maximumReverseSpeed = 1f;





    public override void _Ready()
    {
        
    }


	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		Vector2 direction = Vector2.FromAngle(Rotation - Mathf.DegToRad(90));
		Vector2 InputDirection = Input.GetVector("left", "right", "brake", "accelerator");


		float speedAccelerationReduction = (maxSpeed - velocity.Length()) / maxSpeed;


		if (InputDirection.Y > 0)
		{
			//Accelerating
			speedAccelerationReduction = (maxSpeed - velocity.Length()) / maxSpeed;

			velocity += InputDirection.Y * acceleration * direction * speedAccelerationReduction * (float)delta;
		}
		else if (InputDirection.Y < 0)
		{
			if (velocity > maximumReverseSpeed * direction)
			{
				//Braking
				velocity += InputDirection.Y * brakingPower * -direction * (float)delta;
			}
			else
			{
				//Reverse
				speedAccelerationReduction = (maxReverseSpeed - velocity.Length()) / maxReverseSpeed;
				velocity += InputDirection.Y * acceleration * -direction * speedAccelerationReduction * (float)delta;
			}
		}

		velocity = velocity.MoveToward(Vector2.Zero, weight * (1f - speedAccelerationReduction) * (float)delta);
		
		//Steering
		if (InputDirection.X != 0)
		{
			
			Rotate(InputDirection.X * steeringPower * speedAccelerationReduction);
		}
		


		

		Velocity = velocity;
		MoveAndSlide();
	}
}
