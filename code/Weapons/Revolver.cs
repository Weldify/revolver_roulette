namespace RevolverRoulette;

internal partial class Revolver : BaseWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/revolver/revolver.vmdl" );
	public static readonly Model ViewModel = Model.Load( "models/revolver/v_revolver.vmdl" );

	public override bool CanReload() => false;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Game.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
	}

	private void DryFire()
	{
		PlaySound( "revolver.dryfire" );
		(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );
		ViewModelEntity?.SetAnimParameter( "fire", true );
	}

	public override bool CanPrimaryAttack()
	{
		return Input.Pressed( InputButton.PrimaryAttack )
		       && Owner.IsValid()
		       && Owner.LifeState == LifeState.Alive;
	}

	public override void AttackPrimary()
	{
		var plr = Owner as Player; // CanPrimaryAttack ensured this is valid

		if ( Prediction.FirstTime )
		{
			if ( !plr.TakeBullet() )
			{
				DryFire();
				return;
			}
			
			var forward = plr.ViewAngles.Forward;
			foreach ( var tr in TraceBullet( plr.AimRay.Position, plr.AimRay.Position + forward * 5000f, 2f ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !Game.IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 500f, tr.Entity.Health * 2f )
					.UsingTraceResult( tr )
					.WithAttacker( plr )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}

			PlaySound( "revolver.fire" );

			(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );
			ViewModelEntity?.SetAnimParameter( "fire", true );

			ShootEffects();
		}
	}

	public override bool CanSecondaryAttack()
	{
		return Input.Pressed( InputButton.SecondaryAttack )
		       && Owner.IsValid()
		       && Owner.LifeState == LifeState.Alive;
	}

	public override void AttackSecondary()
	{
		if ( !Prediction.FirstTime ) return;
		// Feign
		DryFire();
	}

	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		anim.HoldType = CitizenAnimationHelper.HoldTypes.Pistol;
		anim.Handedness = CitizenAnimationHelper.Hand.Right;
		anim.AimBodyWeight = 1.0f;
	}

	public override void CreateViewModel()
	{
		Game.AssertClient();

		ViewModelEntity = new ViewModel
		{
			Position = Position, Owner = Owner, EnableViewmodelRendering = true, Model = ViewModel,
		};
	}
}
