using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using AnnaWebKitchenFin.Data;
using AnnaWebKitchenFin.Data.Enums;
using AnnaWebKitchenFin.Utils;

namespace AnnaWebKitchenFin.Models
{
    public class Cook : IDisposable
    {
        public long Id { get; set; }
        private static readonly ObjectIDGenerator idGenerator = new();

        public Kitchen Kitchen { get; set; }

        public int Rank { get; set; }
        public int Proficiency { get; set; }
        public string Name { get; set; }
        public string CatchPhrase { get; set; }


        private readonly Thread _cookThread;

        public Cook(int rank, int proficiency, string name, string catchphrase, Kitchen kitchen)
        {
            Id = idGenerator.GetId(Id, out _);
            Rank = rank;
            Proficiency = proficiency;
            Name = name;
            CatchPhrase = catchphrase;
            Kitchen = kitchen;


            _cookThread = new Thread(StartCookWork)
            {
                IsBackground = true,
                Name = $"CookWorkThread_{Id}"
            };

            _cookThread.Start();
            LogsWriter.Log($"Cook {name} with rank {rank} and proficiency {proficiency} started their work");
        }



        public static void CreateCooks(List<Cook> cooks, Kitchen kitchen)
        {
            cooks.AddRange(new Cook[]
            {
               new Cook(1, 1, "LineCook", "Is this okay Mr. Ramsay?", kitchen),

               new Cook(2, 3, "Saucier", "You better go get the lamb sauce", kitchen),

               new Cook(3, 3, "Saucier", "You better go get the lamb sauce", kitchen),

               new Cook(4, 4, "Chef", "Where is the lamb sauce? The chicken is RAW", kitchen)

            });

            cooks.Sort((cook1, cook2) => cook1.Proficiency - cook2.Proficiency);
        }

        public void StartCookWork()
        {
            while (true)
            {
                for (int i = 0; i < Proficiency; i++)
                {
                    try
                    {
                        while (true)
                        {
                            foreach (var order in Kitchen.Orders)
                            {
                                if (order.IsPrepared)
                                {
                                    LogsWriter.Log($"Order number {order.Id} is done, send it to restaurant");

                                    switch (Proficiency)
                                    {
                                        case 1:
                                            LogsWriter.Log($"Prepared by line cook, proficiency {Proficiency}");
                                            break;
                                        case 2:
                                            LogsWriter.Log($"Prepared by saucier, proficiency {Proficiency}");
                                            break;
                                        case 3:
                                            LogsWriter.Log($"Prepared by chef, ");
                                            break;
                                        default: break;
                                    }

                                    LogsWriter.Log($"{CatchPhrase}");
                                    Kitchen.server.ReturnDoneOrder(order);
                                }

                                else if (!order.IsPrepared)
                                {
                                    foreach (var food in order.ExistingItems)
                                    {
                                        if (food.Comlexity <= Rank && (food.State == KitchenFoodState.Undone && TryGetApparatus(Kitchen, food, out CookingApparatus apparatus)))
                                        {
                                            if (apparatus == null)
                                            {
                                                food.State = KitchenFoodState.Ready;

                                                continue;
                                            }

                                            apparatus.Busy = true;
                                            LogsWriter.Log($"Cook is preparing {food.Name}");

                                            food.State = KitchenFoodState.Preparing;
                                            Prepare(food, apparatus);

                                        }
                                    }
                                }
                            }
                        }
                    }

                    catch
                    {

                    }
                }
            }
        }


        public void Prepare(Food food, CookingApparatus apparatus)
        {
            LogsWriter.Log($"Cook {Id} started preparing {food.Name} ({food.PreparationTime})"
                + (apparatus != null ? $" using {apparatus.Type} {apparatus.Id}" : ""));

            Thread.Sleep(food.PreparationTime * 1000);
            food.State = KitchenFoodState.Ready;
            LogsWriter.Log($"Cook {Id} prepared food {food.Name}");

            if (apparatus != null)
            {
                apparatus.Busy = false;
                LogsWriter.Log($"Cooking apparatus was unlocked, when  {food.Name} was prepared");
            }
        }

        public bool TryGetApparatus(Kitchen kitchen, Food food, out CookingApparatus apparatus)
        {
            if (!food.CookingApparatus.HasValue)
            {
                apparatus = null;
                return true;
            }

            apparatus = kitchen.CookingApparatuses
                .Where(a => a.Type == food.CookingApparatus)
                .FirstOrDefault(a => !a.Busy);

            return apparatus != null;
        }

        public void Dispose()
        {
            _cookThread.Interrupt();
        }
    }
}