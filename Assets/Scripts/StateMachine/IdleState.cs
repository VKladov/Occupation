public class IdleState : PersonState
{
	public override void Enter()
	{
		Owner.Movement.Stop();
		Owner.Animator.SetBool("Aim", false);
	}

	public override void Update()
	{
		
	}

	public override void Exit()
	{
		
	}
}