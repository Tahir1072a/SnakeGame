using App;
// TODO: Oluşturulan foodlar yeşil renkte görünmesi lazım.
// BUG: Yılan başlayınca haritanın bir kenarı kayıp oluyor.
// TODO: Game Over olunca bir tuş ile restart yapılanması kurulabilir.
// BUG: Yılan düz giderken bir anda geriye dönüp kendini yemeye kalkıyor bu hareket engellenebilir.

Console.CursorVisible = false;

var message = "Press any key to start the game";

Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop + 5);
Console.WriteLine(message);

Console.ReadKey();
Console.Clear();

SnakeEngine engine = new SnakeEngine();
await engine.Run();
