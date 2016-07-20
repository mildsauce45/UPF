﻿using FirstWave.Unity.Gui.Data;
using System.Collections.Generic;

namespace FirstWave.UPF.Test
{
	public class TestViewViewModel : INotifyPropertyChanged
	{
		private string message;

		public event PropertyChangedEventHandler PropertyChanged;

		public IList<Enemy> Enemies { get; private set; }
		public IList<PartyMember> Party { get; private set; }

		public string Message
		{
			get { return message; }
			set
			{
				message = value;
				SafeRaisePropertyChanged("Message");
			}
		}

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

			message = "You have encountered a bunch of angels.";
		}

		private void SafeRaisePropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}

	public class Enemy
	{
		public string Sprite
		{
			get { return "Images/KefkaAngel"; }
		}
	}

	public class PartyMember
	{
		public string Name { get; set; }
		public string HP { get; set; }
	}
}
