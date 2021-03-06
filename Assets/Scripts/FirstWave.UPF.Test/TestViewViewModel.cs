﻿using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Data;
using FirstWave.Unity.Gui.Bridge;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstWave.UPF.Test
{
    public class TestViewViewModel : ViewModelBase
    {
        private string message;
        private bool showAbilities;
        private TextDisplayTimer messageTimer;

        public IList<Enemy> Enemies { get; private set; }
        public IList<PartyMember> Party { get; private set; }
        public IList<Ability> Abilities { get; private set; }

        public ICommand FooCommand
        {
            get { return new ExecutableCommand(PrintContext); }
        }

        public ICommand FakeCommand
        {
            get { return new ExecutableCommand(PrintAbility); }
        }

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

        public bool ShowAbilities
        {
            get { return showAbilities; }
            set
            {
                showAbilities = value;
                NotifyPropertyChange(() => ShowAbilities);
            }
        }

		public InnerVM InnerVM { get; private set; }
        public string TextBoxText { get; set; }

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

            Abilities = new List<Ability>();
            Abilities.Add(new Ability { Name = "Fireball" });
            Abilities.Add(new Ability { Name = "Green Fireball" });
            Abilities.Add(new Ability { Name = "Kill" });

			messageTimer = new TextDisplayTimer(0.025f, "You have encountered a bunch of angels.");
			messageTimer.Start();

            ShowAbilities = true;

			InnerVM = new InnerVM();
            InnerVM.TextBoxText = "Test2";

            TextBoxText = "Test";
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

            ShowAbilities = !ShowAbilities;
		}

        private void PrintContext(object context)
        {
            Debug.Log(context);
        }

        private void PrintAbility(object ability)
        {
            var a = ability as Ability;
            Debug.Log(a.Name);
        }
	}

	public class InnerVM
	{
        private string tbt;

		public float TotalWidth { get; set; }
		public float HalfWidth { get; set; }

        public string TextBoxText
        {
            get { return tbt; }
            set { tbt = value; Debug.Log("new value: " + value); }
        }

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

    public class Ability
    {
        public string Name { get; set; }
    }
}
