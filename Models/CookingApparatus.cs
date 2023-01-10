using System;
using System.Runtime.Serialization;
using AnnaWebKitchenFin.Data.Enums;

namespace AnnaWebKitchenFin.Models
{
    public class CookingApparatus
    {
        private static readonly ObjectIDGenerator idGenerator = new();

        private object _busyLocker = new();
        private bool _busy;

        public long Id { get; private set; }
        public CookingApparatusType? Type;

        public bool Busy
        {
            get
            {
                lock (_busyLocker)
                {
                    return _busy;
                }
            }

            set
            {
                lock (_busyLocker)
                {
                    _busy = value;
                }
            }
        }


        public CookingApparatus(CookingApparatusType type)
        {
            Id = idGenerator.GetId(this, out _);
            Type = type;
            Busy = false;
        }
        public static void Initialize(List<CookingApparatus> apparatuses)
        {
            apparatuses.AddRange(new CookingApparatus[]
            {
                new CookingApparatus(CookingApparatusType.Oven),
                new CookingApparatus(CookingApparatusType.Oven),
                new CookingApparatus(CookingApparatusType.Stove)
            });
        }
    }

}