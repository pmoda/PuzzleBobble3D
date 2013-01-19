Patrick Modafferi
10/26/2012

---------------------------------------------------------
Starting the game 
---------------------------------------------------------

1) Go to the A3 folder
2) Open PuzzleBobbleA3.sln with visual studio 2010
3) Press F5 to compile and run in debug mode

if you do not have visual studio

1) go to PuzzleBobble3D\PuzzleBobbleA2\bin\x86\Debug
2) Double click on PuzzleBobbleA2.exe

---------------------------------------------------------
Controls
---------------------------------------------------------

SPACE - Start Game, Next
LEFT ARROW - Aim Left
RIGHT ARROW - Aim Right
UP ARROW - Fire 
W/S - Pane Up and Down
A/D - Pane Left and Right
1/3 - Toggle 1st and 2nd person view

To Change Difficulty Level (in the code)
-----------------------------------------
Currently, the difficulty is set to easy so that even a begginer can 
progress through the entire game without too much difficulty.
You can change the difficulty on Line 19 of Game1.cs
DIFFICULTY_RATING = 7;
1 is the hardest at 10 would be extreemly easy

-------------------------------------------------------------
Desing Notes
-------------------------------------------------------------
Physics - 
Gravity
		Any ball not stuck to the top and moving along the 45 degree slope
	in any direction will be subject to a downward constant acceleration.
	This will cause dropping balls to roll faster and faster and shooting
	balls to slow down or even curve under the force of gravity.
Rolling
		The Ball rolling speeds are driven by how fast the ball is actually
	moving. Specifically, agular velocity = velocity / radius. The heading of 
	the ball (direction) is also driven by the velocity's x and y components.
	Heading = Atan(Vx/Vy)
	
Controller - The program has a structure similar to MVC. The Game Controller
	is in charge of passing information to and from the model and View
	The Bubble Model is in charge of storing all information related 
	to each bubble. When something in the View is changed, the controller 
	makes changes to the model. Then in the draw function for Viewing, we
	call upon the Model as update its infor notably position and state on
	the screen.

Storing the Bubbles - In the controller, the int grid[,] is a 2D array in charge 
	of modeling the board where all the bubbles lie. 

Color Generating - The odds of getting each color ball changes as the game goes
	on so as to havea  playable enjoyable experience. Specifically, the odds
	of color X comming out equal to the percentage X colored bubbles on the
	board

Popping - For popping bubbles, a recursive algorithm is used to check all adjacent
	Bubbles which have the same color as the bubble that was just added to the
	grid. 

Cascading - To check if a bubble must fall off the board, the program recursively
	looks for a path to the top where y == 0 using a Depth First Search.

_______________________________________________________
*******************************************************
DISCALIMER
*******************************************************
_______________________________________________________
I do not own copyrights to any picture, model or music used in this project
I do not take credit for the creation of any sprite, picture or music
This project was meant for educational purposes only and is not intended
to be sold or distrubuted wihtout consent

Patrick Modafferi 
10/26/2012