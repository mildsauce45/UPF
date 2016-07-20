using System.Collections.Generic;

namespace FirstWave.UPF.Test
{
	public class TestViewViewModel
	{
		public IList<Enemy> Enemies { get; private set; }

		public TestViewViewModel()
		{
			Enemies = new List<Enemy>();
			Enemies.Add(new Enemy());
			Enemies.Add(new Enemy());
			Enemies.Add(new Enemy());
		}
	}

	public class Enemy
	{
		public string Sprite
		{
			get { return "Images/KefkaAngel"; }
        }	
	}
}
