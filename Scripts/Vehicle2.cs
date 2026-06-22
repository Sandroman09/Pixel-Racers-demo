using Godot;
using System;

public partial class Vehicle2 : CharacterBody2D
{

	//Stats
	private float acceleration = 3.5f;
	private float maxSpeed = 300f;
	private float maxReverseSpeed = 25f;
	private float brakingPower = 7.5f;
	private float steeringPower = 0.002f;
	private float weight = 10f;


	//Constants
	private const float maximumReverseSpeed = 5f;
	




    public override void _Ready()
    {
        GetNodes();
    }



	

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Vector2.FromAngle(Rotation - Mathf.DegToRad(90));
		Vector2 InputDirection = Input.GetVector("left", "right", "brake", "accelerator");

		float forwardVelocity = velocity.Dot(direction);
		float backwardsVelocity = velocity.Dot(-direction);

		if (InputDirection.Y != 0)
		{
			if (InputDirection.Y > 0f)
			{
				//Accelerating
				velocity += InputDirection.Y * acceleration * direction * SpeedAccelerationFactor(maxSpeed, forwardVelocity) ;
			}
			else
			{
				if (forwardVelocity > maximumReverseSpeed)
				{
					//braking
					velocity += InputDirection.Y * brakingPower * direction;
				}
				else
				{
					//Reversing
					velocity += InputDirection.Y * acceleration * direction * SpeedAccelerationFactor(maxReverseSpeed, backwardsVelocity);
				}

			}
		}


		if (InputDirection.X != 0)
		{
			Rotate(steeringPower * InputDirection.X * forwardVelocity);
		}


		

		Velocity = velocity;
		MoveAndSlide();


		float SpeedAccelerationFactor(float maxSpeed, float speed) => (maxSpeed - speed) / maxSpeed;
		float SpeedSteeringFactor(float speed)
		{
			return 1f;
		}
		



		label.Text = 
		"Input Axis Y: " + InputDirection.Y
		+ "\nInput Axis X: " + InputDirection.X
		;

	}


	//Nodes
	private Label label;
	private void GetNodes()
	{
		label = GetNode<Label>("UI/Label");
	}
}
