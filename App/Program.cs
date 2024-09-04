using App;

Console.CursorVisible = false;

var message = "Press any key to start the game";

Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop + 5);
Console.WriteLine(message);

Console.ReadKey();
Console.Clear();

SnakeEngine engine = new SnakeEngine();
await engine.Run();
