namespace RevolverRoulette;

internal partial class Revolver : BaseWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/rust_pistol/rust_pistol.vmdl" );

	public override bool CanReload() => false;
	public override bool CanSecondaryAttack() => false;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	public override bool CanPrimaryAttack()
	{
		return Input.Pressed( InputButton.PrimaryAttack )
			&& Owner.IsValid()
			&& Owner.LifeState == LifeState.Alive;
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParameter( "fire", true );
	}

	private void DryFire()
	{
		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );
	}

	public override void AttackPrimary()
	{
		var plr = Owner as Player; // CanPrimaryAttack ensured this is valid

		if ( !plr.TakeBullet() )
		{
			DryFire();
			return;
		}

		var forward = plr.EyeRotation.Forward;

		foreach ( var tr in TraceBullet( plr.EyePosition, plr.EyePosition + forward * 5000f, 2f ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 500f, tr.Entity.Health * 2f )
				.UsingTraceResult( tr )
				.WithAttacker( plr )
				.WithWeapon( this );

			tr.Entity.TakeDamage( damageInfo );
		}

		PlaySound("revolver.fire");
		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );

		ShootEffects();
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)CitizenAnimationHelper.HoldTypes.Pistol );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
		anim.SetAnimParameter( "holdtype_handedness", (int)CitizenAnimationHelper.Hand.Right );
	}
}
