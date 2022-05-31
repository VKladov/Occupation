using System.Threading;
using Buildings;
using Cysharp.Threading.Tasks;

public class HomeShopLoop : PersonStrategy
{
	public HomeShopLoop(Person owner) : base(owner)
	{
	}
	
	public override void Start()
	{
		Owner.SetMainObjective(new BuildingObjective(Owner.City.GetNearestSupermarket(Owner.transform.position)));
	}

	public override async void OnReachTargetPoint()
	{
		if (Owner.MainTarget is BuildingObjective buildingObjective)
		{
			if (await Delay(2f))
			{
				return;
			}
			
			if (buildingObjective.Target is Supermarket)
			{
				Owner.SetMainObjective(new BuildingObjective(Owner.House));
			}
			else if (buildingObjective.Target is House)
			{
				Owner.SetMainObjective(new BuildingObjective(Owner.City.GetNearestSupermarket(Owner.transform.position)));
			}
		}
		else
		{
			Owner.SetMainObjective(new BuildingObjective(Owner.House));
		}
	}

	public override void Unsubscribe()
	{
		
	}
}