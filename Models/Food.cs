using AnnaWebKitchenFin.Data.Enums;

namespace AnnaWebKitchenFin.Models
{
    public class Food
    {

        private KitchenFoodState _state;

        private object _stateLocker = new();

        public KitchenFoodState State
        {
            get
            {
                lock (_stateLocker)
                {

                    return _state;
                }
            }

            set
            {
                lock (_stateLocker)
                {
                    _state = value;
                }
            }

        }


        public long Id { get; private set; }
        public string Name { get; private set; }
        public int PreparationTime { get; private set; }
        public int Comlexity { get; private set; }
        public CookingApparatusType? CookingApparatus { get; private set; }

        public Food(long id, string name, int preparationTime, int complexity, CookingApparatusType? cookingApparatus)
        {
            Id = id;
            Name = name;
            PreparationTime = preparationTime;
            Comlexity = complexity;
            CookingApparatus = cookingApparatus;
            State = KitchenFoodState.Undone;
        }
    }
}