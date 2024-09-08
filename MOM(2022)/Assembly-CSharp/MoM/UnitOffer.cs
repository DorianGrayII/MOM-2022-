namespace MOM
{
    public class UnitOffer
    {
        private BaseUnit _baseUnit;

        private Unit _unit;

        private int _quantity;

        private int _exp;

        private int _cost;

        public BaseUnit baseUnit
        {
            get
            {
                return this._baseUnit;
            }
            set
            {
                if (this._baseUnit != value)
                {
                    this._baseUnit = value;
                }
            }
        }

        public Unit unit
        {
            get
            {
                return this._unit;
            }
            set
            {
                if (this._unit != value)
                {
                    this._unit = value;
                    this._baseUnit = value;
                }
            }
        }

        public int quantity
        {
            get
            {
                if (this._quantity == 0)
                {
                    return 1;
                }
                return this._quantity;
            }
            set
            {
                if (this._quantity != value)
                {
                    this._quantity = value;
                }
            }
        }

        public int exp
        {
            get
            {
                return this._exp;
            }
            set
            {
                if (this._exp != value)
                {
                    this._exp = value;
                }
            }
        }

        public int cost
        {
            get
            {
                return this._cost;
            }
            set
            {
                if (this._cost != value)
                {
                    this._cost = value;
                }
            }
        }

        public UnitOffer()
        {
        }

        public UnitOffer(UnitOffer uo)
        {
            this.baseUnit = uo.baseUnit;
            this.unit = uo.unit;
            this.quantity = uo.quantity;
            this.exp = uo.exp;
            this.cost = uo.cost;
        }
    }
}
