using UnityEngine;

public class DestroyBlast : Blast
{
	
	public override void OnCollide (Blastable other)
	{
		if (other.destroyable) {
			Destroy (other.gameObject);
		}
	}

}
