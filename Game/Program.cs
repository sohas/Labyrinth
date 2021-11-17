using System;
using static Game.Direction;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = Environment.CurrentDirectory + "\\Maps\\";
            var map2name = "maze01.txt";

            var map2 = new Map(directory + map2name);
            var player2 = new Player();
            map2.TakePlayer(player2, 1, 1);

            //var walk = new Walk(map2);
            //walk.Move();
            var explore = new Explore(map2);
            explore.Move();
        }
    }
}
