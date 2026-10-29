/*
	NOTE FROM JACOB SEADORF -- PLEASE READ!
	I figure that a small little role playing game would be doable. Nothing like a roguelike, but a simple text adventure.
	(This is something that Jacob Powell and I had discussed over a zoom call)
	
	Maybe the player could still encounter monsters and enemies? 
	If it's not needed, health could be removed, but I added it just in case we end up using it.
 */

using System;
using static System.Console;

class Player {
  public Player() {
		this.health = health;
		this.mana = mana;
		this.name = name;
	}

	public int health { get; set; }
	public int mana { get; set; }
	public string name { get; set; }

	public bool isPlayerDead { get; set; }

	private int _health;
	private int _mana;
	private string _name;

	private bool _playerDead;

}


class Program { 

	static void mainMenu() {
		WriteLine("Main Menu");
		WriteLine("============");
		WriteLine("1) Play");
		WriteLine("2) Credits");
		WriteLine("3) Quit");
	   int menuChoice = Convert.ToInt32(ReadLine());
		switch(menuChoice) {
			case 1:
				Console.Clear(); //clear the screen before the game starts. Makes it so that once the game starts, the menu doesn't show up.
				//TODO: everything under case 1 should probably be its own function, but this works for right now.
				Player player = new Player();
				player.isPlayerDead = false;
				while(player.isPlayerDead == false) {
					//main game loop here; choices are made, the player wins or loses.
					WriteLine("So much action-packed gameplay here.");
					Environment.Exit(0);
				}
				break;
			case 2:
					Console.Clear();
					WriteLine("\tThis game was created for a final project at GRCC.");
					WriteLine("\t==================================================");
					WriteLine("\t\tThe team members for this project were:");
					WriteLine("\t\t* Jacob Seadorf");
					WriteLine("\t\t* Jacob Powell");
					WriteLine("\t\t* Brandon Choang");
					Environment.Exit(0); //exits the program for simplicity so I don't have to use a GOTO.
				break;
			case 3:
					Environment.Exit(0); //zero is used to mean "nothing went wrong"; exiting with 1 or another number usually means an error. 
				break;
			default:
					WriteLine("Error: Invalid Menu Option");
					mainMenu();
				break;
		}
	}

	static void Main() {
		mainMenu();
	}
}
