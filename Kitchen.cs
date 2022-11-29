using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnnaWebKitchenFin.Models;
using AnnaWebKitchenFin.Server;
using Microsoft.Extensions.Hosting;
using AnnaWebKitchenFin.Data;
using System.Threading.Tasks.Dataflow;
using AnnaWebKitchenFin.Data.Enums;
using AnnaWebKitchenFin.Utils;

namespace AnnaWebKitchenFin
{
    public class Kitchen : BackgroundService
    {
        public KitchenServer server;

        private Menu _menu = new();

        private List<Order> _orders = new();
        private List<CookingApparatus> _apparatuses = new();
        private List<Cook> _cooks = new();

        private readonly object _ordersLocker = new();
        private readonly object _cooksLocker = new();
        private readonly object _apparatusLocker = new();

        public List<Order> Orders
        {
            get
            {
                lock (_ordersLocker)
                {
                    return _orders;
                }
            }
        }

        public List<Cook> Cooks
        {
            get
            {
                lock (_cooksLocker)
                {
                    return _cooks;
                }
            }
        }

        public List<CookingApparatus> CookingApparatuses
        {
            get
            {
                lock (_apparatusLocker)
                {
                    return _apparatuses;
                }
            }
        }

        public Kitchen(KitchenServer server)
        {
            this.server = server;
            //Task task = this.server.StartAsync(this);

            server.Start(this);
            _menu.PrepareMenu();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ApparatusOn();
            StartCookingProcess();
            return Task.CompletedTask;
        }

        public void ApparatusOn()
        {
            CookingApparatus.Initialize(CookingApparatuses);
        }

        public void StartCookingProcess()
        {
            Cook.CreateCooks(Cooks, this);
        }

        public void ReceiveOrder(Order order)
        {
            foreach (long id in order.Items)
            {
                var food = _menu.Values.First(f => f.Id == id);
                order.ExistingItems.Add(food);

                LogsWriter.Log($"{food.Name} has been received and put in waiting list");
            }

            Orders.Add(order);

            Orders.Sort((former, latter) => former.Priority - latter.Priority);
            Orders.Sort((former, latter) => (int)(former.ReceivedAt.Ticks - latter.ReceivedAt.Ticks));
        }


    }
}