namespace RevolverRoulette;

public partial class Player
{
	private TimeSince timeSinceRagdolled = 0f;

	[ClientRpc]
	private void BecomeRagdollOnClient( Vector3 force, int forceBone )
	{
		var corpse = new ModelEntity
		{
			Position = Position,
			Rotation = Rotation,
			Model = Model,
			PhysicsEnabled = true,
			UsePhysicsCollision = true,
		};

		corpse.Tags.Add( "debris" );

		corpse.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		corpse.CopyBonesFrom( this );
		corpse.SetRagdollVelocityFrom( this );
		corpse.CopyBodyGroups( this );
		corpse.DeleteAsync( 20.0f );

		// Copy the clothes over
		foreach ( var child in Children.OfType<ModelEntity>() )
		{
			if ( !child.Tags.Has( "clothes" ) )
				continue;

			var clothing = new ModelEntity
			{
				Model = child.Model,
				Position = child.Position,
			};

			clothing.TakeDecalsFrom( child );
			clothing.CopyBodyGroups( child );
			clothing.CopyMaterialGroup( child );

			clothing.SetParent( corpse, true );
		}

		corpse.PhysicsGroup?.AddVelocity( force * 2f );

		if ( forceBone >= 0 )
		{
			var bone = corpse.GetBonePhysicsBody( forceBone );
			bone?.ApplyForce( force * 1000f );
		}
		//? Currently the sound isn't the same for different clients.
		//? Will fix once it's possible to influence which subsound is played.
		corpse.PlaySound( "ragdoll.goofy" );

		Corpse = corpse;

		timeSinceRagdolled = 0f;
	}

	[ClientRpc]
	private void TryBeginSpectating( Vector3 pos, bool forced = false )
	{
		if ( forced || LifeState != LifeState.Dead || timeSinceRagdolled < 3f || CameraMode is FreeFlyCamera ) return;

		CameraMode = new FreeFlyCamera( pos );
	}
}