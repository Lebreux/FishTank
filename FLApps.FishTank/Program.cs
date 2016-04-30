using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FLApps.FishTank
{
    class Program
    {
        private static Random _rand = new Random();
        private static List<Fish> fishies = new List<Fish>();
        private static int FoodSize = 5;
        private static int framerate = 20;
        static void Main(string[] args)
        {
            int width = 1920;
            int height = 1080;

            SFML.Graphics.RenderWindow w = new SFML.Graphics.RenderWindow(
                new SFML.Window.VideoMode((uint)width, (uint)height),
                System.Reflection.Assembly.GetExecutingAssembly().FullName,
                SFML.Window.Styles.Default,
                new SFML.Window.ContextSettings() { AntialiasingLevel = 8 });
            w.Resized += new EventHandler<SFML.Window.SizeEventArgs>(w_Resized);
            w.Closed += new EventHandler(w_Closed);
            w.MouseButtonPressed += new EventHandler<SFML.Window.MouseButtonEventArgs>(w_MouseButtonPressed);
            w.MouseMoved += new EventHandler<SFML.Window.MouseMoveEventArgs>(w_MouseMoved);
            w.MouseButtonReleased += new EventHandler<SFML.Window.MouseButtonEventArgs>(w_MouseButtonReleased);
            w.MouseWheelMoved += W_MouseWheelMoved;

            SFML.Graphics.Font font = new SFML.Graphics.Font("fonts/FontAwesome.otf");

            List<SFML.Window.Vector2f> foods = new List<SFML.Window.Vector2f>();
            int tick = 0;

            while (w.IsOpen())
            {
                while (foods.Count < 25)
                    foods.Add(new SFML.Window.Vector2f(_rand.Next(0, width), _rand.Next(0, height)));
                while (fishies.Count < 10)
                {
                    Fish fish = new FishSpikingNN(_rand.Next(0, width), _rand.Next(0, height))
                    {
                        Food = 100000,
                        Angle = _rand.Next(0, 360)
                    };
                    fishies.Add(fish);
                }

                for (int i = 0; i < fishies.Count;)
                {
                    Fish fish = fishies[i];

                    Fish closestFish = null;
                    double closestDist = double.MaxValue;
                    foreach (var fishOther in fishies)
                    {
                        if (fish != fishOther)
                        {
                            double dist = Utils.Distance(fish.X, fish.Y, fishOther.X, fishOther.Y);
                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                closestFish = fishOther;
                            }
                        }
                    }
                    if (closestFish != null)
                    {
                        fish.SetEye(closestFish.GetLightColor());
                        fish.SetProximity((int)closestDist);
                    }
                    else
                    {
                        fish.SetEye(Color.Black);
                        fish.SetProximity(int.MaxValue);
                    }

                    SFML.Window.Vector2f? foodEaten = null;
                    SFML.Window.Vector2f? foodSmelled = null;
                    int minFoodDist = int.MaxValue;
                    foreach (var food in foods)
                    {
                        double distWithFood = Utils.Distance(food.X, food.Y, fish.X, fish.Y);
                        if (distWithFood < minFoodDist)
                        {
                            minFoodDist = (int)distWithFood;
                            foodSmelled = food;
                        }
                        if (distWithFood < Fish.FishSize + FoodSize + Fish.FishThickness)
                        {
                            foodEaten = food;
                            break;
                        }
                    }
                    if (foodEaten.HasValue)
                    {
                        fish.Food += 1000;
                        foods.Remove(foodEaten.Value);
                    }
                    fish.SetFood(fish.Food - 1);
                    fish.SetSmell(minFoodDist);
                    if (fish.Food >= Fish.ReproductionFoodPercent * Fish.FishMaxFood)
                    {
                        fish.Food -= (int)(Fish.ReproductionFoodUsage * Fish.FishMaxFood);
                        Fish child = new FishSpikingNN(_rand.Next(0, width), _rand.Next(0, height))
                        {
                            Food = 100000,
                            Angle = _rand.Next(0, 360)
                        };
                        child.SetGeneticCode(NeuralNetwork.SpikingNetwork.MutateGeneticCode(fish.GetGeneticCode(), .1d));
                        fishies.Add(child);
                    }

                    if (fish.Food == 0)
                        fishies.Remove(fish);
                    else
                        i++;
                }

                foreach (var fish in fishies)
                {
                    fish.Tick();
                    fish.X = Math.Min(Math.Max(0d, fish.X), width);
                    fish.Y = Math.Min(Math.Max(0, fish.Y), height);
                }

                if (framerate == 0 || tick % framerate == 0)
                {
                    w.DispatchEvents();
                    w.Clear(new SFML.Graphics.Color(Color.CornflowerBlue.R, Color.CornflowerBlue.G, Color.CornflowerBlue.B));
                    w.Draw(new SFML.Graphics.Text(framerate.ToString(), font) { Position = new SFML.Window.Vector2f(25f, 25f) });
                    foreach (var fish in fishies)
                        foreach (var drawable in fish.Render())
                            w.Draw(drawable);
                    foreach (var food in foods)
                    {
                        w.Draw(new SFML.Graphics.CircleShape(FoodSize)
                        {
                            FillColor = SFML.Graphics.Color.Green,
                            Origin = new SFML.Window.Vector2f(FoodSize / 2f, FoodSize / 2f),
                            Position = food
                        });
                    }
                    w.Draw(new SFML.Graphics.RectangleShape(new SFML.Window.Vector2f(width, height))
                    {
                        FillColor = SFML.Graphics.Color.Transparent,
                        OutlineThickness = 5f,
                        OutlineColor = SFML.Graphics.Color.Black
                    });
                    w.Display();
                }

                System.Threading.Thread.Sleep(0);
                tick++;
            }
        }

        private static void W_MouseWheelMoved(object sender, SFML.Window.MouseWheelEventArgs e)
        {
            framerate += e.Delta;
            framerate = Math.Max(0, framerate);
        }

        private static SFML.Window.Vector2f? _point;

        static void w_MouseMoved(object sender, SFML.Window.MouseMoveEventArgs e)
        {
            if (_point.HasValue)
            {
                SFML.Window.Vector2f newPoint = new SFML.Window.Vector2f(e.X, e.Y);
                SFML.Window.Vector2f delta = _point.Value - newPoint;
                SFML.Graphics.RenderWindow w = (SFML.Graphics.RenderWindow)sender;
                w.SetView(new SFML.Graphics.View(w.GetView().Center + delta, w.GetView().Size));
                _point = newPoint;
            }
        }

        static void w_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            _point = new SFML.Window.Vector2f(e.X, e.Y);
        }

        static void w_MouseButtonReleased(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            _point = null;
        }

        static void w_Closed(object sender, EventArgs e)
        {
            ((SFML.Graphics.RenderWindow)sender).Close();
        }

        static void w_Resized(object sender, SFML.Window.SizeEventArgs e)
        {
            ((SFML.Graphics.RenderWindow)sender).SetView(new SFML.Graphics.View(new SFML.Graphics.FloatRect(0, 0, e.Width, e.Height)));
        }
    }
}
