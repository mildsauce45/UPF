using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Data;
using System;
using System.Collections.Generic;

namespace FirstWave.UPF.Test
{
	public class TestViewViewModel : ViewModelBase
	{
		private string message;
		private TextDisplayTimer messageTimer;

		public IList<Enemy> Enemies { get; private set; }
		public IList<PartyMember> Party { get; private set; }

		public string Message
		{
			get { return message; }
			set
			{
				if (value != message)
				{
					message = value;
					NotifyPropertyChange(() => Message);
				}
			}
		}

		public InnerVM InnerVM { get; private set; }

		public TestViewViewModel()
		{
			Enemies = new List<Enemy>();
			Enemies.Add(new Enemy());
			Enemies.Add(new Enemy());
			Enemies.Add(new Enemy());

			Party = new List<PartyMember>();
			Party.Add(new PartyMember { Name = "Cloud", HP = "10" });
			Party.Add(new PartyMember { Name = "Sephiroth", HP = "999" });
			Party.Add(new PartyMember { Name = "Drizzt", HP = "∞" });
			Party.Add(new PartyMember { Name = "Aeris", HP = "1" });

			messageTimer = new TextDisplayTimer(0.025f, "You have encountered a bunch of angels.");
			messageTimer.Start();

			InnerVM = new InnerVM();
		}

		public override void Update()
		{
			messageTimer.Update();

			Message = messageTimer.Text;

			base.Update();
		}

		private void AddHP_Clicked(object sender, EventArgs e)
		{
			Party[0].HP = (int.Parse(Party[0].HP) + 1).ToString();
		}
	}

	public class InnerVM
	{
		public float TotalWidth { get; set; }
		public float HalfWidth { get; set; }

		public InnerVM()
		{
			TotalWidth = 400;
			HalfWidth = 200;
		}
	}

	public class Enemy
	{
		public string Sprite
		{
			get { return "Images/KefkaAngel"; }
		}
	}

	public class PartyMember : NotifyableObject
	{
		private string hp;

		public string Name { get; set; }

		public string HP
		{
			get { return hp; }
			set
			{
				hp = value;
				NotifyPropertyChange(() => HP);
			}
		}		
	}
}
