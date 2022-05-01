public class DeathState : PersonState
{
	public override void Enter()
	{
		Owner.Animator.SetTrigger(Owner.MovementState.AnimatorPrefix + "_Infantry_Die");
		Owner.Movement.enabled = false;
		Owner.Senses.Dispose();
		Owner.StateMachine.ChangeStrategy(null);
	}

	public override void Update()
	{
		
	}

	public override void Exit()
	{
		
	}
}