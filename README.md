# Conhics
Conhics is a library created to make high-frequency updates to the console without delay. Instead of using `Console.Writeline` Conhics directly accessees the console buffer and thereby enables for extremely high-speed updates. This allows for creating real time games in the console without lag and/or delay.
## Content
* [Library methods](#library-methods)
  * [Screen.Setup()](#screensetup)
  * [Screen.Print()](#screenprint)
  * [Screen.Flush()](#screenflush)
  * [Screen.Clear()](#screenclear)
  * [Screen.GetPos()](#screengetpos)
  * [Screen.Input()](#screeninput)
* [Examples](#examples)
  * [Setup](#setup)
  * [Snake](#snake-game)

## Changelog
* Conhics has now renamed the window class to screen
* New event system introduced
* Window.GetLastKey() has been removed in favor of the new event system

## Library methods
### Screen.Setup()
```csharp
void Screen.Setup(string title,  
    int consoleWidth = 120, int consoleHeight = 30,   
    short charWidth = 8, short charHeight = 16, 
    bool activeEventCapture = true
);
```
The purpose of the setup command is to retrieve handles from the windows API which will later make it possible to directly manage the buffer.
The setup has to be called before all other Conhcis commands as the library depends on this method. Most of the parameters of the method are pretty self-explanatory, `title` is the title of the console window, `consoleWidth` and `consoleHeight` are the dimensions of the console in columns and rows (not pixels). The attributes `charWidth` and `charHeight` define the size of each character displayed in the console, the smaller each character is the more unreadable it becomes however it allows for much more variety it gives more elements to work with. The attribute `activeEventCapture` decides whether or not Conhics should listen for keystrokes on a separate thread. This will allow the user of the library to fetch the last hit key.   
**Note: Conhics overrides the default Console.ReadKey() which means that unexpected behavior will arise if this method is called, this is the reason that complementary methods are provided via `activeEventCapture`**

### Screen.Print()
```csharp
void Screen.Print(char character, int x, int y, ConsoleColor color = ConsoleColor.White);
void Screen.Print(string text, int x, int y, ConsoleColor color = ConsoleColor.White);
```
The print command is what is used to make changes to the console. Print either takes a character or a text and adds it to the entered coordinate **(Note that the windows console has (0,0) in the top left corner)**. It is also possible to color each individual print with the windows object `ConsoleColor`. However what is important to note is that Print does NOT render anything to the screen, instead everything is cached in memory until the `Screen.Flush()` command is called.

### Screen.Flush()
```csharp
void Screen.Flush();
```
Flush gathers everything that has been inputted with `Screen.Print()` and renders it to the screen. Flush is therefore needed to get results showing.

### Screen.Clear()
```csharp
void Screen.Clear();
```
Clear cleans the internal cache. Clear does NOT render the screen blank. To clear the console it is therefore necessary to call `Screen.Clear()` followed by `Screen.Flush()` to render the changes.

### Screen.GetPos()
```csharp
char Screen.GetPos(int x, int y);
```
Retrieves the character present at the targeted location on the screen.

### Screen.GetLastKey()
```csharp
ConsoleKeyInfo? Screen.GetLastKey(bool autoClear = true);
```
GetLastKey returns the last key the user hit when called. If `autoClear` is true the key is popped from the stack when fetched, otherwise it will stay until another key overrides it. If there are no keys hit since last time called the method returns null.

### Screen.Input()
```csharp
string Screen.Input(string displayText, int x, int y, bool enforceInput);
```
The input method is very similar to the normal `Console.ReadLine` but with some extras attached to it. First the parameter `displayText` is used to present text before the actual input such as `Name:`. The method then requires a position to start the input from which is decided with the `x` and `y`. Last the argument `enforceInput` is used to prevent users from returning empty strings. If this setting is true the method requires the user to input at least one character before it returns.

## Examples
### Setup
```csharp
static void Main(string[] args) {
    Screen.Setup("MyWindow");
    Screen.Print('X', 5, 5, ConsoleColor.Cyan);
    Screen.Flush();
}
```
### Snake game
```csharp
using Conhics;
using Conhics.Input;

struct Position {
    public int x;
    public int y;
}

enum Direction {
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
}

static void Main(string[] args) {

    Direction direction = Direction.UP;

    // Setup Conhics
    Screen.Setup("Snake game");
    
    // Take the name of the user
    string username = Screen.Input("Name", 10, 10, true);
    
    // Start position
    int x = 0;
    int y = 4;

    List<Position> snake = new List<Position>();

    // Gameloop
    while (true) {
    
        // Clear the screen after every iteration
        Screen.Clear();
        
        // Print out the snake
        for (int i = 0; i < snake.Count; i++) {
            Screen.Print('█', snake[i].x * 2, snake[i].y, ConsoleColor.Green);
            Screen.Print('█', snake[i].x * 2 + 1, snake[i].y);
        }
        // Render the screen
        Screen.Flush();
        Thread.Sleep(50);

        // Ask conhics for the last hit key
        var key = Screen.GetLastKey();

        if (key != null) {
            // Change direction
            switch (key.Value.Key) {
                case ConsoleKey.UpArrow:
                    if (direction == Direction.DOWN) break;
                    direction = Direction.UP;
                    break;
                case ConsoleKey.RightArrow:
                    if (direction == Direction.LEFT) break;
                    direction = Direction.RIGHT;
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction == Direction.RIGHT) break;
                    direction = Direction.LEFT;
                    break;
                case ConsoleKey.DownArrow:
                    if (direction == Direction.UP) break;
                    direction = Direction.DOWN;
                    break;
            }
        }

        // Move
        switch (direction) {
            case Direction.UP:
                if (y == 0)
                    y = WinBuf.BufferHeight - 2;
                else
                    y--;
                break;
            case Direction.RIGHT:
                if (x == WinBuf.BufferWidth / 2 - 1)
                    x = 0;
                else
                    x++;
                break;
            case Direction.DOWN:
                if (y == WinBuf.BufferHeight - 1)
                    y = 0;
                else
                    y++;
                break;
            case Direction.LEFT:
                if (x == 0)
                    x = WinBuf.BufferWidth / 2 - 1;
                else
                    x--;
                break;
        }

        snake.Add(new Position { x = x, y = y });
        if (snake.Count > 10)
            snake.RemoveAt(0);
    }
}
```
