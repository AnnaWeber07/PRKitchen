using System;
using System.Collections.Generic;
using AnnaWebKitchenFin.Data.Enums;

namespace AnnaWebKitchenFin.Models
{
    public class Order
    {
        public long Id { get; set; }
        public List<long> Items { get; set; }
        public int Priority { get; set; }
        public float MaxWaitTime { get; set; }
        public long TableId { get; set; }
        public List<Food> ExistingItems { get; set; }
        public DateTime ReceivedAt { get; set; }

        public Order()
        {
            ExistingItems = new List<Food>();
            ReceivedAt = DateTime.Now;
        }

        //Check if orders are done
        public bool IsPrepared
        {
            get
            {
                bool isPrepared = true;
                ExistingItems.ForEach(kf =>
                {
                    if (kf.State == KitchenFoodState.Undone || kf.State == KitchenFoodState.Preparing) 
                        isPrepared = false;
                });
                return isPrepared;
            }
        }

    }
}