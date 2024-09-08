namespace MOM
{
    using System;

    public class UnitOffer
    {
        private BaseUnit _baseUnit;
        private Unit _unit;
        private int _quantity;
        private int _exp;
        private int _cost;

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

        public BaseUnit baseUnit
        {
            get
            {
                return this._baseUnit;
            }
            set
            {
                if (!ReferenceEquals(this._baseUnit, value))
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
                if (!ReferenceEquals(this._unit, value))
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
                return ((this._quantity != 0) ? this._quantity : 1);
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
    }
}

